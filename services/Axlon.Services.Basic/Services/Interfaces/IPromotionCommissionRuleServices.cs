using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Content;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;

namespace Axlon.Services.Basic.Services.Interfaces
{
    /// <summary>
    /// 分佣比例配置
    /// </summary>
    public interface IPromotionCommissionRuleServices: IBaseServices<PromotionCommissionRules>
    {
        /// <summary>
        /// 根据商家Id获取分佣规则，如果没有则返回全局分佣配置
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public Task<PromotionCommissionRulesBasicDto> ByMerchantIdGetPCRuleAsync(long merchantId);
    }
}
