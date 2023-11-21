using Finance.Ext;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 核价表 交互类
    /// </summary>
    public class PricingFormDto
    {
        /// <summary>
        /// 记录编号=>版本号
        /// </summary>
        public string RecordNumber { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 产能需求
        /// </summary>
        public string RequiredCapacity { get; set; }
        /// <summary>
        /// 编制日期
        /// </summary>
        public DateTime CompileDate { get { return DateTime.Now; } }
        /// <summary>
        /// <summary>
        /// 手板件费用
        /// </summary>
        public List<HandPieceCostModel> HandPieceCost { get; set; }
        /// <summary>
        /// <summary>
        /// 手板件费用合计
        /// </summary>
        public decimal HandPieceCostTotal { get; set; }
        /// <summary>
        /// 模具清单 (模具费用)
        /// </summary>
        public List<MouldInventoryModel> MouldInventory { get; set; }
        /// <summary>
        /// 模具清单 (模具费用)合计
        /// </summary>
        public decimal MouldInventoryTotal { get; set; }
        /// <summary>
        /// 工装费用
        /// </summary>
        public List<ToolingCostModel> ToolingCost { get; set; }
        /// <summary>
        /// 工装费用合计
        /// </summary>
        public decimal ToolingCostTotal { get; set; }
        /// <summary>
        /// 治具费用
        /// </summary>
        public List<FixtureCostModel> FixtureCost { get; set; }
        /// <summary>
        /// 治具费用合计
        /// </summary>
        public decimal FixtureCostTotal { get; set; }
        /// <summary>
        ///  检具费用
        /// </summary>
        public List<QADepartmentQCModel> QAQCDepartments { get; set; }
        /// <summary>
        ///  检具费用合计
        /// </summary>
        public decimal QAQCDepartmentsTotal { get; set; }
        /// <summary>
        /// 生产设备费用
        /// </summary>
        public List<ProductionEquipmentCostModel> ProductionEquipmentCost { get; set; }
        /// <summary>
        /// 生产设备费用合计
        /// </summary>
        public decimal ProductionEquipmentCostTotal { get; set; }
        /// <summary>
        /// 实验费 模型
        /// </summary>
        public List<LaboratoryFeeModel> LaboratoryFeeModels { get; set; }
        /// <summary>
        /// 实验费 模型合计
        /// </summary>
        public decimal LaboratoryFeeModelsTotal { get; set; }
        /// <summary>
        /// 测试软件费用
        /// </summary>
        public List<SoftwareTestingCotsModel> SoftwareTestingCost { get; set; }
        /// <summary>
        /// 测试软件费用合计
        /// </summary>
        public decimal SoftwareTestingCostTotal { get; set; }
        /// <summary>
        /// 差旅费
        /// </summary>
        public List<TravelExpenseModel> TravelExpense { get; set; }
        /// <summary>
        /// 差旅费合计
        /// </summary>
        public decimal TravelExpenseTotal { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public List<RestsCostModel> RestsCost { get; set; }
        /// <summary>
        /// 其他费用合计
        /// </summary>
        public decimal RestsCostTotal { get; set; }
        /// <summary>
        /// (不含税人民币) NRE 总费用
        /// </summary>
        public decimal RMBAllCost { get; set; }
        /// <summary>
        /// (不含税美金) NRE 总费用
        /// </summary>
        public decimal USDAllCost { get; set; }
        /// <summary>
        /// 线体数量和共线分摊率的值
        /// </summary>
        public List<UphAndValue> UphAndValues { get; set; }
    }
    /// <summary>
    /// Nre 核价表 交互类 加修改项
    /// </summary>
    public class ModifyItemPricingFormDto : PricingFormDto
    {
        /// <summary>
        /// 手板件费用修改项
        /// </summary>
        public List<HandPieceCostModifyDto> HandPieceCostModifyDtos { get; set; }
        /// <summary>
        /// 模具费用修改项
        /// </summary>
        public List<MouldInventoryModifyDto> MouldInventoryModifyDtos { get; set; }
        /// <summary>
        /// 工装费用修改项
        /// </summary>
        public List<ToolingCostsModifyDto> ToolingCostsModifyDtos { get; set; }
        /// <summary>
        /// 治具费用修改项
        /// </summary>
        public List<FixtureCostsModifyDto> FixtureCostsModifyDtos { get; set; }
        /// <summary>
        /// 检具费用修改项
        /// </summary>
        public List<InspectionToolCostModifyDto> InspectionToolCostModifyDtos { get; set; }
        /// <summary>
        /// 生产设备费用修改项
        /// </summary>
        public List<ProductionEquipmentCostsModifyDto> ProductionEquipmentCostsModifyDtos { get; set; }
        /// <summary>
        /// 实验费用修改项
        /// </summary>
        public List<ExperimentalExpensesModifyDto> ExperimentalExpensesModifyDtos { get; set; }
        /// <summary>
        /// 测试软件费用修改项
        /// </summary>
        public List<TestingSoftwareCostsModifyDto> TestingSoftwareCostsModifyDtos { get; set; }
        /// <summary>
        /// 差旅费用修改项
        /// </summary>
        public List<TravelExpenseModifyDto> TravelExpenseModifyDtos { get; set; }
        /// <summary>
        /// 其他费用修改项
        /// </summary>
        public List<RestsCostModifyDto> RestsCostModifyDtos { get; set; }
    }
    /// <summary>
    /// 线体数量和共线分摊率的值
    /// </summary>
    public class UphAndValue
    {
        /// <summary>
		/// UPH参数
		/// </summary>
        public string Uph { get; set; }
        /// <summary>
		/// 值
		/// </summary>
		[Column("value")]
        public decimal Value { get; set; }
        /// <summary>
        /// Description注释名
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 年份类型
        /// </summary>       
        public virtual YearType UpDown { get; set; }
    }
    /// <summary>
    /// Nre 核价表 带流程ID 交互类
    /// </summary>
    public class AuditFlowIdPricingFormDto
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        [FriendlyRequired("流程ID")]
        public  long AuditFlowId { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        [FriendlyRequired("方案ID")]
        public long SolutionId { get; set; }
        /// <summary>
        /// 交互数据
        /// </summary>
        public PricingFormDto PricingFormDto { get; set; }
    }
}
