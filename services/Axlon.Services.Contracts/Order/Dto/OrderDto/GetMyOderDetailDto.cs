using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Order.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Order.Dto.OrderDto
{
    public class GetMyOderDetailRes
    {
        public long Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 订单状态
        /// OrderStatus
        /// </summary>
        public string Status { get; set; }

        public string StatusDescription
        {
            get
            {
                var state = Enum.TryParse<OrderStatus>(Status, true, out OrderStatus result);
                return result.GetDescription();
            }
        }

        /// <summary>
        /// 支付状态
        /// </summary>
        public string PayStatus { get; set; }

        public string PayStatusDescription
        {
            get
            {
                var state = Enum.TryParse<OrderPayStatus>(PayStatus, true, out OrderPayStatus result);
                return result.GetDescription();
            }
        }
        /// <summary>
        /// 订单类型
        /// OrderType
        /// </summary>
        public string Type { get; set; }

        public string TypeDescription 
        {
            get
            {
                var state = Enum.TryParse<OrderType>(Type, true, out OrderType result);
                return result.GetDescription();
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public OrderPaymentsPayType PayType { get; set; }

        public string PayTypeDescription
        {
            get
            {
                return PayType.GetDescription();
            }
        }

        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// 商家信息
        /// </summary>
        public GMODD_MerchantDto Merchant { get; set; }

        public GMODD_MerchantTable MerchantTable { get; set; }

        /// <summary>
        /// 优惠
        /// </summary>
        public List<GMODD_Discount> discounts { get; set; }

        /// <summary>
        /// 商品明细
        /// </summary>
        public List<GMODD_ProductDto> products { get; set; }
    }

    public class GMODD_Discount
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 获取订单详情-商家信息
    /// </summary>
    public class GMODD_MerchantDto 
    {
        public long Id { get; set; }

        /// <summary>
        /// 商家名称
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
    }

    public class GMODD_MerchantTable 
    {
        public long Id { get; set; }

        /// <summary>
        /// 桌号（如：A01、B02、VIP03）
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 区域（大厅/包间/卡座/包厢）,Enum：TableArea
        /// </summary>
        public string Area { get; set; }
    }

    public class GMODD_ProductDto
    {
        public long Id { get; set; }

        public long ImageFileId { get; set; }

        public string ImageOss { get; set; }

        /// <summary>
        /// 菜品图片URL
        /// </summary>
        public string Image
        {
            get
            {
                //if (string.IsNullOrEmpty(image)) return "";
                return ImageFileId.ToString().CombinFileAccessPath(ImageOss);
            }
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 菜品单价（快照）
        /// 下单时的价格，后续调价不影响已生成的订单
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小计金额 = ProductPrice * Quantity
        /// 可直接存储，避免重复计算和精度误差
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
