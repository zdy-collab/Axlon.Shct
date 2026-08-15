namespace Axlon.Services.Contracts.Wechat.Base
{
    public class WechatBaseReq
    {
        /// <summary>
        /// 小程序 appId
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 小程序 appSecret
        /// </summary>
        public string secret { get; set; }

        public WechatBaseReq(WechatOptions baseObj)
        {
            this.appid = baseObj.AppId;
            this.secret = baseObj.AppSecret;
        }
    }
}
