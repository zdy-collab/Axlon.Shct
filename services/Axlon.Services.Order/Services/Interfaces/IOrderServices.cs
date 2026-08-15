using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.Order.Dto.OrderDto;

namespace Axlon.Services.Order.Services.Interfaces
{
    public interface IOrderServices : IBaseServices<Orders>
    {
        /// <summary>
        /// 获取我的订单
        /// </summary>
        /// <returns></returns>
        Task<PageResponseModel<GetMyOrdersRes>> GetMyOrdersAsync(GetMyOrdersReq req);

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<GetMyOderDetailRes> GetMyOderDetailsAsync(long orderId);


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        Task<CreateOrderRes> CreateOrderAsync(CreateOrderReq req);

        /// <summary>
        /// 订单完成
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> OrderCompletedAsync(long orderId);
    }
}
