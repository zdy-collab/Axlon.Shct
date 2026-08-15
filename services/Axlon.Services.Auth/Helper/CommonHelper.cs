namespace Axlon.Services.Auth.Helper
{
    public class CommonHelper
    {
        /// <summary>
        /// 手机号脱敏处理
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string MaskPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            // 去除空格和特殊字符
            phone = phone.Trim();

            // 如果长度小于7位（不足以脱敏），直接返回原值
            if (phone.Length < 7)
                return phone;

            // 动态计算：保留前3位，后4位，中间全部变成 *
            int prefixLen = 3;
            int suffixLen = 4;
            int maskLen = phone.Length - prefixLen - suffixLen;

            if (maskLen <= 0)
                return phone;

            string mask = new string('*', maskLen);
            return phone.Substring(0, prefixLen) + mask + phone.Substring(phone.Length - suffixLen);
        }
    }
}
