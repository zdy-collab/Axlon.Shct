using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Core;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 接口管理
    /// </summary>
    [Route("api/basic/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class ModuleController : BaseApiController
    {
        readonly IModuleServices _moduleServices;
        readonly IUser _user;

        public ModuleController(IModuleServices moduleServices, IUser user)
        {
            _moduleServices = moduleServices;
            _user = user;
        }

        /// <summary>
        /// 获取全部接口api
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<Modules>>> Get(int page = 1, string key = "", int pageSize = 50)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            Expression<Func<Modules, bool>> whereExpression = a => a.IsDeleted != true && ((a.Name != null && a.Name.Contains(key) || (a.LinkUrl != null && a.LinkUrl.Contains(key))));

            PageResponseModel<Modules> data = new PageResponseModel<Modules>();

            if (page == -1)
            {
                var modules = await _moduleServices.Query(whereExpression, " Id desc ");
                data.data = modules;
            }
            else
            {
                data = await _moduleServices.QueryPage(whereExpression, page, pageSize, " Id desc ");
            }

            return Success(data, "获取成功");
        }

        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        /// <summary>
        /// 添加一条接口信息
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Modules module)
        {
            module.CreateId = _user.ID;
            module.CreateBy = _user.Name;
            var id = await _moduleServices.Add(module);
            return id > 0 ? Success(id.ObjToString(), "添加成功") : Failed();
        }

        /// <summary>
        /// 更新接口信息
        /// </summary>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Modules module)
        {
            if (module == null || module.Id <= 0)
                return Failed("缺少参数");
            return await _moduleServices.Update(module) ? Success(module?.Id.ObjToString(), "更新成功") : Failed();
        }

        /// <summary>
        /// 删除一条接口
        /// </summary>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(long id)
        {
            if (id <= 0)
                return Failed("缺少参数");
            var userDetail = await _moduleServices.QueryById(id);
            if (userDetail == null)
                return Failed("信息不存在");

            userDetail.IsDeleted = true;
            return await _moduleServices.Update(userDetail) ? Success(userDetail?.Id.ObjToString(), "删除成功") : Failed("删除失败");
        }

        /// <summary>
        /// 导入多条接口信息
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> BatchPost([FromBody] List<Modules> modules)
        {
            string ids = string.Empty;
            int sucCount = 0;

            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module != null)
                {
                    module.CreateId = _user.ID;
                    module.CreateBy = _user.Name;
                    ids += (await _moduleServices.Add(module));
                    sucCount++;
                }
            }
            return ids.IsNotEmptyOrNull() ? Success(ids, $"{sucCount}条数据添加成功") : Failed();
        }
    }
}
