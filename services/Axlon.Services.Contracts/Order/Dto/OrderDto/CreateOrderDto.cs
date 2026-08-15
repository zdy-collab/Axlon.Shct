using Axlon.Services.Contracts.Order.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    public class CreateOrderReq
    {
        /// <summary>
        /// 商家Id
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 桌台Id
        /// </summary>
        public long? TableId { get; set; }

        /// <summary>
        /// 推广人Id
        /// </summary>  
        //public long? PromoUserId { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType Type { get; set; }

        public List<CreateOrderItemReq> orderItems { get; set; }
    }

    public class CreateOrderRes 
    {
        public long OrderId { get; set; }
    }
}
