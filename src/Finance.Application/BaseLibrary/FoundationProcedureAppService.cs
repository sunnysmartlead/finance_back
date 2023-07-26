using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Http;
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
    /// 管理工装库
    /// </summary>
    public class FoundationProcedureAppService : ApplicationService
    {
        private readonly IRepository<FoundationProcedure, long> _foundationProcedureRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationProcedureRepository"></param>
        public FoundationProcedureAppService(
            IRepository<FoundationProcedure, long> foundationProcedureRepository)
        {
            _foundationProcedureRepository = foundationProcedureRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> GetAsyncById(long id)
        {
            FoundationProcedure entity = await _foundationProcedureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity,new FoundationProcedureDto());
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
            var sheet = workbook.GetSheetAt(3);
            List<FoundationProcedureDto> list = new List<FoundationProcedureDto>();
            //判断是否获取到 sheet
            if (sheet != null)
            {
                //跳过表头
                for (int i = 2; i < 1000; i++)//100为自定义，实际循环中不会达到
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
                        entity.ProcessName = initRow.GetCell(2).ToString();
                        entity.ProcessNumber = initRow.GetCell(3).ToString();
                        entity.InstallationName = initRow.GetCell(4).ToString();
                        entity.InstallationPrice = decimal.Parse(initRow.GetCell(5).ToString());
                        entity.InstallationSupplier = initRow.GetCell(6).ToString();
                        entity.TestName = initRow.GetCell(7).ToString();
                        entity.TestPrice = decimal.Parse(initRow.GetCell(8).ToString());
                        entity.CreationTime = DateTime.Now;
                        entity.LastModificationTime = DateTime.Now;
                        if (AbpSession.UserId != null)
                        {
                            entity.CreatorUserId = AbpSession.UserId.Value;
                            entity.LastModifierUserId = AbpSession.UserId.Value;
                        }
                        var entity2 = ObjectMapper.Map<FoundationProcedureDto, FoundationProcedure>(entity, new FoundationProcedure());
                        var result = await this._foundationProcedureRepository.InsertAsync(entity2);
                    }
                }
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
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity,new FoundationProcedureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationProcedureDto> CreateAsync(FoundationProcedureDto input)
        {
            var entity = ObjectMapper.Map<FoundationProcedureDto, FoundationProcedure>(input,new FoundationProcedure());
            entity = await _foundationProcedureRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity,new FoundationProcedureDto());
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
            entity = await _foundationProcedureRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationProcedure, FoundationProcedureDto>(entity,new FoundationProcedureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationProcedureRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
