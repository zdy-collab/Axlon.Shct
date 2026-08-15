using Axlon.Services.Contracts.Order.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    public class CreateOrderCommand
    {
        public long UserId { get; set; }
        public long MerchantId { get; set; }
        public long? TableId { get; set; }
        public long? PromoUserId { get; set; }
        public OrderSource Source { get; set; }
        public OrderType Type { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalAmount { get; set; }
        //public decimal PaidAmount { get; set; }

        public List<CreateOrderItemCommand> orderItems { get; set; }
    }
}
