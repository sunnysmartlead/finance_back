﻿

using Abp.Application.Services;
using Abp.Domain.Repositories;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.Ext;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.LXRequirementEntry;
using Finance.PriceEval;
using Finance.ProjectManagement.Dto;
using Finance.SporadicQuotation.RequirementEntry.Dto;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// 构造函数
        /// </summary>
        /// <param name="requirementEnt"></param>
        /// <param name="dataList"></param>
        /// <param name="department"></param>
        /// <param name="fileManagement"></param>
        /// <param name="financeDictionaryDetail"></param>
        /// <param name="workflowInstanceAppService"></param>
        /// <param name="nodeInstance"></param>
        public RequirementEntryAppService(IRepository<RequirementEnt, long> requirementEnt, IRepository<DataList, long> dataList, IRepository<Department, long> department, IRepository<FileManagement, long> fileManagement, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail, WorkflowInstanceAppService workflowInstanceAppService, IRepository<NodeInstance, long> nodeInstance)
        {
            _requirementEnt = requirementEnt;
            _dataList = dataList;
            _department = department;
            _fileManagement = fileManagement;
            _financeDictionaryDetail = financeDictionaryDetail;
            _workflowInstanceAppService = workflowInstanceAppService;
            _nodeInstance = nodeInstance;
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
                    // 退回后再提交 
                    if (lXRequirementEntDto.NodeInstanceId == default) throw new FriendlyException("NodeInstanceId为空请检查!");
                    await _workflowInstanceAppService.SubmitNode(new WorkFlows.Dto.SubmitNodeInput
                    {
                        NodeInstanceId = lXRequirementEntDto.NodeInstanceId,
                        Comment = "提交",
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
        public async Task<ManagerApprovalDto> QueryLXManagerApproval(long auditFlowId)
        {
            ManagerApprovalDto managerApprovalDto = new ManagerApprovalDto();
            LXRequirementEntDto lXRequirementEntDto = await QueryLXRequirementEnt(auditFlowId);
            if (lXRequirementEntDto is not null) managerApprovalDto = ObjectMapper.Map<ManagerApprovalDto>(lXRequirementEntDto);
            return managerApprovalDto;
        }
        /// <summary>
        /// 下载生成报价审核表
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadQueryLXManagerApproval(long auditFlowId)
        {
            var values = await QueryLXManagerApproval(auditFlowId);
            MemoryStream memoryStream = new MemoryStream();
            await MiniExcel.SaveAsAsync(memoryStream, values);
            return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"报价审核表-{DateTime.Now}.xlsx"
            };
        }


    }
}
