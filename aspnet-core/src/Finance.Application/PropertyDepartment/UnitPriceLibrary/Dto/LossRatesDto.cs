using Finance.EngineeringDepartment.Dto;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 损耗率交互类
    /// </summary>
    public class LossRatesDto
    {
        /// <summary>
        /// 产品大类
        /// </summary>
        public string SuperType { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>   
        public string MaterialCategory { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>    
        public string CategoryName { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public virtual List<LossRatesYearDto> LossRateYearList { get; set; }
    }
    /// <summary>
    /// 损耗率年份类
    /// </summary>
    public class LossRatesYearDto
    {
        /// <summary>
        /// 损耗率别名
        /// </summary>
        public string YearAlias { get; set; }
        /// <summary>
        /// 损耗率年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 损耗率值
        /// </summary>
        public decimal Rate { get; set; }
    }
}
