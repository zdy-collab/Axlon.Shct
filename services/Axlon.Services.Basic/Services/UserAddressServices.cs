using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.Redis;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.User;
using Mapster;

namespace Axlon.Services.Basic.Services
{
    public sealed class UserAddressServices :
        BaseServicesExtend<UserAddressAddInput, UserAddressEditInput, UserAddressOutput, UserAddress>, IUserAddressServices
    {
        private readonly IUser _user;
        private readonly IUnitOfWorkManage _unitOfWork;
        private readonly IAxlonRedisLocks _locks;
        private readonly IAxlonRedisStrings _axlonRedis;

        public UserAddressServices(
            IBaseRepository<UserAddress> repository,
            IUser user,
            IUnitOfWorkManage unitOfWork,
            IAxlonRedisStrings axlonRedis,
            IAxlonRedisLocks locks) : base(repository, user)
        {
            _user = user;
            _unitOfWork = unitOfWork;
            _locks = locks;
            _axlonRedis = axlonRedis;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="addInput"></param>
        /// <returns></returns>
        public override async Task<long> AddAsync(UserAddressAddInput addInput)
        {
            ArgumentNullException.ThrowIfNull(addInput);
            var userId = RequireUserId();
            await using var lease = await _locks.AcquireAsync($"user-data:addresses:default:{userId}");
            _unitOfWork.BeginTran();
            long id;
            try
            {
                var existing = await CurrentRepository.Query(item => item.UserId == userId);
                var makeDefault = existing.Count == 0 || addInput.IsDefault;
                if (makeDefault && existing.Count > 0)
                {
                    var defaults = existing.Where(item => item.IsDefault).ToList();
                    foreach (var item in defaults)
                    {
                        item.IsDefault = false;
                    }
                    if (defaults.Count > 0) await CurrentRepository.Update(defaults);
                }

                var now = DateTime.UtcNow;
                var entity = addInput.Adapt<UserAddress>();
                entity.UserId = userId;
                entity.IsDefault = makeDefault;
                id = await CurrentRepository.AddReturnSnowIdAsync(entity);
                _unitOfWork.CommitTran();
            }
            catch
            {
                _unitOfWork.RollbackTran();
                throw;
            }

            return id;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="editInput"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserAddressEditInput editInput)
        {
            ArgumentNullException.ThrowIfNull(editInput);
            var userId = RequireUserId();
            await using var lease = await _locks.AcquireAsync($"user-data:addresses:default:{userId}");
            _unitOfWork.BeginTran();
            bool updated;
            try
            {
                var entity = await CurrentRepository.First(item => item.Id == editInput.Id && item.UserId == userId);
                if (entity is null)
                {
                    _unitOfWork.RollbackTran();
                    return false;
                }

                var wasDefault = entity.IsDefault;
                if (editInput.IsDefault && !wasDefault)
                    await ClearDefaultAsync(userId);

                editInput.Adapt(entity);
                if (wasDefault && !editInput.IsDefault) entity.IsDefault = true;
                entity.UserId = userId;
                updated = await CurrentRepository.Update(entity);
                _unitOfWork.CommitTran();
            }
            catch
            {
                _unitOfWork.RollbackTran();
                throw;
            }

            return updated;
        }

        /// <summary>
        /// 设置默认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<bool> SetDefaultAsync(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            var userId = RequireUserId();
            await using var lease = await _locks.AcquireAsync($"user-data:addresses:default:{userId}");
            _unitOfWork.BeginTran();
            try
            {
                var target = await CurrentRepository.First(item => item.Id == id && item.UserId == userId);
                if (target is null)
                {
                    _unitOfWork.RollbackTran();
                    return false;
                }
                if (!target.IsDefault)
                {
                    await ClearDefaultAsync(userId);
                    target.IsDefault = true;
                    await CurrentRepository.Update(target);
                }
                _unitOfWork.CommitTran();
            }
            catch
            {
                _unitOfWork.RollbackTran();
                throw;
            }

            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(long id)
        {
            var userId = RequireUserId();
            await using var lease = await _locks.AcquireAsync($"user-data:addresses:default:{userId}");
            _unitOfWork.BeginTran();
            bool deleted;
            try
            {
                var entity = await CurrentRepository.First(item => item.Id == id && item.UserId == userId);
                if (entity is null)
                {
                    _unitOfWork.RollbackTran();
                    return false;
                }
                deleted = await CurrentRepository.Delete(entity);
                if (deleted && entity.IsDefault)
                {
                    var remaining = await CurrentRepository.Query(item => item.UserId == userId, 1, "UpdatedAt desc");
                    var replacement = remaining.FirstOrDefault();
                    if (replacement is not null)
                    {
                        replacement.IsDefault = true;
                        await CurrentRepository.Update(replacement);
                    }
                }
                _unitOfWork.CommitTran();
            }
            catch
            {
                _unitOfWork.RollbackTran();
                throw;
            }

            return deleted;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public async Task<PageResponseModel<UserAddressOutput>> GetPageAsync(UserAddressPageInput pageRequest)
        {
            await _axlonRedis.StringSetAsync("key", "test");
            return await base.GetPageAsync(item => item.UserId == RequireUserId(), pageRequest);
        }

        #region private


        /// <summary>
        /// 移除默认
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task ClearDefaultAsync(long userId)
        {
            var current = await CurrentRepository.Query(item => item.UserId == userId && item.IsDefault);
            foreach (var item in current)
            {
                item.IsDefault = false;
            }
            if (current.Count > 0) await CurrentRepository.Update(current);
        }

        private long RequireUserId() => _user.ID > 0
            ? _user.ID
            : throw new UnauthorizedAccessException("An authenticated user is required.");


        #endregion

    }

}
