using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Merchant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Merchant.Controllers
{
    /// <summary>
    /// 特价团购精选
    /// </summary>
    [Route("api/merchant/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class FeaturedGroupBuyController : BaseApiController
    {
        private readonly IFeaturedGroupBuyServices featuredGroupBuyServices;

        public FeaturedGroupBuyController(IFeaturedGroupBuyServices featuredGroupBuyServices)
        {
            this.featuredGroupBuyServices = featuredGroupBuyServices;
        }

        /// <summary>
        /// 获取有效期内的团购活动
        /// </summary>
        /// <returns></returns>
        [HttpGet("getValidGroupBuy")]
        public async Task<MessageModel<List<FeaturedGroupBuyInfoDto>>> GetValidGroupBuyAsync() 
        {
            return Success(data: await featuredGroupBuyServices.GetValidGroupBuyAsync());
        }
    }
}
