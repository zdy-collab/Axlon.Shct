using Axlon.Framework.Abstractions.IDto;
using Axlon.Services.Contracts.Enums;

namespace Axlon.Services.Basic.Output
{
    /// <summary>
    /// 用户足迹输出
    /// </summary>
    public class UserFootprintOutput : IDetailOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 商家id
        /// </summary>
        public long? MerchantId { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string? MerchantName { get; set; }

        public string TargetType { get; set; } = string.Empty;

        public long? TargetId { get; set; }

        public string TargetKey { get; set; } = string.Empty;

        public string? PageCode { get; set; }

        public string? TargetTitle { get; set; }

        public string? TargetImage { get; set; }

        /// <summary>
        /// 足迹类型    
        /// </summary>
        public FootprintTypeEnum FootprintType { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int OccurrenceCount { get; set; } = 1;

        /// <summary>
        /// 更新时间
        /// </summary>      
        public DateTime? ModifyTime { get; set; } = DateTime.Now;

        public DateTime? CreateTime { get; set; }
    }
}
