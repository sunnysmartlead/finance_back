﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Castle.MicroKernel.Registration;
using Finance.Authorization.Users;
using Finance.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Asn1.Ocsp;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using test;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationDeviceAppService : ApplicationService
    {
        private readonly IRepository<FoundationDevice, long> _foundationDeviceRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.Equipment;


        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationDeviceItem, long> _foundationFoundationDeviceItemRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        public FoundationDeviceAppService(
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
            IRepository<FoundationDevice, long> foundationDeviceRepository,
            IRepository<FoundationDeviceItem, long> foundationFoundationDeviceItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationDeviceRepository = foundationDeviceRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationDeviceItemRepository = foundationFoundationDeviceItemRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        }



        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> GetAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationDeviceDto>> GetListAsync(GetFoundationDevicesInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
            // 数据返回
            return new PagedResultDto<FoundationDeviceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationDeviceDto>> GetListAllAsync(GetFoundationDevicesInput input)
        {

            if (null == input.DeviceName || input.DeviceName.Equals(""))
            {
                // 设置查询条件
                var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);


                // 查询数据
                var list = query.ToList();
                //数据转换
                var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
                foreach (var item in dtos)
                {

                    var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                    var FoundationDeviceItemlist = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == item.Id).ToList();

                    //数据转换
                    var dtosItem = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(FoundationDeviceItemlist, new List<FoundationDeviceItemDto>());
                    //需要转换的地方
                    foreach (var foundationDeviceItem in dtosItem)
                    {
                    }
                    item.DeviceList = dtosItem;
                    if (user != null)
                    {
                        item.LastModifierUserName = user.Name;
                    }
                }
                // 数据返回
                return dtos;
            }
            else
            {
                List<FoundationDeviceDto> foundationDeviceDtos = new List<FoundationDeviceDto>();
                var FoundationHardwareDtoId = await (from u in _foundationFoundationDeviceItemRepository.GetAll()
                                                     join ur in _foundationDeviceRepository.GetAll() on u.ProcessHoursEnterId equals ur.Id
                                                     where u.DeviceName.Contains(input.DeviceName)
                                                     select new
                                                     {
                                                         Id = ur.Id
                                                     }).Distinct().ToListAsync();

                foreach (var item in FoundationHardwareDtoId)
                {
                    FoundationDeviceDto foundationHardwareDto = new FoundationDeviceDto();
                    FoundationDevice entity = await _foundationDeviceRepository.GetAsync(item.Id);
                    var FoundationDeviceItem = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == entity.Id).ToList();

                    //数据转换
                    var dtosItem = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(FoundationDeviceItem, new List<FoundationDeviceItemDto>());

                    foundationHardwareDto = ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
                    foundationHardwareDto.DeviceList = dtosItem;
                    foundationDeviceDtos.Add(foundationHardwareDto);


                }
                return foundationDeviceDtos;

            }

        }


        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> GetEditorAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<FoundationDeviceDto> CreateAsync(FoundationDeviceDto input)
        {

            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == input.ProcessNumber).ToList();
            if (query.Count > 0)
            {
                throw new FriendlyException("工序编号重复");
            }

            var entity = ObjectMapper.Map<FoundationDeviceDto, FoundationDevice>(input, new FoundationDevice());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationDeviceRepository.InsertAsync(entity);
            var foundationDevice = _foundationDeviceRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
            if (input.DeviceList != null)
            {
                await _foundationFoundationDeviceItemRepository.DeleteAsync(t => t.ProcessHoursEnterId == result.Id);
                foreach (var deviceItem in input.DeviceList)
                {
                    var entityItem = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(deviceItem, new FoundationDeviceItem());

                    FoundationDeviceItem foundationDeviceItem = new FoundationDeviceItem();
                    foundationDeviceItem.ProcessHoursEnterId = foundationDevice;
                    foundationDeviceItem.CreationTime = DateTime.Now;
                    foundationDeviceItem.DeviceNumber = entityItem.DeviceNumber;
                    foundationDeviceItem.DeviceName = entityItem.DeviceName;
                    foundationDeviceItem.DeviceStatus = entityItem.DeviceStatus;
                    foundationDeviceItem.DevicePrice = entityItem.DevicePrice;
                    foundationDeviceItem.DeviceProvider = entityItem.DeviceProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationDeviceItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationDeviceItem.LastModificationTime = DateTime.Now;
                        foundationDeviceItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationDeviceItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationDeviceItemRepository.InsertAsync(foundationDeviceItem);
                    ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(foundationDeviceItem, new FoundationDeviceItemDto());
                }
            }
            await this.CreateLog(" 创建设备项目1条");
            return result;
        }


        /// <summary>
        /// 设备导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationDevice(IFormFile file)
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
                    #region 判断单元格是否为空
                    // 检查第一行内容是否符合要求
                    IRow firstRow = sheet.GetRow(0);
                    if (firstRow == null ||
                       firstRow.GetCell(0)?.StringCellValue != "工序编号" ||
                       firstRow.GetCell(1)?.StringCellValue != "工序名称" ||
                       firstRow.GetCell(2)?.StringCellValue != "设备1名称" ||
                       firstRow.GetCell(3)?.StringCellValue != "设备1状态")
                    {
                        throw new FriendlyException("模版错误请见检查!");

                    }
                    for (int rowIdx = 0; rowIdx <= sheet.LastRowNum; rowIdx++)
                    {
                        IRow row = sheet.GetRow(rowIdx);
                        if (row == null)
                            continue;

                        for (int colIdx = 0; colIdx < row.LastCellNum; colIdx++)
                        {
                            ICell cell = row.GetCell(colIdx);
                            if (cell == null || cell.CellType == CellType.Blank)
                            {
                                throw new FriendlyException($"行: {rowIdx + 1}, 列: {colIdx + 1},为空,请检查");
                            }
                        }
                    }
                    #endregion

                    var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
                    var list1 = query.ToList();
                    var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list1, new List<FoundationDeviceDto>());
                    foreach (var item in dtos)
                    {
                        await _foundationDeviceRepository.DeleteAsync(s => s.Id == item.Id);
                        await _foundationFoundationDeviceItemRepository.DeleteAsync(s => s.ProcessHoursEnterId == item.Id);
                    }
                    int lastRowNum = sheet.LastRowNum;
                    List<FoundationDeviceDto> list = new List<FoundationDeviceDto>();
                    //跳过表头
                    for (int i = 1; i <= lastRowNum; i++)
                    {
                        var initRow = sheet.GetRow(i);
                        FoundationDeviceDto entity = new FoundationDeviceDto();
                        entity.IsDeleted = false;
                        //判断单元格是否为空,如果是空就抛出异常
                        //if (initRow.GetCell(0).CellType == CellType.Blank) throw new FriendlyException($"行:{i + 1},的A列(工序编号)为空!请检查!");
                        entity.ProcessNumber = initRow.GetCell(0).ToString();
                        //判断单元格是否为空,如果是空就抛出异常
                        //if (initRow.GetCell(1).CellType == CellType.Blank) throw new FriendlyException($"行:{i + 1},的B列(工序名称)为空!请检查!");
                        entity.ProcessName = initRow.GetCell(1).ToString();
                        entity.CreationTime = DateTime.Now;
                        entity.LastModificationTime = DateTime.Now;
                        if (AbpSession.UserId != null)
                        {
                            entity.CreatorUserId = AbpSession.UserId.Value;
                            entity.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        var lastColNum = initRow.LastCellNum - 2;
                        var deviceCountt = lastColNum / 4;
                        for (int j = 0; j < deviceCountt; j++)
                        {
                            //判断每一个单元格是否为空,如果是空就抛出异常
                            if (sheet.GetRow(i).GetCell(j).CellType == CellType.Blank || string.IsNullOrEmpty(sheet.GetRow(i).GetCell(j).ToString())) throw new FriendlyException($"行:{i + 1};列:{j + 1} 的单元格不能为空,请检查!");
                            int pindex = j * 4 + 2;
                            FoundationDeviceItemDto foundationDeviceItemDto = new FoundationDeviceItemDto();
                            foundationDeviceItemDto.DeviceName = initRow.GetCell(pindex).ToString();
                            foundationDeviceItemDto.DeviceStatus = initRow.GetCell(pindex + 1).ToString();
                            if (initRow.GetCell(pindex + 2).CellType != CellType.Numeric) throw new FriendlyException($"行:{i + 1},的{pindex + 2+1}列(设备单价),必须是数据类型!请检查!");
                            foundationDeviceItemDto.DevicePrice = decimal.Parse(initRow.GetCell(pindex + 2).ToString());
                            foundationDeviceItemDto.DeviceProvider = initRow.GetCell(pindex + 3).ToString();
                            entity.DeviceList.Add(foundationDeviceItemDto);
                        }
                        list.Add(entity);
                    }

                    if (null != list)
                    {
                        foreach (var Item in list)
                        {
                            var entity = ObjectMapper.Map<FoundationDeviceDto, FoundationDevice>(Item, new FoundationDevice());
                            entity.CreationTime = DateTime.Now;
                            if (AbpSession.UserId != null)
                            {
                                entity.CreatorUserId = AbpSession.UserId.Value;
                                entity.LastModificationTime = DateTime.Now;
                                entity.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            entity.LastModificationTime = DateTime.Now;
                            entity = await this._foundationDeviceRepository.InsertAsync(entity);
                            var foundationDevice = _foundationDeviceRepository.InsertAndGetId(entity);
                            var result = ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
                            if (Item.DeviceList != null)
                            {
                                await _foundationFoundationDeviceItemRepository.DeleteAsync(t => t.ProcessHoursEnterId == result.Id);
                                foreach (var deviceItem in Item.DeviceList)
                                {
                                    var entityItem = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(deviceItem, new FoundationDeviceItem());

                                    FoundationDeviceItem foundationDeviceItem = new FoundationDeviceItem();
                                    foundationDeviceItem.ProcessHoursEnterId = foundationDevice;
                                    foundationDeviceItem.CreationTime = DateTime.Now;
                                    foundationDeviceItem.DeviceNumber = entityItem.DeviceNumber;
                                    foundationDeviceItem.DeviceName = entityItem.DeviceName;
                                    //需要转换的地方
                                    if (null != entityItem.DeviceStatus)
                                    {
                                        List<FinanceDictionaryDetail> dics = _financeDictionaryDetailRepository.GetAll().Where(p => p.DisplayName == entityItem.DeviceStatus && p.FinanceDictionaryId == "Sbzt").ToList();
                                        //需要转换的地方

                                        if (dics != null && dics.Count > 0)
                                        {
                                            foundationDeviceItem.DeviceStatus = dics[0].Id;
                                        }
                                        else
                                        {
                                            foundationDeviceItem.DeviceStatus = "";
                                        }
                                    }
                                    foundationDeviceItem.DevicePrice = entityItem.DevicePrice;
                                    foundationDeviceItem.DeviceProvider = entityItem.DeviceProvider;
                                    if (AbpSession.UserId != null)
                                    {
                                        foundationDeviceItem.CreatorUserId = AbpSession.UserId.Value;
                                        foundationDeviceItem.LastModificationTime = DateTime.Now;
                                        foundationDeviceItem.LastModifierUserId = AbpSession.UserId.Value;

                                    }
                                    foundationDeviceItem.LastModificationTime = DateTime.Now;
                                    entityItem = await _foundationFoundationDeviceItemRepository.InsertAsync(foundationDeviceItem);
                                    ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(foundationDeviceItem, new FoundationDeviceItemDto());
                                }
                            }


                        }
                        var query1 = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);

                        await this.CreateLog(" 导入设备项目" + query1.Count() + "条");
                    }
                }
            }
            catch (FriendlyException ex)
            {
                throw new FriendlyException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FriendlyException("模版错误,请检查模版!");
            }
            return true;
        }


        /// <summary>
        /// 导出设备信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationDeviceDownloadStream(GetFoundationDevicesInput input)
        {
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDeviceItemlist = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(FoundationDeviceItemlist, new List<FoundationDeviceItemDto>());
                item.DeviceList = dtosItem;
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
            cols.Add(new DynamicExcelColumn("ProcessNumber") { Index = 0, Name = "工序编号", Width = 10 });
            cols.Add(new DynamicExcelColumn("ProcessName") { Index = 1, Name = "工序名称", Width = 20 });
            if (dtos[0].DeviceList != null && dtos[0].DeviceList.Count > 0)
            {
                List<FoundationDeviceItemDto> DeviceList = dtos[0].DeviceList;
                for (int i = 0; i < DeviceList.Count; i++)
                {
                    int pindex = i * 4 + 2;
                    cols.Add(new DynamicExcelColumn("DeviceName" + i) { Name = string.Format("设备{0}名称", i + 1), Index = pindex, Width = 20 });
                    cols.Add(new DynamicExcelColumn("DeviceStatus" + i) { Name = string.Format("设备{0}状态", i + 1), Index = pindex + 1 });
                    cols.Add(new DynamicExcelColumn("DevicePrice" + i) { Name = string.Format("设备{0}单价", i + 1), Index = pindex + 2 });
                    cols.Add(new DynamicExcelColumn("DeviceProvider" + i) { Name = string.Format("设备{0}供应商", i + 1), Index = pindex + 3, Width = 25 });
                }
            }
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = cols.ToArray()
            };

            // 设置值
            var values = new List<Dictionary<string, object>>();
            for (int i = 0; i < dtos.Count; i++)
            {
                FoundationDeviceDto foundationDeviceDto = dtos[i];
                // 填充数据
                var value = new Dictionary<string, object>()
                {
                    ["ProcessNumber"] = foundationDeviceDto.ProcessNumber,
                    ["ProcessName"] = foundationDeviceDto.ProcessName
                };
                for (int j = 0; j < foundationDeviceDto.DeviceList.Count; j++)
                {
                    FoundationDeviceItemDto foundationDeviceItemDto = foundationDeviceDto.DeviceList[j];
                    value["DeviceName" + j] = foundationDeviceItemDto.DeviceName;
                    //需要转换的地方
                    //需要转换的地方
                    var entity = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == foundationDeviceItemDto.DeviceStatus);
                    if (entity == null)
                    {
                        value["DeviceStatus" + j] = "";

                    }
                    else
                    {
                        value["DeviceStatus" + j] = entity.DisplayName;
                    }
                    value["DevicePrice" + j] = foundationDeviceItemDto.DevicePrice;
                    value["DeviceProvider" + j] = foundationDeviceItemDto.DeviceProvider;
                }
                values.Add(value);
            }

            var memoryStream = new MemoryStream();
            //MiniExcel.SaveAs(memoryStream, values.ToArray(), configuration: config);
            memoryStream.SaveAs(values.ToArray(), configuration: config);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FoundationDevice" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> UpdateAsync(FoundationDeviceDto input)
        {
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false && t.ProcessNumber == input.ProcessNumber && t.Id != input.Id).ToList();
            if (query.Count > 0)
            {
                throw new FriendlyException("工序编号重复");
            }
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationDeviceRepository.UpdateAsync(entity);
            if (input.DeviceList != null)
            {
                await _foundationFoundationDeviceItemRepository.DeleteAsync(t => t.ProcessHoursEnterId == entity.Id);
                foreach (var deviceItem in input.DeviceList)
                {
                    var entityItem = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(deviceItem, new FoundationDeviceItem());

                    FoundationDeviceItem foundationDeviceItem = new FoundationDeviceItem();
                    foundationDeviceItem.ProcessHoursEnterId = entity.Id;
                    foundationDeviceItem.CreationTime = DateTime.Now;
                    foundationDeviceItem.DeviceNumber = entityItem.DeviceNumber;
                    foundationDeviceItem.DeviceName = entityItem.DeviceName;
                    foundationDeviceItem.DeviceStatus = entityItem.DeviceStatus;
                    foundationDeviceItem.DevicePrice = entityItem.DevicePrice;
                    foundationDeviceItem.DeviceProvider = entityItem.DeviceProvider;

                    if (AbpSession.UserId != null)
                    {
                        foundationDeviceItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationDeviceItem.LastModificationTime = DateTime.Now;
                        foundationDeviceItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationDeviceItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationDeviceItemRepository.InsertAsync(foundationDeviceItem);
                    ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(foundationDeviceItem, new FoundationDeviceItemDto());
                }
            }
            this.CreateLog(" 编辑设备项目1条");
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationDeviceRepository.DeleteAsync(s => s.Id == id);
            this.CreateLog(" 删除设备项目1条");
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



            }
            FoundationLogs temp = null;
            try
            {
                temp = _foundationLogsRepository.GetAllList(p => p.Type == logType).OrderByDescending(p => p.LastModificationTime).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            VersionWithIncrement versionWithIncrement = null;
            if (temp != null && !string.IsNullOrEmpty(temp.Version))
            {
                versionWithIncrement = new VersionWithIncrement(temp.Version);
                var str = versionWithIncrement.IncrementRevision();
                entity.Version = str;
            }
            else
            {
                versionWithIncrement = new VersionWithIncrement();
                var str = versionWithIncrement.IncrementRevision();
                entity.Version = str;
            }
            entity.Remark = Remark;
            entity.Type = logType;
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
