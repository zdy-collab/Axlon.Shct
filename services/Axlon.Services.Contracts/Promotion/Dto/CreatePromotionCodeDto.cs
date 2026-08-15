using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Dto
{
    public record class CreatePromotionCodeReq(string Scene, string PagePath);
}
