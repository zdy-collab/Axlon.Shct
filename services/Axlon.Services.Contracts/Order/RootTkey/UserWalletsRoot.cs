using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class UserWalletsRoot<Tkey> : BaseUpdatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户ID（唯一）
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false)]
        public Tkey UserId { get; set; }
    }
}
