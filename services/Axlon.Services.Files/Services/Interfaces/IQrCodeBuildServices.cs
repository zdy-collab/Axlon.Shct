using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Promotion.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.Services.Interfaces
{
    public interface IQrCodeBuildServices: IScopedDependency
    {

        /// <summary>
        /// 创建推广码
        /// </summary>
        /// <param name="scene">qr_scene:scene（唯一编号）</param>
        /// <param name="pagePath">小程序码跳转路径</param>
        /// <returns></returns>
        Task<long> CreatePromotionCodeAsync(CreatePromotionCodeReq req, CancellationToken cancellationToken);
    }
}
