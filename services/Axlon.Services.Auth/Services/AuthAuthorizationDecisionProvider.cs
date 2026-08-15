using Axlon.Framework.Authentication.Authorization;
using Axlon.Services.Auth.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Axlon.Services.Auth.Services;

/// <summary>
/// Auth 服务本地权限数据适配器。ASP.NET 授权流程由框架包统一处理。
/// </summary>
public sealed class AuthAuthorizationDecisionProvider : IAxlonAuthorizationDecisionProvider
{
    private readonly ISysUserInfoServices _users;
    private readonly IRoleModulePermissionServices _permissions;

    public AuthAuthorizationDecisionProvider(
        ISysUserInfoServices users,
        IRoleModulePermissionServices permissions)
    {
        _users = users;
        _permissions = permissions;
    }

    public async ValueTask<AxlonAuthorizationDecision> DecideAsync(
        ClaimsPrincipal principal,
        AxlonAuthorizationDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return AxlonAuthorizationDecision.Deny("unauthenticated");
        }

        // 游客
        string? isVisitorValue = principal.FindFirstValue("isVisitor");
        if (bool.TryParse(isVisitorValue, out bool isVisitor) && isVisitor == true) 
        {
            return AxlonAuthorizationDecision.Allow();
        }

        string? userIdValue = principal.FindFirstValue(JwtRegisteredClaimNames.Jti)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdValue, out long userId) || userId <= 0)
        {
            return AxlonAuthorizationDecision.Deny("invalid_user_id");
        }

        var user = await _users.QueryById(userId);

        string[] roles = principal.FindAll(ClaimTypes.Role)
            .Concat(principal.FindAll("role"))
            .Select(claim => claim.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (user is null || user.IsDeleted || !user.Enable)
        {
            return AxlonAuthorizationEvaluator.Decide(false, roles, [], request.Path);
        }

        // 小程序用户临时通过全部校验
        if(roles.Contains("Mini")) return AxlonAuthorizationDecision.Allow();

        var mappings = await _permissions.RoleModuleMaps();
        IEnumerable<AxlonRoleRoutePermission> routePermissions = mappings
            .Where(mapping => mapping.IsDeleted != true
                && mapping.Role is not null
                && mapping.Module is not null)
            .Select(mapping => new AxlonRoleRoutePermission(
                mapping.Role.Name,
                mapping.Module.LinkUrl));

        return AxlonAuthorizationEvaluator.Decide(
            true,
            roles,
            routePermissions,
            request.Path);
    }
}
