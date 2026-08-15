using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Category.RootTkey
{
    public class CategoriesRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 父级品类ID -> categories.id
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父级ID")]
        public Tkey ParentId { get; set; }

        /// <summary>
        /// image图片文件路径
        /// </summary>
        [SugarColumn(ColumnName = "image_file_id", ColumnDescription = "image图片文件路径",IsNullable = true)]

        public Tkey ImageFileId { get; set; }

    }
}
