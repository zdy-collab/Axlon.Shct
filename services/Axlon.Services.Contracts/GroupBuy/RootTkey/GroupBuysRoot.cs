using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.GroupBuy.RootTkey
{
    public class GroupBuysRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 商家ID，关联 merchants.id
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", ColumnDescription = "商家ID", IsNullable = false)]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 团购图片文件路径
        /// </summary>
        [SugarColumn(ColumnName = "image_file_id", ColumnDescription = "团购图片文件路径", IsNullable = true)]

        public Tkey ImageFileId { get; set; }
    }
}
