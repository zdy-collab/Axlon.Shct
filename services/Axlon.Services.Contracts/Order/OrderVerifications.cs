using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 核销记录
    /// </summary>
    [SugarTable("order_verifications", "核销记录")]
    public class OrderVerifications : OrderVerificationsRoot<long>
    {

        /// <summary>
        /// 核销码
        /// 用户到店展示的二维码/数字码，扫描后完成核销
        /// </summary>
        [SugarColumn(ColumnName = "verify_code", Length = 32, IsNullable = false)]
        public string VerifyCode { get; set; }

        /// <summary>
        /// 核销完成时间
        /// </summary>
        [SugarColumn(ColumnName = "verify_time", IsNullable = false)]
        public DateTime VerifyTime { get; set; }
    }
}
