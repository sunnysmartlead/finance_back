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
    public class FoundationHardwareItemAppService : ApplicationService
    {
        private readonly IRepository<FoundationHardwareItem, long> _foundationHardwareItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationHardwareItemRepository"></param>
        public FoundationHardwareItemAppService(
            IRepository<FoundationHardwareItem, long> foundationHardwareItemRepository)
        {
            _foundationHardwareItemRepository = foundationHardwareItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareItemDto> GetAsyncById(long id)
        {
            FoundationHardwareItem entity = await _foundationHardwareItemRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(entity,new FoundationHardwareItemDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationHardwareItemDto>> GetListAsync(GetFoundationHardwareItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationHardwareItemRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(list, new List<FoundationHardwareItemDto>());
            // 数据返回
            return new PagedResultDto<FoundationHardwareItemDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareItemDto> GetEditorAsyncById(long id)
        {
            FoundationHardwareItem entity = await _foundationHardwareItemRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(entity,new FoundationHardwareItemDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareItemDto> CreateAsync(FoundationHardwareItemDto input)
        {
            var entity = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(input,new FoundationHardwareItem());
            entity = await _foundationHardwareItemRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(entity,new FoundationHardwareItemDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareItemDto> UpdateAsync(FoundationHardwareItemDto input)
        {
            FoundationHardwareItem entity = await _foundationHardwareItemRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationHardwareItemRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(entity,new FoundationHardwareItemDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationHardwareItemRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
