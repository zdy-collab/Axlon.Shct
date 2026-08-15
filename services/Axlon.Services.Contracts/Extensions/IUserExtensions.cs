using Axlon.Framework.Core.HttpContextUser;

namespace Axlon.Services.Contracts.Extensions
{
    public static class IUserExtensions
    {
        public static string GetOpenId(this IUser user)
        {
            var claims = user.GetClaimValueByType("openId");
            if (claims.Count > 0) return claims[0];
            else return null;
        }

        /// <summary>
        /// 获取游客Id
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static long? GetVisitorId(this IUser user)
        {
            var claims = user.GetClaimValueByType("visitorId");
            if (claims.Count > 0) return long.Parse(claims[0]);
            else return null;
        }

        /// <summary>
        /// 是否为游客
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool GetIsVisitor(this IUser user)
        {
            var claims = user.GetClaimValueByType("isVisitor");
            if (claims.Count > 0) return bool.Parse(claims[0]);
            else return false;
        }
    }
}
