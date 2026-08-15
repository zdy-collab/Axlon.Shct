using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.GroupBuy.Dto;

namespace Axlon.Services.Merchant.Services.Interfaces
{
    public interface IFeaturedGroupBuyServices : IBaseServices<FeaturedGroupBuys>
    {
        /// <summary>
        /// 获取有效期内的团购活动
        /// </summary>
        /// <returns></returns>
        Task<List<FeaturedGroupBuyInfoDto>> GetValidGroupBuyAsync();
    }
}
