using Axlon.Services.Contracts.Content.Enum;
using Axlon.Services.Contracts.Content.JsonObj;
using Axlon.Services.Contracts.Content.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Content
{
    [Tenant("Main")]
    [SugarTable("platform_contents", "种子内容/邻里探店")]
    public class PlatformContents : PlatformContentsRoot<long>
    {


        /// <summary>
        /// 内容标题
        /// </summary>
        [SugarColumn(ColumnName = "title", Length = 200, IsNullable = false)]
        public string Title { get; set; }


        /// <summary>
        /// 内容类型
        /// 
        /// article: 图文文章
        /// video: 视频内容
        /// </summary>
        [SugarColumn(ColumnName = "type", Length = 20, IsNullable = false)]
        public string Type { get; set; }


        /// <summary>
        /// 内容主体,JSON格式存储：ImgVideoContentObj
        /// </summary>
        [SugarColumn(ColumnName = "content", ColumnDataType = "json", IsJson = true, IsNullable = true)]
        public ImgVideoContentObj Content { get; set; }


        /// <summary>
        /// 内容封面图片地址
        /// </summary>
        [SugarColumn(ColumnName = "cover_image_oss", Length = 500, IsNullable = true)]
        public string CoverImageOss { get; set; }


        /// <summary>
        /// 发布状态 0:草稿；1:已发布
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", IsNullable = false)]
        public PublishStatus Status { get; set; }


        /// <summary>
        /// 发布时间：内容正式展示时间
        /// </summary>
        [SugarColumn(ColumnName = "publish_time", IsNullable = true)]
        public DateTime? PublishTime { get; set; }
    }
}
