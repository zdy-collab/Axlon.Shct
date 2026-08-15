using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.BdGeography.Helper;

namespace Axlon.Services.Contracts.BdGeography.Dto
{
    #region new

    /// <summary>
    /// 地址信息Dto
    /// </summary>
    public class AddressInfoDto
    {
        /// <summary>
        /// 详细地址
        /// </summary>
        public string addr { get; set; }

        /// <summary>
        /// 地址名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public string mater { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
    }

    #endregion

    #region newReq

    public class AddressSearchReq : LongitudeLatitude
    {
        /// <summary>
        /// 查询参数
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// 行政区域
        /// </summary>
        public string? region { get; set; }

        public int radius { get; set; } = 5000;

        /// <summary>
        /// 0：附近地标推荐，1：圆形检索附近地标，2：行政区域检索地标
        /// </summary>
        public AddressSearchReqQueryType queryType { get; set; }
    }

    public enum AddressSearchReqQueryType
    {
        附近地标推荐 = 0,
        圆形检索附近地标 = 1,
        行政区域检索地标 = 2
    }



    #endregion

    #region req

    public class ReverseGeocodingReq : LongitudeLatitude
    {
    }

    public record ADAreaSearchReq(string query, string region, double longitude, double latitude);
    #endregion

    #region res

    /// <summary>
    /// 附近地址
    /// </summary>
    public class NearbyAddressRes
    {
        /// <summary>
        /// 详细地址
        /// </summary>
        public string addr { get; set; }

        /// <summary>
        /// 地址名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public string mater { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
    }


    /// <summary>
    /// 百度API全球逆地理编码响应
    /// </summary>
    public class ReverseGeocodingRes
    {
        /// <summary>
        /// 返回结果状态码
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 逆地理编码结果
        /// </summary>
        public GeocodingResult result { get; set; }
    }

    /// <summary>
    /// 逆地理编码结果
    /// </summary>
    public class GeocodingResult
    {
        public GeocodingLocation location { get; set; }

        public string formatted_address { get; set; }

        public GeocodingEdz edz { get; set; }

        public string business { get; set; }

        public List<GeocodingBusinessInfo> business_info { get; set; }

        public GeocodingAddressComponent addressComponent { get; set; }

        public List<GeocodingPoi> pois { get; set; }

        public List<GeocodingPoiRegion> poiRegions { get; set; }

        public string sematic_description { get; set; }

        public string formatted_address_poi { get; set; }

        public int cityCode { get; set; }
    }


    public class GeocodingLocation
    {
        public double lng { get; set; }

        public double lat { get; set; }
    }


    public class GeocodingEdz
    {
        public string name { get; set; }
    }


    public class GeocodingBusinessInfo
    {
        public string name { get; set; }

        public GeocodingLocation location { get; set; }

        public int adcode { get; set; }

        public int distance { get; set; }

        public string direction { get; set; }
    }


    public class GeocodingAddressComponent
    {
        public string country { get; set; }

        public int country_code { get; set; }

        public string country_code_iso { get; set; }

        public string country_code_iso2 { get; set; }

        public string province { get; set; }

        public string city { get; set; }

        public int city_level { get; set; }

        public string district { get; set; }

        public string town { get; set; }

        public string town_code { get; set; }

        public string distance { get; set; }

        public string direction { get; set; }

        public string adcode { get; set; }

        public string street { get; set; }

        public string street_number { get; set; }
    }


    public class GeocodingPoi : BaiduPoiInfo
    {
        public override string addr { get; set; }

        public string cp { get; set; }

        public string direction { get; set; }

        public override string distance { get; set; }

        public override string name { get; set; }

        public override string poiType { get; set; }

        public GeocodingPoint point { get; set; }

        public override string tag { get; set; }

        public string tel { get; set; }

        public string uid { get; set; }

        public string zip { get; set; }

        public override string popularity_level { get; set; }

        public GeocodingParentPoi parent_poi { get; set; }
    }


    public class GeocodingPoint
    {
        public double x { get; set; }

        public double y { get; set; }
    }


    public class GeocodingParentPoi
    {
        public string name { get; set; }

        public string tag { get; set; }

        public string addr { get; set; }

        public GeocodingPoint point { get; set; }

        public string direction { get; set; }

        public string distance { get; set; }

        public string uid { get; set; }

        public string popularity_level { get; set; }
    }


    public class GeocodingPoiRegion
    {
        public string direction_desc { get; set; }

        public string name { get; set; }

        public string tag { get; set; }

        public string uid { get; set; }

        public string distance { get; set; }
    }

    #endregion

    #region dto

    /// <summary>
    /// POI排序结果
    /// </summary>
    public class PoiRanking
    {
        public GeocodingPoi Poi { get; set; }
        public double FinalWeight { get; set; }
        public double CategoryWeight { get; set; }
        public double DistanceScore { get; set; }
        public double PopularityScore { get; set; }
    }
    #endregion
}
