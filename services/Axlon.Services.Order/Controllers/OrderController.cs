using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Order.Dto.OrderDto;
using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Order.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Order.Controllers
{

    /// <summary>
    /// 订单管理
    /// </summary>
    [Route("api/order/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderServices orderServices;

        public OrderController(IOrderServices orderServices)
        {
            this.orderServices = orderServices;
        }

        /// <summary>
        /// 获取我的订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("getMyOrders")]
        public async Task<MessageModel<PageResponseModel<GetMyOrdersRes>>> GetMyOrdersAsync([FromQuery] GetMyOrdersReq req)
        {
            var data = await orderServices.GetMyOrdersAsync(req);
            return Success(data: data);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("getMyOderDetails")]
        public async Task<MessageModel<GetMyOderDetailRes>> GetMyOderDetailsAsync([FromQuery] long orderId)
        {
            return Success(data: await orderServices.GetMyOderDetailsAsync(orderId));
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("createOrder")]
        public async Task<MessageModel<CreateOrderRes>> CreateOrderAsync([FromBody] CreateOrderReq req)
        {
            #region 参数校验

            // 堂食订单
            if (req.Type == OrderType.dine_in)
            {
                if (req.TableId == null || req.TableId <= 0) return Failed<CreateOrderRes>("堂食订单未选择桌台！");
            }

            #endregion

            return Success(data: await orderServices.CreateOrderAsync(req));
        }
    }
}
