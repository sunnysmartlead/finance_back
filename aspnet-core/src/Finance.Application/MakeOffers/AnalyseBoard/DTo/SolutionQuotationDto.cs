using System.Collections.Generic;
using Finance.DemandApplyAudit;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class SolutionQuotationDto
{
    /// <summary>
    /// 流程Id
    /// </summary>
    public virtual long AuditFlowId { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    
    /// <summary>
    /// 报价次数
    /// </summary>
    public int ntime { get; set; }
    
    /// <summary>
    /// 报价次数
    /// </summary>
    public List<Solution> solutionList { get; set; }
}