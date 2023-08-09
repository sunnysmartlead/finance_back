

using Abp.Domain.Entities.Auditing;
using System;

namespace Finance.DemandApplyAudit
{
    /// <summary>
    /// 核价团队  其中包含(核价人员以及对应完成时间)
    /// </summary>
    public class PricingTeam : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员
        /// </summary>
        public long EngineerId { get; set; }
        /// <summary>
        ///品质保证部-实验费用录人员
        /// </summary>
        public long QualityBenchId { get; set; }
        /// <summary>
        ///产品开发部-EMC实验费用录入员
        /// </summary>
        public long EMCId { get; set; }
        /// <summary>
        /// 财务部-制造费用录入员
        /// </summary>
        public long ProductCostInputId { get; set; }
        /// <summary>
        /// 生产管理部-物流费用录入员
        /// </summary>
        public long ProductManageTimeId { get; set; }
        /// <summary>
        /// 项目核价审核员
        /// </summary>
        public long AuditId { get; set; }
        /// <summary>
        /// TR预计提交时间望完成时间
        /// </summary>
        public DateTime TrSubmitTime { get; set; }
        /// <summary>
        /// 产品部-电子工程师望完成时间
        /// </summary>
        public DateTime ElecEngineerTime { get; set; }
        /// <summary>
        ///产品部-结构工程师望完成时间
        /// </summary>
        public DateTime StructEngineerTime { get; set; }
        /// <summary>
        ///产品部-EMC实验费录入员望完成时间
        /// </summary>
        public DateTime EMCTime { get; set; }
        /// <summary>
        ///  品质保证部-实验室费用录入员期望完成时间
        /// </summary>
        public DateTime QualityBenchTime { get; set; }
        /// <summary>
        /// 资源管理部-电子资源开发期望完成时间
        /// </summary>
        public DateTime ResourceElecTime { get; set; }
        /// <summary>
        /// 资源管理部-结构资源开发期望完成时间
        /// </summary>
        public DateTime ResourceStructTime { get; set; }
        /// <summary>
        /// 资源部-模具录入员望完成时间
        /// </summary>
        public DateTime MouldWorkHourTime { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员期望完成时间
        /// </summary>
        public DateTime EngineerWorkHourTime { get; set; }
        /// <summary>
        ///  生成管理部-物流成本录入员期望完成时间
        /// </summary>
        public DateTime ProductManageTime { get; set; }
        /// <summary>
        /// 制造成本录入员期望完成时间
        /// </summary>
        public DateTime ProductCostInputTime { get; set; }    

    }
}
