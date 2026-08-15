using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Content.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Content
{
    [Tenant("Main")]
    [SugarTable("platform_banners", "首页入口/Banner")]
    public class PlatformBanners : PlatformBannersRoot<long>
    {
        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(ColumnName = "title", Length = 100, IsNullable = false)]
        public string Title { get; set; }


        /// <summary>
        /// 图片Url
        /// </summary>
        [SugarColumn(ColumnName = "image", Length = 500, IsNullable = false)]
        public string Image { get; set; }


        /// <summary>
        /// 跳转类型：merchant:商家详情/group_buy:团购详情/featured:精选专题/page:自定义页面
        /// </summary>
        [SugarColumn(ColumnName = "link_type", Length = 20, IsNullable = false)]
        public string LinkType { get; set; }


        /// <summary>
        /// 跳转目标
        /// 根据link_type不同含义不同：merchant:商家ID/group_buy:团购ID/featured:专题ID/page:页面路径
        /// 
        /// 示例：
        /// /pages/merchant/detail?id=10001
        /// </summary>
        [SugarColumn(ColumnName = "link_target", Length = 500, IsNullable = false)]
        public string LinkTarget { get; set; }


        /// <summary>
        /// 排序值,数值越小越靠前
        /// </summary>
        [SugarColumn(ColumnName = "sort", IsNullable = false)]
        public int Sort { get; set; }


        /// <summary>
        /// 状态,1:启用/0:停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", IsNullable = false)]
        public DisableEnable Status { get; set; }
    }
}
