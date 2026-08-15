using Axlon.Services.Contracts.Base;

namespace Axlon.Services.Contracts.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">fileId</param>
        /// <returns></returns>
        public static string CombinFileAccessPath(this string? filePath,string? ossAddress = null)
        {
            if (!string.IsNullOrEmpty(ossAddress) && StaticStatus.ReturnOssStatus) return ossAddress;
            var baseUrl = "http://192.168.0.200:4100/api/files/";
            var actionName = "/url";
            if (string.IsNullOrEmpty(filePath) || filePath.StartsWith(baseUrl)) return filePath;

            return baseUrl + filePath + actionName;
        }

    }
}