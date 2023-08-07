using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 获取 其他成本项目 参数输入
    /// </summary>
    public class GetOtherCostItemInput
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
