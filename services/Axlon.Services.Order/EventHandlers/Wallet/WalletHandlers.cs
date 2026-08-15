using Axlon.Services.Contracts.Events;
using Axlon.Services.Order.Services.Interfaces;
using DotNetCore.CAP;

namespace Axlon.Services.Order.EventHandlers.Wallet
{

    public sealed class WalletHandlers(IWalletServices services) : ICapSubscribe
    {

        /// <summary>
        /// 监听佣金计算完成
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [CapSubscribe(PromotionEarningTopics.CalculatedV1)]
        public async Task HandleAsync(
            PromotionEarningCalculatedIntegrationEvent @event, CancellationToken cancellationToken = default) =>
            _ = await services.ByPromotionEarningAddIncomeAsync(@event, cancellationToken);
    }
}
