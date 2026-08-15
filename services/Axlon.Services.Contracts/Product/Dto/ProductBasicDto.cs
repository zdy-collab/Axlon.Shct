using Axlon.Services.Contracts.GroupBuy.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Product.Dto
{
    public class ProductBasicDto
    {
        public long Id { get; set; }

        public long ImageFileId { get; set; }

        /// <summary>
        /// 商家ID -> merchants.id
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 商家分类Id -> product_categories.id
        /// </summary>
        public long CategoryId { get; set; }

        public string ImageOss { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 价格（与线下一致）
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 库存（-1不限）
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 1上架/0下架
        /// </summary>
        public IsOn IsOn { get; set; }
    }
}
