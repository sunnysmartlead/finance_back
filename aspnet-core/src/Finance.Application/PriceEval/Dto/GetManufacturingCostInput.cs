using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 获取 制造成本汇总表  输入参数
    /// </summary>
    public class GetManufacturingCostInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 模组数量Id（即零件Id）
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }
    }
}
