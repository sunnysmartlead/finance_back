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
             IRepository<GradientModel, long> gradientModelRepository, IRepository<GradientModelYear, long> gradientModelYearRepository
            )
        {
            _logisticscostRepository = logisticscostRepository;
            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _resourceSchemeTable = resourceSchemeTable;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);

            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 列表
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
            query.Count();

            var data1 = from m in _gradientModelRepository.GetAll()
                        join y in _gradientModelYearRepository.GetAll() on m.Id equals y.GradientModelId
                        join g in _gradientRepository.GetAll() on m.GradientId equals g.Id
                        join my in _modelCountYearRepository.GetAll() on y.ProductId equals my.ProductId
                        where y.ProductId == entity.Productld// && m.ProductId == productId && my.ProductId == productId
                        && y.Year == my.Year && y.UpDown == my.UpDown
                        select new GradientModelYearListDto
                        {
                            Id = y.Id,
                            AuditFlowId = y.AuditFlowId,
                            PriceEvaluationId = y.PriceEvaluationId,
                            GradientModelId = y.GradientModelId,
                            ProductId = y.ProductId,
                            GradientValue = g.GradientValue,
                            Year = y.Year,
                            UpDown = y.UpDown,
                            Count = y.Count,
                            YearMountCount = my.Quantity
                        };
            var dtos = ObjectMapper.Map<List<String>, List<String>>(query, new List<String>());
            List<LogisticscostResponseDto> logisticscostResponseList = new List<LogisticscostResponseDto>();
            foreach (var item in dtos)
            {
                LogisticscostResponseDto logisticscostResponse = new LogisticscostResponseDto();

                var queryItem = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == entity.Id && t.Classification == item).ToList();
                var dtosItem = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(queryItem, new List<LogisticscostDto>());
                foreach (var dtosItem1 in dtosItem)
                {
                    if (null != data1)
                    {
                        dtosItem1.YearMountCount = data1.ToList()[0].YearMountCount;
                    }
                    else {
                        dtosItem1.YearMountCount = 0;
                    }
                   
                }

                    logisticscostResponse.LogisticscostList = dtosItem;
                logisticscostResponse.Classification = item;
                logisticscostResponseList.Add(logisticscostResponse);
            }

            if (null == logisticscostResponseList || logisticscostResponseList.Count<1)
            {

                List<Gradient>  data= _gradientRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId).Result.ToList();
                foreach (var item in data)
                {
                    LogisticscostResponseDto logisticscostResponse = new LogisticscostResponseDto();
                    logisticscostResponse.Classification = item.GradientValue.ToString();
                    List <LogisticscostDto> logisticscostDtos= new List<LogisticscostDto>();
                    foreach (var item1 in data1)
                    {
                        LogisticscostDto logisticscostDto = new LogisticscostDto();
                        logisticscostDto.Year = item1.Year.ToString();
                        logisticscostDto.YearMountCount = item1.YearMountCount;
                        logisticscostDto.FreightPrice = 0;
                        logisticscostDto.MonthlyDemandPrice= 0;
                        logisticscostDto.PackagingPrice= 0;
                        logisticscostDto.StoragePrice= 0;
                        logisticscostDto.SinglyDemandPrice= 0;
                        logisticscostDto.TransportPrice = 0;

                        logisticscostDtos.Add(logisticscostDto);
                    }
                    logisticscostResponse.LogisticscostList = logisticscostDtos;
                    logisticscostResponseList.Add(logisticscostResponse);
                    }

            }

            // 数据返回
            return logisticscostResponseList;


        }


        /// <summary>
        /// 获取修改
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
                    logisticscost.Year = entity.Year;
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
            var count = (from a in _logisticscostRepository.GetAllList(p =>
         p.IsDeleted == false && p.AuditFlowId == input.AuditFlowId
         ).Select(p => p.SolutionId).Distinct()
                         select a).ToList();
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            int quantity = result.Count - count.Count;
            if (quantity > 0)
            {
                return "还有" + quantity + "个方案没有提交，请先提交";
            }
            else {

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
        public virtual async Task<LogisticscostDto> UpdateAsync(LogisticscostDto input)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(input, entity);
            entity = await _logisticscostRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _logisticscostRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
