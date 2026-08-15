using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Promotion.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 分佣记录
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_earnings", "分佣记录")]
    public class PromotionEarnings : PromotionEarningsRoot<long>
    {

        /// <summary>
        /// 创建分佣记录
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="userId">推广员Id（佣金受益人）</param>
        /// <param name="fromUserId">消费者ID（下单人）</param>
        /// <param name="level">推广层级</param>
        /// <param name="commissionAmount">佣金金额</param>
        /// <param name="orderAmount">订单金额</param>
        /// <param name="commissionRate">分佣比例</param>
        /// <returns></returns>
        public static PromotionEarnings Create(long orderId,long userId,long fromUserId,byte level
            ,decimal commissionAmount,decimal orderAmount,decimal commissionRate) 
        {
            return new PromotionEarnings()
            {
                OrderId = orderId,
                UserId = userId,
                FromUserId = fromUserId,
                Level = level,
                CommissionAmount = commissionAmount,
                OrderAmount = orderAmount,
                CommissionRate = commissionRate,
                Status = PromotionEarningsStatus.pending.ToString()
            };
        }

        /// <summary>
        /// 结算
        /// </summary>
        public void SettleAccounts() 
        {
            Status = PromotionEarningsStatus.settled.ToString();
            SettleTime = DateTime.Now;
        }

        /// <summary>
        /// 分佣层级：1直推/2间推/3/4/5
        /// </summary>
        [SugarColumn(ColumnName = "level", ColumnDescription = "分佣层级", IsNullable = false)]
        public byte Level { get; set; }

        /// <summary>
        /// 佣金金额
        /// </summary>
        [SugarColumn(ColumnName = "commission_amount", ColumnDescription = "佣金金额", DecimalDigits = 2, IsNullable = false)]
        public decimal CommissionAmount { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [SugarColumn(ColumnName = "order_amount", ColumnDescription = "订单金额", DecimalDigits = 2, IsNullable = false)]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 分佣比例
        /// </summary>
        [SugarColumn(ColumnName = "commission_rate", ColumnDescription = "分佣比例", DecimalDigits = 4, IsNullable = false)]
        public decimal CommissionRate { get; set; }

        /// <summary>
        /// 状态：pending/settled/cancelled（退款时cancel）
        /// PromotionEarningsStatus
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "varchar(20)",
            ColumnDescription = "pending/settled/cancelled", IsNullable = false)]
        public string Status { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        [SugarColumn(ColumnName = "settle_time", ColumnDescription = "结算时间", IsNullable = true)]
        public DateTime? SettleTime { get; set; }
    }
}
