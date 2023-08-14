using Finance.Dto;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class LogisticscostResponseDto : PagedInputDto
    {
        public string Classification { get; set; }
        public List<LogisticscostDto> LogisticscostList { get; set; }

        public long? AuditFlowId { get; set; }
        public long? ProductId { get; set; }
    }
}