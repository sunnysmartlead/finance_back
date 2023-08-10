using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 设置投入量 方法 参数输入
    /// </summary>
    public class SetPriceEvaluationTableInputCount
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 模组数量
        /// </summary>
        [Required]
        public virtual IList<ModelCountInputCount> ModelCountInputCount { get; set; }

    }

    /// <summary>
    /// 模组投入数量
    /// </summary>
    public class ModelCountInputCount
    {
        /// <summary>
        /// 模组数量Id（即零件Id）
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        [Required]
        public virtual int InputCount { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }
    }
}
