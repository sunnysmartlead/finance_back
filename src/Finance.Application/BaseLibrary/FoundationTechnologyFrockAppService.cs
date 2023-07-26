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
    public class FoundationTechnologyFrockAppService : ApplicationService
    {
        private readonly IRepository<FoundationTechnologyFrock, long> _foundationTechnologyFrockRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationTechnologyFrockRepository"></param>
        public FoundationTechnologyFrockAppService(
            IRepository<FoundationTechnologyFrock, long> foundationTechnologyFrockRepository)
        {
            _foundationTechnologyFrockRepository = foundationTechnologyFrockRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFrockDto> GetAsyncById(long id)
        {
            FoundationTechnologyFrock entity = await _foundationTechnologyFrockRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyFrock, FoundationTechnologyFrockDto>(entity,new FoundationTechnologyFrockDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationTechnologyFrockDto>> GetListAsync(GetFoundationTechnologyFrocksInput input)
        {
            // 设置查询条件
            var query = this._foundationTechnologyFrockRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationTechnologyFrock>, List<FoundationTechnologyFrockDto>>(list, new List<FoundationTechnologyFrockDto>());
            // 数据返回
            return new PagedResultDto<FoundationTechnologyFrockDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFrockDto> GetEditorAsyncById(long id)
        {
            FoundationTechnologyFrock entity = await _foundationTechnologyFrockRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyFrock, FoundationTechnologyFrockDto>(entity,new FoundationTechnologyFrockDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFrockDto> CreateAsync(FoundationTechnologyFrockDto input)
        {
            var entity = ObjectMapper.Map<FoundationTechnologyFrockDto, FoundationTechnologyFrock>(input,new FoundationTechnologyFrock());
            entity = await _foundationTechnologyFrockRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyFrock, FoundationTechnologyFrockDto>(entity,new FoundationTechnologyFrockDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFrockDto> UpdateAsync(FoundationTechnologyFrockDto input)
        {
            FoundationTechnologyFrock entity = await _foundationTechnologyFrockRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationTechnologyFrockDto, FoundationTechnologyFrock>(input, entity);
            entity = await _foundationTechnologyFrockRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyFrock, FoundationTechnologyFrockDto>(entity,new FoundationTechnologyFrockDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationTechnologyFrockRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
