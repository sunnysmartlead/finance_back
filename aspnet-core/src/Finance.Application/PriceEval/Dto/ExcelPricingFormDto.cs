using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// Excel导入使用
    /// </summary>
    public class ExcelPricingFormDto //: PricingFormDto
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
        public List<ExcelHandPieceCostModel> HandPieceCost { get; set; }

        /// <summary>
        /// 模具清单 (模具费用)
        /// </summary>
        public List<ExcelMouldInventoryModel> MouldInventory { get; set; }
        /// <summary>
        /// 工装费用
        /// </summary>
        public List<ExcelToolingCostModel> ToolingCost { get; set; }
        /// <summary>
        /// 治具费用
        /// </summary>
        public List<ExcelFixtureCostModel> FixtureCost { get; set; }
        /// <summary>
        ///  检具费用
        /// </summary>
        public List<ExcelQADepartmentQCModel> QAQCDepartments { get; set; }
        /// <summary>
        /// 生产设备费用
        /// </summary>
        public List<ExcelProductionEquipmentCostModel> ProductionEquipmentCost { get; set; }
        /// <summary>
        /// 实验费 模型
        /// </summary>
        public List<ExcelLaboratoryFeeModel> LaboratoryFeeModels { get; set; }
        /// <summary>
        /// 测试软件费用
        /// </summary>
        public List<ExcelSoftwareTestingCotsModel> SoftwareTestingCost { get; set; }
        /// <summary>
        /// 差旅费
        /// </summary>
        public List<ExcelTravelExpenseModel> TravelExpense { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public List<ExcelRestsCostModel> RestsCost { get; set; }


        /// <summary>
        /// <summary>
        /// 手板件费用合计
        /// </summary>
        public decimal HandPieceCostSum { get; set; }

        /// <summary>
        /// 模具清单 (模具费用)合计
        /// </summary>
        public decimal MouldInventorySum { get; set; }
        /// <summary>
        /// 工装费用合计
        /// </summary>
        public decimal ToolingCostSum { get; set; }
        /// <summary>
        /// 治具费用合计
        /// </summary>
        public decimal FixtureCostSum { get; set; }
        /// <summary>
        ///  检具费用合计
        /// </summary>
        public decimal QAQCDepartmentsSum { get; set; }
        /// <summary>
        /// 生产设备费用合计
        /// </summary>
        public decimal ProductionEquipmentCostSum { get; set; }
        /// <summary>
        /// 实验费 模型合计
        /// </summary>
        public decimal LaboratoryFeeModelsSum { get; set; }
        /// <summary>
        /// 测试软件费用合计
        /// </summary>
        public decimal SoftwareTestingCostSum { get; set; }
        /// <summary>
        /// 差旅费合计
        /// </summary>
        public decimal TravelExpenseSum { get; set; }
        /// <summary>
        /// 其他费用合计
        /// </summary>
        public decimal RestsCostSum { get; set; }

        /// <summary>
        /// (不含税人民币) NRE 总费用
        /// </summary>
        public decimal RMBAllCost { get; set; }
        /// <summary>
        /// (不含税美金) NRE 总费用
        /// </summary>
        public decimal USDAllCost { get; set; }
    }

    public class ExcelHandPieceCostModel : HandPieceCostModel
    {
        public virtual int Index { get; set; }
    }
    public class ExcelMouldInventoryModel : MouldInventoryModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelToolingCostModel : ToolingCostModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelFixtureCostModel : FixtureCostModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelQADepartmentQCModel : QADepartmentQCModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelProductionEquipmentCostModel : ProductionEquipmentCostModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelLaboratoryFeeModel : LaboratoryFeeModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelSoftwareTestingCotsModel : SoftwareTestingCotsModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelTravelExpenseModel : TravelExpenseModel
    {
        public virtual int Index { get; set; }

    }
    public class ExcelRestsCostModel : RestsCostModel
    {
        public virtual int Index { get; set; }

    }
}
