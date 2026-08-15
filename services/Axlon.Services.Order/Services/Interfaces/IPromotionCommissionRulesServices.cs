using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;

namespace Axlon.Services.Order.Services.Interfaces
{
    /// <summary>
    /// 分佣规则
    /// </summary>
    public interface IPromotionCommissionRulesServices : IBaseServices<PromotionCommissionRules>
    {
        /// <summary>
        /// 分佣
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<bool> ReceiveCommissionAsync(long orderId);
    }
}
