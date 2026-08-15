using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto
{
    public class MerchantBasic_TableDto:MerchantBasicDto
    {
        public List<MerchantTableBasicDto> merchantTables { get; set; }
    }
}
