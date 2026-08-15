using Axlon.Framework.Abstractions;
using Axlon.Framework.Authentication.Helpers;
using Axlon.Framework.Authentication.Policys;
using Axlon.Framework.Core.Helper;
using Axlon.Framework.Web.Controllers;
using Axlon.Framework.Web.OpenApi;
using Axlon.Services.Auth.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Axlon.Services.Auth.Controllers;

/// <summary>
/// 登录管理
/// </summary>
[Produces("application/json")]
[Route("api/auth")]
[Authorize]
public sealed class AuthController : BaseApiController
{
    private readonly ISysUserInfoServices _users;
    private readonly IAxlonJwtTokenService _tokens;
    private readonly AxlonJwtOptions _jwtOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ISysUserInfoServices users,
        IAxlonJwtTokenService tokens,
        IOptions<AxlonJwtOptions> jwtOptions,
        ILogger<AuthController> logger)
    {
        _users = users;
        _tokens = tokens;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    [HttpGet("test")]
    public string Test()
    {
        return "ok";
    }

    [HttpGet("token")]
    [AllowAnonymous]
    public async Task<MessageModel<string>> GetJwtStr(string name, string pass)
    {
        var roles = await _users.GetUserRoleNameStr(name, MD5Helper.MD5Encrypt32(pass));
        if (string.IsNullOrWhiteSpace(roles)) return Failed<string>("登录失败", 401);
        var user = (await _users.Query(item => item.LoginName == name && !item.IsDeleted)).FirstOrDefault();
        return user is null
            ? Failed<string>("登录失败", 401)
            : Success<string>(_tokens.Issue(new TokenModelJwt { Uid = user.Id, Role = roles, Name = user.LoginName, TenantId = user.TenantId }));
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public async Task<MessageModel<TokenInfoViewModel>> Login(string name = "", string pass = "")
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
            return Failed<TokenInfoViewModel>("用户名或密码不能为空", 400);

        var password = MD5Helper.MD5Encrypt32(pass);
        var user = (await _users.Query(item => item.LoginName == name && item.LoginPWD == password && !item.IsDeleted)).FirstOrDefault();
        if (user is null || !user.Enable) return Failed<TokenInfoViewModel>("认证失败", 401);
        var roles = await _users.GetUserRoleNameStr(name, password);
        var token = _tokens.Issue(new TokenModelJwt { Uid = user.Id, Role = roles, Name = user.LoginName, TenantId = user.TenantId });
        return Success(new TokenInfoViewModel
        {
            success = true,
            token = token,
            token_type = "Bearer",
            expires_in = _jwtOptions.ExpirationSeconds
        }, "获取成功");
    }

    [HttpPost("swgLogin")]
    [AllowAnonymous]
    public async Task<object> SwaggerLogin([FromBody] SwaggerLoginRequest request)
    {
        try
        {
            var result = await Login(request.name, request.pwd);
            var token = result.response?.token;
            if (!result.success || string.IsNullOrWhiteSpace(token)) return new { result = false };
            Response.GrantSwaggerAccess(token);
            return new { result = true, token };
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "OpenAPI login failed");
            return new { result = false };
        }
    }

    [HttpGet("refresh")]
    [AllowAnonymous]
    public async Task<MessageModel<TokenInfoViewModel>> RefreshToken(string token = "")
    {
        var model = _tokens.Read(token);
        if (model is null || model.Uid <= 0 || !_tokens.ValidateSignature(token))
            return Failed<TokenInfoViewModel>("Token 无效，请重新登录", 401);
        var user = await _users.QueryById(model.Uid);
        if (user is null || user.IsDeleted || !user.Enable) return Failed<TokenInfoViewModel>("认证失败", 401);
        var roles = await _users.GetUserRoleNameStr(user.LoginName, user.LoginPWD);
        var refreshed = _tokens.Issue(new TokenModelJwt { Uid = user.Id, Role = roles, Name = user.LoginName, TenantId = user.TenantId });
        return Success(new TokenInfoViewModel
        {
            success = true,
            token = refreshed,
            token_type = "Bearer",
            expires_in = _jwtOptions.ExpirationSeconds
        });
    }

    [HttpGet("logout")]
    [Authorize]
    public MessageModel<string> Logout() => Success("ok", "注销成功");

    [HttpGet("md5Password")]
    public string Md5Password(string password = "") => MD5Helper.MD5Encrypt32(password);
}

public sealed class SwaggerLoginRequest
{
    public string name { get; set; } = string.Empty;
    public string pwd { get; set; } = string.Empty;
}
