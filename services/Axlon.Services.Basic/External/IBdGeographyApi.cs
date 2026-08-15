using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.BdGeography.Dto;

namespace Axlon.Services.Basic.External
{
    public interface IBdGeographyApi : IScopedDependency
    {
        /// <summary>
        /// 反地理编码
        /// </summary>
        public Task<ReverseGeocodingRes> ReverseGeocodingAsync(LongitudeLatitude location, string poi_types = "");

        /// <summary>
        /// 行政区域检索
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<ADAreaSearchRes> ADAreaSearchAsync(ADAreaSearchReq req);

        /// <summary>
        /// 圆形区域检索
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RadiusAreaSearchRes> RadiusAreaSearchAsync(RadiusAreaSearchReq req);
    }
}
