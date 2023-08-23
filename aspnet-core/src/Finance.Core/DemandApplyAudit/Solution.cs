using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.DemandApplyAudit
{
    /// <summary>
    /// 营销部审核 方案表
    /// </summary>
    public class Solution : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }
        /// <summary>
        /// 模组id
        /// </summary>
        public long Productld { get; set; }
        /// <summary>
        /// 模组名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SolutionName { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 是否COB方案
        /// </summary>
        public bool IsCOB { get; set; }
        /// <summary>
        /// 电子工程师(用户ID)
        /// </summary>
        public long ElecEngineerId { get; set; }
        /// <summary>
        /// 结构工程师(用户ID)
        /// </summary>
        public long StructEngineerId { get; set; }
        /// <summary>
        /// 是否为首款产品
        /// </summary>
        public bool IsFirst { get; set; }
    }
}
