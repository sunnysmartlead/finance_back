using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.PriceEval.Dto.DataInput
{
    public class GetCustomerTargetPriceInput
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

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
    }
}
