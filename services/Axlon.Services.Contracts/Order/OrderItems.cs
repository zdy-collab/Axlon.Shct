using Axlon.Services.Contracts.Order.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Order
{
    /// <summary>
    /// 订单明细
    /// </summary>
    [SugarTable("order_items", "订单明细")]
    public class OrderItems : OrderItemsRoot<long>
    {
        /// <summary>
        /// 菜品图片Oss地址（快照）
        /// </summary>
        [SugarColumn(ColumnName = "product_image_oss", IsNullable = true)]
        public string ProductImageOss { get; set; }

        /// <summary>
        /// 菜品名称（快照）
        /// 下单时从 products 表复制，菜品改名后此值保持不变
        /// </summary>
        [SugarColumn(ColumnName = "product_name", Length = 200, IsNullable = false)]
        public string ProductName { get; set; }

        /// <summary>
        /// 菜品单价（快照）
        /// 下单时的价格，后续调价不影响已生成的订单
        /// </summary>
        [SugarColumn(ColumnName = "product_price", DecimalDigits = 2, IsNullable = false)]
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [SugarColumn(ColumnName = "quantity", IsNullable = false)]
        public int Quantity { get; set; }

        /// <summary>
        /// 小计金额 = ProductPrice * Quantity
        /// 可直接存储，避免重复计算和精度误差
        /// </summary>
        [SugarColumn(ColumnName = "total_price", DecimalDigits = 2, IsNullable = false)]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 备注信息
        /// 如：少辣、不要香菜、加冰等顾客定制需求
        /// </summary>
        [SugarColumn(ColumnName = "remarks", Length = 500, IsNullable = true)]
        public string Remarks { get; set; }
    }
}
