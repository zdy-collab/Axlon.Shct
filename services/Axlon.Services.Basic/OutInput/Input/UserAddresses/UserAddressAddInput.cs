using Axlon.Framework.Abstractions.IDto;
using System.ComponentModel.DataAnnotations;

namespace Axlon.Services.Basic.Input
{
    public class UserAddressPageInput : IPageRequest
    {
        public string Keyword { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderColumnName { get; set; }
        public bool Asc { get; set; }
    }

    public class UserAddressAddInput : IAddInput
    {
        /// <summary>
        /// 联系人名称
        /// </summary>
        [Required, StringLength(50)]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [Required, StringLength(20), RegularExpression(@"^\+?[0-9\- ]{6,20}$")]
        public string ContactPhone { get; set; } = string.Empty;

        /// <summary>
        /// 省
        /// </summary>
        [Required, StringLength(50)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 市
        /// </summary>
        [Required, StringLength(50)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 区
        /// </summary>
        [Required, StringLength(50)]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Range(-180d, 180d)]
        public decimal Longitude { get; set; }

        [Range(-90d, 90d)]
        public decimal Latitude { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
    }

    public sealed class UserAddressEditInput : UserAddressAddInput, IEditInput
    {
        public long Id { get; set; }
    }

}
