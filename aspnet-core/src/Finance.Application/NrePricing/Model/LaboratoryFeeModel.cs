using Castle.MicroKernel.SubSystems.Conversion;
using Finance.Ext;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 带零件id 的  实验费 模型
    /// </summary>
    public class IsSubmitLaboratoryFeeModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }


        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 实验费 模型
        /// </summary>
        public List<LaboratoryFeeModel> laboratoryFeeModels { get; set; }
    }

    /// <summary>
    /// 带零件id 的 品保部=>试验项目 录入 模型
    /// </summary>
    public class LaboratoryItemsModel
    {
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        ///品保录入  EMC项目表 模型
        /// </summary>
        public List<LaboratoryFeeModel> laboratoryFeeModels { get; set; }
        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }






    /// <summary>
    /// EMC 实验费 模型
    /// </summary>
    public class LaboratoryFeeModel
    {
        /// <summary>
        /// 实验费类型 判断是EMC实验费还是环境实验费  1是EMC实验费2是环境实验费
        /// </summary>
        public int ExpenseType { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 试验项目(根据与客户协定项目)
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 是否指定第三方
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
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountBottomingOut { get; set; }
        /// <summary>
        /// 计数-DV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountDV { get; set; }
        /// <summary>
        /// 计数-PV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
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
    /// EMC 实验费 Excel模型
    /// </summary>
    public class LaboratoryFeeExcelModel
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
        /// 是否指定第三方 (是 true   否 false)
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
