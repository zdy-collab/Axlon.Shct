using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Services.Basic.OutInput.Output.File;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Wechat.Dto;
using Azure.Core;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Axlon.Services.Basic.ServiceInvocation.File
{
    public class FileClient : IFileClient
    {
        private readonly HttpClient httpClient;
        private readonly IUser loginUser;

        public FileClient(IHttpClientFactory factory, IUser loginUser)
        {
            this.httpClient = factory.CreateClient(ServiceName.files.ToString());
            this.loginUser = loginUser;
        }

        public async Task<MessageModel<string>> CreatePromotionCodeAsync(CreatePromotionCodeReq req)
        {
            var apiUrl = InternalApiBaseAdr.InternalQrCodeBuild + "/createPromotionCode";

            var res = await httpClient.PostAsync<CreatePromotionCodeReq,MessageModel<string>>(apiUrl,req, loginUser.GetToken());

            //var request = new HttpRequestMessage(
            //    HttpMethod.Post,
            //    apiUrl);



            //request.Headers.Authorization =
            //new AuthenticationHeaderValue(
            //    "Bearer",
            //    loginUser.GetToken()
            //);

            //var response = await httpClient.SendAsync(request);

            //var message = await response.Content
            //    .ReadFromJsonAsync<MessageModel<string>>();

            return res;
        }

        /*        public async Task<FileMetadataOutput> InternalUploadAsync(string fileName, string visibility, byte[] bytes)
                {

                    var content = new ByteArrayContent(bytes);


                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    var apiUrl = QueryHelpers.AddQueryString(
                        "api/files/836471501127941/preview",
                        new Dictionary<string, string?>
                        {
                            ["fileName"] = fileName,
                            ["visibility"] = visibility
                        });

                    var response = await httpClient.PostAsync(apiUrl, content);

                    var res = await response.Content.ReadFromJsonAsync<MessageModel<FileMetadataOutput>>();
                    return res.response;
                }*/
    }
}
