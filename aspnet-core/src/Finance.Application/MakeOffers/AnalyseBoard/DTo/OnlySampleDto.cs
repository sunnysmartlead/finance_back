using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 样品
/// </summary>
public class OnlySampleDto
{
    /// <summary>
    /// 方案名
    /// </summary>
    public string SolutionName { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 方案表ID
    /// </summary>
    public long SolutionId { get; set; }
    /// <summary>
    /// 样品阶段
    /// </summary>
    public List<SampleQuotation> OnlySampleModels{ get; set; }
}
/// <summary>
/// 样品值
/// </summary>
public class OnlySampleModel
{
    /// <summary>
    /// 样品阶段名称
    /// </summary>
    public string SampleName { get; set; }

    /// <summary>
    /// 需求量
    /// </summary>
    public decimal Pcs { get; set; }

    /// <summary>
    /// 成本
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }

    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal SalesRevenue { get; set; }
}