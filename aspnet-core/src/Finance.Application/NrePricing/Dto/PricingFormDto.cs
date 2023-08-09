using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;

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
        /// 模具清单 (模具费用)
        /// </summary>
        public List<MouldInventoryModel> MouldInventory { get; set; }
        /// <summary>
        /// 工装费用
        /// </summary>
        public List<ToolingCostModel> ToolingCost { get; set; }
        /// <summary>
        /// 治具费用
        /// </summary>
        public List<FixtureCostModel> FixtureCost { get; set; }
        /// <summary>
        ///  检具费用
        /// </summary>
        public List<QADepartmentQCModel> QAQCDepartments { get; set; }
        /// <summary>
        /// 生产设备费用
        /// </summary>
        public List<ProductionEquipmentCostModel> ProductionEquipmentCost { get; set; }
        /// <summary>
        /// 实验费 模型
        /// </summary>
        public List<LaboratoryFeeModel> LaboratoryFeeModels { get; set; }
        /// <summary>
        /// 测试软件费用
        /// </summary>
        public List<SoftwareTestingCotsModel> SoftwareTestingCost { get; set; }
        /// <summary>
        /// 差旅费
        /// </summary>
        public List<TravelExpenseModel> TravelExpense { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public List<RestsCostModel> RestsCost { get; set; }
        /// <summary>
        /// (不含税人民币) NRE 总费用
        /// </summary>
        public decimal RMBAllCost { get; set; }
        /// <summary>
        /// (不含税美金) NRE 总费用
        /// </summary>
        public decimal USDAllCost { get; set; }
    }
}
