using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Category.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    [Route("api/basic/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryServices categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            this.categoryServices = categoryServices;
        }

        /// <summary>
        /// 获取完整树
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTree")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetTreeAsync()
        {
            return Success(data: await categoryServices.GetTreeAsync());
        }

        /// <summary>
        /// 根据父节点获取子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getChildren")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetChildrenAsync([FromQuery] long id)
        {
            return Success(data: await categoryServices.GetChildrenAsync(id));
        }

        /// <summary>
        /// 获取顶部节点
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTopNode")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetTopNodeAsync()
        {
            return Success(data: await categoryServices.GetTopNodeAsync());
        }

        /// <summary>
        /// 根据id集合获取子id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("byIdsGetChidrenIds")]
        [AllowAnonymous]
        public async Task<MessageModel<List<long>>> ByIdsGetChidrenIdsAsync([FromQuery] string ids)
        {
            var listIds = ids.Split(",").Select(x => long.Parse(x)).ToList();
            if(listIds.Count == 0) return Success(data: new List<long>());

            return Success(data: await categoryServices.ByIdsGetChidrenIdsAsync(listIds));
        }

        /// <summary>
        /// 根据主键集合获取信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("byIdsGetCategories")]
        [AllowAnonymous]
        public async Task<MessageModel<List<CategoryNodeDto>>> ByIdsGetCategoriesAsync([FromQuery] List<long> ids)
        {
            return Success(data: await categoryServices.ByIdsGetCategoriesAsync(ids));
        }
    }
}
