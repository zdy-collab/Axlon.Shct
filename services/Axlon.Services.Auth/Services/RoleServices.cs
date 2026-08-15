using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Auth.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Auth.Services
{
    /// <summary>
    /// RoleServices
    /// </summary>
    public class RoleServices(IBaseRepository<Role> repository) : BaseServices<Role>(repository), IRoleServices
    {
        /// <summary>
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> SaveRole(string roleName)
        {
            Role role = new Role(roleName);
            Role model = new Role();
            var userList = await base.Query(a => a.Name == role.Name && a.Enabled);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(role);
                model = await base.QueryById(id);
            }

            return model;
        }

        public async Task<string> GetRoleNameByRid(int rid)
        {
            return ((await base.QueryById(rid))?.Name);
        }
    }
}
