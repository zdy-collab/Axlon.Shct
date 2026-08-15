using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Dto.Mini;

namespace Axlon.Services.Basic.IServices
{
    /// <summary>
    /// 推广关系
    /// </summary>
    public interface IPromotionRelationsServices : IBaseServices<PromotionRelations>
    {
        /// <summary>
        /// 获取我的推广信息
        /// </summary>
        /// <returns></returns>
        Task<GetMyPromotionInfoRes> GetMyPromotionInfoAsync();

        /// <summary>
        /// 绑定推广关系
        /// </summary>
        /// <returns></returns>
        Task<(bool,string)> BindPromotionRelationAsync(BindPromotionRelationReq req);

        /// <summary>
        /// 绑定推广关系
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BindPromotionRelationAsync(OrderCompletedIntegrationEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据用户Id获取推广关系基础信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<PromotionRelationsBasicDto>> ByUserIdGetBasicInfoAsync(long userId);
    }
}
