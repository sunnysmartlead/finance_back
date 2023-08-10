using Abp.UI;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment.Dto
{
    /// <summary>
    /// 损耗率模块Dto
    /// </summary>
    public class LossRateDto
    {
        /// <summary>
        /// 流程表流程号
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件号
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// 产品名称（零件1，零件2...）
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 超级大种类
        /// </summary>
        public string SuperType { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual List<LossRateYearDto> LossRateYearList { get; set; }
    }

    /// <summary>
    /// 损耗率年份类
    /// </summary>
    public class LossRateYearDto
    {
        /// <summary>
        /// 损耗率年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 损耗率值
        /// </summary>
        public Decimal Rate { get; set; }
    }

}
