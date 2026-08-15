using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Category.Domain.IServices;
using Axlon.Services.Contracts.Category.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Category.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryDomainService categoryDomainService;

        public CategoryController(ICategoryDomainService categoryDomainService)
        {
            this.categoryDomainService = categoryDomainService;
        }

        /// <summary>
        /// 获取完整树
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTree")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetTreeAsync() 
        {
            return Success(data:await categoryDomainService.GetTreeAsync());
        }

        /// <summary>
        /// 根据父节点获取子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getChildren")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetChildrenAsync([FromQuery]long id) 
        {
            return Success(data: await categoryDomainService.GetChildrenAsync(id));
        }

        /// <summary>
        /// 获取顶部节点
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTopNode")]
        public async Task<MessageModel<List<CategoryNodeDto>>> GetTopNodeAsync() 
        {
            return Success(data: await categoryDomainService.GetTopNodeAsync());
        }

        /// <summary>
        /// 根据id集合获取子id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("byIdsGetChidrenIds")]
        public async Task<MessageModel<List<long>>> ByIdsGetChidrenIdsAsync([FromBody]List<long> ids)
        {
            return Success(data: await categoryDomainService.ByIdsGetChidrenIdsAsync(ids));
        }
    }
}
