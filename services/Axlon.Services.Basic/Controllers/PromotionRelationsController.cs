using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Promotion.Dto.Mini;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 推广信息接口
    /// </summary>
    [Route("api/basic/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class PromotionRelationsController : BaseApiController
    {
        private readonly IPromotionRelationsServices promotionRelationsMiniServices;

        public PromotionRelationsController(IPromotionRelationsServices promotionRelationsMiniServices)
        {
            this.promotionRelationsMiniServices = promotionRelationsMiniServices;
        }

        /// <summary>
        /// 获取我的推广信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("getMyPromotionInfo")]
        public async Task<MessageModel<GetMyPromotionInfoRes>> GetMyPromotionInfoAsync()
        {
            return Success(data: await promotionRelationsMiniServices.GetMyPromotionInfoAsync());
        }
    }
}
