using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    public interface IPromotionCommissionRuleClient:IScopedDependency
    {
        /// <summary>
        /// 根据商家Id获取分佣规则，如果没有则返回全局分佣配置
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        Task<MessageModel<PromotionCommissionRulesBasicDto>> ByMerchantIdGetPCRuleAsync(long merchantId);
    }
}
