namespace Axlon.Services.Contracts.GroupBuy.JsonObj
{
    /// <summary>
    /// 使用规则（有效期、可用时段等）
    /// </summary>
    public class UseRules
    {
        /// <summary>
        /// 有效期（单位：天）
        /// </summary>
        public int valid_days { get; set; }

        /// <summary>
        /// 可用时段
        /// </summary>
        public List<UsableTime> usable_Time { get; set; }
    }

    /// <summary>
    /// 可用时段
    /// </summary>
    public class UsableTime
    {
        /// <summary>
        /// 开始时间（格式：HH:mm）
        /// </summary>
        public string start { get; set; }
        /// <summary>
        /// 结束时间（格式：HH:mm）
        /// </summary>
        public string end { get; set; }
    }
}
