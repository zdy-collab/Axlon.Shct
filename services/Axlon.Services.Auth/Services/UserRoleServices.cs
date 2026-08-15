using Axlon.Framework.Core;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Auth.IServices;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Auth.Services
{
    /// <summary>
    /// UserRoleServices
    /// </summary>
    public class UserRoleServices(IBaseRepository<UserRole> repository) : BaseServices<UserRole>(repository), IUserRoleServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public async Task<UserRole> SaveUserRole(long uid, long rid)
        {
            UserRole userRole = new UserRole(uid, rid);

            UserRole model = new UserRole();
            var userList = await base.Query(a => a.UserId == userRole.UserId && a.RoleId == userRole.RoleId);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(userRole);
                model = await base.QueryById(id);
            }

            return model;
        }

        public async Task<int> GetRoleIdByUid(long uid)
        {
            return ((await base.Query(d => d.UserId == uid)).OrderByDescending(d => d.Id).LastOrDefault()?.RoleId).ObjToInt();
        }
    }
}
