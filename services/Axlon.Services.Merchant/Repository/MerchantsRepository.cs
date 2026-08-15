using Axlon.Framework.Abstractions.Tenants;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.Repository.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Contracts.Merchant;

namespace Axlon.Services.Merchant.Repository
{
    public class MerchantsRepository : BaseRepository<Merchants>, IMerchantsRepository
    {
        public MerchantsRepository(IUnitOfWorkManage unitOfWorkManage, ITenantProvider tenantProvider, IUser user) : base(unitOfWorkManage, tenantProvider, user)
        {
        }

        public Task<List<MerchantCategoryConfig>> ByCategoryIdsGetMerchantIdsAsync(List<long> ids)
        {
            return Db.Queryable<MerchantCategoryConfig>().Where(x => ids.Contains(x.CategoryId)).ToListAsync();
        }

        //public Task<List<Merchants>> MerchantsIncludesAsync(Expression<Func<MerchantCategoryConfig,bool>> mccWhere)
        //{
        //    //Db.Queryable<Merchants>()
        //    //    .Includes(x => x.MerchantCategoryConfigs)
        //    //    .Where(x => x.MerchantCategoryConfigs.Where(mccWhere))
        //    throw new NotImplementedException();
        //}
    }
}
