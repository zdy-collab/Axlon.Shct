using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Basic.Services;
using Axlon.Services.Contracts.Category.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers.Internal
{
    [Route("api/basic/internal/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InternalCategoryController : BaseApiController
    {
        private readonly ICategoryServices categoryServices;

        public InternalCategoryController(ICategoryServices categoryServices)
        {
            this.categoryServices = categoryServices;
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
            if (listIds.Count == 0) return Success(data: new List<long>());

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
