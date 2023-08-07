using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    /// <summary>
    /// 核报价流程Id输入Dto
    /// </summary>
    public class AuditFlowIdDto
    {
        /// <summary>
        /// 核报价流程Id
        /// </summary>
        public string AuditFlowId { get; set; }
    }
    /// <summary>
    /// 核报价流程Id输入Dto
    /// </summary>
    public class LongAuditFlowIdDto
    {
        /// <summary>
        /// 核报价流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
    }
    /// <summary>
    /// 核报价流程和零件ID输入Dto
    /// </summary>
    public class AuditFlowAndProductIdDto
    {
        /// <summary>
        /// 核报价流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件号
        /// </summary>
        public long ProductId { get; set; }
    }
    /// <summary>
    /// 返回结果Dto
    /// </summary>
    public class ReturnDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = "执行成功";
    }
}
