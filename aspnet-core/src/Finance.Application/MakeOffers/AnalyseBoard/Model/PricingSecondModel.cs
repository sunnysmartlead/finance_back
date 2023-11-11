using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class PricingSecondModel
{
    /// <summary>
    /// 方案名称
    /// </summary>
    public string SolutionName { get; set; }
    /// <summary>
    /// 梯度
    /// </summary>
    public string Gradient { get; set; }
  
    /// <summary>
    /// BOM成本 sop
    /// </summary>
    public decimal BomSop { get; set; }
    /// <summary>
    /// BOM成本 全生命周期成本
    /// </summary>
    public decimal Bomfull { get; set; }
    
    
    /// <summary>
    /// 生产成本 sop
    /// </summary>
    public decimal ScSop { get; set; }
    /// <summary>
    /// 生产成本 全生命周期成本
    /// </summary>
    public decimal Scfull { get; set; }
    
    /// <summary>
    /// 良损率、良损成本 sop
    /// </summary>
    public decimal LsSop { get; set; }
    /// <summary>
    /// 良损率、良损成本 全生命周期成本
    /// </summary>
    public decimal Lsfull { get; set; }
    /// <summary>
    /// 运费 sop
    /// </summary>
    public decimal YfSop { get; set; }
    /// <summary>
    /// 运费 全生命周期成本
    /// </summary>
    public decimal Yffull { get; set; }
    /// <summary>
    /// MOQ分摊成本 sop
    /// </summary>
    public decimal MoqSop { get; set; }
    /// <summary>
    /// MOQ分摊成本 全生命周期成本
    /// </summary>
    public decimal Moqfull { get; set; }
    /// <summary>
    ///质量成本 sop
    /// </summary>
    public decimal QuSop { get; set; }
    /// <summary>
    /// 质量成本 全生命周期成本
    /// </summary>
    public decimal Qufull { get; set; }
    /// <summary>
    ///分摊成本 sop
    /// </summary>
    public decimal FtSop { get; set; }
    /// <summary>
    /// 分摊成本 全生命周期成本
    /// </summary>
    public decimal Ftfull { get; set; }
    /// <summary>
    ///总成本 sop
    /// </summary>
    public decimal AllSop { get; set; }
    /// <summary>
    /// 总成本 全生命周期成本
    /// </summary>
    public decimal Allfull { get; set; }
}