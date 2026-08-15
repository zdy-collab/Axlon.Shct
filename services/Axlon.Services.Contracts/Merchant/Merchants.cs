using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.GroupBuy;
using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Contracts.Merchant.JsonObj;
using Axlon.Services.Contracts.Merchant.RootTkey;
using Axlon.Services.Contracts.Product;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant
{
    /// <summary>
    /// 商家表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("merchants", "商家表")]
    [SugarIndex("idx_merchants_geo_hash", nameof(GeoHash), OrderByType.Asc)]
    [SugarIndex("idx_merchants_city_id", nameof(CityId), OrderByType.Asc)]
    [SugarIndex("idx_merchants_status", nameof(Status), OrderByType.Asc)]
    [SugarIndex("idx_merchants_geo_hash", nameof(GeoHash), OrderByType.Asc)]
    [SugarIndex("idx_merchants_longitude_latitude", nameof(Longitude), OrderByType.Asc, nameof(Latitude), OrderByType.Asc)]


    public class Merchants : MerchantsRoot<long>, IAggregateRoot
    {

        /// <summary>
        /// 门店名称
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = false)]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>

        [SugarColumn(ColumnDataType = "decimal(10,7)", IsNullable = false)]
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>

        [SugarColumn(ColumnDataType = "decimal(10,7)", IsNullable = false)]
        public decimal Latitude { get; set; }

        /// <summary>
        /// GeoHash编码
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(12)", IsNullable = true, ColumnName = "geo_hash")]
        public string GeoHash { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "logo_oss")]

        public string LogoOss { get; set; }

        ///// <summary>
        ///// 门店Logo URL
        ///// </summary>

        //[SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        //public string Logo { get; set; }

        /// <summary>
        /// 营业执照URL
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true, ColumnName = "business_license")]
        public string BusinessLicense { get; set; }

        /// <summary>
        /// 营业时间
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(500)", IsJson = true, IsNullable = true, ColumnName = "business_hours")]
        public WeeklyBusinessHours BusinessHours { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(20)", IsNullable = true)]
        public string Phone { get; set; }

        /// <summary>
        /// 门店公告
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        public string Announcement { get; set; }

        /// <summary>
        /// 1基础版/2高级版
        /// </summary>

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", DefaultValue = "1", ColumnName = "saas_version")]
        public SaasVersion SaasVersion { get; set; } = SaasVersion.基础版;

        /// <summary>
        /// SaaS到期时间
        /// </summary>

        [SugarColumn(IsNullable = true, ColumnName = "saas_expire_at")]
        public DateTime? SaasExpireAt { get; set; }

        /// <summary>
        /// 0否/1是大商家（预约权限）
        /// </summary>

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", DefaultValue = "0", ColumnName = "is_big_merchant")]
        public YesNo IsBigMerchant { get; set; } = 0;

        /// <summary>
        /// 0待审核/1已通过/2已驳回/3已冻结
        /// </summary>

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", DefaultValue = "0")]
        public MerchantStatus Status { get; set; } = 0;

        /// <summary>
        /// 审核驳回原因
        /// </summary>

        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true, ColumnName = "audit_reason")]
        public string AuditReason { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(MerchantCategoryConfig.MerchantId), nameof(Id))]
        public List<MerchantCategoryConfig> merchantCategoryConfigs { get; set; }

        /// <summary>
        /// 团购
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(GroupBuys.MerchantId), nameof(Id))]
        public List<GroupBuys> groupBuys { get; set; }


        [Navigate(NavigateType.OneToMany, nameof(ProductCategories.MerchantId), nameof(Id))]
        public List<ProductCategories> productCategories { get; set; }

        /// <summary>
        /// 桌台
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(MerchantTable.MerchantId), nameof(Id))]
        public List<MerchantTable> merchantTables { get; set; }

    }
}
