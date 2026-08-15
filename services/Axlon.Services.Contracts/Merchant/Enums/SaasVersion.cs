using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// SaaS版本枚举
    /// </summary>
    public enum SaasVersion
    {
        [Description("基础版")]
        基础版 = 1,

        [Description("高级版")]
        高级版 = 2
    }
}
