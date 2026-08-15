namespace Axlon.Services.Contracts.Promotion.JsonObj
{
    /// <summary>
    /// 权益项
    /// </summary>
    public class BenefitItem
    {
        /// <summary>
        /// 权益标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 权益描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 权益图标（可选）
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 额外数据（扩展用）
        /// </summary>
        public object Extra { get; set; }
    }
}
