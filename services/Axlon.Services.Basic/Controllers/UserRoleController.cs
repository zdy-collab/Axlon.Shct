using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 用户角色关系
    /// </summary>
    [Produces("application/json")]
    [Route("api/basic/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UserRoleController : BaseApiController
    {
        private readonly ISysUserInfoServices _sysUserInfoServices;
        private readonly IUserRoleServices _userRoleServices;
        private readonly IRoleServices _roleServices;

        public UserRoleController(ISysUserInfoServices sysUserInfoServices, IUserRoleServices userRoleServices, IRoleServices roleServices)
        {
            _sysUserInfoServices = sysUserInfoServices;
            _userRoleServices = userRoleServices;
            _roleServices = roleServices;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<SysUserInfoDto>> AddUser(string loginName, string loginPwd)
        {
            var userInfo = await _sysUserInfoServices.SaveUserInfo(loginName, loginPwd);
            return new MessageModel<SysUserInfoDto>()
            {
                success = true,
                msg = "添加成功",
                response = userInfo.Adapt<SysUserInfoDto>()
            };
        }

        /// <summary>
        /// 新建Role
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<Role>> AddRole(string roleName)
        {
            return new MessageModel<Role>()
            {
                success = true,
                msg = "添加成功",
                response = await _roleServices.SaveRole(roleName)
            };
        }

        /// <summary>
        /// 新建用户角色关系
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<UserRole>> AddUserRole(long uid, long rid)
        {
            return new MessageModel<UserRole>()
            {
                success = true,
                msg = "添加成功",
                response = await _userRoleServices.SaveUserRole(uid, rid)
            };
        }
    }
}
