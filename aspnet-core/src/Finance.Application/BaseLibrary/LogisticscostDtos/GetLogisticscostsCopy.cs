using Finance.Dto;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.WorkFlows.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 复制条件
    /// </summary>
    public class GetLogisticscostsCopy
    {
        public long? AuditFlowId { get; set; }
        public long? AuditFlowNewId { get; set; }

        //方案信息
        public List<SolutionIdLogisticscostsSolutionId> SolutionIdAndQuoteSolutionIds;

    }

    public class SolutionIdLogisticscostsSolutionId
    {
        /// <summary>
        /// 方案ID
        /// </summary>
        public long? SolutionId { get; set; }
        /// <summary>
        /// 引用流程的的方案ID
        /// </summary>
        public long? QuoteSolutionId { get; set; }
    }
}