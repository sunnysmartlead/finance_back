using Finance.Dto;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 复制条件
    /// </summary>
    public class GetBomEntersCopy    {
        public long? AuditFlowId { get; set; }
        public long? AuditFlowNewId { get; set; }

        public List<SolutionIdBomEnterSolutionId> SolutionIdAndQuoteSolutionIds;

    }

    public class SolutionIdBomEnterSolutionId
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