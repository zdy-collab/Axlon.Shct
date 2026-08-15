using Axlon.Services.Contracts.Content.Enum;
using Axlon.Services.Contracts.Content.JsonObj;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Contracts.Content.Dto
{
    public class PlatformContentInfoDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 关联商家ID -> merchants.id
        /// </summary>
        public long? MerchantId { get; set; }

        /// <summary>
        /// 封面图片文件Id
        /// </summary>
        public long CoverImageFileId { get; set; }

        /// <summary>
        /// 内容标题
        /// </summary>
        public string Title { get; set; }

        public string CoverImageOss { get; set; }


        /// <summary>
        /// 内容类型
        /// 
        /// article: 图文文章
        /// video: 视频内容
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// 内容主体,JSON格式存储：ImgVideoContentObj
        /// </summary>
        public ImgVideoContentObjDto Content { get; set; }


        /// <summary>
        /// 内容封面图片地址
        /// </summary>
        public string CoverImage 
        { 
            get
            {
                return this.CoverImageFileId.ToString().CombinFileAccessPath(CoverImageOss);
            }
        }


        /// <summary>
        /// 发布状态 0:草稿；1:已发布
        /// </summary>
        public PublishStatus Status { get; set; }


        /// <summary>
        /// 发布时间：内容正式展示时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
    }
}
