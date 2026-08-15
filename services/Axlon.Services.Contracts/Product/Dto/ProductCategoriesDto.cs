using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Product.Dto
{
    public class ProductCategoriesInfoDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public List<ProductInfoDto> products { get; set; }
    }
}
