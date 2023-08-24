using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.Processes;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
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


            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(list, new List<FoundationStandardTechnologyDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
               
                if (user != null)
                {
                }
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
                await _foundationTechnologyDeviceRepository.DeleteAsync(s => s.FoundationReliableHoursId == input.Id);
                await _foundationFoundationReliableProcessHoursRepository.DeleteAsync(s => s.StandardTechnologyId == input.Id);
                await _foundationTechnologyHardwareRepository.DeleteAsync(s => s.FoundationReliableHoursId == input.Id);
                await _foundationTechnologyFixtureRepository.DeleteAsync(s => s.FoundationReliableHoursId == input.Id);
                await _fTWorkingHourRepository.DeleteAsync(s => s.FoundationReliableHoursId == input.Id);
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
                        foundationReliableProcessHoursResponse.ProcessName = (row[keys[1]]).ToString();
                        foundationReliableProcessHoursResponse.ProcessNumber = (row[keys[0]]).ToString();

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

                            foundationTechnologyDevice.DevicePrice = val3.ToString();
                            foundationTechnologyDevice.DeviceName = val0.ToString();
                            foundationTechnologyDevice.DeviceNumber = val2.ToString();
                            foundationTechnologyDevice.DeviceStatus = val1.ToString();
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
                        int fromNum = fromCols / 3;
                        for (int j = 0; j < fromNum; j++)
                        {
                            Dictionary<string, object> fromItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + startCols + deviceCols;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            FoundationTechnologyFrockDto foundationTechnologyFrockDto = new FoundationTechnologyFrockDto();
                            foundationTechnologyFrockDto.HardwareDeviceName = val0.ToString();
                            foundationTechnologyFrockDto.HardwareDeviceNumber = decimal.Parse(val1.ToString());
                            foundationTechnologyFrockDto.HardwareDevicePrice = decimal.Parse(val2.ToString());
                            foundationTechnologyDeviceList.Add(foundationTechnologyFrockDto);
                        }
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareInfo = foundationTechnologyDeviceList;
                        // 设备总价
                        int fromNumIndex = ddevNumIndex + fromCols;

                        // 硬件总价
                        rowItem.Add(keys[fromNumIndex], row[keys[fromNumIndex]].ToString());
                        // 追溯软件
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.TraceabilitySoftware = (row[keys[fromNumIndex + 1]].ToString());
                        // 开发费(追溯)
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.Development = decimal.Parse(row[keys[fromNumIndex + 2]].ToString());
                        // 开图软件
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.DrawingSoftware = (row[keys[fromNumIndex + 3]].ToString());
                        // 开发费(开图)
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.PictureDevelopment = decimal.Parse(row[keys[fromNumIndex + 4]].ToString());
                        // 软硬件总价
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.SoftwareHardPrice = decimal.Parse(row[keys[fromNumIndex + 5]].ToString());

                        foundationReliableProcessHoursResponse.DevelopCostInfo = foundationReliableProcessHoursdevelopCostInfoResponseDto;




                        // 解析工装治具部分
                        FoundationReliableProcessHoursFixtureResponseDto foundationReliableProcessHoursFixtureResponseDto = new FoundationReliableProcessHoursFixtureResponseDto();
                        List<FoundationTechnologyFixtureDto> foundationTechnologyFixtures = new List<FoundationTechnologyFixtureDto>();
                        int frocNum = (frockCols - 10) / 3;
                        for (int j = 0; j < frocNum; j++)
                        {
                            FoundationTechnologyFixtureDto foundationTechnologyFixtureDto = new FoundationTechnologyFixtureDto();
                            int fromStartIndex = j * 3 + fromNumIndex + 6;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            foundationTechnologyFixtureDto.FixtureName = val0.ToString();
                            foundationTechnologyFixtureDto.FixtureNumber = decimal.Parse(val1.ToString());
                            foundationTechnologyFixtureDto.FixturePrice = decimal.Parse(val2.ToString());
                            foundationTechnologyFixtures.Add(foundationTechnologyFixtureDto);
                        }
                        foundationReliableProcessHoursFixtureResponseDto.zhiJuArr = foundationTechnologyFixtures;

                        // 设备总价
                        int frocNumIndex = fromNumIndex + 6 + frockCols;
                        // 工装治具检具总价
                        rowItem.Add(keys[frocNumIndex], row[keys[frocNumIndex - 1]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.HardwareDeviceTotalPrice = decimal.Parse(row[keys[frocNumIndex - 1]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLinePrice = decimal.Parse(row[keys[frocNumIndex - 2]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLineNumber = decimal.Parse(row[keys[frocNumIndex - 3]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLineName = (row[keys[frocNumIndex - 4]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockPrice = decimal.Parse(row[keys[frocNumIndex - 5]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockNumber = decimal.Parse(row[keys[frocNumIndex - 6]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockName = (row[keys[frocNumIndex - 7]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixturePrice = decimal.Parse(row[keys[frocNumIndex - 8]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixtureNumber = decimal.Parse(row[keys[frocNumIndex - 9]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixtureName = (row[keys[frocNumIndex - 10]].ToString());
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
                            foundationWorkingHourItem.LaborHour = val0.ToString();
                            foundationWorkingHourItem.MachineHour = val1.ToString(); ;
                            foundationWorkingHourItem.NumberPersonnel = val2.ToString();
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
