using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    public class CreateOrderItemCommand
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 菜品图片快照Id
        /// </summary>
        public long ProductImageFileId { get; set; }

        /// <summary>
        /// 菜品图片Oss快照
        /// </summary>
        public string ProductImageOss { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 菜品单价（快照）
        /// 下单时的价格，后续调价不影响已生成的订单
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小计金额 = ProductPrice * Quantity
        /// 可直接存储，避免重复计算和精度误差
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 备注信息
        /// 如：少辣、不要香菜、加冰等顾客定制需求
        /// </summary>
        public string Remarks { get; set; }
    }
}
