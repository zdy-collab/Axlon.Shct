using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Order.Enums;
using Mapster.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    #region req

    public class GetMyOrdersReq
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; } = 1;

        /// <summary>
        /// 每页数据
        /// </summary>
        public int pageSize { get; set; } = 10;

        /// <summary>
        /// 订单状态
        /// </summary>
        public QueryOrderStatus? status { get; set; }
    }

    public enum QueryOrderStatus
    {
        全部,
        待付款,
        待使用,
        进行中,
        已完成,
        已取消,
        退款中
    }

    #endregion

    #region res

    public class GetMyOrdersRes
    {
        public long Id { get; set; }

        /// <summary>
        /// 商家Id
        /// </summary>
        public long MerchantId { get; set; }

        public string Status { get; set; }

        public string StatusDescription
        {
            get
            {
                var state = Enum.TryParse<OrderStatus>(Status, true, out OrderStatus result);
                return result.GetDescription();
            }
        }

        /// <summary>
        /// 订单类型：dine_in(堂食) / takeout(外带) / delivery(配送) / group_buy(团购)
        /// OrderType
        /// </summary>
        public string Type { get; set; }

        public string TypeDescription
        {
            get
            {
                var state = Enum.TryParse<OrderType>(Type, true, out OrderType result);
                return result.GetDescription();
            }
        }

        /// <summary>
        /// 订单状态（核心状态机）
        /// pending_pay: 待支付 | paid: 已支付 | completed: 已完成（核销/取餐）
        /// refunding: 退款中 | refunded: 已退款 | cancelled: 已取消
        /// OrderStatus
        /// </summary>
        //public string Status { 
        //    get 
        //    {
        //        return status.GetDescription();
        //    } 
        //    set 
        //    {
        //        status = System.Enum.Parse<OrderStatus>(value);
        //    } 
        //}

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 实付金额，用户实际支付金额
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// 订单明细
        /// </summary>
        public List<OrderItemsInfoDto> orderItems { get; set; }

        #region enum

        ///// <summary>
        ///// 订单来源：小程序,桌码,推广码
        ///// </summary>
        //public OrderSource Source { get; set; }

        ///// <summary>
        ///// 支付状态：unpaid(未支付) / paid(已支付) / refunding(退款中) / refunded(已退款)
        ///// </summary>
        //public OrderPayStatus PayStatus { get; set; }

        #endregion

        ///// <summary>
        ///// 订单号
        ///// </summary>
        //public string OrderNo { get; set; }



        ///// <summary>
        ///// 原价总金额（优惠前），所有商品原价之和
        ///// </summary>
        //public decimal TotalAmount { get; set; }

        ///// <summary>
        ///// 优惠总金额 = 原价总金额 - 实付金额
        ///// </summary>
        //public decimal DiscountAmount { get; set; }

        ///// <summary>
        ///// 已退款金额（支持部分退款场景）
        ///// </summary>
        //public decimal RefundAmount { get; set; }

        ///// <summary>
        ///// 支付时间
        ///// </summary>
        //public DateTime? PayTime { get; set; }

        ///// <summary>
        ///// 订单完成时间（核销/取餐/送达）
        ///// </summary>
        //public DateTime? CompleteTime { get; set; }

        ///// <summary>
        ///// 取消原因
        ///// </summary>
        //public string CancelReason { get; set; }

        ///// <summary>
        ///// 退款原因
        ///// </summary>
        //public string RefundReason { get; set; }

        ///// <summary>
        ///// 退款完成时间
        ///// </summary>
        //public DateTime? RefundTime { get; set; }
    }

    #endregion

    #region dto

    public class OrderItemsInfoDto
    {
        /// <summary>
        /// 订单ID，关联 orders.id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 菜品ID，关联 products.id
        /// 仅用于后台统计和溯源，前端展示使用快照字段
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 菜品名称（快照）
        /// 下单时从 products 表复制，菜品改名后此值保持不变
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

    #endregion
    public class OrderInfoDto
    {
    }
}
