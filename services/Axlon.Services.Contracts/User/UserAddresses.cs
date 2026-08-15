using Axlon.Services.Contracts.Models;
using SqlSugar;

namespace Axlon.Services.Contracts.User
{
    /// <summary>
    /// 用户地址
    /// </summary>
    [Tenant("Main")]
    [SugarTable("user_addresses", "用户地址")]
    public class UserAddress : AuditRoot
    {

        /// <summary>
        /// 用户id
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [SugarColumn(ColumnName = "province", Length = 50)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 市
        /// </summary>
        [SugarColumn(ColumnName = "city", Length = 50)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "district", Length = 50)]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址
        /// </summary>
        [SugarColumn(ColumnName = "address", Length = 500, IsNullable = false)]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [SugarColumn(ColumnName = "longitude", DecimalDigits = 7)]
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [SugarColumn(ColumnName = "latitude", DecimalDigits = 7)]
        public decimal Latitude { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [SugarColumn(ColumnName = "contact_name", Length = 50, IsNullable = false)]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "contact_phone", Length = 20, IsNullable = false)]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 是否默认地址
        /// </summary>
        [SugarColumn(ColumnName = "is_default")]
        public bool IsDefault { get; set; }
    }
}
