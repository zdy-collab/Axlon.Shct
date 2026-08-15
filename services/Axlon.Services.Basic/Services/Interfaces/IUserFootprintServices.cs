using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.User;

namespace Axlon.Services.Basic.Services.Interfaces
{
    /// <summary>
    /// 用户足迹
    /// </summary>
    public interface IUserFootprintServices : IBaseServicesExtend<UserFootprintAddInput, UserFootprintEditInput, UserFootprintOutput, UserFootprint>
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        Task<PageResponseModel<UserFootprintOutput>> GetPageAsync(UserFootprintPageInput pageRequest);

        Task<bool> PublishPageViewAsync(UserPageViewInput input, CancellationToken cancellationToken = default);

        Task<bool> RecordViewAsync(UserFootprintViewedIntegrationEvent @event, CancellationToken cancellationToken = default);

        Task<bool> RemoveMineAsync(long id);

        Task<int> ClearMineAsync(string? targetType);
    }
}
