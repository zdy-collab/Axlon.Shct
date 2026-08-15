using Axlon.Framework.Data.IRepository.Base;
using Axlon.Services.Contracts.Merchant;

namespace Axlon.Services.Merchant.Repository
{
    /// <summary>
    /// 商家仓储
    /// </summary>
    public interface IMerchantsRepository : IBaseRepository<Merchants>
    {
        /// <summary>
        /// 根据品类Id集合获取所属商家Id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<MerchantCategoryConfig>> ByCategoryIdsGetMerchantIdsAsync(List<long> ids);

        //Task<List<Merchants>> MerchantsIncludesAsync();
    }
}
