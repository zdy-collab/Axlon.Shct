using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class MerchantPrintersRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID（关联 merchants.id）
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false)]
        public Tkey MerchantId { get; set; }
    }
}
