using Finance.Dto;
using Finance.WorkFlows.Dto;
using System.ComponentModel.DataAnnotations;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetLogisticscostsInput : PagedInputDto
    {
        public long? AuditFlowId { get; set; }
        public long? SolutionId { get; set; }
        /// <summary>
        /// 节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Opinion { get; set; }
        /// <summary>
        /// 审批评论
        /// </summary>
        public string Comment { get; set; }
    }

    /// <summary>
    /// ProcessHoursEnterAppService/CreateSubmitAsync方法的提交参数Dto
    /// </summary>
    public class ProcessHoursEnterCreateSubmitInput : SubmitNodeInput
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }
    }
}