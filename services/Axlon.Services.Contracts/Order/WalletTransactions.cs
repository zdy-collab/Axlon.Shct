using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 钱包流水
    /// </summary>
    [Tenant("Main")]
    [SugarTable("wallet_transactions", "钱包流水")]
    public class WalletTransactions : WalletTransactionsRoot<long>
    {

        /// <summary>
        /// 创建流水
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="orderId">来源订单</param>
        /// <param name="type">交易类型</param>
        /// <param name="amount">交易金额</param>
        /// <param name="balanceBefore">变动前余额</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        public static WalletTransactions Create(long userId,long orderId, WalletTransactionsType type,decimal amount,decimal balanceBefore,string description = null) 
        {
            return new WalletTransactions
            {
                UserId = userId,
                OrderId = orderId,
                Type = type.ToString(),
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore + amount,
                Description = description
            };
        }

        /// <summary>
        /// 交易类型（使用枚举，存储时转为字符串）
        /// WalletTransactionsType
        /// </summary>
        [SugarColumn(ColumnName = "type", IsNullable = false, ColumnDataType = "varchar(30)", ColumnDescription = "交易类型")]
        public string Type { get; set; }

        /// <summary>
        /// 金额（正=收入，负=支出）
        /// </summary>
        [SugarColumn(ColumnName = "amount", DecimalDigits = 2, ColumnDescription = "金额（正=收入，负=支出）")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 变动前余额
        /// </summary>
        [SugarColumn(ColumnName = "balance_before", DecimalDigits = 2, ColumnDescription = "变动前余额")]
        public decimal BalanceBefore { get; set; }

        /// <summary>
        /// 变动后余额
        /// </summary>
        [SugarColumn(ColumnName = "balance_after", DecimalDigits = 2, ColumnDescription = "变动后余额")]
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "description", Length = 500, IsNullable = true, ColumnDescription = "描述")]
        public string Description { get; set; }
    }
}
