using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Content.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    [Route("api/basic/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]

    public class PlatformContentController : BaseApiController
    {
        private readonly IPlatformContentServices platformContentServices;

        public PlatformContentController(IPlatformContentServices platformContentServices)
        {
            this.platformContentServices = platformContentServices;
        }

        [HttpGet("getPlatformContent")]
        public async Task<MessageModel<List<PlatformContentInfoDto>>> GetPlatformContentAsync()
        {
            var data = await platformContentServices.GetPlatformContentAsync();
            return Success(data: data);
        }
    }
}
