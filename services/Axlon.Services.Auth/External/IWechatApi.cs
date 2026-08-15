using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Wechat.Dto;

namespace Axlon.Services.Auth.External
{
    public interface IWechatApi : IScopedDependency
    {
        /// <summary>
        /// 获取接口调用凭据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<WcTokenRes> GetTokenAsync();

        /// <summary>
        /// 前端code换取用户凭证
        /// </summary>
        /// <param name="js_code"></param>
        /// <returns></returns>
        public Task<WcLoginRes> LoginAsync(string js_code);

        /// <summary>
        /// 获取用户手机号信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<WcPhoneRes> GetPhoneInfoAsync(WcPhoneReq req);

    }
}
