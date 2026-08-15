using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 支付记录
    /// </summary>
    [SugarTable("order_payments", "支付记录")]
    public class OrderPayments : OrderPaymentsRoot<long>
    {
        #region enum

        /// <summary>
        /// 支付类型：wechat(微信) / alipay(支付宝) / cash(现金) / combined(组合支付)
        /// </summary>
        [SugarColumn(ColumnName = "pay_type", ColumnDataType = "varchar(20)", IsNullable = false)]
        public OrderPaymentsPayType PayType { get; set; }

        /// <summary>
        /// 交易状态：pending(处理中) / success(成功) / failed(失败) / refund(已退款)
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "varchar(20)", IsNullable = false)]
        public OrderPaymentsStatus Status { get; set; }

        #endregion

        /// <summary>
        /// 第三方支付流水号
        /// 微信支付：transaction_id / 支付宝：trade_no
        /// 用于对账和退款时关联第三方订单
        /// </summary>
        [SugarColumn(ColumnName = "transaction_id", Length = 64, IsNullable = false)]
        public string TransactionId { get; set; }

        /// <summary>
        /// 本次交易金额
        /// 正数：支付 / 负数：退款
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(10,2)", IsNullable = false)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 交易时间（第三方支付成功时间）
        /// </summary>
        [SugarColumn(ColumnName = "pay_time", IsNullable = false)]
        public DateTime PayTime { get; set; }

        /// <summary>
        /// 第三方回调原始数据（JSON 格式）
        /// 存储微信/支付宝回调的完整数据，用于对账和问题排查
        /// </summary>
        [SugarColumn(ColumnName = "callback_data", ColumnDataType = "json", IsNullable = true)]
        public string CallbackData { get; set; }
    }
}
