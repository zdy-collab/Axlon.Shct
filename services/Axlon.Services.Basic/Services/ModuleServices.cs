using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// ModuleServices
    /// </summary>
    public class ModuleServices(IBaseRepository<Modules> repository) : BaseServices<Modules>(repository), IModuleServices
    {
    }
}
