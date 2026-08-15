using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Core;
using Axlon.Framework.Core.Helper;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/basic/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UserController : BaseApiController
    {
        private readonly IUnitOfWorkManage _unitOfWorkManage;
        readonly ISysUserInfoServices _sysUserInfoServices;
        readonly IUserRoleServices _userRoleServices;
        readonly IRoleServices _roleServices;
        private readonly IDepartmentServices _departmentServices;
        private readonly IUser _user;
        private readonly ILogger<UserController> _logger;

        public UserController(IUnitOfWorkManage unitOfWorkManage, ISysUserInfoServices sysUserInfoServices,
            IUserRoleServices userRoleServices,
            IRoleServices roleServices,
            IDepartmentServices departmentServices,
            IUser user, ILogger<UserController> logger)
        {
            _unitOfWorkManage = unitOfWorkManage;
            _sysUserInfoServices = sysUserInfoServices;
            _userRoleServices = userRoleServices;
            _roleServices = roleServices;
            _departmentServices = departmentServices;
            _user = user;
            _logger = logger;
        }

        /// <summary>
        /// 获取全部用户
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<SysUserInfoDto>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            int intPageSize = 50;

            var data = await _sysUserInfoServices.QueryPage(a => a.IsDeleted != true && a.Status >= 0 && ((a.LoginName != null && a.LoginName.Contains(key)) || (a.RealName != null && a.RealName.Contains(key))), page, intPageSize, " Id desc ");

            var allUserRoles = await _userRoleServices.Query(d => d.IsDeleted == false);
            var allRoles = await _roleServices.Query(d => d.IsDeleted == false);
            var allDepartments = await _departmentServices.Query(d => d.IsDeleted == false);

            var sysUserInfos = data.data;
            foreach (var item in sysUserInfos)
            {
                var currentUserRoles = allUserRoles.Where(d => d.UserId == item.Id).Select(d => d.RoleId).ToList();
                item.RIDs = currentUserRoles;
                item.RoleNames = allRoles.Where(d => currentUserRoles.Contains(d.Id)).Select(d => d.Name).ToList();
                var departmentNameAndIds = GetFullDepartmentName(allDepartments, item.DepartmentId);
                item.DepartmentName = departmentNameAndIds.Item1;
                item.Dids = departmentNameAndIds.Item2;
            }

            data.data = sysUserInfos;

            return Success(data.ConvertTo<SysUserInfoDto>());
        }

        private (string, List<long>) GetFullDepartmentName(List<Department> departments, long departmentId)
        {
            var departmentModel = departments.FirstOrDefault(d => d.Id == departmentId);
            if (departmentModel == null)
            {
                return ("", new List<long>());
            }

            var pids = departmentModel.CodeRelationship?.TrimEnd(',').Split(',').Select(d => d.ObjToLong()).ToList();
            pids.Add(departmentModel.Id);
            var pnams = departments.Where(d => pids.Contains(d.Id)).ToList().Select(d => d.Name).ToArray();
            var fullName = string.Join("/", pnams);

            return (fullName, pids);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public string Get(string id)
        {
            _logger.LogError("test wrong");
            return "value";
        }

        /// <summary>
        /// 获取用户详情根据token【无权限】
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<SysUserInfoDto>> GetInfoByToken(string token)
        {
            var data = new MessageModel<SysUserInfoDto>();
            //if (!string.IsNullOrEmpty(token))
            //{
            //    var tokenModel = JwtHelper.SerializeJwt(token);
            //    if (tokenModel != null && tokenModel.Uid > 0)
            //    {
            //        var userinfo = await _sysUserInfoServices.QueryById(tokenModel.Uid);
            //        if (userinfo != null)
            //        {
            //            data.response = userinfo.Adapt<SysUserInfoDto>();
            //            data.success = true;
            //            data.msg = "获取成功";
            //        }
            //    }
            //}

            return data;
        }

        /// <summary>
        /// 添加一个用户
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] SysUserInfoDto sysUserInfo)
        {
            var data = new MessageModel<string>();

            sysUserInfo.uLoginPWD = MD5Helper.MD5Encrypt32(sysUserInfo.uLoginPWD);
            sysUserInfo.uRemark = _user.Name;

            var id = await _sysUserInfoServices.Add(sysUserInfo.Adapt<SysUserInfo>());
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新用户与角色
        /// </summary>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] SysUserInfoDto sysUserInfo)
        {
            var data = new MessageModel<string>();

            var oldUser = await _sysUserInfoServices.QueryById(sysUserInfo.uID);
            if (oldUser is not { Id: > 0 })
            {
                return Failed<string>("用户不存在或已被删除");
            }

            try
            {
                if (sysUserInfo.uLoginPWD != oldUser.LoginPWD)
                {
                    oldUser.CriticalModifyTime = DateTime.Now;
                }

                sysUserInfo.Adapt(oldUser);

                _unitOfWorkManage.BeginTran();
                var usreroles = (await _userRoleServices.Query(d => d.UserId == oldUser.Id));
                if (usreroles.Any())
                {
                    var ids = usreroles.Select(d => d.Id.ToString()).ToArray();
                    var isAllDeleted = await _userRoleServices.DeleteByIds(ids);
                    if (!isAllDeleted)
                    {
                        return Failed("服务器更新异常");
                    }
                }

                if (sysUserInfo.RIDs.Count > 0)
                {
                    var userRolsAdd = new List<UserRole>();
                    sysUserInfo.RIDs.ForEach(rid => { userRolsAdd.Add(new UserRole(oldUser.Id, rid)); });

                    var oldRole = usreroles.Select(s => s.RoleId).OrderBy(i => i).ToArray();
                    var newRole = userRolsAdd.Select(s => s.RoleId).OrderBy(i => i).ToArray();
                    if (!oldRole.SequenceEqual(newRole))
                    {
                        oldUser.CriticalModifyTime = DateTime.Now;
                    }

                    await _userRoleServices.Add(userRolsAdd);
                }

                data.success = await _sysUserInfoServices.Update(oldUser);

                _unitOfWorkManage.CommitTran();

                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = oldUser.Id.ObjToString();
                }
            }
            catch (Exception e)
            {
                _unitOfWorkManage.RollbackTran();
                _logger.LogError(e, e.Message);
            }

            return data;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(long id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var userDetail = await _sysUserInfoServices.QueryById(id);
                userDetail.IsDeleted = true;
                data.success = await _sysUserInfoServices.Update(userDetail);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = userDetail?.Id.ObjToString();
                }
            }

            return data;
        }

        #region 用户地址



        #endregion
    }
}
