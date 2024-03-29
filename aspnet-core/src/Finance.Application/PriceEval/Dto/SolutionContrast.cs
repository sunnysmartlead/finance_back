﻿using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class GetSolutionContrastInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID：1
        /// </summary>
        public long SolutionId_1 { get; set; }

        /// <summary>
        /// 方案表ID：2
        /// </summary>
        public long SolutionId_2 { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        [Required]
        public virtual YearType UpDown { get; set; }
    }
    /// <summary>
    /// 方案对比Dto
    /// </summary>
    public class SolutionContrast
    {
        /// <summary>
        /// 项目名（方案1）
        /// </summary>
        [ExcelColumn(Name = "项目名", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string ItemName { get; set; }

        /// <summary>
        /// 项目名（方案2）
        /// </summary>
        [ExcelColumn(Name = "方案2项目名", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string ItemName2 { get; set; }

        /// <summary>
        /// 方案1：单价
        /// </summary>
        [ExcelColumn(Name = "方案1：单价", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Price_1 { get; set; }

        /// <summary>
        /// 方案1：数量
        /// </summary>
        [ExcelColumn(Name = "方案1：数量", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Count_1 { get; set; }

        /// <summary>
        /// 方案1：汇率
        /// </summary>
        [ExcelColumn(Name = "方案1：汇率", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Rate_1 { get; set; }

        ///// <summary>
        ///// 方案1：MOQ分摊成本
        ///// </summary>
        //[ExcelColumn(Name = "方案1：MOQ分摊成本", Width = FinanceConsts.ExcelColumnWidth)]
        //public virtual decimal? MoqShareCount_1 { get; set; }

        /// <summary>
        /// 方案1：合计
        /// </summary>
        [ExcelColumn(Name = "方案1：合计", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Sum_1 { get; set; }

        /// <summary>
        /// 方案2：单价
        /// </summary>
        [ExcelColumn(Name = "方案2：单价", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Price_2 { get; set; }

        /// <summary>
        /// 方案2：数量
        /// </summary>
        [ExcelColumn(Name = "方案2：数量", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Count_2 { get; set; }

        /// <summary>
        /// 方案2：汇率
        /// </summary>
        [ExcelColumn(Name = "方案2：汇率", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Rate_2 { get; set; }

        ///// <summary>
        ///// 方案2：MOQ分摊成本
        ///// </summary>
        //[ExcelColumn(Name = "方案2：MOQ分摊成本", Width = FinanceConsts.ExcelColumnWidth)]
        //public virtual decimal? MoqShareCount_2 { get; set; }

        /// <summary>
        /// 方案2：合计
        /// </summary>
        [ExcelColumn(Name = "方案2：合计", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual decimal? Sum_2 { get; set; }

        /// <summary>
        /// 差异
        /// </summary>
        public virtual decimal? Change { get; set; }
    }


    /// <summary>
    /// 方案对比DtoExcel
    /// </summary>
    public class SolutionContrastExcel
    {
        /// <summary>
        /// 方案1名称
        /// </summary>
        public virtual string Name1 { get; set; }

        /// <summary>
        /// 方案2名称
        /// </summary>
        public virtual string Name2 { get; set; }

        /// <summary>
        /// 方案对比Dto数据
        /// </summary>
        public virtual List<SolutionContrast> SolutionContrast { get; set; }
    }
}
