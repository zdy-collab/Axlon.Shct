using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Dto
{
    /// <summary>
    /// 绑定推广关系
    /// </summary>
    public class BindPromotionRelationReq
    {
        /// <summary>
        /// 被推广人Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 上级推广人Id
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public long BindOrderId { get; set; }
    }
}
