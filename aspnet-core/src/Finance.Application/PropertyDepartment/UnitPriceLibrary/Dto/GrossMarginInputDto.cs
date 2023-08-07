using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    ///  查询毛利率方案  交互类
    /// </summary>
    public class GrossMarginInputDto : PagedInputDto
    {
        /// <summary>
        /// 查询过滤关键字   毛利率方案名称
        /// </summary>
        public virtual string GrossMarginName { get; set; }
    }
}
