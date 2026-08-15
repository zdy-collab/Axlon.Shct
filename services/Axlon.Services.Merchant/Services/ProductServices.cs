using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Contracts.Product;
using Axlon.Services.Contracts.Product.Dto;
using Axlon.Services.Merchant.Services.Interfaces;
using Mapster;

namespace Axlon.Services.Merchant.Services
{
    public class ProductServices(IBaseRepository<Products> repository) : BaseServices<Products>(repository), IProductServices
    {
        public async Task<List<ProductBasicDto>> ByIdsGetProductsAsync(List<long> productIds)
        {
            var res = (await base.Query(
            //    expression: x => new ProductBasicDto
            //{
            //    Id = x.Id,
            //    ImageFileId = x.ImageFileId,
            //    MerchantId = x.MerchantId,
            //    CategoryId = x.CategoryId,
            //    Name = x.Name,
            //    Price = x.Price,
            //    Stock = x.Stock,
            //    IsOn = x.IsOn
            //}, 
            x => productIds.Contains(x.Id)
            )).Adapt<List<ProductBasicDto>>();

            return res;
        }
    }
}
