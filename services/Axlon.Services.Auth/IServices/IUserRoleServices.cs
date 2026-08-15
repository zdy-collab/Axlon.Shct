using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Auth.IServices
{
    /// <summary>
    /// IUserRoleServices
    /// </summary>
    public interface IUserRoleServices : IBaseServices<UserRole>
    {
        Task<UserRole> SaveUserRole(long uid, long rid);
        Task<int> GetRoleIdByUid(long uid);
    }
}
