using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion.RootTkey
{
    public class PromotionRelationsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 被推广人ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "被推广人ID", IsTreeKey = true, IsNullable = false)]
        public Tkey UserId { get; set; }

        /// <summary>
        /// 上级推广人ID
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "上级推广人ID", IsNullable = false)]
        public Tkey ParentId { get; set; }

        /// <summary>
        /// 绑定订单ID
        /// </summary>
        [SugarColumn(ColumnName = "bind_order_id", ColumnDescription = "绑定订单ID", IsNullable = false)]
        public Tkey BindOrderId { get; set; }

        /// <summary>
        /// 解除操作人ID
        /// </summary>
        [SugarColumn(ColumnName = "unbind_operator_id", ColumnDescription = "解除操作人ID", IsNullable = true)]
        public Tkey? UnbindOperatorId { get; set; }
    }
}
