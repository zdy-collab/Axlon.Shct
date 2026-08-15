using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Models.RootTkey
{
    public class VisitorsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 来源推广人ID（如有）
        /// </summary>
        [SugarColumn(ColumnName = "promo_user_id", IsNullable = true)]
        public Tkey PromoUserId { get; set; }
    }
}
