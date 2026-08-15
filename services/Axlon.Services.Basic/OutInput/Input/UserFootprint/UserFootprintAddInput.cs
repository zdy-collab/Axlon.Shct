using Axlon.Framework.Abstractions.IDto;
using System.ComponentModel.DataAnnotations;

namespace Axlon.Services.Basic.Input
{
    /// <summary>
    /// 新增足迹
    /// </summary>
    public sealed class UserFootprintAddInput : IAddInput
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        [Range(1, long.MaxValue)]
        public long MerchantId { get; set; }
    }

    /// <summary>
    /// 分页
    /// </summary>
    public sealed class UserFootprintPageInput : IPageRequest
    {
        public string? Keyword { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;
        public string? OrderColumnName { get; set; }
        public bool Asc { get; set; }

        /// <summary>
        /// 目标类型：merchant/product/group_buy/content/page
        /// </summary>
        public string? TargetType { get; set; }
    }

    /// <summary>
    /// 前端页面浏览上报。PageCode 使用服务端约定的稳定编码，不传 URL。
    /// </summary>
    public sealed class UserPageViewInput
    {
        [Required, MaxLength(100)]
        public string PageCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// 编辑足迹
    /// </summary>
    public sealed class UserFootprintEditInput : IEditInput
    {
        /// <summary>
        /// id
        /// </summary>
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }
}
