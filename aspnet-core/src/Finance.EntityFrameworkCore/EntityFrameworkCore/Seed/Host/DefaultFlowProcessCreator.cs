using Finance.Audit;
using Finance.Authorization.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultFlowProcessCreator
    {
        private readonly FinanceDbContext _context;

        public DefaultFlowProcessCreator(FinanceDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            CreateEditions();
        }
        private void CreateEditions()
        {
            AddFlowProcessType();
            AddFlowJumpInfo();
            AddFlowClearInfo();
            SaveNoticeEmailInfo();
            _context.SaveChanges();
        }

        private void SaveNoticeEmailInfo()
        {
            NoticeEmailInfo noticeEmailInfos = _context.NoticeEmailInfo.FirstOrDefault(b=>!b.IsDeleted);
            if (noticeEmailInfos is null)
            {
                _context.NoticeEmailInfo.Add(new NoticeEmailInfo { EmailAddress = FinanceConsts.MailFrom_Sunny, EmailPassword = FinanceConsts.MailUserPassword_Sunny, MaintainerEmail = FinanceConsts.MailForMaintainer });
                _context.SaveChanges();
            }
           
        }
        private void AddFlowProcessType()
        {
            List<FlowProcess> processList = new()
            {
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_RequirementInput, ProcessName = "核价需求录入界面", EditRole = StaticRoleNames.Host.SalesMan, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_PMInput, ProcessName = "项目经理录入界面", EditRole = StaticRoleNames.Host.ProjectManager },
                //new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT, ProcessName = "市场部TR主方案审核界面", EditRole = StaticRoleNames.Host.MarketTRAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, ProcessName = "产品开发部TR主方案审核", EditRole = StaticRoleNames.Host.R_D_TRAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },

                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ProcessName = "结构bom导入界面", EditRole = StaticRoleNames.Host.StructuralEngineer },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, ProcessName = "结构bom审核界面", EditRole = StaticRoleNames.Host.StructuralBomAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ProcessName = "电子bom导入界面", EditRole = StaticRoleNames.Host.ElectronicsEngineer },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit, ProcessName = "电子bom审核界面", EditRole = StaticRoleNames.Host.ElectronicsBomAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ProcessName = "结构单价录入界面", EditRole = StaticRoleNames.Host.StructuralPriceInputter },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ProcessName = "电子单价录入界面", EditRole = StaticRoleNames.Host.ElectronicsPriceInputter },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ProcessName = "电子bom单价审核界面", EditRole = StaticRoleNames.Host.ElectronicsPriceAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ProcessName = "结构bom单价审核界面", EditRole = StaticRoleNames.Host.StructuralPriceAuditor, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                //new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ProcessName = "工程电子损耗率录入界面", EditRole = StaticRoleNames.Host.LossRateInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                //new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ProcessName = "工程结构损耗率录入界面", EditRole = StaticRoleNames.Host.LossRateInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ProcessName = "工序工时导入界面", EditRole = StaticRoleNames.Host.ManHourInputter , ReadonlyRole = StaticRoleNames.Host.ProjectManager},
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput, ProcessName = "制造成本录入界面", EditRole = StaticRoleNames.Host.FinanceProductCostInputter , ReadonlyRole = StaticRoleNames.Host.ProjectManager},
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ProcessName = "物流成本录入界面", EditRole = StaticRoleNames.Host.LogisticsCostInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ProcessName = "NRE核价手板件、差旅及其他费用录入界面", EditRole = StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ProcessName = "NRE核价电子-EMC实验费录入界面", EditRole = StaticRoleNames.Host.ElectronicsEngineer, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ProcessName = "NRE模具清单录入界面", EditRole =  StaticRoleNames.Host.StructuralPriceInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                //new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ProcessName = "NRE实验费用录入界面", EditRole = StaticRoleNames.Host.TestCostInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                //new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ProcessName = "NRE检具费用录入界面", EditRole = StaticRoleNames.Host.GageCostInputter, ReadonlyRole =  StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_TradeApproval, ProcessName = "贸易合规审核（结束流程&退回流程）", EditRole = StaticRoleNames.Host.TradeComplianceAuditor },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ProcessName = "核价看板&流程退回界面", EditRole = StaticRoleNames.Host.ProjectManager },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, ProcessName = "项目部核价审核界面", EditRole = StaticRoleNames.Host.ProjectPriceAuditor },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, ProcessName = "财务部核价审核界面", EditRole = StaticRoleNames.Host.FinancePriceAuditor },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor, ProcessName = "成本信息表下载&填报NRE报价系数&产品报价看板界面", EditRole = StaticRoleNames.Host.SalesMan },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_QuoteApproval, ProcessName = "总经理报价审批界面", EditRole = StaticRoleNames.Host.GeneralManager, ReadonlyRole = StaticRoleNames.Host.FinanceProductCostInputter },
                new FlowProcess { ProcessIdentifier = AuditFlowConsts.AF_ArchiveEnd, ProcessName = "归档下载界面", EditRole =  StaticRoleNames.Host.SalesMan, ReadonlyRole = StaticRoleNames.Host.ProjectManager  },
            
            };

            foreach (var list in processList)
            {
                AddProcessTypeIfNotExists(list);
            }
        }

        private void AddProcessTypeIfNotExists(FlowProcess processType)
        {
            if (_context.FlowProcessType.IgnoreQueryFilters().Any(l => l.ProcessName == processType.ProcessName))
            {
                return;
            }

            _context.FlowProcessType.Add(processType);
            _context.SaveChanges();
        }

        private void AddFlowJumpInfo()
        {
            List<FlowJumpInfo> flowJumpInfoList = new()
            {
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_RequirementInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_PMInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_PMInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_PMInput },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_PMInput },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_StructBomImport },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomImport, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_StructBomAudit },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_StructPriceInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ManHourImport },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_StructBomImport },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_StructPriceInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ManHourImport, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_NreInputOther },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_NreInputEmc },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_NreInputTest },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_NreInputGage },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_StructBomAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_NreInputMould },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_NreInputOther, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_NreInputMould, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_NreInputTest, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_NreInputGage, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_TradeApproval },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TradeApproval, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_TradeApproval, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_ArchiveEnd },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_ArchiveEnd },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor, Condition = OPINIONTYPE.Reject, NextProcessIdentifier =AuditFlowConsts.AF_ArchiveEnd },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_QuoteApproval, Condition = OPINIONTYPE.Submit_Agreee, NextProcessIdentifier = AuditFlowConsts.AF_ArchiveEnd },
                new FlowJumpInfo { PreviousProcessIdentifier = AuditFlowConsts.AF_QuoteApproval, Condition = OPINIONTYPE.Reject, NextProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },

            };
            foreach (var list in flowJumpInfoList)
            {
                AddFlowJumpInfoIfNotExists(list);
            }
        }
        private void AddFlowJumpInfoIfNotExists(FlowJumpInfo flowJumpInfo)
        {
            if (_context.FlowJumpInfo.IgnoreQueryFilters().Any(l => l.PreviousProcessIdentifier == flowJumpInfo.PreviousProcessIdentifier && l.Condition == flowJumpInfo.Condition && l.NextProcessIdentifier == flowJumpInfo.NextProcessIdentifier))
            {
                return;
            }

            _context.FlowJumpInfo.Add(flowJumpInfo);
            _context.SaveChanges();
        }

        private void AddFlowClearInfo()
        {
            List<FlowClearInfo> flowClearInfoList = new()
            {
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PMInput, ClearProcessIdentifier = AuditFlowConsts.AF_PMInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PMInput, ClearProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PMInput, ClearProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_StructBomImport },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_StructBomAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_StructPriceInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ManHourImport },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputMould },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomImport, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputEmc },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputEmc, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputTest },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputTest, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputGage },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputGage, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputMould },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputMould, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_NreInputOther },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_NreInputOther, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_StructPriceInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructPriceInput, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructLossRateInput, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_ManHourImport },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ManHourImport, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },


                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },

                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_TradeApproval },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor },
                new FlowClearInfo { CurrentProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit, ClearProcessIdentifier = AuditFlowConsts.AF_QuoteApproval },
            };
            foreach (var list in flowClearInfoList)
            {
                AddFlowClearInfoIfNotExists(list);
            }
        }
        private void AddFlowClearInfoIfNotExists(FlowClearInfo flowClearInfo)
        {
            if (_context.FlowClearInfo.IgnoreQueryFilters().Any(l => l.CurrentProcessIdentifier == flowClearInfo.CurrentProcessIdentifier && l.ClearProcessIdentifier == flowClearInfo.ClearProcessIdentifier))
            {
                return;
            }

            _context.FlowClearInfo.Add(flowClearInfo);
            _context.SaveChanges();
        }
    }
}
