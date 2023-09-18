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
    public class FoundationFixtureItemAppService : ApplicationService
    {
        private readonly IRepository<FoundationFixtureItem, long> _foundationFixtureItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationFixtureItemRepository"></param>
        public FoundationFixtureItemAppService(
            IRepository<FoundationFixtureItem, long> foundationFixtureItemRepository)
        {
            _foundationFixtureItemRepository = foundationFixtureItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureItemDto> GetAsyncById(long id)
        {
            FoundationFixtureItem entity = await _foundationFixtureItemRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(entity,new FoundationFixtureItemDto());
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationFixtureItemDto>> GetListAllAsync(GetFoundationFixtureItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureItemRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.FixtureName))
            {
                query = query.Where(t => t.FixtureName.Contains(input.FixtureName));
            }

            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixtureItem>, List<FoundationFixtureItemDto>>(list, new List<FoundationFixtureItemDto>());

            // 数据返回
            return dtos;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationFixtureItemDto>> GetListAsync(GetFoundationFixtureItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureItemRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixtureItem>, List<FoundationFixtureItemDto>>(list, new List<FoundationFixtureItemDto>());
            // 数据返回
            return new PagedResultDto<FoundationFixtureItemDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureItemDto> GetEditorAsyncById(long id)
        {
            FoundationFixtureItem entity = await _foundationFixtureItemRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(entity,new FoundationFixtureItemDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureItemDto> CreateAsync(FoundationFixtureItemDto input)
        {
            var entity = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(input,new FoundationFixtureItem());
            entity = await _foundationFixtureItemRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(entity,new FoundationFixtureItemDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureItemDto> UpdateAsync(FoundationFixtureItemDto input)
        {
            FoundationFixtureItem entity = await _foundationFixtureItemRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationFixtureItemRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(entity,new FoundationFixtureItemDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationFixtureItemRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
