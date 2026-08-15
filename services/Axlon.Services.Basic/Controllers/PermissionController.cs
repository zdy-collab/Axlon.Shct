using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Authentication.Policys;
using Axlon.Framework.Core;
using Axlon.Framework.Core.Helper;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [Route("api/basic/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class PermissionController : BaseApiController
    {
        readonly IUnitOfWorkManage _unitOfWorkManage;
        readonly IPermissionServices _permissionServices;
        readonly IModuleServices _moduleServices;
        readonly IRoleModulePermissionServices _roleModulePermissionServices;
        readonly IUserRoleServices _userRoleServices;
        private readonly IHttpClientFactory _httpClientFactory;
        readonly IHttpContextAccessor _httpContext;
        readonly IUser _user;
        private readonly PermissionRequirement _requirement;

        public PermissionController(IPermissionServices permissionServices, IModuleServices moduleServices,
            IRoleModulePermissionServices roleModulePermissionServices, IUserRoleServices userRoleServices,
            IUnitOfWorkManage unitOfWorkManage,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContext, IUser user, PermissionRequirement requirement)
        {
            _permissionServices = permissionServices;
            _unitOfWorkManage = unitOfWorkManage;
            _moduleServices = moduleServices;
            _roleModulePermissionServices = roleModulePermissionServices;
            _userRoleServices = userRoleServices;
            _httpClientFactory = httpClientFactory;
            _httpContext = httpContext;
            _user = user;
            _requirement = requirement;
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<PageResponseModel<Permission>>> Get(int page = 1, string key = "", int pageSize = 50)
        {
            PageResponseModel<Permission> permissions = new PageResponseModel<Permission>();
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            permissions = await _permissionServices.QueryPage(a => a.IsDeleted != true && (a.Name != null && a.Name.Contains(key)), page, pageSize, " Id desc ");

            var apis = await _moduleServices.Query(d => d.IsDeleted == false);
            var permissionsView = permissions.data;

            var permissionAll = await _permissionServices.Query(d => d.IsDeleted != true);
            foreach (var item in permissionsView)
            {
                List<long> pidarr = new() { item.Pid };
                if (item.Pid > 0)
                {
                    pidarr.Add(0);
                }
                var parent = permissionAll.FirstOrDefault(d => d.Id == item.Pid);

                while (parent != null)
                {
                    pidarr.Add(parent.Id);
                    parent = permissionAll.FirstOrDefault(d => d.Id == parent.Pid);
                }

                item.PidArr = pidarr.OrderBy(d => d).Distinct().ToList();
                foreach (var pid in item.PidArr)
                {
                    var per = permissionAll.FirstOrDefault(d => d.Id == pid);
                    item.PnameArr.Add((per != null ? per.Name : "根节点") + "/");
                }

                item.MName = apis.FirstOrDefault(d => d.Id == item.Mid)?.LinkUrl;
            }

            permissions.data = permissionsView;

            return permissions.dataCount >= 0 ? Success(permissions, "获取成功") : Failed<PageResponseModel<Permission>>("获取失败");
        }

        /// <summary>
        /// 查询树形 Table
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<Permission>>> GetTreeTable(long f = 0, string key = "")
        {
            List<Permission> permissions = new List<Permission>();
            var apiList = await _moduleServices.Query(d => d.IsDeleted == false);
            var permissionsList = await _permissionServices.Query(d => d.IsDeleted == false);
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            if (key != "")
            {
                permissions = permissionsList.Where(a => a.Name.Contains(key)).OrderBy(a => a.OrderSort).ToList();
            }
            else
            {
                permissions = permissionsList.Where(a => a.Pid == f).OrderBy(a => a.OrderSort).ToList();
            }

            foreach (var item in permissions)
            {
                List<long> pidarr = new() { };
                var parent = permissionsList.FirstOrDefault(d => d.Id == item.Pid);

                while (parent != null)
                {
                    pidarr.Add(parent.Id);
                    parent = permissionsList.FirstOrDefault(d => d.Id == parent.Pid);
                }

                pidarr.Reverse();
                pidarr.Insert(0, 0);
                item.PidArr = pidarr;

                item.MName = apiList.FirstOrDefault(d => d.Id == item.Mid)?.LinkUrl;
                item.hasChildren = permissionsList.Where(d => d.Pid == item.Id).Any();
            }

            return Success(permissions, "获取成功");
        }

        /// <summary>
        /// 添加一个菜单
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Permission permission)
        {
            permission.CreateId = _user.ID;
            permission.CreateBy = _user.Name;

            var id = (await _permissionServices.Add(permission));
            return id > 0 ? Success(id.ObjToString(), "添加成功") : Failed("添加失败");
        }

        /// <summary>
        /// 保存菜单权限分配
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> Assign([FromBody] AssignView assignView)
        {
            if (assignView.rid > 0)
            {
                try
                {
                    var old_rmps = await _roleModulePermissionServices.Query(d => d.RoleId == assignView.rid);

                    _unitOfWorkManage.BeginTran();
                    await _permissionServices.Db.Deleteable<RoleModulePermission>(t => t.RoleId == assignView.rid).ExecuteCommandAsync();
                    var permissions = await _permissionServices.Query(d => d.IsDeleted == false);

                    List<RoleModulePermission> new_rmps = new List<RoleModulePermission>();
                    var nowTime = _permissionServices.Db.GetDate();
                    foreach (var item in assignView.pids)
                    {
                        var moduleid = permissions.Find(p => p.Id == item)?.Mid;
                        var find_old_rmps = old_rmps.Find(p => p.PermissionId == item);

                        RoleModulePermission roleModulePermission = new RoleModulePermission()
                        {
                            IsDeleted = false,
                            RoleId = assignView.rid,
                            ModuleId = moduleid.ObjToLong(),
                            PermissionId = item,
                            CreateId = find_old_rmps == null ? _user.ID : find_old_rmps.CreateId,
                            CreateBy = find_old_rmps == null ? _user.Name : find_old_rmps.CreateBy,
                            CreateTime = find_old_rmps == null ? nowTime : find_old_rmps.CreateTime,
                            ModifyId = _user.ID,
                            ModifyBy = _user.Name,
                            ModifyTime = nowTime
                        };
                        new_rmps.Add(roleModulePermission);
                    }
                    if (new_rmps.Count > 0) await _roleModulePermissionServices.Add(new_rmps);
                    _unitOfWorkManage.CommitTran();
                }
                catch (Exception)
                {
                    _unitOfWorkManage.RollbackTran();
                    throw;
                }
                _requirement.Permissions.Clear();
                return Success<string>("保存成功");
            }
            else
            {
                return Failed<string>("请选择要操作的角色");
            }
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<PermissionTree>> GetPermissionTree(long pid = 0, bool needbtn = false)
        {
            var permissions = await _permissionServices.Query(d => d.IsDeleted == false);
            var permissionTrees = (from child in permissions
                                   where child.IsDeleted == false
                                   orderby child.Id
                                   select new PermissionTree
                                   {
                                       value = child.Id,
                                       label = child.Name,
                                       Pid = child.Pid,
                                       isbtn = child.IsButton,
                                       order = child.OrderSort,
                                   }).ToList();
            PermissionTree rootRoot = new PermissionTree
            {
                value = 0,
                Pid = 0,
                label = "根节点"
            };

            permissionTrees = permissionTrees.OrderBy(d => d.order).ToList();

            RecursionHelper.LoopToAppendChildren(permissionTrees, rootRoot, pid, needbtn);

            return Success(rootRoot, "获取成功");
        }

        /// <summary>
        /// 获取路由树
        /// </summary>
        [HttpGet]
        public async Task<MessageModel<NavigationBar>> GetNavigationBar(long uid)
        {
            var data = new MessageModel<NavigationBar>();

            long uidInHttpcontext1 = 0;
            var roleIds = new List<long>();
            if (Permissions.IsUseIds4)
            {
                uidInHttpcontext1 = (from item in _httpContext.HttpContext.User.Claims
                                     where item.Type == ClaimTypes.NameIdentifier
                                     select item.Value).FirstOrDefault().ObjToLong();
                if (!(uidInHttpcontext1 > 0))
                {
                    uidInHttpcontext1 = (from item in _httpContext.HttpContext.User.Claims
                                         where item.Type == "sub"
                                         select item.Value).FirstOrDefault().ObjToLong();
                }
                roleIds = (from item in _httpContext.HttpContext.User.Claims
                           where item.Type == ClaimTypes.Role
                           select item.Value.ObjToLong()).ToList();
                if (!roleIds.Any())
                {
                    roleIds = (from item in _httpContext.HttpContext.User.Claims
                               where item.Type == "role"
                               select item.Value.ObjToLong()).ToList();
                }
            }
            else
            {
                //TODO
                //uidInHttpcontext1 = ((JwtHelper.SerializeJwt(_httpContext.HttpContext.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "")))?.Uid).ObjToLong();
                roleIds = (await _userRoleServices.Query(d => d.IsDeleted == false && d.UserId == uid)).Select(d => d.RoleId.ObjToLong()).Distinct().ToList();
            }

            if (uid > 0 && uid == uidInHttpcontext1)
            {
                if (roleIds.Any())
                {
                    var pids = (await _roleModulePermissionServices.Query(d => d.IsDeleted == false && roleIds.Contains(d.RoleId))).Select(d => d.PermissionId.ObjToLong()).Distinct();
                    if (pids.Any())
                    {
                        var rolePermissionMoudles = (await _permissionServices.Query(d => pids.Contains(d.Id))).OrderBy(c => c.OrderSort);
                        var permissionTrees = (from child in rolePermissionMoudles
                                               where child.IsDeleted == false
                                               orderby child.Id
                                               select new NavigationBar
                                               {
                                                   id = child.Id,
                                                   name = child.Name,
                                                   pid = child.Pid,
                                                   order = child.OrderSort,
                                                   path = child.Code,
                                                   iconCls = child.Icon,
                                                   Func = child.Func,
                                                   IsHide = child.IsHide.ObjToBool(),
                                                   IsButton = child.IsButton.ObjToBool(),
                                                   meta = new NavigationBarMeta
                                                   {
                                                       icon = child.IconNew,
                                                       requireAuth = true,
                                                       title = child.Name,
                                                       NoTabPage = child.IsHide.ObjToBool(),
                                                       keepAlive = child.IskeepAlive.ObjToBool()
                                                   }
                                               }).ToList();

                        NavigationBar rootRoot = new NavigationBar()
                        {
                            id = 0,
                            pid = 0,
                            order = 0,
                            name = "根节点",
                            path = "",
                            iconCls = "",
                            meta = new NavigationBarMeta(),
                        };

                        permissionTrees = permissionTrees.OrderBy(d => d.order).ToList();
                        RecursionHelper.LoopNaviBarAppendChildren(permissionTrees, rootRoot);

                        data.success = true;
                        if (data.success)
                        {
                            data.response = rootRoot;
                            data.msg = "获取成功";
                        }
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 通过角色获取菜单
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<AssignShow>> GetPermissionIdByRoleId(long rid = 0)
        {
            var rmps = await _roleModulePermissionServices.Query(d => d.IsDeleted == false && d.RoleId == rid);
            var permissionTrees = (from child in rmps
                                   orderby child.Id
                                   select child.PermissionId.ObjToLong()).ToList();

            var permissions = await _permissionServices.Query(d => d.IsDeleted == false);
            List<string> assignbtns = new List<string>();

            foreach (var item in permissionTrees)
            {
                var pername = permissions.FirstOrDefault(d => d.IsButton && d.Id == item)?.Name;
                if (!string.IsNullOrEmpty(pername))
                {
                    assignbtns.Add(item.ObjToString());
                }
            }

            return Success(new AssignShow()
            {
                permissionids = permissionTrees,
                assignbtns = assignbtns,
            }, "获取成功");
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Permission permission)
        {
            var data = new MessageModel<string>();
            if (permission != null && permission.Id > 0)
            {
                data.success = await _permissionServices.Update(permission);
                await _roleModulePermissionServices.UpdateModuleId(permission.Id, permission.Mid);
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = permission?.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(long id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var userDetail = await _permissionServices.QueryById(id);
                userDetail.IsDeleted = true;
                data.success = await _permissionServices.Update(userDetail);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = userDetail?.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 导入多条菜单信息
        /// </summary>
        [HttpPost]
        public async Task<MessageModel<string>> BatchPost([FromBody] List<Permission> permissions)
        {
            var data = new MessageModel<string>();
            string ids = string.Empty;
            int sucCount = 0;

            for (int i = 0; i < permissions.Count; i++)
            {
                var permission = permissions[i];
                if (permission != null)
                {
                    permission.CreateId = _user.ID;
                    permission.CreateBy = _user.Name;
                    ids += (await _permissionServices.Add(permission));
                    sucCount++;
                }
            }

            data.success = ids.IsNotEmptyOrNull();
            if (data.success)
            {
                data.response = ids;
                data.msg = $"{sucCount}条数据添加成功";
            }

            return data;
        }
    }

    public class AssignView
    {
        public List<long> pids { get; set; }
        public long rid { get; set; }
    }
    public class AssignShow
    {
        public List<long> permissionids { get; set; }
        public List<string> assignbtns { get; set; }
    }
}
