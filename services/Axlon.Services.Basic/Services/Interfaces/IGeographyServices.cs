using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.BdGeography.Dto;

namespace Axlon.Services.Basic.IServices
{
    public interface IGeographyServices
    {
        public Task<List<AddressInfoDto>> AddressSearchAsync(AddressSearchReq req);

        /// <summary>
        /// 根据经纬度获取附近地名
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Task<string> ByLocationGetNameAsync(LongitudeLatitude location);
    }
}
