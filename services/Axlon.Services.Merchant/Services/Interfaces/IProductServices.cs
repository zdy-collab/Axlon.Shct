using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Product;
using Axlon.Services.Contracts.Product.Dto;

namespace Axlon.Services.Merchant.Services.Interfaces
{
    public interface IProductServices : IBaseServices<Products>
    {
        /// <summary>
        /// 根据商品id集合获取信息
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        Task<List<ProductBasicDto>> ByIdsGetProductsAsync(List<long> productIds);
    }
}
