using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class OrderRefundsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 订单ID，关联 orders.id
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = false)]
        public Tkey OrderId { get; set; }

        /// <summary>
        /// 处理人ID
        /// 可以是平台客服（users.id）或商家员工（merchant_employees.id）
        /// 建议用单独的处理人类型字段区分
        /// </summary>
        [SugarColumn(ColumnName = "processor_id", IsNullable = true)]
        public Tkey? ProcessorId { get; set; }
    }
}
