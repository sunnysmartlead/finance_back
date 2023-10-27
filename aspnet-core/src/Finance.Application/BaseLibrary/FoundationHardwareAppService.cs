using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.Authorization.Users;
using Interface.Expends;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using test;
using xyxandwxx.Expends;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationHardwareAppService : ApplicationService
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.HardwareAndSoftware;
        private readonly IRepository<FoundationHardware, long> _foundationHardwareRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationHardwareItem, long> _foundationFoundationHardwareItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationHardwareRepository"></param>
        public FoundationHardwareAppService(
             IRepository<FoundationHardwareItem, long> foundationFoundationHardwareItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FoundationHardware, long> foundationHardwareRepository)
        {
            _foundationHardwareRepository = foundationHardwareRepository;
            _foundationFoundationHardwareItemRepository = foundationFoundationHardwareItemRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationHardwareItemRepository = foundationFoundationHardwareItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> GetAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationHardwareDto>> GetListAsync(GetFoundationHardwaresInput input)
        {
            // 设置查询条件
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
            // 数据返回
            return new PagedResultDto<FoundationHardwareDto>(totalCount, dtos);

        }
        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationHardwareDto>> GetListAllAsync(GetFoundationHardwaresInput input)
        {
            if (null != input.DeviceName && !input.DeviceName.Equals("") && (null == input.SoftwareName || input.SoftwareName.Equals("")))
            {
                List<FoundationHardwareDto> foundationHardwares =  new List<FoundationHardwareDto>();
                var FoundationHardwareDtoId = await (from u in _foundationFoundationHardwareItemRepository.GetAll()
                                   join ur in _foundationHardwareRepository.GetAll() on u.FoundationHardwareId equals ur.Id
                                   where  u.HardwareName.Contains(input.DeviceName)
                                   select new
                                   {
                                      Id = ur.Id
                                   }).Distinct().ToListAsync();

                foreach (var item in FoundationHardwareDtoId)
                {
                    FoundationHardwareDto foundationHardwareDto = new FoundationHardwareDto();
                    FoundationHardware entity = await _foundationHardwareRepository.GetAsync(item.Id);
                    var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == entity.Id).ToList();

                    //数据转换
                    var dtosItem = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(FoundationDeviceItemlist, new List<FoundationHardwareItemDto>());
                 
                    foundationHardwareDto =  ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto());
                    foundationHardwareDto.ListHardware = dtosItem;
                    foundationHardwares.Add(foundationHardwareDto);


                }
                    return foundationHardwares;
            }
            if (null != input.DeviceName && !input.DeviceName.Equals("") && null != input.SoftwareName && !input.SoftwareName.Equals(""))
            {
                List<FoundationHardwareDto> foundationHardwares = new List<FoundationHardwareDto>();
                var FoundationHardwareDtoId = await (from u in _foundationFoundationHardwareItemRepository.GetAll()
                                                     join ur in _foundationHardwareRepository.GetAll() on u.FoundationHardwareId equals ur.Id
                                                     where u.HardwareName.Contains(input.DeviceName) &&  ur.SoftwareName.Contains(input.SoftwareName)
                                                     select new
                                                     {
                                                         Id = ur.Id
                                                     }).Distinct().ToListAsync();

                foreach (var item in FoundationHardwareDtoId)
                {
                    FoundationHardwareDto foundationHardwareDto = new FoundationHardwareDto();
                    FoundationHardware entity = await _foundationHardwareRepository.GetAsync(item.Id);
                    var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == entity.Id).ToList();

                    //数据转换
                    var dtosItem = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(FoundationDeviceItemlist, new List<FoundationHardwareItemDto>());
                    var user = this._userRepository.GetAll().Where(u => u.Id == entity.LastModifierUserId).ToList().FirstOrDefault();
                    if (user != null)
                    {
                        foundationHardwareDto.LastModifierUserName = user.Name;
                    }
                    foundationHardwareDto = ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto());
                    foundationHardwareDto.ListHardware = dtosItem;
                    foundationHardwares.Add(foundationHardwareDto);


                }
                return foundationHardwares;
            }
            else {

                // 设置查询条件
                var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
                if (!string.IsNullOrEmpty(input.SoftwareName))
                {
                    query = query.Where(t => t.SoftwareName.Contains(input.SoftwareName));
                }
                if (!string.IsNullOrEmpty(input.TraceabilitySoftware))
                {
                    query = query.Where(t => t.TraceabilitySoftware.Contains(input.TraceabilitySoftware));
                }

                // 查询数据
                var list = query.ToList();
                //数据转换
                var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
                foreach (var item in dtos)
                {
                    var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                    var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == item.Id).ToList();

                    //数据转换
                    var dtosItem = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(FoundationDeviceItemlist, new List<FoundationHardwareItemDto>());
                    item.ListHardware = dtosItem;
                    if (user != null)
                    {
                        item.LastModifierUserName = user.Name;
                    }
                }
                // 数据返回
                return dtos;
            }
       
        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> GetEditorAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> CreateAsync(FoundationHardwareDto input)
        {
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == input.ProcessNumber).ToList();
            if (query.Count > 0)
            {
                throw new FriendlyException("工序编号重复！");
            }
            var entity = ObjectMapper.Map<FoundationHardwareDto, FoundationHardware>(input, new FoundationHardware());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationHardwareRepository.InsertAsync(entity);
            var foundationDevice = _foundationHardwareRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto());
            if (input.ListHardware != null)
            {
                await _foundationFoundationHardwareItemRepository.DeleteAsync(t => t.FoundationHardwareId == result.Id);
                foreach (var deviceItem in input.ListHardware)
                {
                    var entityItem = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(deviceItem, new FoundationHardwareItem());

                    FoundationHardwareItem foundationHardwareItem = new FoundationHardwareItem();
                    foundationHardwareItem.FoundationHardwareId = foundationDevice;
                    foundationHardwareItem.CreationTime = DateTime.Now;
                    foundationHardwareItem.HardwareName = entityItem.HardwareName;
                    foundationHardwareItem.HardwarePrice = entityItem.HardwarePrice;
                    foundationHardwareItem.HardwareBusiness = entityItem.HardwareBusiness;
                    foundationHardwareItem.HardwareState = entityItem.HardwareState;
                    if (AbpSession.UserId != null)
                    {
                        foundationHardwareItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationHardwareItem.LastModificationTime = DateTime.Now;
                        foundationHardwareItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationHardwareItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationHardwareItemRepository.InsertAsync(foundationHardwareItem);
                    ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(foundationHardwareItem, new FoundationHardwareItemDto());


                }
            }
            this.CreateLog("创建软硬件项目1条");
            return result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> UpdateAsync(FoundationHardwareDto input)
        {
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == input.ProcessNumber && t.Id != input.Id).ToList();
            if (query.Count > 0)
            {
                throw new FriendlyException("工序编号重复！");
            }
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationHardwareRepository.UpdateAsync(entity);
             ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());

            if (input.ListHardware != null)
            {
                await _foundationFoundationHardwareItemRepository.DeleteAsync(t => t.FoundationHardwareId == entity.Id);
                foreach (var deviceItem in input.ListHardware)
                {
                    var entityItem = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(deviceItem, new FoundationHardwareItem());

                    FoundationHardwareItem foundationHardwareItem = new FoundationHardwareItem();
                    foundationHardwareItem.FoundationHardwareId = entity.Id;
                    foundationHardwareItem.CreationTime = DateTime.Now;
                    foundationHardwareItem.HardwareName = entityItem.HardwareName;
                    foundationHardwareItem.HardwarePrice = entityItem.HardwarePrice;
                    foundationHardwareItem.HardwareBusiness = entityItem.HardwareBusiness;
                    foundationHardwareItem.HardwareState = entityItem.HardwareState;
                    if (AbpSession.UserId != null)
                    {
                        foundationHardwareItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationHardwareItem.LastModificationTime = DateTime.Now;
                        foundationHardwareItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationHardwareItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationHardwareItemRepository.InsertAsync(foundationHardwareItem);
                    ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(foundationHardwareItem, new FoundationHardwareItemDto());
                }
            }
            this.CreateLog(" 编辑软硬件项目1条");
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto()); ;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationHardwareRepository.DeleteAsync(s => s.Id == id);
            this.CreateLog(" 删除软硬件项目1条");
        }


        /// <summary>
        /// 软硬件导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationHardware(IFormFile file)
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
                if (sheet != null)
                {
                    var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
                    var list1 = query.ToList();
                    var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list1, new List<FoundationHardwareDto>());
                    foreach (var item in dtos)
                    {
                        await _foundationHardwareRepository.DeleteAsync(s => s.Id == item.Id);
                        await _foundationFoundationHardwareItemRepository.DeleteAsync(s => s.FoundationHardwareId == item.Id);
                    }
                    int lastRowNum = sheet.LastRowNum;
                    List<FoundationHardwareDto> list = new List<FoundationHardwareDto>();
                    //跳过表头
                    for (int i = 1; i <= lastRowNum; i++)
                    {
                        var initRow = sheet.GetRow(i);
                        FoundationHardwareDto entity = new FoundationHardwareDto();
                        entity.IsDeleted = false;
                        entity.ProcessNumber = initRow.GetCell(0).ToString();
                        entity.ProcessName = initRow.GetCell(1).ToString();

                        var lastColNum = initRow.LastCellNum - 5;
                        var deviceCountt = lastColNum / 4;
                        for (int j = 0; j < deviceCountt; j++)
                        {
                            int pindex = j * 4 + 2;
                            FoundationHardwareItemDto foundationHardwareDto = new FoundationHardwareItemDto();
                            foundationHardwareDto.HardwareName = initRow.GetCell(pindex).ToString();
                            foundationHardwareDto.HardwareState = initRow.GetCell(pindex + 1).ToString();
                            if (initRow.GetCell(pindex + 2) != null && !string.IsNullOrEmpty(initRow.GetCell(pindex + 2).ToString()))
                            {
                                foundationHardwareDto.HardwarePrice = decimal.Parse(initRow.GetCell(pindex + 2).ToString());
                            }
                            foundationHardwareDto.HardwareBusiness = initRow.GetCell(pindex + 3).ToString();
                            entity.ListHardware.Add(foundationHardwareDto);
                        }
                        
                        entity.TraceabilitySoftware = initRow.GetCell(2 + deviceCountt * 4).ToString();
                        entity.TraceabilitySoftwareCost = decimal.Parse(initRow.GetCell(3 + deviceCountt * 4).ToString());
                         entity.SoftwareName = initRow.GetCell(4 + deviceCountt * 4).ToString();
                        entity.SoftwareState = initRow.GetCell(5 + deviceCountt * 4).ToString();
                        entity.SoftwarePrice = decimal.Parse(initRow.GetCell(6 + deviceCountt * 4).ToString());
                        entity.SoftwareBusiness = initRow.GetCell(7 + deviceCountt * 4).ToString();

                        list.Add(entity);
                    }

                    if (null != list)
                    {
                        foreach (var Item in list)
                        {
                            var entity = ObjectMapper.Map<FoundationHardwareDto, FoundationHardware>(Item, new FoundationHardware());
                            entity.CreationTime = DateTime.Now;
                            if (AbpSession.UserId != null)
                            {
                                entity.CreatorUserId = AbpSession.UserId.Value;
                                entity.LastModificationTime = DateTime.Now;
                                entity.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            entity.LastModificationTime = DateTime.Now;
                            entity = await this._foundationHardwareRepository.InsertAsync(entity);
                            var foundationDevice = _foundationHardwareRepository.InsertAndGetId(entity);
                            var result = ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto());
                            if (Item.ListHardware != null)
                            {
                                await _foundationFoundationHardwareItemRepository.DeleteAsync(t => t.FoundationHardwareId == result.Id);
                                foreach (var deviceItem in Item.ListHardware)
                                {
                                    var entityItem = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(deviceItem, new FoundationHardwareItem());

                                    FoundationHardwareItem foundationHardwareItem = new FoundationHardwareItem();
                                    foundationHardwareItem.FoundationHardwareId = foundationDevice;
                                    foundationHardwareItem.CreationTime = DateTime.Now;
                                    foundationHardwareItem.HardwarePrice = entityItem.HardwarePrice;
                                    foundationHardwareItem.HardwareName = entityItem.HardwareName;
                                    foundationHardwareItem.HardwareBusiness = entityItem.HardwareBusiness;
                                    //需要转换的地方
                                    string p = EnumHelper.GettDescriptionFromEnum(entityItem.HardwareState);
                                    foundationHardwareItem.HardwareState = p;
                                    if (AbpSession.UserId != null)
                                    {
                                        foundationHardwareItem.CreatorUserId = AbpSession.UserId.Value;
                                        foundationHardwareItem.LastModificationTime = DateTime.Now;
                                        foundationHardwareItem.LastModifierUserId = AbpSession.UserId.Value;

                                    }
                                    foundationHardwareItem.LastModificationTime = DateTime.Now;
                                    entityItem = await _foundationFoundationHardwareItemRepository.InsertAsync(foundationHardwareItem);
                                    ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(foundationHardwareItem, new FoundationHardwareItemDto());
                                }
                            }


                        }
                        this.CreateLog(" 导入软硬件项目" + list.Count + "条");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
            return true;
        }


        /// <summary>
        /// 导出软硬件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationHardwareDownloadStream(GetFoundationDevicesInput input)
        {
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(FoundationDeviceItemlist, new List<FoundationHardwareItemDto>());
                item.ListHardware = dtosItem;
                if (user != null)
                {
                    item.LastModifierUserName = user.Name;
                }
            }

            if (dtos == null || dtos.Count == 0)
            {
                throw new Exception("没有相关数据！");
            }

            // 设置表头
            List<DynamicExcelColumn> cols = new List<DynamicExcelColumn>();
            cols.Add(new DynamicExcelColumn("ProcessNumber") { Index = 0, Name = "工序编号", Width = 30 });
            cols.Add(new DynamicExcelColumn("ProcessName") { Index = 1, Name = "工序名称", Width = 30 });
            if (dtos[0].ListHardware != null && dtos[0].ListHardware.Count > 0)
            {
                List<FoundationHardwareItemDto> DeviceList = dtos[0].ListHardware;
                for (int i = 0; i < DeviceList.Count; i++)
                {
                    int pindex = i * 4 + 2;
                    cols.Add(new DynamicExcelColumn("DeviceName" + i) { Name = string.Format("硬件{0}名称", i + 1), Index = pindex, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DeviceStatus" + i) { Name = string.Format("硬件{0}状态", i + 1), Index = pindex + 1, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DevicePrice" + i) { Name = string.Format("硬件{0}单价", i + 1), Index = pindex + 2, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DeviceProvider" + i) { Name = string.Format("硬件{0}供应商", i + 1), Index = pindex + 3, Width = 30 });
                }
            }
            int a = dtos[0].ListHardware.Count * 4 + 2;

            cols.Add(new DynamicExcelColumn("TraceabilitySoftware") { Index = a, Name = "追溯软件", Width = 30 });
            cols.Add(new DynamicExcelColumn("TraceabilitySoftwareCost") { Index = a+1, Name = "追溯软件费用", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugeName") { Index = a+2, Name = "软件名称", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugeState") { Index = a + 3, Name = "软件状态", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugePrice") { Index = a + 4, Name = "软件单价", Width = 30 });
            cols.Add(new DynamicExcelColumn("SoftwareBusiness") { Index = a + 5, Name = "软件供应商", Width = 30 });
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = cols.ToArray()
            };

            // 设置值
            var values = new List<Dictionary<string, object>>();
            for (int i = 0; i < dtos.Count; i++)
            {
                FoundationHardwareDto foundationHardwareDto = dtos[i];
                // 填充数据
                var value = new Dictionary<string, object>()
                {
                    ["ProcessNumber"] = foundationHardwareDto.ProcessNumber,
                    ["ProcessName"] = foundationHardwareDto.ProcessName,
                    ["FixtureGaugeName"] = foundationHardwareDto.SoftwareName,
                    ["FixtureGaugePrice"] = foundationHardwareDto.SoftwarePrice,
                    ["FixtureGaugeState"] = foundationHardwareDto.SoftwareState,
                    ["TraceabilitySoftware"] = foundationHardwareDto.TraceabilitySoftware,
                    ["TraceabilitySoftwareCost"] = foundationHardwareDto.TraceabilitySoftwareCost,
                    ["SoftwareBusiness"] = foundationHardwareDto.SoftwareBusiness,
                };
                for (int j = 0; j < foundationHardwareDto.ListHardware.Count; j++)
                {
                    FoundationHardwareItemDto foundationFixtureItemDto = foundationHardwareDto.ListHardware[j];
                    value["DeviceName" + j] = foundationFixtureItemDto.HardwareName;
                    //需要转换的地方
                    string p = EnumHelper.GetCodeFromEnum(foundationFixtureItemDto.HardwareState);
                    value["DeviceStatus" + j] = p;
                    value["DevicePrice" + j] = foundationFixtureItemDto.HardwarePrice;
                    value["DeviceProvider" + j] = foundationFixtureItemDto.HardwareBusiness;
                }
                values.Add(value);
            }

            var memoryStream = new MemoryStream();
            //MiniExcel.SaveAs(memoryStream, values.ToArray(), configuration: config);
            memoryStream.SaveAs(values.ToArray(), configuration: config);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FoundationHardware" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
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
                entity.Remark = Remark;
                entity.Type = logType;


            }
            entity.Remark = Remark;
            entity.Type = logType;
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
