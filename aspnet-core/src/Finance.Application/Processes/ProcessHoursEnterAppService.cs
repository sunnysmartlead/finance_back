using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
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
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<ProcessHoursEnterDto>> GetListAllAsync(GetProcessHoursEntersInput input)
        {
            // 设置查询条件
            var list = this._processHoursEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId).ToList();
       
            // 查询数据
            //数据转换

           List<ProcessHoursEnterDto> processHoursEnterDtoList = new List<ProcessHoursEnterDto>();
            foreach (var item in list)
            {
                ProcessHoursEnterDto processHoursEnter =   new ProcessHoursEnterDto();
               
                processHoursEnter.Id = item.Id;
                processHoursEnter.ProcessNumber = item.ProcessNumber;
                processHoursEnter.ProcessName   = item.ProcessName;
                //设备的信息
                var listDevice =  _processHoursEnterDeviceRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
                processHoursEnter.DeviceInfo.DeviceTotalCost = item.DeviceTotalPrice;

                List<ProcessHoursEnterDeviceDto> ProcessHoursEnterDeviceDtoList =  new List<ProcessHoursEnterDeviceDto>();
                foreach (var device in listDevice) {

                    ProcessHoursEnterDeviceDto processHoursEnterDeviceDto =     new ProcessHoursEnterDeviceDto();
                    processHoursEnterDeviceDto.DevicePrice = device.DevicePrice;
                    processHoursEnterDeviceDto.ProcessHoursEnterId = device.ProcessHoursEnterId;
                    processHoursEnterDeviceDto.DeviceNumber = device.DeviceNumber;
                    processHoursEnterDeviceDto.DeviceName = device.DeviceName;
                    processHoursEnterDeviceDto.DeviceStatus = device.DeviceStatus;
                    ProcessHoursEnterDeviceDtoList.Add(processHoursEnterDeviceDto);

                }
                processHoursEnter.DeviceInfo.DeviceArr = ProcessHoursEnterDeviceDtoList;

                //追溯部分(硬件及软件开发费用)
                var listFrock =  _processHoursEnterFrockRepository.GetAll().Where(t => t.IsDeleted == false).ToList();

                processHoursEnter.DevelopCostInfo.HardwareTotalPrice = item.HardwareTotalPrice;
                processHoursEnter.DevelopCostInfo.SoftwarePrice= item.SoftwarePrice;
                processHoursEnter.DevelopCostInfo.OpenDrawingSoftware = item.OpenDrawingSoftware;
                processHoursEnter.DevelopCostInfo.HardwareDeviceTotalPrice = item.HardwareTotalPrice;

                List<ProcessHoursEnterFrockDto> ProcessHoursEnterFrockDtoList = new List<ProcessHoursEnterFrockDto>();
                foreach (var device in listFrock)
                {

                    ProcessHoursEnterFrockDto processHoursEnterFrock = new ProcessHoursEnterFrockDto();
                    processHoursEnterFrock.ProcessHoursEnterId = device.Id;
                    processHoursEnterFrock.HardwareDeviceName = device.HardwareDeviceName;
                    processHoursEnterFrock.HardwareDeviceNumber = device.HardwareDeviceNumber;
                    processHoursEnterFrock.HardwareDevicePrice = device.HardwareDevicePrice;
                    ProcessHoursEnterFrockDtoList.Add(processHoursEnterFrock);

                }
                processHoursEnter.DevelopCostInfo.HardwareInfo = ProcessHoursEnterFrockDtoList;




                //工装治具部分
                var listFixture = _processHoursEnterFixtureRepository.GetAll().Where(t => t.IsDeleted == false).ToList();

                processHoursEnter.ToolInfo.FixturePrice = item.FixturePrice;
                processHoursEnter.ToolInfo.FixtureName = item.FixtureName;
                processHoursEnter.ToolInfo.FixtureNumber = item.FixtureNumber;
                processHoursEnter.ToolInfo.FrockPrice = item.FrockPrice;
                processHoursEnter.ToolInfo.FrockName = item.FrockName;
                processHoursEnter.ToolInfo.FrockNumber= item.FrockNumber;
                processHoursEnter.ToolInfo.TestLineName= item.TestLineName;
                processHoursEnter.ToolInfo.TestLineNumber = item.TestLineNumber;
                processHoursEnter.ToolInfo.TestLinePrice= item.TestLinePrice;
                processHoursEnter.ToolInfo.DevelopTotalPrice = item.DevelopTotalPrice;
                List<ProcessHoursEnterFixtureDto> processHoursEnterFixtures = new List<ProcessHoursEnterFixtureDto>();
                foreach (var device in listFixture)
                {

                    ProcessHoursEnterFixtureDto processHoursEnterFixture = new ProcessHoursEnterFixtureDto();
                    processHoursEnterFixture.ProcessHoursEnterId = device.Id;
                    processHoursEnterFixture.FixtureNumber = device.FixtureNumber;
                    processHoursEnterFixture.FixturePrice = device.FixturePrice;
                    processHoursEnterFixture.FixtureName = device.FixtureName;
                    processHoursEnterFixtures.Add(processHoursEnterFixture);

                }
                processHoursEnter.ToolInfo.ZhiJuArr = processHoursEnterFixtures;

                //标准工时
                var queryYear = (from a in _processHoursEnterItemRepository.GetAllList(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id  ).Select(p => p.Year).Distinct()
                             select a).ToList();
                List<ProcessHoursEnterSopInfoDto> processHoursEnteritems = new List<ProcessHoursEnterSopInfoDto>();
                foreach (var device in queryYear)
                {
                    ProcessHoursEnterSopInfoDto processHoursEnteritem =    new ProcessHoursEnterSopInfoDto();
                    processHoursEnteritem.Year = device;
                    var deviceYear = _processHoursEnterItemRepository.GetAll().Where(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id && p.Year ==device ).ToList();
                   List<ProcessHoursEnteritemDto> processHoursEnteritems1 = new List<ProcessHoursEnteritemDto>();
                    foreach (var yearItem in deviceYear)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto=new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = yearItem.LaborHour;
                        processHoursEnteritemDto.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritemDto.MachineHour = yearItem.MachineHour;
                        processHoursEnteritems1.Add(processHoursEnteritemDto);
                    }

                    processHoursEnteritems.Add(processHoursEnteritem);

                    }

                processHoursEnter.SopInfo = processHoursEnteritems;
                processHoursEnterDtoList.Add(processHoursEnter);


            }
            // 数据返回
            return processHoursEnterDtoList;
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
        public virtual async Task CreateAsync(ProcessHoursEnterDto input)
        {
            ProcessHoursEnter entity =   new ProcessHoursEnter();
            entity.ProcessName= input.ProcessName;
            entity.ProcessNumber= input.ProcessNumber;
            entity.SolutionId = input.SolutionId;
            entity.AuditFlowId= input.AuditFlowId;
            entity.DeviceTotalPrice = input.DeviceInfo.DeviceTotalCost;
            entity.HardwareTotalPrice = input.DevelopCostInfo.HardwareTotalPrice;
            entity.SoftwarePrice = input.DevelopCostInfo.SoftwarePrice;
            entity.OpenDrawingSoftware = input.DevelopCostInfo.OpenDrawingSoftware;
            entity.HardwareTotalPrice = input.DevelopCostInfo.HardwareDeviceTotalPrice;
            entity.FixtureName = input.ToolInfo.FixtureName;
            entity.FrockPrice = input.ToolInfo.FrockPrice;
            entity.FixtureNumber = input.ToolInfo.FixtureNumber;
            entity.FrockPrice = input.ToolInfo.FrockPrice;
            entity.FrockName = input.ToolInfo.FrockName;
            entity.FrockNumber = input.ToolInfo.FrockNumber;
            entity.TestLineName = input.ToolInfo.TestLineName;
            entity.TestLineNumber = input.ToolInfo.TestLineNumber;
            entity.TestLinePrice = input.ToolInfo.TestLinePrice;
            entity.DevelopTotalPrice = input.ToolInfo.DevelopTotalPrice;
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _processHoursEnterRepository.InsertAsync(entity);
            var foundationDevice = _processHoursEnterRepository.InsertAndGetId(entity);
            //设备信息
            if (null != input.DeviceInfo.DeviceArr) {
                foreach (var DeviceInfoItem in input.DeviceInfo.DeviceArr)
                {
                    ProcessHoursEnterDevice processHoursEnterDevice = new ProcessHoursEnterDevice();
                    processHoursEnterDevice.ProcessHoursEnterId = foundationDevice;
                    processHoursEnterDevice.DeviceNumber = DeviceInfoItem.DeviceNumber;
                    processHoursEnterDevice.DevicePrice= DeviceInfoItem.DevicePrice;
                    processHoursEnterDevice.DeviceStatus= DeviceInfoItem.DeviceStatus;
                    processHoursEnterDevice.DeviceName = DeviceInfoItem.DeviceName;
                    _processHoursEnterDeviceRepository.InsertAsync(processHoursEnterDevice);
                }
            }
            //追溯部分(硬件及软件开发费用)
            if (null != input.DevelopCostInfo.HardwareInfo)
            {
                foreach (var hardwareInfoItem in input.DevelopCostInfo.HardwareInfo)
                {
                    ProcessHoursEnterFrock processHoursEnterFrock = new ProcessHoursEnterFrock();
                    processHoursEnterFrock.ProcessHoursEnterId = foundationDevice;
                    processHoursEnterFrock.HardwareDevicePrice = hardwareInfoItem.HardwareDevicePrice;
                    processHoursEnterFrock.HardwareDeviceNumber= hardwareInfoItem.HardwareDeviceNumber;
                    processHoursEnterFrock.HardwareDeviceName= hardwareInfoItem.HardwareDeviceName;
                    _processHoursEnterFrockRepository.InsertAsync(processHoursEnterFrock);
                }
            }

            //工装治具部分
            if (null != input.ToolInfo.ZhiJuArr)
            {
                foreach (var zoolInfo in input.ToolInfo.ZhiJuArr)
                {
                    ProcessHoursEnterFixture processHoursEnterFixture = new ProcessHoursEnterFixture();
                    processHoursEnterFixture.ProcessHoursEnterId = foundationDevice;
                    processHoursEnterFixture.FixturePrice = zoolInfo.FixturePrice;
                    processHoursEnterFixture.FixtureNumber= zoolInfo.FixtureNumber;
                    processHoursEnterFixture.FixtureName= zoolInfo.FixtureName;
                    _processHoursEnterFixtureRepository.InsertAsync(processHoursEnterFixture);
                }
            }

            //年
            if (null != input.SopInfo)
            {
                foreach (var year in input.SopInfo)
                {
                    foreach (var yearItem in year.Issues)
                    {
                        ProcessHoursEnteritem processHoursEnteritem =   new ProcessHoursEnteritem();
                        processHoursEnteritem.Year = year.Year;
                        processHoursEnteritem.LaborHour= yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber= yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour= yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }


        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(ProcessHoursEnterDto input)
        {
            ProcessHoursEnter entity = new ProcessHoursEnter();
            entity.ProcessName = input.ProcessName;
            entity.Id = input.Id;
            entity.ProcessNumber = input.ProcessNumber;
            entity.SolutionId = input.SolutionId;
            entity.AuditFlowId = input.AuditFlowId;
            entity.DeviceTotalPrice = input.DeviceInfo.DeviceTotalCost;
            entity.HardwareTotalPrice = input.DevelopCostInfo.HardwareTotalPrice;
            entity.SoftwarePrice = input.DevelopCostInfo.SoftwarePrice;
            entity.OpenDrawingSoftware = input.DevelopCostInfo.OpenDrawingSoftware;
            entity.HardwareTotalPrice = input.DevelopCostInfo.HardwareDeviceTotalPrice;
            entity.FixtureName = input.ToolInfo.FixtureName;
            entity.FrockPrice = input.ToolInfo.FrockPrice;
            entity.FixtureNumber = input.ToolInfo.FixtureNumber;
            entity.FrockPrice = input.ToolInfo.FrockPrice;
            entity.FrockName = input.ToolInfo.FrockName;
            entity.FrockNumber = input.ToolInfo.FrockNumber;
            entity.TestLineName = input.ToolInfo.TestLineName;
            entity.TestLineNumber = input.ToolInfo.TestLineNumber;
            entity.TestLinePrice = input.ToolInfo.TestLinePrice;
            entity.DevelopTotalPrice = input.ToolInfo.DevelopTotalPrice;
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _processHoursEnterRepository.UpdateAsync(entity);
            await _processHoursEnterFixtureRepository.DeleteAsync(s => s.ProcessHoursEnterId == entity.Id);
            await _processHoursEnterDeviceRepository.DeleteAsync(s => s.ProcessHoursEnterId == entity.Id);
            await _processHoursEnterFrockRepository.DeleteAsync(s => s.ProcessHoursEnterId == entity.Id);
            await _processHoursEnterItemRepository.DeleteAsync(s => s.ProcessHoursEnterId == entity.Id);
            //设备信息
            if (null != input.DeviceInfo.DeviceArr)
            {
                foreach (var DeviceInfoItem in input.DeviceInfo.DeviceArr)
                {
                    ProcessHoursEnterDevice processHoursEnterDevice = new ProcessHoursEnterDevice();
                    processHoursEnterDevice.ProcessHoursEnterId = entity.Id;
                    processHoursEnterDevice.DeviceNumber = DeviceInfoItem.DeviceNumber;
                    processHoursEnterDevice.DevicePrice = DeviceInfoItem.DevicePrice;
                    processHoursEnterDevice.DeviceStatus = DeviceInfoItem.DeviceStatus;
                    processHoursEnterDevice.DeviceName = DeviceInfoItem.DeviceName;
                    _processHoursEnterDeviceRepository.InsertAsync(processHoursEnterDevice);
                }
            }
            //追溯部分(硬件及软件开发费用)
            if (null != input.DevelopCostInfo.HardwareInfo)
            {
                foreach (var hardwareInfoItem in input.DevelopCostInfo.HardwareInfo)
                {
                    ProcessHoursEnterFrock processHoursEnterFrock = new ProcessHoursEnterFrock();
                    processHoursEnterFrock.ProcessHoursEnterId = entity.Id;
                    processHoursEnterFrock.HardwareDevicePrice = hardwareInfoItem.HardwareDevicePrice;
                    processHoursEnterFrock.HardwareDeviceNumber = hardwareInfoItem.HardwareDeviceNumber;
                    processHoursEnterFrock.HardwareDeviceName = hardwareInfoItem.HardwareDeviceName;
                    _processHoursEnterFrockRepository.InsertAsync(processHoursEnterFrock);
                }
            }

            //工装治具部分
            if (null != input.ToolInfo.ZhiJuArr)
            {
                foreach (var zoolInfo in input.ToolInfo.ZhiJuArr)
                {
                    ProcessHoursEnterFixture processHoursEnterFixture = new ProcessHoursEnterFixture();
                    processHoursEnterFixture.ProcessHoursEnterId = entity.Id;
                    processHoursEnterFixture.FixturePrice = zoolInfo.FixturePrice;
                    processHoursEnterFixture.FixtureNumber = zoolInfo.FixtureNumber;
                    processHoursEnterFixture.FixtureName = zoolInfo.FixtureName;
                    _processHoursEnterFixtureRepository.InsertAsync(processHoursEnterFixture);
                }
            }

            //年
            if (null != input.SopInfo)
            {
                foreach (var year in input.SopInfo)
                {
                    foreach (var yearItem in year.Issues)
                    {
                        ProcessHoursEnteritem processHoursEnteritem = new ProcessHoursEnteritem();
                        processHoursEnteritem.Year = year.Year;
                        processHoursEnteritem.LaborHour = yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour = yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processHoursEnterRepository.DeleteAsync(s => s.Id == id);
            await _processHoursEnterFixtureRepository.DeleteAsync(s => s.ProcessHoursEnterId == id);
            await _processHoursEnterDeviceRepository.DeleteAsync(s => s.ProcessHoursEnterId == id);
            await _processHoursEnterFrockRepository.DeleteAsync(s => s.ProcessHoursEnterId == id);
            await _processHoursEnterItemRepository.DeleteAsync(s => s.ProcessHoursEnterId == id);
        }
    }
}
