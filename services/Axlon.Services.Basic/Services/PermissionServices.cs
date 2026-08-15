using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// PermissionServices
    /// </summary>
    public class PermissionServices(IBaseRepository<Permission> repository) : BaseServices<Permission>(repository), IPermissionServices
    {
    }
}
