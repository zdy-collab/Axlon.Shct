using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 用户钱包
    /// </summary>
    [Tenant("Main")]
    [SugarTable("user_wallets", "用户钱包")]
    public class UserWallets : UserWalletsRoot<long>
    {

        /// <summary>
        /// 创建用户钱包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserWallets Create(long userId) 
        {
            return new UserWallets
            {
                UserId = userId,
                Balance = 0,
                FrozenBalance = 0
            };
        }

        /// <summary>
        /// 可用余额
        /// </summary>
        [SugarColumn(ColumnName = "balance", DecimalDigits = 2, ColumnDescription = "可用余额")]
        public decimal Balance { get; set; }

        /// <summary>
        /// 冻结余额（提现中）
        /// </summary>
        [SugarColumn(ColumnName = "frozen_balance", DecimalDigits = 2, ColumnDescription = "冻结余额（提现中）")]
        public decimal FrozenBalance { get; set; }
    }
}
