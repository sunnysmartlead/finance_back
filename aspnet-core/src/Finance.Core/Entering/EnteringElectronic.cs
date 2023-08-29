using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Entering
{
    /// <summary>
    /// 资源部电子物料录入
    /// </summary>
    public class EnteringElectronic : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 电子bom表单id
        /// </summary>
        public long ElectronicId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 项目物料的使用量(存json)
        /// </summary>
        public string MaterialsUseCount { get; set; }      
        /// <summary>
        /// 系统单价原币 (存json)
        /// </summary>
        public string SystemiginalCurrency { get; set; }
        /// <summary>
        /// 年将率(存json)
        /// </summary>
        public string InTheRate { get; set; }       
        /// <summary>
        /// 本位币(存json)
        /// </summary>
        public string StandardMoney { get; set; }
        /// <summary>
        /// 物料可返利金额(存json)
        /// </summary>   
        public string RebateMoney { get; set; }
        /// <summary>
        /// MOQ
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal MOQ { get; set; }       
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 确认人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 是否录入 true/1 录入  false/0 未录入
        /// </summary>
        public bool IsEntering { get; set; }
        /// <summary>
        /// 物料管制状态
        /// </summary>  
        public virtual string MaterialControlStatus { get; set; }

    }
}
