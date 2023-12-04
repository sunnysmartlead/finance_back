using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.Ext;
using Finance.FinanceParameter;
using Finance.Infrastructure;
using Finance.Nre;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes.ProcessHoursEnterDtos;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming.Values;
using NPOI.XSSF.UserModel;
using Spire.Pdf.Exporting.XPS.Schema;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using test;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly DataInputAppService _dataInputAppService;
        private readonly IRepository<FoundationDevice, long> _foundationDeviceRepository;
        private readonly IRepository<FoundationDeviceItem, long> _foundationFoundationDeviceItemRepository;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;

        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<FProcesses, long> _fProcessesRepository;

        private readonly IRepository<FoundationHardware, long> _foundationHardwareRepository;
        private readonly IRepository<FoundationHardwareItem, long> _foundationFoundationHardwareItemRepository;

        private readonly IRepository<FoundationFixture, long> _foundationFixtureRepository;
        private readonly IRepository<FoundationFixtureItem, long> _foundationFoundationFixtureItemRepository;
        private readonly IRepository<FoundationProcedure, long> _foundationProcedureRepository;
        private readonly IRepository<FoundationWorkingHour, long> _foundationWorkingHourRepository;
        private readonly IRepository<FoundationWorkingHourItem, long> _foundationFoundationWorkingHourItemRepository;
        private readonly IRepository<ManufacturingCostInfo, long> _manufacturingCostInfoRepository;
        private readonly IRepository<FoundationDeviceItem, long> _foundationDeviceItemRepository;
        private readonly IRepository<FoundationHardwareItem, long> _foundationHardwareItemRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly IRepository<NreIsSubmit, long> _resourceNreIsSubmit;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterRepository"></param>
        public ProcessHoursEnterAppService(
                  IRepository<FProcesses, long> fProcessesRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Solution, long> resourceSchemeTable, IRepository<ProcessHoursEnter, long> processHoursEnterRepository, IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository, IRepository<ProcessHoursEnterFixture, long> processHoursEnterFixtureRepository, IRepository<ProcessHoursEnterFrock, long> processHoursEnterFrockRepository, IRepository<ProcessHoursEnteritem, long> processHoursEnterItemRepository, IRepository<ProcessHoursEnterLine, long> processHoursEnterLineRepository, IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository, DataInputAppService dataInputAppService,
                     IRepository<FoundationDevice, long> foundationDeviceRepository,
                     IRepository<FoundationHardwareItem, long> foundationFoundationHardwareItemRepository,
                     IRepository<FoundationHardware, long> foundationHardwareRepository,
                     IRepository<FoundationDeviceItem, long> foundationFoundationDeviceItemRepository,
                     IRepository<FoundationFixture, long> foundationFixtureRepository,
                     IRepository<FoundationProcedure, long> foundationProcedureRepository,
                      IRepository<FoundationWorkingHour, long> foundationWorkingHourRepository,
                       IRepository<ManufacturingCostInfo, long> manufacturingCostInfoRepository,
                     IRepository<FoundationFixtureItem, long> foundationFoundationFixtureItemRepository,
                      IRepository<FoundationWorkingHourItem, long> foundationFoundationWorkingHourItemRepository,
                      IRepository<FoundationDeviceItem, long> foundationDeviceItemRepository,
                      IRepository<FoundationHardwareItem, long> foundationHardwareItemRepository,
                      IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
                      IRepository<NreIsSubmit, long> resourceNreIsSubmit,
                      WorkflowInstanceAppService workflowInstanceAppService, IRepository<User, long> userRepository)
        {
            _foundationDeviceRepository = foundationDeviceRepository;
            _foundationProcedureRepository = foundationProcedureRepository;
            _foundationFoundationDeviceItemRepository = foundationFoundationDeviceItemRepository;
            _processHoursEnterRepository = processHoursEnterRepository;
            _processHoursEnterDeviceRepository = processHoursEnterDeviceRepository;
            _processHoursEnterFixtureRepository = processHoursEnterFixtureRepository;
            _foundationWorkingHourRepository = foundationWorkingHourRepository;
            _processHoursEnterFrockRepository = processHoursEnterFrockRepository;
            _processHoursEnterItemRepository = processHoursEnterItemRepository;
            _processHoursEnterLineRepository = processHoursEnterLineRepository;
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
            _dataInputAppService = dataInputAppService;
            _resourceNreIsSubmit = resourceNreIsSubmit;
            _resourceSchemeTable = resourceSchemeTable;
            _workflowInstanceAppService = workflowInstanceAppService;
            _foundationHardwareRepository = foundationHardwareRepository;
            _foundationFoundationHardwareItemRepository = foundationFoundationHardwareItemRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _fProcessesRepository = fProcessesRepository;
            _foundationFixtureRepository = foundationFixtureRepository;
            _foundationFoundationFixtureItemRepository = foundationFoundationFixtureItemRepository;
            _foundationFoundationWorkingHourItemRepository = foundationFoundationWorkingHourItemRepository;
            _manufacturingCostInfoRepository = manufacturingCostInfoRepository;
            _foundationDeviceItemRepository = foundationDeviceItemRepository;
            _foundationHardwareItemRepository = foundationHardwareItemRepository;
            _foundationHardwareItemRepository = foundationHardwareItemRepository;
            _userRepository = userRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> GetByIdAsync(long id)
        {
            ProcessHoursEnter entity = await _processHoursEnterRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity, new ProcessHoursEnterDto());
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
        /// 根据工序编号获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> GetEditorByProcessNumber(String ProcessNumber)
        {
            //根据工序编号获取工序名称
            ProcessHoursEnterDto processHoursEnterDto = new ProcessHoursEnterDto();
            var query = this._fProcessesRepository.GetAll().Where(t => t.ProcessNumber == ProcessNumber && t.IsDeleted == false).ToList();
            if (query.Count > 0)
            {
                processHoursEnterDto.ProcessNumber = query[0].ProcessNumber;
                processHoursEnterDto.ProcessName = query[0].ProcessName;
            }
            //设备的
            var queryDevice = this._foundationDeviceRepository.GetAll().Where(t => t.ProcessNumber == ProcessNumber && t.IsDeleted == false).ToList();
            if (queryDevice.Count > 0)
            {
                processHoursEnterDto.DeviceInfo.DeviceTotalCost = 0;
                var FoundationDeviceItemlist = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == queryDevice[0].Id).ToList();
                List<ProcessHoursEnterDeviceDto> processHoursEnterDeviceDtos = new List<ProcessHoursEnterDeviceDto>();
                foreach (var item in FoundationDeviceItemlist)
                {
                    ProcessHoursEnterDeviceDto processHoursEnterDeviceDto = new ProcessHoursEnterDeviceDto();
                    processHoursEnterDeviceDto.DevicePrice = decimal.Parse(item.DevicePrice);
                    processHoursEnterDeviceDto.DeviceStatus = item.DeviceStatus;
                    processHoursEnterDeviceDto.DeviceNumber = 0;
                    processHoursEnterDeviceDto.DeviceName = item.DeviceName;
                    processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);

                }
                if (null == FoundationDeviceItemlist || FoundationDeviceItemlist.Count < 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ProcessHoursEnterDeviceDto processHoursEnterDeviceDto = new ProcessHoursEnterDeviceDto();
                        processHoursEnterDeviceDto.DevicePrice = 0;
                        processHoursEnterDeviceDto.DeviceStatus = "";
                        processHoursEnterDeviceDto.DeviceNumber = 0;
                        processHoursEnterDeviceDto.DeviceName = "";
                        processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);
                    }

                }
                processHoursEnterDto.DeviceInfo.DeviceArr = processHoursEnterDeviceDtos;
            }
            //追溯部分(硬件及软件开发费用)
            var queryHardware = this._foundationHardwareRepository.GetAll().Where(t => t.ProcessNumber == ProcessNumber && t.IsDeleted == false).ToList();
            if (queryHardware.Count > 0)
            {
                processHoursEnterDto.DevelopCostInfo.HardwareDeviceTotalPrice = 0;
                processHoursEnterDto.DevelopCostInfo.OpenDrawingSoftware = queryHardware[0].SoftwareName;
                processHoursEnterDto.DevelopCostInfo.SoftwarePrice = queryHardware[0].SoftwarePrice;
                var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == queryHardware[0].Id).ToList();
                List<ProcessHoursEnterFrockDto> processHoursEnterDeviceDtos = new List<ProcessHoursEnterFrockDto>();
                foreach (var item in FoundationDeviceItemlist)
                {
                    ProcessHoursEnterFrockDto processHoursEnterDeviceDto = new ProcessHoursEnterFrockDto();
                    processHoursEnterDeviceDto.HardwareDevicePrice = item.HardwarePrice;
                    processHoursEnterDeviceDto.HardwareDeviceNumber = 0;
                    processHoursEnterDeviceDto.HardwareDeviceName = item.HardwareName;
                    processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);

                }
                if (null == FoundationDeviceItemlist || FoundationDeviceItemlist.Count < 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ProcessHoursEnterFrockDto processHoursEnterDeviceDto = new ProcessHoursEnterFrockDto();
                        processHoursEnterDeviceDto.HardwareDevicePrice = 0;
                        processHoursEnterDeviceDto.HardwareDeviceNumber = 0;
                        processHoursEnterDeviceDto.HardwareDeviceName = null;
                        processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);
                    }

                }
                processHoursEnterDto.DevelopCostInfo.HardwareInfo = processHoursEnterDeviceDtos;
            }
            else {

                processHoursEnterDto.DevelopCostInfo.HardwareDeviceTotalPrice = 0;
                processHoursEnterDto.DevelopCostInfo.OpenDrawingSoftware = "";
                processHoursEnterDto.DevelopCostInfo.SoftwarePrice = 0;
                List<ProcessHoursEnterFrockDto> processHoursEnterDeviceDtos = new List<ProcessHoursEnterFrockDto>();
        
                    for (int i = 0; i < 2; i++)
                    {
                        ProcessHoursEnterFrockDto processHoursEnterDeviceDto = new ProcessHoursEnterFrockDto();
                        processHoursEnterDeviceDto.HardwareDevicePrice = 0;
                        processHoursEnterDeviceDto.HardwareDeviceNumber = 0;
                        processHoursEnterDeviceDto.HardwareDeviceName = null;
                        processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);
                    }

                
                processHoursEnterDto.DevelopCostInfo.HardwareInfo = processHoursEnterDeviceDtos;
            }
            //工装治具部分
            var queryFixture = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == ProcessNumber).ToList();
            if (queryFixture.Count > 0)
            {

                var FoundationDeviceItemlist = this._foundationFoundationFixtureItemRepository.GetAll().Where(f => f.FoundationFixtureId == queryFixture[0].Id).ToList();
                List<ProcessHoursEnterFixtureDto> processHoursEnterDeviceDtos = new List<ProcessHoursEnterFixtureDto>();
                foreach (var item in FoundationDeviceItemlist)
                {
                    ProcessHoursEnterFixtureDto processHoursEnterDeviceDto = new ProcessHoursEnterFixtureDto();
                    processHoursEnterDeviceDto.FixturePrice = item.FixturePrice;
                    processHoursEnterDeviceDto.FixtureNumber = 0;
                    processHoursEnterDeviceDto.FixtureName = item.FixtureName;
                    processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);

                }
                if (null == FoundationDeviceItemlist || FoundationDeviceItemlist.Count < 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ProcessHoursEnterFixtureDto processHoursEnterDeviceDto = new ProcessHoursEnterFixtureDto();
                        processHoursEnterDeviceDto.FixturePrice = 0;
                        processHoursEnterDeviceDto.FixtureNumber = 0;
                        processHoursEnterDeviceDto.FixtureName = "";
                        processHoursEnterDeviceDtos.Add(processHoursEnterDeviceDto);
                    }

                }
                processHoursEnterDto.ToolInfo.FixtureName = queryFixture[0].FixtureGaugeName;
                processHoursEnterDto.ToolInfo.FixtureNumber = 0;
                processHoursEnterDto.ToolInfo.FixturePrice = queryFixture[0].FixtureGaugePrice;
                processHoursEnterDto.ToolInfo.ZhiJuArr = processHoursEnterDeviceDtos;
            }
            //工装
            var queryProcedure = this._foundationProcedureRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == ProcessNumber).ToList();
            if (queryProcedure.Count > 0)
            if (queryProcedure.Count > 0)
            {

                processHoursEnterDto.ToolInfo.FrockName = queryProcedure[0].InstallationName;
                processHoursEnterDto.ToolInfo.FrockPrice = queryProcedure[0].InstallationPrice;
                processHoursEnterDto.ToolInfo.FrockNumber = 0;
                processHoursEnterDto.ToolInfo.TestLineName = queryProcedure[0].TestName;
                processHoursEnterDto.ToolInfo.TestLinePrice = queryProcedure[0].TestPrice;
                processHoursEnterDto.ToolInfo.TestLineNumber = 0;
            }
            //工时
            var queryWorkingHour = _foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == ProcessNumber).ToList();
            if (queryWorkingHour.Count > 0)
            {
                List<ProcessHoursEnterSopInfoDto> processHoursEnterSopInfoDtos = new List<ProcessHoursEnterSopInfoDto>();
                var queryYear = (from a in _foundationFoundationWorkingHourItemRepository.GetAllList(p => p.IsDeleted == false && p.FoundationWorkingHourId == queryWorkingHour[0].Id).Select(p => p.Year).Distinct()
                                 select a).ToList();
                foreach (var item in queryYear)
                {
                    ProcessHoursEnterSopInfoDto p = new ProcessHoursEnterSopInfoDto();
                    p.Year = item;
                    string str2 = Regex.Replace(item, @"[^0-9]+", "");
                    p.YearInt = Decimal.Parse(str2);
                    var queryYearItem = _foundationFoundationWorkingHourItemRepository.GetAll().Where(t => t.IsDeleted == false && t.Year == item && t.FoundationWorkingHourId == queryWorkingHour[0].Id).ToList();
                    List<ProcessHoursEnteritemDto> processHoursEnteritemDtos = new List<ProcessHoursEnteritemDto>();
                    foreach (var itemYear in queryYearItem)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = decimal.Parse(itemYear.LaborHour);
                        processHoursEnteritemDto.PersonnelNumber = decimal.Parse(itemYear.NumberPersonnel);
                        processHoursEnteritemDto.MachineHour = decimal.Parse(itemYear.MachineHour);
                        processHoursEnteritemDtos.Add(processHoursEnteritemDto);
                    }
                    p.Issues = processHoursEnteritemDtos;
                    processHoursEnterSopInfoDtos.Add(p);

                }
                processHoursEnterDto.SopInfo = processHoursEnterSopInfoDtos;

            }


            return processHoursEnterDto;
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
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();

                processHoursEnter.Id = item.Id;
                processHoursEnter.ProcessNumber = item.ProcessNumber;
                processHoursEnter.ProcessName = item.ProcessName;
                //设备的信息
                var listDevice = _processHoursEnterDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();
                processHoursEnter.DeviceInfo.DeviceTotalCost = item.DeviceTotalPrice;

                List<ProcessHoursEnterDeviceDto> ProcessHoursEnterDeviceDtoList = new List<ProcessHoursEnterDeviceDto>();
                foreach (var device in listDevice)
                {

                    ProcessHoursEnterDeviceDto processHoursEnterDeviceDto = new ProcessHoursEnterDeviceDto();
                    processHoursEnterDeviceDto.DevicePrice = device.DevicePrice;
                    processHoursEnterDeviceDto.ProcessHoursEnterId = device.ProcessHoursEnterId;
                    processHoursEnterDeviceDto.DeviceNumber = device.DeviceNumber;
                    processHoursEnterDeviceDto.DeviceName = device.DeviceName;
                    processHoursEnterDeviceDto.DeviceStatus = device.DeviceStatus;
                    ProcessHoursEnterDeviceDtoList.Add(processHoursEnterDeviceDto);

                }
                processHoursEnter.DeviceInfo.DeviceArr = ProcessHoursEnterDeviceDtoList;

                //追溯部分(硬件及软件开发费用)
                var listFrock = _processHoursEnterFrockRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();

                processHoursEnter.DevelopCostInfo.HardwareTotalPrice = item.HardwareTotalPrice;
                processHoursEnter.DevelopCostInfo.SoftwarePrice = item.SoftwarePrice;
                processHoursEnter.DevelopCostInfo.OpenDrawingSoftware = item.OpenDrawingSoftware;
                processHoursEnter.DevelopCostInfo.HardwareDeviceTotalPrice = item.HardwareTotalPrice;
                processHoursEnter.DevelopCostInfo.TraceabilitySoftwareCost = item.TraceabilitySoftwareCost;
                processHoursEnter.DevelopCostInfo.TraceabilitySoftware = item.TraceabilitySoftware;

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
                var listFixture = _processHoursEnterFixtureRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();

                processHoursEnter.ToolInfo.FixturePrice = item.FixturePrice;
                processHoursEnter.ToolInfo.FixtureName = item.FixtureName;
                processHoursEnter.ToolInfo.FixtureNumber = item.FixtureNumber;
                processHoursEnter.ToolInfo.FrockPrice = item.FrockPrice;
                processHoursEnter.ToolInfo.FrockName = item.FrockName;
                processHoursEnter.ToolInfo.FrockNumber = item.FrockNumber;
                processHoursEnter.ToolInfo.TestLineName = item.TestLineName;
                processHoursEnter.ToolInfo.TestLineNumber = item.TestLineNumber;
                processHoursEnter.ToolInfo.TestLinePrice = item.TestLinePrice;
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
                Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);
                var queryYear = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).OrderBy(y => y.Year).ToList();

                //var queryYear = (from a in _processHoursEnterItemRepository.GetAllList(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id).Select(p => p.ModelCountYearId).Distinct()
                  //               select a).ToList();
                List<ProcessHoursEnterSopInfoDto> processHoursEnteritems = new List<ProcessHoursEnterSopInfoDto>();
                foreach (var device in queryYear)
                {
                    ProcessHoursEnterSopInfoDto processHoursEnteritem = new ProcessHoursEnterSopInfoDto();
                    var deviceYear = _processHoursEnterItemRepository.GetAll().Where(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id && p.ModelCountYearId == device.Id).ToList();
                    List<ProcessHoursEnteritemDto> processHoursEnteritems1 = new List<ProcessHoursEnteritemDto>();
                    foreach (var yearItem in deviceYear)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = yearItem.LaborHour;
                        processHoursEnteritemDto.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritemDto.MachineHour = yearItem.MachineHour;
                        processHoursEnteritemDto.ModelCountYearId = yearItem.ModelCountYearId;
                        processHoursEnteritems1.Add(processHoursEnteritemDto);
                    }
                    processHoursEnteritem.YearInt = (decimal)(device.Year);
                    if (null == processHoursEnteritems1 || processHoursEnteritems1.Count<1)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = 0;
                        processHoursEnteritemDto.PersonnelNumber = 0 ;
                        processHoursEnteritemDto.MachineHour = 0;
                        processHoursEnteritemDto.ModelCountYearId = device.Id;
                        processHoursEnteritems1.Add(processHoursEnteritemDto);
                    }
                    processHoursEnteritem.Issues = processHoursEnteritems1;
                    if (device.UpDown == YearType.FirstHalf)
                    {

                        processHoursEnteritem.Year = device.Year + "上半年";
                    }
                    else if (device.UpDown == YearType.SecondHalf)
                    {
                        processHoursEnteritem.Year = device.Year + "下半年";
                    }
                    else
                    {
                        processHoursEnteritem.Year = device.Year.ToString();
                    }
                    processHoursEnteritems.Add(processHoursEnteritem);
                }

                processHoursEnter.SopInfo = processHoursEnteritems;
                //Uph查询


                //线体数量、共线分摊率


                processHoursEnterDtoList.Add(processHoursEnter);


            }
            // 数据返回
            if (null == processHoursEnterDtoList || processHoursEnterDtoList.Count < 1)
            {


                //无数据的情况下
                Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

                var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
                ProcessHoursEnterDto processHoursEnterDto = new ProcessHoursEnterDto();
                List<ProcessHoursEnterSopInfoDto> processHoursEnteritems = new List<ProcessHoursEnterSopInfoDto>();
                foreach (var device in query)
                {
                    ProcessHoursEnterSopInfoDto processHoursEnteritem = new ProcessHoursEnterSopInfoDto();
                    List<ProcessHoursEnteritemDto> processHoursEnteritems1 = new List<ProcessHoursEnteritemDto>();
                    foreach (var yearItem in query)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = 0;
                        processHoursEnteritemDto.PersonnelNumber = 0;
                        processHoursEnteritemDto.MachineHour = 0;
                        processHoursEnteritemDto.ModelCountYearId = yearItem.Id;
                        processHoursEnteritem.Issues.Add(processHoursEnteritemDto);
                    }
                    processHoursEnteritem.YearInt = (decimal)(device.Year);
                    if (device.UpDown == YearType.FirstHalf)
                    {

                        processHoursEnteritem.Year = device.Year + "上半年";
                    }
                    else if (device.UpDown == YearType.SecondHalf)
                    {
                        processHoursEnteritem.Year = device.Year + "下半年";
                    }
                    else
                    {
                        processHoursEnteritem.Year = device.Year.ToString();
                    }
                    processHoursEnteritems.Add(processHoursEnteritem);

                }

                processHoursEnterDto.SopInfo = processHoursEnteritems;


                processHoursEnterDto.DeviceInfo.DeviceTotalCost = 0;



                List<ProcessHoursEnterDeviceDto> ProcessHoursEnterDeviceDtoList = new List<ProcessHoursEnterDeviceDto>();
                for (int i = 0; i < 3; i++)
                {
                    ProcessHoursEnterDeviceDto processHoursEnterDeviceDto = new ProcessHoursEnterDeviceDto();
                    processHoursEnterDeviceDto.DevicePrice = 0;
                    processHoursEnterDeviceDto.DeviceNumber = 0;
                    processHoursEnterDeviceDto.DeviceName = "";
                    processHoursEnterDeviceDto.DeviceStatus = "";
                    ProcessHoursEnterDeviceDtoList.Add(processHoursEnterDeviceDto);
                }
                processHoursEnterDto.DeviceInfo.DeviceArr = ProcessHoursEnterDeviceDtoList;




                processHoursEnterDto.DevelopCostInfo.HardwareTotalPrice = 0;
                processHoursEnterDto.DevelopCostInfo.SoftwarePrice = 0;
                processHoursEnterDto.DevelopCostInfo.OpenDrawingSoftware = "";
                processHoursEnterDto.DevelopCostInfo.HardwareDeviceTotalPrice = 0;

                List<ProcessHoursEnterFrockDto> ProcessHoursEnterFrockDtoList = new List<ProcessHoursEnterFrockDto>();
                for (int i = 0; i < 2; i++)
                {

                    ProcessHoursEnterFrockDto processHoursEnterFrock = new ProcessHoursEnterFrockDto();
                    processHoursEnterFrock.HardwareDeviceName = "";
                    processHoursEnterFrock.HardwareDeviceNumber = 0;
                    processHoursEnterFrock.HardwareDevicePrice = 0;
                    ProcessHoursEnterFrockDtoList.Add(processHoursEnterFrock);

                }
                processHoursEnterDto.DevelopCostInfo.HardwareInfo = ProcessHoursEnterFrockDtoList;

                processHoursEnterDto.ToolInfo.FixturePrice = 0;
                processHoursEnterDto.ToolInfo.FixtureNumber = 0;
                processHoursEnterDto.ToolInfo.FrockPrice = 0;
                processHoursEnterDto.ToolInfo.FrockNumber = 0;
                processHoursEnterDto.ToolInfo.TestLineNumber = 0;
                processHoursEnterDto.ToolInfo.TestLinePrice = 0;
                processHoursEnterDto.ToolInfo.DevelopTotalPrice = (0).ToString();
                List<ProcessHoursEnterFixtureDto> processHoursEnterFixtures = new List<ProcessHoursEnterFixtureDto>();
                for (int i = 0; i < 2; i++)
                {

                    ProcessHoursEnterFixtureDto processHoursEnterFixture = new ProcessHoursEnterFixtureDto();
                    processHoursEnterFixture.FixtureNumber = 0;
                    processHoursEnterFixture.FixturePrice = 0;
                    processHoursEnterFixtures.Add(processHoursEnterFixture);

                }
                processHoursEnterDto.ToolInfo.ZhiJuArr = processHoursEnterFixtures;

                processHoursEnterDtoList.Add(processHoursEnterDto);
            }

            return processHoursEnterDtoList;
        }

        /// <summary>
        /// 复制流程
        /// </summary>
        /// AuditFlowId 老流程号  AuditFlowNewId 新流程号  SolutionIdAndQuoteSolutionIds 方案数组
        /// <returns>结果</returns>
        public virtual async Task<string> ProcessHoursEnterCopyAsync(long AuditFlowId,long AuditFlowNewId, List<SolutionIdAndQuoteSolutionId> SolutionIdAndQuoteSolutionIds)
        {
            // 设置查询条件
            if (SolutionIdAndQuoteSolutionIds.Count>0)
            {
                foreach (var itemProcessHoursEnterCopy in SolutionIdAndQuoteSolutionIds)
                {
                    var query = this._processHoursEnterRepository.GetAll().Where(s => s.IsDeleted == false && s.AuditFlowId == AuditFlowNewId && s.SolutionId == itemProcessHoursEnterCopy.NewSolutionId).ToList();
                    foreach (var item in query)
                    {
                        _processHoursEnterDeviceRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
                        _processHoursEnterFixtureRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
                        _processHoursEnterFrockRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
                    }
                    await _processHoursEnterRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId == itemProcessHoursEnterCopy.NewSolutionId);
                    await _processHoursEnterUphRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId == itemProcessHoursEnterCopy.NewSolutionId);
                    await _processHoursEnterLineRepository.DeleteAsync(s => s.AuditFlowId == AuditFlowNewId && s.SolutionId == itemProcessHoursEnterCopy.NewSolutionId);

                    var list = this._processHoursEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == AuditFlowId && t.SolutionId == itemProcessHoursEnterCopy.QuoteSolutionId).ToList();

                    // 查询数据
                    foreach (var input in list)
                    {


                        ProcessHoursEnter entity = new ProcessHoursEnter();
                        entity.ProcessName = input.ProcessName;
                        entity.ProcessNumber = input.ProcessNumber;
                        entity.SolutionId = itemProcessHoursEnterCopy.NewSolutionId;
                        entity.AuditFlowId = AuditFlowNewId;
                        entity.DeviceTotalPrice = input.DeviceTotalPrice;
                        entity.HardwareTotalPrice = input.HardwareTotalPrice;
                        entity.SoftwarePrice = input.SoftwarePrice;
                        entity.OpenDrawingSoftware = input.OpenDrawingSoftware;
                        entity.HardwareTotalPrice = input.HardwareDeviceTotalPrice;
                        entity.FixtureName = input.FixtureName;
                        entity.FrockPrice = input.FrockPrice;
                        entity.FixtureNumber = input.FixtureNumber;
                        entity.FrockPrice = input.FrockPrice;
                        entity.FrockName = input.FrockName;
                        entity.FrockNumber = input.FrockNumber;
                        entity.TestLineName = input.TestLineName;
                        entity.TestLineNumber = input.TestLineNumber;
                        entity.TestLinePrice = input.TestLinePrice;
                        entity.DevelopTotalPrice = input.DevelopTotalPrice;
                        entity.TraceabilitySoftware = input.TraceabilitySoftware;
                        entity.TraceabilitySoftwareCost = input.TraceabilitySoftwareCost;
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

                        var listDevice = _processHoursEnterDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == input.Id).ToList();

                        //追溯部分(硬件及软件开发费用)
                        var listFrock = _processHoursEnterFrockRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == input.Id).ToList();


                        //工装治具部分
                        var listFixture = _processHoursEnterFixtureRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == input.Id).ToList();

                        var deviceYear = _processHoursEnterItemRepository.GetAll().Where(p => p.IsDeleted == false && p.ProcessHoursEnterId == input.Id).ToList();

                        if (null != listDevice)
                        {
                            foreach (var DeviceInfoItem in listDevice)
                            {
                                ProcessHoursEnterDevice processHoursEnterDevice = new ProcessHoursEnterDevice();
                                processHoursEnterDevice.ProcessHoursEnterId = foundationDevice;
                                processHoursEnterDevice.DeviceNumber = DeviceInfoItem.DeviceNumber;
                                processHoursEnterDevice.DevicePrice = DeviceInfoItem.DevicePrice;
                                processHoursEnterDevice.DeviceStatus = DeviceInfoItem.DeviceStatus;
                                processHoursEnterDevice.DeviceName = DeviceInfoItem.DeviceName;
                                _processHoursEnterDeviceRepository.InsertAsync(processHoursEnterDevice);
                            }
                        }
                        //追溯部分(硬件及软件开发费用)
                        if (null != listFrock)
                        {
                            foreach (var hardwareInfoItem in listFrock)
                            {
                                ProcessHoursEnterFrock processHoursEnterFrock = new ProcessHoursEnterFrock();
                                processHoursEnterFrock.ProcessHoursEnterId = foundationDevice;
                                processHoursEnterFrock.HardwareDevicePrice = hardwareInfoItem.HardwareDevicePrice;
                                processHoursEnterFrock.HardwareDeviceNumber = hardwareInfoItem.HardwareDeviceNumber;
                                processHoursEnterFrock.HardwareDeviceName = hardwareInfoItem.HardwareDeviceName;
                                _processHoursEnterFrockRepository.InsertAsync(processHoursEnterFrock);
                            }
                        }

                        //工装治具部分
                        if (null != listFixture)
                        {
                            foreach (var zoolInfo in listFixture)
                            {
                                ProcessHoursEnterFixture processHoursEnterFixture = new ProcessHoursEnterFixture();
                                processHoursEnterFixture.ProcessHoursEnterId = foundationDevice;
                                processHoursEnterFixture.FixturePrice = zoolInfo.FixturePrice;
                                processHoursEnterFixture.FixtureNumber = zoolInfo.FixtureNumber;
                                processHoursEnterFixture.FixtureName = zoolInfo.FixtureName;
                                _processHoursEnterFixtureRepository.InsertAsync(processHoursEnterFixture);
                            }
                        }
                        if (null != deviceYear)
                        {
                            foreach (var year in deviceYear)
                            {

                                ProcessHoursEnteritem processHoursEnteritem = new ProcessHoursEnteritem();

                                processHoursEnteritem.ProcessHoursEnterId = foundationDevice;
                                processHoursEnteritem.LaborHour = year.LaborHour;
                                processHoursEnteritem.PersonnelNumber = year.PersonnelNumber;
                                processHoursEnteritem.MachineHour = year.MachineHour;
                                processHoursEnteritem.ModelCountYearId = year.ModelCountYearId;
                                processHoursEnteritem.Year = year.Year;

                                _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                            }
                        }


                    }
                    //uph

                    var listUph = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == AuditFlowId && t.SolutionId == itemProcessHoursEnterCopy.QuoteSolutionId).ToList();
                    //uph
                    if (null != listUph)
                    {
                        foreach (var item in listUph)
                        {

                            ProcessHoursEnterUph processHoursEnterUph = new ProcessHoursEnterUph();
                            processHoursEnterUph.Year = item.Year;
                            processHoursEnterUph.Uph = item.Uph;
                            processHoursEnterUph.Value = item.Value;
                            processHoursEnterUph.ModelCountYearId = item.ModelCountYearId;
                            processHoursEnterUph.SolutionId = (long)itemProcessHoursEnterCopy.NewSolutionId;
                            processHoursEnterUph.AuditFlowId = AuditFlowNewId;
                            await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);


                        }
                    }
                    //线体

                    var listLine = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == AuditFlowId && t.SolutionId == itemProcessHoursEnterCopy.QuoteSolutionId).ToList();

                    //线体数量、共线分摊率
                    if (null != listLine)
                    {
                        foreach (var item in listLine)
                        {
                            ProcessHoursEnterLine processHoursEnterLine = new ProcessHoursEnterLine();
                            processHoursEnterLine.ModelCountYearId = item.ModelCountYearId;
                            processHoursEnterLine.Uph = item.Uph;
                            processHoursEnterLine.Value = item.Value;
                            processHoursEnterLine.SolutionId = (long)itemProcessHoursEnterCopy.NewSolutionId;
                            processHoursEnterLine.AuditFlowId = AuditFlowNewId;
                            await _processHoursEnterLineRepository.InsertAsync(processHoursEnterLine);


                        }
                    }
                }

            }
            return "复制成功";
        }



        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<ProcessHoursEnterSopInfoDto>> GetYearAsync(GetProcessHoursEntersInput input)
        {
            // 设置查询条件

                //无数据的情况下
                Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

                var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
            List<ProcessHoursEnterSopInfoDto> processHoursEnteritems = new List<ProcessHoursEnterSopInfoDto>();
            foreach (var device in query)
            {
                ProcessHoursEnterSopInfoDto processHoursEnteritem = new ProcessHoursEnterSopInfoDto();
                List<ProcessHoursEnteritemDto> processHoursEnteritems1 = new List<ProcessHoursEnteritemDto>();
                foreach (var yearItem in query)
                {
                    ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                    processHoursEnteritemDto.LaborHour = 0;
                    processHoursEnteritemDto.PersonnelNumber = 0;
                    processHoursEnteritemDto.MachineHour = 0;
                    processHoursEnteritemDto.ModelCountYearId = yearItem.Id;
                    processHoursEnteritem.Issues.Add(processHoursEnteritemDto);
                }

                if (device.UpDown == YearType.FirstHalf)
                {

                    processHoursEnteritem.Year = device.Year + "上半年";
                }
                else if (device.UpDown == YearType.SecondHalf)
                {
                    processHoursEnteritem.Year = device.Year + "下半年";
                }
                else
                {
                    processHoursEnteritem.Year = device.Year.ToString();
                }
                processHoursEnteritems.Add(processHoursEnteritem);

            }




            return processHoursEnteritems;
        }


        /// <summary>
        /// 查看项目走量        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<ModuleNumberDto>> GetListModuleNumberDtoAsync(ProcessHoursEnterModuleNumberDto input)
        {
            List<ModuleNumberDto> moduleNumberDtos = new List<ModuleNumberDto>();
            var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == input.SolutionId).ToList();
            foreach (var item in query)
            {

            }

            // 数据返回
            return moduleNumberDtos;
        }



        /// <summary>
        /// 查看uph和线体数量、共线分摊率接口
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<ProcessHoursEnterDto> GetListUphOrLineAsync(GetProcessHoursEntersInput input)
        {
            ProcessHoursEnterDto processHoursEnterDto = new ProcessHoursEnterDto();




            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

            var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();
            List<ProcessHoursEnterUphListDto> processHoursEnterUphListDtos = new List<ProcessHoursEnterUphListDto>();
            List<ProcessHoursEnterLineDtoList> processHoursEnterLineDtos = new List<ProcessHoursEnterLineDtoList>();
            foreach (ModelCountYear row in query)
            {

                ProcessHoursEnterUphListDto processHoursEnterUphListDto = new ProcessHoursEnterUphListDto();
                processHoursEnterUphListDto.ModelCountYearId = row.Id;
                if (row.UpDown == YearType.FirstHalf)
                {

                    processHoursEnterUphListDto.Year = row.Year + "上半年";
                }
                else if (row.UpDown == YearType.SecondHalf)
                {
                    processHoursEnterUphListDto.Year = row.Year + "下半年";
                }
                else
                {
                    processHoursEnterUphListDto.Year = row.Year.ToString();
                }


                var list = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "smtuph" && t.ModelCountYearId == row.Id).ToList();
                if (null != list && list.Count > 0)
                {
                    processHoursEnterUphListDto.Smtuph = list[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.Smtuph = 0;
                }
                var ZcuphList = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "zcuph" && t.ModelCountYearId == row.Id).ToList();
                if (null != ZcuphList && ZcuphList.Count > 0)
                {
                    processHoursEnterUphListDto.Zcuph = ZcuphList[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.Zcuph = 0;
                }

                var CobuphList = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "cobuph" && t.ModelCountYearId == row.Id).ToList();
                if (null != CobuphList && CobuphList.Count > 0)
                {
                    processHoursEnterUphListDto.Cobuph = CobuphList[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.Cobuph = 0;
                }
                processHoursEnterUphListDtos.Add(processHoursEnterUphListDto);

                ProcessHoursEnterLineDtoList processHoursEnterLine = new ProcessHoursEnterLineDtoList();
                processHoursEnterLine.ModelCountYearId = row.Id;
                var XtslList = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "xtsl" && t.ModelCountYearId == row.Id).ToList();
                if (null != XtslList && XtslList.Count > 0)
                {
                    processHoursEnterLine.Xtsl = XtslList[0].Value;
                }
                else
                {

                    processHoursEnterLine.Xtsl = 0;
                }
                var GxftlList = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "gxftl" && t.ModelCountYearId == row.Id).ToList();
                if (null != GxftlList && GxftlList.Count > 0)
                {
                    processHoursEnterLine.Gxftl = GxftlList[0].Value;
                }
                else
                {
                    processHoursEnterLine.Gxftl = 0;
                }
                if (row.UpDown == YearType.FirstHalf)
                {

                    processHoursEnterLine.Year = row.Year + "上半年";
                }
                else if (row.UpDown == YearType.SecondHalf)
                {
                    processHoursEnterLine.Year = row.Year + "下半年";
                }
                else
                {
                    processHoursEnterLine.Year = row.Year.ToString();
                }

                processHoursEnterLineDtos.Add(processHoursEnterLine);

            }

            processHoursEnterDto.processHoursEnterLineList = processHoursEnterLineDtos;
            processHoursEnterDto.processHoursEnterUphList = processHoursEnterUphListDtos;
            processHoursEnterDto.IsCOB = entity.IsCOB;

            // 数据返回
            return processHoursEnterDto;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<String> CreateSubmitAsync(ProcessHoursEnterCreateSubmitInput input)
        {
            //已经录入数量
            //已经录入数量
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId)  && p.EnumSole.Equals(NreIsSubmitDto.ProcessHoursEnter.ToString()));

            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            int quantity = result.Count - nreIsSubmits.Count;
            if (quantity > 0)
            {
                return "还有" + quantity + "个方案没有提交，请先保存";
            }
            else
            {

                //嵌入工作流
                await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                {
                    Comment = input.Comment,
                    FinanceDictionaryDetailId = input.Opinion,
                    NodeInstanceId = input.NodeInstanceId,
                });

                //提交完成  可以在这里做审核处理
                return "提交完成";

            }


        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcessHoursEnterDto> GetEditorByIdAsync(long id)
        {
            ProcessHoursEnter entity = await _processHoursEnterRepository.GetAsync(id);

            return ObjectMapper.Map<ProcessHoursEnter, ProcessHoursEnterDto>(entity, new ProcessHoursEnterDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(ProcessHoursEnterDto input)
        {
            ProcessHoursEnter entity = new ProcessHoursEnter();
            entity.ProcessName = input.ProcessName;
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
            entity.TraceabilitySoftware = input.DevelopCostInfo.TraceabilitySoftware;
            entity.TraceabilitySoftwareCost = input.DevelopCostInfo.TraceabilitySoftwareCost;
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
            if (null != input.DeviceInfo.DeviceArr)
            {
                foreach (var DeviceInfoItem in input.DeviceInfo.DeviceArr)
                {
                    ProcessHoursEnterDevice processHoursEnterDevice = new ProcessHoursEnterDevice();
                    processHoursEnterDevice.ProcessHoursEnterId = foundationDevice;
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
                    processHoursEnterFrock.ProcessHoursEnterId = foundationDevice;
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
                    processHoursEnterFixture.ProcessHoursEnterId = foundationDevice;
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
                        processHoursEnteritem.ProcessHoursEnterId = foundationDevice;
                        processHoursEnteritem.LaborHour = yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour = yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }

            //uph
            if (null != input.processHoursEnterUphList)
            {
                foreach (var item in input.processHoursEnterUphList)
                {
                    ProcessHoursEnterUph processHoursEnterUph = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "cobuph";
                    processHoursEnterUph.Value = item.Cobuph;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);
                    ProcessHoursEnterUph processHoursEnterUph1 = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "zcuph";
                    processHoursEnterUph.Value = item.Zcuph;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);
                    ProcessHoursEnterUph processHoursEnterUph2 = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "smtuph";
                    processHoursEnterUph.Value = item.Smtuph;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);

                }
            }

            //线体数量、共线分摊率
            if (null != input.processHoursEnterLineList)
            {
                foreach (var item in input.processHoursEnterLineList)
                {
                    ProcessHoursEnterUph processHoursEnterUph = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "gxftl";
                    processHoursEnterUph.Value = item.Gxftl;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);
                    ProcessHoursEnterUph processHoursEnterUph1 = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "xtsl";
                    processHoursEnterUph.Value = item.Xtsl;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);

                }
            }
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == (long)input.AuditFlowId && t.SolutionId == (long)input.SolutionId && t.EnumSole == NreIsSubmitDto.Logisticscost.ToString());

            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = (long)input.AuditFlowId, SolutionId = (long)input.SolutionId, EnumSole = NreIsSubmitDto.Logisticscost.ToString() });

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
            entity.TraceabilitySoftware = input.DevelopCostInfo.TraceabilitySoftware;
            entity.TraceabilitySoftwareCost = input.DevelopCostInfo.TraceabilitySoftwareCost;
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
                        processHoursEnteritem.ProcessHoursEnterId = entity.Id;
                        processHoursEnteritem.LaborHour = yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour = yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }
        }

        public int GetFirstRowColumnCount(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow firstRow = dt.Rows[0];
                return firstRow.ItemArray.Length;
            }
            return 0; // 如果表格中没有行，返回0
        }


        /// <summary>
        /// 工时工序导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<ProcessHoursEnterDto>> UploadProcessHoursEnter(IFormFile file, long AuditFlowId, long SolutionId)
        {
            try
            {
                //打开上传文件的输入流
                using (Stream stream = file.OpenReadStream())
                {
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


                    IDictionary<String, Object> cols = rows[0];
                    IDictionary<String, Object> row0 = rows[0];
                    // 从第三个下标开始
                    List<ProcessHoursEnterDto> ProcessHoursEnterDList = new List<ProcessHoursEnterDto>();

                    // 取值
                    var keys = cols.Keys.ToList();
                    var year = (keys.Count - 44) / 3;
                    for (int i = 0; i < year; i++)
                    {
                        int fromStartIndex = i*3  + 44;
                        var y = (row0[keys[fromStartIndex]]).ToString();
                        yearStrs.Add(y);

                    }
                    for (int i = 2; i < rows.Count; i++)
                    {
                        ProcessHoursEnterDto processHoursEnterDto = new ProcessHoursEnterDto();
                        IDictionary<String, Object> row = rows[i];
                        Dictionary<string, object> rowItem = new Dictionary<string, object>();
                        //总数居
                        processHoursEnterDto.ProcessName = (row[keys[2]]).ToString();
                        processHoursEnterDto.ProcessNumber = (row[keys[1]]).ToString();

                        //获取设备
                        Object deviceInfo = new Object();
                        ProcessHoursEnterResponseDeviceDto devices = new ProcessHoursEnterResponseDeviceDto();
                        List<ProcessHoursEnterDeviceDto> processHoursEnterDeviceDtoList = new List<ProcessHoursEnterDeviceDto>();
                        for (int j = 0; j < 3; j++)
                        {
                            ProcessHoursEnterDeviceDto foundationTechnologyDevice = new ProcessHoursEnterDeviceDto();
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
                                foundationTechnologyDevice.DevicePrice = decimal.Parse(val3.ToString());
                            }
                            if (null != val2)
                            {
                                foundationTechnologyDevice.DeviceNumber = decimal.Parse(val2.ToString());

                            }
                            if (null != val0)
                            {
                                foundationTechnologyDevice.DeviceName = val0.ToString();


                            }
                            if (null != val1)
                            {
                                List<FinanceDictionaryDetail> dics = _financeDictionaryDetailRepository.GetAll().Where(p => p.DisplayName == val1.ToString() && p.FinanceDictionaryId == "Sbzt").ToList();
                                //需要转换的地方
                               
                                if (dics != null && dics.Count>0)
                                {
                                    foundationTechnologyDevice.DeviceStatus = dics[0].Id;
                                }
                                else {
                                    foundationTechnologyDevice.DeviceStatus = "";
                                }
                                



                            }
                            processHoursEnterDeviceDtoList.Add(foundationTechnologyDevice);
                        }
                        // 设备总价
                        int ddevNumIndex = startCols + deviceCols;
                        devices.DeviceArr = processHoursEnterDeviceDtoList;
                        devices.DeviceTotalCost = decimal.Parse(row[keys[15]].ToString());
                        processHoursEnterDto.DeviceInfo = devices;



                        // 解析追溯信息
                        ProcessHoursEnterDevelopCostInfoDeviceDto foundationReliableProcessHoursdevelopCostInfoResponseDto = new ProcessHoursEnterDevelopCostInfoDeviceDto();
                        List<ProcessHoursEnterFrockDto> foundationTechnologyDeviceList = new List<ProcessHoursEnterFrockDto>();
                        //硬件设备列表
                        for (int j = 0; j < 2; j++)
                        {
                            Dictionary<string, object> fromItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + 16;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            ProcessHoursEnterFrockDto foundationTechnologyFrockDto = new ProcessHoursEnterFrockDto();
                            foundationTechnologyFrockDto.HardwareDeviceName = val0.ToString();

                            foundationTechnologyFrockDto.HardwareDeviceNumber = decimal.Parse(val1.ToString());
                            foundationTechnologyFrockDto.HardwareDevicePrice = decimal.Parse(val2.ToString());
                            foundationTechnologyDeviceList.Add(foundationTechnologyFrockDto);
                        }
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareInfo = foundationTechnologyDeviceList;
                        // 硬件总价 hardwareTotalPrice
                        if (null != row[keys[22]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareTotalPrice = decimal.Parse(row[keys[22]].ToString());
                        }
                        //追溯软件 traceabilitySoftware
                        if (null != row[keys[23]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.TraceabilitySoftware = (row[keys[23]].ToString());
                        }
                        //追溯软件费用traceabilitySoftwareCost
                        if (null != row[keys[24]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.TraceabilitySoftwareCost = decimal.Parse(row[keys[24]].ToString());
                        }
                        //开图软件 openDrawingSoftware
                        if (null != row[keys[25]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.OpenDrawingSoftware = (row[keys[25]].ToString());
                        }

                        //开图软件费用                        softwarePrice
                        if (null != row[keys[26]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.SoftwarePrice = decimal.Parse(row[keys[26]].ToString()); ;
                        }
                        if (null != row[keys[27]])
                        {
                            foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareDeviceTotalPrice = decimal.Parse(row[keys[27]].ToString());
                        }
                        //软硬件总价                        HardwareDeviceTotalPrice
                        processHoursEnterDto.DevelopCostInfo = foundationReliableProcessHoursdevelopCostInfoResponseDto;




                        // 解析工装治具部分
                        ProcessHoursEnterToolInfoDto foundationReliableProcessHoursFixtureResponseDto = new ProcessHoursEnterToolInfoDto();
                        List<ProcessHoursEnterFixtureDto> foundationTechnologyFixtures = new List<ProcessHoursEnterFixtureDto>();
                        //治具数量
                        for (int j = 0; j < 2; j++)
                        {
                            ProcessHoursEnterFixtureDto foundationTechnologyFixtureDto = new ProcessHoursEnterFixtureDto();
                            int fromStartIndex = j * 3 + 28;
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
                        foundationReliableProcessHoursFixtureResponseDto.ZhiJuArr = foundationTechnologyFixtures;



                        //工装治具总价
                        if (null != row[keys[43]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.DevelopTotalPrice = (row[keys[43]].ToString());


                        }
                        //测试线单价
                        if (null != row[keys[42]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLinePrice = decimal.Parse(row[keys[42]].ToString());


                        }
                        //测试线数量
                        if (null != row[keys[41]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLineNumber = decimal.Parse(row[keys[41]].ToString());

                        }
                        //测试线名称
                        if (null != row[keys[40]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.TestLineName = (row[keys[40]].ToString());

                        }
                        //工装单价
                        if (null != row[keys[39 ]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockPrice = decimal.Parse(row[keys[39]].ToString());

                        }
                        //工装数量
                        if (null != row[keys[38]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockNumber = decimal.Parse(row[keys[38]].ToString());


                        }
                        //工装名称
                        if (null != row[keys[37]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FrockName = (row[keys[37]].ToString());


                        }
                        //检具单价
                        if (null != row[keys[36]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixturePrice = decimal.Parse(row[keys[36]].ToString());


                        }
                        //检具数量
                        if (null != row[keys[35 + 1]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixtureNumber = decimal.Parse(row[keys[35]].ToString());


                        }
                        //检具名称
                        if (null != row[keys[34]])
                        {
                            foundationReliableProcessHoursFixtureResponseDto.FixtureName = (row[keys[34]].ToString());


                        }
                        processHoursEnterDto.ToolInfo = foundationReliableProcessHoursFixtureResponseDto;

                        // 解析年度部分
                        List<ProcessHoursEnterSopInfoDto> foundationWorkingHourItemDtos = new List<ProcessHoursEnterSopInfoDto>();
                        List<Dictionary<string, object>> years = new List<Dictionary<string, object>>();
                        int yearNum = (keys.Count -44) / 3;
                        for (int j = 0; j < yearNum; j++)
                        {
                            ProcessHoursEnterSopInfoDto foundationWorkingHourItem = new ProcessHoursEnterSopInfoDto();

                            string yearstr = yearStrs[j];
                            Dictionary<string, object> yearItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + 44;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            foundationWorkingHourItem.Year = yearstr;
                            List<ProcessHoursEnteritemDto> processHoursEnteritems = new List<ProcessHoursEnteritemDto>();
                            for (int g = 0; g < 1; g++)
                            {
                                Solution entity = await _resourceSchemeTable.GetAsync((long)SolutionId);

                                var query = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == AuditFlowId && t.ProductId == entity.Productld).ToList();

                                ProcessHoursEnteritemDto processHoursEnteritem = new ProcessHoursEnteritemDto();
                                processHoursEnteritem.LaborHour = decimal.Parse(val0.ToString());
                                processHoursEnteritem.MachineHour = decimal.Parse(val1.ToString());
                                processHoursEnteritem.PersonnelNumber = decimal.Parse(val2.ToString());
                                if (query.Count > 0 && null != query[g])
                                {
                                    processHoursEnteritem.ModelCountYearId = query[g].Id;
                                }
                                processHoursEnteritems.Add(processHoursEnteritem);
                            }
                            string str2 = Regex.Replace(yearstr, @"[^0-9]+", "");
                            foundationWorkingHourItem.YearInt = Decimal.Parse(str2);
                            foundationWorkingHourItem.Issues = processHoursEnteritems;
                            foundationWorkingHourItemDtos.Add(foundationWorkingHourItem);


                        }
                        processHoursEnterDto.SopInfo = foundationWorkingHourItemDtos;
                        ProcessHoursEnterDList.Add(processHoursEnterDto);

                    }


                    return ProcessHoursEnterDList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
        }

        /// <summary>
        /// 根据uph值获取线体数量、共线分摊率
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<ProcessHoursEnterLineDtoList>> ListProcessHoursEnterLine(GetProcessHoursEnterUphListDto input)
        {
            List<ProcessHoursEnterLineDtoList> list = new List<ProcessHoursEnterLineDtoList>();
            if (input != null)
            {
                foreach (var item in input.ProcessHoursEnterUphListDtos)
                {
                    ProcessHoursEnterLineDtoList processHoursEnterLineDto = new ProcessHoursEnterLineDtoList();
                    //=组测UPH
                    decimal Zcuph = (decimal)item.Zcuph;

                    decimal rateOfMobilization = 0;
                    decimal CapacityrateOfMobilization = 0;
                    decimal MonthlyWorkingDays = 0;
                    decimal DailyShift = 0;
                    decimal WorkingHours = 0; ;
                    decimal CapacityUtilizationRate = 0; ;
                    //获取年份
                    ModelCountYear modelCountYear = await _modelCountYearRepository.GetAsync(item.ModelCountYearId);
                    //查询制造成本计算参数维护里面的每班正常工作时间*每日班次*月工作天数*稼动率
                    var manufacturingCostInfo = this._manufacturingCostInfoRepository.GetAll().Where(t => t.Year == modelCountYear.Year).ToList();
                    if (manufacturingCostInfo.Count == 0 )
                    {
                        var manufacturingCostInfo1 = this._manufacturingCostInfoRepository.GetAll().Where(y => y.IsDeleted == false).OrderByDescending(t =>t.Year).ToList();
                        //嫁接率
                        rateOfMobilization = manufacturingCostInfo1[0].RateOfMobilization;
                        CapacityrateOfMobilization = manufacturingCostInfo1[0].RateOfMobilization / 100;
                        //月工作天数
                        MonthlyWorkingDays = (decimal)manufacturingCostInfo1[0].MonthlyWorkingDays;
                        //每日班次
                        DailyShift = (decimal)manufacturingCostInfo1[0].DailyShift;
                        //每班正常工作时间
                        WorkingHours = (decimal)manufacturingCostInfo1[0].WorkingHours;
                        //产能利用率
                        CapacityUtilizationRate = (decimal)manufacturingCostInfo1[0].CapacityUtilizationRate / 100;
                    }
                    if (manufacturingCostInfo.Count > 0)
                    {
                        //嫁接率
                        rateOfMobilization = manufacturingCostInfo[0].RateOfMobilization;
                        CapacityrateOfMobilization = manufacturingCostInfo[0].RateOfMobilization/100;
                        //月工作天数
                        MonthlyWorkingDays = (decimal)manufacturingCostInfo[0].MonthlyWorkingDays;
                        //每日班次
                        DailyShift = (decimal)manufacturingCostInfo[0].DailyShift;
                        //每班正常工作时间
                        WorkingHours = (decimal)manufacturingCostInfo[0].WorkingHours;
                        //产能利用率
                        CapacityUtilizationRate = (decimal)manufacturingCostInfo[0].CapacityUtilizationRate/100;
                    }
                    //每月产能
                    decimal Capacity = Zcuph * CapacityrateOfMobilization * MonthlyWorkingDays * DailyShift * WorkingHours;
                    //每月需求
                    decimal month = 0;
                    if (modelCountYear.UpDown == YearType.Year)
                    {
                        month = 12;
                    }
                    else
                    {
                        month = 6;
                    }
                    //每月需求
                    decimal lineQuantity = modelCountYear.Quantity * 1000 / month;
                    //线体数量
                    decimal XtslVale = 0;
                    decimal GtVale = 0;
                    if (!Capacity.Equals(0.000M))
                    {
                        decimal xtftl = 0;
                        decimal Xtsl = Math.Ceiling(lineQuantity / Capacity);
                        //线体分摊率
                        if (!Xtsl.Equals(0.000M))
                        {
                            decimal x = (Capacity / Xtsl);
                            if (x.Equals(0.000M))
                            {
                                xtftl = 0;
                            }
                            else
                            {
                                decimal b = Capacity * Xtsl;
                                if (!b.Equals(0.000M))
                                {
                                    if (!CapacityUtilizationRate.Equals((0.000M)))
                                    {
                                        xtftl = (lineQuantity / b) / CapacityUtilizationRate;
                                    }
                                    else
                                    {
                                        xtftl = 0;
                                    }
                                }
                                else
                                {
                                    xtftl = 0;
                                }
                            }

                        }
                        else
                        {
                            xtftl = 0;
                        }


                        if (xtftl > 1)
                        {
                            xtftl = 100;
                        }
                        if (xtftl < 1)
                        {
                            xtftl = xtftl*100;
                        }

                        XtslVale = Xtsl;
                        GtVale = decimal.Parse(xtftl.ToString("0.00"));
                    }


                    processHoursEnterLineDto.Xtsl = XtslVale;
                    processHoursEnterLineDto.Gxftl = GtVale;
                    processHoursEnterLineDto.ModelCountYearId = item.ModelCountYearId;
                    if (modelCountYear.UpDown == YearType.FirstHalf)
                    {

                        processHoursEnterLineDto.Year = modelCountYear.Year + "上半年";
                    }
                    else if (modelCountYear.UpDown == YearType.SecondHalf)
                    {
                        processHoursEnterLineDto.Year = modelCountYear.Year + "下半年";
                    }
                    else
                    {
                        processHoursEnterLineDto.Year = modelCountYear.Year.ToString();
                    }
                    list.Add(processHoursEnterLineDto);

                }
            }
            return list;
        }

        /// <summary>
        /// 模版下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> TemplateDownload(GetProcessHoursEntersInput input)
        {
            //无数据的情况下
            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

            var yearCountList = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();

            IWorkbook wk = new XSSFWorkbook();
            ISheet sheet = wk.CreateSheet("工时工序模版");
            sheet.DefaultRowHeight = 25 * 20;
            // 表头设置
            IRow herdRow = sheet.CreateRow(0);
            CreateCell(herdRow, 0, "序号", wk);
            sheet.SetColumnWidth(0, 10 * 300);
            CreateCell(herdRow, 1, "工序编号", wk);
            sheet.SetColumnWidth(1, 10 * 300);
            CreateCell(herdRow, 2, "工序名称", wk);
            sheet.SetColumnWidth(2, 10 * 400);
            CreateCell(herdRow, 3, "设备", wk);
            sheet.SetColumnWidth(3, 10 * 500);
            CreateCell(herdRow, 13, "追溯部分(硬件及软件开发费用)", wk);
            sheet.SetColumnWidth(4, 10 * 500);
            CreateCell(herdRow, 25, "工装治具部分", wk);
            sheet.SetColumnWidth(5, 10 * 500);

            MergedRegion(sheet, 0, 1, 0, 0);
            MergedRegion(sheet, 0, 1, 1, 1);
            MergedRegion(sheet, 0, 1, 2, 2);
            MergedRegion(sheet, 0, 0, 3, 15);
            MergedRegion(sheet, 0, 0, 16, 27);
            MergedRegion(sheet, 0, 0, 28, 40);

            int colIndex = 0;


            // 副表头
            IRow herdRow2 = sheet.CreateRow(1);
            CreateCell(herdRow2, 0, string.Empty, wk);
            var query = (from a in _fProcessesRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.ProcessNumber).Distinct() select a).ToList();
            var queryName = (from a in _fProcessesRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.ProcessName).Distinct()  select a).ToList();

            List<string> list = new List<string>();
            int index = 0;
            foreach (var item in queryName)
            {
                list.Add(item);
                index++;
                if (index == 30) 
                {
                    break;
                }
            }
            
            CreateCell(herdRow2, 1, string.Empty, wk);
            CreateCell(herdRow2, 2, string.Empty, wk);
            new ExcelCellDropdownParame(1, 1, query.ToArray()).SetCellDropdownList(sheet);
            new ExcelCellDropdownParame(2, 2, list.ToArray()).SetCellDropdownList(sheet);

            ISheet sheet1 = wk.CreateSheet("工序库");

            //创建头部
            IRow row001 = sheet1.CreateRow(0);
            row001.CreateCell(0).SetCellValue("工序编号");
            sheet1.SetColumnWidth(0, 10 * 500);
            row001.CreateCell(1).SetCellValue("工序名称");
            sheet1.SetColumnWidth(1, 10 * 500);
            row001.CreateCell(2).SetCellValue("工序维护人");
            sheet1.SetColumnWidth(2, 10 * 500);
            row001.CreateCell(3).SetCellValue("维护时间");
            sheet1.SetColumnWidth(3, 10 * 500);
            var fProcessesQuery = _fProcessesRepository.GetAllList(p => p.IsDeleted == false).ToList();

            int row = 0;
            foreach (var item in fProcessesQuery)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                row = row + 1;
                IRow herdRow3 = sheet1.CreateRow(row);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(herdRow3, 0, item.ProcessNumber, wk);
                CreateCell(herdRow3, 1, item.ProcessName, wk);
                if (null != user)
                {

                    CreateCell(herdRow3, 2, user.Name, wk);
                }
                else {

                    CreateCell(herdRow3, 2, "", wk);
                }
                CreateCell(herdRow3, 3, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);
            

            }


            var DeviceItem = (from a in _foundationDeviceItemRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.DeviceName).Distinct()  select a).ToList();

            List<string> listDeviceItem = new List<string>();
            int indexDevice = 0;
            foreach (var item in DeviceItem)
            {
                listDeviceItem.Add(item);
                indexDevice++;
                if (indexDevice == 30)
                {
                    break;
                }
            }
            CreateCell(herdRow2, 3, "设备1名称", wk);
            new ExcelCellDropdownParame(3, 3, listDeviceItem.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 4, "设备1状态", wk);
            CreateCell(herdRow2, 5, "设备1数量", wk);
            CreateCell(herdRow2, 6, "设备1单价", wk);
            CreateCell(herdRow2, 7, "设备2名称", wk);
            CreateCell(herdRow2, 8, "设备2状态", wk);
            new ExcelCellDropdownParame(7, 7, listDeviceItem.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 9, "设备2数量", wk);
            CreateCell(herdRow2, 10, "设备2单价", wk);
            CreateCell(herdRow2, 11, "设备3名称", wk);
            CreateCell(herdRow2, 12, "设备3状态", wk);
            new ExcelCellDropdownParame(11, 11, listDeviceItem.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 13, "设备3数量", wk);
            CreateCell(herdRow2, 14, "设备3单价", wk);
            CreateCell(herdRow2, 15, "设备总价", wk);


            ISheet sheet2 = wk.CreateSheet("设备库");
            //创建头部
            IRow row002 = sheet2.CreateRow(0);
            row002.CreateCell(0).SetCellValue("工序编号");
            sheet2.SetColumnWidth(0, 10 * 500);
            row002.CreateCell(1).SetCellValue("工序名称");
            sheet2.SetColumnWidth(1, 10 * 500);
            row002.CreateCell(2).SetCellValue("设备1名称");
            sheet2.SetColumnWidth(2, 10 * 500);;
            row002.CreateCell(3).SetCellValue("设备1状态");
            sheet2.SetColumnWidth(3, 10 * 500);
            row002.CreateCell(4).SetCellValue("设备1单价");
            sheet2.SetColumnWidth(4, 10 * 500);
            row002.CreateCell(5).SetCellValue("设备1供应商");
            sheet2.SetColumnWidth(5, 10 * 500);
            row002.CreateCell(6).SetCellValue("设备2名称");
            sheet2.SetColumnWidth(6, 10 * 500); ;
            row002.CreateCell(7).SetCellValue("设备2状态");
            sheet2.SetColumnWidth(7, 10 * 500);
            row002.CreateCell(8).SetCellValue("设备2单价");
            sheet2.SetColumnWidth(8, 10 * 500);
            row002.CreateCell(9).SetCellValue("设备2供应商");
            sheet2.SetColumnWidth(9, 10 * 500);
            row002.CreateCell(10).SetCellValue("设备3名称");
            sheet2.SetColumnWidth(10, 10 * 500); ;
            row002.CreateCell(11).SetCellValue("设备3状态");
            sheet2.SetColumnWidth(11, 10 * 500);
            row002.CreateCell(12).SetCellValue("设备3单价");
            sheet2.SetColumnWidth(12, 10 * 500);
            row002.CreateCell(13).SetCellValue("设备3供应商");
            sheet2.SetColumnWidth(13, 10 * 500);
            row002.CreateCell(15).SetCellValue("维护时间");
            sheet2.SetColumnWidth(15, 10 * 500);
            row002.CreateCell(14).SetCellValue("维护人");
            sheet2.SetColumnWidth(14, 10 * 500);

             /*设备*/
            var foundationDeviceQuery = _foundationDeviceRepository.GetAllList(p => p.IsDeleted == false).ToList();

            int rowFoundationDevice = 0;
            foreach (var item in foundationDeviceQuery)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                rowFoundationDevice = rowFoundationDevice + 1;
                IRow row3 = sheet2.CreateRow(rowFoundationDevice);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(row3, 0, item.ProcessNumber, wk);
                CreateCell(row3, 1, item.ProcessName, wk);
                var FoundationDeviceItemlist = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == item.Id).ToList();
                if (null != FoundationDeviceItemlist && FoundationDeviceItemlist.Count==3)
                {
                    CreateCell(row3, 2, FoundationDeviceItemlist[0].DeviceName, wk);
                    if (null != FoundationDeviceItemlist[0].DeviceStatus)
                    {
                        var entityDictionary = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[0].DeviceStatus.ToString());
                        if (null != entityDictionary)
                        {
                            CreateCell(row3, 3, entityDictionary.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 3, "", wk);

                        }
                    }
                    else {

                        CreateCell(row3, 3, "", wk);
                    }
               
                    CreateCell(row3, 4, FoundationDeviceItemlist[0].DevicePrice, wk);
                    CreateCell(row3, 5, FoundationDeviceItemlist[0].DeviceProvider, wk);
                    CreateCell(row3, 6, FoundationDeviceItemlist[1].DeviceName, wk);
                    if (null != FoundationDeviceItemlist[1].DeviceStatus)
                    {
                        var entityDictionary1 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[1].DeviceStatus.ToString());
                        if (null != entityDictionary1)
                        {
                            CreateCell(row3, 7, entityDictionary1.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 7, "", wk);

                        }
                    }
                    else {
                        CreateCell(row3, 7, "", wk);
                    }
                    CreateCell(row3, 8, FoundationDeviceItemlist[1].DevicePrice, wk);
                    CreateCell(row3, 9, FoundationDeviceItemlist[1].DeviceProvider, wk);
                    CreateCell(row3, 10, FoundationDeviceItemlist[2].DeviceName, wk);
                    if (null != FoundationDeviceItemlist[2].DeviceStatus)
                    {
                        var entityDictionary2 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[2].DeviceStatus.ToString());
                        if (null != entityDictionary2)
                        {
                            CreateCell(row3, 11, entityDictionary2.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 11, "", wk);

                        }
                    }
                    else {

                        CreateCell(row3, 11, "", wk);
                    }
                    CreateCell(row3, 12, FoundationDeviceItemlist[2].DevicePrice, wk);
                    CreateCell(row3, 13, FoundationDeviceItemlist[2].DeviceProvider, wk);
                }

                if (null != user)
                {

                    CreateCell(row3, 14, user.Name, wk);
                }
                else
                {

                    CreateCell(row3, 14, "", wk);
                }
                CreateCell(row3, 15, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);


            }

            var HardwareName = (from a in _foundationHardwareItemRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.HardwareName).Distinct()select a).ToList();

            List<string> HardwareItem = new List<string>();
            int indexHardwareItem = 0;
            foreach (var item in HardwareName)
            {
                HardwareItem.Add(item);
                indexHardwareItem++;
                if (indexHardwareItem == 30)
                {
                    break;
                }
            }
            CreateCell(herdRow2, 16, "硬件设备1", wk);
            new ExcelCellDropdownParame(16, 16, HardwareItem.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 17, "数量", wk);
            CreateCell(herdRow2, 18, "单价设备1", wk);
            CreateCell(herdRow2, 19, "硬件设备2", wk);
            new ExcelCellDropdownParame(19, 19, HardwareItem.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 20, "数量", wk);
            CreateCell(herdRow2, 21, "单价设备2", wk);
            CreateCell(herdRow2, 22, "硬件总价", wk);



            ISheet sheet3 = wk.CreateSheet("硬件软件库");
            //创建头部
            IRow row003 = sheet3.CreateRow(0);
            row003.CreateCell(0).SetCellValue("工序编号");
            sheet3.SetColumnWidth(0, 10 * 500);
            row003.CreateCell(1).SetCellValue("工序名称");
            sheet3.SetColumnWidth(1, 10 * 500);
            row003.CreateCell(2).SetCellValue("硬件1名称");
            sheet3.SetColumnWidth(2, 10 * 500);
            row003.CreateCell(3).SetCellValue("硬件1状态");
            sheet3.SetColumnWidth(3, 10 * 500);
            row003.CreateCell(4).SetCellValue("硬件1单价");
            sheet3.SetColumnWidth(4, 10 * 500);
            row003.CreateCell(5).SetCellValue("硬件1供应商");
            sheet3.SetColumnWidth(5, 10 * 500);
            row003.CreateCell(6).SetCellValue("硬件2名称");
            sheet3.SetColumnWidth(6, 10 * 500);
            row003.CreateCell(7).SetCellValue("硬件2状态");
            sheet3.SetColumnWidth(7, 10 * 500);
            row003.CreateCell(8).SetCellValue("硬件2单价");
            sheet3.SetColumnWidth(8, 10 * 500);
            row003.CreateCell(9).SetCellValue("硬件2供应商");
            sheet3.SetColumnWidth(10, 10 * 500);
            row003.CreateCell(10).SetCellValue("追溯软件");
            sheet3.SetColumnWidth(11, 10 * 500);
            row003.CreateCell(11).SetCellValue("追溯软件费用");
            sheet3.SetColumnWidth(12, 10 * 500);
            row003.CreateCell(12).SetCellValue("软件名称");
            sheet3.SetColumnWidth(13, 10 * 500);
            row003.CreateCell(13).SetCellValue("软件状态");
            sheet3.SetColumnWidth(14, 10 * 500);
            row003.CreateCell(14).SetCellValue("软件单价");
            sheet3.SetColumnWidth(15, 10 * 500);
            row003.CreateCell(15).SetCellValue("软件供应商");
            sheet3.SetColumnWidth(17, 10 * 500);
            row003.CreateCell(17).SetCellValue("维护时间");
            sheet3.SetColumnWidth(16, 10 * 500);
            row003.CreateCell(16).SetCellValue("维护人");
            sheet3.SetColumnWidth(18, 10 * 500);
            /*硬件软件库*/
            var foundationHardwareQuery = _foundationHardwareRepository.GetAllList(p => p.IsDeleted == false).ToList();

            int rowfoundationHardware = 0;
            foreach (var item in foundationHardwareQuery)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                rowfoundationHardware = rowfoundationHardware + 1;
                IRow row3 = sheet3.CreateRow(rowfoundationHardware);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(row3, 0, item.ProcessNumber, wk);
                CreateCell(row3, 1, item.ProcessName, wk);
                var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == item.Id).ToList();
                if (null != FoundationDeviceItemlist && FoundationDeviceItemlist.Count == 2)
                {
                    CreateCell(row3, 2, FoundationDeviceItemlist[0].HardwareName, wk);
                    if (null != FoundationDeviceItemlist[0].HardwareState)
                    {
                        var entityDictionary2 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[0].HardwareState.ToString());
                        if (null != entityDictionary2)
                        {
                            CreateCell(row3, 3, entityDictionary2.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 3, "", wk);

                        }
                    }
                    else
                    {
                        CreateCell(row3, 3, "", wk);
                    }

                    CreateCell(row3, 4, FoundationDeviceItemlist[0].HardwarePrice.ToString(), wk);
                    CreateCell(row3, 5, FoundationDeviceItemlist[0].HardwareBusiness, wk);
                    CreateCell(row3, 6, FoundationDeviceItemlist[1].HardwareName, wk);

                    if (null != FoundationDeviceItemlist[1].HardwareState)
                    {
                        var entityDictionary4 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[1].HardwareState.ToString());
                        if (null != entityDictionary4)
                        {
                            CreateCell(row3, 7, entityDictionary4.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 7, "", wk);

                        }
                    }
                    else
                    {
                        CreateCell(row3, 7, "", wk);
                    }


                    CreateCell(row3, 8, FoundationDeviceItemlist[1].HardwarePrice.ToString(), wk);
                    CreateCell(row3, 9, FoundationDeviceItemlist[1].HardwareBusiness, wk);
                }
                if (null != item.TraceabilitySoftware)
                {
                    CreateCell(row3, 10, item.TraceabilitySoftware, wk);
                }
                else
                {
                    CreateCell(row3, 10, null, wk);
                }

                if (null != item.TraceabilitySoftwareCost)
                {
                    CreateCell(row3, 11, item.TraceabilitySoftwareCost.ToString(), wk);
                }
                else
                {
                    CreateCell(row3, 11, null, wk);
                }
                CreateCell(row3, 12, item.SoftwareName, wk);
                if (null != item.SoftwareState)
                {
                    var entityDictionary3 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == item.SoftwareState.ToString());
                    if (null != entityDictionary3)
                    {
                        CreateCell(row3, 13, entityDictionary3.DisplayName, wk);

                    }
                    else
                    {
                        CreateCell(row3, 13, "", wk);

                    }
                }
                else {

                    CreateCell(row3, 13, "", wk);
                }
                CreateCell(row3, 14, item.SoftwarePrice.ToString(), wk);
                if (null != item.SoftwareName)
                {
                    CreateCell(row3, 15, item.SoftwareName, wk);
                }
                else
                {
                    CreateCell(row3, 15, null, wk);
                }
                if (null != user)
                {

                    CreateCell(row3, 16, user.Name, wk);
                }
                else
                {

                    CreateCell(row3, 16, "", wk);
                }
                CreateCell(row3, 17, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);


            }



            CreateCell(herdRow2, 23, "追溯软件", wk);
            CreateCell(herdRow2, 24, "开发费(追溯)", wk);
            CreateCell(herdRow2, 25, "开图软件", wk);
            CreateCell(herdRow2, 26, "开发费(开图)", wk);
            CreateCell(herdRow2, 27, "软硬件总价", wk);
            var FixtureItem = (from a in _foundationFoundationFixtureItemRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.FixtureName).Distinct() select a).ToList();
            CreateCell(herdRow2, 28, "治具1名称", wk);
            List<string> FixtureItemList = new List<string>();
            int indexFixtureItem = 0;
            foreach (var item in FixtureItem)
            {
                FixtureItemList.Add(item);
                indexFixtureItem++;
                if (indexFixtureItem == 25)
                {
                    break;
                }
            }
            new ExcelCellDropdownParame(28, 28, FixtureItemList.ToArray()).SetCellDropdownList(sheet);

            ISheet sheet4 = wk.CreateSheet("治具检具");
            //创建头部
            IRow row004 = sheet4.CreateRow(0);
            row004.CreateCell(0).SetCellValue("工序编号");
            sheet4.SetColumnWidth(0, 10 * 500);
            row004.CreateCell(1).SetCellValue("工序名称");
            sheet4.SetColumnWidth(1, 10 * 500);
            row004.CreateCell(2).SetCellValue("治具1名称");
            sheet4.SetColumnWidth(2, 10 * 500);
            row004.CreateCell(3).SetCellValue("治具1状态");
            sheet4.SetColumnWidth(3, 10 * 500);
            row004.CreateCell(4).SetCellValue("治具1单价");
            sheet4.SetColumnWidth(4, 10 * 500);
            row004.CreateCell(5).SetCellValue("治具1供应商");
            sheet4.SetColumnWidth(5, 10 * 500);
            row004.CreateCell(6).SetCellValue("治具2名称");
            sheet4.SetColumnWidth(6, 10 * 500);
            row004.CreateCell(7).SetCellValue("治具2状态");
            sheet4.SetColumnWidth(7, 10 * 500);
            row004.CreateCell(8).SetCellValue("治具2单价");
            sheet4.SetColumnWidth(8, 10 * 500);
            row004.CreateCell(9).SetCellValue("治具2供应商");
            sheet4.SetColumnWidth(9, 10 * 500);
            row004.CreateCell(10).SetCellValue("检具名称");
            sheet4.SetColumnWidth(10, 10 * 500);
            row004.CreateCell(11).SetCellValue("检具状态");
            sheet4.SetColumnWidth(11, 10 * 500);
            row004.CreateCell(12).SetCellValue("检具单价");
            sheet4.SetColumnWidth(12, 10 * 500);
            row004.CreateCell(13).SetCellValue("检具单价供应商");
            sheet4.SetColumnWidth(13, 10 * 500);
            row004.CreateCell(14).SetCellValue("检具维护人");
            sheet4.SetColumnWidth(14, 10 * 500);
            row004.CreateCell(15).SetCellValue("维护时间");
            sheet4.SetColumnWidth(15, 10 * 500);


            /*检具库*/
            var foundationFixtureQuery = _foundationFixtureRepository.GetAllList(p => p.IsDeleted == false).ToList();

            int rowfoundationFixture = 0;
            foreach (var item in foundationFixtureQuery)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                rowfoundationFixture = rowfoundationFixture + 1;
                IRow row3 = sheet4.CreateRow(rowfoundationFixture);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(row3, 0, item.ProcessNumber, wk);
                CreateCell(row3, 1, item.ProcessName, wk);
                var FoundationDeviceItemlist = this._foundationFoundationFixtureItemRepository.GetAll().Where(f => f.FoundationFixtureId == item.Id).ToList();
                if (null != FoundationDeviceItemlist && FoundationDeviceItemlist.Count == 2)
                {
                    CreateCell(row3, 2, FoundationDeviceItemlist[0].FixtureName, wk);
                    if (null != FoundationDeviceItemlist[0].FixtureState)
                    {


                        var entityDictionary3 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[0].FixtureState.ToString());
                        if (null != entityDictionary3)
                        {
                            CreateCell(row3, 3, entityDictionary3.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 3, "", wk);

                        }
                    }
                    else {
                        CreateCell(row3, 3, "", wk);

                    }
                    CreateCell(row3, 4, FoundationDeviceItemlist[0].FixturePrice.ToString(), wk);
                    CreateCell(row3, 5, FoundationDeviceItemlist[0].FixtureProvider, wk);
                    CreateCell(row3, 6, FoundationDeviceItemlist[1].FixtureName, wk);
                    if (null != FoundationDeviceItemlist[1].FixtureState)
                    {
                        var entityDictionary4 = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == FoundationDeviceItemlist[1].FixtureState.ToString());
                        if (null != entityDictionary4)
                        {
                            CreateCell(row3, 7, entityDictionary4.DisplayName, wk);

                        }
                        else
                        {
                            CreateCell(row3, 7, "", wk);

                        }
                    }
                    else {
                        CreateCell(row3, 7, "", wk);

                    }
                    CreateCell(row3, 8, FoundationDeviceItemlist[1].FixturePrice.ToString(), wk);
                    CreateCell(row3, 9, FoundationDeviceItemlist[1].FixtureProvider, wk);
                }
                CreateCell(row3, 10, item.FixtureGaugeName, wk);
                CreateCell(row3, 11, item.FixtureGaugeState, wk);
                CreateCell(row3, 13, item.FixtureGaugeBusiness, wk);

                if (null != item.FixtureGaugePrice)
                {
                    CreateCell(row3, 12, item.FixtureGaugePrice.ToString(), wk);
                }
                else
                {
                    CreateCell(row3, 12, null, wk);
                }
          
                if (null != user)
                {

                    CreateCell(row3, 14, user.Name, wk);
                }
                else
                {

                    CreateCell(row3, 14, "", wk);
                }
                CreateCell(row3, 15, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);


            }



            CreateCell(herdRow2, 29, "数量", wk);
            CreateCell(herdRow2, 30, "治具单价", wk);
            CreateCell(herdRow2, 31, "治具2名称", wk);
            new ExcelCellDropdownParame(31, 31, FixtureItemList.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 32, "数量", wk);
            CreateCell(herdRow2, 33, "治具单价", wk);

            var FixtureGaugeNameItem = (from a in _foundationFixtureRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.FixtureGaugeName).Distinct()       select a).ToList();
            CreateCell(herdRow2, 34, "检具名称", wk);
            List<string> FixtureGaugeNameList = new List<string>();
            int indexFixtureGaugeName = 0;
            foreach (var item in FixtureGaugeNameItem)
            {
                FixtureGaugeNameList.Add(item);
                indexFixtureItem++;
                if (indexFixtureGaugeName == 30)
                {
                    break;
                }
            }
            new ExcelCellDropdownParame(34, 34, FixtureGaugeNameList.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 35, "数量", wk);
            CreateCell(herdRow2, 36, "检具单价", wk);
            var ProcessNameItem = (from a in _foundationProcedureRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.InstallationName).Distinct()   select a).ToList();
            List<string> ProcessNameList = new List<string>();
            int indexProcessName = 0;
            foreach (var item in ProcessNameItem)
            {
                ProcessNameList.Add(item);
                indexProcessName++;
                if (indexProcessName == 30)
                {
                    break;
                }
            }
            CreateCell(herdRow2, 37, "工装名称", wk);
            new ExcelCellDropdownParame(37, 37, ProcessNameList.ToArray()).SetCellDropdownList(sheet);
            CreateCell(herdRow2, 38, "数量", wk);
            CreateCell(herdRow2, 39, "工装单价", wk);
            var TestNameItem = (from a in _foundationProcedureRepository.GetAllList(p => p.IsDeleted == false).Select(p => p.TestName).Distinct()select a).ToList();
            List<string> TestNameList = new List<string>();
            int indexTestName = 0;
            foreach (var item in TestNameItem)
            {
                TestNameList.Add(item);
                indexTestName++;
                if (indexTestName == 30)
                {
                    break;
                }
            }
            CreateCell(herdRow2, 40, "测试线名称", wk);
            new ExcelCellDropdownParame(40, 40, TestNameList.ToArray()).SetCellDropdownList(sheet);

            CreateCell(herdRow2, 41, "数量", wk);
            CreateCell(herdRow2, 42, "线束单价", wk);
            CreateCell(herdRow2, 43, "工装治具检具总价", wk);

            ISheet sheet5 = wk.CreateSheet("工装库");
            //创建头部
            IRow row005 = sheet5.CreateRow(0);
            row005.CreateCell(0).SetCellValue("工序编号");
            sheet5.SetColumnWidth(0, 10 * 500);
            row005.CreateCell(1).SetCellValue("工序名称");
            sheet5.SetColumnWidth(1, 10 * 500);
            row005.CreateCell(2).SetCellValue("工装名称");
            sheet5.SetColumnWidth(2, 10 * 500);
            row005.CreateCell(3).SetCellValue("工装单价");
            sheet5.SetColumnWidth(3, 10 * 500);
            row005.CreateCell(4).SetCellValue("工装供应商");
            sheet5.SetColumnWidth(4, 10 * 500);
            row005.CreateCell(5).SetCellValue("测试线名称");
            sheet5.SetColumnWidth(5, 10 * 500);
            row005.CreateCell(6).SetCellValue("测试线单价");
            sheet5.SetColumnWidth(2, 10 * 500);
            row005.CreateCell(7).SetCellValue("工装维护人");
            sheet5.SetColumnWidth(16, 10 * 500);
            row005.CreateCell(8).SetCellValue("维护时间");
            sheet5.SetColumnWidth(17, 10 * 500);

            /*工装库*/
            var foundationProcedureQuery = _foundationProcedureRepository.GetAllList(p => p.IsDeleted == false).ToList();

            int rowfoundationProcedure = 0;
            foreach (var item in foundationProcedureQuery)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                rowfoundationProcedure = rowfoundationProcedure + 1;
                IRow row3 = sheet5.CreateRow(rowfoundationProcedure);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(row3, 0, item.ProcessNumber, wk);
                CreateCell(row3, 1, item.ProcessName, wk);
                CreateCell(row3, 2, item.InstallationName, wk);
                CreateCell(row3, 3, item.InstallationPrice.ToString(), wk);
                CreateCell(row3, 4, item.InstallationSupplier, wk);
                CreateCell(row3, 5, item.TestName, wk);
                CreateCell(row3, 6, item.TestPrice.ToString(), wk);
                
                if (null != user)
                {

                    CreateCell(row3, 7, user.Name, wk);
                }
                else
                {

                    CreateCell(row3, 7, "", wk);
                }
                CreateCell(row3, 8, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);


            }



            ISheet sheet6 = wk.CreateSheet("工时库");
            //创建头部
            IRow row006 = sheet6.CreateRow(0);
            IRow row007 = sheet6.CreateRow(1);
            row006.CreateCell(0).SetCellValue("工序编号");
            sheet6.SetColumnWidth(2, 10 * 500);
            row006.CreateCell(1).SetCellValue("工序名称");
            sheet6.SetColumnWidth(1, 10 * 500);

            int colIndexRow =2;
           List<String> FoundationDeviceItemYear = this._foundationFoundationWorkingHourItemRepository.GetAllList(w =>w.IsDeleted ==false).OrderBy(y => y.Year).GroupBy(t => t.Year).Select(s => s.First()).Select(i => i.Year).ToList();
           // var FoundationDeviceItemYear = _foundationFoundationWorkingHourItemRepository.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重

            if (null != FoundationDeviceItemYear && FoundationDeviceItemYear.Count > 0)
            {
                for (int i = 0; i < FoundationDeviceItemYear.Count(); i++)
                {
                    if (i == 0)
                    {
                        string yaer = FoundationDeviceItemYear[i];
                        row006.CreateCell(colIndexRow).SetCellValue(yaer);
                        MergedRegion(sheet6, 0, 0, colIndexRow, colIndexRow + 2);
                        colIndexRow =  3 + colIndexRow;
                        row007.CreateCell(2).SetCellValue("标准人工工时");
                        row007.CreateCell(3).SetCellValue("标准机器工时");
                        row007.CreateCell(4).SetCellValue("人员数量");

                    }
                    else {
                        string yaer = FoundationDeviceItemYear[i];
                        row006.CreateCell(colIndexRow).SetCellValue(yaer);
                        MergedRegion(sheet6, 0, 0, colIndexRow, colIndexRow + 2);
                        row007.CreateCell(colIndexRow).SetCellValue("标准人工工时");
                        row007.CreateCell(colIndexRow + 1).SetCellValue("标准机器工时");
                        row007.CreateCell(colIndexRow + 2).SetCellValue("人员数量");
                        colIndexRow = 3 + colIndexRow;


              
                    }
                  
                   
                }
            }
        
            MergedRegion(sheet6, 0, 1, 0, 0);
            MergedRegion(sheet6, 0, 1, 1, 1);
            row006.CreateCell(colIndexRow).SetCellValue("工时维护人");
            sheet6.SetColumnWidth(colIndexRow, 10 * 500);
            MergedRegion(sheet6, 0, 1, colIndexRow, colIndexRow);
            row006.CreateCell(colIndexRow +1).SetCellValue("维护时间");
            sheet6.SetColumnWidth(colIndexRow+1, 10 * 500);
            MergedRegion(sheet6, 0, 1, colIndexRow+1, colIndexRow+1);

            var queryYear = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false).ToList();

            int rowfoundationYear = 1;
            foreach (var item in queryYear)
            {
                var user = _userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                rowfoundationYear = rowfoundationYear + 1;
                IRow row3 = sheet6.CreateRow(rowfoundationYear);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(row3, 0, item.ProcessNumber, wk);
                CreateCell(row3, 1, item.ProcessName, wk);
                var FoundationDeviceItemlist = this._foundationFoundationWorkingHourItemRepository.GetAll().Where(f => f.FoundationWorkingHourId == item.Id).ToList();
                int WorkingHour = 1;
                foreach (var itemWorkingHour in FoundationDeviceItemlist)
                {
                    CreateCell(row3, WorkingHour + 1, itemWorkingHour.LaborHour, wk);
                    CreateCell(row3, WorkingHour + 2, itemWorkingHour.MachineHour, wk);
                    CreateCell(row3, WorkingHour + 3, itemWorkingHour.NumberPersonnel, wk);
                    WorkingHour += 3;
                }

                if (null != user)
                {

                    CreateCell(row3, WorkingHour + 1, user.Name, wk);
                }
                else
                {

                    CreateCell(row3, WorkingHour + 1, "", wk);
                }

                CreateCell(row3, WorkingHour + 2, item.LastModificationTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), wk);


            }

            int c = 43;
            int d = 6;

            for (int i = 0; i < yearCountList.Count; i++)
            {
                if (i == 0)
                {
                    string year = "";
                    if (yearCountList[i].UpDown.Equals(YearType.FirstHalf))
                    {
                        year = yearCountList[i].Year + "上半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.SecondHalf))
                    {
                        year = yearCountList[i].Year + "下半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.Year))
                    {
                        year = yearCountList[i].Year + "";

                    }

                    CreateCell(herdRow, c + 1, year, wk);
                    sheet.SetColumnWidth(d, 10 * 500);

                    MergedRegion(sheet, 0, 0, c + 1, c + 3);
                    c += 4;
                    d += 2;
                }
                else
                {
                    string year = "";
                    if (yearCountList[i].UpDown.Equals(YearType.FirstHalf))
                    {
                        year = yearCountList[i].Year + "上半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.SecondHalf))
                    {
                        year = yearCountList[i].Year + "下半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.Year))
                    {
                        year = yearCountList[i].Year + "";

                    }

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
        /// 导出工时工序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> exportDownload(GetProcessHoursEntersInput input)
        {
            //无数据的情况下
            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

            var yearCountList = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == entity.Productld).ToList();

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
            sheet.SetColumnWidth(2, 10 * 400);
            CreateCell(herdRow, 3, "设备", wk);
            sheet.SetColumnWidth(3, 10 * 500);
            CreateCell(herdRow, 4, "追溯部分(硬件及软件开发费用)", wk);
            sheet.SetColumnWidth(4, 10 * 500);
            CreateCell(herdRow, 5, "工装治具部分", wk);
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

            for (int i = 0; i < yearCountList.Count; i++)
            {
                if (i == 0)
                {
                    string year = "";
                    if (yearCountList[i].UpDown.Equals(YearType.FirstHalf))
                    {
                        year = yearCountList[i].Year + "上半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.SecondHalf))
                    {
                        year = yearCountList[i].Year + "下半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.Year))
                    {
                        year = yearCountList[i].Year + "";

                    }

                    CreateCell(herdRow, c + 1, year, wk);
                    sheet.SetColumnWidth(d, 10 * 500);

                    MergedRegion(sheet, 0, 0, c + 1, c + 3);
                    c += 4;
                    d += 2;
                }
                else
                {
                    string year = "";
                    if (yearCountList[i].UpDown.Equals(YearType.FirstHalf))
                    {
                        year = yearCountList[i].Year + "上半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.SecondHalf))
                    {
                        year = yearCountList[i].Year + "下半年";

                    }
                    if (yearCountList[i].UpDown.Equals(YearType.Year))
                    {
                        year = yearCountList[i].Year + "";

                    }

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

            // 设置查询条件
            var list = this._processHoursEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId).ToList();

            // 查询数据
            //数据转换
            int row = 1;
            foreach (var item in list)
            {
                row =row +1;
                IRow herdRow3 = sheet.CreateRow(row);
                ProcessHoursEnterDto processHoursEnter = new ProcessHoursEnterDto();
                CreateCell(herdRow3, 0, item.Id.ToString(), wk);
                CreateCell(herdRow3, 1, item.ProcessNumber, wk);
                CreateCell(herdRow3, 2, item.ProcessName, wk);
                //设备的信息
                var listDevice = _processHoursEnterDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();
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
                    var entityDictionary = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == listDevice[0].DeviceStatus.ToString());
                    if (null != entityDictionary && null != entityDictionary.DisplayName)
                    {
                        CreateCell(herdRow3, 4, entityDictionary.DisplayName, wk);
                    }
                    else {
                        CreateCell(herdRow3, 4, "", wk);
                    }
             
                    
             
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
                    //需要转换的地方
                    var entityDictionary = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == listDevice[1].DeviceStatus.ToString());
                    if (null != entityDictionary && null != entityDictionary.DisplayName)
                    {
                        CreateCell(herdRow3, 8, entityDictionary.DisplayName, wk);
                    }
                    else
                    {
                        CreateCell(herdRow3, 8, "", wk);
                    }

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
                    var entityDictionary = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == listDevice[2].DeviceStatus.ToString());
                    if (null != entityDictionary && null != entityDictionary.DisplayName)
                    {
                        CreateCell(herdRow3, 12, entityDictionary.DisplayName, wk);
                    }
                    else {
                        CreateCell(herdRow3, 12, "", wk);
                    }
                  
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
                var listFrock = _processHoursEnterFrockRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();
                CreateCell(herdRow3, 16, listFrock[0].HardwareDeviceName, wk);
                CreateCell(herdRow3, 17, listFrock[0].HardwareDeviceNumber.ToString(), wk);
                CreateCell(herdRow3, 18, listFrock[0].HardwareDevicePrice.ToString(), wk);

                CreateCell(herdRow3, 19, listFrock[1].HardwareDeviceName, wk);
                CreateCell(herdRow3, 20, listFrock[1].HardwareDeviceNumber.ToString(), wk);
                CreateCell(herdRow3, 21, listFrock[1].HardwareDevicePrice.ToString(), wk);
                // 硬件总价
                CreateCell(herdRow3, 22, ((listFrock[0].HardwareDevicePrice * listFrock[0].HardwareDeviceNumber) + (listFrock[1].HardwareDevicePrice * listFrock[1].HardwareDeviceNumber)).ToString(), wk);
                //追溯软件
                if (null != item.TraceabilitySoftware)
                {
                    CreateCell(herdRow3, 23, item.TraceabilitySoftware.ToString(), wk);
                }
                else {
                    CreateCell(herdRow3, 23,string.Empty, wk);
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
                if (null != item.OpenDrawingSoftware)
                {
                    CreateCell(herdRow3, 25, item.OpenDrawingSoftware.ToString(), wk);
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
                if (null != item.HardwareTotalPrice)
                {
                    CreateCell(herdRow3, 27, item.HardwareTotalPrice.ToString(), wk);
                }
                else
                {
                    CreateCell(herdRow3, 27, string.Empty, wk);
                }

                //工装治具部分
                var listFixture = _processHoursEnterFixtureRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessHoursEnterId == item.Id).ToList();
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
              //  var queryYear = (from a in _processHoursEnterItemRepository.GetAllList(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id).Select(p => p.ModelCountYearId).Distinct()
                //                 select a).ToList();
                Solution solution = await _resourceSchemeTable.GetAsync((long)input.SolutionId);

                var queryYear = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == solution.Productld).ToList();
                List<ProcessHoursEnterSopInfoDto> processHoursEnteritems = new List<ProcessHoursEnterSopInfoDto>();
                foreach (var device in queryYear)
                {
                    ProcessHoursEnterSopInfoDto processHoursEnteritem = new ProcessHoursEnterSopInfoDto();
                    var deviceYear = _processHoursEnterItemRepository.GetAll().Where(p => p.IsDeleted == false && p.ProcessHoursEnterId == item.Id && p.ModelCountYearId == device.Id).ToList();
                    List<ProcessHoursEnteritemDto> processHoursEnteritems1 = new List<ProcessHoursEnteritemDto>();
                    foreach (var yearItem in deviceYear)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = yearItem.LaborHour;
                        processHoursEnteritemDto.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritemDto.MachineHour = yearItem.MachineHour;
                        processHoursEnteritemDto.ModelCountYearId = yearItem.ModelCountYearId;
                        processHoursEnteritems1.Add(processHoursEnteritemDto);
                    }
                    if (null == processHoursEnteritems1 || processHoursEnteritems1.Count < 1)
                    {
                        ProcessHoursEnteritemDto processHoursEnteritemDto = new ProcessHoursEnteritemDto();
                        processHoursEnteritemDto.LaborHour = 0;
                        processHoursEnteritemDto.PersonnelNumber = 0;
                        processHoursEnteritemDto.MachineHour = 0;
                        processHoursEnteritemDto.ModelCountYearId = device.Id;
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
                        CreateCell(herdRow3, yaer - 1, processHoursEnteritems[i].Issues[0].MachineHour.ToString(), wk);
                        CreateCell(herdRow3, yaer, processHoursEnteritems[i].Issues[0].PersonnelNumber.ToString(), wk);
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
        /// 创建整个界面保存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateListAsync(ProcessHoursEnterListDto input)
        {
            var query = this._processHoursEnterRepository.GetAll().Where(s => s.IsDeleted == false && s.AuditFlowId == input.AuditFlowId && s.SolutionId == input.SolutionId).ToList();
            foreach (var item in query)
            {
                _processHoursEnterDeviceRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
                _processHoursEnterFixtureRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
                _processHoursEnterFrockRepository.DeleteAsync(t => t.ProcessHoursEnterId == item.Id);
            }
            await _processHoursEnterRepository.DeleteAsync(s => s.AuditFlowId == input.AuditFlowId && s.SolutionId == input.SolutionId);
            await _processHoursEnterUphRepository.DeleteAsync(s => s.AuditFlowId == input.AuditFlowId && s.SolutionId == input.SolutionId);
            await _processHoursEnterLineRepository.DeleteAsync(s => s.AuditFlowId == input.AuditFlowId && s.SolutionId == input.SolutionId);
            foreach (var listItem in input.ListItemDtos)
            {

                ProcessHoursEnter entity = new ProcessHoursEnter();
                entity.ProcessName = listItem.ProcessName;
                entity.ProcessNumber = listItem.ProcessNumber;
                entity.AuditFlowId = input.AuditFlowId;
                entity.SolutionId = input.SolutionId;
                entity.DeviceTotalPrice = listItem.DeviceInfo.DeviceTotalCost;
                entity.HardwareTotalPrice = listItem.DevelopCostInfo.HardwareTotalPrice;
                entity.SoftwarePrice = listItem.DevelopCostInfo.SoftwarePrice;
                entity.OpenDrawingSoftware = listItem.DevelopCostInfo.OpenDrawingSoftware;
                entity.HardwareTotalPrice = listItem.DevelopCostInfo.HardwareDeviceTotalPrice;
                entity.FixtureName = listItem.ToolInfo.FixtureName;
                entity.FrockPrice = listItem.ToolInfo.FrockPrice;
                entity.FixtureNumber = listItem.ToolInfo.FixtureNumber;
                entity.FrockPrice = listItem.ToolInfo.FrockPrice;
                entity.FrockName = listItem.ToolInfo.FrockName;
                entity.FrockNumber = listItem.ToolInfo.FrockNumber;
                entity.TestLineName = listItem.ToolInfo.TestLineName;
                entity.TestLineNumber = listItem.ToolInfo.TestLineNumber;
                entity.TestLinePrice = listItem.ToolInfo.TestLinePrice;
                entity.DevelopTotalPrice = listItem.ToolInfo.DevelopTotalPrice;
                entity.TraceabilitySoftware = listItem.DevelopCostInfo.TraceabilitySoftware;
                entity.TraceabilitySoftwareCost = listItem.DevelopCostInfo.TraceabilitySoftwareCost;
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
                if (null != listItem.DeviceInfo.DeviceArr)
                {
                    foreach (var DeviceInfoItem in listItem.DeviceInfo.DeviceArr)
                    {
                        ProcessHoursEnterDevice processHoursEnterDevice = new ProcessHoursEnterDevice();
                        processHoursEnterDevice.ProcessHoursEnterId = foundationDevice;
                        processHoursEnterDevice.DeviceNumber = DeviceInfoItem.DeviceNumber;
                        processHoursEnterDevice.DevicePrice = DeviceInfoItem.DevicePrice;
                        processHoursEnterDevice.DeviceStatus = DeviceInfoItem.DeviceStatus;
                        processHoursEnterDevice.DeviceName = DeviceInfoItem.DeviceName;
                        _processHoursEnterDeviceRepository.InsertAsync(processHoursEnterDevice);
                    }
                }
                //追溯部分(硬件及软件开发费用)
                if (null != listItem.DevelopCostInfo.HardwareInfo)
                {
                    foreach (var hardwareInfoItem in listItem.DevelopCostInfo.HardwareInfo)
                    {
                        ProcessHoursEnterFrock processHoursEnterFrock = new ProcessHoursEnterFrock();
                        processHoursEnterFrock.ProcessHoursEnterId = foundationDevice;
                        processHoursEnterFrock.HardwareDevicePrice = hardwareInfoItem.HardwareDevicePrice;
                        processHoursEnterFrock.HardwareDeviceNumber = hardwareInfoItem.HardwareDeviceNumber;
                        processHoursEnterFrock.HardwareDeviceName = hardwareInfoItem.HardwareDeviceName;
                        _processHoursEnterFrockRepository.InsertAsync(processHoursEnterFrock);
                    }
                }

                //工装治具部分
                if (null != listItem.ToolInfo.ZhiJuArr)
                {
                    foreach (var zoolInfo in listItem.ToolInfo.ZhiJuArr)
                    {
                        ProcessHoursEnterFixture processHoursEnterFixture = new ProcessHoursEnterFixture();
                        processHoursEnterFixture.ProcessHoursEnterId = foundationDevice;
                        processHoursEnterFixture.FixturePrice = zoolInfo.FixturePrice;
                        processHoursEnterFixture.FixtureNumber = zoolInfo.FixtureNumber;
                        processHoursEnterFixture.FixtureName = zoolInfo.FixtureName;
                        _processHoursEnterFixtureRepository.InsertAsync(processHoursEnterFixture);
                    }
                }
                Solution solution = await _resourceSchemeTable.GetAsync(input.SolutionId);

                var queryYear = this._modelCountYearRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == solution.Productld).ToList();



                //年
                if (null != listItem.SopInfo)
                {
                    foreach (var year in listItem.SopInfo)
                    {

                            ProcessHoursEnteritem processHoursEnteritem = new ProcessHoursEnteritem();

                            processHoursEnteritem.ProcessHoursEnterId = foundationDevice;
                            processHoursEnteritem.LaborHour = year.Issues[0].LaborHour;
                            processHoursEnteritem.PersonnelNumber = year.Issues[0].PersonnelNumber;
                            processHoursEnteritem.MachineHour = year.Issues[0].MachineHour;
                            processHoursEnteritem.ModelCountYearId = year.Issues[0].ModelCountYearId;
                            if (queryYear.Count > 0 && null != queryYear[listItem.SopInfo.IndexOf(year)] && processHoursEnteritem.ModelCountYearId == 0)
                            {
                                processHoursEnteritem.ModelCountYearId = queryYear[listItem.SopInfo.IndexOf(year)].Id;

                            }
                            ModelCountYear modelCountYear = await _modelCountYearRepository.GetAsync(processHoursEnteritem.ModelCountYearId);
                            processHoursEnteritem.Year = modelCountYear.Year.ToString();

                            _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }

            }
            //uph
            if (null != input.ProcessHoursEnterUphList)
            {
                foreach (var item in input.ProcessHoursEnterUphList)
                {
                    ProcessHoursEnterUph processHoursEnterUph = new ProcessHoursEnterUph();
                    processHoursEnterUph.ModelCountYearId = item.ModelCountYearId;
                    processHoursEnterUph.Uph = "cobuph";
                    processHoursEnterUph.Value = item.Cobuph;
                    processHoursEnterUph.SolutionId = input.SolutionId;
                    processHoursEnterUph.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);
                    ProcessHoursEnterUph processHoursEnterUph1 = new ProcessHoursEnterUph();
                    processHoursEnterUph1.ModelCountYearId = item.ModelCountYearId;
                    processHoursEnterUph1.Uph = "zcuph";
                    processHoursEnterUph1.Value = item.Zcuph;
                    processHoursEnterUph1.SolutionId = input.SolutionId;
                    processHoursEnterUph1.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph1);
                    ProcessHoursEnterUph processHoursEnterUph2 = new ProcessHoursEnterUph();
                    processHoursEnterUph2.ModelCountYearId = item.ModelCountYearId;
                    processHoursEnterUph2.Uph = "smtuph";
                    processHoursEnterUph2.SolutionId = input.SolutionId;
                    processHoursEnterUph2.AuditFlowId = input.AuditFlowId;
                    processHoursEnterUph2.Value = item.Smtuph;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph2);

                }
            }

            //线体数量、共线分摊率
            if (null != input.ProcessHoursEnterLineList)
            {
                foreach (var item in input.ProcessHoursEnterLineList)
                {
                    ProcessHoursEnterLine processHoursEnterLine = new ProcessHoursEnterLine();
                    processHoursEnterLine.ModelCountYearId = item.ModelCountYearId;
                    processHoursEnterLine.Uph = "gxftl";
                    processHoursEnterLine.Value = item.Gxftl;
                    processHoursEnterLine.SolutionId = input.SolutionId;
                    processHoursEnterLine.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterLineRepository.InsertAsync(processHoursEnterLine);
                    ProcessHoursEnterLine processHoursEnterUph1 = new ProcessHoursEnterLine();
                    processHoursEnterUph1.ModelCountYearId = item.ModelCountYearId;
                    processHoursEnterUph1.Uph = "xtsl";
                    processHoursEnterUph1.Value = item.Xtsl;
                    processHoursEnterUph1.SolutionId = input.SolutionId;
                    processHoursEnterUph1.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterLineRepository.InsertAsync(processHoursEnterUph1);

                }
            }
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == (long)input.AuditFlowId && t.SolutionId == (long)input.SolutionId && t.EnumSole == NreIsSubmitDto.ProcessHoursEnter.ToString());

            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = (long)input.AuditFlowId, SolutionId = (long)input.SolutionId, EnumSole = NreIsSubmitDto.ProcessHoursEnter.ToString() });



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



        /// <summary>
        /// 流程退出，对应的数据进行删除
        /// </summary>
        /// <param name="AuditFlowId">流程id</param>
        public virtual async Task DeleteAuditFlowIdAsync(long AuditFlowId)
        {
            await _resourceNreIsSubmit.DeleteAsync(t => t.AuditFlowId == AuditFlowId && t.EnumSole == NreIsSubmitDto.ProcessHoursEnter.ToString());


        }

        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumItem>> GetDeviceStatus()
        {

            var filter = _financeDictionaryDetailRepository.GetAll()
            .Where(p => p.FinanceDictionaryId.Equals("Sbzt"));
            List<EnumItem> enumItems= new List<EnumItem>();
            foreach (var item in filter)
            {
                EnumItem enumItem = new EnumItem();
                enumItem.Code = item.Id;
                enumItem.Value = item.DisplayName;
                enumItems.Add(enumItem);

            }
            return enumItems;
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
        /// 根据流程和方案获取硬件总价 、开发费追溯、开发费开图
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<ProcessHoursEnterTotalDto> GetProcessHoursEnterTotal(GetProcessHoursEntersInput input)
        {
            ProcessHoursEnterTotalDto process = new ProcessHoursEnterTotalDto();
         
        // 设置查询条件
            List<ProcessHoursEnter> query = this._processHoursEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == input.SolutionId ).ToList();
            decimal HardwareTotalPrice = 0;
            decimal SoftwarePrice = 0;
            decimal TraceabilitySoftware = 0;
            foreach ( var item in query )
            {
                decimal hardwareTotalPriceItem = 0;
                var frockList=  _processHoursEnterFrockRepository.GetAll().Where(t => t.ProcessHoursEnterId == item.Id).ToList();
                foreach (var itemHardwareTotal in frockList)
                {
                    if (null == itemHardwareTotal.HardwareDeviceNumber || null == itemHardwareTotal.HardwareDevicePrice)
                    {
                        hardwareTotalPriceItem += 0;
                    }
                    else {
                        hardwareTotalPriceItem += (decimal)(itemHardwareTotal.HardwareDeviceNumber * itemHardwareTotal.HardwareDevicePrice);
                    }
                }
                HardwareTotalPrice += hardwareTotalPriceItem;
                SoftwarePrice += item.SoftwarePrice;
                TraceabilitySoftware += item.TraceabilitySoftwareCost;

            }
            process.HardwareTotalPrice = HardwareTotalPrice;
            process.SoftwarePrice = SoftwarePrice;
            process.TraceabilitySoftware = TraceabilitySoftware;
            return process;
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
        /// 处理字段为null判断
        /// </summary>
        /// <param name="sheet"></param>
        public string AndleNull(T value)
        {
           
            if (null != value)
            {
                return value.ToString();
            }
            else
            {
                return string.Empty;


            }
        }
    }
}
