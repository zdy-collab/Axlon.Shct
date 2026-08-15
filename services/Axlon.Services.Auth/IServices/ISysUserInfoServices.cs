using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;
using System.Linq.Expressions;

namespace Axlon.Services.Auth.IServices
{
    /// <summary>
    /// ISysUserInfoServices
    /// </summary>
    public interface ISysUserInfoServices : IBaseServices<SysUserInfo>
    {
        Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd);
        Task<string> GetUserRoleNameStr(string loginName, string loginPwd);

        /// <summary>
        /// 根据openId获取用户角色
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<string> GetUserRoleNameStr(string openId);

        /// <summary>
        /// 根据用户Id获取用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetUserRoleNameStr(long userId);

        Task<bool> AnyAsync(Expression<Func<SysUserInfo, bool>> whereExpression);
    }
}
