using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Core;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/basic/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController : BaseApiController
    {
        readonly IRoleServices _roleServices;
        readonly IUser _user;

        public RoleController(IRoleServices roleServices, IUser user)
        {
            _roleServices = roleServices;
            _user = user;
        }

        /// <summary>
        /// 获取全部角色
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<Role>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            int intPageSize = 50;

            var data = await _roleServices.QueryPage(a => a.IsDeleted != true && (a.Name != null && a.Name.Contains(key)), page, intPageSize, " Id desc ");

            return Success(data, "获取成功");
        }

        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Role role)
        {
            role.CreateId = _user.ID;
            role.CreateBy = _user.Name;
            var id = (await _roleServices.Add(role));
            return id > 0 ? Success(id.ObjToString(), "添加成功") : Failed("添加失败");
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Role role)
        {
            if (role == null || role.Id <= 0)
                return Failed("缺少参数");

            return await _roleServices.Update(role) ? Success(role?.Id.ObjToString(), "更新成功") : Failed("更新失败");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(long id)
        {
            var data = new MessageModel<string>();
            if (id <= 0) return Failed();
            var userDetail = await _roleServices.QueryById(id);
            if (userDetail == null) return Success<string>(null, "角色不存在");
            userDetail.IsDeleted = true;
            return await _roleServices.Update(userDetail) ? Success(userDetail?.Id.ObjToString(), "删除成功") : Failed();
        }
    }
}
