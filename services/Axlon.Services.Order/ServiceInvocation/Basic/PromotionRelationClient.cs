using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Wechat.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    public class PromotionRelationClient : IPromotionRelationClient
    {
        private readonly HttpClient httpClient;

        public PromotionRelationClient(IHttpClientFactory factory)
        {
            this.httpClient = factory.CreateClient(ServiceName.basic.ToString());
        }

        public async Task<MessageModel<string>> BindPromotionRelationAsync([FromBody] BindPromotionRelationReq req)
        {
            var apiUrl = $"{InternalApiBaseAdr.InternalPromotionRelation}/bindPromotionRelation";

            //var response = await httpClient.PostAsync(apiUrl, new StringContent(
            //    JsonSerializer.Serialize(req),
            //    Encoding.UTF8,
            //    "application/json"
            //));

            //var message = await response.Content.ReadFromJsonAsync<MessageModel<string>>();

            var res = await httpClient.PostAsync<BindPromotionRelationReq,MessageModel<string>>(apiUrl,req);

            return res;
        }

        public async Task<MessageModel<List<PromotionRelationsBasicDto>>> ByUserIdGetBasicInfoAsync(long userId)
        {
            //var url = QueryHelpers.AddQueryString(
            //    InternalApiBaseAdr.InternalPromotionRelation + "/byUserIdGetBasicInfo",
            //    new Dictionary<string, string?>
            //    {
            //        ["userId"] = userId.ToString()

            //    });

            //var res = await httpClient.GetFromJsonAsync<MessageModel<List<PromotionRelationsBasicDto>>>(url);

            var apiUrl = $"{InternalApiBaseAdr.InternalPromotionRelation}/byUserIdGetBasicInfo?userId={userId.ToString()}";

            var res = await httpClient.GetAsync<MessageModel<List<PromotionRelationsBasicDto>>>(apiUrl);

            return res;
        }
    }
}
