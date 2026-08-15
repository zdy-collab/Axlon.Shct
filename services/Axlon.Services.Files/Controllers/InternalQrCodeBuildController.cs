using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Files.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.Controllers
{
    [Route("api/files/internal/[controller]")]
    [ApiController]
    //[Authorize(Permissions.Name)]
    [Authorize]
    public class InternalQrCodeBuildController : BaseApiController
    {
        private readonly IQrCodeBuildServices qrCodeBuildServices;

        public InternalQrCodeBuildController(IQrCodeBuildServices qrCodeBuildServices)
        {
            this.qrCodeBuildServices = qrCodeBuildServices;
        }

        /// <summary>
        /// 创建个人推广码
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("createPromotionCode")]
        public async Task<MessageModel<string>> CreatePromotionCodeAsync([FromBody]CreatePromotionCodeReq req, CancellationToken cancellationToken) 
        {
            var id = await qrCodeBuildServices.CreatePromotionCodeAsync(req, cancellationToken);
            if (id <= 0) return Failed("个人推广码创建失败");
            else return Success(data:  id.ToString());
        }
    }
}
