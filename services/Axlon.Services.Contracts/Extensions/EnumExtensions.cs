using System.ComponentModel;
using System.Reflection;

namespace Axlon.Services.Contracts.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值的Description特性描述
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            // 获取枚举字段信息
            FieldInfo field = value.GetType().GetField(value.ToString());

            // 尝试获取DescriptionAttribute
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            // 如果有Description特性则返回其描述，否则返回枚举名称
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
