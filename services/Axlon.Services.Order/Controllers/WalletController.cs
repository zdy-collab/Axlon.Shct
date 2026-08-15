using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Order.Dto.WalletDto;
using Axlon.Services.Order.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Order.Controllers
{
    [Route("api/order/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class WalletController : BaseApiController
    {
        private readonly IWalletServices walletMiniServices;

        public WalletController(IWalletServices walletMiniServices)
        {
            this.walletMiniServices = walletMiniServices;
        }

        /// <summary>
        /// 获取我的钱包信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("getMyWalletInfo")]
        public async Task<MessageModel<GetMyWalletInfoRes>> GetMyWalletInfoAsync()
        {
            return Success(data: await walletMiniServices.GetMyWalletInfoAsync());
        }

        /// <summary>
        /// 获取我的收益明细
        /// </summary>
        /// <param name="queryPage"></param>
        /// <returns></returns>
        [HttpGet("getMyRevenueDetails")]
        public async Task<MessageModel<RevenueDto>> GetMyRevenueDetailsAsync([FromQuery] QueryPage queryPage)
        {
            return Success(data: await walletMiniServices.GetMyRevenueDetailsAsync(queryPage));

        }
    }
}
