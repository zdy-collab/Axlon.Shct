using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Promotion.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 推广关系链
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_relations", "推广关系链")]

    #region UserId、ParentId、Level 复合唯一索引

    [SugarIndex("idx_promotion_relation_unique",
        nameof(UserId),OrderByType.Asc,
        nameof(ParentId),OrderByType.Asc,
        nameof(Level),OrderByType.Asc,
    IsUnique = true)]

    #endregion
    public class PromotionRelations : PromotionRelationsRoot<long>
    {
        public static PromotionRelations Create(long userId,long parentId,long bindOrderId,byte level) 
        {
            var entity = new PromotionRelations
            {
                UserId = userId,
                ParentId = parentId,
                BindOrderId = bindOrderId,
                Level = level,
                BindTime = DateTime.Now,
                IsValid = PromotionRelationsIsValid.有效
            };

            return entity;
        }

        /// <summary>
        /// 层级：1直推/2间推/3/4/5
        /// </summary>
        [SugarColumn(ColumnName = "level", ColumnDescription = "层级：1直推/2间推/3/4/5", IsNullable = false)]
        public byte Level { get; set; }

        /// <summary>
        /// 绑定时间
        /// </summary>
        [SugarColumn(ColumnName = "bind_time", ColumnDescription = "绑定时间", IsNullable = false)]
        public DateTime BindTime { get; set; }

        /// <summary>
        /// 是否有效：1有效/0已解除（软删除）
        /// </summary>
        [SugarColumn(ColumnName = "is_valid", ColumnDataType = "tinyint", ColumnDescription = "1有效/0已解除 软删除", IsNullable = false)]
        public PromotionRelationsIsValid IsValid { get; set; }

        /// <summary>
        /// 解除原因
        /// </summary>
        [SugarColumn(ColumnName = "unbind_reason", ColumnDescription = "解除原因", Length = 500, IsNullable = true)]
        public string UnbindReason { get; set; }

        /// <summary>
        /// 解除时间
        /// </summary>
        [SugarColumn(ColumnName = "unbind_time", ColumnDescription = "解除时间", IsNullable = true)]
        public DateTime? UnbindTime { get; set; }


        [SugarColumn(IsIgnore = true)]
        public List<PromotionRelations> Child { get; set; }
    }
}
