﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using test;

namespace Finance.Processes
{
    /// <summary>
    /// 管理
    /// </summary>
    public class ProcessHoursEnterUphAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterUph, long> _processHoursEnterUphRepository;
        private readonly DataInputAppService _dataInputAppService;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;   
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterUphRepository"></param>
        public ProcessHoursEnterUphAppService(
            DataInputAppService dataInputAppService,
            IRepository<Solution, long> resourceSchemeTable,
            IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository,
            IRepository<ModelCountYear, long> modelCountYearRepository)
        {
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
            _dataInputAppService = dataInputAppService;
            _resourceSchemeTable = resourceSchemeTable;
            _modelCountYearRepository = modelCountYearRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterUphDto> GetByIdAsync(long id)
        {
            ProcessHoursEnterUph entity = await _processHoursEnterUphRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterUph, ProcessHoursEnterUphDto>(entity,new ProcessHoursEnterUphDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterUphDto>> GetListAsync(GetProcessHoursEnterUphsInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnterUph>, List<ProcessHoursEnterUphDto>>(list, new List<ProcessHoursEnterUphDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterUphDto>(totalCount, dtos);
        }



        /// <summary>
        /// 根据流程和方案获取COB-UPH的值
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<ProcessHoursEnterUphBomDto> GetListByAuditFlowIdOrSolutionIdAsync(GetProcessHoursEnterUphsInput input)
        {
            ProcessHoursEnterUphBomDto processHoursEnterUphBomDto = new ProcessHoursEnterUphBomDto(); 
            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);
            // 设置查询条件
            List<GradientModelYearListDto> data = await _dataInputAppService.GetGradientModelYearByProductId((long)entity.Productld);
            List<ProcessHoursEnterUphBomItemDto> processHoursEnterUphBomItemDtoList = new List<ProcessHoursEnterUphBomItemDto>();
            
            var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
            foreach (ModelCountYear row in query)
            {

                ProcessHoursEnterUphBomItemDto processHoursEnterUphListDto = new ProcessHoursEnterUphBomItemDto();
                if (row.UpDown == YearType.FirstHalf)
                {

                    processHoursEnterUphListDto.Year = row.Year + "上半年";
                }
                else if (row.UpDown == YearType.SecondHalf)
                {
                    processHoursEnterUphListDto.Year = row.Year + "下半年";
                }
                else
                {
                    processHoursEnterUphListDto.Year = row.Year.ToString();
                }

                var CobuphList = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "cobuph" && t.ModelCountYearId == row.Id).ToList();
                if (null != CobuphList && CobuphList.Count > 0)
                {
                    processHoursEnterUphListDto.ComPuh = CobuphList[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.ComPuh = 0;
                }
                processHoursEnterUphBomItemDtoList.Add(processHoursEnterUphListDto);


            }

           
            processHoursEnterUphBomDto.IsCOB = entity.IsCOB;
            processHoursEnterUphBomDto.ProcessHoursEnterUphBomItemsList = processHoursEnterUphBomItemDtoList;
            return processHoursEnterUphBomDto;


        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterUphDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnterUph entity = await _processHoursEnterUphRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterUph, ProcessHoursEnterUphDto>(entity,new ProcessHoursEnterUphDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterUphDto> CreateAsync(ProcessHoursEnterUphDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnterUphDto, ProcessHoursEnterUph>(input,new ProcessHoursEnterUph());
            entity = await _processHoursEnterUphRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterUph, ProcessHoursEnterUphDto>(entity,new ProcessHoursEnterUphDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterUphDto> UpdateAsync(ProcessHoursEnterUphDto input)
        {
            ProcessHoursEnterUph entity = await _processHoursEnterUphRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterUphRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterUph, ProcessHoursEnterUphDto>(entity,new ProcessHoursEnterUphDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterUphRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
