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
    public class ProcessHoursEnterAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnter, long> _processHoursEnterRepository;
        private readonly IRepository<ProcessHoursEnterDevice, long> _processHoursEnterDeviceRepository;
        private readonly IRepository<ProcessHoursEnterFixture, long> _processHoursEnterFixtureRepository;
        private readonly IRepository<ProcessHoursEnterFrock, long> _processHoursEnterFrockRepository;
        private readonly IRepository<ProcessHoursEnteritem, long> _processHoursEnterItemRepository;
        private readonly IRepository<ProcessHoursEnterLine, long> _processHoursEnterLineRepository;
        private readonly IRepository<ProcessHoursEnterUph, long> _processHoursEnterUphRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterRepository"></param>
        public ProcessHoursEnterAppService(
            IRepository<ProcessHoursEnter, long> processHoursEnterRepository, IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository, IRepository<ProcessHoursEnterFixture, long> processHoursEnterFixtureRepository, IRepository<ProcessHoursEnterFrock, long> processHoursEnterFrockRepository, IRepository<ProcessHoursEnteritem, long> processHoursEnterItemRepository, IRepository<ProcessHoursEnterLine, long> processHoursEnterLineRepository, IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository)
        {
            _processHoursEnterRepository = processHoursEnterRepository;
            _processHoursEnterDeviceRepository = processHoursEnterDeviceRepository;
            _processHoursEnterFixtureRepository = processHoursEnterFixtureRepository;
            _processHoursEnterFrockRepository = processHoursEnterFrockRepository;
            _processHoursEnterItemRepository = processHoursEnterItemRepository;
            _processHoursEnterLineRepository = processHoursEnterLineRepository;
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> GetByIdAsync(long id)
        {
            ProcessHoursEnter entity = await _processHoursEnterRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity,new ProcessHoursEnterDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterDto>> GetListAsync(GetProcessHoursEntersInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnter>, List<ProcessHoursEnterDto>>(list, new List<ProcessHoursEnterDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnter entity = await _processHoursEnterRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity,new ProcessHoursEnterDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> CreateAsync(BomEnterResponseDto input)
        {
            ProcessHoursEnter entity =   new ProcessHoursEnter();
            entity.ProcessName= input.ProcessName;
            entity.ProcessNumber= input.ProcessNumber;
            entity = await _processHoursEnterRepository.InsertAsync(entity);
            var foundationDevice = _processHoursEnterRepository.InsertAndGetId(entity);
            if (null != input.DeviceInfo) {
                
                
              
            
            }
         

            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity,new ProcessHoursEnterDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> UpdateAsync(ProcessHoursEnterDto input)
        {
            ProcessHoursEnter entity = await _processHoursEnterRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity,new ProcessHoursEnterDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
