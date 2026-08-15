using Axlon.Services.Contracts.Events;
using Axlon.Services.Order.Services.Interfaces;
using DotNetCore.CAP;

namespace Axlon.Services.Order.PromotionEarningData
{
    public sealed class PromotionEarningHandlers(IPromotionEarningServices services) : ICapSubscribe
    {
        
        /// <summary>
        /// 监听计算佣金
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [CapSubscribe(PromotionEarningTopics.CalculateV1)]
        public async Task HandleAsync(
            PromotionEarningCalculateIntegrationEvent @event, CancellationToken cancellationToken = default) =>
            _ = await services.PromotionEarningCreateAsync(@event, cancellationToken);
    }
}
