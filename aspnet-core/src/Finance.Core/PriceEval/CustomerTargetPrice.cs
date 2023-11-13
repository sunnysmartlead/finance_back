using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 客户目标价
    /// </summary>
    [Table("Pe_CustomerTargetPrice")]
    public class CustomerTargetPrice : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 梯度
        /// </summary>
        [Required]
        public virtual decimal Kv { get; set; }


        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 目标价
        /// </summary>
        public virtual string TargetPrice { get; set; }


        /// <summary>
        /// 报价币种（汇率录入表（ExchangeRate）主键）
        /// </summary>
        public virtual long? Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public virtual decimal? ExchangeRate { get; set; }

    }
}
