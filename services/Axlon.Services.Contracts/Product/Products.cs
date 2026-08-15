using Axlon.Services.Contracts.GroupBuy.Enums;
using Axlon.Services.Contracts.Product.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Product
{
    /// <summary>
    /// 菜品
    /// </summary>
    [Tenant("Main")]
    [SugarTable("products", "菜品")]
    public class Products : ProductsRoot<long>
    {
        /// <summary>
        /// 菜品名称
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "菜品名称")]
        public string Name { get; set; }

        /// <summary>
        /// 价格（与线下一致）
        /// </summary>
        [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "价格（与线下一致）")]
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品图片URL
        /// </summary>
        [SugarColumn(ColumnName = "image_oss",IsNullable = true, ColumnDescription = "菜品图片URL")]
        public string ImageOss { get; set; }

        /// <summary>
        /// 库存（-1不限）
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "-1", ColumnDescription = "库存（-1不限）")]
        public int Stock { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 销量（实时更新）
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnDescription = "销量（实时更新）")]
        public int SalesCount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnDescription = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 1上架/0下架
        /// </summary>
        [SugarColumn(ColumnName = "is_on", ColumnDataType = "tinyint", ColumnDescription = "1上架/0下架",
        IsNullable = false)]
        public IsOn IsOn { get; set; }

    }
}
