﻿using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.Entering;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.HPSF;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.DemandApplyAudit
{
    /// <summary>
    /// 营销部录入审核
    /// </summary>
    public class DemandApplyAuditAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 核价团队  其中包含(核价人员以及对应完成时间)
        /// </summary>
        public readonly IRepository<PricingTeam, long> _resourcePricingTeam;
        /// <summary>
        /// 营销部审核中项目设计方案
        /// </summary>
        public readonly IRepository<DesignSolution, long> _resourceDesignScheme;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<SolutionTable, long> _resourceSchemeTable;
        /// <summary>
        /// 模组数量
        /// </summary>
        public readonly IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 核价表 此表是核价流程的主表，其他的核价信息，作为附表，引用此表的Id为外键。
        /// </summary>
        public readonly IRepository<PriceEvaluation, long> _resourcePriceEvaluation;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;

        /// <summary>
        /// 工作流服务
        /// </summary>
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourcePricingTeam"></param>
        /// <param name="resourceDesignScheme"></param>
        /// <param name="resourceSchemeTable"></param>
        /// <param name="resourceModelCount"></param>
        /// <param name="resourcePriceEvaluation"></param>
        /// <param name="fileManagementRepository"></param>
        /// <param name="workflowInstanceAppService"></param>
        public DemandApplyAuditAppService(IRepository<PricingTeam, long> resourcePricingTeam, IRepository<DesignSolution, long> resourceDesignScheme, IRepository<SolutionTable, long> resourceSchemeTable, IRepository<ModelCount, long> resourceModelCount, IRepository<PriceEvaluation, long> resourcePriceEvaluation, IRepository<FileManagement, long> fileManagementRepository, WorkflowInstanceAppService workflowInstanceAppService)
        {
            _resourcePricingTeam = resourcePricingTeam;
            _resourceDesignScheme = resourceDesignScheme;
            _resourceSchemeTable = resourceSchemeTable;
            _resourceModelCount = resourceModelCount;
            _resourcePriceEvaluation = resourcePriceEvaluation;
            _fileManagementRepository = fileManagementRepository;
            _workflowInstanceAppService = workflowInstanceAppService;
        }



        /// <summary>
        /// 营销部审核录入
        /// </summary>
        /// <param name="auditEntering"></param>
        /// <returns></returns>
        public async Task AuditEntering(AuditEntering auditEntering)
        {
            try
            {
                // 核价团队  其中包含(核价人员以及对应完成时间)
                PricingTeam pricingTeam = ObjectMapper.Map<PricingTeam>(auditEntering.PricingTeam);
                pricingTeam.AuditFlowId = auditEntering.AuditFlowId;
                await _resourcePricingTeam.InsertOrUpdateAndGetIdAsync(pricingTeam);
                // 营销部审核中项目设计方案
                List<DesignSolution> designSchemes = ObjectMapper.Map<List<DesignSolution>>(auditEntering.DesignSolutionList);
                designSchemes.Select(p => { p.AuditFlowId = auditEntering.AuditFlowId; return p; }).ToList();
                await _resourceDesignScheme.BulkInsertOrUpdateAsync(designSchemes);
                //现在数据库里有的数据项
                List<DesignSolution> DesignSchemeNow = await _resourceDesignScheme.GetAllListAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                //用户传入的数据项和数据库现有数据项的差异的ID
                List<long> DesignSchemeDiffId = DesignSchemeNow.Where(p => !designSchemes.Any(p2 => p2.Id == p.Id)).Select(p => p.Id).ToList();
                //删除 用户在前端删除的 数据项目
                await _resourceDesignScheme.DeleteAsync(p => DesignSchemeDiffId.Contains(p.Id));
                // 营销部审核 方案表
                List<SolutionTable> schemeTables = ObjectMapper.Map<List<SolutionTable>>(auditEntering.SolutionTableList);
                schemeTables.Select(p => { p.AuditFlowId = auditEntering.AuditFlowId; return p; }).ToList();
                await _resourceSchemeTable.BulkInsertOrUpdateAsync(schemeTables);
                //现在数据库里有的数据项
                List<SolutionTable> SolutionTableNow = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                //用户传入的数据项和数据库现有数据项的差异的ID
                List<long> SolutionTablDiffId = SolutionTableNow.Where(p => !schemeTables.Any(p2 => p2.Id == p.Id)).Select(p => p.Id).ToList();
                //删除 用户在前端删除的 数据项目
                await _resourceSchemeTable.DeleteAsync(p => SolutionTablDiffId.Contains(p.Id));

                //嵌入工作流
                await _workflowInstanceAppService.SubmitNode(new SubmitNodeInput
                {
                    NodeInstanceId = auditEntering.NodeInstanceId,
                    FinanceDictionaryDetailId = auditEntering.Opinion,
                    Comment = auditEntering.Comment,
                });
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }

        }
        /// <summary>
        /// 产品开发部审核
        /// </summary>
        /// <param name="toExamineDto"></param>
        /// <returns></returns>
        public async Task ProductDevelopmentDepartmentReview(ToExamineDto toExamineDto)
        {
            //嵌入工作流
            await _workflowInstanceAppService.SubmitNode(new SubmitNodeInput
            {
                NodeInstanceId = toExamineDto.NodeInstanceId,
                FinanceDictionaryDetailId = toExamineDto.Opinion,
                Comment = toExamineDto.Comment,
            });
        }
        /// <summary>
        /// 营销部审核输出
        /// </summary>
        /// <param name="AuditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AuditEntering> AuditExport(long AuditFlowId)
        {
            try
            {
                AuditEntering auditEntering = new();
                PricingTeam pricingTeam = await _resourcePricingTeam.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(AuditFlowId));
                //核价团队  其中包含(核价人员以及对应完成时间)
                auditEntering.PricingTeam = ObjectMapper.Map<PricingTeamDto>(pricingTeam);
                //核价要求完成时间
                PriceEvaluation priceEvaluation = await _resourcePriceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(AuditFlowId));
                if (auditEntering.PricingTeam == null) { auditEntering.PricingTeam = new(); }
                if (priceEvaluation != null) { auditEntering.PricingTeam.Deadline = priceEvaluation.Deadline; }
                // 营销部审核中项目设计方案
                List<DesignSolution> designSchemes = await _resourceDesignScheme.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId));
                auditEntering.DesignSolutionList = ObjectMapper.Map<List<DesignSolutionDto>>(designSchemes);
                foreach (DesignSolutionDto designScheme in auditEntering.DesignSolutionList)
                {
                    FileManagement fileNames = await _fileManagementRepository.FirstOrDefaultAsync(p => designScheme.FileId == p.Id);

                    if (fileNames is not null) designScheme.File = new FileUploadOutputDto { FileId = fileNames.Id, FileName = fileNames.Name, };
                }
                // 营销部审核 方案表
                //1.是否保存或者是退回过
                List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId));
                List<SolutionTable> solutionTables = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId));

                if (solutionTables.Count is not 0)
                {
                    //跟modelCount联查,modelCount中删除的也会在联查中过滤
                    List<SolutionTable> result = (from modelCount in modelCounts
                                                  join solutionTable in solutionTables
                                                  on modelCount.Id equals solutionTable.Productld into temp
                                                  from solutionTable in temp.DefaultIfEmpty()
                                                  where solutionTable != null
                                                  select new SolutionTable
                                                  {
                                                      Id = solutionTable.Id,
                                                      AuditFlowId = solutionTable.AuditFlowId,
                                                      Productld = solutionTable.Productld,
                                                      ModuleName = modelCount.Product,
                                                      SolutionName = solutionTable.SolutionName,
                                                      Product = solutionTable.Product,
                                                      IsCOB = solutionTable.IsCOB,
                                                      ElecEngineerId = solutionTable.ElecEngineerId,
                                                      StructEngineerId = solutionTable.StructEngineerId,
                                                      IsFirst = solutionTable.IsFirst,
                                                  }
                                                   ).ToList();
                    //退回的时候 如果 录入页面新增数据 此linq会获取
                    List<SolutionTable> addresult = (from modelCount in modelCounts
                                                     join solutionTable in solutionTables
                                                     on modelCount.Id equals solutionTable.Productld into temp
                                                     from solutionTable in temp.DefaultIfEmpty()
                                                     where solutionTable == null
                                                     select new SolutionTable
                                                     {
                                                         Id = 0,
                                                         AuditFlowId = 0,
                                                         Productld = modelCount.Id,
                                                         ModuleName = modelCount.Product,
                                                         SolutionName = "",
                                                         Product = "",
                                                         IsCOB = false,
                                                         ElecEngineerId = 0,
                                                         StructEngineerId = 0,
                                                         IsFirst = false,
                                                     }
                                                   ).ToList();

                    result.AddRange(addresult);
                    auditEntering.SolutionTableList = ObjectMapper.Map<List<SolutionTableDto>>(result);
                }
                else
                {

                    auditEntering.SolutionTableList = new();
                    foreach (ModelCount modelCount in modelCounts)
                    {
                        auditEntering.SolutionTableList.Add(new SolutionTableDto { Productld = modelCount.Id, ModuleName = modelCount.Product });
                    }
                }
                return auditEntering;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 下载模版
        /// </summary>
        /// <param name="designSchemeDtos"></param>
        /// <returns></returns>        
        public async Task<IActionResult> DownloadTemplate(List<DesignSolutionModel> designSchemeDtos)
        {
            var values = new List<Dictionary<string, object>>();
            foreach (DesignSolutionModel item in designSchemeDtos)
            {
                values.Add(new Dictionary<string, object> {
                    { "方案名称", item.SolutionName },
                    { "SENSOR", item.Sensor },
                    { "Serial", item.Serial },
                    { "lens", item.Lens },
                    { "ISP", item.ISP },
                    { "vcsel/LED", item.Vcsel },
                    { "MCU", item.MCU },
                    { "连接器", item.Connector },
                    { "线束", item.Harness },
                    { "支架", item.Stand },
                    { "传动结构", item.TransmissionStructure },
                    { "产品类型", item.ProductType },
                    { "工艺方案", item.ProcessProgram },
                    { "其他", item.Rests }
                });
            }
            try
            {
                var memoryStream = new MemoryStream();
                await MiniExcel.SaveAsAsync(memoryStream, values);
                return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "项目设计方案.xlsx"
                };
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<DesignSolutionDto>> ImportData(IFormFile fileName)
        {
            try
            {
                if (fileName == null || fileName.Length == 0) return new List<DesignSolutionDto>();
                using (var memoryStream = new MemoryStream())
                {
                    await fileName.CopyToAsync(memoryStream);
                    List<DesignSolutionModel> rows = memoryStream.Query<DesignSolutionModel>().ToList();
                    return ObjectMapper.Map<List<DesignSolutionDto>>(rows);
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }

        }
    }
}
