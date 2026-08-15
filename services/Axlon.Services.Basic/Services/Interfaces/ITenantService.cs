using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.IServices;

public interface ITenantService : IBaseServices<SysTenant>
{
    public Task SaveTenant(SysTenant tenant);

    public Task InitTenantDb(SysTenant tenant);
}
