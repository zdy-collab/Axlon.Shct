using Axlon.Framework.Authentication.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Axlon.Services.Contracts.Extensions
{
    public static class IAxlonJwtTokenServiceExtensions
    {
        public static string Issue2(this IAxlonJwtTokenService axlonJwtTokenService, AxlonJwtOptions _options, TokenModelJwt tokenModel, bool isVisitor,long? visitorId = null) 
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var _secret = string.Empty;

            if (!string.IsNullOrWhiteSpace(_options.SecretFile) && File.Exists(_options.SecretFile))
            {
                _secret = File.ReadAllText(_options.SecretFile).Trim();
            }
            else
            {
                _secret = _options.Secret;
            }
            List<Claim> list = new List<Claim>
        {
            new Claim("jti", tokenModel.Uid.ToString()),
            new Claim("iat", utcNow.ToUnixTimeSeconds().ToString(), "http://www.w3.org/2001/XMLSchema#integer64"),
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration", utcNow.AddSeconds(_options.ExpirationSeconds).LocalDateTime.ToString("O")),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", tokenModel.Name ?? string.Empty),
            new Claim("TenantId", tokenModel.TenantId.ToString()),
            new Claim("isVisitor", isVisitor.ToString())
        };
            if (isVisitor) list.Add(new Claim("visitorId", visitorId.ToString()));
            list.AddRange(from role in (tokenModel.Role ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                          select new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
            SigningCredentials val = new SigningCredentials((SecurityKey)new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)), "HS256");
            JwtSecurityToken val2 = new JwtSecurityToken(_options.Issuer, _options.Audience, (IEnumerable<Claim>)list, (DateTime?)utcNow.UtcDateTime, (DateTime?)utcNow.AddSeconds(_options.ExpirationSeconds).UtcDateTime, val);
            return ((SecurityTokenHandler)new JwtSecurityTokenHandler()).WriteToken((SecurityToken)(object)val2);
        }
    }
}
