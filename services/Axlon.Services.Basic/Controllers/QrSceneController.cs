using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.External;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 二维码接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class QrSceneController : BaseApiController
    {
        private readonly IQrSceneServices qrSceneServices;

        public QrSceneController(IQrSceneServices qrSceneServices)
        {
            this.qrSceneServices = qrSceneServices;
        }

        /// <summary>
        /// 获取推广码
        /// </summary>
        /// <returns></returns>
        [HttpGet("getPromotionQrCode")]
        public async Task<MessageModel<object>> GetPromotionQrCodeAsync() 
        {

            var result = await qrSceneServices.GetPromotionQrCodeAsync();

            if (StaticStatus.ReturnOssStatus) return Success<object>(data: new { imageUrl = "https://axlon-hlhl.oss-cn-chengdu.aliyuncs.com/Temporary/DXK0WE.png?OSSAccessKeyId=LTAI5t7XZxsWGq4ioZiA9xUr&Expires=1794278845&Signature=69sSZs%2FFJLnwZEcP21hPV2fYPVs%3D" });
            if (result.Item1) return Success<object>(data: new { imageUrl = result.Item2 });
            else return Failed<object>(result.Item2);
        }

        /// <summary>
        /// 根据Scene获取二维码信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("getQrSceneByScene")]
        [AllowAnonymous]
        public async Task<MessageModel<QrSceneBasicDto>> GetQrSceneBySceneAsync(string scene) 
        {
            return Success(data: await qrSceneServices.GetQrSceneBySceneAsync(scene));
        }
    }
}
