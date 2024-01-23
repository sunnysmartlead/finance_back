using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Entering
{
    /// <summary>
    /// 资源部结构物料录入 复制项
    /// </summary>
    public class StructureElectronicCopy : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 结构bom单价表单id
        /// </summary>     
        public long StructureId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>     
        public string Currency { get; set; }         
        /// <summary>
        /// 物料可返利金额
        /// </summary>       
        public string RebateMoney { get; set; }
        /// <summary>
        /// MOQ
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal MOQ { get; set; }  
        /// <summary>
        /// 项目物料的使用量
        /// </summary>
        public string MaterialsUseCount { get; set; }
        /// <summary>
        /// 系统单价（原币）
        /// </summary>
        public string SystemiginalCurrency { get; set; }
        /// <summary>
        /// 项目物料的年降率
        /// </summary>
        public string InTheRate { get; set; }     
        /// <summary>
        /// 本位币(存json)
        /// </summary>
        public string StandardMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 确认人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 修改人id
        /// </summary>
        public long ModifierId { get; set; }
        /// <summary>
        /// 修改意见
        /// </summary>
        public string ModificationComments { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 是否录入 true/1 录入  false/0 未录入
        /// </summary>
        public bool IsEntering { get; set; }
        /// <summary>
        /// 系统单价是否从单价库中带出
        /// </summary>
        public bool IsSystemiginal { get; set; }
        /// <summary>
        /// 物料管制状态
        /// </summary>     
        public virtual string MaterialControlStatus { get; set; }
    }
}
