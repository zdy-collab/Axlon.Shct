using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.BdGeography;
using Axlon.Services.Contracts.BdGeography.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Axlon.Services.Basic.External
{
    public class BdGeographyApi : IBdGeographyApi
    {
        private BdGeographyOptions bdGeographyOptions { get; set; }

        private HttpClient httpClient;

        public BdGeographyApi(IOptions<BdGeographyOptions> bdGeographyOptions, IHttpClientFactory factory)
        {
            this.bdGeographyOptions = bdGeographyOptions.Value;
            this.httpClient = factory.CreateClient(ServiceName.bdGeography.ToString());
        }

        public async Task<ReverseGeocodingRes> ReverseGeocodingAsync(LongitudeLatitude location, string poi_types)
        {
            var url = QueryHelpers.AddQueryString(
            "reverse_geocoding/v3",
            new Dictionary<string, string?>
            {
                ["ak"] = bdGeographyOptions.AK,
                ["output"] = "json",
                ["coordtype"] = "gcj02ll",
                ["location"] = $"{location.Latitude},{location.Longitude}",
                ["extensions_poi"] = "1",
                ["pois"] = "1",
                ["radius"] = "300",
                ["poi_types"] = poi_types

            });

            var apiUrl = $"reverse_geocoding/v3/?" +
                $"ak={bdGeographyOptions.AK}" +
                $"&extensions_poi=1" +
                $"&entire_poi=1" +
                $"&sort_strategy=distance" +
                $"&output=json" +
                $"&coordtype=bd09ll" +
                $"&location={location.Latitude.ToString() + ',' + location.Longitude.ToString()}";
            if (!string.IsNullOrEmpty(poi_types)) apiUrl += $"&poi_types={poi_types}";

            var response = await httpClient.GetFromJsonAsync<ReverseGeocodingRes>(apiUrl);
            //var res = await response.Content.ReadFromJsonAsync<ReverseGeocodingRes>();

            return response;
        }

        public async Task<ADAreaSearchRes> ADAreaSearchAsync(ADAreaSearchReq req)
        {
            var apiUrl = $"place/v2/search?" +
                $"query={req.query}" +  // 检索内容
                $"&filter=distance" +  // 按距离排序
                $"&center={req.latitude.ToString() + ',' + req.longitude.ToString()}" +  // 经纬度
                $"&coord_type=2" +  // 2（gcj02ll即国测局经纬度坐标）
                $"&region={req.region}" +   // 行政区域
                $"&output=json" +   // 响应格式
                $"&ak={bdGeographyOptions.AK}"; // 密钥

            var response = await httpClient.GetFromJsonAsync<ADAreaSearchRes>(apiUrl);
            //var res = await response.Content.ReadFromJsonAsync<ReverseGeocodingRes>();

            return response;
        }

        public async Task<RadiusAreaSearchRes> RadiusAreaSearchAsync(RadiusAreaSearchReq req)
        {
            var url = QueryHelpers.AddQueryString(
                "place/v2/search",
                new Dictionary<string, string?>
                {
                    ["query"] = req.query,
                    ["location"] = req.latitude.ToString() + ',' + req.longitude.ToString(),
                    ["radius"] = "100000000",   // 直接到城市级搜索
                    ["radius_limit"] = "false",
                    ["scope"] = "2",
                    ["filter"] = "sort_name:distance|sort_rule:1",
                    ["coord_type"] = "2",
                    ["ret_coordtype"] = "gcj02ll",
                    ["page_size"] = "20",
                    ["output"] = "json",
                    ["ak"] = bdGeographyOptions.AK
                });

            var response = await httpClient.GetFromJsonAsync<RadiusAreaSearchRes>(url);
            //var res = await response.Content.ReadFromJsonAsync<ReverseGeocodingRes>();

            return response;
        }
    }
}
