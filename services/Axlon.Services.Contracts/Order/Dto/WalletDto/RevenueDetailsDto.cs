using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Order.Enums;
using System.Drawing;

namespace Axlon.Services.Contracts.Order.Dto.WalletDto
{
    public class RevenueDto : PageResponseModel<RevenueDetailsDto>
    {

        public RevenueDto(int page, int dataCount, int pageSize, List<RevenueDetailsDto> data) : base(page, dataCount, pageSize, data)
        {

        }

        /// <summary>
        /// 我的分成
        /// </summary>
        public decimal MyDivideInto { get; set; }

        /// <summary>
        /// 参与分成
        /// </summary>
        public decimal ParticipateDivideInto { get; set; }

    }

    /// <summary>
    /// 收益明细Dto
    /// </summary>
    public class RevenueDetailsDto
    {
        public string Name { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        public string TypeDescription
        {
            get
            {
                var state = Enum.TryParse<WalletTransactionsType>(Type, true, out WalletTransactionsType result);
                return result.GetDescription();
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
