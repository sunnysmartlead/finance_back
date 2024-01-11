using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Authorization.Roles;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.Entering.Dto;
using static Finance.Authorization.Roles.StaticRoleNames;
using Finance.Roles.Dto;
using Finance.Users;
using Finance.WorkFlows.Dto;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Finance.Ext;

namespace Finance.MakeOffers.AnalyseBoard;

public class AnalyseBoardSecondAppService : FinanceAppServiceBase, IAnalyseBoardSecondAppService
{
    /// <summary>
    /// 分析看板方法
    /// </summary>
    public readonly AnalysisBoardSecondMethod _analysisBoardSecondMethod;

    /// <summary>
    /// 核价梯度相关
    /// </summary>
    private readonly IRepository<Gradient, long> _gradientRepository;

    /// <summary>
    /// 报价审核表
    /// </summary>
    private readonly IRepository<AuditQuotationList, long> _financeAuditQuotationList;

    private readonly IRepository<FileManagement, long> _fileManagementRepository;

    /// <summary>
    /// 营销部审核中方案表
    /// </summary>
    public readonly IRepository<Solution, long> _resourceSchemeTable;

    /// <summary>
    /// 流程流转服务
    /// </summary>
    private readonly AuditFlowAppService _flowAppService;

    /// <summary>
    /// 文件管理接口
    /// </summary>
    private readonly FileCommonService _fileCommonService;

    private readonly IRepository<Role> _roleRepository;

    private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;

    /// <summary>
    /// 核价部分服务
    /// </summary>
    public readonly PriceEvaluationAppService _priceEvaluationAppService;

    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
    private readonly IRepository<UserRole, long> _userRoleRepository;

    /// <summary>
    /// 报价方案组合
    /// </summary>
    private readonly IRepository<SolutionQuotation, long> _solutionQutation;

    /// <summary>
    /// 核价相关
    /// </summary>
    private readonly IRepository<PriceEvaluation, long> _priceEvaluation;

    /// <summary>
    /// 归档文件列表实体类
    /// </summary>
    private readonly IRepository<DownloadListSave, long> _financeDownloadListSave;

    private readonly IUserAppService _userAppService;

    /// <summary>
    /// 报价Nre
    /// </summary>
    private readonly IRepository<NreQuotation, long> _nreQuotation;

    private readonly IRepository<AfterUpdateSumInfo, long> _afterUpdateSumInfoRepository;


    private readonly NrePricingAppService _nrePricingAppService;
    private readonly WorkflowInstanceAppService _workflowInstanceAppService;

    public AnalyseBoardSecondAppService(AnalysisBoardSecondMethod analysisBoardSecondMethod,
        IRepository<Gradient, long> gradientRepository, IRepository<AuditQuotationList, long> financeAuditQuotationList,
        IRepository<Solution, long> resourceSchemeTable, AuditFlowAppService flowAppService,
        FileCommonService fileCommonService, PriceEvaluationGetAppService priceEvaluationGetAppService,
        PriceEvaluationAppService priceEvaluationAppService,
        IUserAppService userAppService,
        IRepository<Role> roleRepository,
        IRepository<UserRole, long> userRoleRepository,
        IRepository<PriceEvaluation, long> priceEvaluation,
        IRepository<FileManagement, long> fileManagementRepository,
        IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
        IRepository<SolutionQuotation, long> solutionQutation,
        IRepository<DownloadListSave, long> financeDownloadListSave, IRepository<NreQuotation, long> nreQuotation,
        IRepository<AfterUpdateSumInfo, long> afterUpdateSumInfoRepository, NrePricingAppService nrePricingAppService,
        WorkflowInstanceAppService workflowInstanceAppService)
    {
        _analysisBoardSecondMethod = analysisBoardSecondMethod;
        _gradientRepository = gradientRepository;
        _fileManagementRepository = fileManagementRepository;
        _financeAuditQuotationList = financeAuditQuotationList;
        _resourceSchemeTable = resourceSchemeTable;
        _flowAppService = flowAppService;
        _roleRepository = roleRepository;
        _fileCommonService = fileCommonService;
        _priceEvaluation = priceEvaluation;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;
        _priceEvaluationAppService = priceEvaluationAppService;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _solutionQutation = solutionQutation;
        _userRoleRepository = userRoleRepository;
        _financeDownloadListSave = financeDownloadListSave;
        _nreQuotation = nreQuotation;
        _afterUpdateSumInfoRepository = afterUpdateSumInfoRepository;
        _nrePricingAppService = nrePricingAppService;
        _userAppService = userAppService;
        _workflowInstanceAppService = workflowInstanceAppService;
    }

