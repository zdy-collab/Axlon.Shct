using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Order.Services.Interfaces
{
    /// <summary>
    /// ISysUserInfoServices
    /// </summary>
    public interface ISysUserInfoServices : IBaseServices<SysUserInfo>
    {
        Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd);
        Task<string> GetUserRoleNameStr(string loginName, string loginPwd);
    }
}
