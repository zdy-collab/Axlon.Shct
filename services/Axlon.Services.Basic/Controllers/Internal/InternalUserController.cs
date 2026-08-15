using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Basic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers.Internal
{
    [Route("api/basic/internal/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class InternalUserController : BaseApiController
    {
        private readonly ISysUserInfoServices sysUserInfoServices;
        private readonly IUser loginUser;

        public InternalUserController(ISysUserInfoServices sysUserInfoServices, IUser loginUser)
        {
            this.sysUserInfoServices = sysUserInfoServices;
            this.loginUser = loginUser;
        }

        /// <summary>
        /// 根据用户Id获取推广人Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("getPromotionIdAsync")]
        public async Task<MessageModel<long?>> GetPromotionIdAsync() 
        {
            return Success(data: (await sysUserInfoServices.QueryById(loginUser.ID)).RegisterFromUserId);
        }
    }
}
