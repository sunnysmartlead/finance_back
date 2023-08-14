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
    public class ProcessHoursEnterUphAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterUph, long> _processHoursEnterUphRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterUphRepository"></param>
        public ProcessHoursEnterUphAppService(
            IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository)
        {
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
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
