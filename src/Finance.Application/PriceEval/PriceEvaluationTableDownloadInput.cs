using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 产品核价表下载参数输入-流
    /// </summary>
    public class PriceEvaluationTableDownloadStreamInput
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
        /// 是否为全生命周期
        /// </summary>
        public virtual bool IsAll { get; set; }
    }

    /// <summary>
    /// 产品核价表下载参数输入
    /// </summary>
    public class PriceEvaluationTableDownloadInput
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
    }

    /// <summary>
    /// Nre核价表下载参数输入
    /// </summary>
    public class NreTableDownloadInput 
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

        ///// <summary>
        ///// 年份
        ///// </summary>
        //[Required]
        //public virtual int Year { get; set; }
    }
}
