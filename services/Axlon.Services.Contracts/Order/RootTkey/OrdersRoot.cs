using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class OrdersRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户ID，-> users.id
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false)]
        public Tkey UserId { get; set; }

        /// <summary>
        /// 商家ID，-> merchants.id
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false)]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 桌台ID（仅堂食订单有效），-> merchant_tables.id
        /// </summary>
        [SugarColumn(ColumnName = "table_id", IsNullable = true)]
        public Tkey? TableId { get; set; }

        /// <summary>
        /// 推广人ID（下单时锁定），仅 Source = 推广码 时有效
        /// </summary>
        [SugarColumn(ColumnName = "promo_user_id", IsNullable = true)]
        public Tkey? PromoUserId { get; set; }
    }
}
