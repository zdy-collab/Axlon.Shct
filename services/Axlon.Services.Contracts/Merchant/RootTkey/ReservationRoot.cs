using Axlon.Services.Contracts.Base.BaseRoot;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class ReservationRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID，关联 merchants 表
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false, ColumnDescription = "商家ID，关联 merchants 表")]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 用户ID，关联 users 表
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false, ColumnDescription = "用户ID，关联 users 表")]
        public Tkey UserId { get; set; }
    }
}
