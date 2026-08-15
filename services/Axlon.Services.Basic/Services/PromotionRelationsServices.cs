using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.EventBus;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Basic.Repository.Interfaces;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Dto.Mini;
using Axlon.Services.Contracts.Promotion.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog.Core;

namespace Axlon.Services.Basic.Services
{
    public class PromotionRelationsServices : BaseServices<PromotionRelations>, IPromotionRelationsServices
    {
        private IUser loginUser { get; set; }
        private IUnitOfWorkManage unitOfWorkManage;
        private readonly IPromotionRelationsRepository promotionRelationsRepository;
        private readonly ISysUserInfoServices sysUserInfoServices;
        private readonly IAxlonEventPublisher eventPublisher;
        private readonly ILogger<PromotionRelationsServices> logger;


        public PromotionRelationsServices(IPromotionRelationsRepository promotionRelationsRepository, IUser loginUser, IUnitOfWorkManage unitOfWorkManage, ISysUserInfoServices sysUserInfoServices, IAxlonEventPublisher eventPublisher, ILogger<PromotionRelationsServices> logger) : base(promotionRelationsRepository)
        {
            this.loginUser = loginUser;
            this.promotionRelationsRepository = promotionRelationsRepository;
            this.unitOfWorkManage = unitOfWorkManage;
            this.sysUserInfoServices = sysUserInfoServices;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task<GetMyPromotionInfoRes> GetMyPromotionInfoAsync()
        {
            var res = new GetMyPromotionInfoRes();

            var user = await base.Db.Queryable<SysUserInfo>().Where(x => x.Id == loginUser.ID).FirstAsync();

            // 直推人数
            res.DirectPromotion = await base.Db.Queryable<PromotionRelations>()
                .Where(x => x.ParentId == loginUser.ID && x.IsValid == PromotionRelationsIsValid.有效)
                .CountAsync();

            // 关系数据
            var promotionRelations = await base.Db.Queryable<PromotionRelations>()
                .ToChildListAsync(x => x.ParentId, loginUser.ID);

            // 五级分佣，是指一个人的下级最多5个？
            res.PromotionTeam = promotionRelations.Count();

            // 今日新增
            res.TodayPromotion = promotionRelations.Where(x => x.BindTime.Date == DateTime.Now.Date).Count();

            res.TotalPromotion = res.PromotionTeam;
            res.PrompterLevel = user.PrompterLevel.ToString();

            var levelRules = await base.Db.Queryable<PromotionLevelRules>().ToListAsync();

            //var thisLevelRule = levelRules.Where(x => x.LevelCode == user.PrompterLevel).FirstOrDefault();

            var nextLevelRule = levelRules.Where(x => x.LevelCode == user.PrompterLevel + 1).FirstOrDefault();

            // 直推人数进度
            res.DirectPromotionPlan = nextLevelRule == null ? 100.0 : (double)res.DirectPromotion / nextLevelRule.RequireDirectCount;

            // 下一等级
            res.NextLevel = nextLevelRule.LevelCode.ToString();

            return res;           
        }

        public async Task<(bool,string)> BindPromotionRelationAsync(BindPromotionRelationReq req)
        {
            var user = await sysUserInfoServices.QueryById(req.UserId);
            if (req.UserId == req.ParentId)
                return (false, "自己不能推广自己！");

            if (req.ParentId == null)
            {
                return (true, "无推广人信息");
            }


            // 是否已有一级推广关系
            var exists = (await Query(x =>
                x.UserId == req.UserId &&
                x.Level == 1 &&
                x.IsValid == PromotionRelationsIsValid.有效))
                .Any();

            if (exists)
            {
                unitOfWorkManage.RollbackTran();
                return (false, "已存在推广关系！");
            }

            var relations = new List<PromotionRelations>();

            // 一级关系
            relations.Add(PromotionRelations.Create(
                    req.UserId,
                    req.ParentId.Value,
                    req.BindOrderId,
                    1
            ));

            // 查询推广人的上级链
            var ancestors = await promotionRelationsRepository.ByParentIdGetParent(req.ParentId.Value);

            //var ancestors = await base.Query(x => x.UserId == req.ParentId && x.Level < 5 && x.IsValid == PromotionRelationsIsValid.有效);


            // 插入 2-5 级关系，查询A的父级，建立与A的父级推广关系
            // 比如说A -> B -> C -> D
            // 假设我们是 D
            // 一级：A是B的一级（Level 1）,B是C的一级（Level 1）,C是D的一级
            // 二级：A是C的二级（Level 2），B是D的二级（Level 2）
            // 三级：A是D的三级（Level 3）
            // 目前只建立了 D与C的一级关系，查询C的所有父级，建立关系
            // 如果父级的Level为5则不再建立，如果再建立Level就超过5了（业务只允许5级推广）
            // 与每个父级建立联系，ParentId 是 UserId 的 第 Level 层推广人

            foreach (var item in ancestors)
            {
                var level = item.Level + 1;

                if (level > 5)
                    continue;


                relations.Add(PromotionRelations.Create(
                        req.UserId,
                        item.ParentId,
                        req.BindOrderId,
                        (byte)level)
                );
            }

            await base.Add(relations);
            return (true,"");
        }

        public async Task<bool> BindPromotionRelationAsync(OrderCompletedIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            #region
            try
            {
                unitOfWorkManage.BeginTran();

                // 是否完成首单（修改用户首次消费状态）
                var firstOrderCompleted = (await sysUserInfoServices.UpdateAsync(x => x.Id == @event.userId && x.FirstOrderCompleted == YesNo.否, x => new SysUserInfo
                {
                    FirstOrderCompleted = YesNo.是
                })) >= 0 ? false : true;

                var user = await sysUserInfoServices.QueryById(@event.userId);
                var hasPromotionUser = user.RegisterFromUserId != null && user.RegisterFromUserId > 0;

                // 如果未完成首单，并且具有上级推广人
                if (!firstOrderCompleted && hasPromotionUser) 
                {
                    var relations = new List<PromotionRelations>();

                    // 一级关系
                    relations.Add(PromotionRelations.Create(
                            user.Id,
                            user.RegisterFromUserId.Value,
                            @event.orderId,
                            1
                    ));

                    // 查询推广人的上级链
                    var ancestors = await promotionRelationsRepository.ByParentIdGetParent(user.RegisterFromUserId.Value);

                    // 插入 2-5 级关系，查询A的父级，建立与A的父级推广关系
                    // 比如说A -> B -> C -> D
                    // 假设我们是 D
                    // 一级：A是B的一级（Level 1）,B是C的一级（Level 1）,C是D的一级
                    // 二级：A是C的二级（Level 2），B是D的二级（Level 2）
                    // 三级：A是D的三级（Level 3）
                    // 目前只建立了 D与C的一级关系，查询C的所有父级，建立关系
                    // 如果父级的Level为5则不再建立，如果再建立Level就超过5了（业务只允许5级推广）
                    // 与每个父级建立联系，ParentId 是 UserId 的 第 Level 层推广人

                    foreach (var item in ancestors)
                    {
                        var level = item.Level + 1;

                        if (level > 5)
                            continue;


                        relations.Add(PromotionRelations.Create(
                                user.Id,
                                item.ParentId,
                                @event.orderId,
                                (byte)level)
                        );
                    }

                    await base.Add(relations);
                }

                unitOfWorkManage.CommitTran();

                // 发布佣金记录创建事件
                await TryPublishPromotionEarningCalculateAsync(@event.userId, @event.orderId);
            }
            catch (Exception ex) 
            {
                unitOfWorkManage.RollbackTran();
            }

            #endregion
            throw new NotImplementedException();
        }

        public async Task<List<PromotionRelationsBasicDto>> ByUserIdGetBasicInfoAsync(long userId)
        {
            var data = await base.Query(expression:x => new PromotionRelationsBasicDto 
            {
                Id = x.Id,
                UserId = x.UserId,
                ParentId = x.ParentId,
                BindOrderId = x.BindOrderId,
                Level = x.Level
            },
            whereExpression: x => x.UserId == userId, "level");

            return data;
        }
        /// <summary>
        /// 发布计算佣金事件
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        private async Task TryPublishPromotionEarningCalculateAsync(long userId,long orderId)
        {
            try
            {
                await eventPublisher.PublishAsync(
                    PromotionEarningTopics.CalculateV1,
                    new PromotionEarningCalculateIntegrationEvent(userId, orderId));
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception,
                    "发布佣金记录创建失败，UserId={UserId}, OrderId={MerchantId}", userId, orderId);
            }
        }
        //PromotionEarning
    }
}
