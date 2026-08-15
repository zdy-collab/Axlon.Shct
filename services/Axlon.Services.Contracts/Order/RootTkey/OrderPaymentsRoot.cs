using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class OrderPaymentsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 订单ID，关联 orders.id
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = false)]
        public Tkey OrderId { get; set; }
    }
}
