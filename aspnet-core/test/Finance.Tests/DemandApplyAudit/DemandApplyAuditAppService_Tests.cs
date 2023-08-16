using Abp.Domain.Uow;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.DemandApplyAudit;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.DemandApplyAudit
{
    public class DemandApplyAuditAppService_Tests : FinanceTestBase
    {
        private readonly DemandApplyAuditAppService _demandApplyAuditAppService;
        public DemandApplyAuditAppService_Tests()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>().Begin();
            _demandApplyAuditAppService = Resolve<DemandApplyAuditAppService>();
        }
        [Fact]
        public async Task AuditEnteringOrAuditExport_Tests()
        {          
            AuditEntering auditEntering = new AuditEntering()
            {
                AuditFlowId = 1,
                PricingTeam = new PricingTeamDto() { AuditId = 1, EngineerId = 1, QualityBenchId = 1, EMCId = 1, ProductCostInputId = 1, ProductManageTimeId = 1, TrSubmitTime = DateTime.Now, ElecEngineerTime = DateTime.Now, StructEngineerTime = DateTime.Now, EMCTime = DateTime.Now, QualityBenchTime = DateTime.Now, ResourceElecTime = DateTime.Now, ResourceStructTime = DateTime.Now, MouldWorkHourTime = DateTime.Now, EngineerWorkHourTime = DateTime.Now, ProductManageTime = DateTime.Now, ProductCostInputTime = DateTime.Now, Deadline = DateTime.Now },
                DesignSolutionList = new List<DesignSolutionDto> { new DesignSolutionDto() {  File = new FileUploadOutputDto { FileId =0, FileName ="0", }, SolutionName = "测试流程", Sensor = "Sensor", Serial = "Serial", Lens = "Lens", ISP = "Lsp", Vcsel = "Vcsel", MCU = "MCU", Harness = "Harness", Stand = "Stand", TransmissionStructure = "TS", ProductType = "ProductType", ProcessProgram= "ProcessProgram", Rests = "Rests", FileId = 0,Id=0 } },
                SolutionTableList = new List<SolutionTableDto> { new SolutionTableDto() { ModuleName = "测试流程", SolutionName = "方案名称", Product = "产品名称", IsCOB = true, ElecEngineerId = 1, StructEngineerId = 1, IsFirst = true } },
            
            };


            await _demandApplyAuditAppService.AuditEntering(auditEntering);

            //可以查出的数据
            var AuditExportResult = await _demandApplyAuditAppService.AuditExport(1);
            //查不出的数据
            var AuditExportResultnull = await _demandApplyAuditAppService.AuditExport(999999);
        }

        [Fact]
        public async Task DownloadTemplateOrImportData_Tests()
        {



            IActionResult DownloadTemplateResult = await _demandApplyAuditAppService.DownloadTemplate(new List<DesignSolutionModel>() { new DesignSolutionModel() { SolutionName = "测试流程", Sensor = "测试流程Sensor", Serial = "测试流程Serial", Lens = "测试流程Lens", ISP = "测试流程Lsp", Vcsel = "测试流程Vcsel", MCU = "测试流程MCU", Harness = "测试流程Harness", Stand = "测试流程Stand", TransmissionStructure = "测试流程TS", ProductType = "测试流程ProductType", Rests = "测试流程Rests" } });

            IFormFile formFile = new FormFile(null, 0,0, null, "测试");
            var ImportDataResult = await _demandApplyAuditAppService.ImportData(formFile);
            var ImportDataResultnull = await _demandApplyAuditAppService.ImportData(null);
        }
    }
}
