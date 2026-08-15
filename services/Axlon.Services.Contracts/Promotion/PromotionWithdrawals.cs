using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Promotion.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 提现记录
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_withdrawals", "提现记录")]
    public class PromotionWithdrawals : PromotionWithdrawalsRoot<long>
    {

        /// <summary>
        /// 提现金额
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDescription = "提现金额", DecimalDigits = 2, IsNullable = false)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 状态：pending/approved/rejected/completed
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "pending/approved/rejected/completed", ColumnDataType = "varchar(20)"
            , IsNullable = false)]
        public PromotionWithdrawalsStatus Status { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [SugarColumn(ColumnName = "apply_time", ColumnDescription = "申请时间", IsNullable = false)]
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [SugarColumn(ColumnName = "audit_time", ColumnDescription = "审核时间", IsNullable = true)]
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        [SugarColumn(ColumnName = "reject_reason", ColumnDescription = "拒绝原因", Length = 500, IsNullable = true)]
        public string RejectReason { get; set; }

        /// <summary>
        /// 微信打款流水号
        /// </summary>
        [SugarColumn(ColumnName = "transaction_id", ColumnDescription = "微信打款流水号", Length = 64, IsNullable = true)]
        public string TransactionId { get; set; }
    }
}
