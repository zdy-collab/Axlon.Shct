using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Product.Dto;
using Axlon.Services.Merchant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Merchant.Controllers.Internal
{
    [Route("api/merchant/internal/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InternalProductController : BaseApiController
    {
        private readonly IProductServices productServices;

        public InternalProductController(IProductServices productServices)
        {
            this.productServices = productServices;
        }

        /// <summary>
        /// 根据商品id集合获取信息
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        [HttpGet("byIdsGetProducts")]
        public async Task<MessageModel<List<ProductBasicDto>>> ByIdsGetProductsAsync([FromQuery]string productIds) 
        {
            var ids = productIds?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();

            return Success(data: await productServices.ByIdsGetProductsAsync(ids));
        }
    }
}
