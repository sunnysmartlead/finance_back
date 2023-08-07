using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
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
    public class ProcessHoursEnterFrockAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterFrock, long> _processHoursEnterFrockRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterFrockRepository"></param>
        public ProcessHoursEnterFrockAppService(
            IRepository<ProcessHoursEnterFrock, long> processHoursEnterFrockRepository)
        {
            _processHoursEnterFrockRepository = processHoursEnterFrockRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFrockDto> GetByIdAsync(long id)
        {
            ProcessHoursEnterFrock entity = await _processHoursEnterFrockRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterFrock, ProcessHoursEnterFrockDto>(entity,new ProcessHoursEnterFrockDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterFrockDto>> GetListAsync(GetProcessHoursEnterFrocksInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterFrockRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnterFrock>, List<ProcessHoursEnterFrockDto>>(list, new List<ProcessHoursEnterFrockDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterFrockDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFrockDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnterFrock entity = await _processHoursEnterFrockRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterFrock, ProcessHoursEnterFrockDto>(entity,new ProcessHoursEnterFrockDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFrockDto> CreateAsync(ProcessHoursEnterFrockDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnterFrockDto, ProcessHoursEnterFrock>(input,new ProcessHoursEnterFrock());
            entity = await _processHoursEnterFrockRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterFrock, ProcessHoursEnterFrockDto>(entity,new ProcessHoursEnterFrockDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFrockDto> UpdateAsync(ProcessHoursEnterFrockDto input)
        {
            ProcessHoursEnterFrock entity = await _processHoursEnterFrockRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterFrockRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterFrock, ProcessHoursEnterFrockDto>(entity,new ProcessHoursEnterFrockDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterFrockRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
