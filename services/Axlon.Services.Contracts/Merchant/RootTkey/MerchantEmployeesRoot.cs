using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant.RootTkey
{
    public class MerchantEmployeesRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID（关联 merchants.id）
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = false)]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 关联用户ID（员工同时也是平台用户，关联 users.id）
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsNullable = false)]
        public Tkey UserId { get; set; }
    }
}
