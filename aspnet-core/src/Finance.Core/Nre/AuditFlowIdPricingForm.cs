using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// Nre 核价表 带流程ID 实体类
    /// </summary>
    public class AuditFlowIdPricingForm : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>         
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>       
        public long SolutionId { get; set; }
        /// <summary>
        /// 将实体保存为JSON
        /// </summary>
        [Column(TypeName = "CLOB")]
        public string JsonData { get; set; }
    }
}
