using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.User.RootTkey
{
    public class UserAddressRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false)]
        public Tkey UserId { get; set; }
    }
}
