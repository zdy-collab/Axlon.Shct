using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Models.Enums;
using Axlon.Services.Contracts.Models.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Models
{
    [SugarTable("visitors", "游客表")]
    public class Visitors : VisitorsRoot<long>
    {
        public void Create(string openid, Source source, long? promoUserId)
        {
            OpenId = openid;
            Source = source.ToString();
            if (promoUserId != null) PromoUserId = promoUserId.Value;
            FirstVisitTime = DateTime.Now;
            LastVisitTime = DateTime.Now;
            VisitCount += 1;
            IsRegistered = YesNo.否;
        }

        public void Login()
        {
            LastVisitTime = DateTime.Now;
            VisitCount += 1;
        }

        /// <summary>
        /// 微信openid
        /// </summary>
        [SugarColumn(ColumnName = "openid", Length = 500, IsNullable = false,
            ColumnDescription = "微信openid")]
        public string OpenId { get; set; }


        /// <summary>
        /// 首次访问时间
        /// </summary>
        [SugarColumn(ColumnName = "first_visit_time", IsNullable = false,
            ColumnDescription = "首次访问时间")]
        public DateTime FirstVisitTime { get; set; }


        /// <summary>
        /// 最近访问时间
        /// </summary>
        [SugarColumn(ColumnName = "last_visit_time", IsNullable = false,
            ColumnDescription = "最近访问时间")]
        public DateTime LastVisitTime { get; set; }


        /// <summary>
        /// 访问次数
        /// </summary>
        [SugarColumn(ColumnName = "visit_count", IsNullable = false,
            ColumnDescription = "访问次数")]
        public int VisitCount { get; set; }


        /// <summary>
        /// 来源（搜索/分享/推广码）
        /// </summary>
        [SugarColumn(ColumnName = "source", Length = 50, IsNullable = true,
            ColumnDescription = "来源（搜索/分享/推广码）")]
        public string Source { get; set; }


        /// <summary>
        /// 是否已转为正式用户 0否/1是
        /// 【什么时候转需根据业务规则确定】
        /// </summary>
        [SugarColumn(ColumnName = "is_registered", ColumnDataType = "tinyint", IsNullable = false,
            ColumnDescription = "是否已转为正式用户 0否/1是")]
        public YesNo IsRegistered { get; set; }
    }
}
