using Axlon.Framework.Abstractions.Tenants;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.Repository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Enums;

namespace Axlon.Services.Order.Repository
{
    public class PromotionEarningRepository : BaseRepository<PromotionEarnings>, IPromotionEarningRepository
    {
        public PromotionEarningRepository(IUnitOfWorkManage unitOfWorkManage, ITenantProvider tenantProvider, IUser user) : base(unitOfWorkManage, tenantProvider, user)
        {
        }

        public Task<List<PromotionEarnings>> ByOrderIdGetPendingInfoAsync(long orderId)
        {
            return base.Query(x => x.OrderId == orderId && x.Status == PromotionEarningsStatus.pending.ToString());
        }
    }
}
