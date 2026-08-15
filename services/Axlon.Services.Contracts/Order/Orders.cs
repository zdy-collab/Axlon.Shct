using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.Models.Enums;
using Axlon.Services.Contracts.Order.Dto.OrderDto;
using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Order.RootTkey;
using Mapster;
using SqlSugar;
using StackExchange.Redis;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 订单主表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("orders", "订单主表")]
    public class Orders : OrdersRoot<long>
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        public static Orders Create(CreateOrderCommand orderCommand)
        {
            var order = new Orders
            {
                Source = orderCommand.Source.ToString(),
                Type = orderCommand.Type.ToString(),
                Status = OrderStatus.PendingPayment.ToString(),
                PayStatus = OrderPayStatus.unpaid.ToString(),
                OrderNo = orderCommand.OrderNo,
                TotalAmount = orderCommand.TotalAmount,
                //DiscountAmount = orderCommand.TotalAmount - orderCommand.PaidAmount,
                DiscountAmount = 0,
                //PaidAmount = orderCommand.PaidAmount,
                PaidAmount = orderCommand.TotalAmount,
                UserId = orderCommand.UserId,
                MerchantId = orderCommand.MerchantId
            };
            order.DiscountAmount = order.TotalAmount - order.PaidAmount;

            if (orderCommand.TableId != null) order.TableId = orderCommand.TableId.Value;
            if (orderCommand.PromoUserId != null) order.PromoUserId = orderCommand.PromoUserId.Value;

            order.orderItems = orderCommand.orderItems.Adapt<List<OrderItems>>();

            return order;
        }

        /// <summary>
        /// 成功支付
        /// </summary>
        public void PaidSuccess() 
        {
            Status = OrderStatus.Preparing.ToString(); // 目前直接 商家备餐中，后续需要商家确认
            PayStatus = OrderPayStatus.paid.ToString();
            PayTime = DateTime.Now;
        }

        public void Completed() 
        {
            Status = OrderStatus.Completed.ToString();
            CompleteTime = DateTime.Now;
        }

        #region enum

        /// <summary>
        /// 订单来源：小程序,桌码,推广码
        /// OrderSource
        /// </summary>
        [SugarColumn(ColumnName = "source", ColumnDataType = "varchar(50)", IsNullable = false)]
        public string Source { get; set; }

        /// <summary>
        /// 订单类型：dine_in(堂食) / takeout(自提) / delivery(配送) / group_buy(团购)
        /// OrderType
        /// </summary>
        [SugarColumn(ColumnName = "type", ColumnDataType = "varchar(20)", IsNullable = false)]
        public string Type { get; set; }

        /// <summary>
        /// 订单状态（核心状态机）
        /// pending_pay: 待支付 | paid: 已支付 | completed: 已完成（核销/取餐）
        /// refunding: 退款中 | refunded: 已退款 | cancelled: 已取消
        /// OrderStatus
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "varchar(20)", IsNullable = false)]
        public string Status { get; set; }

        /// <summary>
        /// 支付状态：unpaid(未支付) / paid(已支付) / refunding(退款中) / refunded(已退款)
        /// OrderPayStatus
        /// </summary>
        [SugarColumn(ColumnName = "pay_status", ColumnDataType = "varchar(20)", IsNullable = false, DefaultValue = "unpaid")]
        public string PayStatus { get; set; }

        #endregion

        /// <summary>
        /// 订单号
        /// </summary>
        [SugarColumn(ColumnName = "order_no", Length = 32, IsNullable = false)]
        public string OrderNo { get; set; }


        /// <summary>
        /// 原价总金额（优惠前），所有商品原价之和
        /// </summary>
        [SugarColumn(ColumnName = "total_amount", DecimalDigits = 2, IsNullable = false)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 优惠总金额
        /// </summary>
        [SugarColumn(ColumnName = "discount_amount", DecimalDigits = 2, IsNullable = false)]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 实付金额，用户实际支付金额
        /// </summary>
        [SugarColumn(ColumnName = "paid_amount", ColumnDataType = "decimal(10,2)", IsNullable = false)]
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// 已退款金额（支持部分退款场景）
        /// </summary>
        [SugarColumn(ColumnName = "refund_amount", ColumnDataType = "decimal(10,2)", IsNullable = false, DefaultValue = "0.00")]
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [SugarColumn(ColumnName = "pay_time", IsNullable = true)]
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 订单完成时间（核销/取餐/送达）
        /// </summary>
        [SugarColumn(ColumnName = "complete_time", IsNullable = true)]
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        [SugarColumn(ColumnName = "cancel_reason", Length = 500, IsNullable = true)]
        public string CancelReason { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [SugarColumn(ColumnName = "refund_reason", Length = 500, IsNullable = true)]
        public string RefundReason { get; set; }

        /// <summary>
        /// 退款完成时间
        /// </summary>
        [SugarColumn(ColumnName = "refund_time", IsNullable = true)]
        public DateTime? RefundTime { get; set; }

        /// <summary>
        /// 订单明细
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(OrderItems.OrderId), nameof(Id))]
        public List<OrderItems> orderItems { get; set; }
    }
}
