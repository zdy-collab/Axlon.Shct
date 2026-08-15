using Axlon.Framework.Web.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public sealed class SwgLoginController(
       IHttpClientFactory httpClientFactory,
       ILogger<SwgLoginController> logger) : ControllerBase
    {
        [HttpPost("/api/swgLogin")]
        public async Task<object> Login([FromBody] SwaggerLoginRequest request)
        {
            try
            {
                var client = httpClientFactory.CreateClient("auth");
                using var response = await client.PostAsJsonAsync("/api/auth/swgLogin", request, HttpContext.RequestAborted);
                if (!response.IsSuccessStatusCode) return new { result = false };

                var login = await response.Content.ReadFromJsonAsync<SwaggerLoginResponse>(HttpContext.RequestAborted);
                if (login is not { result: true } || string.IsNullOrWhiteSpace(login.token))
                    return new { result = false };

                Response.GrantSwaggerAccess(login.token);
                return new { result = true, token = login.token };
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Auth service OpenAPI login failed");
                return new { result = false };
            }
        }
    }

    public sealed record SwaggerLoginRequest(string name, string pwd);
    public sealed record SwaggerLoginResponse(bool result, string? token);
}
