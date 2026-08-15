using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.RootTkey
{
    public class QrSceneRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false, ColumnDescription = "用户Id")]
        public Tkey UserId { get; set; }

        /// <summary>
        /// 图片文件Id
        /// </summary>
        [SugarColumn(ColumnName = "image_file_id", ColumnDescription = "图片文件Id", IsNullable = true)]

        public Tkey ImageFileId { get; set; }
    }
}
