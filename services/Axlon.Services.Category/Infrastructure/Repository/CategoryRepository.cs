using Axlon.Framework.Abstractions.Tenants;
using Axlon.Framework.Data.Repository.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Category.Domain.IRepository;
using Axlon.Services.Contracts.Category;

namespace Axlon.Services.Category.Infrastructure.Repository
{
    public class CategoryRepository : BaseRepository<Categories>, ICategoryRepository
    {
        public CategoryRepository(IUnitOfWorkManage unitOfWorkManage, ITenantProvider tenantProvider) : base(unitOfWorkManage, tenantProvider)
        {
        }
    }
}
