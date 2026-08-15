using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Dto
{
    public class QrSceneBasicDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 微信 scene 参数（唯一标识）
        /// </summary>
        public string Scene { get; set; }

        /// <summary>
        /// 二维码类型
        /// </summary>
        public QrSceneType Type { get; set; }

        /// <summary>
        /// 跳转页面路径
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 扩展参数（JSON格式）
        /// </summary>
        public string ExtraJson { get; set; }
    }
}
