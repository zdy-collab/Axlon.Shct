using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    public enum OrderPaymentsPayType
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")]
        wechat,

        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        alipay,

        /// <summary>
        /// 现金（线下支付）
        /// </summary>
        [Description("现金")]
        cash,

        /// <summary>
        /// 组合支付（如：部分现金+部分微信）
        /// </summary>
        [Description("组合支付")]
        combined
    }
}
