using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// DepartmentServices
    /// </summary>
    public class DepartmentServices(IBaseRepository<Department> repository) : BaseServices<Department>(repository), IDepartmentServices
    {
    }
}
