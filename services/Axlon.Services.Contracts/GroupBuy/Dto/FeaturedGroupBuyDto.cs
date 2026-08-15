using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Contracts.GroupBuy.Dto
{
    public class FeaturedGroupBuyInfoDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 团购Id
        /// </summary>
        public long GroupBuyId { get; set; }

        /// <summary>
        /// 团购标题
        /// </summary>
        public string Title { get; set; }

        public string ImageOss { get; set; }

        public long ImageFileId { get; set; }

        /// <summary>
        /// 图片Url
        /// </summary>
        public string Image
        {
            get
            {
                return ImageFileId.ToString().CombinFileAccessPath(ImageOss);
            }
        }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 团购价
        /// </summary>
        public decimal GroupPrice { get; set; }

        /// <summary>
        /// 排序权重（数值越大越靠前）
        /// </summary>
        public int SortWeight { get; set; }
    }
}
