using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion.RootTkey
{
    public class PromotionEarningsRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [SugarColumn(ColumnName = "order_id", ColumnDescription = "订单ID", IsNullable = false)]
        public Tkey OrderId { get; set; }

        /// <summary>
        /// 推广员ID（佣金受益人）
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "推广员ID（受益人）", IsNullable = false)]
        public Tkey UserId { get; set; }

        /// <summary>
        /// 消费者ID（下单人）
        /// </summary>
        [SugarColumn(ColumnName = "from_user_id", ColumnDescription = "消费者ID", IsNullable = false)]
        public Tkey FromUserId { get; set; }
    }
}
