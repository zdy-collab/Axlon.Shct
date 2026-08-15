using Axlon.Services.Contracts.Wechat.Base;

namespace Axlon.Services.Contracts.Wechat.Dto
{
    #region req

    public class WcTokenReq : WechatBaseReq
    {
        public WcTokenReq(WechatOptions baseObj) : base(baseObj)
        {
        }

        /// <summary>
        /// 授权类型:authorization_code、client_credential
        /// </summary>
        public string? grant_type { get; private set; } = "client_credential";
    }

    public class WcPhoneReq
    {
        /// <summary>
        /// 手机号获取凭证
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 用户openid， 填入后会校验openid和code的绑定关系，并在不匹配时报错，非必填
        /// </summary>
        public string? openid { get; set; }

        public WcPhoneReq(string code, string? openid = null)
        {
            this.code = code;
            this.openid = openid;
        }
    }
    public class WcLoginReq : WechatBaseReq
    {
        public WcLoginReq(WechatOptions baseObj, string js_code) : base(baseObj)
        {
            this.js_code = js_code;
        }

        /// <summary>
        /// 登录时获取的 code，前端可通过wx.login获取
        /// </summary>
        public string js_code { get; set; }

        public string grant_type { get; set; } = "authorization_code";


    }


    #endregion

    #region res
    public class WcTokenRes : WechatBaseRes
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒。目前是7200秒之内的值
        /// </summary>
        public int expires_in { get; set; }
    }

    public class WcLoginRes : WechatBaseRes
    {
        /// <summary>
        /// 会话密钥
        /// </summary>
        public string session_key { get; set; }

        /// <summary>
        /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台帐号下会返回，详见 UnionID 机制说明。
        /// </summary>
        public string unionid { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string openid { get; set; }
    }

    #region WcPhoneRes
    public class WcPhoneRes : WechatBaseRes
    {

        /// <summary>
        /// 用户手机号信息
        /// </summary>
        public WcPhoneInfo phone_info { get; set; }
    }

    public class WcPhoneInfo
    {
        /// <summary>
        /// 用户绑定的手机号（国外手机号会有区号）
        /// </summary>
        public string phoneNumber { get; set; }

        /// <summary>
        /// 没有区号的手机号
        /// </summary>
        public string purePhoneNumber { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string countryCode { get; set; }

        /// <summary>
        /// 数据水印
        /// </summary>
        public WcPhoneWatermark watermark { get; set; }
    }

    public class WcPhoneWatermark
    {
        /// <summary>
        /// 用户获取手机号操作的时间戳
        /// </summary>
        public int timestamp { get; set; }

        /// <summary>
        /// 小程序appid
        /// </summary>
        public string appid { get; set; }
    }
    #endregion

    #endregion
}
