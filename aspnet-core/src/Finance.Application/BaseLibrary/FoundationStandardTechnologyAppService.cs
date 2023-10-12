using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Spire.Pdf.General.Paper.Uof;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationStandardTechnologyAppService : ApplicationService
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.GrossProfitMargin;
        private readonly IRepository<FoundationStandardTechnology, long> _foundationStandardTechnologyRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationReliableProcessHours, long> _foundationFoundationReliableProcessHoursRepository;
        private readonly IRepository<FoundationTechnologyDevice, long> _foundationTechnologyDeviceRepository;
        private readonly IRepository<FoundationTechnologyHardware, long> _foundationTechnologyHardwareRepository;
        private readonly IRepository<FoundationTechnologyFixture, long> _foundationTechnologyFixtureRepository;
        private readonly IRepository<FTWorkingHour, long> _fTWorkingHourRepository;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;


        /// <summary>
        /// .ctorFRProcessHours
        /// </summary>
        /// <param name="foundationStandardTechnologyRepository"></param>
        public FoundationStandardTechnologyAppService(
   
            IRepository<FTWorkingHour, long> fTWorkingHourRepository,
            IRepository<FoundationTechnologyFixture, long> foundationTechnologyFixtureRepository,
            IRepository<FoundationTechnologyHardware, long> foundationTechnologyHardwareRepository,
            IRepository<FoundationTechnologyDevice, long> foundationTechnologyDeviceRepository,
            IRepository<FoundationReliableProcessHours, long> foundationFoundationReliableProcessHoursRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<Solution, long> resourceSchemeTable,
            IRepository<FoundationStandardTechnology, long> foundationStandardTechnologyRepository)
        {
            _foundationStandardTechnologyRepository = foundationStandardTechnologyRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationReliableProcessHoursRepository = foundationFoundationReliableProcessHoursRepository;
            _foundationTechnologyDeviceRepository = foundationTechnologyDeviceRepository;
            _foundationTechnologyHardwareRepository = foundationTechnologyHardwareRepository;
            _foundationTechnologyFixtureRepository = foundationTechnologyFixtureRepository;
            _fTWorkingHourRepository = fTWorkingHourRepository;
            _resourceSchemeTable = resourceSchemeTable;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> GetAsyncById(long id)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);
            FoundationStandardTechnologyDto foundationStandardTechnologyDto =   new FoundationStandardTechnologyDto();
            List<FoundationReliableProcessHoursResponseDto> foundation =  new List<FoundationReliableProcessHoursResponseDto>();
           var dtos =  _foundationFoundationReliableProcessHoursRepository.GetAll().Where(s => s.StandardTechnologyId == entity.Id && s.IsDeleted == false).ToList();
            foreach (var item in dtos)
            {
                FoundationReliableProcessHoursResponseDto f =   new FoundationReliableProcessHoursResponseDto();
               var Device =  _foundationTechnologyDeviceRepository.GetAll().Where(s => s.FoundationReliableHoursId == item.Id && s.IsDeleted == false).ToList();
                f.DeviceInfo.DeviceTotalPrice = item.DeviceTotalPrice;
                List < FoundationTechnologyDeviceDto > fd= new List<FoundationTechnologyDeviceDto>();
                foreach (var DeviceInfoitem in Device)
                {
                    FoundationTechnologyDeviceDto foundationTechnology =   new FoundationTechnologyDeviceDto();
                    foundationTechnology.DevicePrice = DeviceInfoitem.DevicePrice;
                    foundationTechnology.DeviceNumber = DeviceInfoitem.DeviceNumber;
                    foundationTechnology.DeviceStatus   = DeviceInfoitem.DeviceStatus;
                    foundationTechnology.DeviceName = DeviceInfoitem.DeviceName;
                }
                f.DeviceInfo.DeviceArr = fd;

                _foundationTechnologyHardwareRepository.GetAll().Where(s => s.FoundationReliableHoursId == item.Id && s.IsDeleted == false).ToList();



                _foundationTechnologyFixtureRepository.GetAll().Where(s => s.FoundationReliableHoursId == item.Id && s.IsDeleted == false).ToList();

                var Working = _fTWorkingHourRepository.GetAll().Where(s => s.FoundationReliableHoursId == item.Id && s.IsDeleted == false).ToList();
                f.sopInfo = ObjectMapper.Map<List<FTWorkingHour>, List<FoundationWorkingHourItemDto>>(Working, new List<FoundationWorkingHourItemDto>());

             
            }
            foundationStandardTechnologyDto.List = foundation;
            foundationStandardTechnologyDto.Id = entity.Id;
            entity.Name= entity.Name;


            return foundationStandardTechnologyDto;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationStandardTechnologyDto>> GetListAsync()
        {
            // 设置查询条件
            var query = this._foundationStandardTechnologyRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(query, new List<FoundationStandardTechnologyDto>());
            // 数据返回
            return new PagedResultDto<FoundationStandardTechnologyDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationStandardTechnologyDto>> GetListAllAsync(GetFoundationStandardTechnologysInput input)
        {
            // 设置查询条件
            var query = this._foundationStandardTechnologyRepository.GetAll().Where(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(input.Name))
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(list, new List<FoundationStandardTechnologyDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                List<FoundationReliableProcessHoursResponseDto> foundationReliableProcessHoursResponseDtos = new List<FoundationReliableProcessHoursResponseDto>();

             List<FoundationReliableProcessHours> FoundationReliableProcessHourList =  this._foundationFoundationReliableProcessHoursRepository.GetAll().Where(u => u.StandardTechnologyId == item.Id).ToList();

                foreach (var foundationReliableProcessHours in FoundationReliableProcessHourList)
                {
                    FoundationReliableProcessHoursResponseDto foundationReliableProcess = new FoundationReliableProcessHoursResponseDto();
                    //设备信息
                    List<FoundationTechnologyDevice> devices = this._foundationTechnologyDeviceRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    List<FoundationTechnologyDeviceDto> foundationTechnologyDeviceDtos = new List<FoundationTechnologyDeviceDto>();
                    foreach (var device in devices)
                    {
                        FoundationTechnologyDeviceDto foundationTechnologyDevice = new FoundationTechnologyDeviceDto();
                        foundationTechnologyDevice.DevicePrice = device.DevicePrice;
                        foundationTechnologyDevice.DeviceNumber = device.DeviceNumber;
                        foundationTechnologyDevice.DeviceName = device.DeviceName;
                        foundationTechnologyDevice.DeviceStatus = device.DeviceStatus;
                        foundationTechnologyDeviceDtos.Add(foundationTechnologyDevice);

                    }
                    foundationReliableProcess.DeviceInfo.DeviceArr = foundationTechnologyDeviceDtos;
                    foundationReliableProcess.DeviceInfo.DeviceTotalPrice = foundationReliableProcessHours.DeviceTotalPrice;
                    //追溯部分(硬件及软件开发费用)
                    List<FoundationTechnologyFrockDto> foundationTechnologyFrockDtos = new List<FoundationTechnologyFrockDto>();
                    List<FoundationTechnologyHardware> foundationTechnologyHardwares = this._foundationTechnologyHardwareRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    {
                        foreach (var device in foundationTechnologyHardwares)
                        {
                            FoundationTechnologyFrockDto technologyHardware = new FoundationTechnologyFrockDto();
                            technologyHardware.HardwareDeviceName = device.HardwareName;
                            technologyHardware.HardwareDeviceNumber = device.HardwareNumber;
                            technologyHardware.HardwareDevicePrice = device.HardwarePrice;
                            foundationTechnologyFrockDtos.Add(technologyHardware);



                        }

                    }
                    foundationReliableProcess.DevelopCostInfo.TotalHardwarePrice = foundationReliableProcessHours.SoftwareHardPrice;
                    foundationReliableProcess.DevelopCostInfo.PictureDevelopment = foundationReliableProcessHours.PictureDevelopment;
                    foundationReliableProcess.DevelopCostInfo.DrawingSoftware = foundationReliableProcessHours.DrawingSoftware;
                    foundationReliableProcess.DevelopCostInfo.Development = foundationReliableProcessHours.Development;
                    foundationReliableProcess.DevelopCostInfo.TraceabilitySoftware = foundationReliableProcessHours.TraceabilitySoftware;
                    foundationReliableProcess.DevelopCostInfo.HardwareInfo = foundationTechnologyFrockDtos;

                    //工装治具部分
                    List<FoundationTechnologyFixtureDto> foundationTechnologyFixtureDtos = new List<FoundationTechnologyFixtureDto>();
                    List<FoundationTechnologyFixture> foundationTechnologyFixtureList = this._foundationTechnologyFixtureRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    {
                        foreach (var device in foundationTechnologyFixtureList)
                        {
                            FoundationTechnologyFixtureDto technologyHardware = new FoundationTechnologyFixtureDto();
                            technologyHardware.FixturePrice = device.FixturePrice;
                            technologyHardware.FixtureNumber = device.FixtureNumber;
                            technologyHardware.FixtureName = device.FixtureName;
                            foundationTechnologyFixtureDtos.Add(technologyHardware);



                        }

                    }
                    foundationReliableProcess.toolInfo.zhiJuArr = foundationTechnologyFixtureDtos;
                    foundationReliableProcess.toolInfo.TestLineName = foundationReliableProcessHours.TestLineName;
                    foundationReliableProcess.toolInfo.TestLineNumber = foundationReliableProcessHours.TestLineNumber;
                    foundationReliableProcess.toolInfo.TestLinePrice = foundationReliableProcessHours.TestLinePrice;
                    foundationReliableProcess.toolInfo.FrockName = foundationReliableProcessHours.FrockName;
                    foundationReliableProcess.toolInfo.FrockNumber = foundationReliableProcessHours.FrockNumber;
                    foundationReliableProcess.toolInfo.FrockPrice = 0;
                    foundationReliableProcess.toolInfo.HardwareDeviceTotalPrice = foundationReliableProcessHours.HardwareDeviceTotalPrice;
                    foundationReliableProcess.toolInfo.SoftwarePrice = foundationReliableProcessHours.SoftwarePrice;
                    foundationReliableProcess.toolInfo.FixtureName = foundationReliableProcessHours.FrockName;
                    foundationReliableProcess.toolInfo.FixtureNumber = 0;


                    List<FoundationWorkingHourItemDto> foundationWorkingHourItemDtos = new List<FoundationWorkingHourItemDto>();
                    var foundationWorkingHourItemDtosList = this._fTWorkingHourRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    {
                        foreach (var device in foundationWorkingHourItemDtosList)
                        {
                            FoundationWorkingHourItemDto technologyHardware = new FoundationWorkingHourItemDto();
                            technologyHardware.Year = device.Year;
                            technologyHardware.LaborHour = device.LaborHour;
                            technologyHardware.MachineHour = device.MachineHour;
                            technologyHardware.NumberPersonnel = device.NumberPersonnel;
                            foundationWorkingHourItemDtos.Add(technologyHardware);



                        }

                    }
                    foundationReliableProcess.sopInfo = foundationWorkingHourItemDtos;
                    foundationReliableProcess.ProcessName= foundationReliableProcessHours.ProcessName;
                    foundationReliableProcess.ProcessNumber = foundationReliableProcessHours.ProcessNumber;


                    foundationReliableProcessHoursResponseDtos.Add(foundationReliableProcess);

                }
                item.List = foundationReliableProcessHoursResponseDtos;

            }
            // 数据返回
            return dtos;
        }




        /// <summary>
        /// 列表-无分页功能 工时工序导入界面专用
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationStandardTechnologyDto>> GetListAllOnlyAsync(GetFoundationStandardTechnologysInput input)
        {
            // 设置查询条件
            var query = this._foundationStandardTechnologyRepository.GetAll().Where(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(input.Name))
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(list, new List<FoundationStandardTechnologyDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                List<ProcessHoursEnterDto> processHoursEnterDtoList = new List<ProcessHoursEnterDto>();

               var FoundationReliableProcessHourList = this._foundationFoundationReliableProcessHoursRepository.GetAll().Where(u => u.StandardTechnologyId == item.Id && u.IsDeleted == false).ToList();

                foreach (var foundationReliableProcessHours in FoundationReliableProcessHourList)
                {
                    ProcessHoursEnterDto foundationReliableProcess = new ProcessHoursEnterDto();
                    //设备信息
                    List<FoundationTechnologyDevice> devices = this._foundationTechnologyDeviceRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    List<ProcessHoursEnterDeviceDto> foundationTechnologyDeviceDtos = new List<ProcessHoursEnterDeviceDto>();
                    foreach (var device in devices)
                    {
                        ProcessHoursEnterDeviceDto foundationTechnologyDevice = new ProcessHoursEnterDeviceDto();
                        foundationTechnologyDevice.DevicePrice = decimal.Parse(device.DevicePrice);
                        foundationTechnologyDevice.DeviceNumber = decimal.Parse(device.DeviceNumber);
                        foundationTechnologyDevice.DeviceName = device.DeviceName;
                        foundationTechnologyDevice.DeviceStatus = device.DeviceStatus;
                        foundationTechnologyDeviceDtos.Add(foundationTechnologyDevice);

                    }
                    foundationReliableProcess.DeviceInfo.DeviceArr = foundationTechnologyDeviceDtos;
                    if (null != foundationReliableProcessHours.DeviceTotalPrice)
                    {
                        foundationReliableProcess.DeviceInfo.DeviceTotalCost = (long)foundationReliableProcessHours.DeviceTotalPrice;
                    }
                    else
                    {
                        foundationReliableProcess.DeviceInfo.DeviceTotalCost = 0;
                    }
                    //追溯部分(硬件及软件开发费用)
                    List<ProcessHoursEnterFrockDto> foundationTechnologyFrockDtos = new List<ProcessHoursEnterFrockDto>();
                    List<FoundationTechnologyHardware> foundationTechnologyHardwares = this._foundationTechnologyHardwareRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    {
                        foreach (var device in foundationTechnologyHardwares)
                        {
                            ProcessHoursEnterFrockDto technologyHardware = new ProcessHoursEnterFrockDto();
                            technologyHardware.HardwareDeviceName = device.HardwareName;
                            technologyHardware.HardwareDeviceNumber = device.HardwareNumber;
                            technologyHardware.HardwareDevicePrice = device.HardwarePrice;
                            foundationTechnologyFrockDtos.Add(technologyHardware);
                        }
                    }
                    if (null != foundationReliableProcessHours.SoftwareHardPrice)
                    {
                        foundationReliableProcess.DevelopCostInfo.TotalHardwarePrice = (long)foundationReliableProcessHours.SoftwareHardPrice;
                    }
                    else {
                        foundationReliableProcess.DevelopCostInfo.TotalHardwarePrice = 0;
                    }
                    if (null != foundationReliableProcessHours.PictureDevelopment)
                    {
                        foundationReliableProcess.DevelopCostInfo.PictureDevelopment = (long)foundationReliableProcessHours.PictureDevelopment;
                    }
                    else
                    {
                        foundationReliableProcess.DevelopCostInfo.PictureDevelopment = 0;
                    }
                    if (null != foundationReliableProcessHours.Development)
                    {
                        foundationReliableProcess.DevelopCostInfo.Development = (long)foundationReliableProcessHours.Development;
                    }
                    else
                    {
                        foundationReliableProcess.DevelopCostInfo.Development = 0;
                    }
                    foundationReliableProcess.DevelopCostInfo.OpenDrawingSoftware = foundationReliableProcessHours.DrawingSoftware;
                    foundationReliableProcess.DevelopCostInfo.TraceabilitySoftware = foundationReliableProcessHours.TraceabilitySoftware;
                    foundationReliableProcess.DevelopCostInfo.HardwareInfo = foundationTechnologyFrockDtos;

                    //工装治具部分
                    List<ProcessHoursEnterFixtureDto> foundationTechnologyFixtureDtos = new List<ProcessHoursEnterFixtureDto>();
                    List<FoundationTechnologyFixture> foundationTechnologyFixtureList = this._foundationTechnologyFixtureRepository.GetAll().Where(t => t.FoundationReliableHoursId == foundationReliableProcessHours.Id).ToList();
                    {
                        foreach (var device in foundationTechnologyFixtureList)
                        {
                            ProcessHoursEnterFixtureDto technologyHardware = new ProcessHoursEnterFixtureDto();
                            technologyHardware.FixturePrice = device.FixturePrice;
                            technologyHardware.FixtureNumber = device.FixtureNumber;
                            technologyHardware.FixtureName = device.FixtureName;
                            foundationTechnologyFixtureDtos.Add(technologyHardware);



                        }

                    }
                    foundationReliableProcess.ToolInfo.ZhiJuArr = foundationTechnologyFixtureDtos;
                    foundationReliableProcess.ToolInfo.TestLineName = foundationReliableProcessHours.TestLineName;
                    if (null != foundationReliableProcessHours.TestLineNumber)
                    {
                        foundationReliableProcess.ToolInfo.TestLineNumber =(long) foundationReliableProcessHours.TestLineNumber;
                    }
                    else
                    {
                        foundationReliableProcess.ToolInfo.TestLineNumber = 0;
                    }
                    foundationReliableProcess.ToolInfo.TestLinePrice = foundationReliableProcessHours.TestLinePrice;
                    foundationReliableProcess.ToolInfo.FrockName = foundationReliableProcessHours.FrockName;
                    if (null != foundationReliableProcessHours.FrockNumber)
                    {
                        foundationReliableProcess.ToolInfo.FrockNumber = (long)foundationReliableProcessHours.FrockNumber;
                    }
                    else
                    {
                        foundationReliableProcess.ToolInfo.FrockNumber = 0;
                    }
                    foundationReliableProcess.ToolInfo.FrockPrice = 0;
                    foundationReliableProcess.ToolInfo.DevelopTotalPrice =  foundationReliableProcessHours.HardwareDeviceTotalPrice.ToString();

                    if (null != foundationReliableProcessHours.SoftwarePrice)
                    {
                        foundationReliableProcess.ToolInfo.SoftwarePrice = (long)foundationReliableProcessHours.SoftwarePrice;
                    }
                    else
                    {
                        foundationReliableProcess.ToolInfo.SoftwarePrice = 0;
                    }
                    foundationReliableProcess.ToolInfo.FixtureName = foundationReliableProcessHours.FixtureName;
                    foundationReliableProcess.ToolInfo.FixtureNumber = foundationReliableProcessHours.FixtureNumber;


                    List<ProcessHoursEnterSopInfoDto> foundationWorkingHourItemDtos = new List<ProcessHoursEnterSopInfoDto>();

                    var queryYear = (from a in _fTWorkingHourRepository.GetAllList(p => p.IsDeleted == false && p.FoundationReliableHoursId == item.Id).Select(p => p.Year).Distinct()
                                     select a).ToList();
                    foreach (var year in queryYear)
                    {
                        ProcessHoursEnterSopInfoDto processHoursEnteritemDto = new ProcessHoursEnterSopInfoDto();
                        processHoursEnteritemDto.Year = year;
                        List<ProcessHoursEnteritemDto> foundationWorkingHourItemDtosItem = new List<ProcessHoursEnteritemDto>();
                        List<FTWorkingHour> foundationWorkingHourItemDtosList =  _fTWorkingHourRepository.GetAllList(p => p.IsDeleted == false && p.FoundationReliableHoursId == item.Id &&  p.Year == year);
                        foreach (var device in foundationWorkingHourItemDtosList)
                        {
                            ProcessHoursEnteritemDto technologyHardware = new ProcessHoursEnteritemDto();
                            technologyHardware.Year = device.Year;
                            technologyHardware.LaborHour = decimal.Parse(device.LaborHour);
                            technologyHardware.MachineHour = decimal.Parse(device.MachineHour);
                            technologyHardware.PersonnelNumber = decimal.Parse(device.NumberPersonnel);
                            foundationWorkingHourItemDtosItem.Add(technologyHardware);



                        }
                        processHoursEnteritemDto.Issues = foundationWorkingHourItemDtosItem;
                        foundationWorkingHourItemDtos.Add(processHoursEnteritemDto);
                    }
                    


               
                    foundationReliableProcess.SopInfo = foundationWorkingHourItemDtos;
                    foundationReliableProcess.ProcessName = foundationReliableProcessHours.ProcessName;
                    foundationReliableProcess.ProcessNumber = foundationReliableProcessHours.ProcessNumber;


                    processHoursEnterDtoList.Add(foundationReliableProcess);

                }
                item.ProcessHoursEnterDtoList = processHoursEnterDtoList;

            }
            // 数据返回
            return dtos;
        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> GetEditorAsyncById(long id)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity, new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(FoundationStandardTechnologyDto input)
        {
      
            var entity = ObjectMapper.Map<FoundationStandardTechnologyDto, FoundationStandardTechnology>(input, new FoundationStandardTechnology());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationStandardTechnologyRepository.InsertAsync(entity);
            var foundationDevice = _foundationStandardTechnologyRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity, new FoundationStandardTechnologyDto());
        
        

          
            if (input.List != null)
            {
                foreach (var deviceItem in input.List)
                {
                    FoundationReliableProcessHours foundationReliableProcessHours = new FoundationReliableProcessHours();
                    foundationReliableProcessHours.ProcessName = deviceItem.ProcessName;
                    foundationReliableProcessHours.ProcessNumber = deviceItem.ProcessNumber;
                    foundationReliableProcessHours.SoftwarePrice = deviceItem.DeviceInfo.DeviceTotalPrice;
                    //设备保存
                    foundationReliableProcessHours.TotalHardwarePrice = deviceItem.DevelopCostInfo.SoftwareHardPrice;
                    foundationReliableProcessHours.PictureDevelopment = deviceItem.DevelopCostInfo.PictureDevelopment;
                    foundationReliableProcessHours.DrawingSoftware = deviceItem.DevelopCostInfo.DrawingSoftware;
                    foundationReliableProcessHours.Development = deviceItem.DevelopCostInfo.Development;
                    foundationReliableProcessHours.TraceabilitySoftware = deviceItem.DevelopCostInfo.TraceabilitySoftware;
                    //软硬件保存


                    //工装治具
                    foundationReliableProcessHours.DevelopTotalPrice = deviceItem.toolInfo.HardwareDeviceTotalPrice;
                    foundationReliableProcessHours.TestLineName = deviceItem.toolInfo.TestLineName;
                    foundationReliableProcessHours.TestLineNumber = deviceItem.toolInfo.TestLineNumber;
                    foundationReliableProcessHours.TestLinePrice = deviceItem.toolInfo.TestLinePrice;
                    //缺少字段
                    foundationReliableProcessHours.FrockPrice = deviceItem.toolInfo.FrockPrice;
                    foundationReliableProcessHours.FrockName = deviceItem.toolInfo.FrockName;
                    foundationReliableProcessHours.FrockNumber = deviceItem.toolInfo.FrockNumber;
                    foundationReliableProcessHours.FixturePrice = deviceItem.toolInfo.FixturePrice;
                    foundationReliableProcessHours.FixtureNumber = deviceItem.toolInfo.FixtureNumber;
                    foundationReliableProcessHours.FixtureName = deviceItem.toolInfo.FixtureName;


                    //工时





                    foundationReliableProcessHours.CreationTime = DateTime.Now;
                    if (AbpSession.UserId != null)
                    {
                        foundationReliableProcessHours.CreatorUserId = AbpSession.UserId.Value;
                        foundationReliableProcessHours.LastModificationTime = DateTime.Now;
                        foundationReliableProcessHours.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    foundationReliableProcessHours.LastModificationTime = DateTime.Now;
                    foundationReliableProcessHours.StandardTechnologyId = foundationDevice;
                    _foundationFoundationReliableProcessHoursRepository.InsertAsync(foundationReliableProcessHours);

                    var ID = _foundationFoundationReliableProcessHoursRepository.InsertAndGetId(foundationReliableProcessHours);
                    if (null != deviceItem.DeviceInfo.DeviceArr)
                    {
                        foreach (var device in deviceItem.DeviceInfo.DeviceArr)
                        {
                            FoundationTechnologyDevice technologyDevice = new FoundationTechnologyDevice();
                            technologyDevice.DeviceName = device.DeviceName;
                            technologyDevice.DevicePrice = device.DevicePrice;
                            technologyDevice.DeviceStatus = device.DeviceStatus;
                            technologyDevice.DeviceNumber = device.DeviceNumber;
                            technologyDevice.CreationTime = DateTime.Now;
                            technologyDevice.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyDevice.CreatorUserId = AbpSession.UserId.Value;
                                technologyDevice.LastModificationTime = DateTime.Now;
                                technologyDevice.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyDeviceRepository.InsertAsync(technologyDevice);

                        }
                    }
                    if (null != deviceItem.DevelopCostInfo.HardwareInfo)
                    {
                        foreach (var device in deviceItem.DevelopCostInfo.HardwareInfo)
                        {
                            FoundationTechnologyHardware technologyHardware = new FoundationTechnologyHardware();
                            technologyHardware.HardwarePrice = device.HardwareDevicePrice;
                            technologyHardware.HardwareName = device.HardwareDeviceName;
                            technologyHardware.HardwareNumber = device.HardwareDeviceNumber;
                            technologyHardware.CreationTime = DateTime.Now;
                            technologyHardware.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyHardware.CreatorUserId = AbpSession.UserId.Value;
                                technologyHardware.LastModificationTime = DateTime.Now;
                                technologyHardware.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyHardwareRepository.InsertAsync(technologyHardware);

                        }
                    }

                    if (null != deviceItem.toolInfo.zhiJuArr)
                    {
                        foreach (var device in deviceItem.toolInfo.zhiJuArr)
                        {
                            FoundationTechnologyFixture technologyFixture = new FoundationTechnologyFixture();
                            technologyFixture.FixturePrice = device.FixturePrice;
                            technologyFixture.FixtureName = device.FixtureName;
                            technologyFixture.FixtureNumber = device.FixtureNumber;
                            technologyFixture.CreationTime = DateTime.Now;
                            technologyFixture.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyFixture.CreatorUserId = AbpSession.UserId.Value;
                                technologyFixture.LastModificationTime = DateTime.Now;
                                technologyFixture.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyFixtureRepository.InsertAsync(technologyFixture);

                        }
                    }



                    if (null != deviceItem.sopInfo)
                    {
                        foreach (var device in deviceItem.sopInfo)
                        {
                            FTWorkingHour technologyWorkingHour = new FTWorkingHour();
                            technologyWorkingHour.Year = device.Year;
                            technologyWorkingHour.NumberPersonnel = device.NumberPersonnel;
                            technologyWorkingHour.MachineHour = device.MachineHour;
                            technologyWorkingHour.LaborHour = device.LaborHour;
                            technologyWorkingHour.CreationTime = DateTime.Now;
                            technologyWorkingHour.FoundationReliableHoursId = ID;
                            technologyWorkingHour.Isdeleted1 = 1;
                            if (AbpSession.UserId != null)
                            {
                                technologyWorkingHour.CreatorUserId = AbpSession.UserId.Value;
                                technologyWorkingHour.LastModificationTime = DateTime.Now;
                                technologyWorkingHour.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            try
                            {

                                _fTWorkingHourRepository.InsertAsync(technologyWorkingHour);
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                    }
               
               
                }

             this.CreateLog(" 新增标准工艺库项目1条");



        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> UpdateAsync(FoundationStandardTechnologyDto input)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(input.Id);
            entity.Name =   input.Name;
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationStandardTechnologyRepository.UpdateAsync(entity);
            var result =  ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity, new FoundationStandardTechnologyDto());

            if (input.List != null)
            {
                var query = this._foundationFoundationReliableProcessHoursRepository.GetAll().Where(t => t.IsDeleted == false && t.StandardTechnologyId == input.Id).ToList();
                foreach (var item in query) {
                    await _foundationTechnologyDeviceRepository.DeleteAsync(s => s.FoundationReliableHoursId == item.Id);
                    await _foundationFoundationReliableProcessHoursRepository.DeleteAsync(s => s.Id == item.Id);
                    await _foundationTechnologyHardwareRepository.DeleteAsync(s => s.FoundationReliableHoursId == item.Id);
                    await _foundationTechnologyFixtureRepository.DeleteAsync(s => s.FoundationReliableHoursId == item.Id);
                    await _fTWorkingHourRepository.DeleteAsync(s => s.FoundationReliableHoursId == item.Id);
                }
          
                foreach (var deviceItem in input.List)
                {
                    FoundationReliableProcessHours foundationReliableProcessHours = new FoundationReliableProcessHours();
                    foundationReliableProcessHours.ProcessName = deviceItem.ProcessName;
                    foundationReliableProcessHours.ProcessNumber = deviceItem.ProcessNumber;
                    foundationReliableProcessHours.SoftwarePrice = deviceItem.DeviceInfo.DeviceTotalPrice;
                    //设备保存
                    foundationReliableProcessHours.TotalHardwarePrice = deviceItem.DevelopCostInfo.SoftwareHardPrice;
                    foundationReliableProcessHours.PictureDevelopment = deviceItem.DevelopCostInfo.PictureDevelopment;
                    foundationReliableProcessHours.DrawingSoftware = deviceItem.DevelopCostInfo.DrawingSoftware;
                    foundationReliableProcessHours.Development = deviceItem.DevelopCostInfo.Development;
                    foundationReliableProcessHours.TraceabilitySoftware = deviceItem.DevelopCostInfo.TraceabilitySoftware;
                    //软硬件保存


                    //工装治具
                    foundationReliableProcessHours.DevelopTotalPrice = deviceItem.toolInfo.HardwareDeviceTotalPrice;
                    foundationReliableProcessHours.TestLineName = deviceItem.toolInfo.TestLineName;
                    foundationReliableProcessHours.TestLineNumber = deviceItem.toolInfo.TestLineNumber;
                    foundationReliableProcessHours.TestLinePrice = deviceItem.toolInfo.TestLinePrice;
                    //缺少字段
                    //foundationReliableProcessHours.FrockPrice = deviceItem.toolInfo.FrockPrice;
                    foundationReliableProcessHours.FrockName = deviceItem.toolInfo.FrockName;
                    foundationReliableProcessHours.FrockNumber = deviceItem.toolInfo.FrockNumber;
                    //foundationReliableProcessHours.FixturePrice = deviceItem.toolInfo.FixturePrice;
                    //foundationReliableProcessHours.FixtureNumber = deviceItem.toolInfo.FixtureNumber;
                    //foundationReliableProcessHours.FixtureName = deviceItem.toolInfo.FixtureName;


                    //工时
                    foundationReliableProcessHours.CreationTime = DateTime.Now;
                    if (AbpSession.UserId != null)
                    {
                        foundationReliableProcessHours.CreatorUserId = AbpSession.UserId.Value;
                        foundationReliableProcessHours.LastModificationTime = DateTime.Now;
                        foundationReliableProcessHours.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    foundationReliableProcessHours.LastModificationTime = DateTime.Now;
                    foundationReliableProcessHours.StandardTechnologyId = input.Id;
                    _foundationFoundationReliableProcessHoursRepository.InsertAsync(foundationReliableProcessHours);

                    var ID = _foundationFoundationReliableProcessHoursRepository.InsertAndGetId(foundationReliableProcessHours);
                    if (null != deviceItem.DeviceInfo.DeviceArr)
                    {
                        foreach (var device in deviceItem.DeviceInfo.DeviceArr)
                        {
                            FoundationTechnologyDevice technologyDevice = new FoundationTechnologyDevice();
                            technologyDevice.DeviceName = device.DeviceName;
                            technologyDevice.DevicePrice = device.DevicePrice;
                            technologyDevice.DeviceStatus = device.DeviceStatus;
                            technologyDevice.DeviceNumber = device.DeviceNumber;
                            technologyDevice.CreationTime = DateTime.Now;
                            technologyDevice.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyDevice.CreatorUserId = AbpSession.UserId.Value;
                                technologyDevice.LastModificationTime = DateTime.Now;
                                technologyDevice.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyDeviceRepository.InsertAsync(technologyDevice);

                        }
                    }
                    if (null != deviceItem.DevelopCostInfo.HardwareInfo)
                    {
                        foreach (var device in deviceItem.DevelopCostInfo.HardwareInfo)
                        {
                            FoundationTechnologyHardware technologyHardware = new FoundationTechnologyHardware();
                            technologyHardware.HardwarePrice = device.HardwareDevicePrice;
                            technologyHardware.HardwareName = device.HardwareDeviceName;
                            technologyHardware.HardwareNumber = device.HardwareDeviceNumber;
                            technologyHardware.CreationTime = DateTime.Now;
                            technologyHardware.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyHardware.CreatorUserId = AbpSession.UserId.Value;
                                technologyHardware.LastModificationTime = DateTime.Now;
                                technologyHardware.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyHardwareRepository.InsertAsync(technologyHardware);

                        }
                    }

                    if (null != deviceItem.toolInfo.zhiJuArr)
                    {
                        foreach (var device in deviceItem.toolInfo.zhiJuArr)
                        {
                            FoundationTechnologyFixture technologyFixture = new FoundationTechnologyFixture();
                            technologyFixture.FixturePrice = device.FixturePrice;
                            technologyFixture.FixtureName = device.FixtureName;
                            technologyFixture.FixtureNumber = device.FixtureNumber;
                            technologyFixture.CreationTime = DateTime.Now;
                            technologyFixture.FoundationReliableHoursId = ID;
                            if (AbpSession.UserId != null)
                            {
                                technologyFixture.CreatorUserId = AbpSession.UserId.Value;
                                technologyFixture.LastModificationTime = DateTime.Now;
                                technologyFixture.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            _foundationTechnologyFixtureRepository.InsertAsync(technologyFixture);

                        }
                    }



                    if (null != deviceItem.sopInfo)
                    {
                        foreach (var device in deviceItem.sopInfo)
                        {
                            FTWorkingHour technologyWorkingHour = new FTWorkingHour();
                            technologyWorkingHour.Year = device.Year;
                            technologyWorkingHour.NumberPersonnel = device.NumberPersonnel;
                            technologyWorkingHour.MachineHour = device.MachineHour;
                            technologyWorkingHour.LaborHour = device.LaborHour;
                            technologyWorkingHour.CreationTime = DateTime.Now;
                            technologyWorkingHour.FoundationReliableHoursId = ID;
                            technologyWorkingHour.Isdeleted1 = 1;
                            if (AbpSession.UserId != null)
                            {
                                technologyWorkingHour.CreatorUserId = AbpSession.UserId.Value;
                                technologyWorkingHour.LastModificationTime = DateTime.Now;
                                technologyWorkingHour.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            try
                            {

                                _fTWorkingHourRepository.InsertAsync(technologyWorkingHour);
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                }


            }

            this.CreateLog(" 编辑标准工艺库项目1条");

            return result;

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationStandardTechnologyRepository.DeleteAsync(s => s.Id == id);
            await _foundationTechnologyDeviceRepository.DeleteAsync(s => s.FoundationReliableHoursId == id);
            await _foundationFoundationReliableProcessHoursRepository.DeleteAsync(s => s.StandardTechnologyId == id);
            await _foundationTechnologyHardwareRepository.DeleteAsync(s => s.FoundationReliableHoursId == id);
            await _foundationTechnologyFixtureRepository.DeleteAsync(s => s.FoundationReliableHoursId == id);
            await _fTWorkingHourRepository.DeleteAsync(s => s.FoundationReliableHoursId == id);
            this.CreateLog(" 删除标准工艺库项目1条");


        }


        /// <summary>
        /// 标准工艺库导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<FoundationReliableProcessHoursResponseDto>> UploadFoundationStandardTechnology(IFormFile file)
        {
            try
            {
                //打开上传文件的输入流
                using (Stream stream = file.OpenReadStream())
                {

                    ////根据文件流创建excel数据结构
                    //IWorkbook workbook = WorkbookFactory.Create(stream);
                    //stream.Close();

                    ////尝试获取第一个sheet
                    //var sheet = workbook.GetSheetAt(0);
                    ////判断是否获取到 sheet
                    //var tempmbPath = @"D:\aa.xlsx";

                    //var memoryStream = new MemoryStream();
                    //MiniExcel.SaveAsByTemplate(path, tempmbPath, list, configuration: config);
                    var rows = MiniExcel.Query(stream).ToList();
                    // 解析数量
                    int startCols = 3;
                    // 设备列数
                    int deviceCols = 0;
                    // 追溯列数
                    int fromCols = 0;
                    // 工装列数
                    int frockCols = 0;
                    // 年总数
                    int yearCols = 0;
                    // 根据第一行计算
                    var startRow = rows[0];
                    List<string> yearStrs = new List<string>();


                    bool isDevice = false;
                    bool isFrock = false;
                    bool isForm = false;
                    bool isYear = false;

                    IDictionary<String, Object> cols = rows[0];
                    // 从第三个下标开始
                    foreach (var col in cols)
                    {
                        string val = col.Value == null ? string.Empty : col.Value.ToString();
                        if (val.Contains("设备"))
                        {
                            isDevice = true;
                        }
                        else if (val.Contains("追溯部分"))
                        {
                            isForm = true;
                            isDevice = false;
                        }
                        else if (val.Contains("工装治具部分"))
                        {
                            isFrock = true;
                            isForm = false;
                            isDevice = false;
                        }
                        else if (val.Contains("年"))
                        {
                            isYear = true;
                            isFrock = false;
                            isForm = false;
                            isDevice = false;
                        }
                        if (isDevice)
                        {
                            deviceCols++;
                        }
                        else if (isForm)
                        {
                            fromCols++;
                        }
                        else if (isFrock)
                        {
                            frockCols++;
                        }
                        else if (isYear)
                        {
                            yearStrs.Add(val);
                            yearCols += 3;
                            isYear = false;
                        }
                    }
                    List<FoundationReliableProcessHoursResponseDto> foundationReliableProcessHoursResponseDtoList = new List<FoundationReliableProcessHoursResponseDto>();

                    // 取值
                    var keys = cols.Keys.ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        FoundationReliableProcessHoursResponseDto foundationReliableProcessHoursResponse = new FoundationReliableProcessHoursResponseDto();
                        IDictionary<String, Object> row = rows[i];
                        Dictionary<string, object> rowItem = new Dictionary<string, object>();
                        //总数居
                        foundationReliableProcessHoursResponse.ProcessName = (row[keys[2]]).ToString();
                        foundationReliableProcessHoursResponse.ProcessNumber = (row[keys[1]]).ToString();

                        //获取设备
                        Object deviceInfo = new Object();
                        int deviceNum = (deviceCols - 1) / 4;
                        FoundationReliableProcessHoursDeviceResponseDto devices = new FoundationReliableProcessHoursDeviceResponseDto();
                        List<FoundationTechnologyDeviceDto> foundationTechnologyDeviceDtoList = new List<FoundationTechnologyDeviceDto>();
                        for (int j = 0; j < deviceNum; j++)
                        {
                            FoundationTechnologyDeviceDto foundationTechnologyDevice = new FoundationTechnologyDeviceDto();
                            Dictionary<string, object> deviceItem = new Dictionary<string, object>();
                            int deviceStartIndex = j * 4 + startCols;
                            var val0 = row[keys[deviceStartIndex]];
                            var val1 = row[keys[deviceStartIndex + 1]];
                            var val2 = row[keys[deviceStartIndex + 2]];
                            var val3 = row[keys[deviceStartIndex + 3]];
                            deviceItem.Add("deviceName", val0);
                            deviceItem.Add("deviceStatus", val1);
                            deviceItem.Add("deviceNumber", val2);
                            deviceItem.Add("devicePrice", val3);

                            if (null != val3)
                            {
                                foundationTechnologyDevice.DevicePrice = val3.ToString();
                            }
                            if (null != val0)
                            {
                                foundationTechnologyDevice.DeviceName = val0.ToString();
                            }
                            if (null != val2)
                            {
                                foundationTechnologyDevice.DeviceNumber = val2.ToString();
                            }
                            if (null != val1)
                            {
                                foundationTechnologyDevice.DeviceStatus = val1.ToString();
                            }
                            foundationTechnologyDeviceDtoList.Add(foundationTechnologyDevice);
                        }
                        // 设备总价
                        int ddevNumIndex = startCols + deviceCols;
                        devices.DeviceArr = foundationTechnologyDeviceDtoList;
                        devices.DeviceTotalPrice = decimal.Parse(row[keys[ddevNumIndex - 1]].ToString());
                        foundationReliableProcessHoursResponse.DeviceInfo = devices;



                        // 解析追溯信息
                        FoundationReliableProcessHoursdevelopCostInfoResponseDto foundationReliableProcessHoursdevelopCostInfoResponseDto = new FoundationReliableProcessHoursdevelopCostInfoResponseDto();
                        List<FoundationTechnologyFrockDto> foundationTechnologyDeviceList = new List<FoundationTechnologyFrockDto>();

                        // 有6列是总结列，不是子表，需要将数量剔除
                        fromCols = fromCols - 6;
                        int fromNum =2;
                        for (int j = 0; j < fromNum; j++)
                        {
                            Dictionary<string, object> fromItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + startCols + deviceCols;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            FoundationTechnologyFrockDto foundationTechnologyFrockDto = new FoundationTechnologyFrockDto();
                            if (null != val0)
                            {
                                foundationTechnologyFrockDto.HardwareDeviceName = val0.ToString();
                            }
                            if (val1 != val0)
                            {
                                foundationTechnologyFrockDto.HardwareDeviceNumber = decimal.Parse(val1.ToString());
                            }
                            if (val2 != val0)
                            {
                                foundationTechnologyFrockDto.HardwareDevicePrice = decimal.Parse(val2.ToString());
                            }
               
                            foundationTechnologyDeviceList.Add(foundationTechnologyFrockDto);
                        }
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareInfo = foundationTechnologyDeviceList;
                        // 设备总价
                        int fromNumIndex = 22;

                        if (null != row[keys[fromNumIndex]])
                        {
                            // 硬件总价
                            rowItem.Add(keys[fromNumIndex], row[keys[fromNumIndex]].ToString());
                        }
                
                        // 追溯软件
                        if (null != row[keys[fromNumIndex + 1]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.TraceabilitySoftware = (row[keys[fromNumIndex + 1]].ToString());

                        }
                        // 开发费(追溯)
           
                        if (null != row[keys[fromNumIndex + 2]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.Development = decimal.Parse(row[keys[fromNumIndex + 2]].ToString());

                        }
                        // 开图软件
                        if (null != row[keys[fromNumIndex + 3]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.DrawingSoftware = (row[keys[fromNumIndex + 3]].ToString());

                        }
                        // 开发费(开图)
      
                        if (null != row[keys[fromNumIndex + 4]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.PictureDevelopment = decimal.Parse(row[keys[fromNumIndex + 4]].ToString());

                        }
                        // 软硬件总价
                        if (null != row[keys[fromNumIndex + 5]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.SoftwareHardPrice = decimal.Parse(row[keys[fromNumIndex + 5]].ToString());

                        }

                        foundationReliableProcessHoursResponse.DevelopCostInfo = foundationReliableProcessHoursdevelopCostInfoResponseDto;




                        // 解析工装治具部分
                        FoundationReliableProcessHoursFixtureResponseDto foundationReliableProcessHoursFixtureResponseDto = new FoundationReliableProcessHoursFixtureResponseDto();
                        List<FoundationTechnologyFixtureDto> foundationTechnologyFixtures = new List<FoundationTechnologyFixtureDto>();
                        int frocNum = (16 - 10) / 3;
                        for (int j = 0; j < frocNum; j++)
                        {
                            FoundationTechnologyFixtureDto foundationTechnologyFixtureDto = new FoundationTechnologyFixtureDto();
                            int fromStartIndex = j * 3 + fromNumIndex + 6;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            if (null != val0)
                            {
                                foundationTechnologyFixtureDto.FixtureName = val0.ToString();
                            }
                            if (null != val1)
                            {
                                foundationTechnologyFixtureDto.FixtureNumber = decimal.Parse(val1.ToString());
                            }
                            if (null != val2)
                            {
                                foundationTechnologyFixtureDto.FixturePrice = decimal.Parse(val2.ToString());

                            }
                            foundationTechnologyFixtures.Add(foundationTechnologyFixtureDto);
                        }
                        foundationReliableProcessHoursFixtureResponseDto.zhiJuArr = foundationTechnologyFixtures;

                        // 设备总价
                        int frocNumIndex = 22 + 6 + 16;
                        // 工装治具检具总价
                        rowItem.Add(keys[frocNumIndex], row[keys[frocNumIndex - 1]].ToString());
                        if (null != row[keys[frocNumIndex - 1]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.HardwareDeviceTotalPrice = decimal.Parse(row[keys[frocNumIndex - 1]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 2]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLinePrice = decimal.Parse(row[keys[frocNumIndex - 2]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 3]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLineNumber = decimal.Parse(row[keys[frocNumIndex - 3]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 4]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLineName = (row[keys[frocNumIndex - 4]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 5]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockPrice = decimal.Parse(row[keys[frocNumIndex - 5]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 6]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockNumber = decimal.Parse(row[keys[frocNumIndex - 6]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 7]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockName = (row[keys[frocNumIndex - 7]].ToString());
                        }
                        if (null != row[keys[frocNumIndex - 8]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixturePrice = decimal.Parse(row[keys[frocNumIndex - 8]].ToString());

                        }
                        if (null != row[keys[frocNumIndex - 9]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixtureNumber = decimal.Parse(row[keys[frocNumIndex - 9]].ToString());

                        }
                    ; if (null != row[keys[frocNumIndex - 10]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixtureName = (row[keys[frocNumIndex - 10]].ToString());

                        }

          
                        foundationReliableProcessHoursResponse.toolInfo = foundationReliableProcessHoursFixtureResponseDto;

                        // 解析年度部分
                        List<FoundationWorkingHourItemDto> foundationWorkingHourItemDtos = new List<FoundationWorkingHourItemDto>();
                        List<Dictionary<string, object>> years = new List<Dictionary<string, object>>();
                        int yearNum = yearCols / 3;
                        for (int j = 0; j < yearNum; j++)
                        {
                            FoundationWorkingHourItemDto foundationWorkingHourItem = new FoundationWorkingHourItemDto();
                            string yearstr = yearStrs[j];
                            Dictionary<string, object> yearItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + frocNumIndex;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                         
                            if (null != val0)
                            {
                                foundationWorkingHourItem.LaborHour = val0.ToString();

                            }
                            if (null != val1)
                            {
                                foundationWorkingHourItem.MachineHour = val1.ToString(); ;

                            }
                            if (null != val2)
                            {
                                foundationWorkingHourItem.NumberPersonnel = val2.ToString();

                            }
                          
                     
                            foundationWorkingHourItem.Year = yearstr;
                            foundationWorkingHourItemDtos.Add(foundationWorkingHourItem);
                        }
                        foundationReliableProcessHoursResponse.sopInfo = foundationWorkingHourItemDtos;

                        foundationReliableProcessHoursResponseDtoList.Add(foundationReliableProcessHoursResponse);
                    }
                    return foundationReliableProcessHoursResponseDtoList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
        }


        /// <summary>
        /// 导出标准工艺库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> exportDownload(FoundationExport foundationExport)
        {
            //无数据的情况下
            // 设置查询条件
            var list = this._foundationFoundationReliableProcessHoursRepository.GetAll().Where(t => t.IsDeleted == false && t.StandardTechnologyId == foundationExport.Id).ToList();
       
          

            IWorkbook wk = new XSSFWorkbook();
            ISheet sheet = wk.CreateSheet("Sheet1");
            sheet.DefaultRowHeight = 25 * 20;
            // 表头设置
            IRow herdRow = sheet.CreateRow(0);
            CreateCell(herdRow, 0, "序号", wk);
            sheet.SetColumnWidth(0, 10 * 300);
            CreateCell(herdRow, 1, "工序编号", wk);
            sheet.SetColumnWidth(1, 10 * 300);
            CreateCell(herdRow, 2, "工序名称", wk);
            sheet.SetColumnWidth(2, 10 * 300);
            CreateCell(herdRow, 3, "设备", wk);
            sheet.SetColumnWidth(3, 10 * 300);
            CreateCell(herdRow, 4, "追溯部分", wk);
            sheet.SetColumnWidth(4, 10 * 500);
            CreateCell(herdRow, 5, "工装治具", wk);
            sheet.SetColumnWidth(5, 10 * 500);

            MergedRegion(sheet, 0, 1, 0, 0);
            MergedRegion(sheet, 0, 1, 1, 1);
            MergedRegion(sheet, 0, 1, 2, 2);
            MergedRegion(sheet, 0, 0, 3, 15);
            MergedRegion(sheet, 0, 0, 16, 27);
            MergedRegion(sheet, 0, 0, 28, 43);

            int colIndex = 0;


            // 副表头
            IRow herdRow2 = sheet.CreateRow(1);
            CreateCell(herdRow2, 0, string.Empty, wk);
            CreateCell(herdRow2, 1, string.Empty, wk);
            CreateCell(herdRow2, 2, string.Empty, wk);
            CreateCell(herdRow2, 3, "设备1名称", wk);
            CreateCell(herdRow2, 4, "设备1状态", wk);
            CreateCell(herdRow2, 5, "设备1数量", wk);
            CreateCell(herdRow2, 6, "设备1单价", wk);
            CreateCell(herdRow2, 7, "设备2名称", wk);
            CreateCell(herdRow2, 8, "设备2状态", wk);
            CreateCell(herdRow2, 9, "设备2数量", wk);
            CreateCell(herdRow2, 10, "设备2单价", wk);
            CreateCell(herdRow2, 11, "设备3名称", wk);
            CreateCell(herdRow2, 12, "设备3状态", wk);
            CreateCell(herdRow2, 13, "设备3数量", wk);
            CreateCell(herdRow2, 14, "设备3单价", wk);
            CreateCell(herdRow2, 15, "设备总价", wk);

            CreateCell(herdRow2, 16, "硬件设备1", wk);
            CreateCell(herdRow2, 17, "数量", wk);
            CreateCell(herdRow2, 18, "单价设备1", wk);
            CreateCell(herdRow2, 19, "硬件设备2", wk);
            CreateCell(herdRow2, 20, "数量", wk);
            CreateCell(herdRow2, 21, "单价设备2", wk);
            CreateCell(herdRow2, 22, "硬件总价", wk);
            CreateCell(herdRow2, 23, "追溯软件", wk);
            CreateCell(herdRow2, 24, "开发费(追溯)", wk);
            CreateCell(herdRow2, 25, "开图软件", wk);
            CreateCell(herdRow2, 26, "开发费(开图)", wk);
            CreateCell(herdRow2, 27, "软硬件总价", wk);

            CreateCell(herdRow2, 28, "治具1名称", wk);
            CreateCell(herdRow2, 29, "数量", wk);
            CreateCell(herdRow2, 30, "治具单价", wk);
            CreateCell(herdRow2, 31, "治具2名称", wk);
            CreateCell(herdRow2, 32, "数量", wk);
            CreateCell(herdRow2, 33, "治具单价", wk);
            CreateCell(herdRow2, 34, "检具名称", wk);
            CreateCell(herdRow2, 35, "数量", wk);
            CreateCell(herdRow2, 36, "检具单价", wk);
            CreateCell(herdRow2, 37, "工装名称", wk);
            CreateCell(herdRow2, 38, "数量", wk);
            CreateCell(herdRow2, 39, "工装单价", wk);
            CreateCell(herdRow2, 40, "测试线名称", wk);
            CreateCell(herdRow2, 41, "数量", wk);
            CreateCell(herdRow2, 42, "线束单价", wk);
            CreateCell(herdRow2, 43, "工装治具检具总价", wk);

            int c = 43;
            int d = 6;

            if (list.Count > 0)
            {
                var yearCountList = this._fTWorkingHourRepository.GetAll().Where(t => t.FoundationReliableHoursId == list[0].Id).ToList();
                for (int i = 0; i < yearCountList.Count; i++)
                {
                    if (i == 0)
                    {
                        string year = "";

                        year = yearCountList[i].Year + "";


                        CreateCell(herdRow, c + 1, year, wk);
                        sheet.SetColumnWidth(d, 10 * 500);

                        MergedRegion(sheet, 0, 0, c + 1, c + 3);
                        c += 4;
                        d += 2;
                    }
                    else
                    {
                        string year = "";

                        year = yearCountList[i].Year + "";


                        CreateCell(herdRow, c, year, wk);
                        sheet.SetColumnWidth(d, 10 * 500);

                        MergedRegion(sheet, 0, 0, c, c + 2);
                        c += 3;
                        d += 1;
                    }


                }
                int e = 43;
                for (int i = 0; i < yearCountList.Count; i++)
                {
                    CreateCell(herdRow2, e + 1, "标准人工工时", wk);
                    CreateCell(herdRow2, e + 2, "标准机器工时", wk);
                    CreateCell(herdRow2, e + 3, "人员数量", wk);
                    e += 3;
                }
            }


            // 查询数据
            //数据转换
            int row = 1;
            foreach (var item in list)
            {
                row = row + 1;
                IRow herdRow3 = sheet.CreateRow(row);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(herdRow3, 0, item.Id.ToString(), wk);
                CreateCell(herdRow3, 1, item.ProcessNumber, wk);
                CreateCell(herdRow3, 2, item.ProcessName, wk);
                //设备的信息
                var listDevice = _foundationTechnologyDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.FoundationReliableHoursId == item.Id).ToList();
                if (null != listDevice[0].DeviceName)
                {
                    CreateCell(herdRow3, 3, listDevice[0].DeviceName, wk);
                }
                else
                {
                    CreateCell(herdRow3, 3, string.Empty, wk);
                }
                if (null != listDevice[0].DeviceStatus)
                {
                    CreateCell(herdRow3, 4, listDevice[0].DeviceStatus, wk);
                }
                else
                {
                    CreateCell(herdRow3, 4, string.Empty, wk);
                }

                if (null != listDevice[0].DeviceNumber)
                {
                    CreateCell(herdRow3, 5, listDevice[0].DeviceNumber.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 5, string.Empty, wk);
                }
                if (null != listDevice[0].DevicePrice)
                {
                    CreateCell(herdRow3, 6, listDevice[0].DevicePrice.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 6, string.Empty, wk);

                }
                if (null != listDevice[1].DeviceName)
                {
                    CreateCell(herdRow3, 7, listDevice[1].DeviceName.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 7, string.Empty, wk);

                }
                if (null != listDevice[1].DeviceStatus)
                {
                    CreateCell(herdRow3, 8, listDevice[1].DeviceStatus.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 8, string.Empty, wk);

                }
                if (null != listDevice[1].DeviceNumber)
                {
                    CreateCell(herdRow3, 9, listDevice[1].DeviceNumber.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 9, string.Empty, wk);

                }
                if (null != listDevice[1].DevicePrice)
                {
                    CreateCell(herdRow3, 10, listDevice[1].DevicePrice.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 10, string.Empty, wk);

                }
                if (null != listDevice[2].DeviceName)
                {
                    CreateCell(herdRow3, 11, listDevice[2].DeviceName.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 11, string.Empty, wk);

                }
                if (null != listDevice[2].DeviceStatus)
                {
                    CreateCell(herdRow3, 12, listDevice[2].DeviceStatus.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 12, string.Empty, wk);

                }

                if (null != listDevice[2].DeviceNumber)
                {
                    CreateCell(herdRow3, 13, listDevice[2].DeviceNumber.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 13, string.Empty, wk);

                }
                if (null != listDevice[2].DevicePrice)
                {
                    CreateCell(herdRow3, 14, listDevice[2].DevicePrice.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 14, string.Empty, wk);

                }
                if (null != item.DeviceTotalPrice)
                {
                    CreateCell(herdRow3, 15, item.DeviceTotalPrice.ToString(), wk);

                }
                else
                {
                    CreateCell(herdRow3, 15, string.Empty, wk);

                }

                //追溯部分(硬件及软件开发费用)
                var listFrock = _foundationTechnologyHardwareRepository.GetAll().Where(t => t.IsDeleted == false && t.FoundationReliableHoursId == item.Id).ToList();
                CreateCell(herdRow3, 16, listFrock[0].HardwareName, wk);
                CreateCell(herdRow3, 17, listFrock[0].HardwareNumber.ToString(), wk);
                CreateCell(herdRow3, 18, listFrock[0].HardwarePrice.ToString(), wk);

                CreateCell(herdRow3, 19, listFrock[1].HardwareName, wk);
                CreateCell(herdRow3, 20, listFrock[1].HardwareNumber.ToString(), wk);
                CreateCell(herdRow3, 21, listFrock[1].HardwarePrice.ToString(), wk);
                // 硬件总价
                CreateCell(herdRow3, 22, ((listFrock[0].HardwarePrice * listFrock[0].HardwareNumber) + (listFrock[1].HardwarePrice * listFrock[1].HardwareNumber)).ToString(), wk);
                //追溯软件
                if (null != item.TraceabilitySoftware)
                {
                    CreateCell(herdRow3, 23, item.TraceabilitySoftware.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 23, string.Empty, wk);
                }

                //开发费(追溯)
                if (null != item.TraceabilitySoftware)
                {
                    CreateCell(herdRow3, 24, item.TraceabilitySoftwareCost.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 24, string.Empty, wk);
                }
                //开图软件
                if (null != item.DrawingSoftware)
                {
                    CreateCell(herdRow3, 25, item.DrawingSoftware.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 25, string.Empty, wk);
                }
                //开发费(开图)
                if (null != item.SoftwarePrice)
                {
                    CreateCell(herdRow3, 26, item.SoftwarePrice.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 26, string.Empty, wk);
                }
                //软硬件总价
                if (null != item.SoftwareHardPrice)
                {
                    CreateCell(herdRow3, 27, item.SoftwareHardPrice.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 27, string.Empty, wk);
                }

                //工装治具部分
                var listFixture = _foundationTechnologyFixtureRepository.GetAll().Where(t => t.IsDeleted == false && t.FoundationReliableHoursId == item.Id).ToList();
                CreateCell(herdRow3, 28, listFixture[0].FixtureName, wk);
                CreateCell(herdRow3, 29, listFixture[0].FixtureNumber.ToString(), wk);
                CreateCell(herdRow3, 30, listFixture[0].FixturePrice.ToString(), wk);

                CreateCell(herdRow3, 31, listFixture[1].FixtureName, wk);
                CreateCell(herdRow3, 32, listFixture[1].FixtureNumber.ToString(), wk);
                CreateCell(herdRow3, 33, listFixture[1].FixturePrice.ToString(), wk);
                if (null != item.FixtureName)
                {
                    CreateCell(herdRow3, 34, item.FixtureName.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 34, string.Empty, wk);
                }
                CreateCell(herdRow3, 35, item.FixtureNumber.ToString(), wk);
                CreateCell(herdRow3, 36, item.FixturePrice.ToString(), wk);
                if (null != item.FrockName)
                {
                    CreateCell(herdRow3, 37, item.FrockName.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 37, string.Empty, wk);
                }
                CreateCell(herdRow3, 38, item.FrockNumber.ToString(), wk);
                CreateCell(herdRow3, 39, item.FrockPrice.ToString(), wk);
                if (null != item.TestLineName)
                {
                    CreateCell(herdRow3, 40, item.TestLineName.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 40, string.Empty, wk);
                }
                CreateCell(herdRow3, 41, item.TestLineNumber.ToString(), wk);

                CreateCell(herdRow3, 42, item.TestLinePrice.ToString(), wk);
                if (null != item.TestLinePrice)
                {
                    CreateCell(herdRow3, 42, item.TestLinePrice.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 42, string.Empty, wk);
                }
                if (null != item.DevelopTotalPrice)
                {
                    CreateCell(herdRow3, 43, item.DevelopTotalPrice.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 43, string.Empty, wk);
                }



                //标准工时
                var queryYear = (from a in _fTWorkingHourRepository.GetAllList(p => p.IsDeleted == false && p.FoundationReliableHoursId == item.Id).Select(p => p.Year).Distinct()
                                 select a).ToList();
                List<FoundationStandardSopInfoDto> processHoursEnteritems = new List<FoundationStandardSopInfoDto>();
                foreach (var device in queryYear)
                {
                    FoundationStandardSopInfoDto processHoursEnteritem = new FoundationStandardSopInfoDto();
                    var deviceYear = _fTWorkingHourRepository.GetAll().Where(p => p.IsDeleted == false && p.FoundationReliableHoursId == item.Id && p.Year == device).ToList();
                    List<FoundationEnteritemDto> processHoursEnteritems1 = new List<FoundationEnteritemDto>();
                    foreach (var yearItem in deviceYear)
                    {
                        FoundationEnteritemDto processHoursEnteritemDto = new FoundationEnteritemDto();
                        processHoursEnteritemDto.LaborHour = yearItem.LaborHour;
                        processHoursEnteritemDto.NumberPersonnel = yearItem.NumberPersonnel;
                        processHoursEnteritemDto.MachineHour = yearItem.MachineHour;
                        processHoursEnteritems1.Add(processHoursEnteritemDto);
                    }
                    processHoursEnteritem.Issues = processHoursEnteritems1;
                    processHoursEnteritems.Add(processHoursEnteritem);
                }

                //标准工时

                int yaer = 43;


                if (queryYear.Count > 0)
                {
                    for (int i = 0; i < queryYear.Count; i++)
                    {

                        yaer += 3;
                        CreateCell(herdRow3, yaer - 2, processHoursEnteritems[i].Issues[0].LaborHour.ToString(), wk);
                        CreateCell(herdRow3, yaer - 1, processHoursEnteritems[i].Issues[0].LaborHour.ToString(), wk);
                        CreateCell(herdRow3, yaer, processHoursEnteritems[i].Issues[0].NumberPersonnel.ToString(), wk);
                    }

                }


            }



            using (FileStream sw = File.Create("FoundationWorkingHour.xlsx"))
            {
                wk.Write(sw);
            }
            return new FileStreamResult(File.Open("FoundationWorkingHour.xlsx", FileMode.Open), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FProcesses" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }


        /// <summary>
        /// 创建单元格并设置值
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="colIndex">单元格index</param>
        /// <param name="vaule">值</param>
        public static void CreateCell(IRow row, int colIndex, string vaule, IWorkbook wk)
        {
            ICell cel = row.CreateCell(colIndex);
            SetICellStyle(cel, wk);
            cel.SetCellValue(vaule);
        }


        public static void SetICellStyle(ICell MyCell, IWorkbook wk)
        {
            ICellStyle cellStyle = wk.CreateCellStyle();
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            //文字水平和垂直对齐方式  
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            //是否换行  
            //cellStyle.WrapText = true;  //若字符串过大换行填入单元格
            //缩小字体填充  
            //cellStyle.ShrinkToFit = true;//若字符串过大缩小字体后填入单元格
            //IFont font = wk.CreateFont();
            //设置字体加粗样式
            //font.Boldweight = short.MaxValue;
            MyCell.CellStyle = cellStyle;//赋给单元格
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="firstRow">开始行</param>
        /// <param name="lastRow">结束行</param>
        /// <param name="firstCol">开始列</param>
        /// <param name="lastCol">结束列</param>
        public static void MergedRegion(ISheet sheet, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            CellRangeAddress region = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            sheet.AddMergedRegion(region);
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        private async Task<bool> CreateLog(string Remark)
        {
            FoundationLogs entity = new FoundationLogs()
            {
                IsDeleted = false,
                DeletionTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
            };
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;

                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.CreationTime = DateTime.Now;
                
            }
            entity.Remark = Remark;
            entity.Type = logType;
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
