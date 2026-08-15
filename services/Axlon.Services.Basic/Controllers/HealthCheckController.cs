using Axlon.Framework.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.Controllers
{
    /// <summary>
    /// 健康检查（向后兼容 /healthcheck 端点；Aspire 标准 /health 与 /alive 由 ServiceDefaults 提供）。
    /// </summary>
    [Route("api/basic/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthCheckController : BaseApiController
    {
        /// <summary>
        /// 健康检查接口
        /// </summary>
        [HttpGet]
        [Route("/healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy", timestamp = DateTime.Now });
        }
    }
}
