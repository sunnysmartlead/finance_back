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
    public class LogisticscostAppService : ApplicationService
    {
        private readonly IRepository<Logisticscost, long> _logisticscostRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logisticscostRepository"></param>
        public LogisticscostAppService(
            IRepository<Logisticscost, long> logisticscostRepository)
        {
            _logisticscostRepository = logisticscostRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);

            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<LogisticscostDto>> GetListAsync(GetLogisticscostsInput input)
        {
            // 设置查询条件
            var query = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(list, new List<LogisticscostDto>());
            // 数据返回
            return new PagedResultDto<LogisticscostDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetEditorAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> CreateAsync(LogisticscostDto input)
        {
            var entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(input,new Logisticscost());
            entity = await _logisticscostRepository.InsertAsync(entity);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> UpdateAsync(LogisticscostDto input)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(input, entity);
            entity = await _logisticscostRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _logisticscostRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
