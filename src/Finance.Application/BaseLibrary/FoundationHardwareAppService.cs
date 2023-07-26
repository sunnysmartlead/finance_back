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
    public class FoundationHardwareAppService : ApplicationService
    {
        private readonly IRepository<FoundationHardware, long> _foundationHardwareRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationHardwareRepository"></param>
        public FoundationHardwareAppService(
            IRepository<FoundationHardware, long> foundationHardwareRepository)
        {
            _foundationHardwareRepository = foundationHardwareRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> GetAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationHardwareDto>> GetListAsync(GetFoundationHardwaresInput input)
        {
            // 设置查询条件
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
            // 数据返回
            return new PagedResultDto<FoundationHardwareDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> GetEditorAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> CreateAsync(FoundationHardwareDto input)
        {
            var entity = ObjectMapper.Map<FoundationHardwareDto, FoundationHardware>(input,new FoundationHardware());
            entity = await _foundationHardwareRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> UpdateAsync(FoundationHardwareDto input)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationHardwareRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationHardwareRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
