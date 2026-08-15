using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Content;

namespace Axlon.Services.Basic.Services
{
    public class PlatformBannerServices(IBaseRepository<PlatformBanners> repository) : BaseServices<PlatformBanners>(repository), IPlatformBannerServices
    {
        //public Task<List<PlatformBannersInfoDto>> GetPlatformBannersListAsync()
        //{

        //}
    }
}
