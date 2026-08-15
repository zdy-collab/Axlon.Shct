using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Authentication.Helpers;
using Axlon.Framework.Authentication.Policys;
using Axlon.Framework.Core;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Auth.External;
using Axlon.Services.Auth.Helper;
using Axlon.Services.Auth.IServices;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Models.Enums;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.ViewModels;
using Axlon.Services.Contracts.Wechat.Dto;
using Dm.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Axlon.Services.Auth.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    //[Authorize(Permissions.Name)]
    [AllowAnonymous]
    public class AuthWechatController : BaseApiController
    {
        private readonly IWechatApi wechatApi;
        private readonly ISysUserInfoServices userServices;
        private readonly IVisitorServices visitorServices;
        private readonly IUserRoleServices userRoleServices;
        private readonly IUnitOfWorkManage unitOfWorkManage;
        private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IAxlonJwtTokenService _tokens;
        private readonly IUser loginUser;
        private readonly IUserWalletsServices userWalletsServices;
        private readonly AxlonJwtOptions _jwtOptions;
        private readonly PermissionRequirement _requirement;

        //private

        public AuthWechatController(IWechatApi wechatApi, ISysUserInfoServices userServices,
            IVisitorServices visitorServices, PermissionRequirement requirement, IRoleModulePermissionServices roleModulePermissionServices,
            IAxlonJwtTokenService tokens, IOptions<AxlonJwtOptions> jwtOptions, IUser loginUser, IUserRoleServices userRoleServices, 
            IUnitOfWorkManage unitOfWorkManage, IUserWalletsServices userWalletsServices)
        {
            this.wechatApi = wechatApi;
            this.userServices = userServices;
            this.visitorServices = visitorServices;
            _requirement = requirement;
            _roleModulePermissionServices = roleModulePermissionServices;
            _tokens = tokens;
            _jwtOptions = jwtOptions.Value;
            this.loginUser = loginUser;
            this.userRoleServices = userRoleServices;
            this.unitOfWorkManage = unitOfWorkManage;
            this.userWalletsServices = userWalletsServices;
        }

        /// <summary>
        /// 获取接口调用凭据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("getToken")]
        //[AllowAnonymous]
        public async Task<MessageModel<WcTokenRes>> GetTokenAsync()
        {
            return Success(data: await wechatApi.GetTokenAsync());
        }

        ///// <summary>
        ///// 前端code换取用户凭证
        ///// </summary>
        ///// <param name="js_code"></param>
        ///// <returns></returns>
        //[HttpGet("login")]
        //public async Task<MessageModel<WcLoginRes>> LoginAsync([FromQuery] string js_code)
        //{
        //    return Success(data: await wechatServices.LoginAsync(js_code));
        //}

        /// <summary>
        /// 获取用户手机号信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("getPhoneInfo")]
        //[AllowAnonymous]

        public async Task<MessageModel<WcPhoneRes>> GetPhoneInfoAsync([FromQuery] WcPhoneReq req)
        {
            return Success(data: await wechatApi.GetPhoneInfoAsync(req));
        }

        /// <summary>
        /// 微信小程序登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("login")]
        //[AllowAnonymous]

        public async Task<MessageModel<LoginUserInfo>> LoginAsync([FromBody] WechatLoginReq req)
        {
            /*            //if (string.IsNullOrEmpty(req.Code)) return Failed<TokenInfoViewModel>("用户code不能为空！");

                        var wechatUser = await wechatApi.LoginAsync(req.Code); // 微信用户信息
                        var msgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(wechatUser);
                        if (!string.IsNullOrEmpty(req.TestUser))
                        {
                            wechatUser.openid = "oDaVo5OX5yC6GH1er5IFWWb3MbrE";
                            msgModel = null;
                        }
                        if (msgModel != null) return msgModel;

                        //// 获取手机号响应，接口收费暂时不使用
                        //var phoneNumberRes = await wechatApi.GetPhoneInfoAsync(new WcPhoneReq(req.Code,null));
                        //msgModel = WechatApiResHandle.ReturnMsg<TokenInfoViewModel>(wechatUser);
                        //if (phoneNumberRes != null) return msgModel;

                        //wechatUser.openid = "TestUser001";
                        // 分享链接进入：1007、1008、1044
                        // 搜索进入：1005、1006、1053
                        var shareIds = new List<int> { 1007, 1008, 1044 };
                        var searchIds = new List<int> { 1005, 1006, 1053 };

                        Source source = Source.搜索;
                        if (shareIds.Any(x => x == req.SceneID)) source = Source.分享;
                        else if (req.promoUserId != null) source = Source.推广码;

                        // 查询是否为正式用户
                        var user = (await userServices.Query(whereExpression: x => x.OpenId == wechatUser.openid)).FirstOrDefault();

                        #region 构建Jwt
                        var claims = new List<Claim>()
                        {
                            new Claim("openId",wechatUser.openid),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.DateToTimeStamp()),
                            new Claim(ClaimTypes.Expiration,
                                DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
                        };

                        if (user != null)
                        {
                            var userRoles = await userServices.GetUserRoleNameStr(user.Name, user.LoginPWD);

                            claims.AddRange(new List<Claim>()
                            {
                                new Claim(ClaimTypes.Name,user.Name),
                                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                                new Claim("isVisitor",  false.ToString()),

                            });
                            claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
                        }
                        else
                        {
                            var visitor = await visitorServices.VisitorLoginAsync(wechatUser.openid, source, req.promoUserId);
                            claims.AddRange(new List<Claim>()
                            {
                                new Claim(ClaimTypes.Name, "游客"),
                                new Claim(JwtRegisteredClaimNames.Jti, visitor.Id.ToString()),
                                new Claim("isVisitor",  true.ToString()),
                                new Claim(ClaimTypes.Role, "Mini")
                            });
                        }

                        if (!Permissions.IsUseIds4)
                        {
                            var data = await _roleModulePermissionServices.RoleModuleMaps();
                            var list = (from item in data
                                        where item.IsDeleted == false
                                        orderby item.Id
                                        select new PermissionItem
                                        {
                                            Url = item.Module?.LinkUrl,
                                            Role = item.Role?.Name.ObjToString(),
                                        }).ToList();

                            _requirement.Permissions = list;
                        }

                        var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);

                        var res = token.Adapt<LoginUserInfo>();
                        //res.PhoneNumber = phoneNumberRes.phone_info.phoneNumber;
                        // 游客
                        if (user == null)
                        {
                            res.userInfo.nickName = "游客";
                            res.userInfo.userId = null;
                            res.userInfo.IsVisitor = true;
                        }
                        // 正式用户
                        else
                        {
                            res.userInfo.nickName = user.Nickname;
                            res.userInfo.userId = user.Id;
                            res.userInfo.IsVisitor = false;
                        }

                        res.userInfo.PhoneNumber = "";

                        return Success(data: res);

                        #endregion*/
            //if (string.IsNullOrEmpty(req.Code)) return Failed<TokenInfoViewModel>("用户code不能为空！");

            var wechatUser = await wechatApi.LoginAsync(req.Code); // 微信用户信息
            var msgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(wechatUser);
            if (!string.IsNullOrEmpty(req.TestUser))
            {
                //wechatUser.openid = "oDaVo5OX5yC6GH1er5IFWWb3MbrE";
                wechatUser.openid = "test"; // 手机号登录测试

                msgModel = null;
            }
            if (msgModel != null) return msgModel;

            //// 获取手机号响应，接口收费暂时不使用
            //var phoneNumberRes = await wechatApi.GetPhoneInfoAsync(new WcPhoneReq(req.Code,null));
            //msgModel = WechatApiResHandle.ReturnMsg<TokenInfoViewModel>(wechatUser);
            //if (phoneNumberRes != null) return msgModel;

            //wechatUser.openid = "TestUser001";
            // 分享链接进入：1007、1008、1044
            // 搜索进入：1005、1006、1053
            var shareIds = new List<int> { 1007, 1008, 1044 };
            var searchIds = new List<int> { 1005, 1006, 1053 };

            Source source = Source.搜索;
            if (shareIds.Any(x => x == req.SceneID)) source = Source.分享;
            else if (req.promoUserId != null) source = Source.推广码;

            // 查询是否为正式用户
            var user = (await userServices.Query(whereExpression: x => x.OpenId == wechatUser.openid)).FirstOrDefault();
            long? visitorId = null;
            #region 构建Jwt
            //var claims = new List<Claim>()
            //{
            //    new Claim("openId",wechatUser.openid),
            //    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.DateToTimeStamp()),
            //    //new Claim(ClaimTypes.Expiration,
            //    //    DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
            //    new Claim(ClaimTypes.Expiration,
            //        DateTime.Now.AddSeconds(3600).ToString())
            //};
            string token = string.Empty;
            var roles = string.Empty;
            if (user != null)
            {
                roles = await userServices.GetUserRoleNameStr(wechatUser.openid);
                token = _tokens.Issue2(_jwtOptions,new TokenModelJwt { Uid = user.Id, Role = roles, Name = user.LoginName, TenantId = user.TenantId },false);
                //claims.AddRange(new List<Claim>()
                //{
                //    new Claim(ClaimTypes.Name,user.Name),
                //    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                //    new Claim("isVisitor",  false.ToString()),

                //});
                //claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            }
            else
            {
                var visitor = await visitorServices.VisitorLoginAsync(wechatUser.openid, source, req.promoUserId);
                visitorId = visitor.Id;
                token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = 0, Role = roles, Name = "游客", TenantId = 0 },true,visitorId);
                //claims.AddRange(new List<Claim>()
                //{
                //    new Claim(ClaimTypes.Name, "游客"),
                //    new Claim(JwtRegisteredClaimNames.Jti, "10004"),//visitor.Id.ToString()),
                //    new Claim("isVisitor",  true.ToString()),
                //    new Claim(ClaimTypes.Role, "Mini")
                //});
            }
            //if (!Permissions.IsUseIds4)
            //{
            //    var data = await _roleModulePermissionServices.RoleModuleMaps();
            //    var list = (from item in data
            //                where item.IsDeleted == false
            //                orderby item.Id
            //                select new PermissionItem
            //                {
            //                    Url = item.Module?.LinkUrl,
            //                    Role = item.Role?.Name.ObjToString(),
            //                }).ToList();

            //    _requirement.Permissions = list;
            //}

            //var token = JwtToken.BuildJwtToken(claims.ToArray(),_requirement);
            //var token = "";

            var res = new LoginUserInfo()
            {
                success = true,
                token = token,
                token_type = "Bearer",
                expires_in = _jwtOptions.ExpirationSeconds
            };
            res.token = token;
            //res.PhoneNumber = phoneNumberRes.phone_info.phoneNumber;
            // 游客
            if (user == null)
            {
                res.userInfo.nickName = "游客";
                res.userInfo.visitorId = visitorId;
                res.userInfo.IsVisitor = true;
            }
            // 正式用户
            else
            {
                res.userInfo.nickName = user.Nickname;
                res.userInfo.userId = user.Id;
                res.userInfo.IsVisitor = false;
                //res.userInfo.PhoneNumber = "";
            }

            

            return Success(data: res);

            #endregion
        }

        /// <summary>
        /// 手机号登录-正式用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("phoneNumberLogin")]
        //[AllowAnonymous]
        public async Task<MessageModel<LoginUserInfo>> PhoneNumberLoginAsync([FromBody] PhoneNumberLoginReq req)
        {
            var token = string.Empty;

            #region 获取openId
            var wechatUser = await wechatApi.LoginAsync(req.LoginCode); // 获取用户openId
            var wechatUserMsgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(wechatUser);
            if (wechatUserMsgModel != null) return wechatUserMsgModel;
            #endregion

            #region 获取手机号，验证openId与手机号是否匹配
            WcPhoneReq wcPhoneReq = new WcPhoneReq(req.PhoneNumberCode, wechatUser.openid);
            var getPhoneNumber = await wechatApi.GetPhoneInfoAsync(wcPhoneReq);
            var getPhoneNumberMsgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(getPhoneNumber);
            if (getPhoneNumberMsgModel != null) return getPhoneNumberMsgModel;
            //if (msgModel.status != 200)
            //{
            //    getPhoneNumber.phone_info = new();
            //    getPhoneNumber.phone_info.phoneNumber = "test";
            //}
            #endregion
            //wechatUser.openid = "test";
            //getPhoneNumber.phone_info = new()
            //{
            //    phoneNumber = "test"
            //};
            var sysUserInfo = (await userServices.Query(x => x.OpenId == wechatUser.openid)).FirstOrDefault();

            
            // 有正式用户身份，颁发token
            //if (sysUserInfo != null)
            //{
            //    var roles = await userServices.GetUserRoleNameStr(sysUserInfo.Id);
            //    token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = sysUserInfo.Id, Role = roles, Name = sysUserInfo.Nickname, TenantId = sysUserInfo.TenantId }, false);
            //}

            // 没有正式用户身份
            if(sysUserInfo == null) 
            {
                try
                {
                    unitOfWorkManage.BeginTran();
                    var visitor = (await visitorServices.Query(x => x.OpenId == wechatUser.openid)).FirstOrDefault();
                    long? promoUserId = null;   //推广人Id
                    var registerSource = "直接访问";

                    // 有游客身份，赋值推广人Id
                    if (visitor != null)
                    {
                        promoUserId = visitor.PromoUserId;
                        if (visitor.PromoUserId != null && visitor.PromoUserId > 0) registerSource = "推广码";

                        visitor.IsRegistered = Contracts.Base.CommonEnum.YesNo.是;

                        await visitorServices.Update(visitor);  // 转为正式用户
                    }

                    #region 创建正式用户

                    var nickName = "惠邻惠里 ";
                    getPhoneNumber.phone_info.phoneNumber
                    .Substring(getPhoneNumber.phone_info.phoneNumber.length() - 4);

                    sysUserInfo = SysUserInfo.CreateMiniUser(wechatUser.openid, "", nickName, getPhoneNumber.phone_info.phoneNumber, registerSource, null, promoUserId);
                    //try
                    //{

                    sysUserInfo.Id = await userServices.Add(sysUserInfo); // 创建用户

                    // 添加用户角色关系
                    var userRole = new UserRole(sysUserInfo.Id, 10004);

                    await userRoleServices.Add(userRole);   // 创建用户角色关系

                    // 创建用户钱包 事件监听？
                    await userWalletsServices.Add(UserWallets.Create(sysUserInfo.Id));
                    
                    unitOfWorkManage.CommitTran();
                    
                }
                catch (Exception ex)
                {
                    unitOfWorkManage.RollbackTran();

                    return Failed<LoginUserInfo>();
                }
                #endregion
                //token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = 0, Role = roles, Name = "游客", TenantId = 0 }, true, visitorId);
            }
            var roles = await userServices.GetUserRoleNameStr(sysUserInfo.Id);
            token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = sysUserInfo.Id, Role = roles, Name = sysUserInfo.Nickname, TenantId = 0 }, false);
            if (!string.IsNullOrEmpty(token))
            {
                var tokenModel = new LoginUserInfo
                {
                    success = true,
                    token = token,
                    token_type = "Bearer",
                    expires_in = _jwtOptions.ExpirationSeconds,
                    userInfo = new LoginUserInfoObj
                    {
                        userId = sysUserInfo.Id,
                        IsVisitor = false,
                        nickName = sysUserInfo.Nickname
                        //PhoneNumber = sysUserInfo.Phone
                    }
                };
                return Success(tokenModel);
            }
            return Failed<LoginUserInfo>("授权失败");

            /*var isVisitor = loginUser.GetIsVisitor();
            WcPhoneReq wcPhoneReq = new WcPhoneReq(req.PhoneNumberCode);
            var token = string.Empty;
            var roles = string.Empty;
            var user = new SysUserInfo();
            if (isVisitor)
            {
                var visitorId = loginUser.GetVisitorId();
                var visitor = await visitorServices.QueryById(visitorId);
                wcPhoneReq.openid = visitor.OpenId;

                #region 手机号归属验证

                var getPhoneNumber = await wechatApi.GetPhoneInfoAsync(wcPhoneReq);
                var msgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(getPhoneNumber);
                //if (msgModel.status != 200) return msgModel;
                if (msgModel.status != 200)
                {
                    getPhoneNumber.phone_info = new();
                    getPhoneNumber.phone_info.phoneNumber = "test";
                }
                #endregion

                #region 创建正式用户

                var nickName = "惠邻惠里 " + getPhoneNumber.phone_info.phoneNumber;
                var registerSource = "直接访问";
                if (visitor.PromoUserId != null) registerSource = "推广码";
                user = SysUserInfo.CreateMiniUser(visitor.OpenId, "", nickName, getPhoneNumber.phone_info.phoneNumber, registerSource, null, visitor.PromoUserId);

                try
                {
                    user.Id = await userServices.Add(user); // 创建用户
                                                            // 测试阶段先给10000 超级管理员
                    var userRole = new UserRole(user.Id, 10000);
                    await userRoleServices.Add(userRole);   // 创建用户角色关系

                    visitor.IsRegistered = Contracts.Base.CommonEnum.YesNo.是;
                    await visitorServices.Update(visitor);  // 转为正式用户
                }
                catch (Exception ex)
                {
                    unitOfWorkManage.RollbackTran();
                    return Failed<LoginUserInfo>();
                }

                #endregion
                //token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = 0, Role = roles, Name = "游客", TenantId = 0 }, true, visitorId);
            }
            else
            {
                user = await userServices.QueryById(loginUser.ID);
                wcPhoneReq.openid = user.OpenId;

                #region 手机号归属验证

                var getPhoneNumber = await wechatApi.GetPhoneInfoAsync(wcPhoneReq);
                var msgModel = WechatApiResHandle.ReturnMsg<LoginUserInfo>(getPhoneNumber);
                //if (msgModel.status != 200) return msgModel;
                if (msgModel.status != 200)
                {
                    getPhoneNumber.phone_info = new();
                    getPhoneNumber.phone_info.phoneNumber = "test";
                }

                #endregion
            }

            roles = await userServices.GetUserRoleNameStr(user.Id);
            token = _tokens.Issue2(_jwtOptions, new TokenModelJwt { Uid = user.Id, Role = roles, Name = user.LoginName, TenantId = user.TenantId }, false);

            var tokenModel = new LoginUserInfo
            {
                success = true,
                token = token,
                token_type = "Bearer",
                expires_in = _jwtOptions.ExpirationSeconds,
                userInfo = new LoginUserInfoObj
                {
                    userId = user.Id,
                    IsVisitor = false,
                    nickName = user.Nickname,
                    PhoneNumber = user.Phone,
                }
            };*/

            //return Success(data: tokenModel);
        }
    }

    public class LoginUserInfo : TokenInfoViewModel
    {
        public LoginUserInfoObj userInfo { get; set; } = new();
    }

    public class LoginUserInfoObj
    {
        /// <summary>
        /// 正式用户Id
        /// </summary>
        public long? userId { get; set; }

        /// <summary>
        /// 游客Id
        /// </summary>
        public long? visitorId { get; set; }

        public string nickName { get; set; }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return CommonHelper.MaskPhoneNumber(_phoneNumber); }
            set { _phoneNumber = value; }
        }

        public bool IsVisitor { get; set; }
    }
}
