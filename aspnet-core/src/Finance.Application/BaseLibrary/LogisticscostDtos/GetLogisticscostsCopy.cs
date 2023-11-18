using Finance.Dto;
using Finance.WorkFlows.Dto;
using System.ComponentModel.DataAnnotations;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 复制条件
    /// </summary>
    public class GetLogisticscostsCopy
    {
        public long? AuditFlowId { get; set; }
        public long? AuditFlowNewId { get; set; }
      
    }
}