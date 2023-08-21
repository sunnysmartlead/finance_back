using Finance.Dto;

namespace Finance.Processes
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetProcessHoursEnterLinesInput: PagedInputDto
    {
        public long? AuditFlowId { get; set; }
        public long? SolutionId { get; set; }
        public string? Uph { get; set; }
    }
}