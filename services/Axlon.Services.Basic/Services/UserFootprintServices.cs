using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.EventBus;
using Axlon.Framework.Redis;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Enums;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.User;
using SqlSugar;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// 用户足迹
    /// </summary>
    public sealed class UserFootprintServices :
        BaseServicesExtend<UserFootprintAddInput, UserFootprintEditInput, UserFootprintOutput, UserFootprint>, IUserFootprintServices
    {
        private readonly IUser _user;
        private readonly IAxlonIdGenerator _ids;
        private readonly IAxlonTransactionalEventExecutor _events;
        private readonly IAxlonEventPublisher _publisher;
        private readonly IAxlonRedisLocks _locks;
        private readonly IBaseRepository<UserFootprintEventReceipt> _receiptRepository;

        /// <summary>
        /// 可用的页面
        /// </summary>
        private static readonly HashSet<string> AllowedPageCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "home", "merchant_list", "merchant_detail", "merchant_group_buys",
            "orders", "wallet", "promotion_center", "platform_content"
        };

        public UserFootprintServices(
            IBaseRepository<UserFootprint> repository,
            IBaseRepository<UserFootprintEventReceipt> receiptRepository,
            IUser user,
            IAxlonIdGenerator ids,
            IAxlonTransactionalEventExecutor events,
            IAxlonEventPublisher publisher,
            IAxlonRedisLocks locks) : base(repository, user)
        {
            _receiptRepository = receiptRepository;
            _user = user;
            _ids = ids;
            _events = events;
            _publisher = publisher;
            _locks = locks;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public async Task<PageResponseModel<UserFootprintOutput>> GetPageAsync(UserFootprintPageInput pageRequest)
        {
            var userId = RequireUserId();
            var targetType = NormalizeTargetType(pageRequest.TargetType, allowEmpty: true);
            var pageIndex = Math.Max(1, pageRequest.PageIndex);
            var pageSize = Math.Clamp(pageRequest.PageSize, 1, 100);
            var page = string.IsNullOrEmpty(targetType)
                ? await CurrentRepository.QueryPage(item => item.UserId == userId, pageIndex, pageSize, "modify_time desc")
                : await CurrentRepository.QueryPage(item => item.UserId == userId && item.TargetType == targetType,
                    pageIndex, pageSize, "modify_time desc");
            return page.ConvertTo<UserFootprintOutput>();
        }

        public async Task<bool> PublishPageViewAsync(UserPageViewInput input, CancellationToken cancellationToken = default)
        {
            if (_user.ID <= 0 || _user.GetIsVisitor()) return false;
            var pageCode = NormalizePageCode(input.PageCode);
            await _publisher.PublishAsync(UserFootprintTopics.ViewedV1,
                new UserFootprintViewedIntegrationEvent(_user.ID, UserFootprintTargetTypes.Page, PageCode: pageCode),
                cancellationToken);
            return true;
        }

        public async Task<bool> RecordViewAsync(
            UserFootprintViewedIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.UserId <= 0) throw new ArgumentOutOfRangeException(nameof(@event.UserId));

            var targetType = NormalizeTargetType(@event.TargetType, allowEmpty: false)!;
            var pageCode = targetType == UserFootprintTargetTypes.Page
                ? NormalizePageCode(@event.PageCode)
                : null;
            if (targetType != UserFootprintTargetTypes.Page && @event.TargetId is not > 0)
                throw new ArgumentException("业务足迹必须提供有效的 TargetId。", nameof(@event));

            var targetKey = targetType == UserFootprintTargetTypes.Page
                ? $"page:{pageCode}"
                : $"{targetType}:{@event.TargetId}";
            var merchantId = targetType == UserFootprintTargetTypes.Merchant
                ? @event.TargetId
                : @event.MerchantId;
            var occurredAt = @event.OccurredAt.UtcDateTime;

            await using var lease = await _locks.AcquireAsync(
                $"user-data:footprints:event:{@event.EventId:N}", cancellationToken: cancellationToken);

            return await _events.ExecuteAsync(async (_, token) =>
            {
                var eventId = @event.EventId.ToString("D");
                if (await _receiptRepository.Any(item => item.EventId == eventId)) return false;

                await _receiptRepository.AddReturnSnowIdAsync(new UserFootprintEventReceipt
                {
                    EventId = eventId,
                    UserId = @event.UserId,
                    ProcessedAt = DateTime.UtcNow
                });

                const string sql = """
                    INSERT INTO user_footprints
                        (id, user_id, merchant_id, target_type, target_id, target_key, page_code,
                         target_title, target_image, footprint_type, order_id, occurrence_count,
                         create_time, modify_time, is_deleted)
                    VALUES
                        (@id, @user_id, @merchant_id, @target_type, @target_id, @target_key, @page_code,
                         @target_title, @target_image, @footprint_type, NULL, 1,
                         @occurred_at, @occurred_at, 0)
                    ON DUPLICATE KEY UPDATE
                        occurrence_count = occurrence_count + 1,
                        merchant_id = COALESCE(VALUES(merchant_id), merchant_id),
                        target_title = COALESCE(VALUES(target_title), target_title),
                        target_image = COALESCE(VALUES(target_image), target_image),
                        modify_time = GREATEST(COALESCE(modify_time, VALUES(modify_time)), VALUES(modify_time)),
                        is_deleted = 0;
                    """;
                await CurrentRepository.Db.Ado.ExecuteCommandAsync(sql,
                    new SugarParameter("@id", _ids.NextId()),
                    new SugarParameter("@user_id", @event.UserId),
                    new SugarParameter("@merchant_id", merchantId),
                    new SugarParameter("@target_type", targetType),
                    new SugarParameter("@target_id", @event.TargetId),
                    new SugarParameter("@target_key", targetKey),
                    new SugarParameter("@page_code", pageCode),
                    new SugarParameter("@target_title", TrimTo(@event.TargetTitle, 200)),
                    new SugarParameter("@target_image", TrimTo(@event.TargetImage, 500)),
                    new SugarParameter("@footprint_type", (int)FootprintTypeEnum.View),
                    new SugarParameter("@occurred_at", occurredAt));
                return true;
            }, cancellationToken);
        }

        public async Task<bool> RemoveMineAsync(long id)
        {
            var row = await CurrentRepository.First(item => item.Id == id && item.UserId == RequireUserId());
            return row is not null && await CurrentRepository.Delete(row);
        }

        public async Task<int> ClearMineAsync(string? targetType)
        {
            var userId = RequireUserId();
            var normalized = NormalizeTargetType(targetType, allowEmpty: true);
            var rows = string.IsNullOrEmpty(normalized)
                ? await CurrentRepository.Query(item => item.UserId == userId)
                : await CurrentRepository.Query(item => item.UserId == userId && item.TargetType == normalized);
            var count = 0;
            foreach (var row in rows)
                if (await CurrentRepository.Delete(row)) count++;
            return count;
        }

        private long RequireUserId() => _user.ID > 0 && !_user.GetIsVisitor()
            ? _user.ID
            : throw new UnauthorizedAccessException("仅正式登录用户可以访问足迹。");

        private static string? NormalizeTargetType(string? value, bool allowEmpty)
        {
            if (string.IsNullOrWhiteSpace(value))
                return allowEmpty ? null : throw new ArgumentException("TargetType 不能为空。", nameof(value));
            var normalized = value.Trim().ToLowerInvariant();
            return UserFootprintTargetTypes.IsValid(normalized)
                ? normalized
                : throw new ArgumentException("不支持的足迹目标类型。", nameof(value));
        }

        private static string NormalizePageCode(string? value)
        {
            var normalized = value?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalized) || !AllowedPageCodes.Contains(normalized))
                throw new ArgumentException("不支持的 PageCode。", nameof(value));
            return normalized;
        }

        private static string? TrimTo(string? value, int maxLength) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim()[..Math.Min(value.Trim().Length, maxLength)];
    }
}
