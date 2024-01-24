using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.ProjectManagement;
using Finance.VersionManagement.Dto;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Ext;
using Z.EntityFramework.Plus;
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.Identity;
using Abp.Collections.Extensions;

namespace Finance.VersionManagement
{
    /// <summary>
    /// 系统版本管理表后端接口类
    /// </summary>
    public class VersionManagmentAppService : FinanceAppServiceBase
    {
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IRepository<AuditFlowRight, long> _auditFlowRightRepository;
        private readonly IRepository<AuditCurrentProcess, long> _auditCurrentProcessRepository;
        private readonly IRepository<AuditFinishedProcess, long> _auditFinishedProcessRepository;
        private readonly IRepository<AuditFlowDetail, long> _auditFlowDetailRepository;
        private readonly IRepository<FlowProcess, long> _flowProcessRepository;
        private readonly IRepository<UserInputInfo, long> _userInputInfoRepository;

        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;

        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        private readonly AnalyseBoardAppService _analyseBoardAppService;
        private readonly PriceEvaluationAppService _priceEvaluationAppService;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        private readonly IRepository<WorkflowInstance, long> _workflowInstanceRepository;
        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;
        private readonly IRepository<LineInstance, long> _lineInstanceRepository;
        private readonly IRepository<InstanceHistory, long> _instanceHistoryRepository;
        private readonly AuditFlowAppService _auditFlowAppService;
        private readonly IRepository<PricingTeam, long> _pricingTeamRepository;
        private readonly IRepository<TaskReset, long> _taskResetRepository;
        private readonly IRepository<NodeTime, long> _nodeTimeRepository;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;


        private long _projectManager = 0;

        public VersionManagmentAppService(IRepository<AuditFlow, long> auditFlowRepository, IRepository<AuditFlowRight, long> auditFlowRightRepository, IRepository<AuditCurrentProcess, long> auditCurrentProcessRepository, IRepository<AuditFinishedProcess, long> auditFinishedProcessRepository, IRepository<AuditFlowDetail, long> auditFlowDetailRepository, IRepository<FlowProcess, long> flowProcessRepository, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<User, long> userRepository, IRepository<Role> roleRepository, IRepository<UserRole, long> userRoleRepository, AnalyseBoardAppService analyseBoardAppService, PriceEvaluationAppService priceEvaluationAppService, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<WorkflowInstance, long> workflowInstanceRepository, IRepository<NodeInstance, long> nodeInstanceRepository, IRepository<LineInstance, long> lineInstanceRepository, IRepository<InstanceHistory, long> instanceHistoryRepository, AuditFlowAppService auditFlowAppService, IRepository<PricingTeam, long> pricingTeamRepository, IRepository<TaskReset, long> taskResetRepository, IRepository<NodeTime, long> nodeTimeRepository, WorkflowInstanceAppService workflowInstanceAppService)
        {
            _auditFlowRepository = auditFlowRepository;
            _auditFlowRightRepository = auditFlowRightRepository;
            _auditCurrentProcessRepository = auditCurrentProcessRepository;
            _auditFinishedProcessRepository = auditFinishedProcessRepository;
            _auditFlowDetailRepository = auditFlowDetailRepository;
            _flowProcessRepository = flowProcessRepository;
            _userInputInfoRepository = userInputInfoRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _modelCountRepository = modelCountRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _analyseBoardAppService = analyseBoardAppService;
            _priceEvaluationAppService = priceEvaluationAppService;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _nodeInstanceRepository = nodeInstanceRepository;
            _lineInstanceRepository = lineInstanceRepository;
            _instanceHistoryRepository = instanceHistoryRepository;
            _auditFlowAppService = auditFlowAppService;
            _pricingTeamRepository = pricingTeamRepository;
            _taskResetRepository = taskResetRepository;
            _nodeTimeRepository = nodeTimeRepository;
            _workflowInstanceAppService = workflowInstanceAppService;
        }


