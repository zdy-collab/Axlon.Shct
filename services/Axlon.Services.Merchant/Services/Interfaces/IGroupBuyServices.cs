using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.GroupBuy.Dto;

namespace Axlon.Services.Merchant.Services.Interfaces
{
    /// <summary>
    /// 团购商品
    /// </summary>
    public interface IGroupBuyServices : IBaseServices<GroupBuys>
    {
        /// <summary>
        /// 根据商家Id获取团购信息
        /// </summary>
        /// <returns></returns>
        public Task<PageResponseModel<GroupBuyInfoDto>> ByMerchantIdGetInfoAsync(QueryPage queryPage, long merchantId);
    }
}
