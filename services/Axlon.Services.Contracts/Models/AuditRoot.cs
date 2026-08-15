using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Models
{
    /// <summary>
    /// 审计实体基类
    /// </summary>
    public class AuditRoot : RootEntityTkey<long>
    {
        /// <summary>
        /// 创建者id
        /// </summary>
        [SugarColumn(ColumnName = "create_id", IsNullable = true, IsOnlyIgnoreUpdate = true)]
        public long? CreateId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 200, IsOnlyIgnoreUpdate = true)]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, IsOnlyIgnoreUpdate = true)]
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后修改人id
        /// </summary>
        [SugarColumn(ColumnName = "modify_id", IsNullable = true)]
        public long? ModifyId { get; set; }

        /// <summary>
        /// 最后修改人姓名
        /// </summary>
        [SugarColumn(ColumnName = "modify_by", IsNullable = true, Length = 200)]
        public string ModifyBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SugarColumn(ColumnName = "modify_time", IsNullable = true)]
        public DateTime? ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        ///是否删除 
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted", DefaultValue = "0")]
        public bool IsDeleted { get; set; }
    }
}
