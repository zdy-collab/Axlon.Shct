namespace Axlon.Services.Contracts.ViewModels
{
    /// <summary>
    /// 微信登录Dto
    /// </summary>
    public class WechatLoginReq
    {
        /// <summary>
        /// 用户code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 推广人Id
        /// </summary>
        public long? promoUserId { get; set; }

        /// <summary>
        /// 场景值，前端通过 wx.getLaunchOptionsSync() 方法获取场景值
        /// 分享链接进入：1007、1008、1044；搜索进入：1005、1006、1053
        /// </summary>
        public int SceneID { get; set; }

        public string? TestUser { get; set; }

        /// <summary>
        /// 来源（搜索/分享/推广码）,Source
        /// </summary>
        //public Source Source { get; set; }
    }

    public class PhoneNumberLoginReq
    {
        public string LoginCode { get; set; }
        public string PhoneNumberCode { get; set; }
    }
}
