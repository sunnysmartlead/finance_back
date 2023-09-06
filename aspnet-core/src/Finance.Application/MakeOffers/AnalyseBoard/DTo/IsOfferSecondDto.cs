﻿using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class IsOfferSecondDto
{
    /// <summary>
    /// 是否报价 true/1 是  false/0 否
    /// </summary>
    public bool IsOffer { get; set; }
    /// <summary>
    /// 不报价原因
    /// </summary>
    public string NoOfferReason { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 单价表
    /// </summary>
    public List<UnitPriceModel> UnitPrice { get; set; }

    /// <summary>
    /// Nre相关
    /// </summary>
    public List<AnalyseBoardNreDto> nres { get; set; }

    /// <summary>
    /// 样品报价
    /// </summary>
    public List<OnlySampleDto> SampleOffer { get; set; }

    /// <summary>
    /// SOP单价表（SOP年）
    /// </summary>
    public List<SopAnalysisModel> Sops { get; set; }

    /// <summary>
    /// 报价毛利率测算 实际数量
    /// </summary>
    public List<QuotedGrossMarginProjectModel> QuotedGrossMargins { get; set; }
    /// <summary>
    /// 报价毛利率测算阶梯数量
    /// </summary>
    public List<GradientQuotedGrossMarginModel> GradientQuotedGrossMargins { get; set; }
    /// <summary>
    /// 项目全生命周期汇总分析表-实际数量
    /// </summary>
    public List<PooledAnalysisModel> FullLifeCycle{ get; set; }
}