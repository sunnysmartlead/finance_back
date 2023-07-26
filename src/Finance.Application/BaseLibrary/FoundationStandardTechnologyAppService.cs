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
    public class FoundationStandardTechnologyAppService : ApplicationService
    {
        private readonly IRepository<FoundationStandardTechnology, long> _foundationStandardTechnologyRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationStandardTechnologyRepository"></param>
        public FoundationStandardTechnologyAppService(
            IRepository<FoundationStandardTechnology, long> foundationStandardTechnologyRepository)
        {
            _foundationStandardTechnologyRepository = foundationStandardTechnologyRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> GetAsyncById(long id)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationStandardTechnologyDto>> GetListAsync(GetFoundationStandardTechnologysInput input)
        {
            // 设置查询条件
            var query = this._foundationStandardTechnologyRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(list, new List<FoundationStandardTechnologyDto>());
            // 数据返回
            return new PagedResultDto<FoundationStandardTechnologyDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> GetEditorAsyncById(long id)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> CreateAsync(FoundationStandardTechnologyDto input)
        {
            var entity = ObjectMapper.Map<FoundationStandardTechnologyDto, FoundationStandardTechnology>(input,new FoundationStandardTechnology());
            entity = await _foundationStandardTechnologyRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> UpdateAsync(long id, FoundationStandardTechnologyDto input)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationStandardTechnologyRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationStandardTechnologyRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
