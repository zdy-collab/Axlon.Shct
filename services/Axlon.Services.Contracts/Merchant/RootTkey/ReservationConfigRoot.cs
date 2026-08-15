using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class ReservationConfigRoot<Tkey> : BaseUpdatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {

        /// <summary>
        /// 商家ID，唯一标识每个商家
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false, ColumnDescription = "商家ID，关联 merchants 表")]
        public Tkey MerchantId { get; set; }
    }
}
