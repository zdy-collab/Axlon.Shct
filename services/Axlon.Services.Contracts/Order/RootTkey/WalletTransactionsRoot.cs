using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class WalletTransactionsRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false)]
        public Tkey UserId { get; set; }

        /// <summary>
        /// 关联订单ID
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = true)]
        public Tkey? OrderId { get; set; }
    }
}
