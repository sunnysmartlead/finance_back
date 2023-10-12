using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

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
    /// 单车产品数量
    /// </summary>
    public decimal ProductNumber { get; set; }
    
    public QuotedGrossMarginSimple quotedGrossMarginSimple{ get; set; }
}