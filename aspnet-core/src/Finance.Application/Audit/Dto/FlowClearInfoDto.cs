using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 清除哪些流程记录Dto
    /// </summary>
    public class FlowClearInfoDto
    {
        /// <summary>
        /// 退回后的当前流程
        /// </summary>
        public string CurrentProcessIdentifier { get; set; }
        /// <summary>
        /// 需要清除的流程标识符
        /// </summary>
        public string NextProcessIdentifier { get; set; }
    }
}
