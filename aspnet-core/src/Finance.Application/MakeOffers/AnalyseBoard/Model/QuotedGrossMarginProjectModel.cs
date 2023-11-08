using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

/// <summary>
/// 报价分析看板二开返回实体类  
/// </summary>
public class QuotedGrossMarginProjectModel
{
    /// <summary>
    /// 车型
    /// </summary>
    public string project { get; set; }
    public List<GrossMargin> GrossMargins{ get; set; }
    
}


public class GrossMargin
{
    /// <summary>
    /// 产品
    /// </summary>
    public string product { get; set; }
    /// <summary>
    /// 数据id
    /// </summary>
    public long Id{ get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary>
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 方案的id
    /// </summary>
    public long SolutionId { get; set; }
  
    /// <summary>
    /// 单车产品数量
    /// </summary>
    public decimal ProductNumber { get; set; }
    
    public QuotedGrossMarginSimple quotedGrossMarginSimple{ get; set; }
}