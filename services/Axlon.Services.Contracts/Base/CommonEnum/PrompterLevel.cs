using System.ComponentModel;

namespace Axlon.Services.Contracts.Base.CommonEnum
{
    /// <summary>
    /// 推广等级
    /// </summary>
    public enum PrompterLevel
    {
        /// <summary>
        /// 普通推广员
        /// </summary>
        [Description("普通推广员")]
        normal,

        /// <summary>
        /// 银牌推广员
        /// </summary>
        [Description("银牌推广员")]
        silver,

        /// <summary>
        /// 金牌推广员
        /// </summary>
        [Description("金牌推广员")]
        gold,

        /// <summary>
        /// 合伙人
        /// </summary>
        [Description("城市合伙人")]
        partner
    }
}
