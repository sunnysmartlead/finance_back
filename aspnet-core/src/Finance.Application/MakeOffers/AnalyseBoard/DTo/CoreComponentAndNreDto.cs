using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Finance.PriceEval;
using Finance.PriceEval.Dto;

namespace Finance.MakeOffers.AnalyseBoard;

public class CoreComponentAndNreDto
{
    public List<productAndGradient> ProductAndGradients { get; set; }
    public List<NreExpense> nres { get; set; }
    
}

public class productAndGradient
{
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }

    /// <summary>
    /// 梯度(K/Y)
    /// </summary>
    public virtual decimal GradientValue { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    public string Product { get; set; }

    public List<SolutionAndprice> solutionAndprices { get; set; }
   
}

public class SolutionAndprice
{
    /// <summary>
    /// 方案名
    /// </summary>
    public string solutionName { get; set; }

    /// <summary>
    /// 方案表ID
    /// </summary>
    public long SolutionId { get; set; }

    ///数量
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// 汇率
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 合计
    /// </summary>
    public decimal nsum { get; set; }
}

public class NreExpense
{
    /// <summary>
    /// 费用
    /// </summary>
    public string nre { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal price { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string remark { get; set; }
}