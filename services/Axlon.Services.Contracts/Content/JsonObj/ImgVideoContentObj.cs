using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Contracts.Content.JsonObj
{
    public class ImgVideoContentObj
    {
        /// <summary>
        /// 图片文件路径id
        /// </summary>
        public string imgUrlId { get; set; }

        public string imgUrlOss { get; set; }


        /// <summary>
        /// 视频文件路径id
        /// </summary>
        public string videoUrlId { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        public string text { get; set; }
    }

    public class ImgVideoContentObjDto
    {
        /// <summary>
        /// 图片文件路径id
        /// </summary>
        public long imgUrlId { get; set; }

        public string imgUrlOss { get; set; }


        public string imgUrl { get
            {
                return imgUrlId != null && imgUrlId >= 0 ? imgUrlId.ToString().CombinFileAccessPath(imgUrlOss) : "";
            } 
        }


        /// <summary>
        /// 视频文件路径id
        /// </summary>
        public long videoUrlId { get; set; }

        public string videoUrl 
        {
            get 
            {
                return videoUrlId != null && videoUrlId >= 0 ? videoUrlId.ToString().CombinFileAccessPath() : "";
            }
        }

        /// <summary>
        /// 文本内容
        /// </summary>
        public string text { get; set; }
    }
}
