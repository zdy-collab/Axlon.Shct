using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Promotion.RootTkey;
using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 二维码
    /// </summary>
    [Tenant("Main")]
    [SugarTable("qr_scene","二维码")]
    [SugarIndex("idx_qr_scene_scene", nameof(Scene), OrderByType.Asc, IsUnique = true)]
    public class QrScene:QrSceneRoot<long>
    {
        public static QrScene Create(CreateQrSceneCommand command) 
        {
            var entity = command.Adapt<QrScene>();
            entity.Status = DisableEnable.启用;
            return entity;
        }

        /// <summary>
        /// 微信 scene 参数（唯一标识）
        /// </summary>
        [SugarColumn(ColumnName = "scene",
                     ColumnDataType = "varchar",
                     Length = 32,
                     IsNullable = false,
                     ColumnDescription = "微信 scene 参数（唯一）")]
        public string Scene { get; set; }

        /// <summary>
        /// 二维码类型
        /// </summary>
        [SugarColumn(ColumnName = "type",
                     ColumnDataType = "tinyint",
                     IsNullable = false,
                     ColumnDescription = "二维码类型")]
        public QrSceneType Type { get; set; }

        /// <summary>
        /// 跳转页面路径
        /// </summary>
        [SugarColumn(ColumnName = "page",
                     ColumnDataType = "varchar",
                     Length = 100,
                     IsNullable = true,
                     ColumnDescription = "跳转页面")]
        public string Page { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [SugarColumn(ColumnName = "file_path",
             ColumnDataType = "varchar",
             Length = 300,
             IsNullable = true,
             ColumnDescription = "文件路径")]
        public string FilePath { get; set; }

        /// <summary>
        /// 扩展参数（JSON格式）
        /// </summary>
        [SugarColumn(ColumnName = "extra_json",
                     ColumnDataType = "json",
                     IsNullable = true,
                     ColumnDescription = "扩展参数")]
        public string ExtraJson { get; set; }

        /// <summary>
        /// 状态：1启用 0停用
        /// </summary>
        [SugarColumn(ColumnName = "status",
                     ColumnDataType = "tinyint",
                     IsNullable = false,
                     ColumnDescription = "1启用 0停用")]
        public DisableEnable Status { get; set; }
    }

    public record CreateQrSceneCommand(long UserId,long ImageFileId, string Scene, QrSceneType Type, string Page, string ExtraJson = null);
}
