using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion.RootTkey
{
    public class PromotionWithdrawalsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户ID（提现申请人）
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID", IsNullable = false)]
        public long UserId { get; set; }

        /// <summary>
        /// 审核人ID
        /// </summary>
        [SugarColumn(ColumnName = "auditor_id", ColumnDescription = "审核人ID", IsNullable = true)]
        public long? AuditorId { get; set; }
    }
}
