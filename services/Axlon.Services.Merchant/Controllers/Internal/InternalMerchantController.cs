using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Merchant.Dto.Inner;
using Axlon.Services.Merchant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Merchant.Controllers.Internal
{
    [Route("api/merchant/internal/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InternalMerchantController : BaseApiController
    {
        private readonly IMerchantServices merchantServices;

        public InternalMerchantController(IMerchantServices merchantServices)
        {
            this.merchantServices = merchantServices;
        }

        /// <summary>
        /// 获取商家基本信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="tableIds"></param>
        /// <returns></returns>
        [HttpGet("getMerchantBasic")]
        public async Task<MessageModel<MerchantBasic_TableDto>> GetMerchantBasicAsync(long merchantId, string? tableIds) 
        {
            var ids = new List<long>();
            if(!string.IsNullOrEmpty(tableIds)) ids = tableIds.Split(',').Select(long.Parse).ToList();
            return Success(data: await merchantServices.GetMerchantBasicAsync(merchantId, ids));
        }
    }
}
