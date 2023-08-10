using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 流程信息Dto
    /// </summary>
    public class FlowProcessDto
    {
        /// <summary>
        /// 流程Index
        /// </summary>
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 编辑角色
        /// </summary>
        public string EditRole { get; set; }
        /// <summary>
        /// 查看角色
        /// </summary>
        public string ReadonlyRole { get; set; }
    }
}
