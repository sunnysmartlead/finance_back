using Abp.Domain.Entities;
using Castle.MicroKernel.SubSystems.Conversion;
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
    /// 核价开始数据
    /// </summary>
    [Table("Pe_PriceEvaluationStartData")]
    public class PriceEvaluationStartData : Entity<long>
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 录入的数据Json
        /// </summary>
        [Column(TypeName = "CLOB")]
        public virtual string DataJson { get; set; }
    }
}
