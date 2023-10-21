using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.PriceEval.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using MiniExcelLibs;
using NPOI.SS.UserModel;
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
    public class FoundationreliableAppService : ApplicationService
    {
        private readonly IRepository<Foundationreliable, long> _foundationreliableRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.Environment;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationreliableAppService(
            IRepository<Foundationreliable, long> foundationreliableRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationreliableRepository = foundationreliableRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> GetAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 列表-带分页
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationreliableDto>> GetListByPagingAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            // 数据返回
            return new PagedResultDto<FoundationreliableDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationreliableDto>> GetListAllAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
             List<FoundationreliableDto> l= new List<FoundationreliableDto>();
            if (!string.IsNullOrEmpty(input.Name) && null != input.Type)
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }

          /*  if (string.IsNullOrEmpty(input.Name) && null != input.Type)
            {
                return l;
            }*/
            if (!string.IsNullOrEmpty(input.Name) && 1 == input.Type)
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }

            if (!string.IsNullOrEmpty(input.Classification))
            {
                query = query.Where(t => t.Classification.Contains(input.Classification));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
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
        public virtual async Task<FoundationreliableDto> GetEditorAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<TaskFoundationreliableDto> CreateAsync(FoundationreliableDto input)
        {
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false && t.Name.Equals(input.Name));
            var list = query.ToList();
            TaskFoundationreliableDto taskFoundationreliable = new TaskFoundationreliableDto();
            if (list.Count > 0)
            {
                taskFoundationreliable.Error = "名字有重复！！！";
                taskFoundationreliable.Success = false;
                return taskFoundationreliable;

            }
            var entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, new Foundationreliable());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.IsDeleted =  false;
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _foundationreliableRepository.InsertAsync(entity);
            this.CreateLog(" 创建环境项目1条");
            taskFoundationreliable.Success = true;
            return taskFoundationreliable;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<TaskFoundationreliableDto> UpdateAsync(FoundationreliableDto input)
        {
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false && t.Name.Equals(input.Name) && t.Id != input.Id);
            var list = query.ToList();
            TaskFoundationreliableDto taskFoundationreliable = new TaskFoundationreliableDto();
            if (list.Count > 0)
            {
                taskFoundationreliable.Error = "名字有重复！！！";
                taskFoundationreliable.Success = false;
                return taskFoundationreliable;

            }
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, entity);
            entity.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.UpdateAsync(entity);
            this.CreateLog(" 编辑环境项目1条");
            taskFoundationreliable.Success = true;
            return taskFoundationreliable;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationreliableDownloadStream()
        {
            var list = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
            var memoryStream = new MemoryStream();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            List<FoundationreliableExport> values = new List<FoundationreliableExport>();
            foreach (var item in dtos)
            {
                FoundationreliableExport foundationEmc = new FoundationreliableExport();

                foundationEmc.Classification = item.Classification;
                foundationEmc.Name = item.Name;
                if (null != item.Price)
                {
                    foundationEmc.Price = (double)item.Price;
                }
                else {
                    foundationEmc.Price = 0;
                }
              
                foundationEmc.Unit = item.Unit;
                values.Add(foundationEmc);
            }
            var memoryStream1 = new MemoryStream();
            memoryStream1.SaveAs(values, sheetName: "Sheet1");
            memoryStream1.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Foundationreliable" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);
            entity.DeletionTime = DateTime.Now;
            entity.IsDeleted = true;
            if (AbpSession.UserId != null)
            {
                entity.DeleterUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.UpdateAsync(entity);
            this.CreateLog(" 删除环境项目1条");
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationreliable(IFormFile file)
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
                var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
                var list = query.ToList();
                var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
                foreach (var item in dtos)
                {
                    await _foundationreliableRepository.DeleteAsync(s => s.Id == item.Id);
                }
                //跳过表头
                for (int i = 1; i < 1000; i++)//100为自定义，实际循环中不会达到
                {
                    var initRow = sheet.GetRow(i);
                    if (initRow == null) break;
                    var s1 = initRow.GetCell(1);
                    var s2 = initRow.GetCell(2);
                    if (null == initRow.GetCell(1) || string.IsNullOrEmpty(initRow.GetCell(2).ToString()))
                    {
                        break;
                    }
                    else
                    {
                        FoundationreliableDto entity = new FoundationreliableDto();
                        entity.IsDeleted = false;
                        entity.Classification = initRow.GetCell(0).ToString();
                        entity.Name = initRow.GetCell(1).ToString();
                        entity.Unit = initRow.GetCell(3).ToString();
                        entity.Price = double.Parse(initRow.GetCell(2).ToString());
                        entity.Laboratory = initRow.GetCell(4).ToString();
                        entity.CreationTime = DateTime.Now;
                        entity.LastModificationTime = DateTime.Now;
                        if (AbpSession.UserId != null)
                        {
                            entity.CreatorUserId = AbpSession.UserId.Value;
                            entity.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        try
                        {
                            var entity2 = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(entity, new Foundationreliable());
                            var result = await this._foundationreliableRepository.InsertAsync(entity2);
                        }
                        catch (Exception ex)
                        {
                            string str = ex.Message;
                        }
                    }
                }
                // 获取总数
                var totalCount = query.Count();
                this.CreateLog(" 新表单导入，共" + totalCount + "条数据");
            }
            return true;
        }


        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<FileResult> DownloadFoundationreliableStream(FoundationreliableDto input)
        {
            try
            {
                var data = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
                var values = new List<FoundationreliableDto>();
                foreach (var item in data)
                {
                    var obj = new FoundationreliableDto()
                    {
                        Classification = item.Classification,
                        Laboratory = item.Laboratory,
                        Name = item.Name,
                        Price = item.Price,
                        Unit = item.Unit
                    };
                    values.Add(obj);
                }
                DownloadFoundationreliableDto downloadFoundationreliableDto = new DownloadFoundationreliableDto()
                {
                    FLList = values
                };
                using (var memoryStream = new MemoryStream())
                {
                    await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/Foundationreliable.xlsx", downloadFoundationreliableDto);
                    return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = "Foundationreliable" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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


    public class Test 
    {
        public string Name { get; set; } = "Jack";
        public DateTime CreateDate { get; set; } = new DateTime(2021, 01, 01);
        public bool VIP { get; set; } = true;
        public int Points { get; set; } = 123;
    }
}
