using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    public class CreateOrderItemReq
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
