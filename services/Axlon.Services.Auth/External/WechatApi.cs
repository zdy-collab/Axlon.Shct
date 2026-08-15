using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Wechat;
using Axlon.Services.Contracts.Wechat.Dto;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Axlon.Services.Auth.External
{
    public class WechatApi : IWechatApi
    {
        private HttpClient httpClient;
        private readonly WechatOptions wechatOptions;
        private readonly IDistributedCache cache;
        //private readonly IConnectionMultiplexer redis;

        public WechatApi(IHttpClientFactory factory, IOptions<WechatOptions> wcOptions, IDistributedCache cache)
        {
            httpClient = factory.CreateClient(ServiceName.wechat.ToString());
            this.wechatOptions = wcOptions.Value;
            //this.redis = redis;
            this.cache = cache;
        }

        public async Task<WcTokenRes> GetTokenAsync()
        {
            //var redisDb = redis.GetDatabase();

            var req = new WcTokenReq(wechatOptions);

            var tokenKey = "wechat:access_token";

            var token = await cache.GetStringAsync("wechat:access_token");

            if (!string.IsNullOrEmpty(token)) return new WcTokenRes() { access_token = token };

            // 如果token过期，同一时间进来大量请求重新获取token，会有缓存击穿问题
            var lockKey = "wechat:access_token_lock";   // 分布式锁

            var lockValue = Guid.NewGuid().ToString();


            //// 获取锁 TODO
            //var locked = await redisDb.StringSetAsync(lockKey, lockValue, TimeSpan.FromSeconds(10), When.NotExists);
            bool locked = true;
            if (locked)
            {
                // 第二次检查
                token = await cache.GetStringAsync(tokenKey);
                //if (string.IsNullOrEmpty(req.grant_type)) req.grant_type = "client_credential";
                if (!string.IsNullOrEmpty(token)) return new WcTokenRes() { access_token = token };

                #region 请求微信API获取token

                var apiUrl = $"cgi-bin/token?appid={req.appid}&secret={req.secret}&grant_type={req.grant_type}";

                var res = await httpClient.GetFromJsonAsync<WcTokenRes>(apiUrl);

                token = res == null ? "" : res.access_token;

                #endregion

                await cache.SetStringAsync(
                "wechat:access_token",
                token,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                      TimeSpan.FromSeconds(res.expires_in - 300)
                });
            }

            //await cache.SetStringAsync(
            //"wechat:access_token",
            //token,
            //new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow =
            //      TimeSpan.FromSeconds(7000)
            //});

            return new WcTokenRes() { access_token = token };
        }

        public async Task<WcPhoneRes> GetPhoneInfoAsync(WcPhoneReq req)
        {
            //从缓存中取出来，如果过期了则再获取
            var tokenRes = await GetTokenAsync();

            var apiUrl = $"wxa/business/getuserphonenumber?access_token={tokenRes.access_token}";

            var response = await httpClient.PostAsync(apiUrl, new StringContent(
                JsonSerializer.Serialize(req),
                Encoding.UTF8,
                "application/json"
            ));

            var responseJson = await response.Content.ReadFromJsonAsync<WcPhoneRes>();
            //var res = await response.Content.ReadFromJsonAsync<WcPhoneRes>();

            return responseJson;
        }

        public async Task<WcLoginRes> LoginAsync(string js_code)
        {
            var req = new WcLoginReq(wechatOptions, js_code);

            var apiUrl = $"sns/jscode2session?appid={req.appid}&secret={req.secret}&js_code={req.js_code}&grant_type={req.grant_type}";

            //var wcTokenRes = wechatTokenService.GetTokenAsync();

            var res = await httpClient.GetFromJsonAsync<WcLoginRes>(apiUrl);

            return res;
        }
    }
}
