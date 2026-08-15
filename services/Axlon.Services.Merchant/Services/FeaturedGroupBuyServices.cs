using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Contracts.GroupBuy.Enums;
using Axlon.Services.Merchant.Services.Interfaces;

namespace Axlon.Services.Merchant.Services
{
    public class FeaturedGroupBuyServices(IBaseRepository<FeaturedGroupBuys> repository) : BaseServices<FeaturedGroupBuys>(repository), IFeaturedGroupBuyServices
    {
        public Task<List<FeaturedGroupBuyInfoDto>> GetValidGroupBuyAsync()
        {
            var data = base.Db.Queryable<FeaturedGroupBuys>()
                .LeftJoin<GroupBuys>((x, y) => x.GroupBuyId == y.Id)
                .Where((x, y) =>
                    x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now
                    && y.IsOn == IsOn.上架)
                .OrderByDescending(x => x.SortWeight)
                .Select((x, y) => new FeaturedGroupBuyInfoDto
                {
                    Id = x.Id,
                    GroupBuyId = y.Id,
                    ImageFileId = y.ImageFileId,
                    ImageOss = y.ImageOss,
                    Title = y.Title,
                    OriginalPrice = y.OriginalPrice,
                    GroupPrice = y.GroupPrice,
                    SortWeight = x.SortWeight
                })
                .ToListAsync();

            return data;
        }
    }
}
