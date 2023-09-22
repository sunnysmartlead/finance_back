using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class QuotedGrossMarginModel
{
    /// <summary>
    /// 产品
    /// </summary>
    public string product { get; set; }
    public QuotedGrossMarginSimple QuotedGrossMarginSimple { get; set; }
    
}

public class QuotedGrossMarginSimple
{
  
    /// <summary>
    /// 目标价（内部）
    /// </summary>
    public TargetPrice Interior{ get; set; }
    /// <summary>
    /// 目标价（客户）
    /// </summary>
    public TargetPrice Client{ get; set; }
    /// <summary>
    /// 本次报价
    /// </summary>
    public TargetPrice ThisQuotation{ get; set; }
    /// <summary>
    /// 上轮报价
    /// </summary>
    public TargetPrice LastRound{ get; set; }

    
}

public class TargetPrice
{
    /// <summary>
    /// 单价
    /// </summary>
   
    public decimal Price { get; set; }
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
    
}