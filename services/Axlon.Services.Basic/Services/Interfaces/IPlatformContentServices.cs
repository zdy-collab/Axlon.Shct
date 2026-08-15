using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Contracts.Content;
using Axlon.Services.Contracts.Content.Dto;

namespace Axlon.Services.Basic.IServices
{
    public interface IPlatformContentServices : IBaseServices<PlatformContents>
    {
        public Task<List<PlatformContentInfoDto>> GetPlatformContentAsync();
    }
}
