﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class SopSecondModel
{
    /// <summary>
    /// 数据id
    /// </summary>
    public long Id{ get; set; }
    /// <summary>
    /// 年份
    /// </summary>
    public string Year { get; set; }
    /// <summary>
    /// 走量
    /// </summary>
    public decimal Motion { get; set; }
    /// <summary>
    /// 年将率
    /// </summary>
    public decimal AnnualDeclineRate { get; set; }
    /// <summary>
    /// 年度返利要求
    /// </summary>
    public decimal AnnualRebateRequirements { get; set; }
    /// <summary>
    /// 一次性折让
    /// </summary>
    public decimal OneTimeDiscountRate { get; set; }
    /// <summary>
    /// 年度佣金比例
    /// </summary>
    public decimal CommissionRate { get; set; }
}

public class SopAnalysisModel
{
    /// <summary>
    /// 数据id
    /// </summary>
    public long Id{ get; set; }
    /// <summary>
    /// 梯度(K/Y)
    /// </summary>
    public virtual string GradientValue { get; set; }
    /// <summary>
    /// 梯度id
    /// </summary>
    public long GradientId { get; set; }
    /// <summary>
    /// 产品
    /// </summary>
   
    public virtual string Product { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    public List<GrossValue> GrossValues{ get; set; }
    
    /// <summary>
    /// 版本号
    /// </summary> 
    public int version { get; set; }
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