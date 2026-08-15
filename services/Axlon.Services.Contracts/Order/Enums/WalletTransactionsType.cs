using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    public enum WalletTransactionsType
    {
        /// <summary>
        /// 佣金收入
        /// </summary>
        [Description("佣金收入")]
        commission,

        /// <summary>
        /// 首单返现
        /// </summary>
        [Description("首单返现")]
        first_order_cashback,

        /// <summary>
        /// 提现
        /// </summary>
        [Description("提现")]
        withdraw,

        /// <summary>
        /// 退款
        /// </summary>
        [Description("退款")]
        refund,

        /// <summary>
        /// 消费
        /// </summary>
        [Description("消费")]
        consumption
    }
}
