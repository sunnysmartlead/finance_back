using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DynamicExpresso;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.Infrastructure.Dto;
using Finance.PriceEval;
using Finance.WorkFlows.Dto;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowRepository"></param>
        /// <param name="nodeRepository"></param>
        /// <param name="lineRepository"></param>
        /// <param name="workFlowInstanceRepository"></param>
        /// <param name="nodeInstanceRepository"></param>
        /// <param name="lineInstanceRepository"></param>
        /// <param name="instanceHistoryRepository"></param>
        /// <param name="financeDictionaryRepository"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="roleRepository"></param>
        public WorkflowInstanceAppService(IRepository<Workflow, string> workflowRepository, IRepository<Node, string> nodeRepository, IRepository<Line, string> lineRepository,
            IRepository<WorkflowInstance, long> workFlowInstanceRepository, IRepository<NodeInstance, long> nodeInstanceRepository, IRepository<LineInstance, long> lineInstanceRepository,
            IRepository<InstanceHistory, long> instanceHistoryRepository,
            IRepository<FinanceDictionary, string> financeDictionaryRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
            IRepository<UserRole, long> userRoleRepository, IRepository<Role> roleRepository,
            UserManager userManager, RoleManager roleManager,
            IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<TaskReset, long> taskResetRepository
            )
        {
            _workflowRepository = workflowRepository;
            _nodeRepository = nodeRepository;
            _lineRepository = lineRepository;


            _workflowInstanceRepository = workFlowInstanceRepository;
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
        }

        /// <summary>
        /// 手动触发贸易合规（手动测试使用时改为public）
        /// </summary>
        /// <returns></returns>
        private async Task GG()
        {
            var hg = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == 189 && p.NodeId == "主流程_贸易合规");
            hg.LastModificationTime = DateTime.UtcNow;
        }

        public async Task TestLine(long id)
        {
            var lineInstance = await _lineInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == 459);

            var route = await GetNodeRoute(id, 11484);
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
            //将重置给自己的任务取消激活
            var entity = await _taskResetRepository.FirstOrDefaultAsync(
                p => p.NodeInstanceId == input.NodeInstanceId
                   && p.TargetUserId == AbpSession.UserId.Value);
            if (entity is not null)
            {
                entity.IsActive = false;
            }


            //再把任务重置给别人
            await _taskResetRepository.InsertAsync(new TaskReset
            {
                IsActive = true,
                NodeInstanceId = input.NodeInstanceId,
                ResetUserId = AbpSession.UserId.Value,
                TargetUserId = input.TargetUserId,
            });
        }

        /// <summary>
        /// 获取他人重置给自己的任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<UserTask>> GetReset(long userId)
        {
            if (userId == 0)
            {
                userId = AbpSession.UserId == null ? 0 : AbpSession.UserId.Value;
            }

            var node = from n in _nodeInstanceRepository.GetAll()
                       join w in _workflowInstanceRepository.GetAll() on n.WorkFlowInstanceId equals w.Id
                       join t in _taskResetRepository.GetAll() on n.Id equals t.NodeInstanceId
                       where t.TargetUserId == userId && t.IsActive
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
                       };
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
        public async virtual Task<PagedResultDto<UserTask>> GetTaskByUserId(long userId)
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
                .Where(p => p.w.WorkflowState == WorkflowState.Running && p.n.NodeInstanceStatus == NodeInstanceStatus.Current);

            //// 先初步过滤一遍角色Id
            //if (roleIds.Any())
            //{
            //    roleIds.ForEach(o => { node = node.Where(p => p.n.RoleId.Contains(o)); });
            //}
            //else
            //{
            //    node = node.Where(p => false);
            //}

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


            if (roleIds.Any())
            {
                dto = dto.Where(p => p.RoleId.ToHashSet().Overlaps(roleIds))//判断两个集合是否存在交集
                                                                            //.Where(p => p.NodeType == NodeType.End && p.OperatedUserIds.StrToList().Select(o => o.To<long>()).Contains(AbpSession.UserId.Value));
                 .Where(p => p.NodeType != NodeType.End || p.OperatedUserIds.IsNullOrWhiteSpace() || !p.OperatedUserIds.StrToList().Select(o => o.To<long>()).Contains(AbpSession.UserId.Value));

                //foreach (var o in roleIds)
                //{
                //    dto = dto.Where(p => p.RoleId.Contains(o));
                //}
                //roleIds.ForEach(o => { dto = dto.Where(p => p.RoleId.Contains(o)); });
            }
            else
            {
                dto = dto.Where(p => false);
            }
            //var dfgd = dto is null;
            //var dfddfdf = dto.Count();

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

                       where (p != null && p.ProjectManager == AbpSession.UserId) || (h.CreatorUserId == AbpSession.UserId)


                       || n.Name == "贸易合规"

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
                           ProcessIdentifier = n.ProcessIdentifier
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
            var nodeInstance = await _nodeInstanceRepository.GetAsync(nodeInstanceId);

            var data = from l in _lineInstanceRepository.GetAll()
                       join n in _nodeInstanceRepository.GetAll() on l.TargetNodeId equals n.NodeId
                       where l.SoureNodeId == nodeInstance.NodeId && l.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId
                       && n.WorkFlowInstanceId == nodeInstance.WorkFlowInstanceId && l.LineType != LineType.Reset && l.FinanceDictionaryDetailId != FinanceConsts.YesOrNo_No
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

            #endregion

            #region 同意

            //审批意见集合有且仅有同意
            if (input.FinanceDictionaryDetailIds.Count == 1 && input.FinanceDictionaryDetailIds.Contains(FinanceConsts.HjkbSelect_Yes))
            {
                //正常调用流程流转接口
                await SubmitNodeInterfece(new SubmitNodeInput
                {
                    Comment = input.Comment,
                    NodeInstanceId = input.NodeInstanceId,
                    FinanceDictionaryDetailId = input.FinanceDictionaryDetailIds[0]
                });
            }


            #endregion

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


        /// <summary>
        /// 获取普通核报价归档流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<PagedResultDto<WorkflowInstance>> GetWorkflowOvered(GetWorkflowOveredInput input)
        {
            //普通流程的核价原因
            var hjyy = new List<string> { FinanceConsts.EvalReason_Bb1, FinanceConsts.EvalReason_Fabg, FinanceConsts.EvalReason_Qt, FinanceConsts.EvalReason_Jdcbpg, FinanceConsts.EvalReason_Xmbg, FinanceConsts.EvalReason_Nj };

            var data = (from w in _workflowInstanceRepository.GetAll()
                        join h in _instanceHistoryRepository.GetAll() on w.Id equals h.WorkFlowInstanceId
                        where h.NodeId == "主流程_核价需求录入" && hjyy.Contains(h.FinanceDictionaryDetailId)
                        && w.WorkflowState == WorkflowState.Ended
                        select w)
                        .Distinct()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.Title.Contains(input.Filter));

            var count = await data.CountAsync();

            var paged = data.PageBy(input);

            var result = await paged.ToListAsync();

            return new PagedResultDto<WorkflowInstance>(count, result);
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        /// <returns></returns>
        public async virtual Task OverWorkflow(OverWorkflowInput input)
        {
            var wf = await _workflowInstanceRepository.GetAsync(input.AuditFlowId);
            wf.WorkflowState = WorkflowState.Ended;

            var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == input.AuditFlowId && p.Name == "主流程_归档");
            node.NodeInstanceStatus = NodeInstanceStatus.Current;

            var nodes = await _nodeInstanceRepository.GetAllListAsync(p => p.WorkFlowInstanceId == input.AuditFlowId && p.Name != "主流程_归档" && p.NodeInstanceStatus == NodeInstanceStatus.Current);
            foreach (var item in nodes)
            {
                item.NodeInstanceStatus = NodeInstanceStatus.Over;
            }
        }
    }
}
