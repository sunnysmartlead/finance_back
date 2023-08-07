using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.ProjectManagement;
using Finance.VersionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private long _projectManager = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="auditFlowRepository"></param>
        /// <param name="auditFlowRightRepository"></param>
        /// <param name="auditFinishedProcessRepository"></param>
        /// <param name="flowProcessRepository"></param>
        /// <param name="priceEvaluationRepository"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="analyseBoardAppService"></param>
        /// <param name="priceEvaluationAppService"></param>
        /// <param name="auditCurrentProcessRepository"></param>
        /// <param name="auditFlowDetailRepository"></param>
        /// <param name="userInputInfoRepository"></param>
        public VersionManagmentAppService(IRepository<AuditFlow, long> auditFlowRepository, IRepository<AuditFlowRight, long> auditFlowRightRepository, IRepository<AuditFinishedProcess, long> auditFinishedProcessRepository, IRepository<FlowProcess, long> flowProcessRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<User, long> userRepository, IRepository<Role> roleRepository, IRepository<UserRole, long> userRoleRepository, AnalyseBoardAppService analyseBoardAppService, PriceEvaluationAppService priceEvaluationAppService, IRepository<AuditCurrentProcess, long> auditCurrentProcessRepository, IRepository<AuditFlowDetail, long> auditFlowDetailRepository, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository)
        {
            _auditFlowRepository = auditFlowRepository;
            _auditFlowRightRepository = auditFlowRightRepository;
            _auditFinishedProcessRepository = auditFinishedProcessRepository;
            _flowProcessRepository = flowProcessRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _modelCountRepository = modelCountRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _analyseBoardAppService = analyseBoardAppService;
            _priceEvaluationAppService = priceEvaluationAppService;
            _auditCurrentProcessRepository = auditCurrentProcessRepository;
            _auditFlowDetailRepository = auditFlowDetailRepository;
            _userInputInfoRepository = userInputInfoRepository;
            _financeDictionaryDetailRepository= financeDictionaryDetailRepository;  
        }

        /// <summary>
        /// 获取系统版本
        /// </summary>
        /// <param name="versionFilterInput"></param>
        /// <returns></returns>
        public async virtual Task<VersionManageListDto> GetVersionInfos(VersionFilterInputDto versionFilterInput)
        {
            VersionManageListDto versionManageListDto = new();
            List<VersionManageDto> versionManages = new();

            List<long> lastFlowIds = new();

            //通过流程开始时间查询到的流程Id
            List<long> flowIdsByDraftTime = new();
            //通过流程结束时间查询到的流程Id
            List<long> flowIdsByFinishedTime = new();
            //通过项目名称查询到的流程Id
            List<long> flowIdsByProject = new();
            //通过流程编号查询到的流程Id
            List<long> flowIdByNumber = new();
            //输入的流程Id
            List<long> flowIdByInput = new();

            try
            {
                if (versionFilterInput.DraftStartTime != null || versionFilterInput.DraftEndTime != null)
                {
                    flowIdsByDraftTime = await this.GetAllAuditFlowByDraftTime(versionFilterInput);
                }

                if (versionFilterInput.FinishedStartTime != null || versionFilterInput.FinishedEndTime != null)
                {
                    flowIdsByFinishedTime = await this.GetAllAuditFlowByFinishedTime(versionFilterInput);
                }

                if (versionFilterInput.ProjectName != null)
                {
                    List<int> versions;
                    if (versionFilterInput.Version == 0)
                    {
                        versions = await this.GetAllAuditFlowVersion(versionFilterInput.ProjectName);
                        foreach (var version in versions)
                        {
                            versionFilterInput.Version = version;
                            flowIdsByProject.Add(await this.GetAuditFlowIdByVersion(versionFilterInput));
                        }
                    }
                    else
                    {
                        flowIdsByProject.Add(await this.GetAuditFlowIdByVersion(versionFilterInput));
                    }
                }

                if (versionFilterInput.Number != null)
                {
                    var flowInfos = await _priceEvaluationRepository.GetAllListAsync(p => p.Number == versionFilterInput.Number);
                    if (flowInfos.Count > 0)
                    {
                        flowIdByNumber.Add(flowInfos.FirstOrDefault().AuditFlowId);
                    }
                }

                if(versionFilterInput.AuditFlowId > 0)
                {
                    flowIdByInput.Add(versionFilterInput.AuditFlowId);
                }

                //判断是否有时间条件或项目名称条件获得得Id
                if (flowIdsByProject.Count > 0)
                {
                    lastFlowIds = flowIdsByProject;
                }

                if (flowIdsByDraftTime.Count > 0 && lastFlowIds.Count > 0)
                {
                    lastFlowIds = flowIdsByDraftTime.Intersect(lastFlowIds).ToList();
                }
                else if(flowIdsByDraftTime.Count > 0)
                {
                    lastFlowIds = flowIdsByDraftTime;
                }

                if (flowIdsByFinishedTime.Count > 0 && lastFlowIds.Count > 0)
                {
                    lastFlowIds = flowIdsByFinishedTime.Intersect(lastFlowIds).ToList(); 
                }
                else if (flowIdsByFinishedTime.Count > 0)
                {
                    lastFlowIds = flowIdsByFinishedTime;
                }

                //判断单据编号获得的Id 是否不等于0
                if (lastFlowIds.Count > 0 && flowIdByNumber.Count > 0)
                {
                    lastFlowIds = lastFlowIds.Intersect(flowIdByNumber).ToList();
                }
                else if (flowIdByNumber.Count > 0)
                {
                    lastFlowIds = flowIdByNumber;
                }

                //判断输入的流程Id 是否不等于0
                if (lastFlowIds.Count > 0 && flowIdByInput.Count > 0)
                {
                    lastFlowIds = lastFlowIds.Intersect(flowIdByInput).ToList();
                }
                else if (flowIdByInput.Count > 0)
                {
                    lastFlowIds = flowIdByInput;
                }

                if (lastFlowIds.Count > 0)
                {
                    foreach (var flowId in lastFlowIds)
                    {
                        string flowTitle = null;
                        var flow = await _auditFlowRepository.SingleAsync(p => p.Id == flowId);
                        //if(flow.LastModificationTime is null)
                        //{
                        //    throw new FriendlyException("核价还未完成");
                        //}
                        VersionManageDto versionManage = new VersionManageDto();
                        var priceInfo = await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == flowId);

                        var priceEvaluations = await _priceEvaluationRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
                        if (priceEvaluations.Count > 0)
                        {
                            flowTitle = priceEvaluations.FirstOrDefault().Title;
                        }
                        else
                        {
                            throw new FriendlyException("找不到对应核价需求信息！");
                        }
                        long projectManagerId = _projectManager == 0 ? priceEvaluations.FirstOrDefault().ProjectManager : _projectManager;
                        User usrInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == projectManagerId);                    
                        string quoteType = priceEvaluations.FirstOrDefault().QuotationType;
                        string quoteTypeName = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == quoteType).DisplayName;
                        versionManage.VersionBasicInfo = new()
                        {
                            AuditFlowId = flowId,
                            ProjectName = flow.QuoteProjectName,
                            Version = flow.QuoteVersion,
                            Number = priceInfo.Number,
                            ProjectManager =usrInfo != null? usrInfo.Name:"",
                            QuoteTypeName=quoteTypeName,
                            DraftTime = flow.CreationTime,
                            FinishedTime = flow.LastModificationTime
                        };
                        if (flow.LastModificationTime is null)
                        {
                            versionManage.PriceEvaluationTableList = null;
                        }
                        else
                        { 
                            versionManage.PriceEvaluationTableList = new();

                            var modelList = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
                            GetPriceEvaluationTableResultInput resultInput = new();
                            foreach (var productId in modelList)
                            {
                                resultInput.AuditFlowId = flowId;
                                resultInput.ProductId = productId.Id;
                                resultInput.IsAll = false;
                                versionManage.PriceEvaluationTableList.Add(await _priceEvaluationAppService.GetPriceEvaluationTableResult(resultInput));

                                resultInput.IsAll = true;
                                versionManage.PriceEvaluationTableList.Add(await _priceEvaluationAppService.GetPriceEvaluationTableResult(resultInput));
                            }
                            versionManage.QuotationTable = await _analyseBoardAppService.GetQuotationList(flowId);
                        }


                        versionManages.Add(versionManage);
                    }
                }
                versionManageListDto.VersionManageList = versionManages;
            }
            catch (Exception ex)
            {
                throw new FriendlyException("获取核价表版本信息失败，原因：" + ex.Message);
            }
            return versionManageListDto;
        }

        /// <summary>
        /// 获取系统版本操作记录
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public async virtual Task<List<AuditFlowOperateReocrdDto>> GetAuditFlowOperateReocrd(long flowId)
        {
            string projectName = null;
            int version = 0;
            var flowList = await _auditFlowRepository.GetAllListAsync(p => p.Id == flowId);
            if (flowList.Count > 0)
            {
                projectName = (await _auditFlowRepository.SingleAsync(p => p.Id == flowId)).QuoteProjectName;
                version = (await _auditFlowRepository.SingleAsync(p => p.Id == flowId)).QuoteVersion;
                List<FlowProcess> allProcesses = await _flowProcessRepository.GetAllListAsync();
                List<AuditFlowOperateReocrdDto> auditFlowOperates = (from a in await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowId)
                                                                     join c in await _userRepository.GetAllListAsync() on a.UserId equals c.Id
                                                                     join d in await _userRoleRepository.GetAllListAsync() on c.Id equals d.UserId
                                                                     join e in await _roleRepository.GetAllListAsync() on d.RoleId equals e.Id
                                                                     join f in allProcesses on a.ProcessIdentifier equals f.ProcessIdentifier
                                                                     where e.Name == f.EditRole
                                                                     select new AuditFlowOperateReocrdDto
                                                                     {
                                                                         ProjectName = projectName,
                                                                         Version = version,
                                                                         ProcessName = f.ProcessName,
                                                                         ProcessState = (PROCESSTYPE)a.RightType,
                                                                         UserName = c.Name,
                                                                         RoleName = e.DisplayName,
                                                                         RequiredTime = this.GetRequiredTime(flowId, a.ProcessIdentifier),
                                                                         auditFlowOperateTimes = (a.ProcessIdentifier == AuditFlowConsts.AF_RequirementInput)? //判断是否是第一个界面，第一个界面的创建时间和完成时间可以直接取自权限表
                                                                         new List<AuditFlowOperateTime>{ new AuditFlowOperateTime() { StartTime = a.CreationTime, LastModifyTime = a.CreationTime } } 
                                                                         : 
                                                                         (from b in _auditFlowDetailRepository.GetAllList(p => p.AuditFlowId == flowId && p.ReceiveProcessIdentifier == a.ProcessIdentifier && p.ReceiverId == a.UserId)
                                                                            select new AuditFlowOperateTime
                                                                            {
                                                                                StartTime = b.CreationTime,
                                                                                LastModifyTime = this.GetLastModifyTime(flowId, a.ProcessIdentifier, a.UserId, b.CreationTime)
                                                                            }).OrderBy(p => p.StartTime).ToList()
                                                                     }).ToList();

                List<string> allProcessIdentifiers = (from a in allProcesses.Select(p => p.ProcessIdentifier).Distinct() orderby a ascending select a).ToList();

                List<AuditFlowRight> runProcesses = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
                List<string> runProcessIdentifiers = (from a in runProcesses.Select(p => p.ProcessIdentifier).Distinct() orderby a ascending select a).ToList();

                //去除掉权限表中的流程（取剩余流程）
                allProcessIdentifiers.RemoveAll(it => runProcessIdentifiers.Contains(it));

                List<AuditFlowOperateReocrdDto> auditFlowOtherOperates = (from a in allProcessIdentifiers
                                                                          join b in allProcesses on a equals b.ProcessIdentifier
                                                                          select new AuditFlowOperateReocrdDto
                                                                     {
                                                                         ProjectName = projectName,
                                                                         Version = version,
                                                                         ProcessName = b.ProcessName,
                                                                         ProcessState = PROCESSTYPE.ProcessNoStart,
                                                                         UserName = null,
                                                                         RoleName = null,
                                                                         RequiredTime = this.GetRequiredTime(flowId, b.ProcessIdentifier),
                                                                         auditFlowOperateTimes = 
                                                                         (from f in _auditFlowDetailRepository.GetAllList(p => p.AuditFlowId == flowId && p.ReceiveProcessIdentifier == b.ProcessIdentifier)
                                                                          join c in _roleRepository.GetAllList() on b.EditRole equals c.Name
                                                                          join d in _userRoleRepository.GetAllList() on c.Id equals d.RoleId
                                                                          join e in _userRepository.GetAllList() on d.UserId equals e.Id
                                                                          where f.ReceiverId == e.Id
                                                                          select new AuditFlowOperateTime
                                                                          {
                                                                              StartTime = f.CreationTime,
                                                                              LastModifyTime = this.GetLastModifyTime(flowId, b.ProcessIdentifier, e.Id, f.CreationTime)
                                                                          }).OrderBy(p => p.StartTime).ToList()
                                                                     }).ToList();
                auditFlowOperates.AddRange(auditFlowOtherOperates);
                return auditFlowOperates;
            }
            else
            {
                throw new FriendlyException("获取核报价流程失败");
            }
        }

        /// <summary>
        /// 获取界面期望完成时间
        /// </summary>
        /// <param name="getInferaceRequiredTimeDto"></param>
        /// <returns></returns>
        public Task<DateTime?> GetInterfaceRequiredTime(GetInferaceRequiredTimeDto getInferaceRequiredTimeDto)
        {
            DateTime? requiredTime = null;
            if(getInferaceRequiredTimeDto != null)
            {
                requiredTime = this.GetRequiredTime(getInferaceRequiredTimeDto.AuditFlowId, getInferaceRequiredTimeDto.ProcessIdentifier);
            }
            return Task.FromResult(requiredTime);
        }

        internal DateTime? GetRequiredTime(long flowId, string processIdentifier)
        {
            DateTime? requiredTime = null;

            var userInputInfo = _userInputInfoRepository.FirstOrDefault(p => p.AuditFlowId == flowId);
            if(userInputInfo != null)
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

            var flowDetailList = _auditFlowDetailRepository.GetAllList(p => p.AuditFlowId == flowId && p.ProcessIdentifier == processIdentifier && p.UserId ==  userId);

            foreach (var flowDetail in flowDetailList)
            {
                if(lastModifyTime == null && flowDetail.CreationTime > startTime)
                {
                    lastModifyTime = flowDetail.CreationTime;
                }
                else if(lastModifyTime != null)
                {
                    if(lastModifyTime.Value > startTime && flowDetail.CreationTime > startTime && lastModifyTime.Value > flowDetail.CreationTime)
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
        /// 获取项目已有核价流程所有项目名称和项目代码以及对应版本号
        /// </summary>
        public async virtual Task<List<ProjectNameAndVersionDto>> GetAllAuditFlowProjectNameAndVersion()
        {
            List<ProjectNameAndVersionDto> projectNameAndVersions = new List<ProjectNameAndVersionDto>();
            var projectNames = (from a in _auditFlowRepository.GetAll().Select(p => p.QuoteProjectName).Distinct() select a).ToList();
            foreach (var name in projectNames)
            {
                var versions = await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == name);
                projectNameAndVersions.Add(new ProjectNameAndVersionDto()
                {
                    ProjectName = name,
                    ProjectNumber = (await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == name)).FirstOrDefault().QuoteProjectNumber,
                    Versions = (from e in versions.Select(p => p.QuoteVersion).Distinct() orderby e ascending select e).ToList()
                });
            }
            return projectNameAndVersions;
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
