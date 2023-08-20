using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.BaseLibrary;
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
    public class ProcessHoursEnterLineAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterLine, long> _processHoursEnterLineRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterLineRepository"></param>
        public ProcessHoursEnterLineAppService(
            IRepository<ProcessHoursEnterLine, long> processHoursEnterLineRepository)
        {
            _processHoursEnterLineRepository = processHoursEnterLineRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterLineDto> GetByIdAsync(long id)
        {
            ProcessHoursEnterLine entity = await _processHoursEnterLineRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterLine, ProcessHoursEnterLineDto>(entity,new ProcessHoursEnterLineDto());
        }


        /// <summary>
        /// 列表-根据方案 流程  uph值查询
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<ProcessHoursEnterLineDto>> GetListByUphAsync(GetProcessHoursEnterLinesInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.Uph == input.Uph && t.AuditFlowId == input.AuditFlowId && t.SolutionId == input.SolutionId);

            // 查询数据
            var list = query.ToList();
            List<ProcessHoursEnterLineDto> processHoursEnterLines = new List<ProcessHoursEnterLineDto>();
            foreach (var item in list)
            {
                ProcessHoursEnterLineDto processHoursEnterLineDto = new ProcessHoursEnterLineDto();
                processHoursEnterLineDto.Year = item.Year;
                processHoursEnterLineDto.Value = item.Value;
                processHoursEnterLines.Add(processHoursEnterLineDto);

            }
            //数据转换

            return processHoursEnterLines;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterLineDto>> GetListAsync(GetProcessHoursEnterLinesInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnterLine>, List<ProcessHoursEnterLineDto>>(list, new List<ProcessHoursEnterLineDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterLineDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterLineDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnterLine entity = await _processHoursEnterLineRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterLine, ProcessHoursEnterLineDto>(entity,new ProcessHoursEnterLineDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterLineDto> CreateAsync(ProcessHoursEnterLineDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnterLineDto, ProcessHoursEnterLine>(input,new ProcessHoursEnterLine());
            entity = await _processHoursEnterLineRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterLine, ProcessHoursEnterLineDto>(entity,new ProcessHoursEnterLineDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterLineDto> UpdateAsync(ProcessHoursEnterLineDto input)
        {
            ProcessHoursEnterLine entity = await _processHoursEnterLineRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterLineRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterLine, ProcessHoursEnterLineDto>(entity,new ProcessHoursEnterLineDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterLineRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
