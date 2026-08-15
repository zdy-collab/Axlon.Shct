using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Product.RootTkey
{
    public class ProductCategoriesRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID -> merchants.id
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnName = "merchant_id", ColumnDescription = "商家ID -> merchants.id")]

        public Tkey MerchantId { get; set; }
    }
}
