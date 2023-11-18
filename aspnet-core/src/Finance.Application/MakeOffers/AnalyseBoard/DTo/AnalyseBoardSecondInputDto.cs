using System.Collections.Generic;
using Finance.DemandApplyAudit;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

/// <summary>
/// 报价分析看板输入实体类  交互类
/// </summary>
public class AnalyseBoardSecondInputDto
{
    public long auditFlowId { get; set; }
    
    public List<Solution> solutionTables { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    
    /// <summary>
    /// 报价次数
    /// </summary>
    public int ntime { get; set; }
    /// <summary>
    /// 0 报价分析看板，1 报价反馈
    /// </summary>
    public int ntype { get; set; }
  
}