    /// <summary>
    /// 查看报表分析看板  查看报价分析看板不含样品,查看报价分析看板含样品,查看报价分析看板仅含样品   ,特别注意，传入方案，方案中的moduleName不能一样
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        analyseBoardSecondInputDto.ntype = 0;
        try
        {
            return await _analysisBoardSecondMethod.PostStatementAnalysisBoardSecond(analyseBoardSecondInputDto);
        }
        catch (Exception e)
        {
            analyseBoardSecondDto.mes = e.Message;
            return analyseBoardSecondDto;
        }
    }


    /// <summary>
    /// 根据流程id,版本version 查看报表分析看板  查看报价分析看板不含样品,查看报价分析看板含样品,查看报价分析看板仅含样品  报价反馈1、报价分析看板0
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<AnalyseBoardSecondDto> getStatementAnalysisBoardSecond(long auditFlowId, int version, int ntype,int ntime)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        try
        {
            var sou = await _solutionQutation.FirstOrDefaultAsync(p =>
                p.AuditFlowId == auditFlowId && p.version == version && p.ntime == ntime);
        if (sou is null)
        {
            sou = await _solutionQutation.FirstOrDefaultAsync(p =>
                p.AuditFlowId == auditFlowId && p.version == version);
            var solutionList = JsonConvert.DeserializeObject<List<Solution>>(sou.SolutionListJson);
            AnalyseBoardSecondInputDto analyseBoardSecondInputDto = new AnalyseBoardSecondInputDto()
            {
                solutionTables = solutionList,
                auditFlowId = auditFlowId,
                ntype = ntype,
                ntime = ntime
            };
          
            return await _analysisBoardSecondMethod.PostStatementAnalysisBoardSecond(analyseBoardSecondInputDto);


        }
        else
        {
            return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond(auditFlowId, version, ntype);
        }
        }
        catch (Exception e)
        {
            analyseBoardSecondDto.mes = e.Message;
            return analyseBoardSecondDto;
        }
    }

    /// <summary>
    /// 毛利率（阶梯数量）
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GrossMarginSecondDto> PostGrossMarginForGradient(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        return await _analysisBoardSecondMethod.PostGrossMarginForGradient(yearProductBoardProcessSecondDto);
    }

    /// <summary>
    /// 毛利率（实际数量）
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GrossMarginSecondDto> PostGrossMarginForactual(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        return await _analysisBoardSecondMethod.PostGrossMarginForactual(yearProductBoardProcessSecondDto);
    }

    /// <summary>
    /// 毛利率（实际数量）齐套
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GrossMarginSecondDto> PostGrossMarginForactualQt(
        YearProductBoardProcessQtSecondDto yearProductBoardProcessSecondDto)
    {
        return await _analysisBoardSecondMethod.PostGrossMarginForactualQt(yearProductBoardProcessSecondDto);
    }

    /// <summary>
    /// 下载成本信息表二开
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PostDownloadMessageSecond(AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        try
        {
            var solutionTables = analyseBoardSecondInputDto.solutionTables;
            if (solutionTables is null || solutionTables.Count == 0)
            {
                var version = analyseBoardSecondInputDto.version;
                SolutionQuotation sol =
                    await _solutionQutation.FirstOrDefaultAsync(p =>
                        p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId && p.version == version);
                var solutionList = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);
                if (solutionList is null || solutionList.Count == 0)
                {
                    throw new FriendlyException("方案为空");

                }
                
                analyseBoardSecondInputDto.solutionTables = solutionList;

            }

            string FileName = "成本信息表下载";
            return await _analysisBoardSecondMethod.DownloadMessageSecond(analyseBoardSecondInputDto, FileName);
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 查看年度对比（阶梯数量）
    /// </summary>
    /// <param name="yearProductBoardProcessDto"></param>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparisonForGradient(
        YearProductBoardProcessSecondDto yearProductBoardProcessDto)
    {
        return await _analysisBoardSecondMethod.YearDimensionalityComparisonForGradient(yearProductBoardProcessDto);
    }

    /// <summary>
    /// 查看年度对比（实际数量）
    /// </summary>
    /// <param name="yearProductBoardProcessDto"></param>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparisonForactual(
        YearProductBoardProcessSecondDto yearProductBoardProcessDto)
    {
        return await _analysisBoardSecondMethod.PostYearDimensionalityComparisonForactual(yearProductBoardProcessDto);
    }

    /// <summary>
    /// 查看年度对比（实际数量）用于齐套
    /// </summary>
    /// <param name="yearProductBoardProcessDto"></param>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparisonForactualQt(
        YearProductBoardProcessQtSecondDto yearProductBoardProcessDto)
    {
        return await _analysisBoardSecondMethod.PostYearDimensionalityComparisonForactualQt(yearProductBoardProcessDto);
    }

    /// <summary>
    /// 查看 核心器件、Nre费用拆分
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async virtual Task<List<CoreDevice>> PostCoreComponentAndNreList(GetPriceEvaluationTableInput input)
    {
        //BOM成本
        var electronicAndStructureList = await _priceEvaluationAppService.GetBomCost(new GetBomCostInput
        {
            AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, InputCount = input.InputCount,
            SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown
        });

        List<CoreDevice> CoreDeviclist = new List<CoreDevice>();
        decimal moq = 0;
        decimal totalPCBA = 0;
        decimal totalStru = 0;
        foreach (var material in electronicAndStructureList)
        {
            moq = moq + material.MoqShareCount;
            if (material.SuperType.Equals("电子料") && material.TypeName.Equals("Sensor芯片"))
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = "Sensor芯片";
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);
            }

            else if (material.SuperType.Equals("电子料") && material.TypeName.Equals("串行芯片"))
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = "串行芯片";
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);
            }

            else if (material.SuperType.Equals("结构料") && material.TypeName.Equals("镜头"))
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = "镜头";
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);
            }

            else if (material.SuperType.Equals("电子料") && !material.TypeName.Equals("Sensor芯片") ||
                     material.SuperType.Equals("电子料") && !material.TypeName.Equals("串行芯片"))
            {
                totalPCBA = totalPCBA + material.TotalMoneyCynNoCustomerSupply;
            }

            else if (!material.SuperType.Equals("电子料") && !material.TypeName.Equals("镜头"))
            {
                totalStru = totalStru + material.TotalMoneyCynNoCustomerSupply;
            }
        }

        //PCBA
        CoreDevice PCBACoreDevice = new CoreDevice();
        PCBACoreDevice.ProjectName = "PCBA";
        PCBACoreDevice.Sum = totalPCBA;
        CoreDeviclist.Add(PCBACoreDevice);
        //结构料
        CoreDevice StruCoreDevice = new CoreDevice();
        StruCoreDevice.ProjectName = "结构件（除lens）";
        StruCoreDevice.Sum = totalStru;
        CoreDeviclist.Add(StruCoreDevice);


        ExcelPriceEvaluationTableDto ExcelPriceEvaluationTable =
            await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                Year = input.Year,
                UpDown = input.UpDown
            });
        //制造成本
        List<ManufacturingCostDto2> QualityList = ExcelPriceEvaluationTable.ManufacturingCostDto;
        foreach (ManufacturingCostDto2 Quality in QualityList)
        {
            if (Quality.CostType.Equals(CostType.Total))
            {
                CoreDevice zhiliangCoreDevice = new CoreDevice();
                zhiliangCoreDevice.ProjectName = "制造成本";
                zhiliangCoreDevice.Sum = Quality.Subtotal;
                CoreDeviclist.Add(zhiliangCoreDevice);
            }
        }

        //损耗成本
        CoreDevice lossCostCoreDevice = new CoreDevice();
        lossCostCoreDevice.ProjectName = "损耗成本";
        lossCostCoreDevice.Sum = ExcelPriceEvaluationTable.LossCount;
        CoreDeviclist.Add(lossCostCoreDevice);

        //质量成本
        CoreDevice qualityCosDevice = new CoreDevice();
        qualityCosDevice.ProjectName = "质量成本";
        qualityCosDevice.Sum = ExcelPriceEvaluationTable.QualityCost;
        CoreDeviclist.Add(qualityCosDevice);

        //物流成本
        CoreDevice logisticsCosDevice = new CoreDevice();
        logisticsCosDevice.ProjectName = "物流成本";
        logisticsCosDevice.Sum = ExcelPriceEvaluationTable.LogisticsFee;
        CoreDeviclist.Add(logisticsCosDevice);


        //其他成本

        List<OtherCostItem2> otherCostItem2s = ExcelPriceEvaluationTable.OtherCostItem2;


        foreach (OtherCostItem2 otherCostItem in otherCostItem2s)
        {
            if (otherCostItem.ItemName == "单颗成本")
            {
                CoreDevice OtherCostDevice = new CoreDevice();
                OtherCostDevice.ProjectName = "其他成本";
                OtherCostDevice.Sum = Convert.ToDecimal(otherCostItem.Total);
                CoreDeviclist.Add(OtherCostDevice);
            }
        }


        //moq
        CoreDevice moqCoreDevice = new CoreDevice();
        moqCoreDevice.ProjectName = "BOM陈本-MOQ分摊成本";
        moqCoreDevice.Sum = moq;
        CoreDeviclist.Add(moqCoreDevice);


        decimal total = 0;
        foreach (var coreDevice in CoreDeviclist)
        {
            total = total + coreDevice.Sum;
        }

        CoreDevice totalCoreDevice = new CoreDevice();
        totalCoreDevice.ProjectName = "总合计";
        totalCoreDevice.Sum = total;
        CoreDeviclist.Add(totalCoreDevice);


        return CoreDeviclist;
    }


    /// <summary>
    ///   下载核心器件、Nre费用拆分
    /// </summary>
    /// <param name="laboratoryFeeModels"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<FileResult> GetDownloadCoreComponentAndNre(GetPriceEvaluationTableInput input)
    {
        List<CoreDevice> coreDevice = await PostCoreComponentAndNreList(input);
        NreExpense2 nreExpense2 = await GetCoreNRE(input.AuditFlowId, input.SolutionId);


        //创建Workbook
        XSSFWorkbook workbook = new XSSFWorkbook();
        //创建一个sheet
        workbook.CreateSheet("sheet1");


        ISheet sheet = workbook.GetSheetAt(0); //获取sheet

        //创建头部
        IRow row000 = sheet.CreateRow(0);
        row000.CreateCell(0).SetCellValue("项目名称");
        row000.CreateCell(1).SetCellValue("单价");
        row000.CreateCell(2).SetCellValue("数量");
        row000.CreateCell(3).SetCellValue("汇率");
        row000.CreateCell(4).SetCellValue("合计");

        int i = 1;
        foreach (CoreDevice dev in coreDevice)
        {
            IRow rowI = sheet.CreateRow(i);
            rowI.CreateCell(0).SetCellValue(dev.ProjectName);
            rowI.CreateCell(1).SetCellValue(null == dev.UnitPrice ? null : dev.UnitPrice.ToString());
            rowI.CreateCell(2).SetCellValue(null == dev.Number ? null : dev.Number.ToString());
            rowI.CreateCell(3).SetCellValue(null == dev.Rate ? null : dev.Rate.ToString());
            rowI.CreateCell(4).SetCellValue(dev.Sum.ToString());
            i++;
        }

        //空一行
        IRow rowNull = sheet.CreateRow(i);
        i = i + 1;


        IRow rowI01 = sheet.CreateRow(i);
        rowI01.CreateCell(0).SetCellValue("手板件费用");
        rowI01.CreateCell(1)
            .SetCellValue(null == nreExpense2.handPieceCostTotal ? null : nreExpense2.handPieceCostTotal.ToString());

        IRow rowI02 = sheet.CreateRow(i + 1);
        rowI02.CreateCell(0).SetCellValue("模具费用");
        rowI02.CreateCell(1).SetCellValue(null == nreExpense2.mouldInventoryTotal
            ? null
            : nreExpense2.mouldInventoryTotal.ToString());

        IRow rowI03 = sheet.CreateRow(i + 2);
        rowI03.CreateCell(0).SetCellValue("工装费用");
        rowI03.CreateCell(1)
            .SetCellValue(null == nreExpense2.toolingCostTotal ? null : nreExpense2.toolingCostTotal.ToString());

        IRow rowI04 = sheet.CreateRow(i + 3);
        rowI04.CreateCell(0).SetCellValue("治具费用");
        rowI04.CreateCell(1)
            .SetCellValue(null == nreExpense2.fixtureCostTotal ? null : nreExpense2.fixtureCostTotal.ToString());

        IRow rowI05 = sheet.CreateRow(i + 4);
        rowI05.CreateCell(0).SetCellValue("检具费用");
        rowI05.CreateCell(1).SetCellValue(null == nreExpense2.qaqcDepartmentsTotal
            ? null
            : nreExpense2.qaqcDepartmentsTotal.ToString());

        IRow rowI06 = sheet.CreateRow(i + 5);
        rowI06.CreateCell(0).SetCellValue("生产设备费用");
        rowI06.CreateCell(1).SetCellValue(null == nreExpense2.productionEquipmentCostTotal
            ? null
            : nreExpense2.productionEquipmentCostTotal.ToString());

        IRow rowI07 = sheet.CreateRow(i + 6);
        rowI07.CreateCell(0).SetCellValue("专用生产设备");
        rowI07.CreateCell(1).SetCellValue(null == nreExpense2.deviceStatusSpecial
            ? null
            : nreExpense2.deviceStatusSpecial.ToString());

        IRow rowI08 = sheet.CreateRow(i + 7);
        rowI08.CreateCell(0).SetCellValue("非专用生产设备");
        rowI08.CreateCell(1)
            .SetCellValue(null == nreExpense2.deviceStatus ? null : nreExpense2.deviceStatus.ToString());

        IRow rowI09 = sheet.CreateRow(i + 8);
        rowI09.CreateCell(0).SetCellValue("实验费用");
        rowI09.CreateCell(1).SetCellValue(null == nreExpense2.laboratoryFeeModelsTotal
            ? null
            : nreExpense2.laboratoryFeeModelsTotal.ToString());

        IRow rowI10 = sheet.CreateRow(i + 9);
        rowI10.CreateCell(0).SetCellValue("测试软件费用");
        rowI10.CreateCell(1).SetCellValue(null == nreExpense2.softwareTestingCostTotal
            ? null
            : nreExpense2.softwareTestingCostTotal.ToString());

        IRow rowI11 = sheet.CreateRow(i + 10);
        rowI11.CreateCell(0).SetCellValue("差旅费");
        rowI11.CreateCell(1)
            .SetCellValue(null == nreExpense2.travelExpenseTotal ? null : nreExpense2.travelExpenseTotal.ToString());

        IRow rowI12 = sheet.CreateRow(i + 11);
        rowI12.CreateCell(0).SetCellValue("其他费用");
        rowI12.CreateCell(1)
            .SetCellValue(null == nreExpense2.restsCostTotal ? null : nreExpense2.restsCostTotal.ToString());

        IRow rowI13 = sheet.CreateRow(i + 12);
        rowI13.CreateCell(0).SetCellValue("合计");
        rowI13.CreateCell(1).SetCellValue(null == nreExpense2.sum ? null : nreExpense2.sum.ToString());

        IRow rowI14 = sheet.CreateRow(i + 13);
        rowI14.CreateCell(0).SetCellValue("备注");
        rowI14.CreateCell(1).SetCellValue(null == nreExpense2.remark ? null : nreExpense2.remark.ToString());


        //创建  占比  样式和列宽度
        //XSSFCellStyle zhanbiStyle = (XSSFCellStyle)workbook.CreateCellStyle();
        //zhanbiStyle.Alignment = HorizontalAlignment.Center; // 居中
        //sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 1).GetCell(1).CellStyle = zhanbiStyle;
        //sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 2).GetCell(1).CellStyle = zhanbiStyle;
        //sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 3).GetCell(1).CellStyle = zhanbiStyle;


        //创建头部样式和列宽度
        XSSFCellStyle titleStyle = (XSSFCellStyle)workbook.CreateCellStyle();
        titleStyle.Alignment = HorizontalAlignment.Center; // 居中
        IFont titleFont = workbook.CreateFont();
        titleFont.IsBold = true;
        titleFont.FontHeightInPoints = 12;
        titleFont.Color = HSSFColor.Black.Index; //设置字体颜色
        titleStyle.SetFont(titleFont);
        for (int c = 0; c < 5; c++)
        {
            sheet.GetRow(0).GetCell(c).CellStyle = titleStyle;
        }

        for (int c = 0; c < 13; c++)
        {
            sheet.GetRow(i + c).GetCell(0).CellStyle = titleStyle;
        }

        //列宽
        sheet.SetColumnWidth(0, 6000);
        sheet.SetColumnWidth(1, 5000);
        sheet.SetColumnWidth(4, 9000);


        //边框
        XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
        cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

        for (int m = 0; m < i - 1; m++)
        {
            for (int n = 0; n < 5; n++)
            {
                sheet.GetRow(m).GetCell(n).CellStyle = cellStyle;
            }
        }

        for (int m = i; m < 13; m++)
        {
            for (int n = 0; n < 2; n++)
            {
                sheet.GetRow(m).GetCell(n).CellStyle = cellStyle;
            }
        }


        //sheet.SetColumnWidth(1, 5000);
        //sheet.SetColumnWidth(2, 5500);
        //sheet.SetColumnWidth(3, 6000);
        //sheet.SetColumnWidth(4, 12000);
        //sheet.SetColumnWidth(5, 2000);
        //sheet.SetColumnWidth(6, 4000);
        //sheet.SetColumnWidth(7, 5000);
        //sheet.SetColumnWidth(8, 5000);


        MemoryStream ms = new MemoryStream();
        workbook.Write(ms);
        //ms.Seek(0, SeekOrigin.Begin);

        Byte[] btye2 = ms.ToArray();
        FileContentResult fileContent =
            new FileContentResult(btye2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                { FileDownloadName = "aaa.xlsx" };

        return fileContent;
    }


    /// <summary>
    /// 下载对外报价单
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> DownExternalQuotation(long Id, string FileName = "对外报价单")
    {
        try
        {
            return await _analysisBoardSecondMethod.ExternalQuotation(Id, FileName);
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    ///  根据流程号查询对外报价单的版本号
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="solutionId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetExternalQuotationNumberOfQuotations(long auditFlowId, long solutionId)
    {
        return await _analysisBoardSecondMethod.GetExternalQuotationNumberOfQuotations(auditFlowId, solutionId);
    }

    /// <summary>
    /// 对外报价单查询
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ExternalQuotationDto> GetExternalQuotation(long auditFlowId, long solutionId,
        long numberOfQuotations)
    {
        //如果是样品核价，就自动流转报价单进已办
        PriceEvaluation priceEvaluation = await _priceEvaluation
            .FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
        if (priceEvaluation.PriceEvalType == FinanceConsts.PriceEvalType_Sample)
        {
            throw new FriendlyException("当前核价类型为样品核价,报价单查询无效!");
        }

        //暂时注释 等报价看板完成之后放开
        List<SolutionQuotationDto> solutionQuotations = await GetCatalogue(auditFlowId);
        solutionQuotations = solutionQuotations.Where(p => p.Id == solutionId).ToList();
        if (solutionQuotations is null || solutionQuotations.Count == 0)
        {
            throw new FriendlyException($"solutionId:{solutionId}报价方案ID不存在");
        }

        return await _analysisBoardSecondMethod.GetExternalQuotation(auditFlowId, solutionId, numberOfQuotations);
    }

    /// <summary>
    /// 对外报价单保存/提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task SaveExternalQuotation(ExternalQuotationDto externalQuotationDto)
    {
        await externalQuotationDto.ProcessAntiShaking("SaveExternalQuotation");
        //暂时注释 等报价看板完成之后放开
        List<SolutionQuotationDto> solutionQuotations = await GetCatalogue(externalQuotationDto.AuditFlowId);
        solutionQuotations = solutionQuotations.Where(p => p.Id == externalQuotationDto.SolutionId).ToList();
        if (solutionQuotations is null || solutionQuotations.Count == 0)
        {
            throw new FriendlyException($"solutionId:{externalQuotationDto.SolutionId}报价方案ID不存在");
        }

        await _analysisBoardSecondMethod.SaveExternalQuotation(externalQuotationDto);
    }

    /// <summary>
    /// 根据流程号获取报价方案目录
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<SolutionQuotationDto>> GetCatalogue(long auditFlowId)
    {
        return await _analysisBoardSecondMethod.GeCatalogue(auditFlowId);
    }

    /// <summary>
    ///  下载对外报价单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<FileResult> DownloadExternalQuotation(long auditFlowId, long solutionId, long numberOfQuotations)
    {
        return await _analysisBoardSecondMethod.DownloadExternalQuotation(auditFlowId, solutionId, numberOfQuotations);
    }

    /// <summary>
    ///  下载对外报价单-流
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<MemoryStream> DownloadExternalQuotationStream(long auditFlowId, long solutionId,
        long numberOfQuotations)
    {
        return await _analysisBoardSecondMethod.DownloadExternalQuotationStream(auditFlowId, solutionId,
            numberOfQuotations);
    }

    /// <summary>
    /// 根据流程号获取方案
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<Solution>> GetSolution(long auditFlowId)
    {
        try
        {
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => auditFlowId == p.AuditFlowId);
            result = result.OrderBy(p => p.ModuleName).ToList();

            return result;
        }
        catch (Exception ex)
        {
            throw new FriendlyException(ex.Message);
        }
    }


    /// <summary>
    /// 报价分析看板 删除 只有报价分析看板仅保存的情况下才能删除
    /// </summary>
    /// <param name="isOfferDto">version  AuditFlowId必传</param>
    /// <returns></returns>
    public async Task PostIsOfferSecondDelete(IsDeleteSecondDto isOfferDto)
    {
        if (isOfferDto.IsFirst)
        {
            _analysisBoardSecondMethod.delete(isOfferDto.AuditFlowId, isOfferDto.version, 0);
        }
        else
        {
            throw new FriendlyException("已经提交流程不能删除");
        }
    }

    /// <summary>
    /// 流程退回到报价分析看板修改方案状态
    /// </summary>
    /// <param name="isOfferDto">version  AuditFlowId必传</param>
    /// <returns></returns>
    public async Task getUpatefirst(long auditFlowId, int ntime)
    {
        var sols = await _solutionQutation.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.ntime == ntime);
        foreach (var sol in sols)
        {
            sol.IsFirst = false;
            await _solutionQutation.UpdateAsync(sol);
        }
    }

    /// <summary>
    /// 报价分析看板 仅保存
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostIsOfferSecondOnlySave(IsOfferSecondDto isOfferDto)
    {
        isOfferDto.IsFirst = true;
        List<Solution> solutions = isOfferDto.Solutions;
        if (solutions is null || solutions.Count == 0)
        {
            throw new UserFriendlyException("方案组不能为空");
        }

        await _analysisBoardSecondMethod.deleteNoSolution(isOfferDto.AuditFlowId, isOfferDto.version, 0);
        var result =
            await _analysisBoardSecondMethod.getSameSolution(isOfferDto.AuditFlowId, isOfferDto.Solutions,
                isOfferDto.version, isOfferDto.ntime);
        if (result)
        {
            throw new FriendlyException($"此报价方案组合已存在");
        }

        isOfferDto.ntype = 0;
        //进行报价
        await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
    }

    /// <summary>
    /// 报价分析看板 保存
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostIsOfferSecond(IsOfferSecondDto isOfferDto)
    {
        if (isOfferDto.IsOffer)
        {
            List<Solution> solutions = isOfferDto.Solutions;
            if (solutions is null || solutions.Count == 0)
            {
                throw new UserFriendlyException("方案组不能为空");
            }

            await _analysisBoardSecondMethod.deleteNoSolution(isOfferDto.AuditFlowId, isOfferDto.version,
                isOfferDto.ntype);

            isOfferDto.IsFirst = false;
            var result =
                await _analysisBoardSecondMethod.getSameSolution(isOfferDto.AuditFlowId, isOfferDto.Solutions,
                    isOfferDto.version,
                    isOfferDto.ntime);
            if (result)
            {
                throw new FriendlyException($"此报价方案组合已存在");
            }
            //进行报价

            isOfferDto.IsFirst = false;
            isOfferDto.ntype = 0;
            await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
        }
        else
        {
            await this.GetDownloadListSaveNoQuotation(isOfferDto.AuditFlowId);
        }
    }


    /// <summary>
    /// 获取对应方案、梯度、单价  ntype 上轮报价0 ，本轮报价，用于报价反馈时比较值
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<SoltionGradPrice>> GetSoltionGradPriceList(long auditFlowId, int version, int ntype)
    {
        if (ntype == 1)
        {
            return await _analysisBoardSecondMethod.GetSoltionGradPriceList(auditFlowId, version, 1);
        }
        else
        {
            var spsc = await _analysisBoardSecondMethod.getSpc(auditFlowId, version);
            if (spsc is null || spsc.Count == 0)
            {
                return await _analysisBoardSecondMethod.GetSoltionGradPriceList(auditFlowId, version, 0);
            }

            return spsc;
        }
    }

    /// <summary>
    /// 报价分析看板过来的总经理报价审批界面一
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        var spc = await GetSoltionGradPriceList(auditFlowId, version, 0);
        _analysisBoardSecondMethod.UpdateOrInsert(auditFlowId, version, spc);

        return await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId, version, 0);
    }

    /// <summary>
    ///报价分析看板过来 总经理报价审批界面一保存/修改
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostManagerApprovalOfferOneSave(QuotationListSecondDto quotationListSecondDto)
    {
        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(quotationListSecondDto.AuditFlowId,
            quotationListSecondDto.version, 2, 0);
        if (au is not null)
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.UpadatefinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId,
                quotationListSecondDto.version, 1, 0);
        }
        else
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.InsertfinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId, quotationListSecondDto.version, 1, 0);
        }
    }

    /// <summary>
    /// 报价分析看板过来总经理报价审批界面二  
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 0);
    }

    /// <summary>
    /// 总经理报价审批界面二保存/修改
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostManagerApprovalOfferTwoSave(QuotationListSecondDto quotationListSecondDto)
    {
        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(quotationListSecondDto.AuditFlowId,
            quotationListSecondDto.version, 2, 0);
        if (au is not null)
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.UpadatefinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId,
                quotationListSecondDto.version, 2, 0);
        }
        else
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.InsertfinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId, quotationListSecondDto.version, 3, 0);
        }

        //嵌入工作流
        /*await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
        {
            NodeInstanceId = quotationListSecondDto.NodeInstanceId,
            FinanceDictionaryDetailId = quotationListSecondDto.Opinion,
            Comment = quotationListSecondDto.Comment,
        });*/
    }

    /// <summary>
    ///  营销部报价审批    报价审核表 下载
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetDownloadAuditQuotationList(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        try
        {
            var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlowId, version);
            int ntype = isQuotation ? 1 : 0;
            return await _analysisBoardSecondMethod.DownloadAuditQuotationList(auditFlowId, version, ntype);
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 营销部报价审批 来自报价分析看板
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<ExcelApprovalDto> GetQuotationApprovedMarketing(long auditFlowId, int version)

    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlowId, version);//是否经过报价反馈
        int ntype = isQuotation ? 1 : 0;
        return await _analysisBoardSecondMethod.getAppExcel(auditFlowId, version, ntype);
        /*
        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId, version, ntype);
    */
    }

    /// <summary>
    /// 第一次进报价审批表
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task<ExcelApprovalDto> GetQuotationApproved(long auditFlowId, int version)
    {
        var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlowId, version);
        int ntype = isQuotation ? 1 : 0;
        return await _analysisBoardSecondMethod.getAppExcel(auditFlowId, version, ntype);
    }

    /// <summary>
    /// 报价审批表保存
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostQuotationApproved(ExcelApprovalDto quotationListSecondDto)
    {
        var auditFlowId = quotationListSecondDto.auditFlowId;

        if (auditFlowId == 0)
        {
            throw new FriendlyException($"流程编号为空");
        }

        _analysisBoardSecondMethod.PostQuotationApproved(quotationListSecondDto);
    }

    /// <summary>
    /// 获取报价审批表列表
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task<List<AuditQuotationList>> GetQuotationList(long auditFlowId, int version)
    {
        return await _analysisBoardSecondMethod.GetQuotationList(auditFlowId, version);
    }


    /// <summary>
    ///  根据id下载报价审批Excel    
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetDownloadAuditQuotationExcel(long id)
    {
        try
        {
            return await _analysisBoardSecondMethod.GetQuotationExcel(id);
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 根据id获取报价审批表
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task<ExcelApprovalDto> GetQuotation(long id)
    {
        return await _analysisBoardSecondMethod.GetQuotation(id);
    }

    /// <summary>
    /// 营销部报价保存
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostQuotationApprovedMarketingSave(ExcelApprovalDto quotationListSecondDto)

    {
        var auditFlowId = quotationListSecondDto.auditFlowId;

        if (auditFlowId == 0)
        {
            throw new FriendlyException($"流程编号为空");
        }

        _analysisBoardSecondMethod.PostQuotationApproved(quotationListSecondDto);
    }


    /// <summary>
    /// 报价反馈 保存
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostQuotationFeedback(IsOfferSecondDto isOfferDto)
    {
        isOfferDto.ntype = 1;
        SolutionQuotation solutionQuotation =
            await _solutionQutation.FirstOrDefaultAsync(p =>
                p.AuditFlowId == isOfferDto.AuditFlowId && p.version == isOfferDto.version);
        if (solutionQuotation is null)
        {
            throw new UserFriendlyException("请选择报价方案");
        }

        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(solutionQuotation.SolutionListJson);

        isOfferDto.Solutions = solutionList;
        //进行报价
        await _analysisBoardSecondMethod.delete(isOfferDto.AuditFlowId, isOfferDto.version, 1);

        await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
    }


    /// <summary>
    /// 报价反馈
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<AnalyseBoardSecondDto> GetQuotationFeedback(long auditFlowId, int version,int ntime)
    {
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto = new()
        {
            auditFlowId = auditFlowId,
            ntime=ntime,
            version = version
        };
        SolutionQuotation solutionQuotation =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (solutionQuotation is null)
        {
            throw new UserFriendlyException("请选择报价方案");
        }

        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(solutionQuotation.SolutionListJson);

        analyseBoardSecondInputDto.solutionTables = solutionList;
        analyseBoardSecondInputDto.ntype = 1;
        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        try
        {
            return await _analysisBoardSecondMethod.PostStatementAnalysisBoardSecond(analyseBoardSecondInputDto);
        }
        catch (Exception e)
        {
            analyseBoardSecondDto.mes = e.Message;
            return analyseBoardSecondDto;
        }
    }

    /// <summary>
    ///报价反馈到 总经理报价审批界面一
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<ManagerApprovalOfferDto> GetQuotationFeedbacktManagerOne(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        var spc = await GetSoltionGradPriceList(auditFlowId, version, 1);
        _analysisBoardSecondMethod.UpdateOrInsert(auditFlowId, version, spc);
        var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlowId, version);
        int ntype = isQuotation ? 1 : 0;

        return await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId, version, ntype);
    }

    /// <summary>
    ///报价反馈到 总经理报价审批界面一保存/修改
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostQuotationFeedbackManagerOneSave(QuotationListSecondDto quotationListSecondDto)
    {
        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(quotationListSecondDto.AuditFlowId,
            quotationListSecondDto.version, 2, 0);
        if (au is not null)
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.UpadatefinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId,
                quotationListSecondDto.version, 1, 1);
        }
        else
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.InsertfinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId, quotationListSecondDto.version, 1, 1);
        }
    }

    /// <summary>
    ///报价反馈到 总经理报价审批界面二  
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetQuotationFeedbackManagerApprovalOfferTwo(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlowId, version);
        int ntype = isQuotation ? 1 : 0;
        return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, ntype);
    }

    /// <summary>
    /// 总经理报价审批界面二保存/修改
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostQuotationFeedbackManageTwoSave(QuotationListSecondDto quotationListSecondDto)
    {
        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(quotationListSecondDto.AuditFlowId,
            quotationListSecondDto.version, 2, 0);
        if (au is not null)
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.UpadatefinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId,
                quotationListSecondDto.version, 2, 1);
        }
        else
        {
            string content = JsonConvert.SerializeObject(quotationListSecondDto);
            await _analysisBoardSecondMethod.InsertfinanceAuditQuotationList(content,
                quotationListSecondDto.AuditFlowId, quotationListSecondDto.version, 2, 1);
        }
    }

    /// <summary>
    /// 报价反馈到中标确认
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetAcceptanceBid(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 1);
    }

    /// <summary>
    ///报价反馈到 总经理中标查看
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetBidView(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }


        return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 1);
    }

    /// <summary>
    /// 归档
    /// </summary>
    /// <param name="quotationListDto"></param>
    /// <returns></returns>
    public async Task PostAuditQuotationList(AuditQuotationListDto quotationListDto)
    {
        var IsQuotation = quotationListDto.IsQuotation;
        if (IsQuotation)//报价归档
        {
            await this.GetDownloadListSave(quotationListDto.AuditFlowId);
        }
        else
        {//不报价归档
            await this.GetDownloadListSaveNoQuotation(quotationListDto.AuditFlowId);
        }
    }

    /// <summary>
    /// 不报价归档文件列表保存
    /// </summary>
    /// <returns></returns>
    public async Task GetDownloadListSaveNoQuotation(long auditFlow)
    {
        string FileName = "";
        var audit = _financeDownloadListSave.FirstOrDefault(P => P.AuditFlowId == auditFlow);
        if (audit is null)
        {
            var priceEvaluationStartInputResult =
                await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlow);


            var time = _solutionQutation.GetAllList(p => p.AuditFlowId == auditFlow).Max(p => p.ntime);


            List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == auditFlow);
            var gradients = await _analysisBoardSecondMethod.getGradient(auditFlow);
            foreach (var solution in solutions)
            {
                foreach (var gradient in gradients)
                {
                    //核价表
                    var hejia = await _priceEvaluationGetAppService.PriceEvaluationTableDownloadStream(
                        new PriceEvaluationTableDownloadInput()
                        {
                            AuditFlowId = auditFlow,
                            SolutionId = solution.Id,
                            GradientId = gradient.Id
                        });
                    MemoryStream newhejia = new MemoryStream(hejia.ToArray());

                    FileName = "产品" + solution.ModuleName + "梯度" + gradient.GradientValue + "核价表.xlsx";
                    IFormFile fileOfferhejia = new FormFile(newhejia, 0, newhejia.Length, FileName, FileName);
                    FileUploadOutputDto fileUploadOutputDtoOfferhejia =
                        await _fileCommonService.UploadFile(fileOfferhejia);
                    //核价表的路径和名称保存到


                    await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                    {
                        AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                        ProductName = "", ProductId = 0,
                        FileName = FileName,
                        FilePath = fileUploadOutputDtoOfferhejia.FileUrl,
                        FileId = fileUploadOutputDtoOfferhejia.FileId
                    });
                }

