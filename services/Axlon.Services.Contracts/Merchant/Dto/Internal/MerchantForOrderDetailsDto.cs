using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto.Inner
{
    public class MerchantForOrderDetailsDto: MerchantBasicDto
    {
        /// <summary>
        /// 桌号（如：A01、B02、VIP03）
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 区域（大厅/包间/卡座/包厢）,Enum：TableArea
        /// </summary>
        public string Area { get; set; }
    }
}
