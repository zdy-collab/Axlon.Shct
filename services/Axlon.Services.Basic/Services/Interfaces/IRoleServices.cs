using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.IServices
{
    /// <summary>
    /// IRoleServices
    /// </summary>
    public interface IRoleServices : IBaseServices<Role>
    {
        Task<Role> SaveRole(string roleName);
        Task<string> GetRoleNameByRid(int rid);
    }
}
