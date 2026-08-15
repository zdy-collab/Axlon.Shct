using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    public class UserClient : IUserClient
    {
        private readonly HttpClient httpClient;
        private readonly IUser loginUser;


        public UserClient(IHttpClientFactory factory, IUser loginUser)
        {
            this.httpClient = factory.CreateClient(ServiceName.basic.ToString());
            this.loginUser = loginUser;
        }

        public async Task<long?> GetPromotionIdAsync()
        {
            var apiUrl = InternalApiBaseAdr.InternalUser + "/getPromotionIdAsync";

            var msg = await httpClient.GetAsync<MessageModel<long?>>(url: apiUrl,token: loginUser.GetToken());

            if (msg.success != true) throw new Exception("获取用户推广人失败！");

            return msg.response;
        }
    }
}
