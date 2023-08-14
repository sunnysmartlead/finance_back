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
    public class ProcessHoursEnteritemAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnteritem, long> _processHoursEnteritemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnteritemRepository"></param>
        public ProcessHoursEnteritemAppService(
            IRepository<ProcessHoursEnteritem, long> processHoursEnteritemRepository)
        {
            _processHoursEnteritemRepository = processHoursEnteritemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnteritemDto> GetByIdAsync(long id)
        {
            ProcessHoursEnteritem entity = await _processHoursEnteritemRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnteritem, ProcessHoursEnteritemDto>(entity,new ProcessHoursEnteritemDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnteritemDto>> GetListAsync(GetProcessHoursEnteritemsInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnteritemRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnteritem>, List<ProcessHoursEnteritemDto>>(list, new List<ProcessHoursEnteritemDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnteritemDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnteritemDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnteritem entity = await _processHoursEnteritemRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnteritem, ProcessHoursEnteritemDto>(entity,new ProcessHoursEnteritemDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnteritemDto> CreateAsync(ProcessHoursEnteritemDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnteritemDto, ProcessHoursEnteritem>(input,new ProcessHoursEnteritem());
            entity = await _processHoursEnteritemRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnteritem, ProcessHoursEnteritemDto>(entity,new ProcessHoursEnteritemDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnteritemDto> UpdateAsync(ProcessHoursEnteritemDto input)
        {
            ProcessHoursEnteritem entity = await _processHoursEnteritemRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnteritemRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnteritem, ProcessHoursEnteritemDto>(entity,new ProcessHoursEnteritemDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnteritemRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
