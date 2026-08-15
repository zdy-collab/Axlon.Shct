using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.GroupBuy.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Product.Dto
{
    public class ProductInfoDto
    {
        public long Id { get; set; }

        public long ImageFileId { get; set; }

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
        /// 库存（-1不限）
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 销量（实时更新）
        /// </summary>
        public int SalesCount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
