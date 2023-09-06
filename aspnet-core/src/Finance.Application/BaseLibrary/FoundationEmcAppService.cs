using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Finance.Authorization.Users;
using Finance.Processes;
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

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationEmcAppService : ApplicationService
    {
        private readonly IRepository<FoundationEmc, long> _foundationEmcRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly LogType logType = LogType.EMC;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationEmcAppService(
            IRepository<FoundationEmc, long> foundationreliableRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationEmcRepository = foundationreliableRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }


        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> GetAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationEmcDto>> GetListAsync(GetFoundationEmcsInput input)
        {
            // 设置查询条件
            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
            // 数据返回
            return new PagedResultDto<FoundationEmcDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationEmcDto>> GetListAllAsync(GetFoundationEmcsInput input)
        {
            // 设置查询条件
            var query = this._foundationEmcRepository.GetAll();
            if (!string.IsNullOrEmpty(input.Name))
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
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
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
        public virtual async Task<FoundationEmcDto> GetEditorAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<TaskFoundationEmcDto> CreateAsync(FoundationEmcDto input)
        {

            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false && t.Name.Equals(input.Name));
            var list = query.ToList();
            TaskFoundationEmcDto taskFoundationEmcDto = new TaskFoundationEmcDto();
            if (list.Count > 0) {
                taskFoundationEmcDto.Error = "名字有重复！！！";
                taskFoundationEmcDto.Success = false;
                return taskFoundationEmcDto;

            }
           
            var entity = ObjectMapper.Map<FoundationEmcDto, FoundationEmc>(input, new FoundationEmc());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId  = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity.IsDeleted = false;
            entity = await _foundationEmcRepository.InsertAsync(entity);
             this.CreateLog(" 创建EMC项目1条");
            taskFoundationEmcDto.Success = true;
            return taskFoundationEmcDto;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<TaskFoundationEmcDto> UpdateAsync(FoundationEmcDto input)
        {
            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false && t.Name.Equals(input.Name) && t.Id != input.Id);
            var list = query.ToList();
            TaskFoundationEmcDto taskFoundationEmcDto = new TaskFoundationEmcDto();
            if (list.Count > 0)
            {
                taskFoundationEmcDto.Error = "名字有重复！！！";
                taskFoundationEmcDto.Success = false;
                return taskFoundationEmcDto;

            }
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity.LastModificationTime = DateTime.Now;
            entity.IsDeleted= false;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationEmcRepository.UpdateAsync(entity);
             this.CreateLog(" 编辑EMC项目1条");
            taskFoundationEmcDto.Success= true;
            return taskFoundationEmcDto;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationEmcRepository.DeleteAsync(s => s.Id == id);
            this.CreateLog(" 删除EMC项目1条");
        }

        /// <summary>
        /// 导出FoundationEmc
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<FileStreamResult> FoundationEmcDownloadStream()
        {
            var list = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false).ToList();
            var memoryStream = new MemoryStream();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
            List<FoundationEmcExportDto> values = new List<FoundationEmcExportDto>();
            foreach (var item in dtos)
            {
                FoundationEmcExportDto foundationEmc = new FoundationEmcExportDto();

                foundationEmc.Classification = item.Classification;
                foundationEmc.Name = item.Name;
                foundationEmc.Price = item.Price;
                foundationEmc.Unit = item.Unit;
                values.Add(foundationEmc);
            }
            var memoryStream1 = new MemoryStream();
            memoryStream1.SaveAs(values, sheetName: "Sheet1");
            memoryStream1.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "FoundationEMC" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
            };
        }


        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFoundationEmc(IFormFile file)
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
                var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false);
                var list = query.ToList();
                var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
                foreach (var item in dtos)
                {
                    await _foundationEmcRepository.DeleteAsync(s => s.Id == item.Id);
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
                        FoundationEmcDto entity = new FoundationEmcDto();
                        entity.IsDeleted = false;
                        entity.Classification = initRow.GetCell(0).ToString();
                        entity.Name = initRow.GetCell(1).ToString();
                        entity.Unit = initRow.GetCell(3).ToString();
                        entity.Price = decimal.Parse(initRow.GetCell(2).ToString());
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
                            var entity2 = ObjectMapper.Map<FoundationEmcDto, FoundationEmc>(entity, new FoundationEmc());
                            var result = await this._foundationEmcRepository.InsertAsync(entity2);
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
