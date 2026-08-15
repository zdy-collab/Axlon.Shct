using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Order.RootTkey
{
    public class OrderItemsRoot<Tkey> : RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 订单ID，关联 orders.id
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = false)]
        public Tkey OrderId { get; set; }

        /// <summary>
        /// 菜品ID，关联 products.id
        /// 仅用于后台统计和溯源，前端展示使用快照字段
        /// </summary>
        [SugarColumn(ColumnName = "product_id", IsNullable = false)]
        public Tkey ProductId { get; set; }

        /// <summary>
        /// 菜品图片文件路径（快照）
        /// </summary>
        [SugarColumn(ColumnName = "product_image_file_id", ColumnDescription = "菜品图片文件路径（快照）", IsNullable = true)]

        public Tkey ProductImageFileId { get; set; }
    }
}
