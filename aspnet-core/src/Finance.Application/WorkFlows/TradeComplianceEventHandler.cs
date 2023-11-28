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
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.Processes;
using Abp.Authorization.Users;
using Finance.Authorization.Roles;

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
        private readonly IRepository<WorkflowInstance, long> _workflowInstanceRepository;
        private readonly AuditFlowAppService _auditFlowAppService;
        private readonly SendEmail _sendEmail;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role, int> _roleRepository;
        /// <summary>
        /// 物流成本服务
        /// </summary>
        private readonly LogisticscostAppService _logisticscostAppService;
        /// <summary>
        /// 工时工序服务  
        /// </summary>
        private readonly ProcessHoursEnterAppService _processHoursEnterAppService;
        /// <summary>
        /// COB制造成本服务
        /// </summary>
        private readonly BomEnterAppService _bomEnterAppService;


        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;


        public TradeComplianceEventHandler(TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService, IUnitOfWorkManager unitOfWorkManager, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService, ResourceEnteringAppService resourceEnteringAppService, PriceEvaluationGetAppService priceEvaluationGetAppService, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Gradient, long> gradientRepository, IRepository<Solution, long> solutionRepository, IRepository<PanelJson, long> panelJsonRepository, IRepository<PriceEvaluationStartData, long> priceEvaluationStartDataRepository, NrePricingAppService nrePricingAppService, IRepository<WorkflowInstance, long> workflowInstanceRepository, AuditFlowAppService auditFlowAppService, SendEmail sendEmail, IRepository<NoticeEmailInfo, long> noticeEmailInfoRepository, IRepository<User, long> userRepository, LogisticscostAppService logisticscostAppService, ProcessHoursEnterAppService processHoursEnterAppService, BomEnterAppService bomEnterAppService, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<UserRole, long> userRoleRepository, IRepository<Role, int> roleRepository)
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
            _workflowInstanceRepository = workflowInstanceRepository;
            _auditFlowAppService = auditFlowAppService;
            _sendEmail = sendEmail;
            _noticeEmailInfoRepository = noticeEmailInfoRepository;
            _userRepository = userRepository;
            _logisticscostAppService = logisticscostAppService;
            _processHoursEnterAppService = processHoursEnterAppService;
            _bomEnterAppService = bomEnterAppService;
            _priceEvaluationRepository = priceEvaluationRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
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
                    if (eventData.Entity.NodeInstanceStatus == NodeInstanceStatus.Current)
                    {
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
                        //var autoFlows = new List<string> { "主流程_系统生成报价审批表报价单" };//"主流程_生成报价分析界面选择报价方案",
                        //if (autoFlows.Contains(eventData.Entity.NodeId))
                        //{
                        //    await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        //    {
                        //        NodeInstanceId = eventData.Entity.Id,
                        //        FinanceDictionaryDetailId = FinanceConsts.Done,
                        //        Comment = "系统自动流转"
                        //    });
                        //}

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

                        //如果是流转到物流成本录入退回页面的
                        if (eventData.Entity.NodeId == "主流程_物流成本录入")
                        {
                            await _logisticscostAppService.DeleteAuditFlowIdAsync(eventData.Entity.WorkFlowInstanceId);
                        }

                        //如果是流转到工序工时添加退回页面的
                        if (eventData.Entity.NodeId == "主流程_工序工时添加")
                        {
                            await _processHoursEnterAppService.DeleteAuditFlowIdAsync(eventData.Entity.WorkFlowInstanceId);
                        }

                        //如果是流转到COB制造成本录入退回页面的
                        if (eventData.Entity.NodeId == "主流程_COB制造成本录入")
                        {
                            await _bomEnterAppService.DeleteAuditFlowIdAsync(eventData.Entity.WorkFlowInstanceId);
                        }
                        ////如果是流转到主流程_电子BOM匹配修改
                        //if (eventData.Entity.NodeId == "主流程_电子BOM匹配修改")
                        //{
                        //    await _resourceEnteringAppService.ElectronicBOMUnitPriceEliminate(eventData.Entity.WorkFlowInstanceId);
                        //}

                        ////如果是流转到主流程_结构BOM匹配修改
                        //if (eventData.Entity.NodeId == "主流程_结构BOM匹配修改")
                        //{
                        //    await _resourceEnteringAppService.StructureBOMUnitPriceEliminate(eventData.Entity.WorkFlowInstanceId);
                        //}

                        //如果是流转到主流程_核价看板
                        if (eventData.Entity.NodeId == "主流程_核价看板")
                        {
                            #region  流转到核价看板前判断贸易合规
                            try
                            {
                                var isOk = await _tradeComplianceAppService.IsProductsTradeComplianceOK(eventData.Entity.WorkFlowInstanceId);
                                if (isOk)
                                {
                                    await _panelJsonRepository.DeleteAsync(p => p.AuditFlowId == eventData.Entity.WorkFlowInstanceId);
                                }
                                else
                                {
                                    await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                                    {
                                        NodeInstanceId = eventData.Entity.Id,
                                        FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Bhg,
                                        Comment = "系统判断不合规"
                                    });
                                }
                            }
                            catch (Exception)
                            {
                                await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                                {
                                    NodeInstanceId = eventData.Entity.Id,
                                    FinanceDictionaryDetailId = FinanceConsts.HjkbSelect_Bhg,
                                    Comment = "贸易合规判断异常"
                                });
                            }

                            #endregion


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

                        if (eventData.Entity.NodeId == "主流程_NRE_可靠性实验费录入")
                        {
                            await _nrePricingAppService.GetExperimentItemsConfigurationState(eventData.Entity.WorkFlowInstanceId);
                        }

                        if (eventData.Entity.NodeId == "主流程_核心器件成本NRE费用拆分")
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.Done,
                                Comment = "系统自动流转：核心器件成本NRE费用拆分"
                            });
                        }

                        //如果流转到报价看板
                        if (eventData.Entity.NodeId == "主流程_生成报价分析界面选择报价方案")
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.Done,
                                Comment = "系统自动流转"
                            });
                        }

                        //如果流转到查看每个方案初版BOM成本
                        if (eventData.Entity.NodeId == "主流程_查看每个方案初版BOM成本")
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.Done,
                                Comment = "系统自动流转"
                            });
                        }

                        //如果流转到项目部长查看核价表
                        if (eventData.Entity.NodeId == "主流程_项目部长查看核价表")
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.Done,
                                Comment = "系统自动流转"
                            });
                        }

                        //如果流转到总经理查看中标金额
                        if (eventData.Entity.NodeId == "主流程_总经理查看中标金额")
                        {
                            await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                            {
                                NodeInstanceId = eventData.Entity.Id,
                                FinanceDictionaryDetailId = FinanceConsts.Done,
                                Comment = "系统自动流转"
                            });
                        }

                        if (eventData.Entity.NodeId == "主流程_归档")
                        {
                            var wf = await _workflowInstanceRepository.GetAsync(eventData.Entity.WorkFlowInstanceId);
                            wf.WorkflowState = WorkflowState.Ended;


                            //发邮件给拥有这个流程的项目经理
                            #region 邮件发送

                            //#if !DEBUG
                            SendEmail email = new SendEmail();
                            string loginIp = email.GetLoginAddr();
                            var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();

                            //if (loginIp.Equals(FinanceConsts.AliServer_In_IP))
                            //{
                            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == eventData.Entity.WorkFlowInstanceId);
                            var role = await _roleRepository.GetAllListAsync(p =>
                            p.Name == StaticRoleNames.Host.FinanceTableAdmin || p.Name == StaticRoleNames.Host.EvalTableAdmin
                    || p.Name == StaticRoleNames.Host.Bjdgdgly);
                            var userIds = await _userRoleRepository.GetAll().Where(p => role.Select(p => p.Id).Contains(p.RoleId)).Select(p => p.UserId).ToListAsync();

                            if (priceEvaluation != null)
                            {
                                userIds.Add(priceEvaluation.ProjectManager);
                                if (priceEvaluation.CreatorUserId.HasValue 
                                    && priceEvaluation.CreatorUserId != priceEvaluation.ProjectManager)
                                {
                                    userIds.Add(priceEvaluation.CreatorUserId.Value);
                                }
                            }
                            userIds = userIds.Distinct().ToList();
                            foreach (var userId in userIds)
                            {
                                var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);

                                if (userInfo != null)
                                {
                                    string emailAddr = userInfo.EmailAddress;
                                    string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                                    string emailBody = "核价报价提醒：您有新的工作流（" + eventData.Entity.Name + "——流程号：" + eventData.Entity.WorkFlowInstanceId + "）需要完成（" + "<a href=\"" + loginAddr + "\" >系统地址</a>" + "）";

                                    try
                                    {
                                        if (!emailAddr.Contains("@qq.com"))
                                        {
                                            await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{eventData.Entity.Name},流程号{eventData.Entity.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }


                                    #region 嵌套


                                    //#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                                    //                                    Task.Run(async () =>
                                    //                                    {

                                    //                                    });
                                    //#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                                    #endregion
                                }

                            }
                            //}
                            //#endif
                            #endregion
                        }
                        else
                        {



                            #region 邮件发送

                            //#if !DEBUG
                            SendEmail email = new SendEmail();
                            string loginIp = email.GetLoginAddr();

                            //if (loginIp.Equals(FinanceConsts.AliServer_In_IP))
                            //{

                            var allAuditFlowInfos = await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(eventData.Entity.WorkFlowInstanceId, eventData.Entity.Id);
                            foreach (var task in allAuditFlowInfos)
                            {
                                foreach (var userId in task.TaskUserIds)
                                {
                                    var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);
                                    //var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == 272);//测试 ，只发给陈梦瑶

                                    if (userInfo != null)
                                    {
                                        string emailAddr = userInfo.EmailAddress;

                                        var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();

                                        string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                                        string emailBody = "核价报价提醒：您有新的工作流（" + task.NodeName + "——流程号：" + task.WorkFlowInstanceId + "）需要完成（" + "<a href=\"" + loginAddr + "\" >系统地址</a>" + "）";
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                                        Task.Run(async () =>
                                        {
                                            await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{task.NodeName},流程号{task.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                                        });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                                    }
                                }
                            }
                            //}

                            //#endif
                            #endregion
                        }

                    }
                    else if (eventData.Entity.NodeInstanceStatus == NodeInstanceStatus.Passed)
                    {
                        //如果是流转到主流程_电子BOM单价审核
                        if (eventData.Entity.NodeId == "主流程_电子BOM匹配修改")
                        {
                            await _resourceEnteringAppService.ElectronicBOMUnitPriceCopying(eventData.Entity.WorkFlowInstanceId);
                        }

                        //如果是流转到主流程_结构BOM单价审核
                        if (eventData.Entity.NodeId == "主流程_结构BOM匹配修改")
                        {
                            await _resourceEnteringAppService.StructureBOMUnitPriceCopying(eventData.Entity.WorkFlowInstanceId);
                        }
                    }
                }

                uow.Complete();
            }
        }
    }
}
