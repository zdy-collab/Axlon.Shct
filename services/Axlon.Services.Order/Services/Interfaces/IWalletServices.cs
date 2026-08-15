using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.Order.Dto.WalletDto;

namespace Axlon.Services.Order.Services.Interfaces
{
    /// <summary>
    /// 小程序-钱包服务
    /// </summary>
    public interface IWalletServices : IBaseServices<UserWallets>
    {
        /// <summary>
        /// 获取我的钱包信息
        /// </summary>
        /// <returns></returns>
        Task<GetMyWalletInfoRes> GetMyWalletInfoAsync();

        /// <summary>
        /// 获取我的收益明细
        /// </summary>
        /// <returns></returns>
        Task<RevenueDto> GetMyRevenueDetailsAsync(QueryPage queryPage);

        /// <summary>
        /// 根据佣金记录新增收益
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> ByPromotionEarningAddIncomeAsync(PromotionEarningCalculatedIntegrationEvent @event,CancellationToken cancellationToken = default);
    }
}
