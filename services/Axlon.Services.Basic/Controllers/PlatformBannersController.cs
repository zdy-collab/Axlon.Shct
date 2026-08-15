using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Content.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    [Route("api/basic/[controller]")]
    [ApiController]
    public class PlatformBannersController : BaseApiController
    {
        private readonly IPlatformBannerServices platformBannersServices;

        public PlatformBannersController(IPlatformBannerServices platformBannersServices)
        {
            this.platformBannersServices = platformBannersServices;
        }

        [HttpGet("getPlatformBannersList")]
        public async Task<MessageModel<List<PlatformBannerInfoDto>>> GetPlatformBannersListAsync()
        {
            var data = await platformBannersServices.Query(
                expression: x => new PlatformBannerInfoDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Image = x.Image,
                    LinkType = x.LinkType,
                    LinkTarget = x.LinkTarget,
                    Sort = x.Sort,
                    Status = x.Status
                },
                whereExpression: x => x.Status == DisableEnable.启用, "sort");

            return Success(data: data);
        }
    }
}
