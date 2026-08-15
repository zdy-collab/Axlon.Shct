using Axlon.Services.Contracts.Base.CommonEnum;

namespace Axlon.Services.Contracts.ViewModels
{
    public class VisitorInfoDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 来源推广人ID（如有）
        /// </summary>
        public long PromoUserId { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string OpenId { get; set; }


        /// <summary>
        /// 首次访问时间
        /// </summary>
        public DateTime FirstVisitTime { get; set; }


        /// <summary>
        /// 最近访问时间
        /// </summary>
        public DateTime LastVisitTime { get; set; }


        /// <summary>
        /// 访问次数
        /// </summary>
        public int VisitCount { get; set; }


        /// <summary>
        /// 来源（搜索/分享/推广码）
        /// </summary>
        public string Source { get; set; }


        /// <summary>
        /// 是否已转为正式用户 0否/1是
        /// 【什么时候转需根据业务规则确定】
        /// </summary>
        public YesNo IsRegistered { get; set; }
    }
}
