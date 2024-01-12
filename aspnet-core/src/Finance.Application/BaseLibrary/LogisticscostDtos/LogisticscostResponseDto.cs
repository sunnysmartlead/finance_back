using Finance.Dto;
using Finance.PriceEval;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 物流信息返回
    /// </summary>
    public class LogisticscostResponseDto : PagedInputDto
    {
        /// <summary>
        ///分类
        /// </summary>
        public string Classification { get; set; }
        public int UpDown { get; set; }
        public List<LogisticscostDto> LogisticscostList { get; set; }

        public long? AuditFlowId { get; set; }
        public long? ProductId { get; set; }
    }
}