        /// <summary>
        /// 获取系统版本
        /// </summary>
        /// <param name="versionFilterInput"></param>
        /// <returns></returns>
        public async virtual Task<VersionManageListDto> GetVersionInfos(VersionFilterInputDto versionFilterInput)
        {
            var data = (from p in _priceEvaluationRepository.GetAll()
                        join u in _userRepository.GetAll() on p.ProjectManager equals u.Id
                        join n in _nodeInstanceRepository.GetAll() on p.AuditFlowId equals n.WorkFlowInstanceId

                        join f in _financeDictionaryDetailRepository.GetAll() on n.FinanceDictionaryDetailId equals f.Id into f1
                        from f2 in f1.DefaultIfEmpty()
                            //join w in _workflowInstanceRepository.GetAll() on p.AuditFlowId equals w.Id
                        where
                        //w.WorkflowState == WorkflowState.Running &&
                        n.NodeId == "主流程_核价需求录入"
                        select new VersionBasicInfoDto
                        {
                            AuditFlowId = p.AuditFlowId,
                            ProjectName = p.ProjectName,
                            Version = p.QuoteVersion,
                            Number = p.Number,
                            ProjectManager = u.Name,
                            QuoteTypeName = f2 == null ? string.Empty : f2.DisplayName,
                            DraftTime = p.DraftDate,
                            //FinishedTime = p.FinishedTime
                        }).WhereIf(versionFilterInput.Version != default, p => p.Version == versionFilterInput.Version)
                        .WhereIf(versionFilterInput.DraftStartTime.HasValue, p => p.DraftTime >= versionFilterInput.DraftStartTime)
                        .WhereIf(versionFilterInput.DraftEndTime.HasValue, p => p.DraftTime <= versionFilterInput.DraftEndTime)
                        .WhereIf(versionFilterInput.AuditFlowId != default, p => p.AuditFlowId == versionFilterInput.AuditFlowId)
                        .WhereIf(versionFilterInput.ProjectName != default, p => p.ProjectName == versionFilterInput.ProjectName)
                        .WhereIf(versionFilterInput.Number != default, p => p.Number == versionFilterInput.Number);

            var result = await data.ToListAsync();

            var dto = result.Select(p => new VersionManageDto { VersionBasicInfo = p });
            return new VersionManageListDto
            {
                VersionManageList = dto.ToList(),
                IsSuccess = true,
                Message = "查询成功"
            };
        }

        /// <summary>
        /// 获取系统版本（自己是项目经理的）
        /// </summary>
        /// <param name="versionFilterInput"></param>
        /// <returns></returns>
        public async virtual Task<VersionManageListDto> GetVersionInfosSelf(VersionFilterInputDto versionFilterInput)
        {
            var data = (from p in _priceEvaluationRepository.GetAll()
                        join u in _userRepository.GetAll() on p.ProjectManager equals u.Id
                        join n in _nodeInstanceRepository.GetAll() on p.AuditFlowId equals n.WorkFlowInstanceId
                        join f in _financeDictionaryDetailRepository.GetAll() on n.FinanceDictionaryDetailId equals f.Id
                        join w in _workflowInstanceRepository.GetAll() on p.AuditFlowId equals w.Id
                        where w.WorkflowState == WorkflowState.Running
                        && n.NodeId == "主流程_核价需求录入" && p.ProjectManager == AbpSession.UserId
                        select new VersionBasicInfoDto
                        {
                            AuditFlowId = p.AuditFlowId,
                            ProjectName = p.ProjectName,
                            Version = p.QuoteVersion,
                            Number = p.Number,
                            ProjectManager = u.Name,
                            QuoteTypeName = f.DisplayName,
                            DraftTime = p.DraftDate,
                            //FinishedTime = p.FinishedTime
                        }).WhereIf(versionFilterInput.Version != default, p => p.Version == versionFilterInput.Version)
                        .WhereIf(versionFilterInput.DraftStartTime.HasValue, p => p.DraftTime >= versionFilterInput.DraftStartTime)
                        .WhereIf(versionFilterInput.DraftEndTime.HasValue, p => p.DraftTime <= versionFilterInput.DraftEndTime)
                        .WhereIf(versionFilterInput.AuditFlowId != default, p => p.AuditFlowId == versionFilterInput.AuditFlowId)
                        .WhereIf(versionFilterInput.ProjectName != default, p => p.ProjectName == versionFilterInput.ProjectName)
                        .WhereIf(versionFilterInput.Number != default, p => p.Number == versionFilterInput.Number);

            var result = await data.ToListAsync();

            var dto = result.Select(p => new VersionManageDto { VersionBasicInfo = p });
            return new VersionManageListDto
            {
                VersionManageList = dto.ToList(),
                IsSuccess = true,
                Message = "查询成功"
            };
        }

