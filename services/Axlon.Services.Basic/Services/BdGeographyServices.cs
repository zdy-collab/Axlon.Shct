using Axlon.Services.Basic.External;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.BdGeography.Dto;
using Axlon.Services.Contracts.BdGeography.Helper;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// 百度地理服务实现
    /// </summary>
    public class BdGeographyServices : IGeographyServices
    {
        private IBdGeographyApi bdGeographyApi { get; set; }

        public BdGeographyServices(IBdGeographyApi bdGeographyApi)
        {
            this.bdGeographyApi = bdGeographyApi;
        }

        public async Task<List<AddressInfoDto>> AddressSearchAsync(AddressSearchReq req)
        {
            switch (req.queryType)
            {
                case AddressSearchReqQueryType.附近地标推荐:
                    {
                        var res1 = await bdGeographyApi.ReverseGeocodingAsync(req, PoiTypesRecommendation.Standard);

                        // 获取权重排序后的数据
                        var weightOrder = BdGeographyWeightHelper.SortByComprehensiveWeight(res1.result.pois).Take(20);

                        var result1 = weightOrder.Select(x => new AddressInfoDto
                        {
                            addr = x.addr,
                            name = x.name,
                            mater = x.distance,
                            Longitude = x.point.x,
                            Latitude = x.point.y
                        }).OrderBy(x => x.mater).ToList();

                        return result1;
                    }

                case AddressSearchReqQueryType.圆形检索附近地标:
                    {

                        var res2 = await bdGeographyApi.RadiusAreaSearchAsync(new RadiusAreaSearchReq(req.query, req.Longitude, req.Latitude, req.radius));

                        var result2 = res2.results.Select(x => new AddressInfoDto
                        {
                            name = x.name,
                            addr = x.address,
                            mater = x.detail_info.distance.ToString(),
                            Latitude = x.location.lat,
                            Longitude = x.location.lng
                        }).ToList();
                        return result2;

                    }

                case AddressSearchReqQueryType.行政区域检索地标:
                    {
                        var res3 = await bdGeographyApi.ADAreaSearchAsync(new ADAreaSearchReq(req.query, req.region, req.Longitude, req.Latitude));

                        var result3 = res3.results.Select(x => new AddressInfoDto
                        {
                            name = x.name,
                            addr = x.address,
                            mater = null,
                            Latitude = x.location.lat,
                            Longitude = x.location.lng
                        }).ToList();

                        return result3;
                    }
                default:
                    throw new Exception("搜索类型错误");
            }
        }

        public async Task<string> ByLocationGetNameAsync(LongitudeLatitude location)
        {
            var res = await bdGeographyApi.ReverseGeocodingAsync(location, poi_types: PoiTypesRecommendation.ByLocationGetNamePoi);

            var returnData = BaiduGeographyHelper.SelectForHomePage(res.result.pois);

            return returnData.name;
        }
    }
}