//NRE核价表
                var nrehejia = await _priceEvaluationAppService.NreTableDownloadStream(new NreTableDownloadInput()
                {
                    AuditFlowId = auditFlow,
                    SolutionId = solution.Id
                });
                FileName = solution.ModuleName + "NRE核价表.xlsx";
                IFormFile fileOffernrehejia =
                    new FormFile(nrehejia, 0, nrehejia.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoOffernrehejia =
                    await _fileCommonService.UploadFile(fileOffernrehejia);
                //NRE核价表的路径和名称保存到


                await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                {
                    AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                    ProductName = "", ProductId = 0,
                    FileName = FileName, FilePath = fileUploadOutputDtoOffernrehejia.FileUrl,
                    FileId = fileUploadOutputDtoOffernrehejia.FileId
                });
            }
        }
    }

    /// <summary>
    /// 报价归档文件列表保存
    /// </summary>
    /// <returns></returns>
    public async Task GetDownloadListSave(long auditFlow)
    {
        string FileName = "";
        var audit = _financeDownloadListSave.FirstOrDefault(P => P.AuditFlowId == auditFlow);
        if (audit is null)
        {
            var priceEvaluationStartInputResult =
                await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlow);


            List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId == auditFlow);
            var gradients = await _analysisBoardSecondMethod.getGradient(auditFlow);
            foreach (var solution in solutions)
            {
                foreach (var gradient in gradients)
                {
                    //核价表
                    var hejia = await _priceEvaluationGetAppService.PriceEvaluationTableDownloadStream(
                        new PriceEvaluationTableDownloadInput()
                        {
                            AuditFlowId = auditFlow,
                            SolutionId = solution.Id,
                            GradientId = gradient.Id
                        });

                    MemoryStream newhejia = new MemoryStream(hejia.ToArray());

                    FileName = "产品" + solution.ModuleName + "梯度" + gradient.GradientValue + "核价表.xlsx";
                    IFormFile fileOfferhejia = new FormFile(newhejia, 0, newhejia.Length, FileName, FileName);
                    FileUploadOutputDto fileUploadOutputDtoOfferhejia =
                        await _fileCommonService.UploadFile(fileOfferhejia);
                    //核价表的路径和名称保存到


                    await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                    {
                        AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                        ProductName = "", ProductId = 0,
                        FileName = FileName,
                        FilePath = fileUploadOutputDtoOfferhejia.FileUrl,
                        FileId = fileUploadOutputDtoOfferhejia.FileId
                    });
                }