        /// <summary>
        /// 获取系统版本操作记录
        /// </summary>
        /// <returns></returns>
        public async virtual Task<List<AuditFlowOperateReocrdDto>> GetAuditFlowOperateReocrd(long flowId)
        {
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == flowId);

            var data = (from n in _nodeInstanceRepository.GetAll()

                        join t in _taskResetRepository.GetAll() on n.Id equals t.NodeInstanceId into t1
                        from t2 in t1.DefaultIfEmpty()

                        where n.WorkFlowInstanceId == flowId

                        //根据陈梦瑶在24.01.17给出的需求，这两个节点不再显示在时效性页面中
                        && (n.NodeId != "主流程_开始" || n.NodeId != "主流程_生成报价分析界面选择报价方案")

                        select new ResultDto
                        {
                            ProjectName = priceEvaluation.ProjectName,
                            Version = priceEvaluation.QuoteVersion,
                            ProcessName = n.Name,
                            NodeInstanceStatus = n.NodeInstanceStatus,
                            CreationTime = n.StartTime,
                            LastModificationTime = n.LastModificationTime,
                            RoleId = n.RoleId,
                            WorkFlowInstanceId = n.WorkFlowInstanceId,
                            ProcessIdentifier = n.ProcessIdentifier,
                            ResetTime = t2 == null ? DateTime.MinValue : t2.CreationTime,
                            Id = t2 == null ? 0 : t2.Id,
                            NodeInstanceId = n.Id,
                        });
            var result = await data.ToListAsync();

            var roles = await _roleRepository.GetAllListAsync();

            //获取期望完成时间
            var pricingTeam = await _pricingTeamRepository.FirstOrDefaultAsync(p => p.AuditFlowId == flowId);

            //获取核价团队姓名
            var pricingTeamUser = await GetPricingTeamUser(flowId);

            var isOver = result.First(p => p.ProcessIdentifier == "ArchiveEnd").NodeInstanceStatus == NodeInstanceStatus.Current;

            var nodeTimes = await _nodeTimeRepository.GetAllListAsync(p => p.WorkFlowInstanceId == flowId);


            var dto = (await result.OrderByDescending(p => p.Id).DistinctBy(p => p.NodeInstanceId).SelectAsync(async item => new AuditFlowOperateReocrdDto
            {
                ProcessIdentifier = item.ProcessIdentifier,
                Title = item.ProcessName.GetTitle(),
                TypeName = item.ProcessName.GetTypeName(),
                ProjectName = item.ProjectName,
                Version = item.Version,
                ProcessName = item.ProcessName,
                ProcessState = item.NodeInstanceStatus.ToProcessType(isOver, item.ProcessIdentifier),
                UserName = await GetPricingTeamUserName(item.ProcessIdentifier, pricingTeamUser, flowId, item.NodeInstanceId),
                auditFlowOperateTimes = nodeTimes.Where(p => p.NodeInstance == item.NodeInstanceId).OrderBy(p => p.Id).Select(p => new AuditFlowOperateTime
                {
                    LastModifyTime = p.UpdateTime,
                    StartTime = p.StartTime
                }).ToList(),
                ResetTime = item.ResetTime == DateTime.MinValue ? null : item.ResetTime,
                RoleName = item.RoleId.IsNullOrWhiteSpace() ? string.Empty : string.Join("，", item.RoleId.Split(',').Select(p => roles.FirstOrDefault(o => o.Id == p.To<int>())?.Name)),
                RequiredTime = item.ProcessIdentifier.GetRequiredTime(pricingTeam),

            })).OrderBy(p => p.ProcessName.GetTypeNameSort()).ToList();

            return dto;
        }

