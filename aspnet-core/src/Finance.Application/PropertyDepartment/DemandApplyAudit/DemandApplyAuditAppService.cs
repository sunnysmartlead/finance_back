using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.Entering;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProjectManagement;
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
using static Finance.Ext.FriendlyRequiredAttribute;

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
        public readonly IRepository<Solution, long> _resourceSchemeTable;
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
        /// 文件管理接口
        /// </summary>
        private readonly FileCommonService _fileCommonService;
        /// <summary>
        /// 营销部录入审核
        /// </summary>
        private PriceEvaluationGetAppService _priceEvaluationAppService;


        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;

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
        /// <param name="fileCommonService"></param>
        public DemandApplyAuditAppService(IRepository<PricingTeam, long> resourcePricingTeam, IRepository<DesignSolution, long> resourceDesignScheme, IRepository<Solution, long> resourceSchemeTable, IRepository<ModelCount, long> resourceModelCount, IRepository<PriceEvaluation, long> resourcePriceEvaluation, IRepository<FileManagement, long> fileManagementRepository, WorkflowInstanceAppService workflowInstanceAppService, FileCommonService fileCommonService, PriceEvaluationGetAppService priceEvaluationAppService, IRepository<NodeInstance, long> nodeInstanceRepository)
        {
            _resourcePricingTeam = resourcePricingTeam;
            _resourceDesignScheme = resourceDesignScheme;
            _resourceSchemeTable = resourceSchemeTable;
            _resourceModelCount = resourceModelCount;
            _resourcePriceEvaluation = resourcePriceEvaluation;
            _fileManagementRepository = fileManagementRepository;
            _workflowInstanceAppService = workflowInstanceAppService;
            _fileCommonService = fileCommonService;
            _priceEvaluationAppService = priceEvaluationAppService;
            _nodeInstanceRepository = nodeInstanceRepository;
        }
        #region 快速核报价内容
        /// <summary>
        /// BOM页面枚举
        /// </summary>
        public enum BOMtype
        {
            /// <summary>
            /// 结构
            /// </summary>
            Structure = 0,
            /// <summary>
            /// 电子
            /// </summary>
            Electronics = 1
        }
        /// <summary>
        /// BOM查看的权限
        /// </summary>
        /// <param name="AuditFlowId"></param>
        /// <param name="SolutionId"></param>
        /// <param name="bOMtype"></param>
        /// <returns></returns>
        public async Task<bool> GetBOMViewPermissions(long AuditFlowId, long SolutionId, BOMtype bOMtype)
        {
            long userId = AbpSession.GetUserId();
            //结构
            if (bOMtype.Equals(BOMtype.Structure))
            {
                Solution solution = await _resourceSchemeTable.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.Id.Equals(SolutionId) && p.StructEngineerId.Equals(userId));
                return solution == null ? false : true;
            }
            //电子
            if (bOMtype.Equals(BOMtype.Electronics))
            {
                Solution solution = await _resourceSchemeTable.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.Id.Equals(SolutionId) && p.ElecEngineerId.Equals(userId));
                return solution == null ? false : true;
            }
            throw new FriendlyException("BOM查看的权限接口bOMtype参数异常!");
        }
        /// <summary>
        /// 核价需求审批快速核报价
        /// </summary>
        /// <param name="AuditFlowId"></param>
        /// <param name="QuoteAuditFlowId"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<List<SolutionIdAndQuoteSolutionId>> FastAuditEntering(long AuditFlowId, long QuoteAuditFlowId)
        {
            List<SolutionIdAndQuoteSolutionId> solutionIdAndQuoteSolutionIds = new();
            AuditEntering auditEntering = await AuditExport(QuoteAuditFlowId);
            //判断项目设计方案和方案是否一对一 如果不 则全段传值错误
            bool exists = auditEntering.SolutionTableList.All(a => auditEntering.DesignSolutionList.Any(b => b.SolutionName == a.Product));
            if (!exists)
            {
                throw new FriendlyException("设计方案的方案名称和方案的产品名称不一一对应");
            }
            // 核价团队  其中包含(核价人员以及对应完成时间)
            PricingTeam pricingTeam = ObjectMapper.Map<PricingTeam>(auditEntering.PricingTeam);
            pricingTeam.AuditFlowId = AuditFlowId;
            pricingTeam.Id = 0;
            await _resourcePricingTeam.InsertAsync(pricingTeam);
            #region 方案表
            // 营销部审核 方案表
            List<Solution> schemeTables = ObjectMapper.Map<List<Solution>>(auditEntering.SolutionTableList);
            schemeTables.Select(async p =>
            {
                p.AuditFlowId = AuditFlowId;
                p.Id = 0;
                p.Productld = await _priceEvaluationAppService.GetProductId(AuditFlowId, p.Productld);
                return p;
            }).ToList();
            schemeTables = await _resourceSchemeTable.BulkInsertAsync(schemeTables);
            #endregion
            #region 设计方案
            foreach (DesignSolutionDto design in auditEntering.DesignSolutionList)
            {
                Solution solution = schemeTables.FirstOrDefault(a => a.Product == design.SolutionName);
                if (solution != null)
                {
                    solutionIdAndQuoteSolutionIds.Add(new SolutionIdAndQuoteSolutionId() { NewSolutionId = solution.Id, QuoteSolutionId = design.SolutionId });
                    design.SolutionId = solution.Id;
                }
            }
            // 营销部审核中项目设计方案
            List<DesignSolution> designSchemes = ObjectMapper.Map<List<DesignSolution>>(auditEntering.DesignSolutionList);
            designSchemes.Select(p => { p.AuditFlowId = AuditFlowId; p.Id = 0; return p; }).ToList();
            designSchemes = await _resourceDesignScheme.BulkInsertAsync(designSchemes);
            #endregion
            return solutionIdAndQuoteSolutionIds;
        }
        #endregion
        /// <summary>
        /// 营销部审核录入
        /// </summary>
        /// <param name="auditEntering"></param>
        /// <returns></returns>
        public async Task AuditEntering(AuditEntering auditEntering)
        {
            try
            {
                if (auditEntering.Opinion != FinanceConsts.YesOrNo_No)
                {
                    //判断项目设计方案和方案是否一对一 如果不 则全段传值错误
                    bool exists = auditEntering.SolutionTableList.All(a => auditEntering.DesignSolutionList.Any(b => b.SolutionName == a.Product));
                    if (!exists)
                    {
                        throw new FriendlyException("设计方案的方案名称和方案的产品名称不一一对应");
                    }
                    //方案表
                    await _resourceSchemeTable.HardDeleteAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                    //设计方案
                    await _resourceDesignScheme.HardDeleteAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                    // 核价团队  其中包含(核价人员以及对应完成时间)
                    PricingTeam pricingTeam = ObjectMapper.Map<PricingTeam>(auditEntering.PricingTeam);
                    pricingTeam.AuditFlowId = auditEntering.AuditFlowId;
                    await _resourcePricingTeam.InsertOrUpdateAndGetIdAsync(pricingTeam);
                    #region 方案表
                    // 营销部审核 方案表
                    List<Solution> schemeTables = ObjectMapper.Map<List<Solution>>(auditEntering.SolutionTableList);
                    //判断方案名称是否有重复的            
                    bool ProducteCount = schemeTables.GroupBy(p => p.Product).Count() > 1;
                    HashSet<string> idSet = new HashSet<string>();
                    foreach (Solution person in schemeTables)
                    {
                        if (!idSet.Add(person.Product))
                        {
                            throw new FriendlyException("产品名称不能相同");
                        }
                    }
                    schemeTables.Select(p => { p.AuditFlowId = auditEntering.AuditFlowId; return p; }).ToList();
                    schemeTables = await _resourceSchemeTable.BulkInsertOrUpdateAsync(schemeTables);
                    ////现在数据库里有的数据项
                    //List<Solution> SolutionTableNow = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                    ////用户传入的数据项和数据库现有数据项的差异的ID
                    //List<long> SolutionTablDiffId = SolutionTableNow.Where(p => !schemeTables.Any(p2 => p2.Id == p.Id)).Select(p => p.Id).ToList();
                    ////删除 用户在前端删除的 数据项目
                    //await _resourceSchemeTable.DeleteAsync(p => SolutionTablDiffId.Contains(p.Id));
                    #endregion
                    #region 设计方案
                    foreach (DesignSolutionDto design in auditEntering.DesignSolutionList)
                    {
                        Solution solution = schemeTables.FirstOrDefault(a => a.Product == design.SolutionName);
                        if (solution != null)
                        {
                            design.SolutionId = solution.Id;
                        }
                    }
                    // 营销部审核中项目设计方案
                    List<DesignSolution> designSchemes = ObjectMapper.Map<List<DesignSolution>>(auditEntering.DesignSolutionList);
                    designSchemes.Select(p => { p.AuditFlowId = auditEntering.AuditFlowId; return p; }).ToList();
                    designSchemes = await _resourceDesignScheme.BulkInsertOrUpdateAsync(designSchemes);
                    ////现在数据库里有的数据项
                    //List<DesignSolution> DesignSchemeNow = await _resourceDesignScheme.GetAllListAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                    ////用户传入的数据项和数据库现有数据项的差异的ID
                    //List<long> DesignSchemeDiffId = DesignSchemeNow.Where(p => !designSchemes.Any(p2 => p2.Id == p.Id)).Select(p => p.Id).ToList();
                    ////删除 用户在前端删除的 数据项目
                    //await _resourceDesignScheme.DeleteAsync(p => DesignSchemeDiffId.Contains(p.Id));
                    #endregion
                }
                else if (auditEntering.Opinion == FinanceConsts.YesOrNo_No)
                {
                    //方案表
                    await _resourceSchemeTable.HardDeleteAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                    //设计方案
                    await _resourceDesignScheme.HardDeleteAsync(p => p.AuditFlowId.Equals(auditEntering.AuditFlowId));
                }

                #region 工作流
                //嵌入工作流
                await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                {
                    NodeInstanceId = auditEntering.NodeInstanceId,
                    FinanceDictionaryDetailId = auditEntering.Opinion,
                    Comment = auditEntering.Comment,
                });
                #endregion

                //直接上传快速核价流程的核价原因
                var list = new List<string> { FinanceConsts.EvalReason_Shj, FinanceConsts.EvalReason_Qtsclc, FinanceConsts.EvalReason_Bnnj };

                //在这里要判断：如果是直接上传核报价，就直接激活核价看板
                var node = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditEntering.AuditFlowId && p.NodeId == "主流程_核价需求录入");
                if (list.Contains(node.FinanceDictionaryDetailId))
                {
                    if (auditEntering.Opinion == FinanceConsts.YesOrNo_Yes)
                    {
                        var kanBan = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditEntering.AuditFlowId && p.NodeId == "主流程_核价看板");
                        kanBan.NodeInstanceStatus = NodeInstanceStatus.Current;

                        var shenPi = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditEntering.AuditFlowId && p.NodeId == "主流程_核价审批录入");
                        shenPi.NodeInstanceStatus = NodeInstanceStatus.Passed;

                    }
                    else if (auditEntering.Opinion == FinanceConsts.YesOrNo_No)
                    {
                        throw new FriendlyException($"快速核报价引用流程不允许退回到核价需求录入！");
                    }
                }
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
            await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
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
        public async Task<AuditEntering> AuditExport([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long AuditFlowId)
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
                List<Solution> solutionTables = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId));

                if (solutionTables.Count is not 0)
                {
                    //跟modelCount联查,modelCount中删除的也会在联查中过滤
                    //List<Solution> result = (from modelCount in modelCounts
                    //                         join solutionTable in solutionTables
                    //                         on modelCount.Id equals solutionTable.Productld into temp
                    //                         from solutionTable in temp.DefaultIfEmpty()
                    //                         where solutionTable != null
                    //                         select new Solution
                    //                         {
                    //                             Id = solutionTable.Id,
                    //                             AuditFlowId = solutionTable.AuditFlowId,
                    //                             Productld = solutionTable.Productld,
                    //                             ModuleName = modelCount.Product,
                    //                             SolutionName = solutionTable.SolutionName,
                    //                             Product = solutionTable.Product,
                    //                             IsCOB = solutionTable.IsCOB,
                    //                             ElecEngineerId = solutionTable.ElecEngineerId,
                    //                             StructEngineerId = solutionTable.StructEngineerId,
                    //                             IsFirst = solutionTable.IsFirst,
                    //                         }
                    //                               ).ToList();
                    ////退回的时候 如果 录入页面新增数据 此linq会获取
                    //List<Solution> addresult = (from modelCount in modelCounts
                    //                            join solutionTable in solutionTables
                    //                            on modelCount.Id equals solutionTable.Productld into temp
                    //                            from solutionTable in temp.DefaultIfEmpty()
                    //                            where solutionTable == null
                    //                            select new Solution
                    //                            {
                    //                                Id = 0,
                    //                                AuditFlowId = 0,
                    //                                Productld = modelCount.Id,
                    //                                ModuleName = modelCount.Product,
                    //                                SolutionName = "",
                    //                                Product = "",
                    //                                IsCOB = false,
                    //                                ElecEngineerId = 0,
                    //                                StructEngineerId = 0,
                    //                                IsFirst = false,
                    //                            }
                    //                               ).ToList();

                    //result.AddRange(addresult);
                    auditEntering.SolutionTableList = ObjectMapper.Map<List<SolutionTableDto>>(solutionTables);
                }
                else
                {

                    auditEntering.SolutionTableList = new();
                    foreach (ModelCount modelCount in modelCounts)
                    {
                        auditEntering.SolutionTableList.Add(new SolutionTableDto { Productld = modelCount.Id, ModuleName = modelCount.Product });
                    }
                }
                //auditEntering.SolutionTableList = ObjectMapper.Map<List<SolutionTableDto>>(solutionTables);
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
        /// <summary>
        /// 根据流程号和方案号下载3D爆炸图
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<FileResult> DownloadExplosives([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            // 营销部审核中项目设计方案
            DesignSolution designScheme = await _resourceDesignScheme.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
            return await _fileCommonService.DownloadFile(designScheme.FileId);
        }
    }
}
