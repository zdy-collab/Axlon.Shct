using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Basic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 用户地址
    /// </summary>
    /// <param name="services"></param>
    [ApiController]
    [Authorize]
    [Route("api/basic/[controller]/[action]")]
    public sealed class UserAddressesController(IUserAddressServices services) : BaseApiController
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageInput"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<UserAddressOutput>>> GetPage([FromForm] UserAddressPageInput pageInput) =>
            Success(await services.GetPageAsync(pageInput));

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<MessageModel<UserAddressOutput>>> Get([Range(1, long.MaxValue)] long id)
        {
            try { return Success(await services.GetAsync(id)); }
            catch (KeyNotFoundException) { return NotFound(Failed<UserAddressOutput>("地址不存在", 404)); }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MessageModel<long>>> Add([FromBody] UserAddressAddInput input)
        {
            var id = await services.AddAsync(input);
            return CreatedAtAction(nameof(Get), new { id }, Success(id, "地址已创建"));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id:long}")]
        public async Task<ActionResult<MessageModel<bool>>> Update([Range(1, long.MaxValue)] long id, [FromBody] UserAddressEditInput input)
        {
            if (input.Id != 0 && input.Id != id) return BadRequest(Failed<bool>("路径 ID 与请求 ID 不一致", 400));
            input.Id = id;
            return await services.UpdateAsync(input)
                ? Success(true, "地址已更新")
                : NotFound(Failed<bool>("地址不存在", 404));
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/default")]
        public async Task<ActionResult<MessageModel<bool>>> SetDefault([Range(1, long.MaxValue)] long id) =>
            await services.SetDefaultAsync(id)
                ? Success(true, "默认地址已更新")
                : NotFound(Failed<bool>("地址不存在", 404));

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<MessageModel<bool>>> Delete([Range(1, long.MaxValue)] long id) =>
            await services.DeleteAsync(id)
                ? Success(true, "地址已删除")
                : NotFound(Failed<bool>("地址不存在", 404));
    }

}
