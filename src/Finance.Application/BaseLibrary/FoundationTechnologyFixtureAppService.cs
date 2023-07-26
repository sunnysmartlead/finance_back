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
    public class FoundationTechnologyFixtureAppService : ApplicationService
    {
        private readonly IRepository<FoundationTechnologyFixture, long> _foundationTechnologyFixtureRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationTechnologyFixtureRepository"></param>
        public FoundationTechnologyFixtureAppService(
            IRepository<FoundationTechnologyFixture, long> foundationTechnologyFixtureRepository)
        {
            _foundationTechnologyFixtureRepository = foundationTechnologyFixtureRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFixtureDto> GetAsyncById(long id)
        {
            FoundationTechnologyFixture entity = await _foundationTechnologyFixtureRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyFixture, FoundationTechnologyFixtureDto>(entity,new FoundationTechnologyFixtureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationTechnologyFixtureDto>> GetListAsync(GetFoundationTechnologyFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationTechnologyFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationTechnologyFixture>, List<FoundationTechnologyFixtureDto>>(list, new List<FoundationTechnologyFixtureDto>());
            // 数据返回
            return new PagedResultDto<FoundationTechnologyFixtureDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFixtureDto> GetEditorAsyncById(long id)
        {
            FoundationTechnologyFixture entity = await _foundationTechnologyFixtureRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationTechnologyFixture, FoundationTechnologyFixtureDto>(entity,new FoundationTechnologyFixtureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFixtureDto> CreateAsync(FoundationTechnologyFixtureDto input)
        {
            var entity = ObjectMapper.Map<FoundationTechnologyFixtureDto, FoundationTechnologyFixture>(input,new FoundationTechnologyFixture());
            entity = await _foundationTechnologyFixtureRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyFixture, FoundationTechnologyFixtureDto>(entity,new FoundationTechnologyFixtureDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationTechnologyFixtureDto> UpdateAsync(FoundationTechnologyFixtureDto input)
        {
            FoundationTechnologyFixture entity = await _foundationTechnologyFixtureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationTechnologyFixtureDto, FoundationTechnologyFixture>(input, entity);
            entity = await _foundationTechnologyFixtureRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationTechnologyFixture, FoundationTechnologyFixtureDto>(entity,new FoundationTechnologyFixtureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationTechnologyFixtureRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
