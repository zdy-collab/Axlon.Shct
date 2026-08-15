using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class OrderVerificationsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 订单ID（唯一索引）
        /// 关联 orders.id，一笔订单只能有一条核销记录
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = false)]
        public Tkey OrderId { get; set; }

        /// <summary>
        /// 核销人ID
        /// 关联 merchant_employees.id，记录是哪位店员操作的核销
        /// </summary>
        [SugarColumn(ColumnName = "verifier_id", IsNullable = false)]
        public Tkey VerifierId { get; set; }

        /// <summary>
        /// 核销设备ID
        /// 关联 devices.id，记录核销使用的设备（POS机/手机/扫码枪等）
        /// </summary>
        [SugarColumn(ColumnName = "verify_device_id", IsNullable = true)]
        public Tkey? VerifyDeviceId { get; set; }
    }
}
