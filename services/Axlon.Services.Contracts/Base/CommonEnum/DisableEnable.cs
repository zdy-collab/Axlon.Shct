using System.ComponentModel;

namespace Axlon.Services.Contracts.Base.CommonEnum
{
    /// <summary>
    /// 0停用，1启用
    /// </summary>
    public enum DisableEnable
    {
        [Description("停用")]

        停用 = 0,

        [Description("启用")]
        启用 = 1
    }
}
