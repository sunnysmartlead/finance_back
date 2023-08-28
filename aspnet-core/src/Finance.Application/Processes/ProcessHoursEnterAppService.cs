﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.Ext;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using NPOI.POIFS.FileSystem;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using test;

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
        private readonly DataInputAppService _dataInputAppService;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processHoursEnterRepository"></param>
        public ProcessHoursEnterAppService(
               IRepository<Solution, long> resourceSchemeTable, IRepository<ProcessHoursEnter, long> processHoursEnterRepository, IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository, IRepository<ProcessHoursEnterFixture, long> processHoursEnterFixtureRepository, IRepository<ProcessHoursEnterFrock, long> processHoursEnterFrockRepository, IRepository<ProcessHoursEnteritem, long> processHoursEnterItemRepository, IRepository<ProcessHoursEnterLine, long> processHoursEnterLineRepository, IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository, DataInputAppService dataInputAppService)
        {
            _processHoursEnterRepository = processHoursEnterRepository;
            _processHoursEnterDeviceRepository = processHoursEnterDeviceRepository;
            _processHoursEnterFixtureRepository = processHoursEnterFixtureRepository;
            _processHoursEnterFrockRepository = processHoursEnterFrockRepository;
            _processHoursEnterItemRepository = processHoursEnterItemRepository;
            _processHoursEnterLineRepository = processHoursEnterLineRepository;
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
            _dataInputAppService = dataInputAppService;
            _resourceSchemeTable = resourceSchemeTable;
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
                        processHoursEnteritem.Issues.Add(processHoursEnteritemDto);
                    }
                    processHoursEnteritems.Add(processHoursEnteritem);
                    }

                processHoursEnter.SopInfo = processHoursEnteritems;
                //Uph查询


                //线体数量、共线分摊率


                processHoursEnterDtoList.Add(processHoursEnter);


            }
            // 数据返回
            return processHoursEnterDtoList;
        }



        /// <summary>
        /// 查看uph和线体数量、共线分摊率接口
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<ProcessHoursEnterDto> GetListUphOrLineAsync(GetProcessHoursEntersInput input)
        {
            ProcessHoursEnterDto processHoursEnterDto= new ProcessHoursEnterDto();

            

            
            Solution entity = await _resourceSchemeTable.GetAsync((long)input.SolutionId);





            List<GradientModelYearListDto> data = await _dataInputAppService.GetGradientModelYearByProductId((long)entity.Productld);
            List<ProcessHoursEnterUphListDto> processHoursEnterUphListDtos = new List<ProcessHoursEnterUphListDto>();
            
            foreach (GradientModelYearListDto row in data) {

                ProcessHoursEnterUphListDto processHoursEnterUphListDto = new ProcessHoursEnterUphListDto();
                var list = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "zcuph" && t.Year == row.Year.ToString()).ToList();
                if (null != list && list.Count > 0)
                {
                    processHoursEnterUphListDto.Smtuph = list[0].Value;
                }
                else {
                    processHoursEnterUphListDto.Smtuph = 0;
                }
                var ZcuphList = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "zcuph" && t.Year == row.Year.ToString()).ToList();
                if (null != ZcuphList && ZcuphList.Count > 0)
                {
                    processHoursEnterUphListDto.Zcuph = ZcuphList[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.Zcuph = 0;
                }

                var CobuphList = this._processHoursEnterUphRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "cobuph" && t.Year == row.Year.ToString()).ToList();
                if (null != CobuphList && CobuphList.Count > 0)
                {
                    processHoursEnterUphListDto.Cobuph = CobuphList[0].Value;
                }
                else
                {
                    processHoursEnterUphListDto.Cobuph = 0;
                }
                processHoursEnterUphListDto.Year = row.Year.ToString();
                processHoursEnterUphListDtos.Add(processHoursEnterUphListDto);

            }
            List<ProcessHoursEnterLineDtoList> processHoursEnterLineDtos = new List<ProcessHoursEnterLineDtoList>();
            foreach (GradientModelYearListDto row in data)
            {

                ProcessHoursEnterLineDtoList processHoursEnterLine = new ProcessHoursEnterLineDtoList();

                var XtslList = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "xtsl" && t.Year == row.Year.ToString()).ToList();
                if (null != XtslList && XtslList.Count > 0)
                {
                    processHoursEnterLine.Xtsl = XtslList[0].Value;
                }
                else
                {
                    processHoursEnterLine.Xtsl = 0;
                }
                var GxftlList = this._processHoursEnterLineRepository.GetAll().Where(t => t.IsDeleted == false && t.SolutionId == input.SolutionId && t.AuditFlowId == input.AuditFlowId && t.Uph == "gxftl" && t.Year == row.Year.ToString() ).ToList();
                if (null != GxftlList && GxftlList.Count > 0)
                {
                    processHoursEnterLine.Gxftl = GxftlList[0].Value;
                }
                else
                {
                    processHoursEnterLine.Gxftl = 0;
                }
                processHoursEnterLine.Year = row.Year.ToString();
                processHoursEnterLineDtos.Add(processHoursEnterLine);

            }
            processHoursEnterDto.processHoursEnterLineList= processHoursEnterLineDtos;
            processHoursEnterDto.processHoursEnterUphList= processHoursEnterUphListDtos;
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
        public virtual async Task<String> CreateSubmitAsync(GetLogisticscostsInput input)
        {
            //已经录入数量
            var count = (from a in _processHoursEnterRepository.GetAllList(p =>
         p.IsDeleted == false && p.AuditFlowId == input.AuditFlowId
         ).Select(p => p.SolutionId).Distinct()
                         select a).ToList();
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            int quantity = result.Count - count.Count;
            if (quantity > 0)
            {
                return "还有" + quantity + "个方案没有提交，请先提交";
            }
            else
            {

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
                        processHoursEnteritem.ProcessHoursEnterId = foundationDevice;
                        processHoursEnteritem.LaborHour= yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber= yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour= yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }

            //uph
            if (null != input.processHoursEnterUphList) {
                foreach (var item in input.processHoursEnterUphList)
                {
                    ProcessHoursEnterUph processHoursEnterUph= new ProcessHoursEnterUph();
                    processHoursEnterUph.Year= item.Year;
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
                        processHoursEnteritem.ProcessHoursEnterId = entity.Id;
                        processHoursEnteritem.LaborHour = yearItem.LaborHour;
                        processHoursEnteritem.PersonnelNumber = yearItem.PersonnelNumber;
                        processHoursEnteritem.MachineHour = yearItem.MachineHour;
                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                    }
                }
            }
        }



        /// <summary>
        /// 工时工序导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<ProcessHoursEnterDto>> UploadProcessHoursEnter(IFormFile file)
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
                    List<ProcessHoursEnterDto> ProcessHoursEnterDList = new List<ProcessHoursEnterDto>();

                    // 取值
                    var keys = cols.Keys.ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        ProcessHoursEnterDto processHoursEnterDto = new ProcessHoursEnterDto();
                        IDictionary<String, Object> row = rows[i];
                        Dictionary<string, object> rowItem = new Dictionary<string, object>();
                        //总数居
                        processHoursEnterDto.ProcessName = (row[keys[1]]).ToString();
                        processHoursEnterDto.ProcessNumber = (row[keys[0]]).ToString();

                        //获取设备
                        Object deviceInfo = new Object();
                        int deviceNum = (deviceCols - 1) / 4;
                        ProcessHoursEnterResponseDeviceDto devices = new ProcessHoursEnterResponseDeviceDto();
                        List<ProcessHoursEnterDeviceDto> processHoursEnterDeviceDtoList = new List<ProcessHoursEnterDeviceDto>();
                        for (int j = 0; j < deviceNum; j++)
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

                            foundationTechnologyDevice.DevicePrice = decimal.Parse(val3.ToString());
                            foundationTechnologyDevice.DeviceName = val0.ToString();
                            foundationTechnologyDevice.DeviceNumber = decimal.Parse(val2.ToString());
                            foundationTechnologyDevice.DeviceStatus = val1.ToString();
                            processHoursEnterDeviceDtoList.Add(foundationTechnologyDevice);
                        }
                        // 设备总价
                        int ddevNumIndex = startCols + deviceCols;
                        devices.DeviceArr = processHoursEnterDeviceDtoList;
                        devices.DeviceTotalCost = decimal.Parse(row[keys[ddevNumIndex - 1]].ToString());
                        processHoursEnterDto.DeviceInfo = devices;



                        // 解析追溯信息
                        ProcessHoursEnterDevelopCostInfoDeviceDto foundationReliableProcessHoursdevelopCostInfoResponseDto = new ProcessHoursEnterDevelopCostInfoDeviceDto();
                        List<ProcessHoursEnterFrockDto> foundationTechnologyDeviceList = new List<ProcessHoursEnterFrockDto>();

                        // 有6列是总结列，不是子表，需要将数量剔除
                        fromCols = fromCols - 4;
                        int fromNum = fromCols / 3;
                        for (int j = 0; j < fromNum; j++)
                        {
                            Dictionary<string, object> fromItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + startCols + deviceCols;
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
                        // 设备总价
                        int fromNumIndex = ddevNumIndex + fromCols;


                        // 硬件总价
                        rowItem.Add(keys[fromNumIndex], row[keys[fromNumIndex]].ToString());
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareTotalPrice = decimal.Parse(row[keys[fromNumIndex]].ToString());
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.OpenDrawingSoftware = (row[keys[fromNumIndex +1]].ToString());
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.SoftwarePrice = decimal.Parse(row[keys[fromNumIndex + 2]].ToString());
                        foundationReliableProcessHoursdevelopCostInfoResponseDto.HardwareDeviceTotalPrice = decimal.Parse(row[keys[fromNumIndex + 3]].ToString());

                        processHoursEnterDto.DevelopCostInfo = foundationReliableProcessHoursdevelopCostInfoResponseDto;




                        // 解析工装治具部分
                        ProcessHoursEnterToolInfoDto foundationReliableProcessHoursFixtureResponseDto = new ProcessHoursEnterToolInfoDto();
                        List<ProcessHoursEnterFixtureDto> foundationTechnologyFixtures = new List<ProcessHoursEnterFixtureDto>();
                        int frocNum = (frockCols - 10) / 3;
                        for (int j = 0; j < frocNum; j++)
                        {
                            ProcessHoursEnterFixtureDto foundationTechnologyFixtureDto = new ProcessHoursEnterFixtureDto();
                            int fromStartIndex = j * 3 + fromNumIndex + 4;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            foundationTechnologyFixtureDto.FixtureName = val0.ToString();
                            foundationTechnologyFixtureDto.FixtureNumber = decimal.Parse(val1.ToString());
                            foundationTechnologyFixtureDto.FixturePrice = decimal.Parse(val2.ToString());
                            foundationTechnologyFixtures.Add(foundationTechnologyFixtureDto);
                        }
                        foundationReliableProcessHoursFixtureResponseDto.ZhiJuArr = foundationTechnologyFixtures;

                        // 设备总价
                        int frocNumIndex = fromNumIndex + 4 + frockCols;
                        // 工装治具检具总价
                        rowItem.Add(keys[frocNumIndex], row[keys[frocNumIndex - 1]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.DevelopTotalPrice = (row[keys[frocNumIndex - 1]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLinePrice = decimal.Parse(row[keys[frocNumIndex - 2]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLineNumber = decimal.Parse(row[keys[frocNumIndex - 3]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.TestLineName = (row[keys[frocNumIndex - 4]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockPrice = decimal.Parse(row[keys[frocNumIndex - 5]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockNumber = decimal.Parse(row[keys[frocNumIndex - 6]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FrockName = (row[keys[frocNumIndex - 7]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixturePrice = decimal.Parse(row[keys[frocNumIndex - 8]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixtureNumber = decimal.Parse(row[keys[frocNumIndex - 9]].ToString());
                        foundationReliableProcessHoursFixtureResponseDto.FixtureName = (row[keys[frocNumIndex - 10]].ToString());
                        processHoursEnterDto.ToolInfo = foundationReliableProcessHoursFixtureResponseDto;

                        // 解析年度部分
                        List<ProcessHoursEnteritemDto> foundationWorkingHourItemDtos = new List<ProcessHoursEnteritemDto>();
                        List<Dictionary<string, object>> years = new List<Dictionary<string, object>>();
                        int yearNum = yearCols / 3;
                        for (int j = 0; j < yearNum; j++)
                        {
                            ProcessHoursEnteritemDto foundationWorkingHourItem = new ProcessHoursEnteritemDto();
                            string yearstr = yearStrs[j];
                            Dictionary<string, object> yearItem = new Dictionary<string, object>();
                            int fromStartIndex = j * 3 + frocNumIndex;
                            var val0 = row[keys[fromStartIndex]];
                            var val1 = row[keys[fromStartIndex + 1]];
                            var val2 = row[keys[fromStartIndex + 2]];
                            foundationWorkingHourItem.LaborHour = decimal.Parse(val0.ToString());
                            foundationWorkingHourItem.MachineHour = decimal.Parse(val1.ToString());
                            foundationWorkingHourItem.PersonnelNumber = decimal.Parse(val2.ToString());
                            foundationWorkingHourItem.Year = yearstr;
                            foundationWorkingHourItemDtos.Add(foundationWorkingHourItem);
                        }
                        processHoursEnterDto.SopInfoAll = foundationWorkingHourItemDtos;
                        ProcessHoursEnterDList.Add(processHoursEnterDto);

                    }

           /*         if (null != ProcessHoursEnterDList)
                    {
                        foreach (var item in ProcessHoursEnterDList)
                        {
                            ProcessHoursEnter entity = new ProcessHoursEnter();
                            entity.ProcessName = item.ProcessName;
                            entity.ProcessNumber = item.ProcessNumber;
                            entity.SolutionId = 100;
                            entity.AuditFlowId = 99;
                            entity.DeviceTotalPrice = item.DeviceInfo.DeviceTotalCost;
                            entity.HardwareTotalPrice = item.DevelopCostInfo.HardwareTotalPrice;
                            entity.SoftwarePrice = item.DevelopCostInfo.SoftwarePrice;
                            entity.OpenDrawingSoftware = item.DevelopCostInfo.OpenDrawingSoftware;
                            entity.HardwareTotalPrice = item.DevelopCostInfo.HardwareDeviceTotalPrice;
                            entity.FixtureName = item.ToolInfo.FixtureName;
                            entity.FrockPrice = item.ToolInfo.FrockPrice;
                            entity.FixtureNumber = item.ToolInfo.FixtureNumber;
                            entity.FrockPrice = item.ToolInfo.FrockPrice;
                            entity.FrockName = item.ToolInfo.FrockName;
                            entity.FrockNumber = item.ToolInfo.FrockNumber;
                            entity.TestLineName = item.ToolInfo.TestLineName;
                            entity.TestLineNumber = item.ToolInfo.TestLineNumber;
                            entity.TestLinePrice = item.ToolInfo.TestLinePrice;
                            entity.DevelopTotalPrice = item.ToolInfo.DevelopTotalPrice;
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
                            if (null != item.DeviceInfo.DeviceArr)
                            {
                                foreach (var DeviceInfoItem in item.DeviceInfo.DeviceArr)
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
                            if (null != item.DevelopCostInfo.HardwareInfo)
                            {
                                foreach (var hardwareInfoItem in item.DevelopCostInfo.HardwareInfo)
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
                            if (null != item.ToolInfo.ZhiJuArr)
                            {
                                foreach (var zoolInfo in item.ToolInfo.ZhiJuArr)
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
                            if (null != item.SopInfoAll)
                            {
                               
                                    foreach (var yearItem in item.SopInfoAll)
                                    {
                                        ProcessHoursEnteritem processHoursEnteritem = new ProcessHoursEnteritem();
                                        processHoursEnteritem.Year = yearItem.Year;
                                        processHoursEnteritem.ProcessHoursEnterId = foundationDevice;
                                        processHoursEnteritem.LaborHour = yearItem.LaborHour;
                                        processHoursEnteritem.PersonnelNumber = yearItem.PersonnelNumber;
                                        processHoursEnteritem.MachineHour = yearItem.MachineHour;
                                        _processHoursEnterItemRepository.InsertAsync(processHoursEnteritem);
                                    }
                                
                            }

                        }
                    }*/
                    
                    return ProcessHoursEnterDList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
        }


        /// <summary>
        /// 创建整个界面保存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateListAsync(ProcessHoursEnterListDto input)
        {

            foreach (var listItem in input.ListItemDtos)
            {
                await _processHoursEnterRepository.DeleteAsync(s => s.AuditFlowId == listItem.AuditFlowId && s.SolutionId == listItem.SolutionId);
                await _processHoursEnterUphRepository.DeleteAsync(s => s.AuditFlowId == listItem.AuditFlowId && s.SolutionId == listItem.SolutionId);
                await _processHoursEnterLineRepository.DeleteAsync(s => s.AuditFlowId == listItem.AuditFlowId && s.SolutionId == listItem.SolutionId);
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

                //年
                if (null != listItem.SopInfo)
                {
                    foreach (var year in listItem.SopInfo)
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

               }
            //uph
            if (null != input.ProcessHoursEnterUphList)
            {
                foreach (var item in input.ProcessHoursEnterUphList)
                {
                    ProcessHoursEnterUph processHoursEnterUph = new ProcessHoursEnterUph();
                    processHoursEnterUph.Year = item.Year;
                    processHoursEnterUph.Uph = "cobuph";
                    processHoursEnterUph.Value = item.Cobuph;
                    processHoursEnterUph.SolutionId = input.SolutionId;
                    processHoursEnterUph.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph);
                    ProcessHoursEnterUph processHoursEnterUph1 = new ProcessHoursEnterUph();
                    processHoursEnterUph1.Year = item.Year;
                    processHoursEnterUph1.Uph = "zcuph";
                    processHoursEnterUph1.Value = item.Zcuph;
                    processHoursEnterUph1.SolutionId = input.SolutionId;
                    processHoursEnterUph1.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterUphRepository.InsertAsync(processHoursEnterUph1);
                    ProcessHoursEnterUph processHoursEnterUph2 = new ProcessHoursEnterUph();
                    processHoursEnterUph2.Year = item.Year;
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
                    processHoursEnterLine.Year = item.Year;
                    processHoursEnterLine.Uph = "gxftl";
                    processHoursEnterLine.Value = item.Gxftl;
                    processHoursEnterLine.SolutionId = input.SolutionId;
                    processHoursEnterLine.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterLineRepository.InsertAsync(processHoursEnterLine);
                    ProcessHoursEnterLine processHoursEnterUph1 = new ProcessHoursEnterLine();
                    processHoursEnterUph1.Year = item.Year;
                    processHoursEnterUph1.Uph = "xtsl";
                    processHoursEnterUph1.Value = item.Xtsl;
                    processHoursEnterUph1.SolutionId = input.SolutionId;
                    processHoursEnterUph1.AuditFlowId = input.AuditFlowId;
                    await _processHoursEnterLineRepository.InsertAsync(processHoursEnterUph1);

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
