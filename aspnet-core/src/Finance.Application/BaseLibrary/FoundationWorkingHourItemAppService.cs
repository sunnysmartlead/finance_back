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
    public class FoundationWorkingHourItemAppService : ApplicationService
    {
        private readonly IRepository<FoundationWorkingHourItem, long> _foundationWorkingHourItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationWorkingHourItemRepository"></param>
        public FoundationWorkingHourItemAppService(
            IRepository<FoundationWorkingHourItem, long> foundationWorkingHourItemRepository)
        {
            _foundationWorkingHourItemRepository = foundationWorkingHourItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourItemDto> GetAsyncById(long id)
        {
            FoundationWorkingHourItem entity = await _foundationWorkingHourItemRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHourItem, FoundationWorkingHourItemDto>(entity,new FoundationWorkingHourItemDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationWorkingHourItemDto>> GetListAsync(GetFoundationWorkingHourItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourItemRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHourItem>, List<FoundationWorkingHourItemDto>>(list, new List<FoundationWorkingHourItemDto>());
            // 数据返回
            return new PagedResultDto<FoundationWorkingHourItemDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourItemDto> GetEditorAsyncById(long id)
        {
            FoundationWorkingHourItem entity = await _foundationWorkingHourItemRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHourItem, FoundationWorkingHourItemDto>(entity,new FoundationWorkingHourItemDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourItemDto> CreateAsync(FoundationWorkingHourItemDto input)
        {
            var entity = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(input,new FoundationWorkingHourItem());
            entity = await _foundationWorkingHourItemRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationWorkingHourItem, FoundationWorkingHourItemDto>(entity,new FoundationWorkingHourItemDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourItemDto> UpdateAsync(FoundationWorkingHourItemDto input)
        {
            FoundationWorkingHourItem entity = await _foundationWorkingHourItemRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(input, entity);
            entity = await _foundationWorkingHourItemRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationWorkingHourItem, FoundationWorkingHourItemDto>(entity,new FoundationWorkingHourItemDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationWorkingHourItemRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
