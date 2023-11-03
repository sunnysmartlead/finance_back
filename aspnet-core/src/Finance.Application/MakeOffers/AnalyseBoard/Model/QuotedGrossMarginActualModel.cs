using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

/// <summary>
/// 报价分析看板二开返回实体类  -报价毛利率测算实际数量
/// </summary>
public class QuotedGrossMarginActualModel
{
    /// <summary>
    /// 车型
    /// </summary>
    public string project { get; set; }

    public List<QuotedGrossMarginActual> QuotedGrossMarginActualList;
}

public class QuotedGrossMarginActual
{
    
    /// <summary>
    /// 车型
    /// </summary>
    public string carModel { get; set; }

    /// <summary>
    /// 方案的id
    /// </summary>
    public long SolutionId { get; set; }

    /// <summary>
    /// 产品
    /// </summary>
    public string product { get; set; }
    /// <summary>
    /// 单车数量
    /// </summary>
    public int carNum { get; set; }
    /// <summary>
    /// 数据id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }

    /// <summary>
    /// 流程号Id
    /// </summary>
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 目标价（内部）单价
    /// </summary>
    public decimal InteriorPrice { get; set; }

    /// <summary>
    /// 目标价（内部）毛利率
    /// </summary>
    public decimal InteriorGrossMargin { get; set; }

    /// <summary>
    /// 目标价（内部）增加客供料毛利率
    /// </summary>
    public decimal InteriorClientGrossMargin { get; set; }

    /// <summary>
    /// 目标价（内部）剔除分摊费用毛利率
    /// </summary>
    public decimal InteriorNreGrossMargin { get; set; }

    /// <summary>
    /// 目标价（客户）单价
    /// </summary>
    public decimal ClientPrice { get; set; }

    /// <summary>
    /// 目标价（客户）毛利率
    /// </summary>
    public decimal ClientGrossMargin { get; set; }

    /// <summary>
    /// 目标价（客户）增加客供料毛利率
    /// </summary>
    public decimal ClientClientGrossMargin { get; set; }

    /// <summary>
    /// 目标价（客户）剔除分摊费用毛利率
    /// </summary>
    public decimal ClientNreGrossMargin { get; set; }

    /// <summary>
    /// 本次报价单价
    /// </summary>
    public decimal ThisQuotationPrice { get; set; }

    /// <summary>
    /// 本次报价毛利率
    /// </summary>
    public decimal ThisQuotationGrossMargin { get; set; }

    /// <summary>
    /// 本次报价增加客供料毛利率
    /// </summary>
    public decimal ThisQuotationClientGrossMargin { get; set; }

    /// <summary>
    /// 本次报价剔除NRE分摊费用毛利率
    /// </summary>
    public decimal ThisQuotationNreGrossMargin { get; set; }

    /// <summary>
    /// 上轮报价单价
    /// </summary>
    public decimal LastRoundPrice { get; set; }


    /// <summary>
    /// 上轮报价毛利率
    /// </summary>
    public decimal LastRoundGrossMargin { get; set; }

    /// <summary>
    /// 上轮报价增加客供料毛利率
    /// </summary>
    public decimal LastRoundClientGrossMargin { get; set; }

    /// <summary>
    /// 上轮报价剔除NRE分摊费用毛利率
    /// </summary>
    public decimal LastRoundNreGrossMargin { get; set; }
}