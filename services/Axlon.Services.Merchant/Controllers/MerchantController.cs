using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Merchant.Helper;
using Axlon.Services.Merchant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Merchant.Controllers
{
    [Route("api/merchant/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]

    public class MerchantController : BaseApiController
    {
        private IMerchantServices merchantServices;

        public MerchantController(IMerchantServices merchantServices)
        {
            this.merchantServices = merchantServices;
        }

        /// <summary>
        /// 根据经纬度获取附近商家信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("byJwGetMerchantList")]
        public async Task<PageResponseModel<ByJwGetMerchantListRes>> ByJwGetMerchantListAsync([FromQuery]ByJwGetMerchantListReq req) 
        {
            return await merchantServices.ByJwGetMerchantListAsync(req);
        }

        ///// <summary>
        ///// 根据筛选条件获取商家信息
        ///// </summary>
        ///// <param name="req"></param>
        ///// <returns></returns>
        //[HttpPost("queryMerchants")]

        //public async Task<QueryMerchantsRes> QueryMerchantsAsync([FromBody]QueryMerchantsReq req) 
        //{
        //    return await merchantService.QueryMerchantsAsync(req);
        //}

        /// <summary>
        /// 附近300米商家
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("nearbyMerchantQuery")]

        public async Task<MessageModel<List<MerchantInfoDto>>> NearbyMerchantQueryAsync([FromQuery]NearbyMerchantQueryReq req) 
        {
            return  Success(data: await merchantServices.NearbyMerchantQueryAsync(req));
        }

        [HttpGet("GetGeoHash")]
        [AllowAnonymous]
        public string GetGeoHash(double Longitude, double Latitude) 
        {
            return GeoHashHelper.GetGeoHash(Longitude, Latitude);  //得到当前点geo前缀坐标
        }

        /// <summary>
        /// 为你推荐商家
        /// </summary>
        /// <returns></returns>
        [HttpGet("recommendMerchantQuery")]
        public async Task<MessageModel<List<MerchantInfoDto>>> RecommendMerchantQueryAsync([FromQuery] RecommendMerchantQueryReq req) 
        {
            return Success(data: await merchantServices.RecommendMerchantQueryAsync(req));
        }

        /// <summary>
        /// 商家列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("searchMerchantQuery")]
        public async Task<MessageModel<List<MerchantInfoDto>>> SearchMerchantQueryAsync([FromBody]SearchMerchantQueryReq req) 
        {
            return Success(data: await merchantServices.SearchMerchantQueryAsync(req));
        }

        /// <summary>
        /// 小程序获取商家详情
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [HttpGet("miniGetMerchantDetails")]

        public async Task<MessageModel<MerchantInfoDto>> MiniGetMerchantDetailsAsync([FromQuery]MiniGetMerchantDetailsReq req) 
        {
            return Success(data: await merchantServices.MiniGetMerchantDetailsAsync(req));
        }
    }
}
