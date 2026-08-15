using Axlon.Services.Contracts.Merchant.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto
{
    public class MerchantTableBasicDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 商家ID（关联 merchants.id）
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 桌号（如：A01、B02、VIP03）
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 区域（大厅/包间/卡座/包厢）,Enum：TableArea
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 人数上限
        /// </summary>
        public int Capacity { get; set; } = 1;

        /// <summary>
        /// 桌台状态：0-空闲 / 1-已开台 / 2-待结账 / 3-停用
        /// </summary>
        //public TableStatus Status { get; set; }
    }
}
