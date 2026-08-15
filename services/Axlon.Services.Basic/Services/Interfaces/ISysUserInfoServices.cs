using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;
using System.Linq.Expressions;

namespace Axlon.Services.Basic.IServices
{
    /// <summary>
    /// ISysUserInfoServices
    /// </summary>
    public interface ISysUserInfoServices : IBaseServices<SysUserInfo>
    {
        Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd);
        Task<string> GetUserRoleNameStr(string loginName, string loginPwd);

        /// <summary>
        /// 拓展修改
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="setColumns"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<SysUserInfo, bool>> whereExpression, Expression<Func<SysUserInfo, SysUserInfo>> setColumns);
    }
}
