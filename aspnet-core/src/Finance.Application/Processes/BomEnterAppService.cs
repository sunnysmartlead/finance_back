using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.Nre;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.Processes
{
    /// <summary>
    /// 管理
    /// </summary>
    public class BomEnterAppService : ApplicationService
    {
        private readonly IRepository<BomEnter, long> _bomEnterRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<BomEnterTotal, long> _bomEnterTotalRepository;
        private readonly DataInputAppService _dataInputAppService;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<NreIsSubmit, long> _resourceNreIsSubmit;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;

        private readonly WorkflowInstanceAppService _workflowInstanceAppService;


        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="bomEnterRepository"></param>
        public BomEnterAppService(
            IRepository<BomEnterTotal, long> foundationFoundationWorkingHourItemRepository,
            DataInputAppService dataInputAppService,
            IRepository<User, long> userRepository,
              IRepository<Solution, long> resourceSchemeTable,
              IRepository<ModelCountYear, long> modelCountYearRepository,
            WorkflowInstanceAppService workflowInstanceAppService,
             IRepository<NreIsSubmit, long> resourceNreIsSubmit,
            IRepository<BomEnter, long> bomEnterRepository)

        {
            _bomEnterRepository = bomEnterRepository;
            _bomEnterTotalRepository = foundationFoundationWorkingHourItemRepository;
            _userRepository = userRepository;
            _dataInputAppService = dataInputAppService;
            _resourceSchemeTable = resourceSchemeTable;

            _workflowInstanceAppService = workflowInstanceAppService;

            _modelCountYearRepository = modelCountYearRepository;
            _resourceNreIsSubmit = resourceNreIsSubmit;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> GetByIdAsync(long id)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<BomEnterDto>> GetListAsync(GetBomEntersInput input)
        {
            // 设置查询条件
            var query = this._bomEnterRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<BomEnter>, List<BomEnterDto>>(list, new List<BomEnterDto>());
            // 数据返回
            return new PagedResultDto<BomEnterDto>(totalCount, dtos);
        }


        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<BomEnterDto>> GetListAllAsync(GetBomEntersInput input)
        {

            try
            {
                Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

                //有数据的返回
                var query = (from a in _bomEnterRepository.GetAllList(p =>
         p.IsDeleted == false && p.SolutionId == entity.Id && p.AuditFlowId == input.AuditFlowId
         ).Select(c => c.Classification).Distinct()
                             select a).ToList();
                var dtos = ObjectMapper.Map<List<String>, List<String>>(query, new List<String>());
                List<BomEnterDto> logisticscostResponseList = new List<BomEnterDto>();
                foreach (var item in dtos)
                {
                    BomEnterDto logisticscostResponse = new BomEnterDto();

                    var queryItem = this._bomEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == entity.Id && t.Classification == item).ToList();
                    List<BomEnterDto> bomEnterDto =   new List<BomEnterDto>();
                    foreach (var dtosItem in queryItem)
                    {
                        BomEnterDto bomEnterDto1 = new BomEnterDto();
                        if (dtosItem.ModelCountYearId != 0)
                        {
                            ModelCountYear entitySolution = await _modelCountYearRepository.GetAsync((long)dtosItem.ModelCountYearId);

                            bomEnterDto1.ModelCountYearId = entitySolution.Id;
                            if (entitySolution.UpDown == YearType.FirstHalf)
                            {

                                bomEnterDto1.Year = entitySolution.Year + "上半年";
                            }
                            else if (entitySolution.UpDown == YearType.SecondHalf)
                            {
                                bomEnterDto1.Year = entitySolution.Year + "下半年";
                            }
                            else
                            {
                                bomEnterDto1.Year = entitySolution.Year.ToString();
                            }
                        }
                        else {
                            bomEnterDto1.ModelCountYearId = 0;
                            bomEnterDto1.Year = "全生命周期";
                        }
                       

                        bomEnterDto1.TotalCost = dtosItem.TotalCost;
                        bomEnterDto1.Remark = dtosItem.Remark;
                        bomEnterDto1.IndirectSummary = dtosItem.IndirectSummary;
                        bomEnterDto1.IndirectManufacturingCosts = dtosItem.IndirectManufacturingCosts;
                        bomEnterDto1.IndirectLaborPrice = dtosItem.IndirectLaborPrice;
                        bomEnterDto1.IndirectDepreciation = dtosItem.IndirectDepreciation;
                        bomEnterDto1.DirectSummary = dtosItem.DirectSummary;
                        bomEnterDto1.DirectManufacturingCosts = dtosItem.DirectManufacturingCosts;
                        bomEnterDto1.DirectLineChangeCost = dtosItem.DirectLineChangeCost;
                        bomEnterDto1.DirectLaborPrice = dtosItem.DirectLaborPrice;
                     
                        bomEnterDto1.DirectDepreciation = dtosItem.DirectDepreciation;
                        bomEnterDto.Add(bomEnterDto1);
                    }
                    var queryItemTotal = this._bomEnterTotalRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == entity.Id && t.Classification == item).ToList();

                    List<BomEnterTotalDto> ListbomEnterDto = new List<BomEnterTotalDto>();  
                    foreach (var dtosItemTotal in queryItemTotal)
                    {
                        BomEnterTotalDto bomEnterTotal = new BomEnterTotalDto();
                        if (dtosItemTotal.ModelCountYearId != 0)
                        {
                            ModelCountYear entitySolution = await _modelCountYearRepository.GetAsync((long)dtosItemTotal.ModelCountYearId);

                            bomEnterTotal.ModelCountYearId = entitySolution.Id;
                            if (entitySolution.UpDown == YearType.FirstHalf)
                            {

                                bomEnterTotal.Year = entitySolution.Year + "上半年";
                            }
                            else if (entitySolution.UpDown == YearType.SecondHalf)
                            {
                                bomEnterTotal.Year = entitySolution.Year + "下半年";
                            }
                            else
                            {
                                bomEnterTotal.Year = entitySolution.Year.ToString();
                            }
                        }
                        else
                        {
                            bomEnterTotal.ModelCountYearId = 0;
                            bomEnterTotal.Year = "全生命周期";
                        }

                        bomEnterTotal.Remark= dtosItemTotal.Remark;
                        bomEnterTotal.TotalCost = dtosItemTotal.TotalCost;
                        ListbomEnterDto.Add(bomEnterTotal);
                    }
                    logisticscostResponse.ListBomEnter = bomEnterDto;
                    logisticscostResponse.ListBomEnterTotal = ListbomEnterDto;
                    logisticscostResponse.Classification = item;
                    logisticscostResponseList.Add(logisticscostResponse);
                }

                //没有数据的情况下

                Solution solution = await _resourceSchemeTable.GetAsync((long)input.SolutionId);
                List<Gradient> ListGradient = await _dataInputAppService.GetGradientByAuditFlowId((long)input.AuditFlowId);
                var year = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
                if (null == logisticscostResponseList || logisticscostResponseList.Count <1) {
              
                    foreach (var item in ListGradient)
                    {
                  
                        List<BomEnterDto> bomEnterDto = new List<BomEnterDto>();
                        BomEnterDto logisticscostResponse = new BomEnterDto();
                        foreach (var dtosItem in year)
                        {
                            BomEnterDto bomEnterDto1 = new BomEnterDto();
                            if (dtosItem.UpDown == YearType.FirstHalf)
                            {

                                bomEnterDto1.Year = dtosItem.Year + "上半年";
                            }
                            else if (dtosItem.UpDown == YearType.SecondHalf)
                            {
                                bomEnterDto1.Year = dtosItem.Year + "下半年";
                            }
                            else
                            {
                                bomEnterDto1.Year = dtosItem.Year.ToString();
                            }

                            bomEnterDto1.IndirectSummary = 0;
                            bomEnterDto1.IndirectManufacturingCosts = 0;
                            bomEnterDto1.IndirectLaborPrice = 0;
                            bomEnterDto1.IndirectDepreciation = 0;
                            bomEnterDto1.DirectSummary = 0;
                            bomEnterDto1.DirectManufacturingCosts = 0;
                            bomEnterDto1.DirectLineChangeCost =0;
                            bomEnterDto1.ModelCountYearId = dtosItem.Id;
                            bomEnterDto1.DirectLaborPrice = 0;
                            bomEnterDto1.DirectDepreciation = 0;

                            bomEnterDto.Add(bomEnterDto1);
                        }
                        BomEnterDto bomEnterDto3 = new BomEnterDto();


                        bomEnterDto3.Year = "全生命周期";
                        bomEnterDto3.IndirectSummary = 0;
                        bomEnterDto3.IndirectManufacturingCosts = 0;
                        bomEnterDto3.IndirectLaborPrice = 0;
                        bomEnterDto3.IndirectDepreciation = 0;
                        bomEnterDto3.DirectSummary = 0;
                        bomEnterDto3.DirectManufacturingCosts = 0;
                        bomEnterDto3.DirectLineChangeCost = 0;
                        bomEnterDto3.ModelCountYearId = 0;
                        bomEnterDto3.DirectLaborPrice = 0;
                        bomEnterDto3.DirectDepreciation = 0;
                        bomEnterDto.Add(bomEnterDto3);
                        List<BomEnterTotalDto> ListbomEnterDto = new List<BomEnterTotalDto>();
                        foreach (var dtosItemTotal in year)
                        {
                            BomEnterTotalDto bomEnterTotal = new BomEnterTotalDto();
                            bomEnterTotal.Year = dtosItemTotal.Year.ToString();
                            bomEnterTotal.TotalCost = 0;
                            bomEnterTotal.ModelCountYearId = dtosItemTotal.Id;
                            ListbomEnterDto.Add(bomEnterTotal);
                        }
                        BomEnterTotalDto bomEnterTota3 = new BomEnterTotalDto();
                        bomEnterTota3.Year = "全生命周期";
                        bomEnterTota3.TotalCost = 0;
                        bomEnterTota3.ModelCountYearId = 0;
                        ListbomEnterDto.Add(bomEnterTota3);
                        logisticscostResponse.ListBomEnter = bomEnterDto;
                        logisticscostResponse.ListBomEnterTotal = ListbomEnterDto;
                        logisticscostResponse.Classification = item.GradientValue.ToString();
                        logisticscostResponseList.Add(logisticscostResponse);


                    }


                    }
                // 数据返回 
                return logisticscostResponseList;
            }
            catch (Exception)
            {

                throw;
            }
         
        }




        /// <summary>
        /// 复制流程 AuditFlowId 老流程号  AuditFlowNewId 新流程号  SolutionIdAndQuoteSolutionIds 方案数组
        /// </summary>
        /// <returns>结果</returns>
        public virtual async Task<string> GetBomEntersCopyAsync(long AuditFlowId, long AuditFlowNewId, List<SolutionIdAndQuoteSolutionId> SolutionIdAndQuoteSolutionIds)
        {
            if (SolutionIdAndQuoteSolutionIds.Count>0)
            {
                foreach (var item in SolutionIdAndQuoteSolutionIds)
                {
                    await _bomEnterRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId == item.QuoteSolutionId);
                    await _bomEnterTotalRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId == item.QuoteSolutionId);
                    //有数据的返回
                    var query = _bomEnterRepository.GetAllList(p =>p.IsDeleted == false && p.AuditFlowId == AuditFlowId && p.SolutionId == item.NewSolutionId).ToList();

                    var queryEnterTotal = _bomEnterTotalRepository.GetAllList(p =>p.IsDeleted == false && p.AuditFlowId == AuditFlowId && p.SolutionId == item.NewSolutionId).ToList();
                    foreach (var ListBomEnterItem in query)
                    {
                        BomEnter bomEnter = new BomEnter();
                        bomEnter.SolutionId = item.NewSolutionId;
                        bomEnter.AuditFlowId = AuditFlowNewId;
                        bomEnter.Classification = ListBomEnterItem.Classification;
                        bomEnter.CreationTime = DateTime.Now;
                        bomEnter.DirectDepreciation = ListBomEnterItem.DirectDepreciation;
                        bomEnter.DirectLaborPrice = ListBomEnterItem.DirectLaborPrice;
                        bomEnter.DirectLineChangeCost = ListBomEnterItem.DirectLineChangeCost;
                        bomEnter.DirectManufacturingCosts = ListBomEnterItem.DirectManufacturingCosts;
                        bomEnter.DirectSummary = ListBomEnterItem.DirectSummary;
                        bomEnter.IndirectDepreciation = ListBomEnterItem.IndirectDepreciation;
                        bomEnter.IndirectLaborPrice = ListBomEnterItem.IndirectLaborPrice;
                        bomEnter.IndirectManufacturingCosts = ListBomEnterItem.IndirectManufacturingCosts;
                        bomEnter.IndirectSummary = ListBomEnterItem.IndirectSummary;
                        bomEnter.TotalCost = ListBomEnterItem.TotalCost;
                        bomEnter.Year = ListBomEnterItem.Year;
                        bomEnter.ModelCountYearId = ListBomEnterItem.ModelCountYearId;
                        bomEnter.Remark = ListBomEnterItem.Remark;
                        if (AbpSession.UserId != null)
                        {
                            bomEnter.CreatorUserId = AbpSession.UserId.Value;
                            bomEnter.LastModificationTime = DateTime.Now;
                            bomEnter.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        bomEnter.LastModificationTime = DateTime.Now;
                        await _bomEnterRepository.InsertAsync(bomEnter);



                    }
                    foreach (var ListBomEnterTotalItem in queryEnterTotal)
                    {
                        BomEnterTotal bomEnterTotal = new BomEnterTotal();
                        bomEnterTotal.SolutionId = (long)item.NewSolutionId;
                        bomEnterTotal.AuditFlowId = AuditFlowNewId;
                        bomEnterTotal.Classification = ListBomEnterTotalItem.Classification;
                        bomEnterTotal.CreationTime = DateTime.Now;
                        bomEnterTotal.TotalCost = ListBomEnterTotalItem.TotalCost;
                        bomEnterTotal.Remark = ListBomEnterTotalItem.Remark;
                        bomEnterTotal.ModelCountYearId = ListBomEnterTotalItem.ModelCountYearId;
                        bomEnterTotal.Remark = ListBomEnterTotalItem.Remark;
                        if (AbpSession.UserId != null)
                        {
                            bomEnterTotal.CreatorUserId = AbpSession.UserId.Value;
                            bomEnterTotal.LastModificationTime = DateTime.Now;
                            bomEnterTotal.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        bomEnterTotal.LastModificationTime = DateTime.Now;
                        await _bomEnterTotalRepository.InsertAsync(bomEnterTotal);
                    }

                }

            }


            // 数据返回
            return "复制成功";
            }
          

        
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> GetEditorByIdAsync(long id)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }


  

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(BomEnterDto input)
        {
            Solution entitySolution = await _resourceSchemeTable.GetAsync((long)input.SolutionId);
            await _bomEnterRepository.DeleteAsync(s => s.SolutionId == entitySolution.Id && s.AuditFlowId == input.AuditFlowId);


            await _bomEnterTotalRepository.DeleteAsync(s => s.SolutionId == entitySolution.Id && s.AuditFlowId == input.AuditFlowId);

            List<BomEnterDto> LogisticscostList = input.ListBomEnter;

            foreach (var item in LogisticscostList)
            {
                foreach (var ListBomEnterItem in item.ListBomEnter)
                {
                    BomEnter bomEnter = new BomEnter();
                    bomEnter.SolutionId = entitySolution.Id;
                    bomEnter.AuditFlowId = input.AuditFlowId;
                    bomEnter.Classification = item.Classification;
                    bomEnter.CreationTime = DateTime.Now;
                    bomEnter.DirectDepreciation = ListBomEnterItem.DirectDepreciation;
                    bomEnter.DirectLaborPrice = ListBomEnterItem.DirectLaborPrice;
                    bomEnter.DirectLineChangeCost = ListBomEnterItem.DirectLineChangeCost;
                    bomEnter.DirectManufacturingCosts = ListBomEnterItem.DirectManufacturingCosts;
                    bomEnter.DirectSummary = ListBomEnterItem.DirectSummary;
                    bomEnter.IndirectDepreciation = ListBomEnterItem.IndirectDepreciation;
                    bomEnter.IndirectLaborPrice = ListBomEnterItem.IndirectLaborPrice;
                    bomEnter.IndirectManufacturingCosts = ListBomEnterItem.IndirectManufacturingCosts;
                    bomEnter.IndirectSummary = ListBomEnterItem.IndirectSummary;
                    bomEnter.TotalCost = ListBomEnterItem.TotalCost;
                    bomEnter.Year = ListBomEnterItem.Year;
                    bomEnter.ModelCountYearId = ListBomEnterItem.ModelCountYearId;
                    bomEnter.Remark = ListBomEnterItem.Remark;
                    if (AbpSession.UserId != null)
                    {
                        bomEnter.CreatorUserId = AbpSession.UserId.Value;
                        bomEnter.LastModificationTime = DateTime.Now;
                        bomEnter.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    bomEnter.LastModificationTime = DateTime.Now;
                    await _bomEnterRepository.InsertAsync(bomEnter);

                   

                }
                foreach (var ListBomEnterTotalItem in item.ListBomEnterTotal)
                {
                    BomEnterTotal bomEnterTotal = new BomEnterTotal();
                    bomEnterTotal.SolutionId = entitySolution.Id;
                    bomEnterTotal.AuditFlowId = input.AuditFlowId;
                    bomEnterTotal.Classification = item.Classification;
                    bomEnterTotal.CreationTime = DateTime.Now;
                    bomEnterTotal.TotalCost = ListBomEnterTotalItem.TotalCost;
                    bomEnterTotal.Remark = ListBomEnterTotalItem.Remark;
                    bomEnterTotal.ModelCountYearId = ListBomEnterTotalItem.ModelCountYearId;
                    bomEnterTotal.Remark = ListBomEnterTotalItem.Remark;
                    if (AbpSession.UserId != null)
                    {
                        bomEnterTotal.CreatorUserId = AbpSession.UserId.Value;
                        bomEnterTotal.LastModificationTime = DateTime.Now;
                        bomEnterTotal.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    bomEnterTotal.LastModificationTime = DateTime.Now;
                    await _bomEnterTotalRepository.InsertAsync(bomEnterTotal);
                }
            }
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == (long)input.AuditFlowId && t.SolutionId == (long)input.SolutionId && t.EnumSole == NreIsSubmitDto.COB.ToString());

            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = (long)input.AuditFlowId, SolutionId = (long)input.SolutionId, EnumSole = NreIsSubmitDto.COB.ToString() });


        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<String> CreateSubmitAsync(CreateSubmitInput input)
        {

            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.COB.ToString()));

            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            int quantity = result.Count - nreIsSubmits.Count;
            if (quantity > 0)
            {
                return "还有" + quantity + "个方案没有提交，请先保存";
            }
            else
            {

                //嵌入工作流
                await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                {
                    Comment = input.Comment,
                    FinanceDictionaryDetailId = input.Opinion,
                    NodeInstanceId = input.NodeInstanceId,
                });

                //提交完成  可以在这里做审核处理
                return "提交完成";

            }


        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> UpdateAsync(BomEnterDto input)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _bomEnterRepository.UpdateAsync(entity);
            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _bomEnterRepository.DeleteAsync(s => s.Id == id);
        }



        /// <summary>
        /// 流程退出，对应的数据进行删除
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        public virtual async Task DeleteAuditFlowIdAsync(long AuditFlowId)
        {
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == AuditFlowId && t.EnumSole == NreIsSubmitDto.COB.ToString());
        }
    }
}
