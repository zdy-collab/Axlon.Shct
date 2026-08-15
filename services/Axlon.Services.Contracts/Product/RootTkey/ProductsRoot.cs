using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Product.RootTkey
{
    public class ProductsRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID -> merchants.id
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnName = "merchant_id", ColumnDescription = "商家ID -> merchants.id")]

        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 商家分类Id -> product_categories.id
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnName = "category_id", ColumnDescription = "商家分类Id -> product_categories.id")]

        public Tkey CategoryId { get; set; }

        /// <summary>
        /// 菜品图片文件路径
        /// </summary>
        [SugarColumn(ColumnName = "image_file_id", ColumnDescription = "菜品图片文件路径",IsNullable = true)]

        public Tkey ImageFileId { get; set; }
    }
}
