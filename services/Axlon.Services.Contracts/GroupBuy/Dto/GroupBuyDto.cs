using Axlon.Services.Contracts.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;

namespace Axlon.Services.Contracts.GroupBuy.Dto
{
    public class GroupBuyInfoDto
    {
        public long Id { get; set; }

        public long ImageFileId { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImageOss { get; set; }

        /// <summary>
        /// 团购标题
        /// </summary>
        public string Title { get; set; }

        //private string image { get; set; }

        /// <summary>
        /// 图片Url
        /// </summary>
        public string Image
        {
            get
            {
                //if (string.IsNullOrEmpty(image)) return "";
                return ImageFileId.ToString().CombinFileAccessPath(ImageOss);
            }
        }

        /// <summary>
        /// 团购类型：gift-赠品型，subsidy-补贴型，GroupBuysType
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 团购价
        /// </summary>
        public decimal GroupPrice { get; set; }
    }
}
