using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationStandardTechnologyAppService : ApplicationService
    {
        private readonly IRepository<FoundationStandardTechnology, long> _foundationStandardTechnologyRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationReliableProcessHours, long> _foundationFoundationReliableProcessHoursRepository;
        private readonly IRepository<FoundationTechnologyDevice, long> _foundationFoundationTechnologyDeviceHoursRepository;
        /// <summary>
        /// .ctorFRProcessHours
        /// </summary>
        /// <param name="foundationStandardTechnologyRepository"></param>
        public FoundationStandardTechnologyAppService(
                IRepository<FoundationReliableProcessHours, long> foundationFoundationReliableProcessHoursRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FoundationTechnologyDevice, long> foundationFoundationTechnologyDeviceHoursRepository,
            IRepository<FoundationStandardTechnology, long> foundationStandardTechnologyRepository)
        {
            _foundationStandardTechnologyRepository = foundationStandardTechnologyRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationReliableProcessHoursRepository = foundationFoundationReliableProcessHoursRepository;
            _foundationFoundationTechnologyDeviceHoursRepository = foundationFoundationTechnologyDeviceHoursRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> GetAsyncById(long id)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationStandardTechnologyDto>> GetListAsync(GetFoundationStandardTechnologysInput input)
        {
            // 设置查询条件
            var query = this._foundationStandardTechnologyRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationStandardTechnology>, List<FoundationStandardTechnologyDto>>(list, new List<FoundationStandardTechnologyDto>());
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
                FoundationReliableProcessHours entity = await _foundationFoundationReliableProcessHoursRepository.GetAsync(item.Id).ConfigureAwait(false);
                //数据转换
                FoundationReliableProcessHoursDto dtosItem = ObjectMapper.Map<FoundationReliableProcessHours, FoundationReliableProcessHoursDto>(entity, new FoundationReliableProcessHoursDto());
                if (entity != null)
                {

                }
                item.FoundationReliableProcessHoursDto = dtosItem;
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

            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> CreateAsync(FoundationStandardTechnologyDto input)
        {
            var entity = ObjectMapper.Map<FoundationStandardTechnologyDto, FoundationStandardTechnology>(input,new FoundationStandardTechnology());
            entity = await _foundationStandardTechnologyRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationStandardTechnologyDto> UpdateAsync(long id, FoundationStandardTechnologyDto input)
        {
            FoundationStandardTechnology entity = await _foundationStandardTechnologyRepository.GetAsync(id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationStandardTechnologyRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationStandardTechnology, FoundationStandardTechnologyDto>(entity,new FoundationStandardTechnologyDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationStandardTechnologyRepository.DeleteAsync(s => s.Id == id);
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
                Stream stream = file.OpenReadStream();

                //根据文件流创建excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(stream);
                stream.Close();

                //尝试获取第一个sheet
                var sheet = workbook.GetSheetAt(0);
                //判断是否获取到 sheet
                var tempmbPath = @"D:\aa.xlsx";

                //var memoryStream = new MemoryStream();
                //MiniExcel.SaveAsByTemplate(path, tempmbPath, list, configuration: config);
                var rows = MiniExcel.Query(tempmbPath).ToList();
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
                List<FoundationReliableProcessHoursResponseDto> foundationReliableProcessHoursResponseDtoList =   new List<FoundationReliableProcessHoursResponseDto>();

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
                        FoundationTechnologyFixtureDto foundationTechnologyFixtureDto =  new FoundationTechnologyFixtureDto(); 
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
                    List <Dictionary<string, object>> years = new List<Dictionary<string, object>>();
                    int yearNum = yearCols / 3;
                    for (int j = 0; j < yearNum; j++)
                    {
                        FoundationWorkingHourItemDto  foundationWorkingHourItem =   new FoundationWorkingHourItemDto();
                        string yearstr = yearStrs[j];
                        Dictionary<string, object> yearItem = new Dictionary<string, object>();
                        int fromStartIndex = j * 3 + frocNumIndex;
                        var val0 = row[keys[fromStartIndex]];
                        var val1 = row[keys[fromStartIndex + 1]];
                        var val2 = row[keys[fromStartIndex + 2]];
                        foundationWorkingHourItem.LaborHour = val0.ToString();
                        foundationWorkingHourItem.MachineHour = val1.ToString(); ;
                        foundationWorkingHourItem.NumberPersonnel = val2.ToString();
                        foundationWorkingHourItem.Year= yearstr;
                        foundationWorkingHourItemDtos.Add(foundationWorkingHourItem);
                    }
                    foundationReliableProcessHoursResponse.sopInfo = foundationWorkingHourItemDtos;

                    foundationReliableProcessHoursResponseDtoList.Add(foundationReliableProcessHoursResponse);
                }
                return foundationReliableProcessHoursResponseDtoList;



            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
        }

    }
}