//NRE核价表
                var nrehejia = await _priceEvaluationAppService.NreTableDownloadStream(new NreTableDownloadInput()
                {
                    AuditFlowId = auditFlow,
                    SolutionId = solution.Id
                });
                FileName = solution.ModuleName + "NRE核价表.xlsx";
                IFormFile fileOffernrehejia =
                    new FormFile(nrehejia, 0, nrehejia.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoOffernrehejia =
                    await _fileCommonService.UploadFile(fileOffernrehejia);
                //NRE核价表的路径和名称保存到


                await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                {
                    AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                    ProductName = "", ProductId = 0,
                    FileName = FileName, FilePath = fileUploadOutputDtoOffernrehejia.FileUrl,
                    FileId = fileUploadOutputDtoOffernrehejia.FileId
                });
            }

            // 报价审核表 文件保存

            var time = _solutionQutation.GetAllList(p => p.AuditFlowId == auditFlow).Max(p => p.ntime);
            var sols = _solutionQutation.GetAllList(p => p.AuditFlowId == auditFlow && p.ntime == time);
            foreach (var sol in sols)
            {
                var isQuotation = await _analysisBoardSecondMethod.getQuotation(auditFlow, sol.version);
                int ntype = isQuotation ? 1 : 0;
                MemoryStream memoryStreamOffer =
                    await _analysisBoardSecondMethod.DownloadAuditQuotationListStream(auditFlow, sol.version, ntype,
                        "报价审核表" + sol.version);
                //报价审核表
                //将报价审核表保存到硬盘中
                FileName = "版本" + sol.version + "报价审核表.xlsx";
                IFormFile fileOffer = new FormFile(memoryStreamOffer, 0, memoryStreamOffer.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoOffer = await _fileCommonService.UploadFile(fileOffer);
                //报价审核表的路径和名称保存到


                await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                {
                    AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                    ProductName = "", ProductId = 0,
                    FileName = FileName, FilePath = fileUploadOutputDtoOffer.FileUrl,
                    FileId = fileUploadOutputDtoOffer.FileId
                });
                if (sol.Productld != 0) //附件上传
                {
                    await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                    {
                        AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                        ProductName = "", ProductId = 0,
                        FileName = "附件" + sol.ModuleName, FilePath = sol.Product,
                        FileId = sol.Productld
                    });
                }

                var pricetype = priceEvaluationStartInputResult.PriceEvalType;
                if ("PriceEvalType_Sample".Equals(pricetype))
                {
                    continue;
                }

                //对外报价单
                MemoryStream dwbjd =
                    await _analysisBoardSecondMethod.DownloadExternalQuotationStream(auditFlow, sol.Id,
                        sol.version);
                FileName = sol.version + "对外报价单.xlsx";
                IFormFile fileOfferdwbj = new FormFile(dwbjd, 0, memoryStreamOffer.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoOfferdwbj =
                    await _fileCommonService.UploadFile(fileOfferdwbj);
                //对外报价单的路径和名称保存到
                await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                {
                    AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                    ProductName = "", ProductId = 0,
                    FileName = FileName, FilePath = fileUploadOutputDtoOfferdwbj.FileUrl,
                    FileId = fileUploadOutputDtoOfferdwbj.FileId
                });
            }
        }
    }

    /// <summary>
    /// 归档文件列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<PigeonholeDownloadTableModel>> GetDownloadList(long? auditFlowId)
    {
        ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());
        List<DownloadListSave> downloadListSaves = new();

        List<PigeonholeDownloadTableModel> pigeonholeDownloadTableModels = new();

        List<RoleDto> generalManager = Roles.Items.Where(p => p.Name.Equals(Host.GeneralManager)).ToList(); //总经理
        if (generalManager.Count is not 0)
        {
            if (auditFlowId is not null)
            {
                downloadListSaves =
                    await _financeDownloadListSave.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            }
            else
            {
                downloadListSaves = await _financeDownloadListSave.GetAllListAsync();
            }
        }
        else
        {
            var userid = AbpSession.GetUserId();
            var price = await _priceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            var ProjectManager = price.ProjectManager; //相关项目经理
            var CreatorUserId = price.CreatorUserId; //相关业务员  
            //只有相关人员和总经理才能看相关文档
            var HJ = await _roleRepository.GetAllListAsync(p =>
                p.Name == StaticRoleNames.Host.FinanceTableAdmin
            );
            var HJuserIds = await _userRoleRepository.GetAll().Where(p => HJ.Select(p => p.Id).Contains(p.RoleId))
                .Select(p => p.UserId).ToListAsync();
            //财务部-核价表归档管理员 
            if (HJuserIds.Contains(userid) || ProjectManager == userid)
            {
                if (auditFlowId is not null)
                {
                    downloadListSaves.AddRange(await _financeDownloadListSave.GetAllListAsync(p =>
                        p.AuditFlowId.Equals(auditFlowId) && p.FileName.Contains("核价表")));
                }
                else
                {
                    downloadListSaves.AddRange(
                        await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("核价表")));
                }
            }

            //只有相关人员和总经理才能看相关文档
            var bj = await _roleRepository.GetAllListAsync(p =>
                p.Name == StaticRoleNames.Host.EvalTableAdmin
            );
            var bjuserIds = await _userRoleRepository.GetAll().Where(p => bj.Select(p => p.Id).Contains(p.RoleId))
                .Select(p => p.UserId).ToListAsync();
            //只有相关人员和总经理才能看相关文档

            //营销部-业务员
            if (bjuserIds.Contains(userid) || CreatorUserId == userid)
            {
                if (auditFlowId is not null)
                {
                    downloadListSaves.AddRange(await _financeDownloadListSave.GetAllListAsync(p =>
                        p.AuditFlowId.Equals(auditFlowId) && p.FileName.Contains("报价审核表")));
                    downloadListSaves.AddRange(await _financeDownloadListSave.GetAllListAsync(p =>
                        p.AuditFlowId.Equals(auditFlowId) && p.FileName.Contains("附件")));
                }
                else
                {
                    downloadListSaves.AddRange(
                        await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("报价审核表")));
                    downloadListSaves.AddRange(
                        await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("附件")));
                }
            }

            var bjd = await _roleRepository.GetAllListAsync(p =>
                p.Name == StaticRoleNames.Host.Bjdgdgly
            );
            var bjduserIds = await _userRoleRepository.GetAll().Where(p => bjd.Select(p => p.Id).Contains(p.RoleId))
                .Select(p => p.UserId).ToListAsync();
            if (bjduserIds.Contains(userid) || CreatorUserId == userid)
            {
                if (auditFlowId is not null)
                {
                    downloadListSaves.AddRange(await _financeDownloadListSave.GetAllListAsync(p =>
                        p.AuditFlowId.Equals(auditFlowId) && p.FileName.Contains("报价单")));
                }
                else
                {
                    downloadListSaves.AddRange(
                        await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("报价单")));
                }
            }
        }


        foreach (DownloadListSave item in downloadListSaves)
        {
            PigeonholeDownloadTableModel pigeonholeDownloadTableModel = new();
            pigeonholeDownloadTableModel.DownloadListSaveId = item.Id; // 归档文件列表id
            pigeonholeDownloadTableModel.FileName = item.FileName; // 归档文件名称
            pigeonholeDownloadTableModel.ProductName = item.ProductName; // 零件名称
            pigeonholeDownloadTableModel.QuoteProjectName = item.QuoteProjectName; // 核报价项目名称
            pigeonholeDownloadTableModels.Add(pigeonholeDownloadTableModel);
        }


        return pigeonholeDownloadTableModels;
    }

    /// <summary>
    /// 归档文件下载 传 id组成的list
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PostPigeonholeDownload(List<long> DownloadListSaveIds)
    {
        List<DownloadListSave> downloadListSaves = (from a in await _financeDownloadListSave.GetAllListAsync()
            join b in DownloadListSaveIds on a.Id equals b
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

        return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
            { FileDownloadName = "归档文件下载.zip" };
    }

    /// <summary>
    /// 用于对外报价产品清单
    /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    ///  <param name="version">0报价看板数据，1报价反馈数据</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<ProductDto>> GetProductList(long auditFlowId, int version, int ntime, int type)
    {
        return await _analysisBoardSecondMethod.GetProductList(auditFlowId, version, ntime, type);
    }

    /// <summary>
    /// 用于对外报价NRE清单
    ///<param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotationNreDto>> GetNREList(long auditFlowId, int version, int time, int type)
    {
        return await _analysisBoardSecondMethod.GetNREList(auditFlowId, version, time, type);
    }

    /// <summary>
    /// 核心器件NRE部分接口整合
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="solutionId"></param>
    /// <returns></returns>
    public async Task<NreExpense2> GetCoreNRE(long auditFlowId, long solutionId)
    {
        PricingFormDto pricingFormDto =
            await _nrePricingAppService.GetPricingFormDownload(auditFlowId, solutionId);
        decimal deviceStatusSpecial = 0m;
        foreach (var ProductionEquipment in pricingFormDto.ProductionEquipmentCost)
        {
            if (ProductionEquipment.DeviceStatus == "Sbzt_Zy")

            {
                deviceStatusSpecial = deviceStatusSpecial + ProductionEquipment.Cost;
            }
        }

        NreExpense2 nreExpense = new()
        {
            handPieceCostTotal = pricingFormDto.HandPieceCostTotal,
            mouldInventoryTotal = pricingFormDto.MouldInventoryTotal,
            toolingCostTotal = pricingFormDto.ToolingCostTotal,
            fixtureCostTotal = pricingFormDto.FixtureCostTotal,
            qaqcDepartmentsTotal = pricingFormDto.QAQCDepartmentsTotal,
            productionEquipmentCostTotal = pricingFormDto.ProductionEquipmentCostTotal,
            deviceStatusSpecial = deviceStatusSpecial,
            deviceStatus = pricingFormDto.ProductionEquipmentCostTotal - deviceStatusSpecial,
            laboratoryFeeModelsTotal = pricingFormDto.LaboratoryFeeModelsTotal,
            softwareTestingCostTotal = pricingFormDto.SoftwareTestingCostTotal,
            travelExpenseTotal = pricingFormDto.TravelExpenseTotal,
            restsCostTotal = pricingFormDto.RestsCostTotal,
            sum = pricingFormDto.HandPieceCostTotal + pricingFormDto.MouldInventoryTotal +
                  pricingFormDto.ToolingCostTotal + pricingFormDto.FixtureCostTotal +
                  pricingFormDto.QAQCDepartmentsTotal + pricingFormDto.ProductionEquipmentCostTotal +
                  pricingFormDto.LaboratoryFeeModelsTotal + pricingFormDto.SoftwareTestingCostTotal +
                  pricingFormDto.TravelExpenseTotal + pricingFormDto.RestsCostTotal,
        };
        return nreExpense;
    }


    /// <summary>
    /// 上传附件
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task<FileUploadOutputDto> UploadFile(IFormFile file)
    {
        FileUploadOutputDto fileUploadOutput = await _fileCommonService.UploadFile(file);
        return fileUploadOutput;
    }
}