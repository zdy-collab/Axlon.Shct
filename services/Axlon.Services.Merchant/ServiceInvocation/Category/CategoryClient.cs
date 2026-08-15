using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Category.Dto;
using System.Text;
using System.Text.Json;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Merchant.ServiceInvocation.Category
{
    public class CategoryClient : ICategoryClient
    {
        private HttpClient httpClient;

        public CategoryClient(IHttpClientFactory factory)
        {

            this.httpClient = factory.CreateClient(ServiceName.basic.ToString());

        }

        public async Task<List<long>> ByIdsGetChidrenIdsAsync(List<long> ids)
        {
            var idsString = String.Join(",", ids);
            var apiUrl = $"{InternalApiBaseAdr.InternalCategory}/byIdsGetChidrenIds?ids={idsString}";
            var res = await httpClient.GetAsync<MessageModel<List<long>>>(apiUrl);

            return res.response;
            //var response = await httpClient.GetAsync("/health");
            //var json = JsonSerializer.Serialize(ids);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var res = await httpClient.PostAsync(InternalApiBaseAdr.InternalCategory + "/byIdsGetChidrenIds", content);
            //// 处理响应
            ////var responseContent = await response.Content.ReadAsStringAsync();
            //var resJson = await res.Content.ReadAsStringAsync();
            //var messageModel = JsonSerializer.Deserialize<MessageModel<List<string>>>(await res.Content.ReadAsStringAsync());

            //var result = messageModel.response.Select(x => long.Parse(x)).ToList();
            //return result;
        }

        public async Task<List<CategoryNodeDto>> ByIdsGetCategoriesAsync(List<long> ids)
        {
            var query = string.Join("&", ids.Select(id => $"ids={id}"));
            var apiUrl = $"{InternalApiBaseAdr.InternalCategory}/byIdsGetCategories?{query}";
            var res = await httpClient.GetAsync<MessageModel<List<CategoryNodeDto>>>(apiUrl);

            return res.response;
        }
    }
}
