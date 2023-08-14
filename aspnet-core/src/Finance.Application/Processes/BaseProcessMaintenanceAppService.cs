using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.BaseLibrary;
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
    public class BaseProcessMaintenanceAppService : ApplicationService
    {
        private readonly IRepository<BaseProcessMaintenance, long> _baseProcessMaintenanceRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="baseProcessMaintenanceRepository"></param>
        public BaseProcessMaintenanceAppService(
            IRepository<BaseProcessMaintenance, long> baseProcessMaintenanceRepository)
        {
            _baseProcessMaintenanceRepository = baseProcessMaintenanceRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BaseProcessMaintenanceDto> GetByIdAsync(long id)
        {
            BaseProcessMaintenance entity = await _baseProcessMaintenanceRepository.GetAsync(id);
            return ObjectMapper.Map<BaseProcessMaintenance, BaseProcessMaintenanceDto>(entity,new BaseProcessMaintenanceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<BaseProcessMaintenanceDto>> GetListAsync(GetBaseProcessMaintenancesInput input)
        {
            // 设置查询条件
            var query = this._baseProcessMaintenanceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<BaseProcessMaintenance>, List<BaseProcessMaintenanceDto>>(list, new List<BaseProcessMaintenanceDto>());
            // 数据返回
            return new PagedResultDto<BaseProcessMaintenanceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BaseProcessMaintenanceDto> GetEditorByIdAsync(long id)
        {
            BaseProcessMaintenance entity = await _baseProcessMaintenanceRepository.GetAsync(id);

            return ObjectMapper.Map<BaseProcessMaintenance, BaseProcessMaintenanceDto>(entity,new BaseProcessMaintenanceDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BaseProcessMaintenanceDto> CreateAsync(BaseProcessMaintenanceDto input)
        {
            var entity = ObjectMapper.Map<BaseProcessMaintenanceDto, BaseProcessMaintenance>(input,new BaseProcessMaintenance());
            entity = await _baseProcessMaintenanceRepository.InsertAsync(entity);
            return ObjectMapper.Map<BaseProcessMaintenance, BaseProcessMaintenanceDto>(entity,new BaseProcessMaintenanceDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BaseProcessMaintenanceDto> UpdateAsync(BaseProcessMaintenanceDto input)
        {
            BaseProcessMaintenance entity = await _baseProcessMaintenanceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _baseProcessMaintenanceRepository.UpdateAsync(entity);
            return ObjectMapper.Map<BaseProcessMaintenance, BaseProcessMaintenanceDto>(entity,new BaseProcessMaintenanceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _baseProcessMaintenanceRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
