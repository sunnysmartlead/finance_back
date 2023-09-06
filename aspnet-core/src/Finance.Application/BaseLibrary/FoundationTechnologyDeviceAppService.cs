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
    public class FoundationTechnologyDeviceAppService : ApplicationService
    {
        private readonly IRepository<FoundationTechnologyDevice, long> _foundationTechnologyDeviceRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationTechnologyDeviceRepository"></param>
        public FoundationTechnologyDeviceAppService(
            IRepository<FoundationTechnologyDevice, long> foundationTechnologyDeviceRepository)
        {
            _foundationTechnologyDeviceRepository = foundationTechnologyDeviceRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyDeviceDto> GetAsyncById(long id)
        {
            FoundationTechnologyDevice entity = await _foundationTechnologyDeviceRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyDevice, FoundationTechnologyDeviceDto>(entity,new FoundationTechnologyDeviceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationTechnologyDeviceDto>> GetListAsync(GetFoundationTechnologyDevicesInput input)
        {
            // 设置查询条件
            var query = this._foundationTechnologyDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationTechnologyDevice>, List<FoundationTechnologyDeviceDto>>(list, new List<FoundationTechnologyDeviceDto>());
            // 数据
            // 
            return new PagedResultDto<FoundationTechnologyDeviceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyDeviceDto> GetEditorAsyncById(long id)
        {
            FoundationTechnologyDevice entity = await _foundationTechnologyDeviceRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyDevice, FoundationTechnologyDeviceDto>(entity,new FoundationTechnologyDeviceDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyDeviceDto> CreateAsync(FoundationTechnologyDeviceDto input)
        {
            var entity = ObjectMapper.Map<FoundationTechnologyDeviceDto, FoundationTechnologyDevice>(input,new FoundationTechnologyDevice());
            entity = await _foundationTechnologyDeviceRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyDevice, FoundationTechnologyDeviceDto>(entity,new FoundationTechnologyDeviceDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyDeviceDto> UpdateAsync(FoundationTechnologyDeviceDto input)
        {
            FoundationTechnologyDevice entity = await _foundationTechnologyDeviceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationTechnologyDeviceRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyDevice, FoundationTechnologyDeviceDto>(entity,new FoundationTechnologyDeviceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationTechnologyDeviceRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
