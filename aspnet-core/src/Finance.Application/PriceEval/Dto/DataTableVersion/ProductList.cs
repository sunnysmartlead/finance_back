using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 产品列表
    /// </summary>
    public class ProductList : EntityDto<long>
    {
        /// <summary>
        /// Product 产品名
        /// </summary>
        public virtual string Product { get; set; }
    }
}
