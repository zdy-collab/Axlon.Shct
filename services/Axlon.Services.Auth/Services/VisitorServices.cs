using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Auth.IServices;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Models.Enums;

namespace Axlon.Services.Auth.Services
{
    public class VisitorServices(IBaseRepository<Visitors> repository) : BaseServices<Visitors>(repository), IVisitorServices
    {
        public async Task<Visitors> VisitorLoginAsync(string openId, Source source, long? promoUserId)
        {
            // 游客
            var visitors = await base.Query(x => x.OpenId == openId);

            // 如果visitors中不存在游客信息
            if (visitors.Count == 0)
            {
                var visitor = new Visitors();
                visitor.Create(openId, source, promoUserId);
                await base.Add(visitor);
                return visitor;
            }
            // 如果存在
            else if (visitors.Count == 1)
            {
                var visitor = visitors[0];
                visitor.Login();
                await base.Update(visitor);
                return visitor;
            }

            throw new Exception("找到多个符合条件的用户信息");
        }
    }
}
