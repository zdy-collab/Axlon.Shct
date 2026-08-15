using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class MerchantCategoryConfigRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnName = "merchant_id")]

        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 品类ID
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnName = "category_id")]

        public Tkey CategoryId { get; set; }

    }
}
