using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Contracts.GroupBuy.Enums;
using Mapster;
using Axlon.Services.Merchant.Services.Interfaces;

namespace Axlon.Services.Merchant.Services
{
    public class GroupBuyServices(IBaseRepository<GroupBuys> repository) : BaseServices<GroupBuys>(repository), IGroupBuyServices
    {
        public async Task<PageResponseModel<GroupBuyInfoDto>> ByMerchantIdGetInfoAsync(QueryPage queryPage, long merchantId)
        {
            var data = await base.QueryPage(
                whereExpression: x => x.MerchantId == merchantId
                    && x.IsOn == IsOn.上架
                    && x.StartTime <= DateTime.Now
                    && x.EndTime >= DateTime.Now,
                pageIndex: queryPage.page,
                pageSize: queryPage.pageSize,
                "created_at");

            var res = data.Adapt<PageResponseModel<GroupBuyInfoDto>>();


            return res;
        }
    }
}
