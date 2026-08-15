namespace Axlon.Services.Contracts.Order.Dto.WalletDto
{
    public class GetMyWalletInfoReq
    {

    }

    public class GetMyWalletInfoRes
    {
        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 冻结余额（提现中）
        /// </summary>
        public decimal FrozenBalance { get; set; }

        /// <summary>
        /// 今日收益
        /// </summary>
        public decimal TodayEarnings { get; set; }

        /// <summary>
        /// 本月累计
        /// </summary>
        public decimal MonthEarnings { get; set; }

        /// <summary>
        /// 累计总收益
        /// </summary>
        public decimal TotalEarnings { get; set; }

        /// <summary>
        /// 收益明细
        /// </summary>
        public List<RevenueDetailsDto> revenueBreakdown { get; set; }
    }
}
