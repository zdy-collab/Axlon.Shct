using Axlon.Services.Contracts.Category.Dto;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.GroupBuy.Dto;
using Axlon.Services.Contracts.Product.Dto;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto
{
    public class MerchantInfoDto
    {
        public long Id { get; set; }

        public long LogoFileId { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string Name { get; set; }

        public string LogoOss { get; set; }

        /// <summary>
        /// Logo路径
        /// </summary>
        public string Logo
        {
            get
            {
                //if (string.IsNullOrEmpty(logo)) return "";
                return LogoFileId.ToString().CombinFileAccessPath(LogoOss);
            }
        }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 商家介绍
        /// </summary>
        public string Introduce { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public int Meter { get; set; }

        /// <summary>
        /// 人均消费
        /// </summary>
        public int PerCapita { get; set; }

        /// <summary>
        /// 推荐人数
        /// </summary>
        public int RecommendNumber { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 品类名称集合
        /// </summary>
        public List<CategoryNodeDto> Categories { get; set; }

        /// <summary>
        /// 团购信息
        /// </summary>
        public List<GroupBuyInfoDto> groupBuys { get; set; }

        /// <summary>
        /// 菜品信息
        /// </summary>
        public List<ProductCategoriesInfoDto> productCategories { get; set; }

        /// <summary>
        /// 桌台信息
        /// </summary>
        public List<MerchantTableBasicDto> merchantTables { get; set; }
    }
}
