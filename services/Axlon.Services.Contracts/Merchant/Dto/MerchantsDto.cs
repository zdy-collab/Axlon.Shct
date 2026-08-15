using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Category.Dto;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Contracts.Merchant.JsonObj;
using Axlon.Services.Contracts.Product.Dto;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto
{
    #region req

    /// <summary>
    /// 附近商家查询请求
    /// </summary>
    public class NearbyMerchantQueryReq: LongitudeLatitude
    {
        /// <summary>
        /// 取数据条数
        /// </summary>
        public int dataCount { get; set; } = 3;
    }

    /// <summary>
    /// 为你推荐商家查询请求
    /// </summary>
    public class RecommendMerchantQueryReq : LongitudeLatitude
    {
    }

    /// <summary>
    /// 商家列表
    /// </summary>
    public class SearchMerchantQueryReq : LongitudeLatitude
    {
        //public LongitudeLatitude Location { get; set; }

        /// <summary>
        /// 已获取商家Id
        /// </summary>
        public List<long> MerchantsIds { get; set; }

        /// <summary>
        /// 品类ID筛选
        /// </summary>
        public List<long> CategoryIds { get; set; }

        /// <summary>
        /// 筛选规则，默认距离优先
        /// </summary>
        public SortType SortBy { get; set; } = SortType.distance;

        ///// <summary>
        ///// 取的数据条数
        ///// </summary>
        //public int pageSize { get; set; } = 10;

        ///// <summary>
        ///// 页数
        ///// </summary>
        //public int page { get; set; } = 1;

        /// <summary>
        /// 距离
        /// </summary>
        public int Mater { get; set; }
    }

    public class MiniGetMerchantDetailsReq : LongitudeLatitude
    {
        public long MerchantId { get; set; }
    }

    /// <summary>
    /// 根据经纬度获取附近商家列表
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="Latitude">纬度</param>
    public class ByJwGetMerchantListReq : QueryPage 
    {
        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

    }

    /// <summary>
    /// 根据Id获取商家信息
    /// </summary>
    /// <typeparam name="Tkey">主键类型</typeparam>
    public class ByIdGetMerchantsReq
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long id { get; set; }
    }

    #region 根据条件查询商家
    /// <summary>
    /// 根据条件查询商家
    /// </summary>
    public class QueryMerchantsReq
    {
        public LongitudeLatitude location { get; set; }

        public List<long> merchantsIds { get; set; }

        /// <summary>
        /// 品类ID筛选
        /// </summary>
        public List<long> categoryIds { get; set; }

        /// <summary>
        /// 筛选规则，默认距离优先
        /// </summary>
        public SortType sortBy { get; set; } = SortType.distance;

        /// <summary>
        /// 取数据条数
        /// </summary>
        public int dataCount { get; set; } = 4;

        public int mater { get; set; }
    }

    public enum SortType 
    {
        /// <summary>
        /// 距离优先
        /// </summary>
        distance = 1,

        /// <summary>
        /// 评分优先
        /// </summary>
        score = 2,

        /// <summary>
        /// 销量优先
        /// </summary>
        salesVolume = 3
    }
    #endregion

    #endregion

    #region res

    
    public class ByJwGetMerchantListRes
    {
        /// <summary>
        /// 商家信息
        /// </summary>
        public MerchantsDto merchants { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public int meter { get; set; }
    }

    public class MerchantsMeterDto
    {
        public MerchantsDto merchants { get; set; }

        public int? meter { get; set; }
    }

    public class QueryMerchantsRes
    {
        public List<MerchantsMeterDto> merchantsMeters;

        /// <summary>
        /// 返回的数据总数
        /// </summary>
        public int DataCount { get { return merchantsMeters.Count; } }
    }

    public class ByIdGetMerchantsRes
    {
        /// <summary>
        /// 商家信息
        /// </summary>
        public MerchantsDto merchants { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public int meter { get; set; }
    }
    #endregion

    #region dto
    public class MerchantsDto
    {
        public long Id { get; set; }

        public long? CityId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// GeoHash编码
        /// </summary>
        public string GeoHash { get; set; }

        /// <summary>
        /// 门店Logo URL
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 营业执照URL
        /// </summary>
        public string BusinessLicense { get; set; }

        /// <summary>
        /// 营业时间
        /// </summary>
        public WeeklyBusinessHours BusinessHours { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 门店公告
        /// </summary>
        public string Announcement { get; set; }

        /// <summary>
        /// 1基础版/2高级版
        /// </summary>
        public SaasVersion SaasVersion { get; set; } = SaasVersion.基础版;

        /// <summary>
        /// SaaS到期时间
        /// </summary>
        public DateTime? SaasExpireAt { get; set; }

        /// <summary>
        /// 0否/1是大商家（预约权限）
        /// </summary>
        public YesNo IsBigMerchant { get; set; } = 0;

        /// <summary>
        /// 0待审核/1已通过/2已驳回/3已冻结
        /// </summary>
        public MerchantStatus Status { get; set; } = 0;

        /// <summary>
        /// 审核驳回原因
        /// </summary>
        public string AuditReason { get; set; }
    }
    #endregion
}
