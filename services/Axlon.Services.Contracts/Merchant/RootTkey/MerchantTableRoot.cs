using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class MerchantTableRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID（关联 merchants.id）
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false)]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 绑定核销盒子ID（关联 devices.id，可为空）
        /// </summary>
        [SugarColumn(ColumnName = "device_id", IsNullable = true)]
        public Tkey? DeviceId { get; set; }
    }
}
