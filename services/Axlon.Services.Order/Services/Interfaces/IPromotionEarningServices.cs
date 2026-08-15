using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Promotion;

namespace Axlon.Services.Order.Services.Interfaces
{
    /// <summary>
    /// 分佣记录
    /// </summary>
    public interface IPromotionEarningServices:IBaseServices<PromotionEarnings>
    {
        /// <summary>
        /// 创建佣金记录
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> PromotionEarningCreateAsync(PromotionEarningCalculateIntegrationEvent @event,CancellationToken cancellationToken = default);
    }
}
