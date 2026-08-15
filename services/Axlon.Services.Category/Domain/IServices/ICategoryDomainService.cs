using Axlon.Services.Contracts.Category.Dto;

namespace Axlon.Services.Category.Domain.IServices
{
    /// <summary>
    /// 全平台品类树
    /// </summary>
    public interface ICategoryDomainService
    {
        /// <summary>
        /// 获取完整树
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryNodeDto>> GetTreeAsync();

        /// <summary>
        /// 根据父节点获取子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<CategoryNodeDto>> GetChildrenAsync(long id);

        /// <summary>
        /// 获取顶部节点
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryNodeDto>> GetTopNodeAsync();

        /// <summary>
        /// 根据id集合获取子id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<long>> ByIdsGetChidrenIdsAsync(List<long> ids);
    }
}
