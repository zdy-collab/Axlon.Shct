using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Dto;

namespace Axlon.Services.Merchant.Services.Interfaces
{
    public interface IMerchantServices : IBaseServices<Merchants>
    {
        /// <summary>
        /// 根据经纬度获取附近商家列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<PageResponseModel<ByJwGetMerchantListRes>> ByJwGetMerchantListAsync(ByJwGetMerchantListReq req);

        /// <summary>
        /// 根据条件查询商家
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<QueryMerchantsRes> QueryMerchantsAsync(QueryMerchantsReq req);

        /// <summary>
        /// 根据Id获取商家信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MerchantsDto> ByIdGetMerchantsAsync(ByIdGetMerchantsReq req);


        /// <summary>
        /// 附近300米商家
        /// </summary>
        /// <returns></returns>
        Task<List<MerchantInfoDto>> NearbyMerchantQueryAsync(NearbyMerchantQueryReq req);

        /// <summary>
        /// 为你推荐商家
        /// </summary>
        /// <returns></returns>
        Task<List<MerchantInfoDto>> RecommendMerchantQueryAsync(RecommendMerchantQueryReq req);


        /// <summary>
        /// 商家列表
        /// </summary>
        /// <returns></returns>
        Task<List<MerchantInfoDto>> SearchMerchantQueryAsync(SearchMerchantQueryReq req);

        /// <summary>
        /// 小程序获取商家详情
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        Task<MerchantInfoDto> MiniGetMerchantDetailsAsync(MiniGetMerchantDetailsReq req);

        /// <summary>
        /// 获取商家基础信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="tableIds"></param>
        /// <returns></returns>
        Task<MerchantBasic_TableDto> GetMerchantBasicAsync(long merchantId, List<long> tableIds);
    }
}
