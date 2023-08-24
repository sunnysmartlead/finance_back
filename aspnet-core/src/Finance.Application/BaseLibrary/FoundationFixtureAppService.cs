using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using NPOI.SS.UserModel;
using Spire.Pdf.General.Paper.Uof;
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
    public class FoundationFixtureAppService : ApplicationService
    {
        private readonly IRepository<FoundationFixture, long> _foundationFixtureRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationFixtureItem, long> _foundationFoundationFixtureItemRepository;

        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.Fixture;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationFixtureRepository"></param>
        public FoundationFixtureAppService(
            IRepository<FoundationFixture, long> foundationFixtureRepository,
            IRepository<FoundationFixtureItem, long> foundationFoundationFixtureItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationFixtureRepository = foundationFixtureRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationFixtureItemRepository = foundationFoundationFixtureItemRepository;
        }


        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> GetAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationFixtureDto>> GetListAsync(GetFoundationFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            // 数据返回
            return new PagedResultDto<FoundationFixtureDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationFixtureDto>> GetListAllAsync(GetFoundationFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);


            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDFixtureItemlist = this._foundationFoundationFixtureItemRepository.GetAll().Where(f => f.FoundationFixtureId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationFixtureItem>, List<FoundationFixtureItemDto>>(FoundationDFixtureItemlist, new List<FoundationFixtureItemDto>());
                item.FixtureList = dtosItem;
                if (user != null)
                {
                    item.LastModifierUserName = user.Name;
                }
            }
            // 数据返回
            return dtos;
        }

        /// <summary>
        /// 检具导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationFixture(IFormFile file)
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
                    var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
                    var list1 = query.ToList();
                    var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list1, new List<FoundationFixtureDto>());
                    foreach (var item in dtos)
                    {
                        await _foundationFixtureRepository.DeleteAsync(s => s.Id == item.Id);
                        await _foundationFoundationFixtureItemRepository.DeleteAsync(s => s.FoundationFixtureId == item.Id);
                    }
                    int lastRowNum = sheet.LastRowNum;
                    List<FoundationFixtureDto> list = new List<FoundationFixtureDto>();
                    //跳过表头
                    for (int i = 1; i <= lastRowNum; i++)
                    {
                        var initRow = sheet.GetRow(i);
                        FoundationFixtureDto entity = new FoundationFixtureDto();
                        entity.IsDeleted = false;
                        entity.ProcessName = initRow.GetCell(0).ToString();
                        entity.ProcessNumber = initRow.GetCell(1).ToString();
                    
                        var lastColNum = initRow.LastCellNum - 6;
                        var deviceCountt = lastColNum / 4;
                        for (int j = 0; j < deviceCountt; j++)
                        {
                            int pindex = j * 4 + 2;
                            FoundationFixtureItemDto foundationFixtureItemDto = new FoundationFixtureItemDto();
                            foundationFixtureItemDto.FixtureName = initRow.GetCell(pindex).ToString();
                            foundationFixtureItemDto.FixtureState = initRow.GetCell(pindex + 1).ToString();
                            if (initRow.GetCell(pindex + 2) != null && !string.IsNullOrEmpty(initRow.GetCell(pindex + 2).ToString()))
                            {
                                foundationFixtureItemDto.FixturePrice = decimal.Parse(initRow.GetCell(pindex + 2).ToString());
                            }
                            foundationFixtureItemDto.FixtureProvider = initRow.GetCell(pindex + 3).ToString();
                            entity.FixtureList.Add(foundationFixtureItemDto);
                        }
                        entity.FixtureGaugeName = initRow.GetCell(2 + deviceCountt * 4).ToString();
                        entity.FixtureGaugeState = initRow.GetCell(3 + deviceCountt * 4).ToString();
                       entity.FixtureGaugePrice = decimal.Parse(initRow.GetCell(4 + deviceCountt * 4).ToString());
                        entity.FixtureGaugeBusiness = initRow.GetCell(5 + deviceCountt * 4).ToString();
                        list.Add(entity);
                    }

                    if (null != list)
                    {
                        foreach (var Item in list)
                        {
                            var entity = ObjectMapper.Map<FoundationFixtureDto, FoundationFixture>(Item, new FoundationFixture());
                            entity.CreationTime = DateTime.Now;
                            if (AbpSession.UserId != null)
                            {
                                entity.CreatorUserId = AbpSession.UserId.Value;
                                entity.LastModificationTime = DateTime.Now;
                                entity.LastModifierUserId = AbpSession.UserId.Value;
                            }
                            entity.LastModificationTime = DateTime.Now;
                            entity = await this._foundationFixtureRepository.InsertAsync(entity);
                            var foundationDevice = _foundationFixtureRepository.InsertAndGetId(entity);
                            var result = ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity, new FoundationFixtureDto());
                            if (Item.FixtureList != null)
                            {
                                await _foundationFoundationFixtureItemRepository.DeleteAsync(t => t.FoundationFixtureId == result.Id);
                                foreach (var deviceItem in Item.FixtureList)
                                {
                                    var entityItem = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(deviceItem, new FoundationFixtureItem());

                                    FoundationFixtureItem foundationFixtureItem = new FoundationFixtureItem();
                                    foundationFixtureItem.FoundationFixtureId = foundationDevice;
                                    foundationFixtureItem.CreationTime = DateTime.Now;
                                    foundationFixtureItem.FixtureName = entityItem.FixtureName;
                                    foundationFixtureItem.FixturePrice = entityItem.FixturePrice;
                                    foundationFixtureItem.FixtureState = entityItem.FixtureState;
                                    foundationFixtureItem.FixtureProvider = entityItem.FixtureProvider;
                                    if (AbpSession.UserId != null)
                                    {
                                        foundationFixtureItem.CreatorUserId = AbpSession.UserId.Value;
                                        foundationFixtureItem.LastModificationTime = DateTime.Now;
                                        foundationFixtureItem.LastModifierUserId = AbpSession.UserId.Value;

                                    }
                                    foundationFixtureItem.LastModificationTime = DateTime.Now;
                                    entityItem = await _foundationFoundationFixtureItemRepository.InsertAsync(foundationFixtureItem);
                                    ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(foundationFixtureItem, new FoundationFixtureItemDto());
                                }
                            }


                        }
                        this.CreateLog(" 导入治具项目" + list.Count + "条");
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
        /// 导出治具信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationFixtureDownloadStream(GetFoundationDevicesInput input)
        {
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDeviceItemlist = this._foundationFoundationFixtureItemRepository.GetAll().Where(f => f.FoundationFixtureId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationFixtureItem>, List<FoundationFixtureItemDto>>(FoundationDeviceItemlist, new List<FoundationFixtureItemDto>());
                item.FixtureList = dtosItem;
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
            if (dtos[0].FixtureList != null && dtos[0].FixtureList.Count > 0)
            {
                List<FoundationFixtureItemDto> DeviceList = dtos[0].FixtureList;
                for (int i = 0; i < DeviceList.Count; i++)
                {
                    int pindex = i * 4 + 2;
                    cols.Add(new DynamicExcelColumn("DeviceName" + i) { Name = string.Format("治具{0}名称", i + 1), Index = pindex, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DeviceStatus" + i) { Name = string.Format("治具{0}状态", i + 1), Index = pindex + 1, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DevicePrice" + i) { Name = string.Format("治具{0}单价", i + 1), Index = pindex + 2, Width = 30 });
                    cols.Add(new DynamicExcelColumn("DeviceProvider" + i) { Name = string.Format("治具{0}供应商", i + 1), Index = pindex + 3, Width = 30 });
                }
            }
            int a = dtos[0].FixtureList.Count * 4 + 2;
          
            cols.Add(new DynamicExcelColumn("FixtureGaugeName") { Index = a, Name = "检具名称", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugeState") { Index = a + 1, Name = "检具状态", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugePrice") { Index = a+2, Name = "检具单价", Width = 30 });
            cols.Add(new DynamicExcelColumn("FixtureGaugeBusiness") { Index = a+3, Name = "检具供应商", Width = 30 });
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = cols.ToArray()
            };

            // 设置值
            var values = new List<Dictionary<string, object>>();
            for (int i = 0; i < dtos.Count; i++)
            {
                FoundationFixtureDto foundationFixtureDto = dtos[i];
                // 填充数据
                var value = new Dictionary<string, object>()
                {
                    ["ProcessNumber"] = foundationFixtureDto.ProcessNumber,
                    ["ProcessName"] = foundationFixtureDto.ProcessName,
                    ["FixtureGaugeName"] = foundationFixtureDto.FixtureGaugeName,
                    ["FixtureGaugePrice"] = foundationFixtureDto.FixtureGaugePrice,
                    ["FixtureGaugeState"] = foundationFixtureDto.FixtureGaugeState,
                    ["FixtureGaugeBusiness"] = foundationFixtureDto.FixtureGaugeBusiness
                };
                for (int j = 0; j < foundationFixtureDto.FixtureList.Count; j++)
                {
                    FoundationFixtureItemDto foundationFixtureItemDto = foundationFixtureDto.FixtureList[j];
                    value["DeviceName" + j] = foundationFixtureItemDto.FixtureName;
                    value["DeviceStatus" + j] = foundationFixtureItemDto.FixtureState;
                    value["DevicePrice" + j] = foundationFixtureItemDto.FixturePrice;
                    value["DeviceProvider" + j] = foundationFixtureItemDto.FixtureProvider;
                }
                values.Add(value);
            }

            var memoryStream = new MemoryStream();
            //MiniExcel.SaveAs(memoryStream, values.ToArray(), configuration: config);
            memoryStream.SaveAs(values.ToArray(), configuration: config);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FoundationFixture" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> GetEditorAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> CreateAsync(FoundationFixtureDto input)
        {
         

            var entity = ObjectMapper.Map<FoundationFixtureDto, FoundationFixture>(input, new FoundationFixture());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationFixtureRepository.InsertAsync(entity);
            var foundationDevice = _foundationFixtureRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity, new FoundationFixtureDto());
            if (input.FixtureList != null)
            {
                await _foundationFoundationFixtureItemRepository.DeleteAsync(t => t.FoundationFixtureId == result.Id);
                foreach (var deviceItem in input.FixtureList)
                {
                    var entityItem = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(deviceItem, new FoundationFixtureItem());

                    FoundationFixtureItem foundationFixtureItem = new FoundationFixtureItem();
                    foundationFixtureItem.FoundationFixtureId = foundationDevice;
                    foundationFixtureItem.CreationTime = DateTime.Now;
                    foundationFixtureItem.FixtureName = entityItem.FixtureName;
                    foundationFixtureItem.FixturePrice = entityItem.FixturePrice;
                    foundationFixtureItem.FixtureBusiness = entityItem.FixtureBusiness;
                    foundationFixtureItem.FixtureState = entityItem.FixtureState;
                    foundationFixtureItem.FixtureProvider = entityItem.FixtureProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationFixtureItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationFixtureItem.LastModificationTime = DateTime.Now;
                        foundationFixtureItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationFixtureItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationFixtureItemRepository.InsertAsync(foundationFixtureItem);
                    ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(foundationFixtureItem, new FoundationFixtureItemDto());
                }
            }
            this.CreateLog(" 创建治具项目1条");
            return result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> UpdateAsync(FoundationFixtureDto input)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationFixtureRepository.UpdateAsync(entity);
            if (input.FixtureList != null)
            {
                await _foundationFoundationFixtureItemRepository.DeleteAsync(t => t.FoundationFixtureId == entity.Id);
                foreach (var deviceItem in input.FixtureList)
                {
                    var entityItem = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(deviceItem, new FoundationFixtureItem());

                    FoundationFixtureItem foundationFixtureItem = new FoundationFixtureItem();
                    foundationFixtureItem.FoundationFixtureId = entity.Id;
                    foundationFixtureItem.CreationTime = DateTime.Now;
                    foundationFixtureItem.FixtureName = entityItem.FixtureName;
                    foundationFixtureItem.FixturePrice = entityItem.FixturePrice;
                    foundationFixtureItem.FixtureBusiness = entityItem.FixtureBusiness;
                    foundationFixtureItem.FixtureState = entityItem.FixtureState;
                    foundationFixtureItem.FixtureProvider = entityItem.FixtureProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationFixtureItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationFixtureItem.LastModificationTime = DateTime.Now;
                        foundationFixtureItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationFixtureItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationFixtureItemRepository.InsertAsync(foundationFixtureItem);
                    ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(foundationFixtureItem, new FoundationFixtureItemDto());
                }
            }
            this.CreateLog(" 编辑治具项目1条");
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationFixtureRepository.DeleteAsync(s => s.Id == id);
            this.CreateLog(" 删除治具项目1条");
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
            entity.Remark = Remark;
            entity.Type = logType;
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
