using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Category.Dto;

namespace Axlon.Services.Merchant.ServiceInvocation.Category
{
    public interface ICategoryClient : IScopedDependency
    {
        /// <summary>
        /// 根据id集合获取子id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<long>> ByIdsGetChidrenIdsAsync(List<long> ids);

        /// <summary>
        /// 根据主键集合获取信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<CategoryNodeDto>> ByIdsGetCategoriesAsync(List<long> ids);
    }
}
