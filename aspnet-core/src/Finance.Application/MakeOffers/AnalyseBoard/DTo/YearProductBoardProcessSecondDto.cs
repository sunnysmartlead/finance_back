using Spire.Pdf.Exporting.XPS.Schema;
using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

/// <summary>
/// 年份维度对比 动态单价表计算 交互类 二开
/// </summary>
public class YearProductBoardProcessSecondDto
{
    /// <summary>
    /// 流程表id  实际数量和阶梯数量必传
    /// </summary>
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 梯度数量主键 若无需获取对应梯度
    /// 此行不传
    /// </summary>
    public long GradientId { get; set; }

    /// <summary>
    /// 本次报价-单价 阶梯数量必传
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 方案表ID 实际数量和阶梯数量必传
    /// </summary>
    public long SolutionId { get; set; }

    /// <summary>
    /// 车型  实际数量必传
    /// </summary>
    public string CarModel { get; set; }

    /// <summary>
    /// 阶梯数量-方案id、梯度、单价，用实际数量测算必传
    /// </summary>
    public List<SoltionGradPrice> SoltionGradPrices { get; set; }
}

public class SoltionGradPrice
{
    /// <summary>
    /// 梯度id
    /// 
    /// </summary>
    public long Gradientid { get; set; }

    /// <summary>
    /// 本次报价-单价
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 方案表ID
    /// </summary>
    public long SolutionId { get; set; }
}

public class YearProductBoardProcessQtSecondDto
{
    /// <summary>
    /// 流程表id  实际数量和阶梯数量必传
    /// </summary>
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 车型  实际数量必传
    /// </summary>
    public string CarModel { get; set; }

    /// <summary>
    /// 方案表ID必传
    /// </summary>
    public List<SolutionIdsAndcarNum> SolutionIdsAndcarNums { get; set; }

    /// <summary>
    /// 阶梯数量-方案id、梯度、单价，用实际数量测算必传
    /// </summary>
    public List<SoltionGradPrice> SoltionGradPrices { get; set; }
}

public class SolutionIdsAndcarNum
{
    /// <summary>
    /// 方案表ID
    /// </summary>
    public long SolutionId { get; set; }

    /// <summary>
    /// 单车数量
    /// </summary>
    public int carNum { get; set; }
}