using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Content.RootTkey
{
    public class PlatformContentsRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 关联商家ID -> merchants.id
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = true)]
        public Tkey MerchantId { get; set; }

        /// <summary>
        /// 封面图片文件Id
        /// </summary>
        [SugarColumn(ColumnName = "cover_image_file_id", IsNullable = true)]
        public Tkey CoverImageFileId { get; set; }
    }
}
