using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Promotion.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 推广物料
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_materials", "推广物料")]
    public class PromotionMaterials : PromotionMaterialsRoot<long>
    {

        /// <summary>
        /// 物料标题
        /// </summary>
        [SugarColumn(ColumnName = "title", ColumnDescription = "物料标题", Length = 200, IsNullable = false)]
        public string Title { get; set; }

        /// <summary>
        /// 物料类型：poster/script/card_template
        /// </summary>
        [SugarColumn(ColumnName = "type", ColumnDescription = "poster/script/card_template", ColumnDataType = "varchar(20)", IsNullable = false)]
        public PromotionMaterialsType Type { get; set; }

        /// <summary>
        /// 物料文件URL（海报/模板文件地址）
        /// </summary>
        [SugarColumn(ColumnName = "content_url", ColumnDescription = "物料文件URL", Length = 500, IsNullable = true)]
        public string ContentUrl { get; set; }

        /// <summary>
        /// 文本内容（话术/文案内容）
        /// </summary>
        [SugarColumn(ColumnName = "content_text", ColumnDescription = "文本内容（话术）", ColumnDataType = "text", IsNullable = true)]
        public string ContentText { get; set; }

        /// <summary>
        /// 状态：1启用/0停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "1启用/0停用", ColumnDataType = "tinyint", IsNullable = false)]
        public DisableEnable Status { get; set; }

        /// <summary>
        /// 版本号（每次更新递增）
        /// </summary>
        [SugarColumn(ColumnName = "version", ColumnDescription = "版本号", IsNullable = false)]
        public int Version { get; set; }
    }
}
