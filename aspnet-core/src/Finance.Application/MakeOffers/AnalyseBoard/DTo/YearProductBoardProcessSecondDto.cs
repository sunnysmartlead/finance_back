namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 年份维度对比 动态单价表计算 交互类 二开
/// </summary>
public class YearProductBoardProcessSecondDto
{
    /// <summary>
    /// 流程表id
    /// </summary>
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
}