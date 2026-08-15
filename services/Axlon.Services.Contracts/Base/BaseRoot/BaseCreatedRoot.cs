using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Base.BaseRoot
{
    public class BaseCreatedRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "created_at", IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
