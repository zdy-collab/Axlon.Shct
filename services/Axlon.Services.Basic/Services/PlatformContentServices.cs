using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Content;
using Axlon.Services.Contracts.Content.Dto;
using Axlon.Services.Contracts.Content.Enum;
using Axlon.Services.Contracts.Extensions;
using Mapster;

namespace Axlon.Services.Basic.Services
{
    public class PlatformContentServices(IBaseRepository<PlatformContents> repository) : BaseServices<PlatformContents>(repository), IPlatformContentServices
    {
        public async Task<List<PlatformContentInfoDto>> GetPlatformContentAsync()
        {
            var data = await base.Query(x => x.Status == PublishStatus.已发布 && x.PublishTime <= DateTime.Now, "publish_time");
            var res = data.Adapt<List<PlatformContentInfoDto>>();

            return res;
        }
    }
}
