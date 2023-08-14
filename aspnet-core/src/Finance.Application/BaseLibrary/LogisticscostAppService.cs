using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Finance.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
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
    public class LogisticscostAppService : ApplicationService
    {
        private readonly IRepository<Logisticscost, long> _logisticscostRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logisticscostRepository"></param>
        public LogisticscostAppService(
            IRepository<Logisticscost, long> logisticscostRepository)
        {
            _logisticscostRepository = logisticscostRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);

            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<LogisticscostDto>> GetListAsync(GetLogisticscostsInput input)
        {
            // 设置查询条件
            var query = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(list, new List<LogisticscostDto>());
            // 数据返回
            return new PagedResultDto<LogisticscostDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<LogisticscostResponseDto>> GetListAllAsync(GetLogisticscostsInput input)
        {
    
            var query = (from a in _logisticscostRepository.GetAllList(p =>
            p.IsDeleted == false && p.ProductId == input.ProductId && p.AuditFlowId == input.AuditFlowId
            ).Select(p => p.Classification).Distinct()
                         select a).ToList();
            query.Count();
            var dtos = ObjectMapper.Map<List<String>, List<String>>(query, new List<String>());
            List<LogisticscostResponseDto> logisticscostResponseList = new List<LogisticscostResponseDto>();
            foreach (var item in dtos)
            {
                LogisticscostResponseDto logisticscostResponse = new LogisticscostResponseDto();

                var queryItem = this._logisticscostRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.ProductId == input.ProductId && t.Classification == item).ToList();
                var dtosItem = ObjectMapper.Map<List<Logisticscost>, List<LogisticscostDto>>(queryItem, new List<LogisticscostDto>());
                logisticscostResponse.LogisticscostList = dtosItem;
                logisticscostResponse.Classification = item;
                logisticscostResponseList.Add(logisticscostResponse);
            }

            // 数据返回
            return logisticscostResponseList;
        }


        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> GetEditorAsyncById(long id)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(id);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(LogisticscostDto input)
        {
           
            //先根据零件和流程删除之前的数据
            var query = _logisticscostRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.ProductId == input.ProductId);
            var list = query.ToList();
            foreach (var item in list)
            {
                await _logisticscostRepository.DeleteAsync(s => s.Id == item.Id);
            }
            List<LogisticscostResponseDto> LogisticscostList = input.LogisticscostList;
            foreach (var item in LogisticscostList)
            {
                foreach (var item1 in item.LogisticscostList)
                {
                    var entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(item1, new Logisticscost());
                    Logisticscost logisticscost =  new Logisticscost();
                    logisticscost.ProductId =  input.ProductId;
                    logisticscost.AuditFlowId = input.AuditFlowId;
                    logisticscost.Classification = item.Classification;
                    logisticscost.CreationTime = DateTime.Now;
                    logisticscost.FreightPrice = entity.FreightPrice;
                    logisticscost.MonthlyDemandPrice = entity.MonthlyDemandPrice;
                    logisticscost.PackagingPrice = entity.PackagingPrice;
                    logisticscost.SinglyDemandPrice = entity.SinglyDemandPrice;
                    logisticscost.StoragePrice = entity.StoragePrice;
                    logisticscost.TransportPrice = entity.TransportPrice;
                    logisticscost.Remark = entity.Remark;
                    logisticscost.Year = entity.Year;
                    if (AbpSession.UserId != null)
                    {
                        logisticscost.CreatorUserId = AbpSession.UserId.Value;
                        logisticscost.LastModificationTime = DateTime.Now;
                        logisticscost.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    logisticscost.LastModificationTime = DateTime.Now;
                    entity = await _logisticscostRepository.InsertAsync(logisticscost);
                }
            }

        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<LogisticscostDto> UpdateAsync(LogisticscostDto input)
        {
            Logisticscost entity = await _logisticscostRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<LogisticscostDto, Logisticscost>(input, entity);
            entity = await _logisticscostRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Logisticscost, LogisticscostDto>(entity,new LogisticscostDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _logisticscostRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
