using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.Processes
{
    /// <summary>
    /// 管理
    /// </summary>
    public class ProcessHoursEnterFixtureAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterFixture, long> _processHoursEnterFixtureRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterFixtureRepository"></param>
        public ProcessHoursEnterFixtureAppService(
            IRepository<ProcessHoursEnterFixture, long> processHoursEnterFixtureRepository)
        {
            _processHoursEnterFixtureRepository = processHoursEnterFixtureRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFixtureDto> GetByIdAsync(long id)
        {
            ProcessHoursEnterFixture entity = await _processHoursEnterFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<ProcessHoursEnterFixture, ProcessHoursEnterFixtureDto>(entity,new ProcessHoursEnterFixtureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterFixtureDto>> GetListAsync(GetProcessHoursEnterFixturesInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnterFixture>, List<ProcessHoursEnterFixtureDto>>(list, new List<ProcessHoursEnterFixtureDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterFixtureDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFixtureDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnterFixture entity = await _processHoursEnterFixtureRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterFixture, ProcessHoursEnterFixtureDto>(entity,new ProcessHoursEnterFixtureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFixtureDto> CreateAsync(ProcessHoursEnterFixtureDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnterFixtureDto, ProcessHoursEnterFixture>(input,new ProcessHoursEnterFixture());
            entity = await _processHoursEnterFixtureRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterFixture, ProcessHoursEnterFixtureDto>(entity,new ProcessHoursEnterFixtureDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterFixtureDto> UpdateAsync(ProcessHoursEnterFixtureDto input)
        {
            ProcessHoursEnterFixture entity = await _processHoursEnterFixtureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterFixtureRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterFixture, ProcessHoursEnterFixtureDto>(entity,new ProcessHoursEnterFixtureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterFixtureRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
