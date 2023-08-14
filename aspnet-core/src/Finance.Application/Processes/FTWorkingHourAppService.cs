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
    public class FTWorkingHourAppService : ApplicationService
    {
        private readonly IRepository<FTWorkingHour, long> _fTWorkingHourRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="fTWorkingHourRepository"></param>
        public FTWorkingHourAppService(
            IRepository<FTWorkingHour, long> fTWorkingHourRepository)
        {
            _fTWorkingHourRepository = fTWorkingHourRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FTWorkingHourDto> GetByIdAsync(long id)
        {
            FTWorkingHour entity = await _fTWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FTWorkingHour, FTWorkingHourDto>(entity,new FTWorkingHourDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FTWorkingHourDto>> GetListAsync(GetFTWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._fTWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FTWorkingHour>, List<FTWorkingHourDto>>(list, new List<FTWorkingHourDto>());
            // 数据返回
            return new PagedResultDto<FTWorkingHourDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FTWorkingHourDto> GetEditorAsync(long id)
        {
            FTWorkingHour entity = await _fTWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FTWorkingHour, FTWorkingHourDto>(entity,new FTWorkingHourDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FTWorkingHourDto> CreateAsync(FTWorkingHourDto input)
        {
            var entity = ObjectMapper.Map<FTWorkingHourDto, FTWorkingHour>(input,new FTWorkingHour());
            entity.CreationTime = DateTime.Now;
            entity.Isdeleted1 = 0;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _fTWorkingHourRepository.InsertAsync(entity);
            return ObjectMapper.Map<FTWorkingHour, FTWorkingHourDto>(entity,new FTWorkingHourDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FTWorkingHourDto> UpdateAsync(FTWorkingHourDto input)
        {
            FTWorkingHour entity = await _fTWorkingHourRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _fTWorkingHourRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FTWorkingHour, FTWorkingHourDto>(entity,new FTWorkingHourDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _fTWorkingHourRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
