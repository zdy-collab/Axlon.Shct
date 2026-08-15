using Axlon.Framework.Data.IRepository.Base;
using Axlon.Services.Contracts.Models;

namespace Axlon.Services.Basic.Repository
{
    /// <summary>
    /// IRoleModulePermissionRepository
    /// </summary>
    public interface IRoleModulePermissionRepository : IBaseRepository<RoleModulePermission>
    {
        Task<List<TestMuchTableResult>> QueryMuchTable();
        Task<List<RoleModulePermission>> RoleModuleMaps();
        Task<List<RoleModulePermission>> GetRMPMaps();
        /// <summary>
        /// 批量更新菜单与接口的关系
        /// </summary>
        /// <param name="permissionId">菜单主键</param>
        /// <param name="moduleId">接口主键</param>
        /// <returns></returns>
        Task UpdateModuleId(long permissionId, long moduleId);
    }
}
