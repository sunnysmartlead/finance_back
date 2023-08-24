using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Finance.Processes
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FProcessesAppService : ApplicationService
    {
        private readonly LogType logType = LogType.WorkingProcedure;
        private readonly IRepository<FProcesses, long> _fProcessesRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="fProcessesRepository"></param>
        public FProcessesAppService(
             IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FProcesses, long> fProcessesRepository)
        {
            _fProcessesRepository = fProcessesRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FProcessesDto> GetByIdAsync(long id)
        {
            FProcesses entity = await _fProcessesRepository.GetAsync(id);

            return ObjectMapper.Map<FProcesses, FProcessesDto>(entity,new FProcessesDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FProcessesDto>> GetListAsync(GetFProcessessInput input)
        {
            // 设置查询条件
            var query = this._fProcessesRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FProcesses>, List<FProcessesDto>>(list, new List<FProcessesDto>());
            // 数据返回
            return new PagedResultDto<FProcessesDto>(totalCount, dtos);
        }


        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FProcessesDto>> GetListAllAsync(GetFProcessessInput input)
        {
            // 设置查询条件
            var query = this._fProcessesRepository.GetAll();
            if (!string.IsNullOrEmpty(input.ProcessName))
            {
                query = query.Where(t => t.ProcessName.Contains(input.ProcessName));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            List<FProcessesDto> list1 = new List<FProcessesDto>();
            if (null != list ) {
             
                foreach (var item in list)
                {
                    FProcessesDto f=  new FProcessesDto();
                    f.ProcessNumber = item.ProcessNumber;
                    f.ProcessName = item.ProcessName;
                    f.Id= item.Id;
                    f.LastModificationTime= item.LastModificationTime;
                    var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                    if (user != null)
                    {
                        f.LastModifierUserName = user.Name;
                    }
                    list1.Add(f);

                }

            }
            // 数据返回
            return list1;
        }
        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FProcessesDto> GetEditorAsync(long id)
        {
            FProcesses entity = await _fProcessesRepository.GetAsync(id);
            await this.CreateLog("修改工序项目1条");
            return ObjectMapper.Map<FProcesses, FProcessesDto>(entity,new FProcessesDto());
        }


        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationFProcesses(IFormFile file)
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
                var query = this._fProcessesRepository.GetAll().Where(t => t.IsDeleted == false);
                var list = query.ToList();
                foreach (var item in list)
                {
                    await _fProcessesRepository.DeleteAsync(s => s.Id == item.Id);
                }
                //跳过表头
                for (int i = 1; i < 1000; i++)//100为自定义，实际循环中不会达到
                {
                    var initRow = sheet.GetRow(i);
                    if (initRow == null) break;
                    var s1 = initRow.GetCell(0);
                    var s2 = initRow.GetCell(1);
                    if (null == initRow.GetCell(0) || string.IsNullOrEmpty(initRow.GetCell(1).ToString()))
                    {
                        break;
                    }
                    else
                    {
                        FProcesses p = new FProcesses();
                        p.IsDeleted = false;
                        p.ProcessNumber = initRow.GetCell(0).ToString();
                        p.ProcessName = initRow.GetCell(1).ToString();
                        p.CreationTime = DateTime.Now;
                        p.LastModificationTime = DateTime.Now;
                        if (AbpSession.UserId != null)
                        {
                            p.CreatorUserId = AbpSession.UserId.Value;
                            p.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        try
                        {
                            var result = await this._fProcessesRepository.InsertAsync(p);
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
        /// 导出FoundationProcedure
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationProcedureDownloadStream(GetFProcessessInput input)
        {


            var dtoAll = this._fProcessesRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
            var memoryStream = new MemoryStream();
            //数据转换
            var list = dtoAll.ToList();
            var values = new List<DownloadFProcessesDto>() { };
            foreach (var item in list)
            {
                var s = new DownloadFProcessesDto()
                {
                    ProcessName = item.ProcessName,
                    ProcessNumber = item.ProcessNumber,
                 
                };
                values.Add(s);
            }
            var memoryStream1 = new MemoryStream();
            memoryStream1.SaveAs(values, sheetName: "Sheet1");
            memoryStream1.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FProcesses" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FProcessesDto> CreateAsync(FProcessesDto input)
        {
            FProcesses f =  new FProcesses();
            f.ProcessName = input.ProcessName;
            f.ProcessNumber = input.ProcessNumber;
            f.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                f.CreatorUserId = AbpSession.UserId.Value;
                f.LastModificationTime = DateTime.Now;
                f.LastModifierUserId = AbpSession.UserId.Value;
            }
            f.LastModificationTime = DateTime.Now;
            await _fProcessesRepository.InsertAsync(f);
            await this.CreateLog(" 删除工序项目1条");
            FProcessesDto f1 = new FProcessesDto();
            f1.CreationTime = DateTime.Now;
      
            f1.ProcessNumber = input.ProcessNumber;
            f1.ProcessName = input.ProcessName;
            f1.LastModifierUserName = input.LastModifierUserName;
            f1.LastModificationTime = DateTime.Now;
             return f1;
        }




        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FProcessesDto> UpdateAsync(FProcessesDto input)
        {
            FProcesses entity = await _fProcessesRepository.GetAsync(input.Id);
            entity.ProcessName= input.ProcessName;
            entity.ProcessNumber = input.ProcessNumber;
            entity.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _fProcessesRepository.UpdateAsync(entity);
            this.CreateLog("编辑工序项目1条");
            FProcessesDto f = new FProcessesDto();
            f.CreationTime = DateTime.Now;
            f.Id= input.Id;
            f.ProcessNumber = input.ProcessNumber;
            f.ProcessName   = input.ProcessName;
            f.LastModifierUserName = input.LastModifierUserName;
            f.LastModificationTime = DateTime.Now;
;            return f;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _fProcessesRepository.DeleteAsync(s => s.Id == id);
            this.CreateLog("删除工序项目1条");
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
