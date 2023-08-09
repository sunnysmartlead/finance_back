using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    ///品保录入  环境实验费 模型
    /// </summary>
    public class EnvironmentalExperimentFeeModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 试验项目(根据与客户协定项目)
        /// </summary>        
        public string ProjectName { get; set; }
        /// <summary>
        /// 是否指定第三方 (是 true 否 false)
        /// </summary>        
        public bool IsThirdParty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>      
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 调整系数
        /// </summary>
        public decimal AdjustmentCoefficient { get; set; }
        /// <summary>
        /// 计价单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 计数-摸底
        /// </summary>
        public decimal CountBottomingOut { get; set; }
        /// <summary>
        /// 计数-DV
        /// </summary>
        public decimal CountDV { get; set; }
        /// <summary>
        /// 计数-PV
        /// </summary>
        public decimal CountPV { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>       
        public decimal AllCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>       
        public string Remark { get; set; }

    }
    /// <summary>
    ///品保录入  试验项目表 模型
    /// </summary>
    public class QADepartmentTestExcelModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 试验项目(根据与客户协定项目)
        /// </summary>
        [ExcelColumnName("试验项目")]
        public string ProjectName { get; set; }
        /// <summary>
        /// 是否指定第三方 (是 true 否 false)
        /// </summary>
        [ExcelColumnName("是否指定第三方")]
        public string IsThirdParty { get; set; }
        /// <summary>
        /// 计数-摸底
        /// </summary>
        [ExcelColumnName("计数-摸底")]
        public decimal CountBottomingOut { get; set; }
        /// <summary>
        /// 计数-DV
        /// </summary>
        [ExcelColumnName("计数-DV")]
        public decimal CountDV { get; set; }
        /// <summary>
        /// 计数-PV
        /// </summary>
        [ExcelColumnName("计数-PV")]
        public decimal CountPV { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumnName("备注")]
        public string Remark { get; set; }

    }
}
