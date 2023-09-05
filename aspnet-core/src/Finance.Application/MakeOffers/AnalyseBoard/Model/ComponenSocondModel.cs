using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

/// <summary>
/// 零件 模型
/// </summary>
public class ComponenSocondModel
{
    /// <summary>
    /// 核心部件
    /// </summary>
    public string CoreComponent { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 方案，规格
    /// </summary>
    public List<SolutionOrSpecification> Specifications{ get; set; }
}
/// <summary>
/// 规格
/// </summary>
public class SolutionOrSpecification
{
    /// <summary>
    /// 方案
    /// </summary>
    public string solutionname { get; set; }

    /// <summary>
    /// 规格
    /// </summary>
    public string specification { get; set; }
}