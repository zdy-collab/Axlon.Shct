using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    /// <summary>
    /// 商家表
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    public class MerchantsRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        ///// <summary>
        ///// 主键
        ///// </summary>
        //[SugarColumn(IsNullable = false, IsPrimaryKey = true,ColumnName = "id")]
        //public Tkey Id { get; set; }

        /// <summary>
        /// 城市id -> cities.id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "city_id")]
        public Tkey? CityId { get; set; }

        /// <summary>
        /// 系统用户Id -> sysuserinfo.id
        /// </summary>

        [SugarColumn(IsNullable = true, ColumnName = "sysuser_id")]
        public Tkey? SysUserId { get; set; }

        /// <summary>
        /// 门店Logo文件Id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "logo_file_Id")]
        public Tkey LogoFileId { get; set; }

        /// <summary>
        /// 营业执照文件Id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "busines_license_file_Id")]
        public Tkey BusinessLicenseFileId { get; set; }

    }
}
