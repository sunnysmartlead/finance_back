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
    public class FoundationLogsAppService : ApplicationService
    {
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationLogsRepository"></param>
        public FoundationLogsAppService(
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationLogsRepository = foundationLogsRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationLogsDto> GetAsyncById(long id)
        {
            FoundationLogs entity = await _foundationLogsRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationLogs, FoundationLogsDto>(entity,new FoundationLogsDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationLogsDto>> GetListAsync(GetFoundationLogssInput input)
        {
            // 设置查询条件
            var query = this._foundationLogsRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationLogs>, List<FoundationLogsDto>>(list, new List<FoundationLogsDto>());
            // 数据返回
            return new PagedResultDto<FoundationLogsDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationLogsDto> GetEditorAsyncById(long id)
        {
            FoundationLogs entity = await _foundationLogsRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationLogs, FoundationLogsDto>(entity,new FoundationLogsDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationLogsDto> CreateAsync(FoundationLogsDto input)
        {
            var entity = ObjectMapper.Map<FoundationLogsDto, FoundationLogs>(input,new FoundationLogs());
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationLogs, FoundationLogsDto>(entity,new FoundationLogsDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationLogsDto> UpdateAsync(FoundationLogsDto input)
        {
            FoundationLogs entity = await _foundationLogsRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationLogsRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationLogs, FoundationLogsDto>(entity,new FoundationLogsDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationLogsRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