        /// <summary>
        /// 获取责任人
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetPricingTeamUserName(string processIdentifier, PricingTeamUser pricingTeamUser, long flowId, long nodeInstanceId)
        {
            if (processIdentifier == "QuoteApproval" || processIdentifier == "ConfirmWinningBid")
            {
                return "张宝忠";
            }
            var allAuditFlowInfos = await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(flowId, nodeInstanceId);
            if (allAuditFlowInfos.IsNullOrEmpty()) { return string.Empty; }
            var allAuditFlowInfo = allAuditFlowInfos.FirstOrDefault();
            if (allAuditFlowInfo == null) { return string.Empty; }
            if (allAuditFlowInfo.TaskUserIds.IsNullOrEmpty()) { return string.Empty; }
            var userNames = await _userRepository.GetAll().Where(p => allAuditFlowInfo.TaskUserIds.Select(x => x.To<long>()).Contains(p.Id)).Select(p => p.Name).ToListAsync();

            return string.Join("，", userNames);
            //if (pricingTeamUser is null)
            //{
            //    return string.Empty;
            //}
            //return processIdentifier switch
            //{
            //    FinanceConsts.ProjectChiefAudit => pricingTeamUser.Audit,
            //    FinanceConsts.COBManufacturingCostEntry => pricingTeamUser.ProductCostInput,
            //    FinanceConsts.NRE_EMCExperimentalFeeInput => pricingTeamUser.EMC,
            //    FinanceConsts.NRE_ReliabilityExperimentFeeInput => pricingTeamUser.QualityBench,
            //    FinanceConsts.FormulaOperationAddition => pricingTeamUser.Engineer,
            //    FinanceConsts.LogisticsCostEntry => pricingTeamUser.ProductManageTime,
            //    "QuoteApproval" => "张宝忠",//硬编码，以免暴露陈梦瑶的账号
            //    FinanceConsts.PricingDemandInput => pricingTeamUser.PriceInput,
            //    FinanceConsts.PriceDemandReview => pricingTeamUser.ProjectManager,

            //    _ => await GetRoleUserName(nodeInstanceId),
            //};
        }

