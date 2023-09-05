using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class GradientGrossMarginModel
{
    /// <summary>
    /// 标题
    /// </summary>
    public string title { get; set; }
    public List<ItemGrossMarginModel> _itemGrossMarginModels{ get; set; }
    
}

public class ItemGrossMarginModel
{
    /// <summary>
    /// 项目
    /// </summary>
    public string item { get; set; }
    /// <summary>
    /// 目标价（内部）
    /// </summary>
    public decimal Interior{ get; set; }
    /// <summary>
    /// 目标价（客户）
    /// </summary>
    public decimal Client{ get; set; }
    /// <summary>
    /// 本次报价
    /// </summary>
    public decimal ThisQuotation{ get; set; }
    /// <summary>
    /// 上轮报价
    /// </summary>
    public decimal LastRound{ get; set; }
}