using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.BdGeography;
using Axlon.Services.Contracts.Category.Dto;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Merchant.Dto.Inner;
using Microsoft.AspNetCore.WebUtilities;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Order.ServiceInvocation.Merchant
{
    public class MerchantClient : IMerchantClient
    {
        private HttpClient httpClient;

        public MerchantClient(IHttpClientFactory factory)
        {

            this.httpClient = factory.CreateClient(ServiceName.merchant.ToString());
        }
        //public async Task<MerchantForOrderDetailsDto> ByMerchantIdForOrderDetailsAsync(long merchantId, long? merchantTableId)
        //{
        //    var url = QueryHelpers.AddQueryString(
        //    "api/merchant/MerchantInternal/byMerchantIdForOrderDetails",
        //    new Dictionary<string, string?>
        //    {
        //        ["merchantId"] = merchantId.ToString(),
        //        ["merchantTableId"] = merchantTableId != null ? merchantTableId.ToString() : null

        //    });

        //    var res = await httpClient.GetFromJsonAsync<MessageModel<MerchantForOrderDetailsDto>>(url);

        //    return res.response;
        //}

        public async Task<MerchantBasic_TableDto> GetMerchantBasicAsync(long merchantId, List<long>? tableIds = null)
        {
            var url = QueryHelpers.AddQueryString(
                InternalApiBaseAdr.InternalMerchant + "/getMerchantBasic",
                new Dictionary<string, string?>
                {
                    ["merchantId"] = merchantId.ToString(),
                    ["tableIds"] = tableIds != null ? string.Join(",", tableIds) : null

                });

            var res = await httpClient.GetAsync<MessageModel<MerchantBasic_TableDto>>(url);
            //var res = await httpClient.GetFromJsonAsync<MessageModel<MerchantBasic_TableDto>>(url);

            return res.response;
        }
    }
}
