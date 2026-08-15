using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers.Internal
{
    [Route("api/basic/internal/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InternalPromotionCommissionRuleController : BaseApiController
    {
        private readonly IPromotionCommissionRuleServices pcrServices;

        public InternalPromotionCommissionRuleController(IPromotionCommissionRuleServices pcrServices)
        {
            this.pcrServices = pcrServices;
        }

        /// <summary>
        /// 根据商家Id获取分佣规则，如果没有则返回全局分佣配置
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [HttpGet("byMerchantIdGetPCRule")]
        public async Task<MessageModel<PromotionCommissionRulesBasicDto>> ByMerchantIdGetPCRuleAsync([FromQuery]long merchantId) 
        {
            return Success(data: await pcrServices.ByMerchantIdGetPCRuleAsync(merchantId));
        }
    }
}
