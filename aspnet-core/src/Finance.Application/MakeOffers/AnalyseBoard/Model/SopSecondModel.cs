using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class SopSecondModel
{
    /// <summary>
    /// 年份
    /// </summary>
    public int Year { get; set; }
    /// <summary>
    /// 走量
    /// </summary>
    public decimal Motion { get; set; }
    /// <summary>
    /// 年将率
    /// </summary>
    public decimal YearDrop { get; set; }
    /// <summary>
    /// 年度返利要求
    /// </summary>
    public decimal RebateRequest { get; set; }
    /// <summary>
    /// 一次性折让
    /// </summary>
    public decimal DiscountRate { get; set; }
    /// <summary>
    /// 年度佣金比例
    /// </summary>
    public decimal CommissionRate { get; set; }
}

public class SopAnalysisModel
{
    /// <summary>
    /// 梯度(K/Y)
    /// </summary>
    public virtual string GradientValue { get; set; }
    /// <summary>
    /// 产品
    /// </summary>
   
    public virtual string Product { get; set; }
    public List<GrossValue> GrossValues{ get; set; }
    
    
}

public class GrossValue
{
    /// <summary>
    /// 毛利率
    /// </summary>
    public  string Gross { get; set; }
    /// <summary>
    /// 值
    /// </summary>
   
    public virtual decimal Grossvalue { get; set; }
}