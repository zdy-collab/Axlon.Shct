using Axlon.Services.Contracts.Base.CommonEnum;

namespace Axlon.Services.Contracts.Content.Dto
{
    public class PlatformBannerInfoDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 图片Url
        /// </summary>
        public string Image { get; set; }


        /// <summary>
        /// 跳转类型：merchant:商家详情/group_buy:团购详情/featured:精选专题/page:自定义页面
        /// </summary>
        public string LinkType { get; set; }


        /// <summary>
        /// 跳转目标
        /// 根据link_type不同含义不同：merchant:商家ID/group_buy:团购ID/featured:专题ID/page:页面路径
        /// 
        /// 示例：
        /// /pages/merchant/detail?id=10001
        /// </summary>
        public string LinkTarget { get; set; }


        /// <summary>
        /// 排序值,数值越小越靠前
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 状态,1:启用/0:停用
        /// </summary>
        public DisableEnable Status { get; set; }
    }
}
