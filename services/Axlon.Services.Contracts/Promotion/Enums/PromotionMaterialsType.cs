using System.ComponentModel;

namespace Axlon.Services.Contracts.Promotion.Enums
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public enum PromotionMaterialsType
    {
        /// <summary>
        /// 海报
        /// </summary>
        [Description("海报")]
        poster,

        /// <summary>
        /// 话术
        /// </summary>
        [Description("话术")]
        script,

        /// <summary>
        /// 名片模板
        /// </summary>
        [Description("名片模板")]
        card_template
    }
}
