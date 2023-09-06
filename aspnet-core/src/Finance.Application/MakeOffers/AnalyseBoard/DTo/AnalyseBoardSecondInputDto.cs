﻿using System.Collections.Generic;
using Finance.DemandApplyAudit;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 报价分析看板输入实体类  交互类
/// </summary>
public class AnalyseBoardSecondInputDto
{
   public long auditFlowId { get; set; }
   /// <summary>
   /// 毛利率方案id
   ///
   /// 
   /// </summary>
   public long GrossMarginId{ get; set; }
   
   public List<Solution> solutionTables{ get; set; }
}