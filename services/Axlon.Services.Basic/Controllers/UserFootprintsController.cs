using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Basic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 我的足迹
    /// </summary>
    /// <param name="services"></param>
    [ApiController]
    [Authorize]
    [Route("api/user-footprints")]
    public sealed class UserFootprintsController(IUserFootprintServices services) : BaseApiController
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageInput"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<UserFootprintOutput>>> GetPage([FromQuery] UserFootprintPageInput pageInput) =>
            Success(await services.GetPageAsync(pageInput));

        /// <summary>
        /// 上报前端页面浏览；仅接受服务端约定的 PageCode。
        /// </summary>
        [HttpPost("events/pages")]
        public async Task<MessageModel<bool>> RecordPageView(
            [FromBody] UserPageViewInput input, CancellationToken cancellationToken) =>
            Success(await services.PublishPageViewAsync(input, cancellationToken));

        [HttpDelete("{id:long}")]
        public async Task<MessageModel<bool>> Remove(long id) =>
            Success(await services.RemoveMineAsync(id));

        [HttpDelete]
        public async Task<MessageModel<int>> Clear([FromQuery] string? targetType = null) =>
            Success(await services.ClearMineAsync(targetType));
    }
}
