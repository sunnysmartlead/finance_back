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
    public class ProcessHoursEnterDeviceAppService : ApplicationService
    {
        private readonly IRepository<ProcessHoursEnterDevice, long> _processHoursEnterDeviceRepository;
        private readonly IRepository<ProcessHoursEnter, long> _processHoursEnterRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterDeviceRepository"></param>
        public ProcessHoursEnterDeviceAppService(
            IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository, IRepository<ProcessHoursEnter, long> processHoursEnterRepository)
        {
            _processHoursEnterDeviceRepository = processHoursEnterDeviceRepository;
            _processHoursEnterRepository = processHoursEnterRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDeviceDto> GetAsync(long id)
        {
            ProcessHoursEnterDevice entity = await _processHoursEnterDeviceRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterDevice, ProcessHoursEnterDeviceDto>(entity,new ProcessHoursEnterDeviceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcessHoursEnterDeviceDto>> GetListAsync(GetProcessHoursEnterDevicesInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<ProcessHoursEnterDevice>, List<ProcessHoursEnterDeviceDto>>(list, new List<ProcessHoursEnterDeviceDto>());
            // 数据返回
            return new PagedResultDto<ProcessHoursEnterDeviceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<ProcessHoursEnterDeviceDto>> GetListByAuditFlowIdOrSolutionId(GetProcessHoursEntersInput input)
        {
            // 设置查询条件
            var query = this._processHoursEnterRepository.GetAll();
         
            // 查询数据
            var list = query.ToList();

            List<ProcessHoursEnterDeviceDto> listProcessHoursEnterDeviceDto = new List<ProcessHoursEnterDeviceDto>();
            //数据转换
            foreach (var item in list)
            {
                var processHoursEnterDevicr = this._processHoursEnterDeviceRepository.GetAll().Where(u => u.ProcessHoursEnterId == item.Id && u.DeviceStatus.Equals("Sbzt_Zy")).ToList();
                foreach (var item1 in processHoursEnterDevicr)
                {
                    ProcessHoursEnterDeviceDto p = new ProcessHoursEnterDeviceDto();
                    p.ProcessHoursEnterId = item1.Id;
                    p.DeviceName    = item1.DeviceName;
                    p.DeviceStatus = item1.DeviceStatus;
                    p.DevicePrice = item1.DevicePrice;
                    p.DeviceNumber = item1.DeviceNumber;
                    listProcessHoursEnterDeviceDto.Add(p);
                }
            }
            // 数据返回
            return listProcessHoursEnterDeviceDto;
        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDeviceDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnterDevice entity = await _processHoursEnterDeviceRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnterDevice, ProcessHoursEnterDeviceDto>(entity,new ProcessHoursEnterDeviceDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDeviceDto> CreateAsync(ProcessHoursEnterDeviceDto input)
        {
            var entity = ObjectMapper.Map<ProcessHoursEnterDeviceDto, ProcessHoursEnterDevice>(input,new ProcessHoursEnterDevice());
            entity = await _processHoursEnterDeviceRepository.InsertAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterDevice, ProcessHoursEnterDeviceDto>(entity,new ProcessHoursEnterDeviceDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDeviceDto> UpdateAsync(ProcessHoursEnterDeviceDto input)
        {
            ProcessHoursEnterDevice entity = await _processHoursEnterDeviceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processHoursEnterDeviceRepository.UpdateAsync(entity);
            return ObjectMapper.Map<ProcessHoursEnterDevice, ProcessHoursEnterDeviceDto>(entity,new ProcessHoursEnterDeviceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterDeviceRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
