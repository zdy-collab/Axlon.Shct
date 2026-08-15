using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Base.BaseRoot
{
    public class BaseUpdatedRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "updated_at", IsOnlyIgnoreUpdate = false)]
        public DateTime UpdatedAt { get; set; }
    }
}
