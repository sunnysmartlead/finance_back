using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价审核表
    /// </summary>
    public class AuditQuotationList : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 审核表内容 存json  实体类为 AuditQuotationListDto
        /// </summary>
        [Column(TypeName = "CLOB")]
        public string AuditQuotationListJson { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
        
        /// <summary>
        /// 1 营销部审批、总经理审批1、总经理2
        /// </summary>
        public int ntype { get; set; }
        /// <summary>
        /// 1 类别  0报价分析看板，1报价反馈
        /// </summary>
        public int nsource { get; set; }

    }
}
