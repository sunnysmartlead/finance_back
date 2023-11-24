using System.Collections.Generic;
using Finance.DemandApplyAudit;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class SolutionQuotationDto
{
    /// <summary>
    /// 方案组合id
    /// </summary>
    public virtual long Id { get; set; }
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
    
    /// <summary>
    /// 是否报价反馈
    /// </summary>
    public bool isQuotation{ get; set; }
}