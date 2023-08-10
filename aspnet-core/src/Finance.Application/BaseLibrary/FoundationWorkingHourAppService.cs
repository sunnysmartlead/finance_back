using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using Spire.Pdf.Exporting.XPS.Schema;
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
    public class FoundationWorkingHourAppService : ApplicationService
    {
        private readonly IRepository<FoundationWorkingHour, long> _foundationWorkingHourRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationWorkingHourItem, long> _foundationFoundationWorkingHourItemRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly int logType = 8;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationWorkingHourRepository"></param>
        public FoundationWorkingHourAppService(
            IRepository<FoundationWorkingHourItem, long> foundationFoundationWorkingHourItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FoundationWorkingHour, long> foundationWorkingHourRepository)
        {
            _foundationWorkingHourRepository = foundationWorkingHourRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationWorkingHourItemRepository = foundationFoundationWorkingHourItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> GetAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationWorkingHourDto>> GetListAsync(GetFoundationWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHour>, List<FoundationWorkingHourDto>>(list, new List<FoundationWorkingHourDto>());
            // 数据返回
            return new PagedResultDto<FoundationWorkingHourDto>(totalCount, dtos);
        }


        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationWorkingHourDto>> GetListAllAsync(GetFoundationWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.ProcessName))
            {
                query = query.Where(t => t.ProcessName.Contains(input.ProcessName));
            }

            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHour>, List<FoundationWorkingHourDto>>(list, new List<FoundationWorkingHourDto>());
            foreach (var item in dtos)
            {
                var FoundationDeviceItemlist = this._foundationFoundationWorkingHourItemRepository.GetAll().Where(f => f.FoundationWorkingHourId == item.Id).ToList();
                var dtosItem = ObjectMapper.Map<List<FoundationWorkingHourItem>, List<FoundationWorkingHourItemDto>>(FoundationDeviceItemlist, new List<FoundationWorkingHourItemDto>());
                item.ListFoundationWorkingHour = dtosItem;
                var user = this._userRepository.GetAll().Where(u => u.Id == item.CreatorUserId).ToList().FirstOrDefault();
                if (user != null)
                {
                    item.LastModifierUserName = user.Name;
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
        public virtual async Task<FoundationWorkingHourDto> GetEditorAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> CreateAsync(FoundationWorkingHourDto input)
        {
            var entity = ObjectMapper.Map<FoundationWorkingHourDto, FoundationWorkingHour>(input,new FoundationWorkingHour());

            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationWorkingHourRepository.InsertAsync(entity);
            var foundationDevice = _foundationWorkingHourRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity, new FoundationWorkingHourDto());
            if (input.ListFoundationWorkingHour != null)
            {
                await _foundationFoundationWorkingHourItemRepository.DeleteAsync(t => t.FoundationWorkingHourId == result.Id);
                foreach (var deviceItem in input.ListFoundationWorkingHour)
                {
                    var entityItem = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(deviceItem, new FoundationWorkingHourItem());

                    FoundationWorkingHourItem foundationWorkingHourItem = new FoundationWorkingHourItem();
                    foundationWorkingHourItem.FoundationWorkingHourId = foundationDevice;
                    foundationWorkingHourItem.CreationTime = DateTime.Now;
                    foundationWorkingHourItem.Year = entityItem.Year;
                    foundationWorkingHourItem.MachineHour = entityItem.MachineHour;
                    foundationWorkingHourItem.NumberPersonnel = entityItem.NumberPersonnel;
                    foundationWorkingHourItem.LaborHour = entityItem.LaborHour;
                    if (AbpSession.UserId != null)
                    {
                        foundationWorkingHourItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                        foundationWorkingHourItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationWorkingHourItemRepository.InsertAsync(foundationWorkingHourItem);
                }
            }
            await this.CreateLog(" 新增工时项目1条");
            return result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> UpdateAsync(FoundationWorkingHourDto input)
        {
        
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationWorkingHourRepository.UpdateAsync(entity);
            var result = ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity, new FoundationWorkingHourDto());

            if (input.ListFoundationWorkingHour != null)
            {
                await _foundationFoundationWorkingHourItemRepository.DeleteAsync(t => t.FoundationWorkingHourId == result.Id);
                foreach (var deviceItem in input.ListFoundationWorkingHour)
                {
                    var entityItem = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(deviceItem, new FoundationWorkingHourItem());

                    FoundationWorkingHourItem foundationWorkingHourItem = new FoundationWorkingHourItem();
                    foundationWorkingHourItem.FoundationWorkingHourId = entity.Id;
                    foundationWorkingHourItem.CreationTime = DateTime.Now;
                    foundationWorkingHourItem.Year = entityItem.Year;
                    foundationWorkingHourItem.MachineHour = entityItem.MachineHour;
                    foundationWorkingHourItem.NumberPersonnel = entityItem.NumberPersonnel;
                    foundationWorkingHourItem.LaborHour = entityItem.LaborHour;
                    if (AbpSession.UserId != null)
                    {
                        foundationWorkingHourItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                        foundationWorkingHourItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationWorkingHourItemRepository.InsertAsync(foundationWorkingHourItem);
                }
            }
            await this.CreateLog(" 编辑工时项目1条");
            return result ;
        }

        /// <summary>
        /// 工时导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<FoundationWorkingHourDto>> UploadFoundationStandardTechnology(IFormFile file)
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
                var tempmbPath = @"D:\1.xlsx";

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
               
                 
                    if (val.Contains("年"))
                    {
                        isYear = true;
                        isFrock = false;
                        isForm = false;
                        isDevice = false;
                    }
                    
                     if (isYear)
                    {
                        yearStrs.Add(val);
                        yearCols += 3;
                        isYear = false;
                    }
                }
                List<FoundationWorkingHourDto> foundationWorkingHourDtos = new List<FoundationWorkingHourDto>();

                // 取值
                var keys = cols.Keys.ToList();
                for (int i = 2; i < rows.Count; i++)
                {
                    FoundationWorkingHourDto  foundationWorkingHourDto=   new FoundationWorkingHourDto();
                    IDictionary<String, Object> row = rows[i];
                    Dictionary<string, object> rowItem = new Dictionary<string, object>();
                    //总数居
                    foundationWorkingHourDto.ProcessName = (row[keys[1]]).ToString();
                    foundationWorkingHourDto.ProcessNumber = (row[keys[0]]).ToString();
                    // 解析年度部分
                    List<FoundationWorkingHourItemDto> foundationWorkingHourItemDtos = new List<FoundationWorkingHourItemDto>();
                    List<Dictionary<string, object>> years = new List<Dictionary<string, object>>();
                    int yearNum = yearCols / 3;
                    for (int j = 0; j < yearNum; j++)
                    {
                        FoundationWorkingHourItemDto foundationWorkingHourItem = new FoundationWorkingHourItemDto();
                        string yearstr = yearStrs[j];
                        Dictionary<string, object> yearItem = new Dictionary<string, object>();
                        int fromStartIndex = j * 3 + 3;
                        var val0 = row[keys[fromStartIndex]];
                        var val1 = row[keys[fromStartIndex + 1]];
                        var val2 = row[keys[fromStartIndex + 2]];
                        foundationWorkingHourItem.LaborHour = val0.ToString();
                        foundationWorkingHourItem.MachineHour = val1.ToString(); ;
                        foundationWorkingHourItem.NumberPersonnel = val2.ToString();
                        foundationWorkingHourItem.Year = yearstr;
                        foundationWorkingHourItemDtos.Add(foundationWorkingHourItem);
                    }
                    foundationWorkingHourDto.ListFoundationWorkingHour = foundationWorkingHourItemDtos;

                    foundationWorkingHourDtos.Add(foundationWorkingHourDto);
                }
                if (null != foundationWorkingHourDtos) {
                    for (int i = 0; i < foundationWorkingHourDtos.Count; i++)//100为自定义，实际循环中不会达到
                    {
                        FoundationWorkingHour entity =   new FoundationWorkingHour();
                        entity.CreationTime = DateTime.Now;
                        entity.ProcessName = foundationWorkingHourDtos[i].ProcessName;
                        entity.ProcessNumber = foundationWorkingHourDtos[i].ProcessNumber;
                        if (AbpSession.UserId != null)
                        {
                            entity.CreatorUserId = AbpSession.UserId.Value;
                            entity.LastModificationTime = DateTime.Now;
                            entity.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        entity.LastModificationTime = DateTime.Now;
                        entity = await this._foundationWorkingHourRepository.InsertAsync(entity);
                        var foundationDevice = _foundationWorkingHourRepository.InsertAndGetId(entity);
                        var result = ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity, new FoundationWorkingHourDto());
                        if (foundationWorkingHourDtos[i].ListFoundationWorkingHour != null)
                        {
                            await _foundationFoundationWorkingHourItemRepository.DeleteAsync(t => t.FoundationWorkingHourId == result.Id);
                            foreach (var deviceItem in foundationWorkingHourDtos[i].ListFoundationWorkingHour)
                            {
                                var entityItem = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(deviceItem, new FoundationWorkingHourItem());

                                FoundationWorkingHourItem foundationWorkingHourItem = new FoundationWorkingHourItem();
                                foundationWorkingHourItem.FoundationWorkingHourId = foundationDevice;
                                foundationWorkingHourItem.CreationTime = DateTime.Now;
                                foundationWorkingHourItem.Year = entityItem.Year;
                                foundationWorkingHourItem.MachineHour = entityItem.MachineHour;
                                foundationWorkingHourItem.NumberPersonnel = entityItem.NumberPersonnel;
                                foundationWorkingHourItem.LaborHour = entityItem.LaborHour;
                                if (AbpSession.UserId != null)
                                {
                                    foundationWorkingHourItem.CreatorUserId = AbpSession.UserId.Value;
                                    foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                                    foundationWorkingHourItem.LastModifierUserId = AbpSession.UserId.Value;

                                }
                                foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                                entityItem = await _foundationFoundationWorkingHourItemRepository.InsertAsync(foundationWorkingHourItem);
                            }
                        }
                    }

                }
                await this.CreateLog(" 导入工时项目" + foundationWorkingHourDtos.Count + "条");
                return foundationWorkingHourDtos;



            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败！");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationWorkingHourRepository.DeleteAsync(s => s.Id == id);
            await this.CreateLog(" 删除工时项目1条");
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
