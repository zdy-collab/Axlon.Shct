using Axlon.Framework.Core.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Category.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class SwgLoginController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SwgLoginController> _logger;

        public SwgLoginController(IHttpClientFactory httpClientFactory, ILogger<SwgLoginController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// 转调 Auth 服务完成 Swagger 登录，写入 Basic 本地 Session。
        /// </summary>
        [HttpPost]
        [Route("/api/swgLogin")]
        public async Task<dynamic> SwgLogin([FromBody] SwaggerLoginRequest req)
        {
            if (req is null)
            {
                return new { result = false };
            }

            try
            {
                var client = _httpClientFactory.CreateClient("auth-service");
                var resp = await client.PostAsJsonAsync("/api/auth/swgLogin", req);
                if (!resp.IsSuccessStatusCode)
                {
                    return new { result = false };
                }

                var data = await resp.Content.ReadFromJsonAsync<SwgLoginResponse>();
                if (data?.result == true && !string.IsNullOrEmpty(data.token))
                {
                    HttpContext.SuccessSwagger();
                    HttpContext.SuccessSwaggerJwt(data.token);
                    return new { result = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Basic SwgLogin 转调 Auth 失败");
            }

            return new { result = false };
        }
    }

    /// <summary>
    /// Swagger 登录请求体（与 Auth 服务 SwaggerLoginRequest 对齐）。
    /// </summary>
    public record SwaggerLoginRequest(string name, string pwd);

    /// <summary>
    /// Auth 服务 SwgLogin 成功响应。
    /// </summary>
    internal record SwgLoginResponse(bool result, string? token);
}
