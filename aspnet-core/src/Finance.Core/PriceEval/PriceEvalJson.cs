using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 储存核价表生成的Json
    /// </summary>
    public class PriceEvalJson : Entity<long>
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        [Required]
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 储存核价表的Json
        /// </summary>
        public virtual string Json { get; set; }
    }
}
