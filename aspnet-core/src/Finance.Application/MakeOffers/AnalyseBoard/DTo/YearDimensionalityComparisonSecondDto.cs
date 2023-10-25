using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class YearDimensionalityComparisonSecondDto
{
    /// <summary>
    /// 数量K
    /// </summary>
    public List<YearValue> numk { get; set; }
    /// <summary>
    /// 单价
    /// </summary>
    public List<YearValue> Prices { get; set; }
    /// <summary>
    /// 销售成本
    /// </summary>
    public List<YearValue> SellingCost { get; set; }
    /// <summary>
    /// 单位平均成本
    /// </summary>
    public List<YearValue> AverageCost { get; set; }
    /// <summary>
    /// 销售收入
    /// </summary>
    public List<YearValue> SalesRevenue { get; set; }
    /// <summary>
    /// 销售毛利（千元）
    /// </summary>
    public List<YearValue> SalesMargin { get; set; }
    /// <summary>
    /// 佣金
    /// </summary>
    public List<YearValue> commission { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public List<YearValue> GrossMargin { get; set; }
}

public class YearValue
{
    /// <summary>
    /// 年份
    /// </summary>
    public string key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public decimal value { get; set; }
}