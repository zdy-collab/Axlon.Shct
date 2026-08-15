using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Order.Dto.PaymentDto;

namespace Axlon.Services.Order.Services.Interfaces
{
    public interface IPaymentServices: IScopedDependency
    {
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <returns></returns>
        Task<bool> PayOrderAsync(PayOrderReq req);
    }
}
