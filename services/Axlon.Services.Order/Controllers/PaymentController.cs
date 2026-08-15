using Axlon.Framework.Abstractions;
using Axlon.Framework.Abstractions.GlobalVar;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Order.Dto.PaymentDto;
using Axlon.Services.Order.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentServices paymentServices;

        public PaymentController(IPaymentServices paymentServices)
        {
            this.paymentServices = paymentServices;
        }

        [HttpPost("payOrder")]
        public async Task<MessageModel<string>> PayOrderAsync([FromBody] PayOrderReq req) 
        {
            var state = await paymentServices.PayOrderAsync(req);
            if (state) return Success<string>("");
            else return Failed();
        }
    }
}
