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
    public class FoundationTechnologyHardwareAppService : ApplicationService
    {
        private readonly IRepository<FoundationTechnologyHardware, long> _foundationTechnologyHardwareRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationTechnologyHardwareRepository"></param>
        public FoundationTechnologyHardwareAppService(
            IRepository<FoundationTechnologyHardware, long> foundationTechnologyHardwareRepository)
        {
            _foundationTechnologyHardwareRepository = foundationTechnologyHardwareRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyHardwareDto> GetAsyncById(long id)
        {
            FoundationTechnologyHardware entity = await _foundationTechnologyHardwareRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyHardware, FoundationTechnologyHardwareDto>(entity,new FoundationTechnologyHardwareDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationTechnologyHardwareDto>> GetListAsync(GetFoundationTechnologyHardwaresInput input)
        {
            // 设置查询条件
            var query = this._foundationTechnologyHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationTechnologyHardware>, List<FoundationTechnologyHardwareDto>>(list, new List<FoundationTechnologyHardwareDto>());
            // 数据返回
            return new PagedResultDto<FoundationTechnologyHardwareDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyHardwareDto> GetEditorAsync(long id)
        {
            FoundationTechnologyHardware entity = await _foundationTechnologyHardwareRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyHardware, FoundationTechnologyHardwareDto>(entity,new FoundationTechnologyHardwareDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyHardwareDto> CreateAsync(FoundationTechnologyHardwareDto input)
        {
            var entity = ObjectMapper.Map<FoundationTechnologyHardwareDto, FoundationTechnologyHardware>(input,new FoundationTechnologyHardware());
            entity = await _foundationTechnologyHardwareRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyHardware, FoundationTechnologyHardwareDto>(entity,new FoundationTechnologyHardwareDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyHardwareDto> UpdateAsync(FoundationTechnologyHardwareDto input)
        {
            FoundationTechnologyHardware entity = await _foundationTechnologyHardwareRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationTechnologyHardwareDto, FoundationTechnologyHardware>(input, entity);
            entity = await _foundationTechnologyHardwareRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyHardware, FoundationTechnologyHardwareDto>(entity,new FoundationTechnologyHardwareDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationTechnologyHardwareRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
