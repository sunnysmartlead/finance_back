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
    public class FoundationDeviceItemAppService : ApplicationService
    {
        private readonly IRepository<FoundationDeviceItem, long> _foundationDeviceItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationDeviceItemRepository"></param>
        public FoundationDeviceItemAppService(
            IRepository<FoundationDeviceItem, long> foundationDeviceItemRepository)
        {
            _foundationDeviceItemRepository = foundationDeviceItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceItemDto> GetAsyncById(long id)
        {
            FoundationDeviceItem entity = await _foundationDeviceItemRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(entity,new FoundationDeviceItemDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationDeviceItemDto>> GetListAsync(GetFoundationDeviceItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceItemRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(list, new List<FoundationDeviceItemDto>());
            // 数据返回
            return new PagedResultDto<FoundationDeviceItemDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationDeviceItemDto>> GetListAllAsync(GetFoundationDeviceItemsInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceItemRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.DeviceName))
            {
                query = query.Where(t => t.DeviceName.Contains(input.DeviceName));
            }
        
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(list, new List<FoundationDeviceItemDto>());

            // 数据返回
            return dtos;
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceItemDto> GetEditorAsyncById(long id)
        {
            FoundationDeviceItem entity = await _foundationDeviceItemRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(entity,new FoundationDeviceItemDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceItemDto> CreateAsync(FoundationDeviceItemDto input)
        {
            var entity = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(input,new FoundationDeviceItem());
            entity = await _foundationDeviceItemRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(entity, new FoundationDeviceItemDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceItemDto> UpdateAsync(FoundationDeviceItemDto input)
        {
            FoundationDeviceItem entity = await _foundationDeviceItemRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(input, entity);
            entity = await _foundationDeviceItemRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(entity,new FoundationDeviceItemDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationDeviceItemRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
