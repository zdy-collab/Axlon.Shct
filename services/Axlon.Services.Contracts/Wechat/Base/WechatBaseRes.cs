namespace Axlon.Services.Contracts.Wechat.Base
{
    public class WechatBaseRes
    {
        /// <summary>
        /// 错误码，请求失败时返回
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误信息，请求失败时返回
        /// </summary>
        public string errmsg { get; set; }
    }
}
