using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Product.Dto;

namespace Axlon.Services.Order.ServiceInvocation.Merchant
{
    /// <summary>
    /// 商品服务
    /// </summary>
    public interface IProductClient: IScopedDependency
    {
        /// <summary>
        /// 根据商品Id获取信息
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        Task<List<ProductBasicDto>> ByIdsGetProductsAsync(List<long> productIds);
    }
}
