using Axlon.Services.Contracts.Product.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Product
{
    /// <summary>
    /// 商家自定义菜品分类
    /// </summary>
    [Tenant("Main")]
    [SugarTable("product_categories", "商家自定义菜品分类")]
    public class ProductCategories : ProductCategoriesRoot<long>
    {
        /// <summary>
        /// 分类名
        /// </summary>
        [SugarColumn(Length = 100)]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Products.CategoryId), nameof(Id))]
        public List<Products> products { get; set; }
    }
}