        private async Task<string> GetRoleUserName(long nodeInstanceId)
        {
            var dto = await _nodeInstanceRepository.GetAsync(nodeInstanceId);
            if (dto.RoleId.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            var roles = dto.RoleId.StrToList().Select(p => p.To<long>()).Distinct();
            //根据角色获取用户
            var users = await (from u in _userRepository.GetAll()
                               join ur in _userRoleRepository.GetAll() on u.Id equals ur.UserId
                               //join r in roles on ur.RoleId equals r
                               join r in _roleRepository.GetAll() on ur.RoleId equals r.Id
                               where roles.Contains(r.Id)
                               select new
                               {
                                   r.Id,
                                   u.Name
                               }).ToListAsync();
            return string.Join("，", users.Select(p => p.Name));
        }

        private DateTime? GetLastModifyTime(string processIdentifier, PROCESSTYPE processType, DateTime? creationTime, DateTime? lastModificationTime)
        {
            //如果节点正在进行中，返回空，即没有更新时间
            if (processType == PROCESSTYPE.ProcessRunning)
            {
                return null;
            }

            //如果不是正在进行中，又是核价需求录入。就判断其最后更新时间是否存在，如果存在，返回最后更新时间。不存在，返回创建时间
            if (processIdentifier == FinanceConsts.PricingDemandInput)
            {
                if (lastModificationTime.HasValue)
                {
                    return lastModificationTime;
                }
                else
                {
                    return creationTime;
                }
            }

            //以上条件都不满足，返回最后更新时间
            return lastModificationTime;
        }

        /// <summary>
        /// 获取核价团队人员的姓名
        /// </summary>
        /// <returns></returns>
        private async Task<PricingTeamUser> GetPricingTeamUser(long flowId)
        {
            //获取核价团队
            var pricingTeam = await _pricingTeamRepository.FirstOrDefaultAsync(p => p.AuditFlowId == flowId);

            var engineer = string.Empty;
            var qualityBench = string.Empty;
            var emc = string.Empty;
            var productCostInput = string.Empty;
            var productManageTime = string.Empty;
            var audit = string.Empty;

            if (pricingTeam is not null)
            {
                var fEngineer = _userRepository.GetAll().Where(p => p.Id == pricingTeam.EngineerId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();
                var fQualityBench = _userRepository.GetAll().Where(p => p.Id == pricingTeam.QualityBenchId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();
                var fEmc = _userRepository.GetAll().Where(p => p.Id == pricingTeam.EMCId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();
                var fProductCostInput = _userRepository.GetAll().Where(p => p.Id == pricingTeam.ProductCostInputId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();
                var fProductManageTime = _userRepository.GetAll().Where(p => p.Id == pricingTeam.ProductManageTimeId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();
                var fAudit = _userRepository.GetAll().Where(p => p.Id == pricingTeam.AuditId).Select(p => p.Name).DeferredFirstOrDefault().FutureValue();

                engineer = fEngineer.Value;
                qualityBench = fQualityBench.Value;
                emc = fEmc.Value;
                productCostInput = fProductCostInput.Value;
                productManageTime = fProductManageTime.Value;
                audit = fAudit.Value;
            }

            //获取核价需求录入责任人
            var priceEvaluation = await _priceEvaluationRepository.GetAll().Where(p => p.AuditFlowId == flowId).Select(p => new { p.CreatorUserId, p.ProjectManager }).FirstOrDefaultAsync();
            var priceInput = string.Empty;
            var projectManager = string.Empty;
            if (priceEvaluation is not null)
            {
                var prvaiceInputUser = await _userRepository.FirstOrDefaultAsync(p => p.Id == priceEvaluation.CreatorUserId);
                var ProjectManagerUser = await _userRepository.FirstOrDefaultAsync(p => p.Id == priceEvaluation.ProjectManager);

                priceInput = prvaiceInputUser.Name;
                projectManager = ProjectManagerUser.Name;
            }


            return new PricingTeamUser
            {
                Engineer = engineer,
                QualityBench = qualityBench,
                EMC = emc,
                ProductCostInput = productCostInput,
                ProductManageTime = productManageTime,
                Audit = audit,
                PriceInput = priceInput,
                ProjectManager = projectManager
            };
        }

        /// <summary>
        /// 获取系统版本操作记录（根据流程实例Id）
        /// </summary>
        /// <returns></returns>
        public async virtual Task<List<AuditFlowOperateReocrdDto>> GetAuditFlowOperateReocrdByNodeInstanceId(long flowId, long nodeInstanceId)
        {
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == flowId);

            var data = from n in _nodeInstanceRepository.GetAll()

                       join i in _instanceHistoryRepository.GetAll() on n.Id equals i.NodeInstanceId into i1
                       from i2 in i1.DefaultIfEmpty()

                       join u in _userRepository.GetAll() on i2.CreatorUserId equals u.Id into u1
                       from u2 in u1.DefaultIfEmpty()

                       join t in _taskResetRepository.GetAll() on n.Id equals t.NodeInstanceId into t1
                       from t2 in t1.DefaultIfEmpty()

                       where n.WorkFlowInstanceId == flowId && n.Id == nodeInstanceId

                       select new// AuditFlowOperateReocrdDto
                       {
                           ProjectName = priceEvaluation.ProjectName,
                           Version = priceEvaluation.QuoteVersion,
                           ProcessName = n.Name,
                           //ProcessState = n.NodeInstanceStatus,
                           n.NodeInstanceStatus,
                           UserName = u2.Name,
                           //RoleName = 
                           n.CreationTime,
                           n.LastModificationTime,
                           n.RoleId,
                           n.WorkFlowInstanceId,
                           n.ProcessIdentifier,
                           ResetTime = t2 == null ? DateTime.MinValue : t2.CreationTime
                       };
            var result = await data.ToListAsync();

            var roles = await _roleRepository.GetAllListAsync();

            var auditFlowOperateReocrdDto = new List<AuditFlowOperateReocrdDto>();
            foreach (var item in result)
            {
                var dto = new AuditFlowOperateReocrdDto
                {
                    Title = item.ProcessName.GetTitle(),
                    TypeName = item.ProcessName.GetTypeName(),
                    ProjectName = item.ProjectName,
                    Version = item.Version,
                    ProcessName = item.ProcessName,
                    ProcessState = item.UserName.IsNullOrWhiteSpace() ? PROCESSTYPE.ProcessNoStart : PROCESSTYPE.ProcessFinished,
                    UserName = item.UserName,
                    auditFlowOperateTimes = new List<AuditFlowOperateTime> { new AuditFlowOperateTime
                    { LastModifyTime = item.LastModificationTime, StartTime =item. CreationTime } },
                    ResetTime = item.ResetTime == DateTime.MinValue ? null : item.ResetTime,
                };
                if (item.NodeInstanceStatus == NodeInstanceStatus.Current)
                {
                    dto.ProcessState = PROCESSTYPE.ProcessRunning;
                }


                if (!item.RoleId.IsNullOrWhiteSpace())
                {
                    var roleNames = string.Join("，", item.RoleId.Split(',').Select(p => roles.FirstOrDefault(o => o.Id == p.To<int>())?.Name));
                    dto.RoleName = roleNames;
                }

                //获取期望完成时间
                var pricingTeam = await _pricingTeamRepository.FirstOrDefaultAsync(p => p.AuditFlowId == item.WorkFlowInstanceId);
                if (pricingTeam is not null)
                {
                    //dto.ProcessName
                    if (item.ProcessIdentifier == FinanceConsts.ElectronicsBOM)
                    {
                        dto.RequiredTime = pricingTeam.ElecEngineerTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.StructureBOM)
                    {
                        dto.RequiredTime = pricingTeam.StructEngineerTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.NRE_EMCExperimentalFeeInput)
                    {
                        dto.RequiredTime = pricingTeam.EMCTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.NRE_ReliabilityExperimentFeeInput)
                    {
                        dto.RequiredTime = pricingTeam.QualityBenchTime;
                    }
                    if (item.ProcessIdentifier == "ElectronicUnitPriceEntry")
                    {
                        dto.RequiredTime = pricingTeam.ResourceElecTime;
                    }
                    if (item.ProcessIdentifier == "StructureUnitPriceEntry")
                    {
                        dto.RequiredTime = pricingTeam.ResourceStructTime;
                    }
                    if (item.ProcessIdentifier == "NRE_MoldFeeEntry")
                    {
                        dto.RequiredTime = pricingTeam.MouldWorkHourTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.FormulaOperationAddition)
                    {
                        dto.RequiredTime = pricingTeam.EngineerWorkHourTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.LogisticsCostEntry)
                    {
                        dto.RequiredTime = pricingTeam.ProductManageTime;
                    }
                    if (item.ProcessIdentifier == FinanceConsts.COBManufacturingCostEntry)
                    {
                        dto.RequiredTime = pricingTeam.ProductCostInputTime;
                    }
                }

                auditFlowOperateReocrdDto.Add(dto);
            }


            return auditFlowOperateReocrdDto.OrderBy(p => p.ProcessName.GetTypeNameSort()).ToList();
        }

        /// <summary>
        /// 获取界面期望完成时间
        /// </summary>
        /// <param name="getInferaceRequiredTimeDto"></param>
        /// <returns></returns>
        public Task<DateTime?> GetInterfaceRequiredTime(GetInferaceRequiredTimeDto getInferaceRequiredTimeDto)
        {
            DateTime? requiredTime = null;
            if (getInferaceRequiredTimeDto != null)
            {
                requiredTime = this.GetRequiredTime(getInferaceRequiredTimeDto.AuditFlowId, getInferaceRequiredTimeDto.ProcessIdentifier);
            }
            return Task.FromResult(requiredTime);
        }

        internal DateTime? GetRequiredTime(long flowId, string processIdentifier)
        {
            DateTime? requiredTime = null;

            var userInputInfo = _userInputInfoRepository.FirstOrDefault(p => p.AuditFlowId == flowId);
            if (userInputInfo != null)
            {
                if (processIdentifier == AuditFlowConsts.AF_ElectronicBomImport || processIdentifier == AuditFlowConsts.AF_ElectronicBomAudit || processIdentifier == AuditFlowConsts.AF_NreInputEmc)
                {
                    requiredTime = userInputInfo.ElecEngineerTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_StructBomImport || processIdentifier == AuditFlowConsts.AF_StructBomAudit)
                {
                    requiredTime = userInputInfo.StructEngineerTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_NreInputTest)
                {
                    requiredTime = userInputInfo.QualityBenchTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_NreInputGage)
                {
                    requiredTime = userInputInfo.QualityToolTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_ElectronicPriceInput || processIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit)
                {
                    requiredTime = userInputInfo.ResourceElecTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_StructPriceInput || processIdentifier == AuditFlowConsts.AF_StructBomPriceAudit || processIdentifier == AuditFlowConsts.AF_NreInputMould)
                {
                    requiredTime = userInputInfo.ResourceStructTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_ElecLossRateInput || processIdentifier == AuditFlowConsts.AF_StructLossRateInput)
                {
                    requiredTime = userInputInfo.EngineerLossRateTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_ManHourImport)
                {
                    requiredTime = userInputInfo.EngineerWorkHourTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_LogisticsCostInput)
                {
                    requiredTime = userInputInfo.ProductManageTime;
                }
                else if (processIdentifier == AuditFlowConsts.AF_ProductionCostInput)
                {
                    requiredTime = userInputInfo.ProductCostInputTime;
                }
            }

            return requiredTime;
        }

        internal DateTime? GetLastModifyTime(long flowId, string processIdentifier, long userId, DateTime startTime)
        {
            DateTime? lastModifyTime = null;

            var flowDetailList = _auditFlowDetailRepository.GetAllList(p => p.AuditFlowId == flowId && p.ProcessIdentifier == processIdentifier && p.UserId == userId);

            foreach (var flowDetail in flowDetailList)
            {
                if (lastModifyTime == null && flowDetail.CreationTime > startTime)
                {
                    lastModifyTime = flowDetail.CreationTime;
                }
                else if (lastModifyTime != null)
                {
                    if (lastModifyTime.Value > startTime && flowDetail.CreationTime > startTime && lastModifyTime.Value > flowDetail.CreationTime)
                    {
                        lastModifyTime = flowDetail.CreationTime;
                    }
                }
            }

            return lastModifyTime;
        }

        /// <summary>
        /// 根据拟稿时间获取该时间段内所有核价流程ID
        /// </summary>
        public async virtual Task<List<long>> GetAllAuditFlowByDraftTime(VersionFilterInputDto auditFlowTimeRequest)
        {
            if (auditFlowTimeRequest.DraftStartTime == null)
            {
                auditFlowTimeRequest.DraftStartTime = DateTime.MinValue;
            }
            if (auditFlowTimeRequest.DraftEndTime == null)
            {
                auditFlowTimeRequest.DraftEndTime = DateTime.Now;
            }

            var flowInfos = await _auditFlowRepository.GetAll()
                                .Where(p => p.CreationTime > auditFlowTimeRequest.DraftStartTime && p.CreationTime < auditFlowTimeRequest.DraftEndTime)
                                .OrderBy(p => p.Id)
                                .ToListAsync();

            List<long> AuditFlowIdList = (from a in flowInfos.Select(p => p.Id).Distinct() select a).ToList();
            return AuditFlowIdList;
        }

        /// <summary>
        /// 根据完成时间获取该时间段内所有核价流程ID
        /// </summary>
        public async virtual Task<List<long>> GetAllAuditFlowByFinishedTime(VersionFilterInputDto auditFlowTimeRequest)
        {
            if (auditFlowTimeRequest.FinishedStartTime == null)
            {
                auditFlowTimeRequest.FinishedStartTime = DateTime.MinValue;
            }
            if (auditFlowTimeRequest.FinishedEndTime == null)
            {
                auditFlowTimeRequest.FinishedEndTime = DateTime.Now;
            }

            var flowInfos = await _auditFlowRepository.GetAll()
                                .Where(p => p.CreationTime > auditFlowTimeRequest.FinishedStartTime && p.CreationTime < auditFlowTimeRequest.FinishedEndTime)
                                .Where(p => p.IsValid)
                                .OrderBy(p => p.Id)
                                .ToListAsync();

            List<long> AuditFlowIdList = (from a in flowInfos.Select(p => p.Id).Distinct() select a).ToList();
            return AuditFlowIdList;
        }


        /// <summary>
        /// 获取项目已有核价流程所有项目名称
        /// </summary>
        public async virtual Task<List<string>> GetAllAuditFlowProjectName()
        {
            var flowInfos = await _auditFlowRepository.GetAllListAsync();

            List<string> auditFlowProjectNameList = (from a in flowInfos.Select(p => p.QuoteProjectName).Distinct() select a).ToList();
            return auditFlowProjectNameList;
        }

        /// <summary>
        /// 根据项目名称获取项目已有核价流程所有版本
        /// </summary>
        public async virtual Task<List<int>> GetAllAuditFlowVersion(string projectName)
        {
            var flowInfos = await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == projectName);

            List<int> allVersion = (from a in flowInfos.Select(p => p.QuoteVersion).Distinct() orderby a ascending select a).ToList();
            return allVersion;
        }

        /// <summary>
        /// 获取项目已有核价流程所有项目名称和项目代码以及对应版本号（获取自己有的）
        /// </summary>
        public async virtual Task<List<ProjectNameAndVersionDto>> GetAllAuditFlowProjectNameAndVersionBySelf()
        {
            //var task = (await _auditFlowAppService.GetAllAuditFlowInfosByTask());

            var priceEvaluations = await _priceEvaluationRepository.GetAll().OrderByDescending(p => p.Id).ToListAsync();

            var data = (from p in priceEvaluations
                        where p.ProjectManager == AbpSession.UserId
                        group p by new { p.ProjectCode, p.ProjectName, p.AuditFlowId } into g
                        select new ProjectNameAndVersionDto
                        {
                            ProjectName = g.Key.ProjectName,
                            ProjectNumber = g.Key.ProjectCode,
                            Versions = g.Select(p => p.QuoteVersion).ToList(),
                            AuditFlowId = g.Key.AuditFlowId
                        })
                        //.Where(p => task.Select(o => o.AuditFlowId).Contains(p.AuditFlowId))
                        ;

            return data.ToList();
        }

        /// <summary>
        /// 获取项目已有核价流程所有项目名称和项目代码以及对应版本号
        /// </summary>
        public async virtual Task<List<ProjectNameAndVersionDto>> GetAllAuditFlowProjectNameAndVersion()
        {
            var priceEvaluations = await _priceEvaluationRepository.GetAll().OrderByDescending(p => p.Id).ToListAsync();

            var data = from p in priceEvaluations
                       group p by new { p.ProjectCode, p.ProjectName } into g
                       select new ProjectNameAndVersionDto
                       {
                           ProjectName = g.Key.ProjectName,
                           ProjectNumber = g.Key.ProjectCode,
                           Versions = g.Select(p => p.QuoteVersion).ToList()
                       };

            return data.ToList();
        }

        /// <summary>
        /// 根据项目名称和版本获取项目核价流程ID
        /// </summary>
        public async virtual Task<long> GetAuditFlowIdByVersion(VersionFilterInputDto input)
        {
            var flowInfos = await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == input.ProjectName && p.QuoteVersion == input.Version);
            if (flowInfos.Count > 0)
            {
                return flowInfos.FirstOrDefault().Id;
            }
            return 0;
        }

        /// <summary>
        /// 获取核价流程ID，返回所有
        /// </summary>
        public async virtual Task<List<long>> GetAllAuditFlowIds()
        {
            var flowInfos = await _auditFlowRepository.GetAllListAsync();

            List<long> auditFlowIdList = (from a in flowInfos.Select(p => p.Id).Distinct() select a).ToList();
            return auditFlowIdList;
        }

        /// <summary>
        /// 根据项目名称返回项目代码准备新建的版本号
        /// </summary>
        public async virtual Task<NewProjectVersionDto> GetAuditFlowNewVersionByProjectName(string projectName)
        {
            NewProjectVersionDto newProjectVersion = new();
            newProjectVersion.ProjectName = projectName;
            var flowInfos = await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == projectName);
            if (flowInfos.Count > 0)
            {
                newProjectVersion.ProjectNumber = flowInfos.FirstOrDefault().QuoteProjectNumber;
                newProjectVersion.NewVersion = flowInfos.Max(p => p.QuoteVersion) + 1;
            }
            else
            {
                newProjectVersion.NewVersion = 1;
            }
            return newProjectVersion;
        }
    }
}
