using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.Users.Dto;
using Finance.WorkFlows.Dto;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Finance.Processes;
using Finance.ProductDevelopment.Dto;
using Newtonsoft.Json;
using Finance.Ext;
using Finance.ProjectManagement;
using static Finance.Ext.FriendlyRequiredAttribute;
using Finance.Authorization.Users;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.Nre;
using Finance.NrePricing.Model;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class LogisticscostAppService : ApplicationService
    {
        private readonly IRepository<Logisticscost, long> _logisticscostRepository;
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<GradientModel, long> _gradientModelRepository;
        private readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
            private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<PriceEvaluation ,long> _priceEvaluationRepository;
        private readonly IRepository<NreIsSubmit, long> _resourceNreIsSubmit;
        /// <summary>
        /// 文件管理接口
        /// </summary>
        private readonly FileCommonService _fileCommonService;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logisticscostRepository"></param>
        public LogisticscostAppService(
            IRepository<Logisticscost, long> logisticscostRepository,
            IRepository<Gradient, long> gradientRepository ,
            IRepository<ModelCount, long> modelCountRepository,
            IRepository<ModelCountYear, long> modelCountYearRepository,
            IRepository<Solution, long> resourceSchemeTable,
            IRepository<GradientModel, long> gradientModelRepository, IRepository<GradientModelYear, long> gradientModelYearRepository,
            WorkflowInstanceAppService workflowInstanceAppService,
            IRepository<PriceEvaluation, long> priceEvaluationRepository,
            FileCommonService fileCommonService,
            IRepository<NreIsSubmit, long> resourceNreIsSubmit
            )
        {
            _logisticscostRepository = logisticscostRepository;
            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _resourceSchemeTable = resourceSchemeTable;
            _workflowInstanceAppService= workflowInstanceAppService;
            _priceEvaluationRepository = priceEvaluationRepository;
            _workflowInstanceAppService= workflowInstanceAppService;
            _fileCommonService = fileCommonService;
            _resourceNreIsSubmit = resourceNreIsSubmit;
        }

        /// <summary>
        /// 详情---无用接口
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);

            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 列表---无用接口
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<LogisticscostDto>> GetListAsync(GetLogisticscostsInput input)
        {
            // 设置查询条件
            var query = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(list, new List<LogisticscostDto>());
            // 数据返回
            return new PagedResultDto<LogisticscostDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<LogisticscostResponseDto>> GetListAllAsync(GetLogisticscostsInput input)
        {

            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);
            var query = (from a in _logisticscostRepository.GetAllList(p =>
            p.IsDeleted == false && p.SolutionId == entity.Id && p.AuditFlowId == input.AuditFlowId
            ).Select(p => p.Classification).Distinct()
                         select a).ToList();
            //获取年度方案
            var year = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
            var dtos = ObjectMapper.Map<List<String>, List<String>>(query, new List<String>());
            List<LogisticscostResponseDto> logisticscostResponseList = new List<LogisticscostResponseDto>();
            foreach (var item in dtos)
            {
                LogisticscostResponseDto logisticscostResponse = new LogisticscostResponseDto();

                var queryItem = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == entity.Id && t.Classification == item).ToList();
                var dtosItem = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(queryItem, new List<LogisticscostDto>());
               
                foreach (var dtosItem1 in dtosItem)
                {
                   
                    ModelCountYear entitySolution = await _modelCountYearRepository.GetAsync((long)dtosItem1.ModelCountYearId);
              

                    dtosItem1.ModelCountYearId = entitySolution.Id;
                    if (entitySolution.UpDown == YearType.FirstHalf)
                    {

                        dtosItem1.Year = entitySolution.Year + "上半年";
                        dtosItem1.Moon = 6;
                    }
                    else if (entitySolution.UpDown == YearType.SecondHalf)
                    {
                        dtosItem1.Year = entitySolution.Year + "下半年";
                        dtosItem1.Moon = 6;
                    }
                    else
                    {
                        dtosItem1.Year = entitySolution.Year.ToString();
                        dtosItem1.Moon = 12;
                    }
                    decimal A = decimal.Parse(item);
                    List<GradientModelYear> GradientModelYearList = (from a in await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.GradientValue == A )
                                                                     join b in await _gradientModelRepository.GetAllListAsync(u => u.ProductId == entity.Productld) on a.Id equals b.GradientId
                                                                     join c in await _gradientModelYearRepository.GetAllListAsync(h => h.Year == entitySolution.Year && h.UpDown == entitySolution.UpDown) on b.Id equals c.GradientModelId
                                                                     select new GradientModelYear
                                                                     {
                                                                         Count = c.Count
                                                                     }).ToList();
                    if (GradientModelYearList.Count > 0)
                    {
                        dtosItem1.YearMountCount = GradientModelYearList[0].Count;
                    }
                    else
                    {
                        dtosItem1.YearMountCount = 0;
                    }
                }

                logisticscostResponse.LogisticscostList = dtosItem;
                logisticscostResponse.Classification = item;
                if (dtosItem.Count>0)
                {
                    logisticscostResponse.UpDown = (int)(dtosItem[0].Moon);
                }
                logisticscostResponseList.Add(logisticscostResponse);
            }
            //无数据的情况下返回

            if (null == logisticscostResponseList || logisticscostResponseList.Count<1)
            {

                List<Gradient>  data= _gradientRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId).Result.ToList();
                foreach (var item in data)
                {
                    LogisticscostResponseDto logisticscostResponse = new LogisticscostResponseDto();
                    logisticscostResponse.Classification = item.GradientValue.ToString();
                    List <LogisticscostDto> logisticscostDtos= new List<LogisticscostDto>();
                    foreach (var item1 in year)
                    {
                        LogisticscostDto logisticscostDto = new LogisticscostDto();
                        if (item1.UpDown == YearType.FirstHalf)
                        {

                            logisticscostDto.Year = item1.Year + "上半年";
                            logisticscostDto.Moon = 6;
                        }
                        else if (item1.UpDown == YearType.SecondHalf)
                        {
                            logisticscostDto.Year = item1.Year + "下半年";
                            logisticscostDto.Moon = 6;
                        }
                        else
                        {
                            logisticscostDto.Year = item1.Year.ToString();
                            logisticscostDto.Moon = 12;
                        }
                        ModelCountYear entitySolution = await _modelCountYearRepository.GetAsync((long)item1.Id);

                        List<GradientModelYear> gradientModelYears = _gradientModelYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == entity.Productld && p.Year == entitySolution.Year && p.UpDown == entitySolution.UpDown).ToList();
                       

                        List<GradientModelYear> GradientModelYearList = (from a in await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == item.Id)
                                                            join b in await _gradientModelRepository.GetAllListAsync(u => u.ProductId == entity.Productld) on a.Id equals b.GradientId
                                                            join c in await _gradientModelYearRepository.GetAllListAsync(h => h.Year == entitySolution.Year && h.UpDown == entitySolution.UpDown) on b.Id equals c.GradientModelId
                                                            select new GradientModelYear
                                                            {
                                                                Count = c.Count
                                                            }).ToList();
                        if (GradientModelYearList.Count > 0)
                        {
                            logisticscostDto.YearMountCount = GradientModelYearList[0].Count;
                        }
                        else
                        {
                            logisticscostDto.YearMountCount = 0;
                        }
                        logisticscostDto.ModelCountYearId = item1.Id;
                        logisticscostDto.FreightPrice = 0;
                        logisticscostDto.MonthlyDemandPrice= 0;
                        logisticscostDto.PackagingPrice= 0;
                        logisticscostDto.StoragePrice= 0;
                        logisticscostDto.SinglyDemandPrice= 0;
                        logisticscostDto.TransportPrice = 0;

                        logisticscostDtos.Add(logisticscostDto);
                    }
                    logisticscostResponse.LogisticscostList = logisticscostDtos;
                    if (logisticscostDtos.Count > 0)
                    {
                        logisticscostResponse.UpDown = (int)(logisticscostDtos[0].Moon);
                    }
                    logisticscostResponseList.Add(logisticscostResponse);
                    }

            }

            // 数据返回
            return logisticscostResponseList;


        }


        /// <summary>
        /// 根据当前流程号复制新的流程 AuditFlowId 老流程号  AuditFlowNewId 新流程号  SolutionIdAndQuoteSolutionIds 方案数组
        /// </summary>
        /// <returns>结果</returns>
        public virtual async Task<string> LogisticscostsCopyAsync(long AuditFlowId, long AuditFlowNewId, List<SolutionIdAndQuoteSolutionId> SolutionIdAndQuoteSolutionIds)
        {
            if (SolutionIdAndQuoteSolutionIds.Count>0) {
                foreach (var item in SolutionIdAndQuoteSolutionIds)
                {
                    await _logisticscostRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId ==item.QuoteSolutionId);
                    var query = _logisticscostRepository.GetAllList(p => p.IsDeleted == false && p.AuditFlowId == AuditFlowId && p.SolutionId == item.QuoteSolutionId);
                    foreach (var itemQuery in query)
                    {
                        Logisticscost logisticscost = new Logisticscost();
                        logisticscost.SolutionId = item.NewSolutionId;
                        logisticscost.AuditFlowId = AuditFlowNewId;
                        logisticscost.Classification = itemQuery.Classification;
                        logisticscost.CreationTime = DateTime.Now;
                        logisticscost.FreightPrice = itemQuery.FreightPrice;
                        logisticscost.MonthlyDemandPrice = itemQuery.MonthlyDemandPrice;
                        logisticscost.PackagingPrice = itemQuery.PackagingPrice;
                        logisticscost.SinglyDemandPrice = itemQuery.SinglyDemandPrice;
                        logisticscost.StoragePrice = itemQuery.StoragePrice;
                        logisticscost.TransportPrice = itemQuery.TransportPrice;
                        logisticscost.Remark = itemQuery.Remark;
                        logisticscost.ModelCountYearId = itemQuery.ModelCountYearId;
                        if (AbpSession.UserId != null)
                        {
                            logisticscost.CreatorUserId = AbpSession.UserId.Value;
                            logisticscost.LastModificationTime = DateTime.Now;
                            logisticscost.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        logisticscost.LastModificationTime = DateTime.Now;
                        await _logisticscostRepository.InsertAsync(logisticscost);

                    }

                }
            }

            // 数据返回
            return "复制成功";


        }


        /// <summary>
        /// 物流成本SOR下载
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task<FileResult> GetSorByAuditFlowId(long auditFlowId)
        {
            try
            {
                var query = this._priceEvaluationRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == auditFlowId).ToList();
                if (query.Count() < 1) {
                    throw new FriendlyException("没有对应文件");

                }
                long SorFileId = JsonConvert.DeserializeObject<List<long>>(query[0].SorFile).FirstOrDefault();
                //long SorFileId = long.Parse(priceInfo.SorFile);
                if (null != SorFileId)
                {
                    return await _fileCommonService.DownloadFile(SorFileId);
                }
                else
                {
                    throw new FriendlyException("文件找不到");
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 获取修改--无用接口
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetEditorAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(LogisticscostDto input)
        {

            //先根据零件和流程删除之前的数据
            var query = _logisticscostRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == input.SolutionId);
            var list = query.ToList();
            foreach (var item in list)
            {
                await _logisticscostRepository.DeleteAsync(s => s.Id == item.Id);
            }
            List<LogisticscostResponseDto> LogisticscostList = input.LogisticscostList;
            foreach (var item in LogisticscostList)
            {
                foreach (var item1 in item.LogisticscostList)
                {
                    var entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(item1, new Logisticscost());
                    Logisticscost logisticscost =  new Logisticscost();
                    logisticscost.SolutionId = input.SolutionId;
                    logisticscost.AuditFlowId = input.AuditFlowId;
                    logisticscost.Status = input.Status;
                    logisticscost.Classification = item.Classification;
                    logisticscost.CreationTime = DateTime.Now;
                    logisticscost.FreightPrice = entity.FreightPrice;
                    logisticscost.MonthlyDemandPrice = entity.MonthlyDemandPrice;
                    logisticscost.PackagingPrice = entity.PackagingPrice;
                    logisticscost.SinglyDemandPrice = entity.SinglyDemandPrice;
                    logisticscost.StoragePrice = entity.StoragePrice;
                    logisticscost.TransportPrice = entity.TransportPrice;
                    logisticscost.Remark = entity.Remark;
                    logisticscost.ModelCountYearId = entity.ModelCountYearId;
                    if (AbpSession.UserId != null)
                    {
                        logisticscost.CreatorUserId = AbpSession.UserId.Value;
                        logisticscost.LastModificationTime = DateTime.Now;
                        logisticscost.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    logisticscost.LastModificationTime = DateTime.Now;
                    entity = await _logisticscostRepository.InsertAsync(logisticscost);
                }
            }
            //插入零件表
            await _resourceNreIsSubmit.DeleteAsync(t=> t.AuditFlowId == (long)input.AuditFlowId && t.SolutionId == (long)input.SolutionId && t.EnumSole == NreIsSubmitDto.Logisticscost.ToString());

            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = (long)input.AuditFlowId, SolutionId = (long)input.SolutionId, EnumSole = NreIsSubmitDto.Logisticscost.ToString() });
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<String> CreateSubmitAsync(GetLogisticscostsInput input)
        {
            //已经录入数量
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.Logisticscost.ToString()));

            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            int quantity = result.Count - nreIsSubmits.Count;
            if (quantity > 0)
            {
                return "还有" + quantity + "个方案没有提交，请先保存";
            }
            else {
                //嵌入工作流
                await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                {
                    NodeInstanceId = input.NodeInstanceId,
                    FinanceDictionaryDetailId = input.Opinion,
                    Comment = input.Comment,
                });
                //提交完成  可以在这里做审核处理
                return "提交完成";

            }
           

        }



        /// <summary>
        /// 编辑---无用接口
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> UpdateAsync(LogisticscostDto input)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(input, entity);
            entity = await _logisticscostRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 删除---无用接口
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _logisticscostRepository.DeleteAsync(s => s.Id == id);
        }


        /// <summary>
        /// 流程退出，对应的数据进行删除
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        /// <returns></returns>
        public virtual async Task DeleteAuditFlowIdAsync(long AuditFlowId)
        {
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == AuditFlowId && t.EnumSole == NreIsSubmitDto.Logisticscost.ToString());
        }
    }
}
