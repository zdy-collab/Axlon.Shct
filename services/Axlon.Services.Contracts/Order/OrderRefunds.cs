using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 退款记录
    /// </summary>
    [SugarTable("order_refunds", "退款记录")]
    public class OrderRefunds : OrderRefundsRoot<long>
    {
        #region enum

        /// <summary>
        /// 退款状态
        /// pending: 待审批 | approved: 已通过（待执行退款）| rejected: 已拒绝 | completed: 已完成（资金已退回）
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "varchar(20)", IsNullable = false)]
        public OrderRefundStatus Status { get; set; }

        #endregion

        /// <summary>
        /// 退款单号（业务唯一标识）
        /// </summary>
        [SugarColumn(ColumnName = "refund_no", Length = 32, IsNullable = false)]
        public string RefundNo { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(10,2)", IsNullable = false)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 退款原因
        /// 用户填写或客服选择的退款理由
        /// </summary>
        [SugarColumn(ColumnName = "reason", Length = 500, IsNullable = false)]
        public string Reason { get; set; }

        /// <summary>
        /// 申请时间（用户发起退款的时间）
        /// </summary>
        [SugarColumn(ColumnName = "apply_time", IsNullable = false)]
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 处理时间（审批或拒绝的时间）
        /// </summary>
        [SugarColumn(ColumnName = "process_time", IsNullable = true)]
        public DateTime? ProcessTime { get; set; }
    }
}
