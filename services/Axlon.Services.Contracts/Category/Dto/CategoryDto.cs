using Axlon.Services.Contracts.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Axlon.Services.Contracts.Category.Dto
{
    public class CategoryNodeDto
    {
        public long Id { get; set; }

        public long ImageFileId { get; set; }

        public string ImageOss { get; set; }
        //public long ParentId { get; set; }

        //public long Sort { get; set; }

        /// <summary>
        /// 品类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string Image
        {
            get
            {
                //if (string.IsNullOrEmpty(image)) return "";
                return ImageFileId.ToString().CombinFileAccessPath(ImageOss);
            }
        }

        public List<CategoryNodeDto> children { get; set; } = new();
    }

    public class AddCategoryReq
    {

        public long Id { get; set; }

        /// <summary>
        /// 父级品类ID -> categories.id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 品类名称
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// 品类层级
        /// </summary>
        public byte Level { get; set; }


        /// <summary>
        /// 分类路径
        /// </summary>
        public string Path { get; set; } = string.Empty;


        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }


        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<AddCategoryReq> Children { get; set; }
    }
}
