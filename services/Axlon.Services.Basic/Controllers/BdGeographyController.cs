using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.External;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.BdGeography.Dto;
using Axlon.Services.Contracts.BdGeography.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    [Route("api/basic/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class BdGeographyController : BaseApiController
    {
        private readonly IBdGeographyApi bdGeographyApi;
        private readonly IGeographyServices geographyServices;

        public BdGeographyController(IBdGeographyApi bdGeographyApi, IGeographyServices geographyServices)
        {
            this.bdGeographyApi = bdGeographyApi;
            this.geographyServices = geographyServices;
        }

        [HttpGet("reverseGeocoding")]
        public async Task<MessageModel<ReverseGeocodingRes>> ReverseGeocodingAsync([FromQuery] LongitudeLatitude req)
        {
            //ReverseGeocodingReq r = req.Adapt<ReverseGeocodingReq>();

            return Success(data: await bdGeographyApi.ReverseGeocodingAsync(req, PoiTypesRecommendation.Standard));
        }

        [HttpGet("byLocationGetName")]
        public async Task<MessageModel<string>> ByLocationGetNameAsync([FromQuery] LongitudeLatitude req)
        {
            if (req == null) return Failed("请传入经纬度参数");
            //if (req.Longitude == null || req.long) return Failed("经度不能为空");
            //if (req.Latitude == null) return Failed("纬度不能为空");
            //ReverseGeocodingReq r = req.Adapt<ReverseGeocodingReq>();

            //var res = await bdGeographyApi.ReverseGeocodingAsync(req);

            var name = await geographyServices.ByLocationGetNameAsync(req);
            return Success(data: name);
        }

        /// <summary>
        /// 获取附近地址信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("getNearbyAddress")]
        public async Task<MessageModel<List<NearbyAddressRes>>> GetNearbyAddressAsync([FromQuery] LongitudeLatitude req)
        {
            if (req == null) return Failed<List<NearbyAddressRes>>("请传入经纬度参数");

            var res = await bdGeographyApi.ReverseGeocodingAsync(req, PoiTypesRecommendation.Standard);

            // 获取权重排序后的数据
            var weightOrder = BdGeographyWeightHelper.SortByComprehensiveWeight(res.result.pois).Take(20);

            var result = weightOrder.Select(x => new NearbyAddressRes
            {
                addr = x.addr,
                name = x.name,
                mater = x.distance,
                Longitude = x.point.x,
                Latitude = x.point.y
            }).OrderBy(x => x.mater).ToList();

            return Success(data: result);
        }

        /// <summary>
        /// 行政区域检索
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("adAreaSearch")]
        public async Task<MessageModel<ADAreaSearchRes>> ADAreaSearchAsync([FromQuery] ADAreaSearchReq req)
        {
            return Success(data: await bdGeographyApi.ADAreaSearchAsync(req));
        }

        /// <summary>
        /// 圆形区域检索
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("radiusAreaSearch")]
        public async Task<MessageModel<List<NearbyAddressRes>>> RadiusAreaSearchAsync([FromQuery] RadiusAreaSearchReq req)
        {
            var res = await bdGeographyApi.RadiusAreaSearchAsync(req);

            var result = res.results.Select(x => new NearbyAddressRes
            {
                name = x.name,
                addr = x.address,
                mater = x.detail_info.distance.ToString(),
                Latitude = x.location.lat,
                Longitude = x.location.lng
            }).ToList();

            return Success(data: result);
        }

        [HttpGet("addressSearch")]
        public async Task<MessageModel<List<AddressInfoDto>>> AddressSearchAsync([FromQuery] AddressSearchReq req)
        {
            return Success(data: await geographyServices.AddressSearchAsync(req));
        }


        //[HttpGet("byLocationGetNameAsync2")]
        //[AllowAnonymous]
        //public async Task<ReverseGeocodingRes> ByLocationGetNameAsync2([FromQuery]LongitudeLatitude location) 
        //{
        //    return await geographyServices.ByLocationGetNameAsync(location);
        //}
    }
}
