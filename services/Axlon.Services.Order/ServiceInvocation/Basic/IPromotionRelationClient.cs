using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    
    /// <summary>
    /// 推广关系服务
    /// </summary>
    public interface IPromotionRelationClient: IScopedDependency
    {
        /// <summary>
        /// 绑定推广关系
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<MessageModel<string>> BindPromotionRelationAsync([FromBody] BindPromotionRelationReq req);

        /// <summary>
        /// 根据用户Id获取推广关系基础信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<MessageModel<List<PromotionRelationsBasicDto>>> ByUserIdGetBasicInfoAsync(long userId);
    }
}
