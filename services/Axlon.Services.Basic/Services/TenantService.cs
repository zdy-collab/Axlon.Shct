using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.Services;

public class TenantService : BaseServices<SysTenant>, ITenantService
{
    private readonly IUnitOfWorkManage _uowManager;

    public TenantService(IBaseRepository<SysTenant> repository, IUnitOfWorkManage uowManage) : base(repository)
    {
        this._uowManager = uowManage;
    }

    public async Task SaveTenant(SysTenant tenant)
    {
        bool initDb = tenant.Id == 0;
        using (var uow = _uowManager.CreateUnitOfWork())
        {
            //TODO
            //tenant.ApplyDefaultConfig();

            if (tenant.Id == 0)
            {
                await Db.Insertable(tenant).ExecuteReturnSnowflakeIdAsync();
            }
            else
            {
                var oldTenant = await QueryById(tenant.Id);
                if (oldTenant.Connection != tenant.Connection)
                {
                    initDb = true;
                }

                await Db.Updateable(tenant).ExecuteCommandAsync();
            }

            uow.Commit();
        }

        if (initDb)
        {
            await InitTenantDb(tenant);
        }
    }

    public async Task InitTenantDb(SysTenant tenant)
    {
        // Tenant DB initialization is handled by the framework's seed infrastructure.
        // This method can be extended with custom tenant-specific seeding logic if needed.
        await Task.CompletedTask;
    }
}
