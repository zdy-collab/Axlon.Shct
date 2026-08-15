using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Models.Enums;

namespace Axlon.Services.Auth.IServices
{
    public interface IVisitorServices : IBaseServices<Visitors>
    {
        /// <summary>
        /// 根据微信openId，游客登录
        /// </summary>
        /// <param name="openId">微信id</param>
        /// <param name="source">推广来源</param>
        /// <param name="promoUserId">推广人id</param>
        /// <returns></returns>
        Task<Visitors> VisitorLoginAsync(string openId, Source source, long? promoUserId);
    }
}
