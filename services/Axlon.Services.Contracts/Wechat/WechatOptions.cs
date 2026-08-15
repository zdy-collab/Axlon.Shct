namespace Axlon.Services.Contracts.Wechat
{
    public class WechatOptions
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        /// <summary>
        /// 首页路径
        /// </summary>
        public string HomePage { get; set; }
    }
}
