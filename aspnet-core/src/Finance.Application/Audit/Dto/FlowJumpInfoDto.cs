using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 流程跳转Dto
    /// </summary>
    public class FlowJumpInfoDto
    {
        /// <summary>
        /// 上一个流程
        /// </summary>
        public string PreviousProcessIdentifier { get; set; }
        /// <summary>
        /// 触发条件
        /// </summary>
        public OPINIONTYPE Condition { get; set; }
        /// <summary>
        /// 下一个流程
        /// </summary>
        public string NextProcessIdentifier { get; set; }
    }
}
