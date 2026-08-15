using Axlon.Services.Contracts.Category.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Category
{
    [Tenant("Main")]
    [SugarTable("categories", "全平台品类树")]
    public class Categories : CategoriesRoot<long>
    {
        /// <summary>
        /// 品类名称
        /// 
        /// 示例：
        /// 火锅、川菜、奶茶
        /// </summary>
        [SugarColumn(ColumnName = "name", Length = 100, ColumnDescription = "品类名称")]
        public string Name { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [SugarColumn(ColumnName = "image_oss",ColumnDescription = "图片路径", IsNullable = true)]
        public string ImageOss { get; set; }

        /// <summary>
        /// 品类层级
        /// </summary>
        [SugarColumn(ColumnName = "level", ColumnDescription = "层级")]
        public byte Level { get; set; }


        /// <summary>
        /// 分类路径
        /// </summary>
        [SugarColumn(ColumnName = "path", Length = 500, ColumnDescription = "分类路径")]
        public string Path { get; set; }


        /// <summary>
        /// 排序值
        /// </summary>
        [SugarColumn(ColumnName = "sort", ColumnDescription = "排序")]
        public int Sort { get; set; }


        /// <summary>
        /// 状态
        /// 1：启用
        /// 0：停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "状态：1启用/0停用")]
        public byte Status { get; set; }


        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<Categories> Children { get; set; }
    }
}
