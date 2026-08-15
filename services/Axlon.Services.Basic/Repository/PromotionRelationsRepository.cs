using Axlon.Framework.Abstractions.Tenants;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.Repository.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Basic.Repository.Interfaces;
using Axlon.Services.Contracts.Promotion;

namespace Axlon.Services.Basic.Repository
{
    public class PromotionRelationsRepository : BaseRepository<PromotionRelations>, IPromotionRelationsRepository
    {
        public PromotionRelationsRepository(IUnitOfWorkManage unitOfWorkManage, ITenantProvider tenantProvider, IUser user) : base(unitOfWorkManage, tenantProvider, user)
        {
        }

        public Task<List<PromotionRelations>> ByParentIdGetParent(long parentId)
        {
            return base.Db.Queryable<PromotionRelations>().ToParentListAsync(x => x.ParentId, parentId);
        }
    }
}
