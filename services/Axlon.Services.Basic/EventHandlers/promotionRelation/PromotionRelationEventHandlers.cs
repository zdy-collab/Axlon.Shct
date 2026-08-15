using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Events;
using DotNetCore.CAP;

namespace Axlon.Services.Basic.EventHandlers.promotionRelation
{
    public class PromotionRelationEventHandler(IPromotionRelationsServices promotionRelationsServices) : ICapSubscribe
    {

        /// <summary>
        /// 监听订单完成
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [CapSubscribe(OrderTopics.CompletedV1)]
        public async Task HandleAsync(
        OrderCompletedIntegrationEvent @event, CancellationToken cancellationToken = default) =>
    _ = await promotionRelationsServices.BindPromotionRelationAsync(@event, cancellationToken);
    }
}
