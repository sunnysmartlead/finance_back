using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 所有流程流转用到的返回Dto
    /// </summary>
    public class AuditFlowRetDto
    {
        /// <summary>
        /// 流程信息List
        /// </summary>
        public List<AuditFlow> AuditFlowList { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "执行成功";
    }

    /// <summary>
    /// 返回流程ID的Dto
    /// </summary>
    public class AuditFlowIdRetDto
    {
        /// <summary>
        /// 流程信息List
        /// </summary>
        public List<long> AuditFlowIdList { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "执行成功";
    }
}
