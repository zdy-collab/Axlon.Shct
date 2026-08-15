using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Merchant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Merchant.Controllers
{
    /// <summary>
    /// 团购商品
    /// </summary>
    [Route("api/merchant/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class GroupBuyController : BaseApiController
    {
        private readonly IGroupBuyServices groupBuyServices;

        public GroupBuyController(IGroupBuyServices groupBuyServices)
        {
            this.groupBuyServices = groupBuyServices;
        }

        /// <summary>
        /// 根据商家Id获取团购商品信息
        /// </summary>
        /// <param name="queryPage"></param>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [HttpGet("byMerchantIdGetInfo")]
        public async Task<MessageModel<PageResponseModel<GroupBuyInfoDto>>> ByMerchantIdGetInfoAsync([FromQuery]QueryPage queryPage,[FromQuery]long merchantId) 
        {
            return Success(data:await groupBuyServices.ByMerchantIdGetInfoAsync(queryPage, merchantId));
        }
    }
}
