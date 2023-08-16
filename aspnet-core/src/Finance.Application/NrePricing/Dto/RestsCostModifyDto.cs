using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 项目管理部 其他费用 修改项 交互类
    /// </summary>
    public class RestsCostModifyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 费用名称
        /// </summary>
        public string ConstName { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
