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
    public class FoundationFixtureAppService : ApplicationService
    {
        private readonly IRepository<FoundationFixture, long> _foundationFixtureRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationFixtureRepository"></param>
        public FoundationFixtureAppService(
            IRepository<FoundationFixture, long> foundationFixtureRepository)
        {
            _foundationFixtureRepository = foundationFixtureRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> GetAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationFixtureDto>> GetListAsync(GetFoundationFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            // 数据返回
            return new PagedResultDto<FoundationFixtureDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> GetEditorAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> CreateAsync(FoundationFixtureDto input)
        {
            var entity = ObjectMapper.Map<FoundationFixtureDto, FoundationFixture>(input,new FoundationFixture());
            entity = await _foundationFixtureRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> UpdateAsync(FoundationFixtureDto input)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationFixtureRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationFixtureRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
