using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;

namespace Axlon.Services.Basic.Services.Interfaces
{
    /// <summary>
    /// 二维码场景服务接口
    /// </summary>
    public interface IQrSceneServices:IBaseServices<QrScene>
    {

        /// <summary>
        /// 获取推广码访问地址
        /// </summary>
        /// <returns></returns>
        Task<(bool, string)> GetPromotionQrCodeAsync();

        /// <summary>
        /// 根据Scene获取二维码信息
        /// </summary>
        /// <returns></returns>
        Task<QrSceneBasicDto> GetQrSceneBySceneAsync(string scene);
    }
}
