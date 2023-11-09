using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Audit;
using Finance.TradeCompliance;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.ProductDevelopment;
using Finance.Entering;
using Finance.PriceEval;
using Finance.DemandApplyAudit;
using Finance.PriceEval.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Json;
using Finance.NerPricing;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 系统判断贸易贸易合规等其他需要自动流转的流程并对接工作流
    /// </summary>
    public class TradeComplianceEventHandler : IEventHandler<EntityUpdatedEventData<NodeInstance>>, ITransientDependency
    {

        private readonly TradeComplianceAppService _tradeComplianceAppService;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ElectronicBomAppService _electronicBomAppService;
        private readonly StructionBomAppService _structionBomAppService;
        private readonly ResourceEnteringAppService _resourceEnteringAppService;
        private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<PanelJson, long> _panelJsonRepository;
        private readonly IRepository<PriceEvaluationStartData, long> _priceEvaluationStartDataRepository;
        private readonly NrePricingAppService _nrePricingAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeComplianceAppService"></param>
        /// <param name="workflowInstanceAppService"></param>
        /// <param name="unitOfWorkManager"></param>
        /// <param name="electronicBomAppService"></param>
        /// <param name="structionBomAppService"></param>
        /// <param name="resourceEnteringAppService"></param>
        /// <param name="priceEvaluationGetAppService"></param>
        /// <param name="modelCountYearRepository"></param>
        /// <param name="gradientRepository"></param>
        /// <param name="solutionRepository"></param>
        /// <param name="panelJsonRepository"></param>
        /// <param name="priceEvaluationStartDataRepository"></param>
        /// <param name="nrePricingAppService"></param>
        public TradeComplianceEventHandler(TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService, IUnitOfWorkManager unitOfWorkManager, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService, ResourceEnteringAppService resourceEnteringAppService, PriceEvaluationGetAppService priceEvaluationGetAppService, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Gradient, long> gradientRepository, IRepository<Solution, long> solutionRepository, IRepository<PanelJson, long> panelJsonRepository, IRepository<PriceEvaluationStartData, long> priceEvaluationStartDataRepository, NrePricingAppService nrePricingAppService)
        {
            _tradeComplianceAppService = tradeComplianceAppService;
            _workflowInstanceAppService = workflowInstanceAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _electronicBomAppService = electronicBomAppService;
            _structionBomAppService = structionBomAppService;
            _resourceEnteringAppService = resourceEnteringAppService;
            _priceEvaluationGetAppService = priceEvaluationGetAppService;
            _modelCountYearRepository = modelCountYearRepository;
            _gradientRepository = gradientRepository;
            _solutionRepository = solutionRepository;
            _panelJsonRepository = panelJsonRepository;
            _priceEvaluationStartDataRepository = priceEvaluationStartDataRepository;
            _nrePricingAppService = nrePricingAppService;
        }

        /// <summary>
        /// 贸易合规等节点被激活时触发
        /// </summary>
        /// <param name="eventData"></param>
        public async void HandleEvent(EntityUpdatedEventData<NodeInstance> eventData)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    //必须被激活才能执行
                    if (eventData.Entity.NodeInstanceStatus != NodeInstanceStatus.Current)
                    {
                        return;
                    }
                    if (eventData.Entity.NodeId == "主流程_贸易合规")
                    {
                        try
                        {

                            var isOk = await _tradeComplianceAppService.IsProductsTradeComplianceOK(eventData.Entity.WorkFlowInstanceId);
                            if (isOk)
                            {
                                await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                                {
                                    NodeInstanceId = eventData.Entity.Id,
                                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                                    Comment = "系统判断合规"
                                });
                            }
                            else
                            {
                                await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                                {
                                    NodeInstanceId = eventData.Entity.Id,
                                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                                    Comment = "系统判断不合规"
                                });
                            }
                        }
                        catch (Exception)
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                                Comment = "系统判断不合规"
                            });
                        }
                    }

                    //如果是自动生成相关的流转页面
                    var autoFlows = new List<string> { "主流程_系统生成报价审批表报价单" };//"主流程_生成报价分析界面选择报价方案",
                    if (autoFlows.Contains(eventData.Entity.NodeId))
                    {
                        await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        {
                            NodeInstanceId = eventData.Entity.Id,
                            FinanceDictionaryDetailId = FinanceConsts.Done,
                            Comment = "系统自动流转"
                        });
                    }

                    //如果是流转到电子BOM退回页面的
                    if (eventData.Entity.NodeId == "主流程_上传电子BOM")
                    {
                        await _electronicBomAppService.ClearElecBomImportState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到结构BOM退回页面的
                    if (eventData.Entity.NodeId == "主流程_上传结构BOM")
                    {
                        await _structionBomAppService.ClearStructBomImportState(eventData.Entity.WorkFlowInstanceId);
                    }


                    //如果是流转到主流程_电子BOM匹配修改
                    if (eventData.Entity.NodeId == "主流程_电子BOM匹配修改")
                    {
                        await _resourceEnteringAppService.ElectronicBOMUnitPriceEliminate(eventData.Entity.WorkFlowInstanceId);

                        //await _resourceEnteringAppService.GetElectronicConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_结构BOM匹配修改
                    if (eventData.Entity.NodeId == "主流程_结构BOM匹配修改")
                    {
                        await _resourceEnteringAppService.StructureBOMUnitPriceEliminate(eventData.Entity.WorkFlowInstanceId);


                        //await _resourceEnteringAppService.GetStructuralConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_核价看板
                    if (eventData.Entity.NodeId == "主流程_核价看板")
                    {
                        await _resourceEnteringAppService.ElectronicBOMUnitPriceCopying(eventData.Entity.WorkFlowInstanceId);
                        await _resourceEnteringAppService.StructureBOMUnitPriceCopying(eventData.Entity.WorkFlowInstanceId);

                        await _panelJsonRepository.DeleteAsync(p => p.AuditFlowId == eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果流转到核价看板之后，就缓存核价看板的全部信息
                    if (eventData.Entity.NodeId == "主流程_项目部课长审核")
                    {
                        var data = from g in _gradientRepository.GetAll()
                                   from s in _solutionRepository.GetAll()
                                   from y in _modelCountYearRepository.GetAll()
                                   where g.AuditFlowId == eventData.Entity.WorkFlowInstanceId
                                   && s.AuditFlowId == eventData.Entity.WorkFlowInstanceId
                                   && y.AuditFlowId == eventData.Entity.WorkFlowInstanceId
                                   select new GetPriceEvaluationTableInput
                                   {
                                       AuditFlowId = eventData.Entity.WorkFlowInstanceId,
                                       GradientId = g.Id,
                                       InputCount = 0,
                                       SolutionId = s.Id,
                                       Year = y.Year,
                                       UpDown = y.UpDown
                                   };
                        var result = await data.ToListAsync();
                        var all = result.GroupBy(p => new { p.AuditFlowId, p.GradientId, p.InputCount, p.SolutionId, })
                            .Select(p => new GetPriceEvaluationTableInput
                            {
                                AuditFlowId = p.Key.AuditFlowId,
                                GradientId = p.Key.GradientId,
                                InputCount = p.Key.InputCount,
                                SolutionId = p.Key.SolutionId,
                                Year = PriceEvalConsts.AllYear,
                                UpDown = YearType.Year
                            }).Distinct();
                        result.AddRange(all);
                        foreach (var item in result)
                        {
                            var priceEvaluationTable = await _priceEvaluationGetAppService.GetPriceEvaluationTable(item);
                            await _panelJsonRepository.InsertAsync(new PanelJson
                            {
                                AuditFlowId = item.AuditFlowId,
                                GradientId = item.GradientId,
                                InputCount = item.InputCount,
                                SolutionId = item.SolutionId,
                                Year = item.Year,
                                UpDown = item.UpDown,
                                DataJson = priceEvaluationTable.ToJsonString()
                            });
                        }
                    }

                    //到核价审批录入后，要清空核价需求录入的缓存
                    if (eventData.Entity.NodeId == "主流程_核价审批录入")
                    {
                        await _priceEvaluationStartDataRepository.DeleteAsync(p => p.AuditFlowId == eventData.Entity.WorkFlowInstanceId);
                    }

                    //NRE手板件清空
                    if (eventData.Entity.NodeId == "主流程_NRE手板件")
                    {
                        await _nrePricingAppService.GetProjectManagementConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    if (eventData.Entity.NodeId == "主流程_NRE_EMC实验费录入")
                    {
                        await _nrePricingAppService.GetProductDepartmentConfigurationState(eventData.Entity.WorkFlowInstanceId);

                    }
                }

                uow.Complete();
            }
        }
    }
}
