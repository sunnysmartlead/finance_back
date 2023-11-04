namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class GrossMarginSecondDto
{
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
    /// <summary>
    /// 增加客供料毛利率
    /// </summary>
    public decimal ClientGrossMargin { get; set; }
    /// <summary>
    /// 剔除NRE分摊费用毛利率
    /// </summary>
    public decimal NreGrossMargin { get; set; }
    
    /// <summary>
    /// 数量
    /// </summary>
    public decimal sl { get; set; }
    /// <summary>
    /// 销售成本
    /// </summary>
    public decimal xscb { get; set; }
    /// <summary>
    /// 单位平均成本
    /// </summary>
    public decimal dwpjcb { get; set; }
    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal xssr { get; set; }
    /// <summary>
    /// 客供销售收入
    /// </summary>
    public decimal kgxssr { get; set; }
    /// <summary>
    /// 客供销售收入
    /// </summary>
    public decimal ftxssr { get; set; }
    /// <summary>
    /// 平均单价
    /// </summary>
    public decimal pjdj { get; set; }
    /// <summary>
    /// 销售毛利
    /// </summary>
    public decimal xsml { get; set; }
    /// <summary>
    /// 客供销售毛利
    /// </summary>
    public decimal kgxsml { get; set; }
    /// <summary>
    /// 分摊销售毛利
    /// </summary>
    public decimal ftxsml { get; set; }
    /// <summary>
    /// 佣金
    /// </summary>
    public decimal yj { get; set; }
    
    /// <summary>
    /// 单价
    /// </summary>
    public decimal unitPrice { get; set; }

   
    
}