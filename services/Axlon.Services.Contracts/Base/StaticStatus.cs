using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Base
{
    public class StaticStatus
    {
        // 是否全局文件资源返回Oss地址
        public static bool ReturnOssStatus = true;

        // 商家GeHash筛选
        public static bool MerchantGoHashStatus = false;

    }
}
