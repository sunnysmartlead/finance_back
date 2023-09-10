using Finance.Authorization.Roles;
using Finance.WorkFlows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class WorkFlowCreator
    {
        private readonly FinanceDbContext _context;

        public WorkFlowCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateWorkFlows();
        }
        public const string MainFlowId = "主流程";
        private void CreateWorkFlows()
        {
            //获取角色1
            var role1 = _context.Roles.FirstOrDefault(p => p.Name == "测试角色1");
            var roleId = role1.Id.ToString();


            #region 获取角色

            var salesMan = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.SalesMan);
            var projectManager = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ProjectManager);
            var electronicsEngineer = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ElectronicsEngineer);
            var electronicsBomAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ElectronicsBomAuditor);
            var structuralEngineer = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.StructuralEngineer);
            var structuralBomAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.StructuralBomAuditor);

            var r_D_TRAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.R_D_TRAuditor);
            var electronicsPriceInputter = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ElectronicsPriceInputter);
            var structuralPriceInputter = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.StructuralPriceInputter);
            var electronicsPriceAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ElectronicsPriceAuditor);
            var structuralPriceAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.StructuralPriceAuditor);
            var manHourInputter = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ManHourInputter);

            var logisticsCostInputter = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.LogisticsCostInputter);
            var generalManager = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.GeneralManager);
            var tradeComplianceAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.TradeComplianceAuditor);
            var financeProductCostInputter = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.FinanceProductCostInputter);
            var projectPriceAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ProjectPriceAuditor);
            var financePriceAuditor = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.FinancePriceAuditor);

            var timeliness = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.Timeliness);
            var projectChief = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ProjectChief);
            var marketProjectMinister = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.MarketProjectMinister);
            var marketProjectManager = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.MarketProjectManager);
            var marketProjectChief = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.MarketProjectChief);
            var projectMinister = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ProjectMinister);

            var structuralInput = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.StructuralInput);
            var bomInput = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.BomInput);
            var modelInput = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ModelInput);
            var modelEval = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.ModelEval);
            var environmentInput = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.EnvironmentInput);
            var environmentEval = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.EnvironmentEval);

            var emcInput = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.EmcInput);
            var emcEval = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.EmcEval);
            var financeEval = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.FinanceEval);
            var financeTableAdmin = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.FinanceTableAdmin);
            var evalTableAdmin = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.EvalTableAdmin);
            var costSplit = _context.Roles.FirstOrDefault(p => p.Name == StaticRoleNames.Host.CostSplit);
            #endregion


            #region 主流程结构定义
            var workFlow = new Workflow
            {
                Id = MainFlowId,
                Name = "主流程",
                Version = 1
            };

            var nodeList = new List<Node>
            {
                new Node
                {
                    Name = "开始",
                    FinanceDictionaryId = string.Empty,
                    Activation = string.Empty,
                    NodeType= NodeType.Start,
                },
                new Node
                {
                    Name="核价需求录入",
                    FinanceDictionaryId = FinanceConsts.EvalReason,
                    Activation = $"{MainFlowId}_核价审批录入_{MainFlowId}_核价需求录入 || {MainFlowId}_开始_{MainFlowId}_核价需求录入",
                    RoleId = salesMan.Id.ToString(),
                    ProcessIdentifier = "PricingDemandInput",
                },
                new Node
                {
                    Name="核价审批录入",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_核价需求录入_{MainFlowId}_核价审批录入 || {MainFlowId}_TR审核_{MainFlowId}_核价审批录入",
                    RoleId = $"{projectManager.Id},{marketProjectManager.Id}",
                    ProcessIdentifier = "PriceDemandReview",
                },
                new Node
                {
                    Name="TR审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_核价审批录入_{MainFlowId}_TR审核",
                    RoleId = r_D_TRAuditor.Id.ToString(),
                    ProcessIdentifier = "TRToExamine",
                },
                new Node
                {
                    Name="NRE_可靠性实验费录入",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_TR审核_{MainFlowId}_NRE_可靠性实验费录入 || {MainFlowId}_NRE_可靠性实验费审核_{MainFlowId}_NRE_可靠性实验费录入 || {MainFlowId}_核价看板_{MainFlowId}_NRE_可靠性实验费录入",
                    RoleId = environmentInput.Id.ToString(),
                    ProcessIdentifier = "NRE_ReliabilityExperimentFeeInput",
                },
                new Node
                {
                    Name="NRE_可靠性实验费审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_NRE_可靠性实验费录入_{MainFlowId}_NRE_可靠性实验费审核",
                    RoleId = environmentEval.Id.ToString(),
                    ProcessIdentifier = "NRE_ReliabilityExperimentFeeInputoExamine",
                },
                new Node
                {
                    Name="NRE_EMC实验费审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_NRE_EMC实验费录入_{MainFlowId}_NRE_EMC实验费审核",
                    RoleId = emcEval.Id.ToString(),
                    ProcessIdentifier = "NRE_EMCExperimentalFeeInputToExamine",
                },
                new Node
                {
                    Name="NRE_EMC实验费录入",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation =  $"{MainFlowId}_TR审核_{MainFlowId}_NRE_EMC实验费录入 || {MainFlowId}_NRE_EMC实验费审核_{MainFlowId}_NRE_EMC实验费录入 || {MainFlowId}_核价看板_{MainFlowId}_NRE_EMC实验费录入",
                    RoleId = emcInput.Id.ToString(),
                    ProcessIdentifier = "NRE_EMCExperimentalFeeInput",
                },
                new Node
                {
                    Name="上传结构BOM",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation =  $"{MainFlowId}_TR审核_{MainFlowId}_上传结构BOM || {MainFlowId}_结构BOM审核_{MainFlowId}_上传结构BOM || {MainFlowId}_核价看板_{MainFlowId}_上传结构BOM || {MainFlowId}_不合规是否退回_{MainFlowId}_上传结构BOM",
                    RoleId = structuralEngineer.Id.ToString(),
                    ProcessIdentifier = "StructureBOM",
                },
                new Node
                {
                    Name="上传电子BOM",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_TR审核_{MainFlowId}_上传电子BOM || {MainFlowId}_电子BOM审核_{MainFlowId}_上传电子BOM || {MainFlowId}_核价看板_{MainFlowId}_上传电子BOM || {MainFlowId}_不合规是否退回_{MainFlowId}_上传电子BOM",
                    RoleId = electronicsEngineer.Id.ToString(),
                    ProcessIdentifier = "ElectronicsBOM",
                },
                new Node
                {
                    Name="电子BOM审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_上传电子BOM_{MainFlowId}_电子BOM审核",
                    RoleId = electronicsBomAuditor.Id.ToString(),
                    ProcessIdentifier = "ElectronicsBOMToExamine",
                },
                new Node
                {
                    Name="NRE手板件",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_核价审批录入_{MainFlowId}_NRE手板件 || {MainFlowId}_核价看板_{MainFlowId}_NRE手板件",
                    RoleId= $"{projectManager.Id},{marketProjectManager.Id}",
                    ProcessIdentifier = "NRE_ManualComponentInput",
                },
                new Node
                {
                    Name="结构BOM审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_上传结构BOM_{MainFlowId}_结构BOM审核",
                    RoleId = structuralBomAuditor.Id.ToString(),
                    ProcessIdentifier = "StructureBOMToExamine",
                },
                 new Node
                {
                    Name="NRE模具费录入",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_结构BOM审核_{MainFlowId}_NRE模具费录入 || {MainFlowId}_模具费审核_{MainFlowId}_NRE模具费录入 || {MainFlowId}_核价看板_{MainFlowId}_NRE模具费录入",
                    RoleId = modelInput.Id.ToString(),
                    ProcessIdentifier = "NRE_MoldFeeEntry",
                 },
                new Node
                {
                    Name="物流成本录入",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_结构BOM审核_{MainFlowId}_物流成本录入 || {MainFlowId}_不合规是否退回_{MainFlowId}_物流成本录入 || {MainFlowId}_核价看板_{MainFlowId}_物流成本录入",
                    RoleId = logisticsCostInputter.Id.ToString(),
                    ProcessIdentifier = "LogisticsCostEntry",
                },
                new Node
                {
                    Name="工序工时添加",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"({MainFlowId}_结构BOM审核_{MainFlowId}_工序工时添加 && {MainFlowId}_电子BOM审核_{MainFlowId}_工序工时添加) || {MainFlowId}_不合规是否退回_{MainFlowId}_工序工时添加 || {MainFlowId}_核价看板_{MainFlowId}_工序工时添加",
                    RoleId = manHourInputter.Id.ToString(),
                    ProcessIdentifier = "FormulaOperationAddition",
                },
                new Node
                {
                    Name="定制结构件",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_结构BOM审核_{MainFlowId}_定制结构件 || {MainFlowId}_结构BOM单价审核_{MainFlowId}_定制结构件 || {MainFlowId}_BOM成本审核_{MainFlowId}_定制结构件 || {MainFlowId}_不合规是否退回_{MainFlowId}_定制结构件 || {MainFlowId}_核价看板_{MainFlowId}_定制结构件",
                    RoleId = structuralInput.Id.ToString(),
                    ProcessIdentifier = "CustomizedStructuralComponents",
                },
                new Node
                {
                    Name="结构BOM匹配修改",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_结构BOM审核_{MainFlowId}_结构BOM匹配修改 || {MainFlowId}_结构BOM单价审核_{MainFlowId}_结构BOM匹配修改 || {MainFlowId}_BOM成本审核_{MainFlowId}_结构BOM匹配修改 || {MainFlowId}_不合规是否退回_{MainFlowId}_结构BOM匹配修改 || {MainFlowId}_核价看板_{MainFlowId}_结构BOM匹配修改",
                    RoleId = structuralPriceInputter.Id.ToString(),
                    ProcessIdentifier = "StructureUnitPriceEntry",
                },
                new Node
                {
                    Name="电子BOM匹配修改",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_电子BOM审核_{MainFlowId}_电子BOM匹配修改 || {MainFlowId}_BOM成本审核_{MainFlowId}_电子BOM匹配修改 || {MainFlowId}_不合规是否退回_{MainFlowId}_电子BOM匹配修改 || {MainFlowId}_核价看板_{MainFlowId}_电子BOM匹配修改",
                    RoleId = electronicsPriceInputter.Id.ToString(),
                    ProcessIdentifier = "ElectronicUnitPriceEntry",
                },
                new Node
                {
                    Name="模具费审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_NRE模具费录入_{MainFlowId}_模具费审核",
                    RoleId = modelEval.Id.ToString(),
                    ProcessIdentifier = "NRE_MoldFeeEntryToExamine",
                },
                new Node
                {
                    Name="贸易合规",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_物流成本录入_{MainFlowId}_贸易合规 && {MainFlowId}_COB制造成本录入_{MainFlowId}_贸易合规",
                    RoleId = tradeComplianceAuditor.Id.ToString(),
                    ProcessIdentifier = "TradeCompliance",
                },
                new Node
                {
                    Name="COB制造成本录入",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"({MainFlowId}_工序工时添加_{MainFlowId}_COB制造成本录入 && {MainFlowId}_BOM成本审核_{MainFlowId}_COB制造成本录入) || {MainFlowId}_不合规是否退回_{MainFlowId}_COB制造成本录入 || {MainFlowId}_核价看板_{MainFlowId}_COB制造成本录入",
                    RoleId = financeProductCostInputter.Id.ToString(),
                    ProcessIdentifier = "COBManufacturingCostEntry",
                },
                new Node
                {
                    Name="BOM成本审核",
                    FinanceDictionaryId = FinanceConsts.BomEvalSelect,
                    Activation = $"{MainFlowId}_结构BOM单价审核_{MainFlowId}_BOM成本审核 && {MainFlowId}_电子BOM单价审核_{MainFlowId}_BOM成本审核",
                    RoleId = bomInput.Id.ToString(),
                    ProcessIdentifier = "UnitPriceInputReviewToExamine",
                },
                new Node
                {
                    Name="结构BOM单价审核",
                    FinanceDictionaryId = FinanceConsts.StructBomEvalSelect,
                    Activation = $"{MainFlowId}_定制结构件_{MainFlowId}_结构BOM单价审核 && {MainFlowId}_结构BOM匹配修改_{MainFlowId}_结构BOM单价审核",
                    RoleId = structuralPriceAuditor.Id.ToString(),
                    ProcessIdentifier = "StructureUnitPriceEntryToExamine",
                },
                  new Node
                {
                    Name="电子BOM单价审核",
                    FinanceDictionaryId = FinanceConsts.ElectronicBomEvalSelect,
                    Activation = $"{MainFlowId}_电子BOM匹配修改_{MainFlowId}_电子BOM单价审核",
                    RoleId = electronicsPriceAuditor.Id.ToString(),
                    ProcessIdentifier = "ElectronicUnitPriceEntryToExamine",
                },
                new Node
                {
                    Name="不合规是否退回",
                    FinanceDictionaryId = FinanceConsts.MybhgSelect,
                    Activation = $"{MainFlowId}_贸易合规_{MainFlowId}_不合规是否退回",
                    RoleId = tradeComplianceAuditor.Id.ToString(),//和 贸易合规 相同
                },
                new Node
                {
                    Name="查看每个方案初版BOM成本",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_BOM成本审核_{MainFlowId}_查看每个方案初版BOM成本",
                    RoleId = $"{projectManager.Id},{marketProjectManager.Id}",
                    ProcessIdentifier = "UnitPriceInputReviewToExamine",
                },
                new Node
                {
                    Name="核价看板",
                    FinanceDictionaryId = FinanceConsts.HjkbSelect,
                    Activation = $"({MainFlowId}_模具费审核_{MainFlowId}_核价看板 && {MainFlowId}_贸易合规_{MainFlowId}_核价看板 && {MainFlowId}_NRE_EMC实验费审核_{MainFlowId}_核价看板 && {MainFlowId}_NRE_可靠性实验费审核_{MainFlowId}_核价看板 && {MainFlowId}_NRE手板件_{MainFlowId}_核价看板) || ({MainFlowId}_核价需求录入_{MainFlowId}_核价看板 || {MainFlowId}_财务审核_{MainFlowId}_核价看板 || {MainFlowId}_项目部课长审核_{MainFlowId}_核价看板 || {MainFlowId}_审批报价策略与核价表_{MainFlowId}_核价看板)",
                    RoleId = $"{projectManager.Id},{marketProjectManager.Id}",
                    ProcessIdentifier = "PriceEvaluationBoard",
                },
                new Node
                {
                    Name="项目部课长审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_核价看板_{MainFlowId}_项目部课长审核",
                    RoleId = $"{projectChief.Id},{marketProjectChief.Id}",
                    ProcessIdentifier = "ProjectChiefAudit",

                },
                 new Node
                {
                    Name="财务审核",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_项目部课长审核_{MainFlowId}_财务审核",
                    RoleId = financePriceAuditor.Id.ToString(),
                    ProcessIdentifier = "FinanceDirectorAudit",
                },
                 new Node
                {
                    Name="生成报价分析界面选择报价方案",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_财务审核_{MainFlowId}_生成报价分析界面选择报价方案 || {MainFlowId}_报价反馈_{MainFlowId}_生成报价分析界面选择报价方案 || {MainFlowId}_确认中标金额_{MainFlowId}_生成报价分析界面选择报价方案",
                    RoleId = salesMan.Id.ToString(),
                    ProcessIdentifier = "QuoteAnalysis",

                 },
                new Node
                {
                    Name="选择是否报价",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_生成报价分析界面选择报价方案_{MainFlowId}_选择是否报价",
                    RoleId = salesMan.Id.ToString(),//和 生成报价分析界面选择报价方案  相同
                },
                new Node
                {
                    Name="审批报价策略与核价表",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_选择是否报价_{MainFlowId}_审批报价策略与核价表 || {MainFlowId}_报价反馈_{MainFlowId}_审批报价策略与核价表",
                    RoleId = generalManager.Id.ToString(),
                },
                new Node
                {
                    Name="系统生成报价审批表报价单",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_审批报价策略与核价表_{MainFlowId}_系统生成报价审批表报价单",
                    RoleId = salesMan.Id.ToString(),
                },
                new Node
                {
                    Name="项目部长查看核价表",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_财务审核_{MainFlowId}_项目部长查看核价表",
                    RoleId = $"{marketProjectMinister.Id},{projectMinister.Id}",
                    ProcessIdentifier = "ProjectDirectorLook",

                },
                new Node
                {
                    Name="核心器件成本NRE费用拆分",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_财务审核_{MainFlowId}_核心器件成本NRE费用拆分",
                    RoleId = costSplit.Id.ToString(),
                },
                new Node
                {
                    Name="确认中标金额",
                    FinanceDictionaryId = FinanceConsts.YesOrNo,
                    Activation = $"{MainFlowId}_报价反馈_{MainFlowId}_确认中标金额",
                    RoleId = financeEval.Id.ToString()
                },
                new Node
                {
                    Name="总经理查看中标金额",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_确认中标金额_{MainFlowId}_总经理查看中标金额",
                    RoleId = generalManager.Id.ToString()
                },
                new Node
                {
                    Name="报价反馈",
                    FinanceDictionaryId = FinanceConsts.EvalFeedback,
                    Activation = $"{MainFlowId}_系统生成报价审批表报价单_{MainFlowId}_报价反馈",
                    RoleId = salesMan.Id.ToString(),
                },
                new Node
                {
                    Name="归档",
                    FinanceDictionaryId = FinanceConsts.Done,
                    Activation = $"{MainFlowId}_不合规是否退回_{MainFlowId}_归档 || {MainFlowId}_确认中标金额_{MainFlowId}_归档 || {MainFlowId}_选择是否报价_{MainFlowId}_归档 || {MainFlowId}_报价反馈_{MainFlowId}_归档",
                    NodeType= NodeType.End,
                    RoleId = $"{projectManager.Id},{marketProjectManager.Id},{financeTableAdmin.Id},{evalTableAdmin.Id}"
                },
            };

            var lineList = new List<Line>
            {
                new Line
                {
                    SoureNodeId = "开始",
                    TargetNodeId = "核价需求录入",
                    Index = 0,
                    FinanceDictionaryDetailId = string.Empty,
                },
                new Line
                {
                    SoureNodeId = "核价需求录入",
                    TargetNodeId = "核价审批录入",
                    Index = 0,
                    FinanceDictionaryDetailId = $"{FinanceConsts.EvalReason_Bb1},{FinanceConsts.EvalReason_Fabg},{FinanceConsts.EvalReason_Qt},{FinanceConsts.EvalReason_Jdcbpg},{FinanceConsts.EvalReason_Xmbg}",
                },
                new Line
                {
                    SoureNodeId = "核价审批录入",
                    TargetNodeId = "核价需求录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "核价审批录入",
                    TargetNodeId = "NRE手板件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "核价审批录入",
                    TargetNodeId = "TR审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "TR审核",
                    TargetNodeId = "核价审批录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "TR审核",
                    TargetNodeId = "NRE_EMC实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "TR审核",
                    TargetNodeId = "上传结构BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "TR审核",
                    TargetNodeId = "上传电子BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "TR审核",
                    TargetNodeId = "NRE_可靠性实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "NRE_可靠性实验费录入",
                    TargetNodeId = "NRE_可靠性实验费审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "NRE_可靠性实验费审核",
                    TargetNodeId = "NRE_可靠性实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "NRE_EMC实验费录入",
                    TargetNodeId = "NRE_EMC实验费审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "NRE_EMC实验费审核",
                    TargetNodeId = "NRE_EMC实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "上传结构BOM",
                    TargetNodeId = "结构BOM审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "上传结构BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "上传电子BOM",
                    TargetNodeId = "电子BOM审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "电子BOM审核",
                    TargetNodeId = "上传电子BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "NRE模具费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "物流成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "工序工时添加",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "定制结构件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "结构BOM审核",
                    TargetNodeId = "结构BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "电子BOM审核",
                    TargetNodeId = "工序工时添加",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "电子BOM审核",
                    TargetNodeId = "电子BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "NRE模具费录入",
                    TargetNodeId = "模具费审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "模具费审核",
                    TargetNodeId = "NRE模具费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "物流成本录入",
                    TargetNodeId = "贸易合规",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "工序工时添加",
                    TargetNodeId = "COB制造成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "定制结构件",
                    TargetNodeId = "结构BOM单价审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "结构BOM匹配修改",
                    TargetNodeId = "结构BOM单价审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "电子BOM匹配修改",
                    TargetNodeId = "电子BOM单价审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "电子BOM单价审核",
                    TargetNodeId = "BOM成本审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.ElectronicBomEvalSelect_Yes,
                },
                new Line
                {
                    SoureNodeId = "结构BOM单价审核",
                    TargetNodeId = "BOM成本审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.StructBomEvalSelect_Yes,
                },
                new Line
                {
                    SoureNodeId = "BOM成本审核",
                    TargetNodeId = "COB制造成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Yes,
                },
                new Line
                {
                    SoureNodeId = "COB制造成本录入",
                    TargetNodeId = "贸易合规",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "贸易合规",
                    TargetNodeId = "不合规是否退回",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "贸易合规",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "模具费审核",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "NRE_可靠性实验费审核",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "NRE手板件",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "NRE_EMC实验费审核",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                    FinanceDictionaryDetailIds =$"{FinanceConsts.HjkbSelect_Yes},{FinanceConsts.HjkbSelect_Scjgbom},{FinanceConsts.HjkbSelect_Dzjgj},{FinanceConsts.HjkbSelect_Jgbomppxg},{FinanceConsts.HjkbSelect_Scdzbom},{FinanceConsts.HjkbSelect_Dzbomppxg},{FinanceConsts.HjkbSelect_Wlcblr},{FinanceConsts.HjkbSelect_Gsgxtj},{FinanceConsts.HjkbSelect_Cobzzcblr},{FinanceConsts.HjkbSelect_Nremjflr},{FinanceConsts.HjkbSelect_Nrekkxsyflr},{FinanceConsts.HjkbSelect_Nresbj},{FinanceConsts.HjkbSelect_Nreemcsyflr}"
                },
                new Line
                {
                    SoureNodeId = "核价需求录入",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = $"{FinanceConsts.EvalReason_Yp},{FinanceConsts.EvalReason_Ffabg},{FinanceConsts.EvalReason_Nj},{FinanceConsts.EvalReason_Shj}",
                    FinanceDictionaryDetailIds =$"{FinanceConsts.HjkbSelect_Yes},{FinanceConsts.HjkbSelect_Input}"
                },
                new Line
                {
                    SoureNodeId = "BOM成本审核",
                    TargetNodeId = "查看每个方案初版BOM成本",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Yes,
                },
                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "核价需求录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Input,
                },
                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "项目部课长审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Yes,
                },
                new Line
                {
                    SoureNodeId = "项目部课长审核",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "项目部课长审核",
                    TargetNodeId = "财务审核",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "财务审核",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "财务审核",
                    TargetNodeId = "项目部长查看核价表",
                    Index = 0,
                    FinanceDictionaryDetailId =  FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "财务审核",
                    TargetNodeId = "生成报价分析界面选择报价方案",
                    Index = 0,
                    FinanceDictionaryDetailId =  FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "生成报价分析界面选择报价方案",
                    TargetNodeId = "选择是否报价",
                    Index = 0,
                    FinanceDictionaryDetailId =  FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "选择是否报价",
                    TargetNodeId = "审批报价策略与核价表",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "审批报价策略与核价表",
                    TargetNodeId = "核价看板",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "财务审核",
                    TargetNodeId = "核心器件成本NRE费用拆分",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "审批报价策略与核价表",
                    TargetNodeId = "系统生成报价审批表报价单",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "系统生成报价审批表报价单",
                    TargetNodeId = "报价反馈",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.Done,
                },
                new Line
                {
                    SoureNodeId = "报价反馈",
                    TargetNodeId = "审批报价策略与核价表",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.EvalFeedback_Bjxysp,
                },
                new Line
                {
                    SoureNodeId = "报价反馈",
                    TargetNodeId = "生成报价分析界面选择报价方案",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.EvalFeedback_Bjsdjsjj,
                },
                new Line
                {
                    SoureNodeId = "报价反馈",
                    TargetNodeId = "确认中标金额",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.EvalFeedback_Js,
                },
                new Line
                {
                    SoureNodeId = "确认中标金额",
                    TargetNodeId = "归档",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "确认中标金额",
                    TargetNodeId = "总经理查看中标金额",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                },
                new Line
                {
                    SoureNodeId = "确认中标金额",
                    TargetNodeId = "生成报价分析界面选择报价方案",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "选择是否报价",
                    TargetNodeId = "归档",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },
                new Line
                {
                    SoureNodeId = "报价反馈",
                    TargetNodeId = "归档",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.EvalFeedback_Bjsbzc,
                },
                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "归档",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                },

                #region 随机退回的线定义 

                //结构BOM单价审核

                new Line
                {
                    SoureNodeId = "结构BOM单价审核",
                    TargetNodeId = "定制结构件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.StructBomEvalSelect_Dzjgj,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "结构BOM单价审核",
                    TargetNodeId = "结构BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.StructBomEvalSelect_Jgbomppxg,
                    LineType = LineType.Reset,

                },

                //电子BOM单价审核
                //new Line
                //{
                //    SoureNodeId = "电子BOM单价审核",
                //    TargetNodeId = "上传结构BOM",
                //    Index = 0,
                //    FinanceDictionaryDetailId = FinanceConsts.ElectronicBomEvalSelect_Scjgbom,
                //    LineType = LineType.Reset,

                //},

                new Line
                {
                    SoureNodeId = "电子BOM单价审核",
                    TargetNodeId = "电子BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.ElectronicBomEvalSelect_Dzbomppxg,
                    LineType = LineType.Reset,

                },

                //BOM成本审核
                //new Line
                //{
                //    SoureNodeId = "BOM成本审核",
                //    TargetNodeId = "上传结构BOM",
                //    Index = 0,
                //    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Scjgbom,
                //    LineType = LineType.Reset,

                //},

                new Line
                {
                    SoureNodeId = "BOM成本审核",
                    TargetNodeId = "定制结构件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Dzjgj,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "BOM成本审核",
                    TargetNodeId = "结构BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Jgbomppxg,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "BOM成本审核",
                    TargetNodeId = "电子BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Dzbomppxg,
                    LineType = LineType.Reset,

                },

                //new Line
                //{
                //    SoureNodeId = "BOM成本审核",
                //    TargetNodeId = "上传电子BOM",
                //    Index = 0,
                //    FinanceDictionaryDetailId = FinanceConsts.BomEvalSelect_Scdzbom,
                //    LineType = LineType.Reset,

                //},

                //不合规是否退回
                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "上传结构BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Scjgbom,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "定制结构件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Dzjgj,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "结构BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Jgbomppxg,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "电子BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Dzbomppxg,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "上传电子BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Scdzbom,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "物流成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Wlcblr,
                    LineType = LineType.Reset,

                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "工序工时添加",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Gsgxtj,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "不合规是否退回",
                    TargetNodeId = "COB制造成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.MybhgSelect_Cobzzcblr,
                    LineType = LineType.Reset,
                },


                //核价看板
                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "上传结构BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Scjgbom,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "定制结构件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Dzjgj,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "结构BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Jgbomppxg,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "电子BOM匹配修改",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Dzbomppxg,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "上传电子BOM",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Scdzbom,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "物流成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Wlcblr,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "工序工时添加",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Gsgxtj,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "COB制造成本录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Cobzzcblr,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "NRE模具费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Nremjflr,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "NRE_可靠性实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Nrekkxsyflr,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "NRE手板件",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Nresbj,
                    LineType = LineType.Reset,
                },

                new Line
                {
                    SoureNodeId = "核价看板",
                    TargetNodeId = "NRE_EMC实验费录入",
                    Index = 0,
                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Nreemcsyflr,
                    LineType = LineType.Reset,
                },
                #endregion
            };

            //统一定义Id和所属工作流Id
            nodeList.ForEach(p =>
            {
                p.Id = $"{MainFlowId}_{p.Name}";
                p.WorkFlowId = MainFlowId;
                //p.RoleId = roleId;
            });
            lineList.ForEach(p =>
            {
                p.WorkFlowId = MainFlowId;
                p.SoureNodeId = $"{MainFlowId}_{p.SoureNodeId}";
                p.TargetNodeId = $"{MainFlowId}_{p.TargetNodeId}";
                p.Id = $"{p.SoureNodeId}_{p.TargetNodeId}";
            });

            var isHasWorkFlow = _context.Workflow.Any(p => p.Id == workFlow.Id);
            if (!isHasWorkFlow)
            {
                _context.Workflow.AddRange(workFlow);
            }

            var noDb = nodeList.Where(p => !_context.Node.Contains(p));
            if (noDb.Any())
            {
                _context.Node.AddRange(noDb);
            }


            var noDb2 = lineList.Where(p => !_context.Line.Contains(p));
            if (noDb2.Any())
            {
                _context.Line.AddRange(noDb2);
            }



            #endregion
        }
    }
}
