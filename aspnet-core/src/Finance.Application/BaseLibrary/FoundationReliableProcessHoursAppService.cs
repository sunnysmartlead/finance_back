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
    public class FoundationReliableProcessHoursAppService : ApplicationService
    {
        private readonly IRepository<FoundationReliableProcessHours, long> _foundationReliableProcessHoursRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationReliableProcessHoursRepository"></param>
        public FoundationReliableProcessHoursAppService(
            IRepository<FoundationReliableProcessHours, long> foundationReliableProcessHoursRepository)
        {
            _foundationReliableProcessHoursRepository = foundationReliableProcessHoursRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationReliableProcessHoursDto> GetAsyncById(long id)
        {
            FoundationReliableProcessHours entity = await _foundationReliableProcessHoursRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationReliableProcessHours, FoundationReliableProcessHoursDto>(entity,new FoundationReliableProcessHoursDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationReliableProcessHoursDto>> GetListAsync(GetFoundationReliableProcessHourssInput input)
        {
            // 设置查询条件
            var query = this._foundationReliableProcessHoursRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationReliableProcessHours>, List<FoundationReliableProcessHoursDto>>(list, new List<FoundationReliableProcessHoursDto>());
            // 数据返回
            return new PagedResultDto<FoundationReliableProcessHoursDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationReliableProcessHoursDto> GetEditorAsyncById(long id)
        {
            FoundationReliableProcessHours entity = await _foundationReliableProcessHoursRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationReliableProcessHours, FoundationReliableProcessHoursDto>(entity,new FoundationReliableProcessHoursDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationReliableProcessHoursDto> CreateAsync(FoundationReliableProcessHoursDto input)
        {
            var entity = ObjectMapper.Map<FoundationReliableProcessHoursDto, FoundationReliableProcessHours>(input,new FoundationReliableProcessHours());
            entity = await _foundationReliableProcessHoursRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationReliableProcessHours, FoundationReliableProcessHoursDto>(entity,new FoundationReliableProcessHoursDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationReliableProcessHoursDto> UpdateAsync(FoundationReliableProcessHoursDto input)
        {
            FoundationReliableProcessHours entity = await _foundationReliableProcessHoursRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationReliableProcessHoursDto, FoundationReliableProcessHours>(input, entity);
            entity = await _foundationReliableProcessHoursRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationReliableProcessHours, FoundationReliableProcessHoursDto>(entity,new FoundationReliableProcessHoursDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationReliableProcessHoursRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
