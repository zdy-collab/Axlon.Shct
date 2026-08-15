using Axlon.Framework.Core;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using System.Linq.Expressions;

namespace Axlon.Services.Basic.Services
{
    /// <summary>
    /// SysUserInfoServices
    /// </summary>
    public class SysUserInfoServices : BaseServices<SysUserInfo>, ISysUserInfoServices
    {
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IBaseRepository<Role> _roleRepository;

        public SysUserInfoServices(
            IBaseRepository<SysUserInfo> repository,
            IBaseRepository<UserRole> userRoleRepository,
            IBaseRepository<Role> roleRepository) : base(repository)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd)
        {
            SysUserInfo sysUserInfo = new SysUserInfo(loginName, loginPwd);
            SysUserInfo model = new SysUserInfo();
            var userList = await base.Query(a => a.LoginName == sysUserInfo.LoginName && a.LoginPWD == sysUserInfo.LoginPWD);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(sysUserInfo);
                model = await base.QueryById(id);
            }

            return model;
        }

        /// <summary>
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        {
            string roleName = "";
            var user = (await base.Query(a => a.LoginName == loginName && a.LoginPWD == loginPwd)).FirstOrDefault();
            var roleList = await _roleRepository.Query(a => a.IsDeleted == false);
            if (user != null)
            {
                var userRoles = await _userRoleRepository.Query(ur => ur.UserId == user.Id);
                if (userRoles.Count > 0)
                {
                    var arr = userRoles.Select(ur => ur.RoleId.ObjToString()).ToList();
                    var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));

                    roleName = string.Join(',', roles.Select(r => r.Name).ToArray());
                }
            }
            return roleName;
        }

        public Task<int> UpdateUserFirstOrderCompletedAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Expression<Func<SysUserInfo,bool>> whereExpression, Expression<Func<SysUserInfo, SysUserInfo>> setColumns)
        {
            return base.Db.Updateable<SysUserInfo>()
                .SetColumns(setColumns)
                .Where(whereExpression)
                .ExecuteCommandAsync();
        }
    }
}
