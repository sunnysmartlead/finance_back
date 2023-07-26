using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationWorkingHourAppService : ApplicationService
    {
        private readonly IRepository<FoundationWorkingHour, long> _foundationWorkingHourRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationWorkingHourRepository"></param>
        public FoundationWorkingHourAppService(
            IRepository<FoundationWorkingHour, long> foundationWorkingHourRepository)
        {
            _foundationWorkingHourRepository = foundationWorkingHourRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> GetAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationWorkingHourDto>> GetListAsync(GetFoundationWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHour>, List<FoundationWorkingHourDto>>(list, new List<FoundationWorkingHourDto>());
            // 数据返回
            return new PagedResultDto<FoundationWorkingHourDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> GetEditorAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> CreateAsync(FoundationWorkingHourDto input)
        {
            var entity = ObjectMapper.Map<FoundationWorkingHourDto, FoundationWorkingHour>(input,new FoundationWorkingHour());
            entity = await _foundationWorkingHourRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> UpdateAsync(FoundationWorkingHourDto input)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationWorkingHourRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationWorkingHourRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
