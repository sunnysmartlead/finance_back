using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.PriceEval.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using Spire.Pdf.Exporting.XPS.Schema;
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
    /// 管理工装库
    /// </summary>
    public class FoundationProcedureAppService : ApplicationService
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.WorkClothes;
        private readonly IRepository<FoundationProcedure, long> _foundationProcedureRepository;


        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationProcedureAppService(
            IRepository<FoundationProcedure, long> foundationProcedureRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationProcedureRepository = foundationProcedureRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> GetAsyncById(long id)
        {
            FoundationProcedure entity = await _foundationProcedureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity, new FoundationProcedureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationProcedureDto>> GetListAsync(GetFoundationProceduresInput input)
        {
            // 设置查询条件
            var query = this._foundationProcedureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationProcedure>, List<FoundationProcedureDto>>(list, new List<FoundationProcedureDto>());
            // 数据返回
            return new PagedResultDto<FoundationProcedureDto>(totalCount, dtos);
        }


        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationProcedureDto>> GetListAllAsync(GetFoundationProceduresInput input)
        {
            // 设置查询条件
            var query = this._foundationProcedureRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.ProcessName))
            {
                query = query.Where(t => t.InstallationName.Contains(input.ProcessName));
            }
            if (!string.IsNullOrEmpty(input.InstallationName))
            {
                query = query.Where(t => t.InstallationName.Contains(input.InstallationName));
            }
            if (!string.IsNullOrEmpty(input.TestName))
            {
                query = query.Where(t => t.TestName.Contains(input.TestName));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationProcedure>, List<FoundationProcedureDto>>(list, new List<FoundationProcedureDto>());
            foreach (var item in dtos)
            {
                var user = this._foundationProcedureRepository.GetAll().Where(u => u.Id == item.CreatorUserId).ToList().FirstOrDefault();
                if (user != null)
                {
                    item.LastModifierUserName = user.Name;
                }
            }
            // 数据返回
            return dtos;
        }


        /// <summary>
        /// 导出FoundationProcedure
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationProcedureDownloadStream(GetFoundationProceduresInput input)
        {
            var dtoAll = this._foundationProcedureRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
            var memoryStream = new MemoryStream();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationProcedure>, List<FoundationProcedureDto>>(dtoAll, new List<FoundationProcedureDto>());
            var values = new List<DownloadFoundationProcedureDto>() { };
            foreach (var item in dtos)
            {
                var s = new DownloadFoundationProcedureDto()
                {
                    InstallationName = item.InstallationName,
                    InstallationPrice = item.InstallationPrice,
                    InstallationSupplier = item.InstallationSupplier,
                    ProcessName = item.ProcessName,
                    ProcessNumber = item.ProcessNumber,
                    TestName = item.TestName,
                    TestPrice = item.TestPrice
                };
                values.Add(s);
            }
            var memoryStream1 = new MemoryStream();
            memoryStream1.SaveAs(values, sheetName: "Sheet1");
            memoryStream1.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FoundationProcedure" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }

        /// <summary>
        /// 工装库数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationProcedure(IFormFile file)
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
                   var query = this._foundationProcedureRepository.GetAll().Where(t => t.IsDeleted == false);
                var list = query.ToList();
                var totalCount = query.Count();
                var dtos = ObjectMapper.Map<List<FoundationProcedure>, List<FoundationProcedureDto>>(list, new List<FoundationProcedureDto>());
                foreach (var item in dtos)
                {
                    await _foundationProcedureRepository.DeleteAsync(s => s.Id == item.Id);
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
                        FoundationProcedureDto entity = new FoundationProcedureDto();
                        entity.IsDeleted = false;
                        entity.ProcessNumber = initRow.GetCell(0).ToString();
                        entity.ProcessName = initRow.GetCell(1).ToString();
                        entity.InstallationName = initRow.GetCell(2).ToString();
                        entity.InstallationPrice = decimal.Parse(initRow.GetCell(3).ToString());
                        entity.InstallationSupplier = initRow.GetCell(4).ToString();
                        entity.TestName = initRow.GetCell(5).ToString();
                        entity.TestPrice = decimal.Parse(initRow.GetCell(6).ToString());
                        entity.CreationTime = DateTime.Now;
                        entity.LastModificationTime = DateTime.Now;
                        if (AbpSession.UserId != null)
                        {
                            entity.CreatorUserId = AbpSession.UserId.Value;
                            entity.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        try
                        {
                            var entity2 = ObjectMapper.Map<FoundationProcedureDto, FoundationProcedure>(entity, new FoundationProcedure());
                            var result = await this._foundationProcedureRepository.InsertAsync(entity2);
                        }
                        catch (Exception ex)
                        {
                            string str = ex.Message;
                        }

                    }
               
                }

                // 获取总数
                this.CreateLog(" 新表单导入，共" + totalCount + "条数据");

            }
            return true;
        }


        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> GetEditorAsyncById(long id)
        {
            FoundationProcedure entity = await _foundationProcedureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity, new FoundationProcedureDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> CreateAsync(FoundationProcedureDto input)
        {

            var entity = ObjectMapper.Map<FoundationProcedureDto, FoundationProcedure>(input, new FoundationProcedure());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _foundationProcedureRepository.InsertAsync(entity);
            this.CreateLog(" 创建工装项目1条");
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity, new FoundationProcedureDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> UpdateAsync(FoundationProcedureDto input)
        {
            FoundationProcedure entity = await _foundationProcedureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationProcedureRepository.UpdateAsync(entity);
            this.CreateLog(" 修改工装项目1条");
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity, new FoundationProcedureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationProcedureRepository.DeleteAsync(s => s.Id == id);
            await this.CreateLog(" 删除工装项目1条");
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
