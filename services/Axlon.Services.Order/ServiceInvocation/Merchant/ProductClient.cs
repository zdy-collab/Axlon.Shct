using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Dto.Inner;
using Axlon.Services.Contracts.Product.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Base;

namespace Axlon.Services.Order.ServiceInvocation.Merchant
{
    public class ProductClient : IProductClient
    {
        private HttpClient httpClient;

        public ProductClient(IHttpClientFactory factory)
        {

            this.httpClient = factory.CreateClient(ServiceName.merchant.ToString());
        }
        public async Task<List<ProductBasicDto>> ByIdsGetProductsAsync(List<long> productIds)
        {
            var url = QueryHelpers.AddQueryString(
            InternalApiBaseAdr.InternalProduct + "/byIdsGetProducts",
            new Dictionary<string, string?>
            {
                ["productIds"] = string.Join(",",productIds)

            });
            var res = await httpClient.GetAsync<MessageModel<List<ProductBasicDto>>>(url);
            //var res = await httpClient.GetFromJsonAsync<MessageModel<List<ProductBasicDto>>>(url);

            return res.response;
        }
    }
}
