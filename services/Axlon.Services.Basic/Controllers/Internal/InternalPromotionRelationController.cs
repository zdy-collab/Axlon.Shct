using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Axlon.Services.Basic.Controllers.Internal
{
    [Route("api/basic/internal/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InternalPromotionRelationController : BaseApiController
    {
        private readonly IPromotionRelationsServices promotionRelationsServices;

        public InternalPromotionRelationController(IPromotionRelationsServices promotionRelationsServices)
        {
            this.promotionRelationsServices = promotionRelationsServices;
        }

        /// <summary>
        /// 创建推广关系
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("bindPromotionRelation")]
        public async Task<MessageModel<string>> BindPromotionRelationAsync([FromBody]BindPromotionRelationReq req) 
        {
            var data = await promotionRelationsServices.BindPromotionRelationAsync(req);
            if (data.Item1) return Failed(data.Item2);
            return Success<string>(string.Empty);
        }

        /// <summary>
        /// 根据用户Id获取推广关系基础信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("byUserIdGetBasicInfo")]
        public async Task<MessageModel<List<PromotionRelationsBasicDto>>> ByUserIdGetBasicInfoAsync([FromQuery]long userId) 
        {
            return Success(data: await promotionRelationsServices.ByUserIdGetBasicInfoAsync(userId));
        }
    }
}
