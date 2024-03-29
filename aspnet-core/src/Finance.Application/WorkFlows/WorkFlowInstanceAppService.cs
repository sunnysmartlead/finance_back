﻿using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DynamicExpresso;
using Finance.Audit;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.Ext;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.Infrastructure.Dto;
using Finance.Job;
using Finance.Nre;
using Finance.PriceEval;
using Finance.WorkFlows.Dto;
//using Interface.Expends;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 工作流程实例服务
    /// </summary>
    public class WorkflowInstanceAppService : FinanceAppServiceBase
    {
        private readonly IRepository<Workflow, string> _workflowRepository;
        private readonly IRepository<Node, string> _nodeRepository;
        private readonly IRepository<Line, string> _lineRepository;

        private readonly IRepository<WorkflowInstance, long> _workflowInstanceRepository;
        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;
        private readonly IRepository<LineInstance, long> _lineInstanceRepository;

        private readonly IRepository<InstanceHistory, long> _instanceHistoryRepository;

        private readonly IRepository<FinanceDictionary, string> _financeDictionaryRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;

        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;


        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

        private readonly IRepository<TaskReset, long> _taskResetRepository;

        private readonly IRepository<Solution, long> _solutionRepository;

        private readonly IRepository<PricingTeam, long> _pricingTeamRepository;

        private readonly IRepository<PriceEvaluationStartData, long> _priceEvaluationStartDataRepository;
        private readonly IRepository<Fu_Bom, long> _fu_BomRepository;

        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<AuditFlowIdPricingForm, long> _auditFlowIdPricingForm;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<Department, long> _departmentRepository;
        private readonly IRepository<NodeTime, long> _nodeTimeRepository;
        private readonly RuleAppService _ruleAppService;

        public WorkflowInstanceAppService(IRepository<Workflow, string> workflowRepository, IRepository<Node, string> nodeRepository, IRepository<Line, string> lineRepository, IRepository<WorkflowInstance, long> workflowInstanceRepository, IRepository<NodeInstance, long> nodeInstanceRepository, IRepository<LineInstance, long> lineInstanceRepository, IRepository<InstanceHistory, long> instanceHistoryRepository, IRepository<FinanceDictionary, string> financeDictionaryRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<UserRole, long> userRoleRepository, IRepository<Role> roleRepository, UserManager userManager, RoleManager roleManager, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<TaskReset, long> taskResetRepository, IRepository<Solution, long> solutionRepository, IRepository<PricingTeam, long> pricingTeamRepository, IRepository<PriceEvaluationStartData, long> priceEvaluationStartDataRepository, IRepository<Fu_Bom, long> fu_BomRepository, IRepository<Gradient, long> gradientRepository, IRepository<AuditFlowIdPricingForm, long> auditFlowIdPricingForm, IBackgroundJobManager backgroundJobManager, IRepository<Department, long> departmentRepository, IRepository<NodeTime, long> nodeTimeRepository, RuleAppService ruleAppService)
        {
            _workflowRepository = workflowRepository;
            _nodeRepository = nodeRepository;
            _lineRepository = lineRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _nodeInstanceRepository = nodeInstanceRepository;
            _lineInstanceRepository = lineInstanceRepository;
            _instanceHistoryRepository = instanceHistoryRepository;
            _financeDictionaryRepository = financeDictionaryRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _priceEvaluationRepository = priceEvaluationRepository;
            _taskResetRepository = taskResetRepository;
            _solutionRepository = solutionRepository;
            _pricingTeamRepository = pricingTeamRepository;
            _priceEvaluationStartDataRepository = priceEvaluationStartDataRepository;
            _fu_BomRepository = fu_BomRepository;
            _gradientRepository = gradientRepository;
            _auditFlowIdPricingForm = auditFlowIdPricingForm;
            _backgroundJobManager = backgroundJobManager;
            _departmentRepository = departmentRepository;
            _nodeTimeRepository = nodeTimeRepository;
            _ruleAppService = ruleAppService;
        }

        /// <summary>
        /// 手动触发贸易合规（手动测试使用时改为public）
        /// </summary>
        /// <returns></returns>
        public async Task GG(long id)
        {
            //var hg = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == id && p.NodeId == "主流程_贸易合规");
            //hg.LastModificationTime = DateTime.UtcNow;
            //hg.NodeInstanceStatus = NodeInstanceStatus.Current;

            var fg = await _nodeInstanceRepository.GetAll().Where(p => p.WorkFlowInstanceId == id && p.NodeId == "主流程_贸易合规")
                .UpdateFromQueryAsync(p => new NodeInstance { LastModificationTime = DateTime.Now });

        }

        /// <summary>
        /// 刷新退回线
        /// </summary>
        /// <returns></returns>
        public async Task ShuaTuiHui()
        {
            var data = await (from l in _lineRepository.GetAll()
                              join i in _lineInstanceRepository.GetAll() on l.Id equals i.LineId
                              where l.LineType != i.LineType
                              select new
                              {
                                  i.Id,
                                  l.LineType
                              }).ToListAsync();

            foreach (var item in data)
            {
                var l = await _lineInstanceRepository.GetAsync(item.Id);
                l.LineType = item.LineType;
            }
        }

        /// <summary>
        /// 刷新退回线
        /// </summary>
        /// <returns></returns>
        public async Task ShuaNode()
        {
            var data = await (from n in _nodeRepository.GetAll()
                              join i in _nodeInstanceRepository.GetAll() on n.Id equals i.NodeId
                              where n.RoleId != i.RoleId
                              select new
                              {
                                  i.Id,
                                  n.RoleId
                              }).ToListAsync();

            foreach (var item in data)
            {
                var l = await _nodeInstanceRepository.GetAsync(item.Id);
                l.RoleId = item.RoleId;
            }
        }

        /// <summary>
        /// 刷新线的激活条件
        /// </summary>
        /// <returns></returns>
        public async Task ShuaLineJiHuo()
        {
            var data = await (from l in _lineRepository.GetAll()
                              join i in _lineInstanceRepository.GetAll() on l.Id equals i.LineId
                              where l.FinanceDictionaryDetailId != i.FinanceDictionaryDetailId
                              select new
                              {
                                  i.Id,
                                  l.FinanceDictionaryDetailId
                              }).ToListAsync();

            foreach (var item in data)
            {
                var l = await _lineInstanceRepository.GetAsync(item.Id);
                l.FinanceDictionaryDetailId = item.FinanceDictionaryDetailId;
            }
        }

        private async Task TestLine(long id, long id2)
        {
            var lineInstance = await _lineInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == 501);

            var route = await GetNodeRoute(id, id2);
            if (route.Any())
            {
                var lines = route.Select(p => p.Zip(p.Skip(1), (a, b) => lineInstance.FirstOrDefault(o => o.SoureNodeId == a.NodeId && o.TargetNodeId == b.NodeId)))
                    .SelectMany(p => p).Where(p => p != null).DistinctBy(p => p.Id).ToList();
            }
        }

        /// <summary>
        /// 开启一个工作流实例
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<long> StartWorkflowInstance(StartWorkflowInstanceInput input)
        {
            #region 获取工作流结构

            var workflow = await _workflowRepository.FirstOrDefaultAsync(p => p.Id == input.WorkflowId);
            if (workflow == null) { return 0; }

            var nodes = await _nodeRepository.GetAllListAsync(p => p.WorkFlowId == input.WorkflowId);
            var lines = await _lineRepository.GetAllListAsync(p => p.WorkFlowId == input.WorkflowId);

            #endregion

            #region 创建工作流实例

            //主表
            var workFlowInstanceId = await _workflowInstanceRepository.InsertAndGetIdAsync(new WorkflowInstance
            {
                WorkFlowId = workflow.Id,
                Name = workflow.Name,
                Version = workflow.Version,
                Title = input.Title
            });

            //节点和线
            var nodeInstanceList = ObjectMapper.Map<List<NodeInstance>>(nodes);
            var lineInstanceList = ObjectMapper.Map<List<LineInstance>>(lines);

            //关联主表
            nodeInstanceList.ForEach(p => p.WorkFlowInstanceId = workFlowInstanceId);
            lineInstanceList.ForEach(p => p.WorkFlowInstanceId = workFlowInstanceId);



            #endregion

            #region 定义开始后的状态

            //寻找系统开始节点
            var startNode = nodeInstanceList.First(p => p.Activation.IsNullOrWhiteSpace());//激活条件为空的是开始节点
            startNode.NodeInstanceStatus = NodeInstanceStatus.Passed;



            //寻找系统开始导出的线
            var startLine = lineInstanceList.First(p => p.SoureNodeId == startNode.NodeId);
            startLine.NodeInstanceStatus = NodeInstanceStatus.Passed;

            //寻找业务开始节点Id
            var businessStartNodeId = lineInstanceList.First(p => p.SoureNodeId == startNode.NodeId).TargetNodeId;

            //改变业务开始节点的数据
            var businessStartNode = nodeInstanceList.First(p => p.NodeId == businessStartNodeId);
            businessStartNode.NodeInstanceStatus = NodeInstanceStatus.Current;
            businessStartNode.FinanceDictionaryDetailId = input.FinanceDictionaryDetailId;

            //把数据定义进业务开始节点

            //寻找业务开始节点导出的线。只查找被激活的
            var business2Lines = lineInstanceList
                .Where(p => p.SoureNodeId == businessStartNodeId)
                .Where(p => p.FinanceDictionaryDetailId.StrToList().Contains(input.FinanceDictionaryDetailId));

            //改变这些被激活的线的状态
            foreach (var item in business2Lines)
            {
                item.NodeInstanceStatus = NodeInstanceStatus.Current;
                item.IsCurrent = true;
            }

            //寻找业务开始节点被激活的线连接的节点
            var business2Node = nodeInstanceList.Where(p => business2Lines.Select(o => o.TargetNodeId).Contains(p.NodeId));


            //判断这些节点是否被激活
            foreach (var item in business2Node)
            {
                //先获取目标为它们的线
                var targetBusiness2NodeLines = lineInstanceList.Where(p => p.TargetNodeId == item.NodeId);

                //将线转条件数组
                var parameters = targetBusiness2NodeLines.Select(p => new DynamicExpresso.Parameter($"{p.LineId}", typeof(bool), p.IsCurrent)).ToArray();


                //执行条件
                var target = new Interpreter();
                var result = target.Eval<bool>(item.Activation, parameters);

                //如果节点被激活
                if (result)
                {
                    businessStartNode.NodeInstanceStatus = NodeInstanceStatus.Passed;

                    item.NodeInstanceStatus = NodeInstanceStatus.Current;

                    foreach (var line in business2Lines.Intersect(targetBusiness2NodeLines))
                    {
                        line.NodeInstanceStatus = NodeInstanceStatus.Passed;
                        line.IsCurrent = false;
                    }
                }
            }


            #endregion

            #region 把节点和线保存到数据库
            await _nodeInstanceRepository.BulkInsertAsync(nodeInstanceList);
            await _lineInstanceRepository.BulkInsertAsync(lineInstanceList);
            #endregion

            #region 增加历史

            //给系统开始节点增加历史记录
            await _instanceHistoryRepository.InsertAsync(new InstanceHistory
            {
                WorkFlowId = startNode.WorkFlowId,
                WorkFlowInstanceId = workFlowInstanceId,
                NodeId = startNode.NodeId,
                NodeInstanceId = startNode.Id
            });

            //给业务开始节点增加历史记录
            await _instanceHistoryRepository.InsertAsync(new InstanceHistory
            {
                WorkFlowId = businessStartNode.WorkFlowId,
                WorkFlowInstanceId = workFlowInstanceId,
                NodeId = businessStartNode.NodeId,
                FinanceDictionaryDetailId = businessStartNode.FinanceDictionaryDetailId,
                NodeInstanceId = businessStartNode.Id
            });

            #endregion

            #region 增加开始和更新时间

            if (businessStartNode.NodeInstanceStatus == NodeInstanceStatus.Current)
            {
                await _nodeTimeRepository.InsertAsync(new NodeTime
                {
                    NodeInstance = businessStartNode.Id,
                    StartTime = DateTime.Now,
                    UpdateTime = null,
                    WorkFlowInstanceId = businessStartNode.WorkFlowInstanceId,
                });
            }
            else
            {
                await _nodeTimeRepository.InsertAsync(new NodeTime
                {
                    NodeInstance = businessStartNode.Id,
                    StartTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    WorkFlowInstanceId = businessStartNode.WorkFlowInstanceId,
                });
            }
            var evalInput = nodeInstanceList.Where(p => p.Id != businessStartNode.Id && p.NodeInstanceStatus == NodeInstanceStatus.Current);
            foreach (var item in evalInput)
            {
                await _nodeTimeRepository.InsertAsync(new NodeTime
                {
                    NodeInstance = item.Id,
                    StartTime = DateTime.Now,
                    UpdateTime = null,
                    WorkFlowInstanceId = item.WorkFlowInstanceId,
                });
            }

            #endregion

            return workFlowInstanceId;
        }

        /// <summary>
        /// 流程节点提交（结束当前节点，开启下个节点）
        /// </summary>
        /// <returns></returns>
        public async virtual Task SubmitNode(SubmitNodeInput input)
        {
            await SubmitNodeInterfece(input);
        }

        /// <summary>
        /// 流程节点提交（结束当前节点，开启下个节点）
        /// </summary>
        /// <returns></returns>
        internal async virtual Task SubmitNodeInterfece(ISubmitNodeInput input, bool isCheck = true)
        {
            //退回意见必填校验
            var fd = new List<string> {
                FinanceConsts.YesOrNo,
                FinanceConsts.EvalFeedback,
                FinanceConsts.StructBomEvalSelect,
                FinanceConsts.BomEvalSelect,
                FinanceConsts.MybhgSelect,
                FinanceConsts.HjkbSelect,
                FinanceConsts.ElectronicBomEvalSelect,
                FinanceConsts.Spbjclyhjb,
            };
            var list = await _financeDictionaryDetailRepository.GetAll().Where(p => fd.Contains(p.FinanceDictionaryId)).Select(p => p.Id).ToListAsync();

            var yes = new List<string> { FinanceConsts.YesOrNo_Yes,
                FinanceConsts.EvalFeedback_Js,
                FinanceConsts.StructBomEvalSelect_Yes,
                FinanceConsts.BomEvalSelect_Yes,
                FinanceConsts.MybhgSelect_No,
                FinanceConsts.HjkbSelect_Yes,
                FinanceConsts.ElectronicBomEvalSelect_Yes,
                FinanceConsts.Spbjclyhjb_Yes,
                FinanceConsts.Save,
                FinanceConsts.Save,
                FinanceConsts.YesOrNo_Save
            };
            foreach (var item in yes)
            {
                if (list.Contains(item))
                {
                    list.Remove(item);
                }
            }
            if (list.Contains(input.FinanceDictionaryDetailId) && input.Comment.IsNullOrWhiteSpace())
            {

                throw new FriendlyException($"必须填写退回原因！");
            }

            //获取全部的线和节点
            var workFlowInstanceId = await _nodeInstanceRepository.GetAll().Where(p => p.Id == input.NodeInstanceId).Select(p => p.WorkFlowInstanceId).FirstAsync();
            var nodeInstance = await _nodeInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == workFlowInstanceId);
            var lineInstance = await _lineInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == workFlowInstanceId);

            //将信息写入节点中
            var changeNode = nodeInstance.First(p => p.Id == input.NodeInstanceId);

            if (changeNode.NodeInstanceStatus != NodeInstanceStatus.Current && isCheck)
            {
                throw new FriendlyException($"该节点已流转或尚未激活！");
            }

            #region 核价看板流转逻辑

            if (changeNode.Name == "核价看板" && input.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Yes)
            {
                var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == changeNode.WorkFlowInstanceId);
                if (priceEvaluation is null || !priceEvaluation.TrProgramme.HasValue)
                {
                    throw new FriendlyException($"必须上传TR方案！");
                }
            }

            #endregion

            changeNode.FinanceDictionaryDetailId = input.FinanceDictionaryDetailId;


            //设置任务重置
            var taskResets = await _taskResetRepository.GetAllListAsync(p => p.NodeInstanceId == input.NodeInstanceId);
            foreach (var taskReset in taskResets)
            {
                taskReset.IsActive = false;
            }

            //给业务节点增加历史记录
            await _instanceHistoryRepository.InsertAsync(new InstanceHistory
            {
                WorkFlowId = changeNode.WorkFlowId,
                WorkFlowInstanceId = workFlowInstanceId,
                NodeId = changeNode.NodeId,
                FinanceDictionaryDetailId = changeNode.FinanceDictionaryDetailId,
                NodeInstanceId = changeNode.Id,
                Comment = input.Comment,
            });

            //如果是归档节点
            if (changeNode.NodeType == NodeType.End)
            {
                //就把当前操作人写入节点中，在待办中不出现这些用户的信息
                var operatedUserIds = changeNode.OperatedUserIds.StrToList();
                operatedUserIds.Add(AbpSession.UserId.Value.ToString());
                changeNode.OperatedUserIds = string.Join(",", operatedUserIds);
                return;
            }


            //获取被激活的线，并将状态置为当前
            var activeLine = lineInstance.Where(p => p.SoureNodeId == changeNode.NodeId)
                .Where(p => p.FinanceDictionaryDetailId.StrToList().Contains(input.FinanceDictionaryDetailId));
            foreach (var item in activeLine)
            {
                item.NodeInstanceStatus = NodeInstanceStatus.Current;
                item.IsCurrent = true;
            }

            //获取被激活的线连接的节点，执行表达式，判断节点是否被激活。如果被激活，则改变此节点的状态为当前，并且把前面的线状态改为已经过
            //如果未被激活，不执行任何操作 
            var business2Node = nodeInstance
                .Where(p => activeLine.Select(o => o.TargetNodeId).Contains(p.NodeId));

            //如果当前节点没有后续的连线，就把当前节点设置为已经过
            if (!lineInstance.Any(p => p.SoureNodeId == changeNode.NodeId))
            {
                changeNode.NodeInstanceStatus = NodeInstanceStatus.Passed;

                //先获取目标为它们的线
                var targetBusiness2NodeLines1 = lineInstance.Where(p => p.TargetNodeId == changeNode.NodeId);

                foreach (var line in targetBusiness2NodeLines1.Where(p => p.IsCurrent))
                {
                    line.NodeInstanceStatus = NodeInstanceStatus.Passed;
                    line.IsCurrent = false;
                }
            }

            //判断这些节点是否被激活
            foreach (var item in business2Node)
            {
                //先获取目标为它们的线
                var targetBusiness2NodeLines = lineInstance.Where(p => p.TargetNodeId == item.NodeId);

                //将线转条件数组
                //var parameters = targetBusiness2NodeLines.Select(p => new DynamicExpresso.Parameter($"{p.LineId}", typeof(bool), p.IsCurrent)).ToArray();
                var parameters = targetBusiness2NodeLines.Select(p => new DynamicExpresso.Parameter($"{p.LineId}", typeof(bool), p.NodeInstanceStatus == NodeInstanceStatus.Current || p.NodeInstanceStatus == NodeInstanceStatus.Passed)).ToArray();


                //执行条件
                var target = new Interpreter();
                var result = target.Eval<bool>(item.Activation, parameters);

                //如果节点被激活
                if (result)
                {
                    //把当前节点设置为已经过
                    changeNode.NodeInstanceStatus = NodeInstanceStatus.Passed;

                    //状态改为当前，填充数据变为空
                    item.NodeInstanceStatus = NodeInstanceStatus.Current;
                    item.FinanceDictionaryDetailId = string.Empty;
                    item.Comment = input.Comment;

                    //退回状态复位
                    item.IsBack = false;

                    //多路退回
                    if (targetBusiness2NodeLines.Select(p => p.FinanceDictionaryDetailIds).Any(p => !p.IsNullOrWhiteSpace()))
                    {
                        item.FinanceDictionaryDetailIds = string.Join(",", targetBusiness2NodeLines.Where(p => !p.FinanceDictionaryDetailIds.IsNullOrWhiteSpace()).SelectMany(p => p.FinanceDictionaryDetailIds.StrToList()));
                    }

                    foreach (var line in targetBusiness2NodeLines.Where(p => p.IsCurrent))
                    {
                        line.NodeInstanceStatus = NodeInstanceStatus.Passed;
                        line.IsCurrent = false;


                        //退回逻辑，如果被激活的节点和目标节点的连线，类型是退回连线，就把两者之间所有可能的路径，都设置为已重置
                        if (line.LineType == LineType.Reset)
                        {
                            var route = await GetNodeRoute(nodeInstance.FirstOrDefault(p => p.NodeId == line.TargetNodeId).Id, nodeInstance.FirstOrDefault(p => p.NodeId == line.SoureNodeId).Id);
                            if (route.Any())
                            {
                                var lines = route.Select(p => p.Zip(p.Skip(1), (a, b) => lineInstance.FirstOrDefault(o => o.SoureNodeId == a.NodeId && o.TargetNodeId == b.NodeId)))
                                    .SelectMany(p => p).Where(p => p != null).DistinctBy(p => p.Id);
                                lines.ForEach(p => p.NodeInstanceStatus = NodeInstanceStatus.Reset);
                            }
                        }


                        if (line.LineType == LineType.Reset || line.FinanceDictionaryDetailId == FinanceConsts.YesOrNo_No)
                        {
                            //设置退回状态
                            item.IsBack = true;
                        }
                    }

                    #region 定制结构件特殊处理逻辑
                    //如果是定制结构件节点，就再调用本函数一次，自动提交本函数
                    //该节点开发完毕后删除本region代码
                    if (item.NodeId == "主流程_定制结构件")
                    {
                        item.NodeInstanceStatus = NodeInstanceStatus.Passed;
                        item.FinanceDictionaryDetailId = FinanceConsts.Done;

                        var linrjg = lineInstance.FirstOrDefault(p => p.LineId == "主流程_定制结构件_主流程_结构BOM单价审核");
                        linrjg.NodeInstanceStatus = NodeInstanceStatus.Passed;
                        linrjg.IsCurrent = true;
                        //await SubmitNode(new SubmitNodeInput
                        //{
                        //    NodeInstanceId = item.Id,
                        //    FinanceDictionaryDetailId = FinanceConsts.Done,
                        //    Comment = "该节点暂未开发，系统自动提交。"
                        //});
                    }
                    #endregion
                }
                else
                {
                    //如果节点不被激活，就看两点之间的线是否激活，如果激活，就把当前节点设置为已经过
                    var line = lineInstance.FirstOrDefault(p => p.SoureNodeId == changeNode.NodeId && p.TargetNodeId == item.NodeId);
                    if (line is not null && line.IsCurrent)
                    {
                        changeNode.NodeInstanceStatus = NodeInstanceStatus.Passed;
                    }
                }
            }
        }

        /// <summary>
        /// 将任务重置给别人
        /// </summary>
        /// <returns></returns>
        public async virtual Task ResetTask(ResetTaskInput input)
        {
            if (AbpSession.UserId.Value == input.TargetUserId)
            {
                throw new FriendlyException("不能将任务重置给自己！");
            }

            var isHas = await _taskResetRepository.GetAll().AnyAsync(p =>
            p.NodeInstanceId == input.NodeInstanceId &&
            p.ResetUserId == AbpSession.UserId
            && p.TargetUserId == input.TargetUserId
            && p.IsActive);
            if (isHas)
            {
                throw new FriendlyException("此任务已经重置给这个用户了");
            }

            //将重置给自己的任务取消激活
            var entitys = await _taskResetRepository.GetAllListAsync(
                p => p.NodeInstanceId == input.NodeInstanceId && p.IsActive
                   && p.TargetUserId == AbpSession.UserId.Value);

            foreach (var entity in entitys)
            {
                if (entity is not null)
                {
                    entity.IsActive = false;
                }
            }

            //再把任务重置给别人
            var taskReset = new TaskReset
            {
                IsActive = true,
                NodeInstanceId = input.NodeInstanceId,
                ResetUserId = AbpSession.UserId.Value,
                TargetUserId = input.TargetUserId,
            };
            await _taskResetRepository.InsertAsync(taskReset);

            //发送邮件
            await _backgroundJobManager.EnqueueAsync<SendResetEmailJob, TaskReset>(taskReset);

        }

        /// <summary>
        /// 获取他人重置给自己的任务
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetReset(long userId, bool isFilter = true)
        {
            if (userId == 0)
            {
                userId = AbpSession.UserId == null ? 0 : AbpSession.UserId.Value;
            }

            var node = (from n in _nodeInstanceRepository.GetAll()
                        join w in _workflowInstanceRepository.GetAll() on n.WorkFlowInstanceId equals w.Id
                        join t in _taskResetRepository.GetAll() on n.Id equals t.NodeInstanceId
                        where t.IsActive
                        && w.WorkflowState == WorkflowState.Running && n.NodeInstanceStatus == NodeInstanceStatus.Current
                        select new UserTask
                        {
                            Id = n.Id,
                            WorkFlowName = w.Name,
                            Title = w.Title,
                            NodeName = n.Name,
                            CreationTime = n.CreationTime,
                            WorkflowState = w.WorkflowState,
                            WorkFlowInstanceId = w.Id,
                            ProcessIdentifier = n.ProcessIdentifier,
                            IsBack = n.IsBack,
                            Comment = n.Comment,
                            IsReset = true,
                            TargetUserId = t.TargetUserId,
                            ResetUserId = t.ResetUserId,
                        }).WhereIf(isFilter, t => t.TargetUserId == userId);
            var result = await node.ToListAsync();
            return new PagedResultDto<UserTask>(result.Count, result);

        }

        /// <summary>
        /// 获取自己重置给他人的任务
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetReseted(long userId, bool isFilter = true)
        {
            if (userId == 0)
            {
                userId = AbpSession.UserId == null ? 0 : AbpSession.UserId.Value;
            }

            var node = (from n in _nodeInstanceRepository.GetAll()
                        join w in _workflowInstanceRepository.GetAll() on n.WorkFlowInstanceId equals w.Id
                        join t in _taskResetRepository.GetAll() on n.Id equals t.NodeInstanceId
                        where t.IsActive
                        && w.WorkflowState == WorkflowState.Running && n.NodeInstanceStatus == NodeInstanceStatus.Current
                        select new UserTask
                        {
                            Id = n.Id,
                            WorkFlowName = w.Name,
                            Title = w.Title,
                            NodeName = n.Name,
                            CreationTime = n.CreationTime,
                            WorkflowState = w.WorkflowState,
                            WorkFlowInstanceId = w.Id,
                            ProcessIdentifier = n.ProcessIdentifier,
                            IsBack = n.IsBack,
                            Comment = n.Comment,
                            IsReset = true,
                            TargetUserId = t.TargetUserId,
                            ResetUserId = t.ResetUserId,
                        }).WhereIf(isFilter, t => t.ResetUserId == userId);
            var result = await node.ToListAsync();
            return new PagedResultDto<UserTask>(result.Count, result);

        }

        /// <summary>
        /// 获取任务重置详情（重置页面专用）
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<ResetList>> GetResetList(GetResetListInput input)
        {
            var data = (from t in _taskResetRepository.GetAll()
                        join ur in _userManager.Users on t.ResetUserId equals ur.Id
                        join ut in _userManager.Users on t.TargetUserId equals ut.Id
                        join n in _nodeInstanceRepository.GetAll() on t.NodeInstanceId equals n.Id
                        select new ResetList
                        {
                            Id = t.Id,
                            NodeName = n.Name,
                            ResetUser = ur.Name,
                            TargetUser = ut.Name,
                            NodeStatus = n.NodeInstanceStatus,
                            AuditFlowId = n.WorkFlowInstanceId,
                            CreationTime = t.CreationTime,
                            CreatorUserId = t.CreatorUserId,
                        })
                       .WhereIf(!input.NodeName.IsNullOrWhiteSpace(), p => p.NodeName.Contains(input.NodeName))
                       .WhereIf(!input.ResetUser.IsNullOrWhiteSpace(), p => p.ResetUser.Contains(input.ResetUser))
                       .WhereIf(!input.TargetUser.IsNullOrWhiteSpace(), p => p.TargetUser.Contains(input.TargetUser));
            var count = await data.CountAsync();
            var result = await data.PageBy(input).ToListAsync();
            return new PagedResultDto<ResetList>(count, result);
        }

        /// <summary>
        /// 根据用户Id 获取待办
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetTaskByUserId(long userId, bool isFilter = true)
        {
            if (userId == 0)
            {
                userId = AbpSession.UserId == null ? 0 : AbpSession.UserId.Value;
            }
            //获取用户所有拥有的工作流相关的角色
            var roleIds = await _userRoleRepository.GetAll().Where(p => p.UserId == userId)
                .Join(_roleRepository.GetAll(), p => p.RoleId, p => p.Id, (a, b) => b).Select(p => p.Id.ToString()).ToListAsync();

            //获取角色在未归档的工作流实例里活动的节点
            var node = _nodeInstanceRepository.GetAll()
                .Join(_workflowInstanceRepository.GetAll(), n => n.WorkFlowInstanceId, w => w.Id, (n, w) => new { n, w })
                .Where(p =>
                (p.w.WorkflowState == WorkflowState.Running && p.n.NodeInstanceStatus == NodeInstanceStatus.Current)
                //|| (p.w.WorkflowState == WorkflowState.Ended && p.n.NodeInstanceStatus == NodeInstanceStatus.Current && p.n.NodeId == "主流程_归档")
                );


            //取入内存中
            var nodeList = await node.ToListAsync();

            var dto = nodeList.Select(p => new
            {
                p.n.Id,
                WorkFlowName = p.w.Name,
                p.w.Title,
                NodeName = p.n.Name,
                p.n.CreationTime,
                RoleId = p.n.RoleId.StrToList(),
                p.w.WorkflowState,
                WorkFlowInstanceId = p.w.Id,
                p.n.NodeType,
                p.n.OperatedUserIds,
                p.n.ProcessIdentifier,
                p.n.IsBack,
                p.n.Comment,
            });

            //是否筛选
            if (isFilter)
            {
                if (roleIds.Any())
                {
                    //判断两个集合是否存在交集（不过滤归档）
                    dto = dto.Where(p => p.RoleId.ToHashSet().Overlaps(roleIds) || p.NodeType == NodeType.End)
                     .Where(p => p.NodeType != NodeType.End || p.OperatedUserIds.IsNullOrWhiteSpace() || !p.OperatedUserIds.StrToList().Select(o => o.To<long>()).Contains(AbpSession.UserId.Value));
                }
                else
                {
                    dto = dto.Where(p => false);
                }
            }

            var roles = dto.SelectMany(p => p.RoleId).Select(p => p.To<long>()).Distinct();

            //根据角色获取用户
            var users = await (from u in _userManager.Users
                               join ur in _userRoleRepository.GetAll() on u.Id equals ur.UserId
                               //join r in roles on ur.RoleId equals r
                               join r in _roleManager.Roles on ur.RoleId equals r.Id
                               where roles.Contains(r.Id)
                               select new
                               {
                                   r.Id,
                                   u.Name
                               }).ToListAsync();

            var result = dto.Select(p => new UserTask
            {
                Id = p.Id,
                CreationTime = p.CreationTime,
                NodeName = p.NodeName,
                Title = p.Title,
                WorkFlowName = p.WorkFlowName,
                TaskUser = string.Join(",", p.RoleId.SelectMany(o => users.Where(x => x.Id == o.To<long>()).Select(p => p.Name)).Distinct()),
                TaskUserIds = p.RoleId.SelectMany(o => users.Where(x => x.Id == o.To<long>()).Select(p => p.Id)).Distinct().ToList(),
                WorkflowState = p.WorkflowState,
                WorkFlowInstanceId = p.WorkFlowInstanceId,
                ProcessIdentifier = p.ProcessIdentifier,
                IsBack = p.IsBack,
                Comment = p.Comment,
            }).ToList();
            return new PagedResultDto<UserTask>(result.Count, result);
        }

        /// <summary>
        /// 根据流程Id，获取待办项
        /// </summary>
        /// <returns></returns>
        public async virtual Task<List<UserTask>> GetTaskByWorkflowInstanceId(long workflowInstanceId, long? nodeInstanceId = null)
        {
            var data = await (from n in _nodeInstanceRepository.GetAll()
                              join w in _workflowInstanceRepository.GetAll() on n.WorkFlowInstanceId equals w.Id
                              where n.WorkFlowInstanceId == workflowInstanceId //&& n.NodeInstanceStatus == NodeInstanceStatus.Current
                              select new UserTask
                              {
                                  Id = n.Id,
                                  WorkFlowInstanceId = n.WorkFlowInstanceId,
                                  WorkFlowName = w.Name,
                                  Title = w.Title,
                                  NodeName = n.Name,
                                  CreationTime = w.CreationTime,
                                  WorkflowState = w.WorkflowState,
                                  ProcessIdentifier = n.ProcessIdentifier,
                                  RoleId = n.RoleId,

                              }).WhereIf(nodeInstanceId.HasValue, p => p.Id == nodeInstanceId).ToListAsync();

            foreach (var item in data)
            {
                if (item.RoleId.IsNullOrWhiteSpace())
                {
                    return null;
                }
                var roleids = item.RoleId.Split(",").Select(p => p.To<int>());
                var userIds = await _userRoleRepository.GetAll().Where(p => roleids.Contains(p.RoleId)).Select(p => p.UserId).ToListAsync();
                item.TaskUserIds = userIds.Select(p => p.To<int>()).ToList();



                #region 用户权限

                //    //获取当前流程方案列表
                //    var solutionList = await _solutionRepository.GetAllListAsync(p => p.AuditFlowId == workflowInstanceId);

                //    //获取核价团队
                //    var pricingTeam = await _pricingTeamRepository.FirstOrDefaultAsync(p => p.AuditFlowId == workflowInstanceId);

                //    //获取项目经理
                //    var projectPm = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == workflowInstanceId);

                //    //获取核价需求录入保存项
                //    var priceEvaluationStartData = await _priceEvaluationStartDataRepository.FirstOrDefaultAsync(p => p.AuditFlowId == workflowInstanceId);

                //    //项目经理控制的页面
                //    var pmPage = new List<string> { FinanceConsts.PriceDemandReview, FinanceConsts.NRE_ManualComponentInput, FinanceConsts.UnitPriceInputReviewToExamine, FinanceConsts.PriceEvaluationBoard };

                //    //拥有能看归档的角色的用户
                //    var role = await _roleRepository.GetAllListAsync(p =>
                //                    p.Name == StaticRoleNames.Host.FinanceTableAdmin
                //                    || p.Name == StaticRoleNames.Host.EvalTableAdmin
                //            || p.Name == StaticRoleNames.Host.Bjdgdgly);
                //    var endUserIds = await _userRoleRepository.GetAll().Where(p => role.Select(p => p.Id).Contains(p.RoleId)).Select(p => p.UserId).ToListAsync();

                var deleteUserIds = new List<int>();

                foreach (var userId in item.TaskUserIds)
                {
                    var dto = await _ruleAppService.FilteTask(workflowInstanceId, new List<Audit.Dto.AuditFlowRightDetailDto> { new Audit.Dto.AuditFlowRightDetailDto
                    {
                        ProcessIdentifier = item.ProcessIdentifier,
                        ProcessName = item.NodeName
                    }}, userId);
                    if (dto.IsNullOrEmpty())
                    {
                        deleteUserIds.Add(userId);
                    }
                }
                //    foreach (var userId in item.TaskUserIds)
                //    {
                //        if (
                //            (!solutionList.Any(p => p.ElecEngineerId == userId) && item.ProcessIdentifier == FinanceConsts.ElectronicsBOM)
                //            || (!solutionList.Any(p => p.StructEngineerId == userId) && item.ProcessIdentifier == FinanceConsts.StructureBOM)
                //|| (pricingTeam == null || pricingTeam.EngineerId != userId && item.ProcessIdentifier == FinanceConsts.FormulaOperationAddition)
                //|| (pricingTeam == null || pricingTeam.QualityBenchId != userId && item.ProcessIdentifier == FinanceConsts.NRE_ReliabilityExperimentFeeInput)
                //|| (pricingTeam == null || pricingTeam.EMCId != userId && item.ProcessIdentifier == FinanceConsts.NRE_EMCExperimentalFeeInput)
                //|| (pricingTeam == null || pricingTeam.ProductCostInputId != userId && item.ProcessIdentifier == FinanceConsts.COBManufacturingCostEntry)
                //|| (pricingTeam == null || pricingTeam.ProductManageTimeId != userId && item.ProcessIdentifier == FinanceConsts.LogisticsCostEntry)
                //|| (pricingTeam == null || pricingTeam.AuditId != userId && item.ProcessIdentifier == FinanceConsts.ProjectChiefAudit)
                //|| (projectPm == null || projectPm.ProjectManager != userId && ((pmPage.Contains(item.ProcessIdentifier)) && item.NodeName != FinanceConsts.Bomcbsh))
                //|| (projectPm == null || projectPm.CreatorUserId != userId && item.ProcessIdentifier == FinanceConsts.QuoteAnalysis)

                //|| ((priceEvaluationStartData != null && priceEvaluationStartData.CreatorUserId != null && priceEvaluationStartData.CreatorUserId != userId && item.ProcessIdentifier == FinanceConsts.PricingDemandInput)
                //|| (projectPm != null && projectPm.CreatorUserId != userId) && item.ProcessIdentifier == FinanceConsts.PricingDemandInput)

                //|| (projectPm == null || projectPm.CreatorUserId != userId && item.ProcessIdentifier == "ExternalQuotation")

                //|| (projectPm == null || projectPm.CreatorUserId != userId && (item.ProcessIdentifier == "QuotationApprovalForm" || item.ProcessIdentifier == "QuoteFeedback"))

                //|| ((projectPm == null) || (projectPm.ProjectManager == userId && (item.ProcessIdentifier == "QuoteApproval"
                //|| item.ProcessIdentifier == "QuoteFeedback" || item.ProcessIdentifier == "BidWinningConfirmation"
                //)))

                //|| (projectPm == null || (projectPm.CreatorUserId != userId && projectPm.ProjectManager != userId)
                //&& (!endUserIds.Contains(userId) && item.ProcessIdentifier == "ArchiveEnd")
                //            )
                //            )
                //        {
                //            deleteUserIds.Add(userId);
                //        }
                //    }

                foreach (var deleteUserId in deleteUserIds)
                {
                    item.TaskUserIds.Remove(deleteUserId);
                }

                #endregion

                //查询重置
                var resets = await _taskResetRepository.GetAllListAsync(p => p.NodeInstanceId == item.Id && p.IsActive);
                foreach (var reset in resets)
                {
                    item.TaskUserIds.Remove(reset.ResetUserId.To<int>());
                    item.TaskUserIds.Add(reset.TargetUserId.To<int>());
                }
            }
            return data;
        }

        /// <summary>
        /// 根据当前用户Id 获取已办，基于项目经理过滤
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetTaskCompletedFilter()
        {
            var data = from h in _instanceHistoryRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on h.WorkFlowInstanceId equals w.Id
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id

                       join pe in _priceEvaluationRepository.GetAll() on h.WorkFlowInstanceId equals pe.AuditFlowId into pe1
                       from p in pe1.DefaultIfEmpty()

                       where ((p != null && p.ProjectManager == AbpSession.UserId) || (h.CreatorUserId == AbpSession.UserId)


                       || n.Name == "贸易合规" || n.Name == "查看每个方案初版BOM成本" || n.Name == "项目部长查看核价表"
                        || n.Name == "总经理查看中标金额" || n.Name == "核心器件成本NRE费用拆分" || n.Name == "开始"
                        || n.Name == "生成报价分析界面选择报价方案"

                        || n.Name == "归档") && (!h.FinanceDictionaryDetailId.Contains("Save"))

                       //|| n.Name == "报价单" || n.Name == "报价审批表" ||n.Name == "报价反馈" || n.Name == "选择是否报价"
                       //|| n.Name == "审批报价策略与核价表" 
                       //|| n.Name == "确认中标金额" 
                       select new UserTask
                       {
                           Id = h.NodeInstanceId,
                           WorkFlowName = w.Name,
                           Title = w.Title,
                           NodeName = n.Name,
                           CreationTime = w.CreationTime,
                           TaskUser = u.Name,
                           WorkflowState = w.WorkflowState,
                           WorkFlowInstanceId = h.WorkFlowInstanceId,
                           ProcessIdentifier = n.ProcessIdentifier,
                           Time = h.CreationTime,
                       };
            var result = data.ToList().DistinctBy(p => new { p.Id, p.WorkFlowInstanceId }).ToList();
            var count = result.Count;

            return new PagedResultDto<UserTask>(count, result);
        }

        /// <summary>
        /// 根据当前用户Id 获取已办，不过滤
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetTaskCompleted()
        {
            var data = from h in _instanceHistoryRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on h.WorkFlowInstanceId equals w.Id
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id
                       where !h.FinanceDictionaryDetailId.Contains("Save")
                       select new UserTask
                       {
                           Id = h.NodeInstanceId,
                           WorkFlowName = w.Name,
                           Title = w.Title,
                           NodeName = n.Name,
                           CreationTime = w.CreationTime,
                           TaskUser = u.Name,
                           WorkflowState = w.WorkflowState,
                           WorkFlowInstanceId = h.WorkFlowInstanceId,
                           ProcessIdentifier = n.ProcessIdentifier,
                           Time = h.CreationTime,
                       };
            var result = data.ToList().DistinctBy(p => new { p.Id, p.WorkFlowInstanceId }).ToList();
            var count = result.Count;

            return new PagedResultDto<UserTask>(count, result);
        }

        /// <summary>
        /// 根据用户Id 获取已办
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workflowState"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetTaskCompletedByUserId(long userId, WorkflowState workflowState)
        {
            if (userId == 0)
            {
                userId = AbpSession.UserId == null ? 0 : AbpSession.UserId.Value;
            }

            var data = from h in _instanceHistoryRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on h.WorkFlowInstanceId equals w.Id
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id
                       where w.WorkflowState == workflowState && h.CreatorUserId == userId
                       select new UserTask
                       {
                           Id = h.NodeInstanceId,
                           WorkFlowName = w.Name,
                           Title = w.Title,
                           NodeName = n.Name,
                           CreationTime = w.CreationTime,
                           TaskUser = u.Name,
                           WorkflowState = w.WorkflowState,
                           WorkFlowInstanceId = h.WorkFlowInstanceId
                       };
            var count = await data.CountAsync();
            var result = await data.ToListAsync();
            return new PagedResultDto<UserTask>(count, result);
        }

        /// <summary>
        /// 根据工作流实例Id 获取流程历史
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetInstanceHistoryByWorkflowInstanceId(long workflowInstanceId)
        {
            var data = from h in _instanceHistoryRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on h.WorkFlowInstanceId equals w.Id
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id
                       where h.WorkFlowInstanceId == workflowInstanceId
                       select new UserTask
                       {
                           Id = h.NodeInstanceId,
                           WorkFlowName = w.Name,
                           Title = w.Title,
                           NodeName = n.Name,
                           CreationTime = w.CreationTime,
                           TaskUser = u.Name,
                           WorkflowState = w.WorkflowState,
                           ProcessIdentifier = n.ProcessIdentifier,
                           WorkFlowInstanceId = h.WorkFlowInstanceId
                       };
            var count = await data.CountAsync();
            var result = await data.ToListAsync();
            return new PagedResultDto<UserTask>(count, result);
        }

        /// <summary>
        /// 根据流程Id 获取流程历史
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<InstanceHistoryListDto>> GetInstanceHistoryById(long workflowInstanceId)
        {
            var data = from h in _instanceHistoryRepository.GetAll()
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id
                       join f in _financeDictionaryDetailRepository.GetAll() on h.FinanceDictionaryDetailId equals f.Id
                       where h.WorkFlowInstanceId == workflowInstanceId
                       orderby h.Id descending
                       select new InstanceHistoryListDto
                       {
                           UserName = u.Name,
                           NodeName = n.Name,
                           DisplayName = f.DisplayName,
                           Comment = h.Comment,

                           Id = h.Id,
                           CreationTime = h.CreationTime,
                           CreatorUserId = h.CreatorUserId,
                           DeleterUserId = h.DeleterUserId,
                           DeletionTime = h.DeletionTime,
                           IsDeleted = h.IsDeleted,
                           LastModificationTime = h.LastModificationTime,
                           LastModifierUserId = h.LastModifierUserId,
                       };
            var count = await data.CountAsync();
            var result = await data.ToListAsync();
            return new PagedResultDto<InstanceHistoryListDto>(count, result);
        }

        /// <summary>
        /// 获取流程历史
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<InstanceHistorys>> GetInstanceHistorys(long workflowInstanceId)
        {
            var data = from i in _instanceHistoryRepository.GetAll()
                       join u in _userManager.Users on i.CreatorUserId equals u.Id

                       join d in _departmentRepository.GetAll() on u.DepartmentId equals d.Id into d1
                       from d2 in d1.DefaultIfEmpty()


                       join n in _nodeInstanceRepository.GetAll() on i.NodeInstanceId equals n.Id
                       join f in _financeDictionaryDetailRepository.GetAll() on i.FinanceDictionaryDetailId equals f.Id
                       where i.WorkFlowInstanceId == workflowInstanceId
                       orderby i.CreationTime descending
                       select new InstanceHistorys
                       {
                           Id = i.Id,
                           CreatorUserId = i.CreatorUserId,
                           CreationTime = i.CreationTime,
                           LastModificationTime = i.LastModificationTime,
                           LastModifierUserId = i.LastModifierUserId,
                           DeleterUserId = i.DeleterUserId,
                           DeletionTime = i.DeletionTime,
                           IsDeleted = i.IsDeleted,

                           UserName = u.Name,
                           UserDepartmentName = d2 == null ? string.Empty : d2.Name,
                           NodeName = n.Name,
                           DisplayName = f.DisplayName,
                           Comment = i.Comment
                       };
            var result = await data.ToListAsync();
            return new PagedResultDto<InstanceHistorys>(result.Count, result);
        }

        /// <summary>
        /// 获取当前用户的已办
        /// </summary>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetInstanceHistory()
        {
            //获取用户所有拥有的工作流相关的角色
            var roleIds = await _userRoleRepository.GetAll().Where(p => p.UserId == AbpSession.UserId)
                .Join(_roleRepository.GetAll(), p => p.RoleId, p => p.Id, (a, b) => b).Select(p => p.Id.ToString()).ToListAsync();

            var data = from h in _instanceHistoryRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on h.WorkFlowInstanceId equals w.Id
                       join n in _nodeInstanceRepository.GetAll() on h.NodeInstanceId equals n.Id
                       join u in _userManager.Users on h.CreatorUserId equals u.Id
                       where h.CreatorUserId == AbpSession.UserId || roleIds.Contains(n.RoleId)
                       select new UserTask
                       {
                           Id = h.NodeInstanceId,
                           WorkFlowInstanceId = h.WorkFlowInstanceId,
                           WorkFlowName = w.Name,
                           Title = w.Title,
                           NodeName = n.Name,
                           CreationTime = w.CreationTime,
                           TaskUser = u.Name,
                           WorkflowState = w.WorkflowState,
                           ProcessIdentifier = n.ProcessIdentifier,
                           RoleIds = n.RoleId
                       };
            var count = await data.CountAsync();
            var result = await data.ToListAsync();
            var dto = result.Where(p => p.RoleIds.IsNullOrWhiteSpace() || p.RoleIds.Split(",").ToList().ToHashSet().Overlaps(roleIds))
               .Distinct().ToList();
            return new PagedResultDto<UserTask>(count, dto);
        }



        /// <summary>
        /// 根据流程实例Id 获取流程整体状态
        /// </summary>
        /// <param name="workFlowInstanceId"></param>
        /// <returns></returns>
        public async virtual Task<List<NodeInstance>> GetWorkflowStatus(long workFlowInstanceId)
        {
            var data = await _nodeInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == workFlowInstanceId);
            return data;
        }

        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<PagedResultDto<WorkflowInstance>> GetWorkflowInstance(GetWorkflowInstanceInput input)
        {
            var data = _workflowInstanceRepository.GetAll().Where(p => p.WorkflowState == input.WorkflowState);

            var count = await data.CountAsync();

            var paged = data.PageBy(input);

            var result = await paged.ToListAsync();

            return new PagedResultDto<WorkflowInstance>(count, result);
        }

        /// <summary>
        /// 根据实例Id获取审批意见
        /// </summary>
        /// <param name="nodeInstanceId"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<FinanceDictionaryDetailListDto>> GetEvalDataByNodeInstanceId(long nodeInstanceId)
        {
            var node = await _nodeInstanceRepository.GetAsync(nodeInstanceId);

            //先获取目标为它的线
            var yj = await _lineInstanceRepository.GetAll()
                .Where(p => p.TargetNodeId == node.NodeId && p.NodeInstanceStatus == NodeInstanceStatus.Passed)
                .Select(p => p.FinanceDictionaryDetailIds).ToListAsync();
            if (yj.All(p => p.IsNullOrWhiteSpace()))
            {
                var data = from n in _nodeInstanceRepository.GetAll()
                           join d in _financeDictionaryDetailRepository.GetAll() on n.FinanceDictionaryId equals d.FinanceDictionaryId
                           where n.Id == nodeInstanceId
                           select d;
                var count = await data.CountAsync();
                var result = await data.ToListAsync();
                return new PagedResultDto<FinanceDictionaryDetailListDto>(count, ObjectMapper.Map<List<FinanceDictionaryDetailListDto>>(result));
            }
            else
            {
                var tj = yj.Where(p => !p.IsNullOrWhiteSpace()).SelectMany(p => p.StrToList());
                var data = _financeDictionaryDetailRepository.GetAll().Where(p => tj.Contains(p.Id));
                var result = await data.ToListAsync();
                var count = await data.CountAsync();
                return new PagedResultDto<FinanceDictionaryDetailListDto>(count, ObjectMapper.Map<List<FinanceDictionaryDetailListDto>>(result));
            }

        }

        /// <summary>
        /// 获取两个节点之间的线路
        /// </summary>
        /// <returns></returns>
        private async Task<List<List<NodeInstance>>> GetNodeRoute(long sourceNodeInstanceId, long targetNodeInstanceId)
        {
            var sourceNodeInstance = await _nodeInstanceRepository.GetAsync(sourceNodeInstanceId);

            var targetNodeInstance = await _nodeInstanceRepository.GetAsync(targetNodeInstanceId);

            var paths = new List<List<NodeInstance>>();

            await IsHasTargetNode(sourceNodeInstance, targetNodeInstance, paths);

            return paths;
        }

        /// <summary>
        /// 获取一个节点的目标节点，以及目标节点的目标节点中是否包含指定的目标节点（在查询途中，顺手记录true的节点的路径）
        /// </summary>
        /// <param name="sourceNodeInstance"></param>
        /// <param name="targetNodeInstance"></param>
        /// <param name="pathNode"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public async Task<bool> IsHasTargetNode(NodeInstance sourceNodeInstance, NodeInstance targetNodeInstance, List<List<NodeInstance>> paths, List<NodeInstance> pathNode = null)
        {
            var sourceNodeInstanceId = sourceNodeInstance.Id;
            var targetNodeInstanceId = targetNodeInstance.Id;

            if (pathNode is null)
            {
                pathNode = new List<NodeInstance>();
            }

            pathNode.Add(sourceNodeInstance);

            var data = await GetTargetNodeByNodeId(sourceNodeInstanceId);

            if (data is null || !data.Any())
            {
                return false;
            }
            else
            {
                if (data.Select(p => p.Id).Any(p => p == targetNodeInstanceId))
                {

                    pathNode.Add(targetNodeInstance);

                    paths.Add(pathNode);
                    return true;
                }
                else
                {
                    var result = false;
                    foreach (var item in data)
                    {
                        result = await IsHasTargetNode(item, targetNodeInstance, paths, new List<NodeInstance>(pathNode));//pathNode
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 根据节点Id获取目标节点
        /// </summary>
        /// <returns></returns>
        private async Task<List<NodeInstance>> GetTargetNodeByNodeId(long nodeInstanceId)
        {
            var hjkb = new List<string> { FinanceConsts .HjkbSelect_Input ,
                FinanceConsts .HjkbSelect_Yes ,
            };

            var nodeInstance = await _nodeInstanceRepository.GetAsync(nodeInstanceId);

            var data = from l in _lineInstanceRepository.GetAll()
                       join n in _nodeInstanceRepository.GetAll() on l.TargetNodeId equals n.NodeId
                       where l.SoureNodeId == nodeInstance.NodeId && l.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId
                       && n.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId && l.LineType != LineType.Reset && l.FinanceDictionaryDetailId != FinanceConsts.YesOrNo_No
                       && (!hjkb.Contains(l.FinanceDictionaryDetailId))
                       select n;
            var result = await data.ToListAsync();

            return result;
        }

        /// <summary>
        /// 核价看板专用流转接口
        /// </summary>
        /// <returns></returns>
        public async Task PanelSubmitNode(PanelSubmitNodeInput input)
        {
            #region 参数校验

            //如果审批意见有【同意】，且有存在【同意】以外的审批意见
            if (input.FinanceDictionaryDetailIds.Contains(FinanceConsts.HjkbSelect_Yes) && input.FinanceDictionaryDetailIds.Count > 1)
            {
                throw new FriendlyException($"不可同时选择【同意】和【退回】！");
            }

            //只要审批意见里存在不是【同意】意见的，且审批评论为空
            if (input.FinanceDictionaryDetailIds.Any(p => p != FinanceConsts.HjkbSelect_Yes) && input.Comment.IsNullOrWhiteSpace())
            {
                throw new FriendlyException($"必须填写退回原因！");
            }

            //只要审批意见里存在退回到核价需求录入的
            if (input.FinanceDictionaryDetailIds.Contains(FinanceConsts.HjkbSelect_Input))
            {
                throw new FriendlyException($"核价看板不允许退回到核价需求录入！");
            }

            #endregion

            #region 同意

            //审批意见集合有且仅有同意
            if (input.FinanceDictionaryDetailIds.Count == 1 && input.FinanceDictionaryDetailIds.Contains(FinanceConsts.HjkbSelect_Yes))
            {
                #region 直接上传流程：文档上传校验

                var nodeInstance = await _nodeInstanceRepository.GetAsync(input.NodeInstanceId);
                var list = new List<string> { FinanceConsts.EvalReason_Shj, FinanceConsts.EvalReason_Qtsclc, FinanceConsts.EvalReason_Bnnj };

                var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId && p.NodeId == "主流程_核价需求录入");

                //若是上传流程
                if (list.Contains(node.FinanceDictionaryDetailId))
                {
                    //判断核价表上传是否完整，梯度和方案是否均已上传
                    var gradientCount = await _gradientRepository.CountAsync(p => p.AuditFlowId == nodeInstance.WorkFlowInstanceId);
                    var solutionCount = await _solutionRepository.CountAsync(p => p.AuditFlowId == nodeInstance.WorkFlowInstanceId);

                    var fuBomCount = await _fu_BomRepository.GetAll().Where(p => p.AuditFlowId == nodeInstance.WorkFlowInstanceId)
                        .Select(p => new { p.GradientId, p.SolutionId })
                        .Distinct()
                        .CountAsync();

                    if (fuBomCount != (gradientCount * solutionCount))
                    {
                        throw new FriendlyException($"没有上传完整的核价表，不可流转！");
                    }

                    var auditFlowIdPricingForms = await _auditFlowIdPricingForm.GetAll().Where(p => p.AuditFlowId == nodeInstance.WorkFlowInstanceId)
                        .Select(p => new { p.SolutionId }).ToListAsync();
                    var auditFlowIdPricingFormsCount = auditFlowIdPricingForms.DistinctBy(p => p.SolutionId).Count();

                    if (auditFlowIdPricingFormsCount != solutionCount)
                    {
                        throw new FriendlyException($"没有上传完整的NRE核价表，不可流转！");
                    }
                }

                #endregion


                //正常调用流程流转接口
                await SubmitNodeInterfece(new SubmitNodeInput
                {
                    Comment = input.Comment,
                    NodeInstanceId = input.NodeInstanceId,
                    FinanceDictionaryDetailId = input.FinanceDictionaryDetailIds[0]
                });
            }
            else
            {
                #region 退回

                foreach (var financeDictionaryDetailId in input.FinanceDictionaryDetailIds)
                {
                    //正常调用流程流转接口
                    await SubmitNodeInterfece(new SubmitNodeInput
                    {
                        Comment = input.Comment,
                        NodeInstanceId = input.NodeInstanceId,
                        FinanceDictionaryDetailId = financeDictionaryDetailId
                    }, false);
                }

                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 快速核报价：判断是否为上传流程
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsUploadAuditFlow(long nodeInstanceId)
        {
            var nodeInstance = await _nodeInstanceRepository.GetAsync(nodeInstanceId);
            var list = new List<string> { FinanceConsts.EvalReason_Shj, FinanceConsts.EvalReason_Qtsclc, FinanceConsts.EvalReason_Bnnj };

            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId && p.NodeId == "主流程_核价需求录入");

            return list.Contains(node.FinanceDictionaryDetailId);
        }


        /// <summary>
        /// 获取普通核报价归档流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<PagedResultDto<WorkflowInstance>> GetWorkflowOvered(GetWorkflowOveredInput input)
        {
            //普通流程的核价原因
            var hjyy = new List<string> { FinanceConsts.EvalReason_Schj, FinanceConsts.EvalReason_Fabg, FinanceConsts.EvalReason_Lcyp, FinanceConsts.EvalReason_Qt, FinanceConsts.EvalReason_Bb1, FinanceConsts.EvalReason_Jdcbpg, FinanceConsts.EvalReason_Xmbg, FinanceConsts.EvalReason_Xnnj, FinanceConsts.EvalReason_Nj };

            var data = (from w in _workflowInstanceRepository.GetAll()
                        join h in _instanceHistoryRepository.GetAll() on w.Id equals h.WorkFlowInstanceId
                        where h.NodeId == "主流程_核价需求录入" && hjyy.Contains(h.FinanceDictionaryDetailId)
                        && w.WorkflowState == WorkflowState.Ended
                        select w)
                        .Distinct()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.Title.Contains(input.Filter))
                        .WhereIf(input.AuditFlowId.HasValue && input.AuditFlowId.Value != default, p => p.Id == input.AuditFlowId);

            var count = await data.CountAsync();

            var paged = data.PageBy(input);

            var result = await paged.ToListAsync();

            return new PagedResultDto<WorkflowInstance>(count, result);
        }

        /// <summary>
        /// 获取被删除的流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<PagedResultDto<WorkflowDeleteDto>> GetWorkflowDelete(GetWorkflowOveredInput input)
        {
            //调用OverWorkflow接口归档的流程

            //普通流程的核价原因

            var data = (from w in _workflowInstanceRepository.GetAll()
                        join n in _nodeInstanceRepository.GetAll() on w.Id equals n.WorkFlowInstanceId
                        join p in _priceEvaluationRepository.GetAll() on w.Id equals p.AuditFlowId
                        where n.NodeId == "主流程_归档" && n.Comment.StartsWith("删除流程：")
                        && w.WorkflowState == WorkflowState.Ended
                        select new WorkflowDeleteDto
                        {
                            AuditFlowId = w.Id,
                            Title = p.Title,
                            Number = p.Number,
                            Version = p.QuoteVersion,
                            Comment = n.Comment,
                        })
                        .Distinct()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.Title.Contains(input.Filter));

            var count = await data.CountAsync();

            var paged = data.PageBy(input);

            var result = await paged.ToListAsync();
            foreach (var item in result)
            {
                item.Comment = item.Comment.Replace("删除流程：", string.Empty);
            }

            return new PagedResultDto<WorkflowDeleteDto>(count, result);
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        /// <returns></returns>
        public async virtual Task OverWorkflow(OverWorkflowInput input)
        {
            //var wf = await _workflowInstanceRepository.GetAsync(input.AuditFlowId);
            //wf.WorkflowState = WorkflowState.Ended;

            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == input.AuditFlowId && p.NodeId == "主流程_归档");
            node.NodeInstanceStatus = NodeInstanceStatus.Current;
            node.Comment = $"删除流程：{input.DeleteReason}";

            //var nodes = await _nodeInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == input.AuditFlowId && p.NodeId != "主流程_归档" && p.NodeInstanceStatus == NodeInstanceStatus.Current);
            //foreach (var item in nodes)
            //{
            //    item.NodeInstanceStatus = NodeInstanceStatus.Over;
            //}
        }


        /// <summary>
        /// 激活结构BOM单价审核
        /// </summary>
        /// <returns></returns>
        public async virtual Task ActivateStructBomEval(long auditFlowId)
        {
            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditFlowId && p.Name == "结构BOM单价审核");
            if (node.NodeInstanceStatus != NodeInstanceStatus.Current)
            {
                node.NodeInstanceStatus = NodeInstanceStatus.Current;
            }
        }

        /// <summary>
        /// 失活结构BOM单价审核
        /// </summary>
        /// <returns></returns>
        public async virtual Task DeactivateStructBomEval(long auditFlowId)
        {
            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditFlowId && p.Name == "结构BOM单价审核");
            if (node.NodeInstanceStatus != NodeInstanceStatus.Passed)
            {
                node.NodeInstanceStatus = NodeInstanceStatus.Passed;
            }
        }

        /// <summary>
        /// 激活电子BOM单价审核
        /// </summary>
        /// <returns></returns>
        public async virtual Task ActivateElectronicBomEval(long auditFlowId)
        {
            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditFlowId && p.Name == "电子BOM单价审核");
            if (node.NodeInstanceStatus != NodeInstanceStatus.Current)
            {
                node.NodeInstanceStatus = NodeInstanceStatus.Current;
            }
        }

        /// <summary>
        /// 失活电子BOM单价审核
        /// </summary>
        /// <returns></returns>
        public async virtual Task DeactivateElectronicBomEval(long auditFlowId)
        {
            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditFlowId && p.Name == "电子BOM单价审核");
            if (node.NodeInstanceStatus != NodeInstanceStatus.Passed)
            {
                node.NodeInstanceStatus = NodeInstanceStatus.Passed;
            }
        }
    }
}
