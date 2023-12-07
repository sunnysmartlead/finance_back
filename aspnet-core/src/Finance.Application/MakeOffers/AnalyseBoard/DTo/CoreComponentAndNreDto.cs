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
public class CoreDeviceDto
{ 
    List<CoreDevice> CoreDevice { get; set; }
}

public class CoreDevice
{
    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; }
    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Number { get; set; }
    /// <summary>
    /// 汇率
    /// </summary>
    public decimal Rate { get; set; }
    /// <summary>
    /// 合计
    /// </summary>
    public decimal Sum { get; set; }

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


public class NreExpense2
{
    /// <summary>
    /// 手板件费用
    /// </summary>
    public decimal handPieceCostTotal { get; set; }

    /// <summary>
    /// 模具费用
    /// </summary>
    public decimal mouldInventoryTotal { get; set; }

    /// <summary>
    /// 工装费用
    /// </summary>
    public decimal toolingCostTotal { get; set; }
    /// <summary>
    /// 治具费用
    /// </summary>
    public decimal fixtureCostTotal { get; set; }
    /// <summary>
    /// 检具费用
    /// </summary>
    public decimal qaqcDepartmentsTotal { get; set; }
    /// <summary>
    /// 生产设备费用
    /// </summary>
    public decimal productionEquipmentCostTotal { get; set; }
    /// <summary>
    /// 专用生产设备
    /// </summary>
    public decimal deviceStatusSpecial { get; set; }
    /// <summary>
    /// 非专用生产设备
    /// </summary>
    public decimal deviceStatus { get; set; }
    /// <summary>
    /// 实验费用
    /// </summary>
    public decimal laboratoryFeeModelsTotal { get; set; }
    /// <summary>
    /// 测试软件费用
    /// </summary>
    public decimal softwareTestingCostTotal { get; set; }
    /// <summary>
    /// 差旅费
    /// </summary>
    public decimal travelExpenseTotal { get; set; }
    /// <summary>
    /// 其他费用
    /// </summary>
    public decimal restsCostTotal { get; set; }

    /// <summary>
    /// 合计
    /// </summary>
    public decimal sum { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string remark { get; set; }
}