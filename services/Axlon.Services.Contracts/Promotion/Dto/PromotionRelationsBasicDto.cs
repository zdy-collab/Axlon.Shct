using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Dto
{
    public class PromotionRelationsBasicDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 被推广人ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 上级推广人ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 绑定订单ID
        /// </summary>
        public long BindOrderId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public byte Level { get; set; }
    }
}
