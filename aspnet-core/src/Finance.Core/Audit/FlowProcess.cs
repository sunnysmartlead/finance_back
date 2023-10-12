using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    /// <summary>
    /// 表示流转意见类型（1：“提交”，2：“拒绝”）
    /// </summary>
    public enum OPINIONTYPE : byte
    {
        /// <summary>
        ///提交
        /// </summary>
        [Description("提交或同意")]
        Submit_Agreee = 1,
        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Reject = 2,
    }
    /// <summary>
    /// 角色权限（1：“查看”，2：“编辑”）
    /// </summary>
    public enum RIGHTTYPE : byte
    {
        /// <summary>
        ///查看
        /// </summary>
        [Description("查看")]
        ReadOnly = 1,
        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        Edit = 2,
    }

    /// <summary>
    /// 界面标识符定义
    /// </summary>
    public class AuditFlowConsts
    {
        /// <summary>
        /// 核价需求录入界面
        /// </summary>
        public const string AF_RequirementInput = "RequirementInput";
        /// <summary>
        /// 项目经理录入界面
        /// </summary>
        public const string AF_PMInput = "PMInput";
        /// <summary>
        /// 市场部TR主方案审核界面
        /// </summary>
        public const string AF_TRAuditMKT = "TRAuditMKT";
        /// <summary>
        /// 产品开发部TR主方案界面
        /// </summary>
        public const string AF_TRAuditR_D = "TRAuditR_D";
        /// <summary>
        /// 结构bom导入界面
        /// </summary>
        public const string AF_StructBomImport = "StructBomImport";
        /// <summary>
        /// 结构bom审核界面
        /// </summary>
        public const string AF_StructBomAudit = "StructBomAudit";
        /// <summary>
        /// 电子bom导入界面
        /// </summary>
        public const string AF_ElectronicBomImport = "ElectronicBomImport";
        /// <summary>
        /// 电子bom导入界面-拼版
        /// </summary>
        public const string AF_ElectronicBomImportPb = "ElectronicBomImportPb";
        /// <summary>
        /// 电子bom审核界面
        /// </summary>
        public const string AF_ElectronicBomAudit = "ElectronicBomAudit";
        /// <summary>
        /// 结构单价录入界面
        /// </summary>
        public const string AF_StructPriceInput = "StructPriceInput";
        /// <summary>
        /// 电子单价录入界面
        /// </summary>
        public const string AF_ElectronicPriceInput = "ElectronicPriceInput";
        /// <summary>
        /// 电子bom单价审核界面
        /// </summary>
        public const string AF_ElecBomPriceAudit = "ElecBomPriceAudit";
        /// <summary>
        /// 结构bom单价审核界面
        /// </summary>
        public const string AF_StructBomPriceAudit = "StructBomPriceAudit";
        /// <summary>
        /// 工程电子损耗率录入界面
        /// </summary>
        public const string AF_ElecLossRateInput = "ElecLossRateInput";
        /// <summary>
        /// 工程结构损耗率录入界面
        /// </summary>
        public const string AF_StructLossRateInput = "StructLossRateInput";
        /// <summary>
        /// 工序工时导入界面
        /// </summary>
        public const string AF_ManHourImport = "ManHourImport";
        /// <summary>
        /// 制造成本录入界面
        /// </summary>
        public const string AF_ProductionCostInput = "ProductionCostInput";
        /// <summary>
        /// 物流成本录入界面
        /// </summary>
        public const string AF_LogisticsCostInput = "LogisticsCostInput";
        /// <summary>
        /// NRE核价手板件、差旅及其他费用录入界面
        /// </summary>
        public const string AF_NreInputOther = "NreInputOther";
        /// <summary>
        /// NRE核价电子-EMC实验费录入界面
        /// </summary>
        public const string AF_NreInputEmc = "NreInputEmc";
        /// <summary>
        /// NRE模具清单录入界面
        /// </summary>
        public const string AF_NreInputMould = "NreInputMould";
        /// <summary>
        /// NRE实验费用录入界面
        /// </summary>
        public const string AF_NreInputTest = "NreInputTest";
        /// <summary>
        /// NRE检具费用录入界面
        /// </summary>
        public const string AF_NreInputGage = "NreInputGage";
        /// <summary>
        /// 贸易合规审核（结束界面&退回界面）
        /// </summary>
        public const string AF_TradeApproval = "TradeApproval";
        /// <summary>
        /// 核价看板&界面退回界面
        /// </summary>
        public const string AF_PriceBoardAudit = "PriceBoardAudit";
        /// <summary>
        /// 项目部核价审核界面
        /// </summary>
        public const string AF_ProjectPriceAudit = "ProjectPriceAudit";
        /// <summary>
        /// 财务部核价审核界面
        /// </summary>
        public const string AF_FinancePriceAudit = "FinancePriceAudit";
        /// <summary>
        /// 成本信息表下载&填报NRE报价系数&产品报价看板界面
        /// </summary>
        public const string AF_CostCheckNreFactor = "CostCheckNreFactor";
        /// <summary>
        /// 总经理报价审批界面
        /// </summary>
        public const string AF_QuoteApproval = "QuoteApproval";
        /// <summary>
        /// "归档下载界面
        /// </summary>
        public const string AF_ArchiveEnd = "ArchiveEnd";
    }

    /// <summary>
    /// 界面类型
    /// </summary>
    public class FlowProcess : FullAuditedEntity<long>
    {
        /// <summary>
        /// 界面Index
        /// </summary>
        [Column("PROCESSIDENTIFIER")]
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 界面名称
        /// </summary>
        [Column("PROCESSNAME")]
        public string ProcessName { get; set; }
        /// <summary>
        /// 编辑角色
        /// </summary>
        [Column("EDITROLE")]
        public string EditRole { get; set; }
        /// <summary>
        /// 查看角色
        /// </summary>
        [Column("READONLYROLE")]
        public string ReadonlyRole { get; set; }
    }


    public class GeneralDefinition
    {
        public const string TRADE_COMPLIANCE_OK = "合规";

        public const string TRADE_COMPLIANCE_NOT_OK = "不合规";
    }

    public class OpinionDescription
    {
        public const string OD_MKT_TRMainCheck = "市场部TR主方案审核：";

        public const string OD_MKT_TRMainAgree = "市场部TR主方案审核默认同意";

        public const string OD_R_D_TRMainCheck = "产品开发部TR主方案审核：";

        public const string OD_ElecBomCheck = "电子bom审核：";

        public const string OD_StructBomCheck = "结构bom审核：";

        public const string OD_ElecBomPriceCheck = "电子bom单价审核：";

        public const string OD_StructBomPriceCheck = "结构bom单价审核：";

        public const string OD_PriceBoardCheck = "核价看板审核：";

        public const string OD_ProjectPriceCheck = "项目部核价审核：";

        public const string OD_FinancePriceCheck = "财务部核价审核：";

        public const string OD_TradeComplianceJudge = "贸易合规判定：";

        public const string OD_QuotationCheck = "营销部不报价：";

        public const string OD_GM_QuotationJudge = "总经理报价审批：";

    }
}
