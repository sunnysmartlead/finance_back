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
    public class FoundationDeviceAppService : ApplicationService
    {
        private readonly IRepository<FoundationDevice, long> _foundationDeviceRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationDeviceRepository"></param>
        public FoundationDeviceAppService(
            IRepository<FoundationDevice, long> foundationDeviceRepository)
        {
            _foundationDeviceRepository = foundationDeviceRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> GetAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationDeviceDto>> GetListAsync(GetFoundationDevicesInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
            // 数据返回
            return new PagedResultDto<FoundationDeviceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> GetEditorAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity,new FoundationDeviceDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> CreateAsync(FoundationDeviceDto input)
        {
            var entity = ObjectMapper.Map<FoundationDeviceDto, FoundationDevice>(input,new FoundationDevice());
            entity = await this._foundationDeviceRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity,new FoundationDeviceDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> UpdateAsync(FoundationDeviceDto input)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationDeviceRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity,new FoundationDeviceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationDeviceRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
