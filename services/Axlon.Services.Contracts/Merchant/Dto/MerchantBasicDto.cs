using Axlon.Services.Contracts.Merchant.JsonObj;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Dto
{
    public class MerchantBasicDto
    {
        public long Id { get; set; }

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
        /// 门店URL
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 营业时间
        /// </summary>
        public WeeklyBusinessHours BusinessHours { get; set; }

    }
}
