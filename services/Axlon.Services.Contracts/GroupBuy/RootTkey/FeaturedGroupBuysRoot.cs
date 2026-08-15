using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.GroupBuy.RootTkey
{
    public class FeaturedGroupBuysRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 团购ID，关联 group_buys.id
        /// </summary>
        [SugarColumn(ColumnName = "group_buy_id", ColumnDescription = "团购ID", IsNullable = false)]
        public Tkey GroupBuyId { get; set; }
    }
}
