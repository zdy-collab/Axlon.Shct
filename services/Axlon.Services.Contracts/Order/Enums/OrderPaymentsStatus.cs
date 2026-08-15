using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    public enum OrderPaymentsStatus
    {
        /// <summary>
        /// 处理中（已发起，等待回调）
        /// </summary>
        [Description("处理中")]
        pending,

        /// <summary>
        /// 支付成功
        /// </summary>
        [Description("支付成功")]
        success,

        /// <summary>
        /// 支付失败
        /// </summary>
        [Description("支付失败")]
        failed,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        refund
    }
}
