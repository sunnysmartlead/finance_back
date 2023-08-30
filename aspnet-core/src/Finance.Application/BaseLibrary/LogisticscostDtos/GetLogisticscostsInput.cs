using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetLogisticscostsInput: PagedInputDto
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
}