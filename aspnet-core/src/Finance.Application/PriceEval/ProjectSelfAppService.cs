using Finance.Ext;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.PriceEval.Dto.ProjectSelf;
using Abp.Linq.Extensions;
using Finance.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using Finance.Users.Dto;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using MiniExcelLibs;
using System.IO;
using System.Linq.Dynamic.Core;
using Abp.Domain.Entities;
using NPOI.SS.Formula.Functions;

namespace Finance.PriceEval
{
    /// <summary>
    /// 项目自建表
    /// </summary>
    public class ProjectSelfAppService : AsyncCrudAppService<ProjectSelf, ProjectSelfListDto, long, GetProjectSelfInput, CreateProjectSelfInput, UpdateProjectSelfInput>
    {
        private readonly IRepository<BaseStoreLog, long> _baseStoreLogRepository;
        private readonly UserManager _userManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="baseStoreLogRepository"></param>
        /// <param name="userManager"></param>
        public ProjectSelfAppService(IRepository<ProjectSelf, long> repository, IRepository<BaseStoreLog, long> baseStoreLogRepository, UserManager userManager) : base(repository)
        {
            _baseStoreLogRepository = baseStoreLogRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// 自建表查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<ProjectSelf> CreateFilteredQuery(GetProjectSelfInput input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), p => p.Code.Contains(input.Code))
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                p => p.Custom.Contains(input.Filter)
                || p.CustomName.Contains(input.Filter)
                || p.Code.Contains(input.Filter)
                || p.Description.Contains(input.Filter)
                || p.SubCode.Contains(input.Filter)
                || p.SubDescription.Contains(input.Filter));
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<ProjectSelf> ApplySorting(IQueryable<ProjectSelf> query, GetProjectSelfInput input)
        {
            return query.OrderByDescending(r => r.Id);
        }

        /// <summary>
        /// 创建自建表记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ProjectSelfListDto> CreateAsync(CreateProjectSelfInput input)
        {
            await _baseStoreLogRepository.InsertAsync(new BaseStoreLog
            {
                OperationType = OperationType.Insert,
                Count = 1,
                Text = "增加了一条记录"
            });
            return await base.CreateAsync(input);
        }

        /// <summary>
        /// 修改自建表记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ProjectSelfListDto> UpdateAsync(UpdateProjectSelfInput input)
        {
            await _baseStoreLogRepository.InsertAsync(new BaseStoreLog
            {
                OperationType = OperationType.Update,
                Count = 1,
                Text = $"修改了记录，Id为：{input.Id}",
            });
            return await base.UpdateAsync(input);
        }

        /// <summary>
        /// 删除自建表记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await _baseStoreLogRepository.InsertAsync(new BaseStoreLog
            {
                OperationType = OperationType.Delete,
                Count = 1,
                Text = $"删除了记录，Id为：{input.Id}",
            });
            await base.DeleteAsync(input);
        }

        /// <summary>
        /// 修改基本库日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task UpdateBaseStoreLog(UpdateBaseStoreLogInput input)
        {
            var entity = await _baseStoreLogRepository.GetAsync(input.Id);
            entity.Text = input.Text;
        }

        /// <summary>
        /// 获取基础库日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<BaseStoreLogListDto>> GetBaseStoreLog(GetBaseStoreLogInput input)
        {
            var data = (from l in _baseStoreLogRepository.GetAll()
                        join u in _userManager.Users on l.CreatorUserId equals u.Id
                        select new BaseStoreLogListDto
                        {
                            Id = l.Id,
                            CreatorUserId = l.CreatorUserId,
                            CreationTime = l.CreationTime,
                            OperationType = l.OperationType,
                            Count = l.Count,
                            Text = l.Text,
                            UserName = u.Name
                        }).WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.UserName.Contains(input.Filter) || p.Text.Contains(input.Filter));
            var count = await data.CountAsync();
            var paged = data.OrderByDescending(p => p.Id).PageBy(input);
            var result = await paged.ToListAsync();
            return new PagedResultDto<BaseStoreLogListDto>(count, result);
        }

        /// <summary>
        /// 批量导入（Excel文件里的信息必须全部合法，如果有不合法信息，不执行导入，并返回不合法信息详情。修改至Excel文件中的信息全部合法为止）
        /// </summary>
        /// <param name="excle"></param>
        /// <returns></returns>
        public async virtual Task<ExcelImportResult> ExcelImport([Required] IFormFile excle)
        {
            var stream = excle.OpenReadStream();
            var rows = MiniExcel.Query<CreateProjectSelfInput>(stream).ToList();

            var dbEntity = await Repository.GetAllListAsync();
            dbEntity.ForEach(async p => await Repository.DeleteAsync(p));

            var entity = ObjectMapper.Map<List<ProjectSelf>>(rows);
            await Repository.BulkInsertAsync(entity);


            //foreach (var row in rows)
            //{
            //    var entity = await Repository.FirstOrDefaultAsync(p => p.Code == row.Code);

            //    if (entity is null)
            //    {
            //        await Repository.InsertAsync(ObjectMapper.Map<ProjectSelf>(row));
            //    }
            //    else
            //    {
            //        ObjectMapper.Map(row, entity);
            //        await Repository.UpdateAsync(entity);
            //    }
            //}

            await _baseStoreLogRepository.InsertAsync(new BaseStoreLog
            {
                OperationType = OperationType.Import,
                Count = rows.Count,
                Text = $"使用Excel导入了{rows.Count}条记录",
            });

            return new ExcelImportResult
            {
                Total = rows.Count,
                InsertTotal = rows.Count,
                UpdateTotal = 0,
                NegativeTotal = 0,
                IsSuccess = true,
                Message = "添加成功"
            };
        }

        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <returns></returns>
        public virtual async Task<FileResult> GetImportTemplate()
        {
            var memoryStream = new MemoryStream();
            await MiniExcel.SaveAsAsync(memoryStream, new[] { new CreateProjectSelfInput { Custom = string.Empty } });
            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "项目自建表导入模板.xlsx" };
        }

        /// <summary>
        /// 导出项目自建表
        /// </summary>
        /// <returns></returns>
        public virtual async Task<FileResult> GetProjectSelfImport()
        {
            var data = await Repository.GetAllListAsync();
            var dto = ObjectMapper.Map<List<CreateProjectSelfInput>>(data);

            var memoryStream = new MemoryStream();
            await MiniExcel.SaveAsAsync(memoryStream, dto);
            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "项目自建表.xlsx" };
        }
    }
}
