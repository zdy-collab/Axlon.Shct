using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    public enum OrderPayStatus
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [Description("未支付")]
        unpaid,

        /// <summary>
        /// 已支付
        /// </summary>
        [Description("已支付")]
        paid,

        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        refunding,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        refunded
    }
}
