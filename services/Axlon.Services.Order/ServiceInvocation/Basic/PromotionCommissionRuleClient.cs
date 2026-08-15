using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Base;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    public class PromotionCommissionRuleClient : IPromotionCommissionRuleClient
    {
        private readonly HttpClient httpClient;

        public PromotionCommissionRuleClient(IHttpClientFactory factory)
        {
            this.httpClient = factory.CreateClient(ServiceName.basic.ToString());
        }

        public Task<MessageModel<PromotionCommissionRulesBasicDto>> ByMerchantIdGetPCRuleAsync(long merchantId)
        {
            var apiUrl = $"{InternalApiBaseAdr.InternalPromotionCommissionRule}/byMerchantIdGetPCRule?merchantId={merchantId.ToString()}";
            //var url = QueryHelpers.AddQueryString(
            //    InternalApiBaseAdr.InternalPromotionCommissionRule + "/byMerchantIdGetPCRule",
            //    new Dictionary<string, string?>
            //    {
            //        ["merchantId"] = merchantId.ToString()

            //    });

            var res = httpClient.GetAsync<MessageModel<PromotionCommissionRulesBasicDto>>(apiUrl);

            return res;
        }
    }
}
