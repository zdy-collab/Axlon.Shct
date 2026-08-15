using Axlon.Services.Contracts.Wechat.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Wechat.Dto
{
    public class GetUnlimitedQRCodeRes:WechatBaseRes
    {
        /// <summary>
        /// 图片 Buffer
        /// </summary>
        public byte[]? buffer { get; set; }

        public string fileType { get; set; }
    }
}
