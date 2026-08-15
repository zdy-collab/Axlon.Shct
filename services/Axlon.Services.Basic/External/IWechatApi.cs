using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Wechat.Dto;

namespace Axlon.Services.Basic.External
{
    /// <summary>
    /// 微信Api接口
    /// </summary>
    public interface IWechatApi: IScopedDependency
    {
        /// <summary>
        /// 获取接口调用凭据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<WcTokenRes> GetTokenAsync();

        /// <summary>
        /// 获取无限制二维码
        /// </summary>
        /// <returns></returns>
        public Task<GetUnlimitedQRCodeRes> GetUnlimitedQRCodeAsync(string scene, string page);
    }
}
