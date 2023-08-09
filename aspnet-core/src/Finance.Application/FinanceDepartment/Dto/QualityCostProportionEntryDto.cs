using Finance.FinanceParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceDepartment.Dto
{
    /// <summary>
    /// 质量成本比例录入DTO
    /// </summary>
    public class QualityCostProportionEntryDto
    {
        public List<QualityCostDto> QualityCostList { get; set; }
    }
    /// <summary>
    /// 质量成本比例实体类DTO
    /// </summary>
    public class QualityCostDto
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 是否首款
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual List<QualityCostYearDto> QualityCostYearList { get; set; }
    }
    /// <summary>
    /// 质量成本比例年份类
    /// </summary>
    public class QualityCostYearDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public decimal Rate { get; set; }
    }

}
