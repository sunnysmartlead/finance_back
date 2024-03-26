using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.Audit;
using Finance.Dto;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.Ext;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.LXRequirementEntry;
using Finance.MakeOffers;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.SporadicQuotation.RequirementEntry.Dto;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.SporadicQuotation.RequirementEntry
{
    /// <summary>
    /// 零星报价的需求录入页面
    /// </summary>
    public class RequirementEntryAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 零星报价需求录入表
        /// </summary>
        private readonly IRepository<RequirementEnt, long> _requirementEnt;

        /// <summary>
        /// 数据列表
        /// </summary>
        private readonly IRepository<DataList, long> _dataList;

        /// <summary>
        /// 部门
        /// </summary>
        private readonly IRepository<Department, long> _department;
        /// <summary>
        /// 文件管理表
        /// </summary>
        private readonly IRepository<FileManagement, long> _fileManagement;

        /// <summary>
        /// 字典明细表
        /// </summary>
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetail;

        /// <summary>
        /// 工作流服务
        /// </summary>
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        /// <summary>
        /// 工作流节点实例
        /// </summary>
        private readonly IRepository<NodeInstance, long> _nodeInstance;

        /// <summary>
        /// 归档文件列表实体类
        /// </summary>
        private readonly IRepository<DownloadListSave, long> _downloadListSave;


        /// <summary>
        /// 文件管理接口
        /// </summary>
        private readonly FileCommonService _fileCommonService;
   
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requirementEnt"></param>
        /// <param name="dataList"></param>
        /// <param name="department"></param>
        /// <param name="fileManagement"></param>
        /// <param name="financeDictionaryDetail"></param>
        /// <param name="workflowInstanceAppService"></param>
        /// <param name="nodeInstance"></param>
        /// <param name="downloadListSave"></param>
        /// <param name="fileCommonService"></param>
        public RequirementEntryAppService(IRepository<RequirementEnt, long> requirementEnt, IRepository<DataList, long> dataList, IRepository<Department, long> department, IRepository<FileManagement, long> fileManagement, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail, WorkflowInstanceAppService workflowInstanceAppService, IRepository<NodeInstance, long> nodeInstance, IRepository<DownloadListSave, long> downloadListSave, FileCommonService fileCommonService)
        {
            _requirementEnt = requirementEnt;
            _dataList = dataList;
            _department = department;
            _fileManagement = fileManagement;
            _financeDictionaryDetail = financeDictionaryDetail;
            _workflowInstanceAppService = workflowInstanceAppService;
            _nodeInstance = nodeInstance;
            _downloadListSave = downloadListSave;
            _fileCommonService = fileCommonService;
        }
        /// <summary>
        /// 零星报价需求录入 保存\提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task LXRequirementEnt(LXRequirementEntDto lXRequirementEntDto)
        {
            try
            {
                #region  参数校验
                if (lXRequirementEntDto.IsSubmit)
                {

                    lXRequirementEntDto.LXDataListDtos.Select((item, indx) =>
                    {
                        List<ListName> nuber = new List<ListName>() { ListName.数量, ListName.单价, ListName.单位成本, ListName.销售收入, ListName.销售成本, ListName.销售毛利, ListName.毛利率 };
                        item.Data.Select((pitem, pindex) =>
                        {
                            if (item.ListName != ListName.备注 && string.IsNullOrEmpty(pitem))
                                throw new FriendlyException(item.ListName.GetDescription() + $"第{pindex + 1} 零件不能为空!");

                            if (nuber.Contains(item.ListName))
                            {
                                double result;
                                if (!double.TryParse(pitem, out result))
                                {
                                    throw new FriendlyException(item.ListName.GetDescription() + $"第{pindex + 1} 零件必须是数字类型!");
                                }
                            }
                            return pitem;
                        }).ToList();
                        return item;
                    }).ToList();
                }
                #endregion
                long AuditFlowId = 0;
                // 保存
                if (lXRequirementEntDto.NodeInstanceId == default && !lXRequirementEntDto.IsSubmit && lXRequirementEntDto.AuditFlowId == default)
                {
                    AuditFlowId = await _workflowInstanceAppService.StartWorkflowInstance(new WorkFlows.Dto.StartWorkflowInstanceInput
                    {
                        WorkflowId = WorkFlowCreator.LXFlowId,
                        Title = lXRequirementEntDto.Title,
                        FinanceDictionaryDetailId = FinanceConsts.Save,
                    });
                }// 提交
                else if (lXRequirementEntDto.IsSubmit && lXRequirementEntDto.AuditFlowId == default)
                {
                    AuditFlowId = await _workflowInstanceAppService.StartWorkflowInstance(new WorkFlows.Dto.StartWorkflowInstanceInput
                    {
                        WorkflowId = WorkFlowCreator.LXFlowId,
                        Title = lXRequirementEntDto.Title,
                        FinanceDictionaryDetailId = lXRequirementEntDto.Opinion
                    });
                }
                else if (lXRequirementEntDto.IsSubmit && lXRequirementEntDto.AuditFlowId != default)
                {
                    // 退回后再提交 
                    if (lXRequirementEntDto.NodeInstanceId == default) throw new FriendlyException("NodeInstanceId为空请检查!");
                    await _workflowInstanceAppService.SubmitNode(new WorkFlows.Dto.SubmitNodeInput
                    {
                        NodeInstanceId = lXRequirementEntDto.NodeInstanceId,
                        Comment = "提交",
                        FinanceDictionaryDetailId = lXRequirementEntDto.Opinion
                    });
                    AuditFlowId = lXRequirementEntDto.AuditFlowId;
                }
                else if (!lXRequirementEntDto.IsSubmit && lXRequirementEntDto.AuditFlowId != default)
                {
                    // 退回后再保存
                    if (lXRequirementEntDto.NodeInstanceId == default) throw new FriendlyException("NodeInstanceId为空请检查!");
                    await _workflowInstanceAppService.SubmitNode(new WorkFlows.Dto.SubmitNodeInput
                    {
                        NodeInstanceId = lXRequirementEntDto.NodeInstanceId,
                        Comment = "保存",
                        FinanceDictionaryDetailId = FinanceConsts.Save
                    });
                    AuditFlowId = lXRequirementEntDto.AuditFlowId;
                }

                if (AuditFlowId == 0) throw new FriendlyException("流程ID为0,请见检查!");
                lXRequirementEntDto.AuditFlowId = AuditFlowId;
                var user = await UserManager.GetUserByIdAsync(AbpSession.UserId.Value);
                lXRequirementEntDto.DraftingDepartmentId = user.DepartmentId;
                var department = await _department.FirstOrDefaultAsync(user.DepartmentId);
                if (department is not null)
                {
                    lXRequirementEntDto.DraftingCompanyId = department.CompanyId;
                }
                //保存\修改 需求录入实体类
                RequirementEnt requirementEnt = ObjectMapper.Map<RequirementEnt>(lXRequirementEntDto);
                long id = await _requirementEnt.InsertOrUpdateAndGetIdAsync(requirementEnt);
                lXRequirementEntDto?.LXDataListDtos.Select(p => p.RequirementEntryId = id).ToList();
                //保存\修改 数据列表实体类
                List<DataList> dataLists = ObjectMapper.Map<List<DataList>>(lXRequirementEntDto?.LXDataListDtos);
                await _dataList.BulkInsertOrUpdateAsync(dataLists);

            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 零星报价 查询需求录入
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LXRequirementEntDto> QueryLXRequirementEnt(long auditFlowId)
        {
            try
            {

                LXRequirementEntDto lXRequirementEntDto = new();
                RequirementEnt requirementEnt = await _requirementEnt.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId));
                if (requirementEnt is null)
                {
                    lXRequirementEntDto = new();
                }
                else
                {
                    lXRequirementEntDto = ObjectMapper.Map<LXRequirementEntDto>(requirementEnt);
                }
                List<DataList> dataLists = await _dataList.GetAllListAsync(p => p.RequirementEntryId.Equals(lXRequirementEntDto.Id));
                List<LXDataListDto> dataList = new();
                if (dataLists is null || dataLists.Count is 0)
                {
                    dataList = new();
                    List<ListName> strings = new List<ListName>() { ListName.零件名称, ListName.数量, ListName.单价, ListName.单位成本, ListName.销售收入, ListName.销售成本, ListName.销售毛利, ListName.毛利率, ListName.备注 };
                    foreach (var item in strings)
                    {
                        dataList.Add(new LXDataListDto() { ListName = item, ListNameDisplayName = item.ToString(), RequirementEntryId = lXRequirementEntDto.Id, Data = new List<string>() { "" } });
                    }
                }
                else
                {
                    dataList = ObjectMapper.Map<List<LXDataListDto>>(dataLists);
                }
                lXRequirementEntDto.LXDataListDtos = dataList;

                FileManagement fileNames = await _fileManagement.FirstOrDefaultAsync(p => lXRequirementEntDto.EnclosureId == p.Id);
                if (fileNames is not null) lXRequirementEntDto.File = new FileUploadOutputDto { FileId = fileNames.Id, FileName = fileNames.Name, };
                lXRequirementEntDto.ComponentTypeDisplayName = _financeDictionaryDetail.FirstOrDefault(p => p.Id.Equals(lXRequirementEntDto.ComponentType))?.DisplayName;
                return lXRequirementEntDto;
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 总经理审批查询
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApprovalFormDto> QueryLXManagerApproval(long auditFlowId)
        {
            ApprovalFormDto approvalFormDto = new();
            LXRequirementEntDto lXRequirementEntDto = await QueryLXRequirementEnt(auditFlowId);
            if (lXRequirementEntDto is not null)
            {
                approvalFormDto = ObjectMapper.Map<ApprovalFormDto>(lXRequirementEntDto);
                //列表行转列处理
                List<LXDataListDto> lXDataListDtos = lXRequirementEntDto?.LXDataListDtos;
                //走量
                List<decimal> TravelVolumes = lXDataListDtos.Where(p => p.ListName.Equals(ListName.数量)).SelectMany(p => p.Data).Select(p => decimal.Parse(p)).ToList();
                //成本
                List<decimal> Costs = lXDataListDtos.Where(p => p.ListName.Equals(ListName.单位成本)).SelectMany(p => p.Data).Select(p => decimal.Parse(p)).ToList();
                //价格
                List<decimal> Prices = lXDataListDtos.Where(p => p.ListName.Equals(ListName.单价)).SelectMany(p => p.Data).Select(p => decimal.Parse(p)).ToList();
                //备注
                List<string> Remarkss = lXDataListDtos.Where(p => p.ListName.Equals(ListName.备注)).SelectMany(p => p.Data).ToList();
                List<int> count = new() { TravelVolumes.Count, Costs.Count, Prices.Count, Remarkss.Count };
                if (!count.Any(p => p.Equals(TravelVolumes.Count)))
                {
                    throw new FriendlyException("列表长度不一致，无法转换成表格格式,请检查之前的数据");
                }
                for (int i = 0; i < count.Min(); i++)
                {
                    approvalFormDto.LXApprovalFormListDtos.Add(new LXApprovalFormListDto() { TravelVolume = TravelVolumes[i], Cost = Costs[i], Price = Prices[i], Remarks = Remarkss[i], GrossProfitMargin = (Prices[i] - Costs[i]) / Prices[i] });
                }
            }
            return approvalFormDto;
        }
        /// <summary>
        /// 审核报价策略 提交
        /// </summary>
        /// <param name="toExamineDtoLX"></param>
        /// <returns></returns>
        public async Task ReviewQuotationStrategy(ToExamineDtoLX toExamineDtoLX)
        {
            await _workflowInstanceAppService.SubmitNode(new() { Comment=toExamineDtoLX.Comment,NodeInstanceId=toExamineDtoLX.NodeInstanceId,FinanceDictionaryDetailId=toExamineDtoLX.Opinion });
            if(toExamineDtoLX.Opinion== FinanceConsts.YesOrNo_Yes)
            {
                NodeInstance prop = await _nodeInstance.FirstOrDefaultAsync(p => p.Id.Equals(toExamineDtoLX.NodeInstanceId));
                await Filed(prop.WorkFlowInstanceId);
            }
        }
        /// <summary>
        /// 下载生成报价审核表
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadQueryLXManagerApproval(long auditFlowId)
        {
            try
            {
                var memoryStream2 = await DownloadQueryLXManagerApprovalStream(auditFlowId);
                return new FileContentResult(memoryStream2.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"报价审核表-{DateTime.Now}.xlsx"
                };
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 下载生成报价审核表-流
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MemoryStream> DownloadQueryLXManagerApprovalStream(long auditFlowId)
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\零星件报价审核表.xlsx";
                ApprovalFormDto values = await QueryLXManagerApproval(auditFlowId);
                MemoryStream memoryStream = new MemoryStream();
                //根据模版填充数据
                await memoryStream.SaveAsByTemplateAsync(templatePath, values);
                memoryStream.Position = 0;
                XSSFWorkbook workbook = new XSSFWorkbook(memoryStream);
                workbook.GetSheetAt(0);
                ISheet sheet = workbook.GetSheetAt(0);//获取sheet
                //合并单元格
                sheet.MergedRegion(12, 12 + values.LXApprovalFormListDtos.Count, 2, 3);
                MemoryStream memoryStream2 = new MemoryStream();
                workbook.Write(memoryStream2);
                return memoryStream2;
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 归档
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task Filed(long auditFlowId)
        {
            var audit = _downloadListSave.FirstOrDefault(P => P.AuditFlowId == auditFlowId);
            if (audit is null)
            {
                LXRequirementEntDto lXRequirementEntDto = await QueryLXRequirementEnt(auditFlowId);
                //下载生成报价审核表-流
                MemoryStream downloadQueryLXManagerApprovalStream = await DownloadQueryLXManagerApprovalStream(auditFlowId);

                IFormFile fileOfferhejia = new FormFile(downloadQueryLXManagerApprovalStream, 0, downloadQueryLXManagerApprovalStream.Length, "报价审核表.xlsx", "报价审核表.xlsx");
                FileUploadOutputDto fileUploadOutputDtoOfferhejia = await _fileCommonService.UploadFile(fileOfferhejia);
                await _downloadListSave.InsertAsync(new DownloadListSave()
                {
                    AuditFlowId = auditFlowId,
                    QuoteProjectName = lXRequirementEntDto.ProjectName,
                    ProductName = "",
                    ProductId = 0,
                    FileName = "报价审核表.xlsx",
                    FilePath = fileUploadOutputDtoOfferhejia.FileUrl,
                    FileId = fileUploadOutputDtoOfferhejia.FileId
                });
            }
        }
        /// <summary>
        /// 归档文件列表
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task<List<DownloadListSave>> GetDownloadList(long auditFlowId)
        {
            return await _downloadListSave.GetAllListAsync(P => P.AuditFlowId == auditFlowId);
        }
        /// <summary>
        /// 归档文件下载  多个
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PostPigeonholeDownloads(List<long> Ids)
        {
            List<DownloadListSave> downloadListSaves = (from a in await _downloadListSave.GetAllListAsync()
                                                        join b in Ids on a.Id equals b
                                                        select a).ToList();
            var memoryStream = new MemoryStream();
            using (var zipArich = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (DownloadListSave item in downloadListSaves)
                {
                    FileStream fileStream = new FileStream(item.FilePath, FileMode.Open, FileAccess.Read); //创建文件流
                    MemoryStream memory = new MemoryStream();
                    fileStream.CopyTo(memory);
                    var entry = zipArich.CreateEntry(item.FileName);
                    using (System.IO.Stream stream = entry.Open())
                    {
                        stream.Write(memory.ToArray(), 0, fileStream.Length.To<int>());
                    }

                }
            }
            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "归档文件下载.zip" };
        }
        /// <summary>
        /// 归档文件下载 单个
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPigeonholeDownload(long Id)
        {
            try
            {
                DownloadListSave downloadListSave = await _downloadListSave.FirstOrDefaultAsync(p => p.Id.Equals(Id));
                FileStream fileStream = new FileStream(downloadListSave.FilePath, FileMode.Open, FileAccess.Read); //创建文件流
                MemoryStream memory = new MemoryStream();
                fileStream.CopyTo(memory);

                return new FileContentResult(memory.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"报价审核表-{DateTime.Now}.xlsx"
                };
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
    }
}
