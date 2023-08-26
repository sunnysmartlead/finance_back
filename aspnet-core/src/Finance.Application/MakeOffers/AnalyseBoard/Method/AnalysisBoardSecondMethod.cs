using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Finance.DemandApplyAudit;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.NrePricing.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Newtonsoft.Json;
using test;

namespace Finance.MakeOffers.AnalyseBoard.Method;

public class AnalysisBoardSecondMethod : AbpServiceBase, ISingletonDependency
{
    /// <summary>
    /// 模组数量年份
    /// </summary>
    private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;

    protected readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

    /// <summary>
    /// 要求
    /// </summary>
    private readonly IRepository<Requirement, long> _resourceRequirement;

    /// <summary>
    /// 模组数量
    /// </summary>
    private readonly IRepository<ModelCount, long> _resourceModelCount;

    /// <summary>
    /// 产品信息表
    /// </summary>
    private readonly IRepository<ProductInformation, long> _resourceProductInformation;

    /// <summary>
    /// 财务维护 毛利率方案
    /// </summary>
    private readonly IRepository<GrossMarginForm, long> _resourceGrossMarginForm;

    /// <summary>
    /// 制造成本里计算字段参数维护
    /// </summary>
    private readonly IRepository<ManufacturingCostInfo, long> _resourceManufacturingCostInfo;

    /// <summary>
    /// 产品开发部结构BOM输入信息
    /// </summary>
    private readonly IRepository<StructureBomInfo, long> _resourceStructureBomInfo;

    private readonly IRepository<Sample, long> _sampleRepository;

    private readonly NrePricingAppService _nrePricingAppService;
    private readonly ProcessHoursEnterDeviceAppService _processHoursEnterDeviceAppService;

    public AnalysisBoardSecondMethod(IRepository<ModelCountYear, long> modelCountYear,
        IRepository<PriceEvaluation, long> priceEvaluationRepository,
        IRepository<ModelCount, long> modelCount,
        IRepository<Sample, long> sampleRepository,
        IRepository<ManufacturingCostInfo, long> manufacturingCostInfo,
        IRepository<StructureBomInfo, long> structureBomInfo,
        IRepository<Requirement, long> requirement,
        ProcessHoursEnterDeviceAppService processHoursEnterDeviceAppService,
        IRepository<ProductInformation, long> productInformation,
        NrePricingAppService nrePricingAppService,
        IRepository<GrossMarginForm, long> resourceGrossMarginForm)
    {
        _resourceRequirement = requirement;
        _sampleRepository = sampleRepository;
        _resourceModelCount = modelCount;
        _processHoursEnterDeviceAppService = processHoursEnterDeviceAppService;
        _nrePricingAppService = nrePricingAppService;
        _resourceManufacturingCostInfo = manufacturingCostInfo;
        _resourceStructureBomInfo = structureBomInfo;
        _resourceProductInformation = productInformation;
        _priceEvaluationRepository = priceEvaluationRepository;
        _resourceGrossMarginForm = resourceGrossMarginForm;
        _resourceModelCountYear = modelCountYear;
    }

    private async Task<List<int>> GetYear(long processId)
    {
        List<ModelCountYear> modelCountYears =
            await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
        List<int> yearList = modelCountYears.Select(p => p.Year).Distinct().OrderBy(p => p).ToList();
        return yearList;
    }

    /// <summary>
    /// 查询毛利率方案(查询依据 GrossMarginName)
    /// </summary>
    /// <returns></returns>
    public async Task<List<decimal>> GetGrossMargin()
    {
        GrossMarginForm price = await _resourceGrossMarginForm.FirstOrDefaultAsync(p => p.IsDefaultn);

        //毛利率
        GrossMarginDto gross = ObjectMapper.Map<GrossMarginDto>(price);
        return gross.GrossMarginPrice;
    }

    /// <summary>
    /// 年度对比
    /// </summary>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> YearDimensionalityComparison(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        YearDimensionalityComparisonSecondDto yearDimensionalityComparisonSecondDto = new();
        List<YearValue> yearValues = new();
        YearValue yearValue = new();
        yearValue.year = "2023";
        yearValue.value = 12;
        yearValues.Add(yearValue);
        yearDimensionalityComparisonSecondDto.numk = yearValues;
        return yearDimensionalityComparisonSecondDto;
    }

    /// <summary>
    /// 下载核心器件、Nre费用拆分
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<IActionResult> DownloadCoreComponentAndNre(long processId, string fileName)
    {
        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\核心器件、Nre费用拆分.xlsx";


        var value = new
        {
        };

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsByTemplateAsync(templatePath, value);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{fileName}.xlsx"
        };
    }


    /// <summary>
    /// 下载成本信息表二开
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<IActionResult> DownloadMessageSecond(long auditFlowId, string fileName)
    {
        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\新成本信息表模板.xlsx";
        //获取核价相关
        var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
        var priceEvaluationDto = ObjectMapper.Map<PriceEvaluationStartInputResult>(priceEvaluation);
        //  SOP 5年走量信息
        List<CreateRequirementDto> createRequirementDtos = priceEvaluationDto.Requirement;
//年份
        List<int> YearList = new List<int>();
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var year in YearList)
        {
            CreateRequirementDto c = createRequirementDtos.Find(p => p.Year == year);
            SopSecondModel sop = new SopSecondModel();
            sop.YearDrop = c.AnnualDeclineRate; //年将率
            sop.RebateRequest = c.AnnualRebateRequirements; // 年度返利要求
            sop.DiscountRate = c.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = c.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }

//核心部件


//NRE费用信息


//成本单价信息


        var value = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
            RecordNumber = priceEvaluationDto.Number, //记录编号           
            Versions = priceEvaluationDto.QuoteVersion, //版本
            DirectCustomerName = priceEvaluationDto.CustomerName, //直接客户名称
            TerminalCustomerName = priceEvaluationDto.TerminalName, //终端客户名称
            OfferForm = priceEvaluationDto.PriceEvalType, //报价形式
            SopTime = priceEvaluationDto.SopTime, //SOP时间
            ProjectCycle = priceEvaluationDto.ProjectCycle, //项目生命周期
            ForSale = priceEvaluationDto.SalesType, //销售类型
            modeOfTrade = priceEvaluationDto.TradeMode, //贸易方式
            PaymentMethod = priceEvaluationDto.PaymentMethod, //付款方式
            // ExchangeRate = quotationListDto.ExchangeRate, //汇率???
            Sop = Sop,
            ProjectName = priceEvaluationDto.ProjectName, //项目名称
            // Parts = partsModels,
            // NRE = nREModels,
            //  Cost = pricingModels,
        };
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsByTemplateAsync(templatePath, value);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{fileName}.xlsx"
        };


        return null;
    }

    public async Task<QuotationListDto> QuotationList(long processId)
    {
        return null;
    }

    public async Task<List<OnlySampleModel>> getSample(List<CreateSampleDto> samples,
        decimal totalcost)
    {
        List<OnlySampleModel> onlySampleModels = new();
        //样品
        foreach (var createSampleDto in samples)
        {
            OnlySampleModel onlySampleModel = new OnlySampleModel();
            onlySampleModel.Pcs = createSampleDto.Pcs; //需求量
            onlySampleModel.SampleName = createSampleDto.Name; //样品阶段名称
            onlySampleModel.Cost = totalcost; //成本：核价看板的最小梯度第一年的成本

            onlySampleModels.Add(onlySampleModel);
        }

        return onlySampleModels;
    }

    public async Task<List<AnalyseBoardNreDto>> getNre(long processId, List<Solution> solutionTables)
    {
        List<AnalyseBoardNreDto> nres = new List<AnalyseBoardNreDto>();
        decimal handPieceCostTotals = 0;
        decimal mouldInventoryTotals = 0;
        decimal productionEquipmentCostTotals = 0;
        decimal toolingCostTotals = 0;
        decimal fixtureCostTotals = 0;
        decimal qAQCDepartmentsTotals = 0;
        decimal laboratoryFeeModelsTotals = 0;
        decimal softwareTestingCostTotals = 0;
        decimal travelExpenseTotals = 0;
        decimal restsCostTotals = 0;
        List<DeviceModel> hzde = new();
        foreach (var solutionTable in solutionTables)
        {
            var solutionName = solutionTable.SolutionName;

            GetProcessHoursEntersInput getProcessHoursEntersInput = new();
            getProcessHoursEntersInput.AuditFlowId = processId;
            getProcessHoursEntersInput.SolutionId = solutionTable.Id;
            //获取相关流程方案的专用设备
            List<ProcessHoursEnterDeviceDto> deviceDtos =
                await _processHoursEnterDeviceAppService.GetListByAuditFlowIdOrSolutionId(getProcessHoursEntersInput);
            //NRE
            AnalyseBoardNreDto analyseBoardNreDto = new();
            analyseBoardNreDto.solutionName = "NRE " + solutionName;
            PricingFormDto pricingFormDto =
                await _nrePricingAppService.GetPricingFormDownload(processId, solutionTable.Id);

            List<ReturnSalesDepartmentDto> models = new List<ReturnSalesDepartmentDto>();
            ReturnSalesDepartmentDto shouban = new();
            shouban.FormName = "手板件费";
            shouban.PricingMoney = pricingFormDto.HandPieceCostTotal;
            handPieceCostTotals += pricingFormDto.HandPieceCostTotal;
            models.Add(shouban);
            ReturnSalesDepartmentDto mouju = new();
            mouju.FormName = "模具费";
            mouju.PricingMoney = pricingFormDto.MouldInventoryTotal;
            mouldInventoryTotals += pricingFormDto.MouldInventoryTotal;
            models.Add(mouju);
            ReturnSalesDepartmentDto scsb = new();
            scsb.FormName = "生产设备费";
            scsb.PricingMoney = pricingFormDto.ProductionEquipmentCostTotal;
            productionEquipmentCostTotals += pricingFormDto.ProductionEquipmentCostTotal;
            models.Add(scsb);
            ReturnSalesDepartmentDto gzf = new();
            gzf.FormName = "工装费";
            gzf.PricingMoney = pricingFormDto.ToolingCostTotal;
            toolingCostTotals += pricingFormDto.ToolingCostTotal;
            models.Add(gzf);
            ReturnSalesDepartmentDto yjf = new();
            yjf.FormName = "治具费";
            yjf.PricingMoney = pricingFormDto.FixtureCostTotal;
            fixtureCostTotals += pricingFormDto.FixtureCostTotal;
            models.Add(yjf);
            ReturnSalesDepartmentDto jjf = new();
            jjf.FormName = "检具费";
            jjf.PricingMoney = pricingFormDto.QAQCDepartmentsTotal;
            qAQCDepartmentsTotals += pricingFormDto.QAQCDepartmentsTotal;
            models.Add(jjf);
            ReturnSalesDepartmentDto syf = new();
            syf.FormName = "实验费";
            syf.PricingMoney = pricingFormDto.LaboratoryFeeModelsTotal;
            laboratoryFeeModelsTotals += pricingFormDto.LaboratoryFeeModelsTotal;
            models.Add(syf);
            ReturnSalesDepartmentDto csrjf = new();
            csrjf.FormName = "测试软件费";
            csrjf.PricingMoney = pricingFormDto.SoftwareTestingCostTotal;
            softwareTestingCostTotals += pricingFormDto.SoftwareTestingCostTotal;
            models.Add(csrjf);
            ReturnSalesDepartmentDto clf = new();
            clf.FormName = "差旅费";
            clf.PricingMoney = pricingFormDto.TravelExpenseTotal;
            travelExpenseTotals += pricingFormDto.TravelExpenseTotal;
            models.Add(clf);
            ReturnSalesDepartmentDto qtfy = new();
            qtfy.FormName = "其他费用";
            qtfy.PricingMoney = pricingFormDto.RestsCostTotal;
            restsCostTotals += pricingFormDto.RestsCostTotal;
            models.Add(qtfy);


            List<UphAndValue> UphAndValues = pricingFormDto.UphAndValues;
            foreach (var uphAndValue in UphAndValues)
            {
                if (OperateTypeCode.xtsl.Equals(uphAndValue.Uph))
                {
                    analyseBoardNreDto.numberLine = uphAndValue.Value; //线体数量
                }

                if (OperateTypeCode.gxftl.Equals(uphAndValue.Uph))
                {
                    analyseBoardNreDto.collinearAllocationRate = uphAndValue.Value; //共线分摊率
                }
            }

//获取设备
            List<DeviceModel> deviceModels = new List<DeviceModel>();
            foreach (var deviceDto in deviceDtos)
            {
                DeviceModel deviceModel = new();
                deviceModel.DeviceName = deviceDto.DeviceName;
                deviceModel.Number = deviceDto.DeviceNumber.Value;
                deviceModel.DevicePrice = deviceDto.DevicePrice.Value;
                deviceModel.equipmentMoney = deviceModel.Number * deviceModel.DevicePrice;
                deviceModels.Add(deviceModel);
                hzde.Add(deviceModel);
            }

            analyseBoardNreDto.devices = deviceModels;
            analyseBoardNreDto.models = models;
            nres.Add(analyseBoardNreDto);
        }

        AnalyseBoardNreDto analyseBoardNreDto1 = new();
        analyseBoardNreDto1.solutionName = "NRE 汇总";
        List<ReturnSalesDepartmentDto> hz = new List<ReturnSalesDepartmentDto>();
        ReturnSalesDepartmentDto shoubanhz = new();
        shoubanhz.FormName = "手板件费";
        shoubanhz.PricingMoney = handPieceCostTotals;
        hz.Add(shoubanhz);
        ReturnSalesDepartmentDto moujuhz = new();
        moujuhz.FormName = "模具费";
        moujuhz.PricingMoney = mouldInventoryTotals;

        hz.Add(moujuhz);
        ReturnSalesDepartmentDto scsbhz = new();
        scsbhz.FormName = "生产设备费";
        scsbhz.PricingMoney = productionEquipmentCostTotals;

        hz.Add(scsbhz);
        ReturnSalesDepartmentDto gzfhz = new();
        gzfhz.FormName = "工装费";
        gzfhz.PricingMoney = toolingCostTotals;
        hz.Add(gzfhz);
        ReturnSalesDepartmentDto yjfhz = new();
        yjfhz.FormName = "治具费";
        yjfhz.PricingMoney = fixtureCostTotals;
        hz.Add(yjfhz);
        ReturnSalesDepartmentDto jjfhz = new();
        jjfhz.FormName = "检具费";
        jjfhz.PricingMoney = qAQCDepartmentsTotals;
        hz.Add(jjfhz);
        ReturnSalesDepartmentDto syfhz = new();
        syfhz.FormName = "实验费";
        syfhz.PricingMoney = laboratoryFeeModelsTotals;
        hz.Add(syfhz);
        ReturnSalesDepartmentDto csrjfhz = new();
        csrjfhz.FormName = "测试软件费";
        csrjfhz.PricingMoney = softwareTestingCostTotals;
        hz.Add(csrjfhz);
        ReturnSalesDepartmentDto clfhz = new();
        clfhz.FormName = "差旅费";
        clfhz.PricingMoney = travelExpenseTotals;
        hz.Add(clfhz);
        ReturnSalesDepartmentDto qtfyhz = new();
        qtfyhz.FormName = "其他费用";
        qtfyhz.PricingMoney = restsCostTotals;
        hz.Add(qtfyhz);

        analyseBoardNreDto1.devices = hzde;
        analyseBoardNreDto1.models = hz;
        nres.Add(analyseBoardNreDto1);

        return nres;
    }

    public async Task<IActionResult> ExternalQuotation(long auditFlowId, string fileName)
    {
        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\对外报价单模板.xlsx";
        //获取核价相关
        var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
        var priceEvaluationDto = ObjectMapper.Map<PriceEvaluationStartInputResult>(priceEvaluation);
        var value = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
            RecordNumber = priceEvaluationDto.Number, //记录编号           
            Versions = priceEvaluationDto.QuoteVersion, //版本
            DirectCustomerName = priceEvaluationDto.CustomerName, //直接客户名称
            TerminalCustomerName = priceEvaluationDto.TerminalName, //终端客户名称
            OfferForm = priceEvaluationDto.PriceEvalType, //报价形式
            SopTime = priceEvaluationDto.SopTime, //SOP时间
            ProjectCycle = priceEvaluationDto.ProjectCycle, //项目生命周期
            ForSale = priceEvaluationDto.SalesType, //销售类型
            modeOfTrade = priceEvaluationDto.TradeMode, //贸易方式
            PaymentMethod = priceEvaluationDto.PaymentMethod, //付款方式
            // ExchangeRate = quotationListDto.ExchangeRate, //汇率???

            ProjectName = priceEvaluationDto.ProjectName, //项目名称
        };
        var memoryStream = new MemoryStream();

        await memoryStream.SaveAsByTemplateAsync(templatePath, value);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{fileName}.xlsx"
        };


        return null;
    }

    /// <summary>
    /// 费用实际数量
    /// </summary>     
    /// <returns></returns>
    public async Task<List<GradientGrossMarginModel>> GetActual(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult,
        List<Gradient> gradients)
    {
        List<GradientGrossMarginModel> list = new();
        foreach (var gradient in gradients)
        {
            GradientGrossMarginModel grossMarginModel = new();
            grossMarginModel.title = gradient.GradientValue.ToString() + "K/V";
            List<ItemGrossMarginModel> iitemlist = new List<ItemGrossMarginModel>();
            ItemGrossMarginModel sl = new();
            sl.item = "数量";
            sl.Client = 10;
            sl.Interior = 24;
            sl.ThisQuotation = 12;
            sl.LastRound = 11;
            iitemlist.Add(sl);
            ItemGrossMarginModel xscb = new();
            xscb.item = "销售成本";
            xscb.Client = 10;
            xscb.Interior = 24;
            xscb.ThisQuotation = 12;
            xscb.LastRound = 11;
            iitemlist.Add(xscb);
            ItemGrossMarginModel dwpjcb = new();
            dwpjcb.item = "单位平均成本";
            dwpjcb.Client = 10;
            dwpjcb.Interior = 24;
            dwpjcb.ThisQuotation = 12;
            dwpjcb.LastRound = 11;
            iitemlist.Add(dwpjcb);
            ItemGrossMarginModel xssr = new();
            xssr.item = "销售收入";
            xssr.Client = 10;
            xssr.Interior = 24;
            xssr.ThisQuotation = 12;
            xssr.LastRound = 11;
            iitemlist.Add(xssr);
            ItemGrossMarginModel pjdj = new();
            pjdj.item = "平均单价";
            pjdj.Client = 10;
            pjdj.Interior = 24;
            pjdj.ThisQuotation = 12;
            pjdj.LastRound = 11;
            iitemlist.Add(pjdj);
            ItemGrossMarginModel xsml = new();
            xsml.item = "销售毛利";
            xsml.Client = 10;
            xsml.Interior = 24;
            xsml.ThisQuotation = 12;
            xsml.LastRound = 11;
            iitemlist.Add(xsml);
            ItemGrossMarginModel mll = new();
            mll.item = "毛利率";
            mll.Client = 10;
            mll.Interior = 24;
            mll.ThisQuotation = 12;
            mll.LastRound = 11;
            iitemlist.Add(mll);
            grossMarginModel._itemGrossMarginModels = iitemlist;
            list.Add(grossMarginModel);
        }

        return list;
    }

    /// <summary>
    /// 报价毛利率测算实际数量
    /// </summary>     
    /// <returns></returns>
    public async Task<List<QuotedGrossMarginProjectModel>> GetActual(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<Solution> solutions)
    {
        List<QuotedGrossMarginProjectModel> list = new();
        List<CreateCarModelCountDto> carModelCountDtos = priceEvaluationStartInputResult.CarModelCount;
        foreach (var carModelCount in carModelCountDtos)
        {
            QuotedGrossMarginProjectModel grossMarginModel = new();
            grossMarginModel.project = "报价毛利率测算-实际数量-" + carModelCount.CarModel;
            List<GrossMargin> grossMargins = new List<GrossMargin>();
            foreach (var solution in solutions)
            {
                GrossMargin grossMargin = new();
                grossMargin.proudct = solution.SolutionName;
                grossMargin.ProductNumber = carModelCount.SingleCarProductsQuantity;
                QuotedGrossMarginSimple grossMarginSimple = new();
                decimal unprice = 1;
                TargetPrice Interior = new TargetPrice();
                Interior.Price = unprice;
                Interior.GrossMargin = 20;
                Interior.NreGrossMargin = 20;
                Interior.ClientGrossMargin = 22;
                TargetPrice Client = new TargetPrice();
                Client.Price = unprice;
                Client.GrossMargin = 20;
                Client.NreGrossMargin = 25;
                Client.ClientGrossMargin = 22;
                TargetPrice ThisQuotation = new TargetPrice();
                ThisQuotation.Price = unprice;
                ThisQuotation.GrossMargin = 20;
                ThisQuotation.NreGrossMargin = 25;
                ThisQuotation.ClientGrossMargin = 24;
                TargetPrice LastRound = new TargetPrice();
                LastRound.Price = unprice;
                LastRound.GrossMargin = 20;
                LastRound.NreGrossMargin = 25;
                LastRound.ClientGrossMargin = 21;
                grossMarginSimple.Interior = Interior; //目标价（内部）
                grossMarginSimple.Client = Client; //目标价（客户）
                grossMarginSimple.ThisQuotation = ThisQuotation; //本次报价
                grossMarginSimple.LastRound = LastRound; //上轮报价

                grossMargin.quotedGrossMarginSimple = grossMarginSimple;
                grossMargins.Add(grossMargin);
            }

            grossMarginModel.GrossMargins = grossMargins;
        }

        return list;
    }

    /// <summary>
    /// 报价毛利率测算阶梯数量
    /// </summary>     
    /// <returns></returns>
    public async Task<List<GradientQuotedGrossMarginModel>> GetstepsNum(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<Solution> solutions,
        List<Gradient> gradients)
    {
        List<GradientQuotedGrossMarginModel> list = new List<GradientQuotedGrossMarginModel>();
        List<CreateCustomerTargetPriceDto> createCustomerTargetPriceDtos =
            priceEvaluationStartInputResult.CustomerTargetPrice;
        foreach (var gradient in gradients)
        {
            foreach (var solution in solutions)
            {
                GradientQuotedGrossMarginModel grossMarginModel = new GradientQuotedGrossMarginModel();
                grossMarginModel.gradient = gradient.GradientValue + "K/Y";
                grossMarginModel.proudct = solution.SolutionName;
                var smple = createCustomerTargetPriceDtos.Where(p => p.Kv == gradient.GradientValue).First();
                decimal unprice = smple.ExchangeRate * (decimal.Parse(smple.TargetPrice)); //单价
                QuotedGrossMarginSimple quotedGrossMarginSimple = new();
                TargetPrice Interior = new TargetPrice();
                Interior.Price = unprice;
                Interior.GrossMargin = 20;
                Interior.NreGrossMargin = 20;
                Interior.ClientGrossMargin = 22;
                TargetPrice Client = new TargetPrice();
                Client.Price = unprice;
                Client.GrossMargin = 20;
                Client.NreGrossMargin = 25;
                Client.ClientGrossMargin = 22;
                TargetPrice ThisQuotation = new TargetPrice();
                ThisQuotation.Price = unprice;
                ThisQuotation.GrossMargin = 20;
                ThisQuotation.NreGrossMargin = 25;
                ThisQuotation.ClientGrossMargin = 24;
                TargetPrice LastRound = new TargetPrice();
                LastRound.Price = unprice;
                LastRound.GrossMargin = 20;
                LastRound.NreGrossMargin = 25;
                LastRound.ClientGrossMargin = 21;
                quotedGrossMarginSimple.Interior = Interior; //目标价（内部）
                quotedGrossMarginSimple.Client = Client; //目标价（客户）
                quotedGrossMarginSimple.ThisQuotation = ThisQuotation; //本次报价
                quotedGrossMarginSimple.LastRound = LastRound; //上轮报价
                grossMarginModel.QuotedGrossMarginSimple = quotedGrossMarginSimple;
                list.Add(grossMarginModel);
            }
        }

        return list;
    }

    /// <summary>
    /// 汇总分析表
    /// </summary>     
    /// <returns></returns>
    public async Task<List<PooledAnalysisModel>> GetPoolAnalysis(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<decimal> gross)
    {
        List<PooledAnalysisModel> result = new List<PooledAnalysisModel>();
        PooledAnalysisModel pooledAnalysisModelsl = new();
        pooledAnalysisModelsl.ProjectName = "数量";
        var nsum = priceEvaluationStartInputResult.ModelCount.Sum(e => e.SumQuantity);

        List<GrossMarginModel> slsl = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = nsum / (1 - (gros / 100));

            slsl.Add(grossMarginModel);
        }

        pooledAnalysisModelsl.GrossMarginList = slsl;
        result.Add(pooledAnalysisModelsl);
        PooledAnalysisModel pooledAnalysisModelxscb = new();
        pooledAnalysisModelxscb.ProjectName = "销售成本";
        decimal xscb = 1;
        List<GrossMarginModel> xscbsl = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = xscb / (1 - (gros / 100));

            xscbsl.Add(grossMarginModel);
        }

        pooledAnalysisModelxscb.GrossMarginList = xscbsl;
        result.Add(pooledAnalysisModelxscb);


        PooledAnalysisModel pooledAnalysisModelpjcb = new();
        pooledAnalysisModelpjcb.ProjectName = "单位平均成本";
        decimal pjcb = xscb / nsum;
        List<GrossMarginModel> pjcbs = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = pjcb / (1 - (gros / 100));

            pjcbs.Add(grossMarginModel);
        }

        pooledAnalysisModelpjcb.GrossMarginList = pjcbs;
        result.Add(pooledAnalysisModelpjcb);


        PooledAnalysisModel pooledAnalysisModelfl = new();
        pooledAnalysisModelfl.ProjectName = "返利后销售收入";
        decimal flxssr = 2;
        List<GrossMarginModel> fls = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = flxssr / (1 - (gros / 100));

            fls.Add(grossMarginModel);
        }

        pooledAnalysisModelfl.GrossMarginList = fls;
        result.Add(pooledAnalysisModelfl);


        PooledAnalysisModel pooledAnalysisModelpjdj = new();
        pooledAnalysisModelpjdj.ProjectName = "平均单价";
        List<GrossMarginModel> pjdj = new List<GrossMarginModel>();
        decimal dj = flxssr / nsum;
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = dj / (1 - (gros / 100));

            pjdj.Add(grossMarginModel);
        }

        pooledAnalysisModelpjdj.GrossMarginList = pjdj;
        result.Add(pooledAnalysisModelpjdj);

        PooledAnalysisModel pooledAnalysisModelpjxsml = new();
        pooledAnalysisModelpjxsml.ProjectName = "销售毛利";
        var xsmls = flxssr - xscb;
        List<GrossMarginModel> xsml = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = xsmls / (1 - (gros / 100));

            xsml.Add(grossMarginModel);
        }

        pooledAnalysisModelpjxsml.GrossMarginList = xsml;
        result.Add(pooledAnalysisModelpjxsml);


        PooledAnalysisModel pooledAnalysisModelpjxmll = new();
        pooledAnalysisModelpjxmll.ProjectName = "毛利率";
        decimal ml = xsmls / flxssr;
        List<GrossMarginModel> mlls = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = ml / (1 - (gros / 100));

            mlls.Add(grossMarginModel);
        }

        pooledAnalysisModelpjxmll.GrossMarginList = mlls;
        result.Add(pooledAnalysisModelpjxmll);
        return result;
    }

    /// <summary>
    /// 汇总分析表
    /// </summary>     
    /// <param name="GrossMarginId"></param>
    /// <param name="processId"></param>
    /// <returns></returns>
    public async Task<List<PooledAnalysisModel>> GetPooledAnalysisSecond(long? GrossMarginId, long processId)
    {
        //获取毛利率

        #region

        List<UnitPriceModel> unitPriceModels = new List<UnitPriceModel>();
        GrossMarginForm price = new();
        if (GrossMarginId.Equals(null)) //判断 前端 时候传 毛利率id  如果不传 则取默认
        {
            price = await _resourceGrossMarginForm.FirstOrDefaultAsync(p => p.IsDefaultn);
        }
        else
        {
            price = await _resourceGrossMarginForm.FirstOrDefaultAsync(p => p.Id.Equals(GrossMarginId));
        }

        //毛利率
        GrossMarginDto gross = ObjectMapper.Map<GrossMarginDto>(price);

        #endregion

        List<PooledAnalysisModel> result = new List<PooledAnalysisModel>();
        PooledAnalysisModel pooledAnalysisModel = new PooledAnalysisModel();
        List<GrossMarginModel> grossMarginModels = new List<GrossMarginModel>();
        pooledAnalysisModel.ProjectName = "数量";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            grossMarginModel.GrossMarginNumber = _resourceModelCount.GetAllList()
                .Where(p => p.AuditFlowId.Equals(processId)).Sum(p => p.ModelTotal);
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
        pooledAnalysisModel.ProjectName = "销售成本";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            grossMarginModel.GrossMarginNumber = await AllsellingCost(processId);
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
        pooledAnalysisModel.ProjectName = "单位平均成本";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            //该 毛利率的 销售数量
            decimal GrossMarginNumberSL = result[0].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            //该 毛利率的 销售成本
            decimal GrossMarginNumberxSCB = result[1].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            if (GrossMarginNumberSL is not 0.0M)
                grossMarginModel.GrossMarginNumber = GrossMarginNumberxSCB / GrossMarginNumberSL;
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
        pooledAnalysisModel.ProjectName = "返利后销售收入";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            //该 毛利率 全生命周期的 收入
            decimal ALLincome = await Allincome(processId, item);
            //该 毛利率 全生命周期的 折让
            decimal InteriorALLAllowance = await AllAllowance(processId, item);
            grossMarginModel.GrossMarginNumber = ALLincome - InteriorALLAllowance;
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
        pooledAnalysisModel.ProjectName = "平均单价";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            //该 毛利率的  销售收入
            decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            //该 毛利率的 数量
            decimal GrossMarginNumberSL = result[0].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();

            if (GrossMarginNumberSL is not 0.0M)
                grossMarginModel.GrossMarginNumber = GrossMarginNumberSR / GrossMarginNumberSL;
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });

        pooledAnalysisModel.ProjectName = "销售毛利";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            //该 毛利率的  销售收入
            decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            //该 毛利率的 销售成本
            decimal GrossMarginNumberCB = result[1].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            //该 毛利率 全生命周期的返利金额
            decimal ALLRebateMoney = await AllRebateMoney(processId, item);
            //该 毛利率 全生命周期的税费损失
            decimal ALLAddTaxRate = await AllAddTaxRate(processId, item);
            grossMarginModel.GrossMarginNumber =
                GrossMarginNumberSR - GrossMarginNumberCB - ALLRebateMoney - ALLAddTaxRate;
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });

        pooledAnalysisModel.ProjectName = "毛利率";
        grossMarginModels.Clear();
        foreach (decimal item in gross.GrossMarginPrice) //循环毛利率
        {
            GrossMarginModel grossMarginModel = new();
            grossMarginModel.GrossMargin = item;
            //该 毛利率的  销售毛利
            decimal GrossMarginNumberML = result[5].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();
            //该 毛利率的 销售收入
            decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item))
                .Select(p => p.GrossMarginNumber).First();

            if (GrossMarginNumberSR is not 0.0M)
                grossMarginModel.GrossMarginNumber = (GrossMarginNumberML / GrossMarginNumberSR) * 100;
            grossMarginModels.Add(grossMarginModel);
        }

        pooledAnalysisModel.GrossMarginList =
            JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
        result.Add(new PooledAnalysisModel()
            { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
        return result;
    }

    /// <summary>
    /// 计算 前声明周期的  销售成本
    /// </summary>
    /// <param name="processId"></param>   
    /// <returns></returns>
    private async Task<decimal> AllsellingCost(long processId)
    {
        //年份
        List<int> YearList = await GetYear(processId);
        List<long> AllProductId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id)
            .ToList();
        decimal ALLsellingCost = 0.0M;
        foreach (var Year in YearList)
        {
            foreach (var ProductId in AllProductId) //循环每一个模组
            {
                //每一个模组每年的单位成本
                decimal unitCost = 0.0M; //todo 正义接口
                ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p =>
                    p.AuditFlowId.Equals(processId) && p.ProductId.Equals(ProductId) && p.Year.Equals(Year));
                PriceEvaluationTableDto priceEvaluationTableDto = new();
                if (!string.IsNullOrWhiteSpace(modelCountYear.TableJson))
                    priceEvaluationTableDto =
                        JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                //每一个模组每年的 销售数量
                decimal Count = await SalesQuantity(processId, ProductId, Year);
                decimal prop = unitCost * Count;
                ALLsellingCost += prop;
            }
        }

        return ALLsellingCost;
    }

    /// <summary>
    /// 销售数量  终端走量*车辆的份额比率*模组配比数量 (需求录入中的模组总量)
    /// </summary>
    /// <param name="processId">流程id</param>
    /// <param name="partId">零件id</param>
    /// <param name="year">年份</param>
    /// <returns></returns>
    private async Task<decimal> SalesQuantity(long processId, long partId, int year)
    {
        ModelCountYear modelCount = new ModelCountYear();
        if (partId is 0)
        {
            modelCount =
                await _resourceModelCountYear.FirstOrDefaultAsync(p =>
                    p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
        }
        else
        {
            //销售数量=模组总量  取模组数量中的模组总量(某一个年份)
            modelCount = await _resourceModelCountYear.FirstOrDefaultAsync(p =>
                p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partId) && p.Year.Equals(year));
        }

        if (modelCount is not null)
        {
            return modelCount.Quantity;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 计算 全生命周期的  收入
    /// </summary>
    /// <returns></returns>
    private async Task<decimal> Allincome(long processId, decimal grossMargin)
    {
        //年份
        List<int> YearList = await GetYear(processId);
        List<long> AllProductId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id)
            .ToList();
        decimal ALLYearIncome = 0.0M;
        foreach (var ProductId in AllProductId) //循环每一个模组
        {
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var Year in YearList)
            {
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                //每一年的单价
                decimal ALLYearPrice =
                    await SellingPrice(processId, ProductId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = ALLYearPrice;
                //销售数量
                decimal Count = await SalesQuantity(processId, ProductId, Year);
                decimal prop = ALLYearPrice * Count;
                ALLYearIncome += prop;
            }
        }

        return ALLYearIncome;
    }

    /// <summary>
    /// 售价(单价)  sop(当年)直接取 sop年的单价    sop+N(上一年单价*(1-年将率))
    /// </summary>
    /// <param name="processId">流程id</param>
    /// <param name="partId">零件id</param>
    /// <param name="year">年份</param>
    /// <param name="lastYearUnitPrice">上一年单价 如果为0 获取今年单价</param>
    /// <param name="grossMargin">毛利率</param>
    /// <param name="YearUnitPrice">单价</param>
    /// <returns></returns>
    private async Task<decimal> SellingPrice(long processId, long partId, decimal grossMargin, int year,
        decimal lastYearUnitPrice, decimal YearUnitPrice)
    {
        if (YearUnitPrice is not 0.0M) //如果传入单价直接返回
        {
            return YearUnitPrice;
        }

        if (grossMargin is 0.0M) //如果毛利率等于0  则直接取  营销部录入(第一张表的)  产品信息的 单颗产品目标价
        {
            var prop = (from a in await _resourceModelCount.GetAllListAsync(p =>
                    p.AuditFlowId.Equals(processId) && p.Id.Equals(partId))
                join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on
                    a.Product equals b.Product
                select new
                {
                    CustomerTargetPrice = b.CustomerTargetPrice
                }).FirstOrDefault();
            return prop.CustomerTargetPrice;
        }

        if (lastYearUnitPrice is 0.0M) //如果不传上一年的单价  就证明 是获取今年的单价  
        {
            //单位价格调用正义的接口 拿到单位成本算单价 传的参数有  流程id  和零件 id
            decimal unitCost = 0M; //先写死 单位成本
            if (year is 0)
            {
                unitCost = 0M; //全模组的单位成本
                ModelCount modelCount =
                    await _resourceModelCount.FirstOrDefaultAsync(p =>
                        p.AuditFlowId.Equals(processId) && p.Id.Equals(partId));
                PriceEvaluationTableDto priceEvaluationTableDto = new();
                if (modelCount is not null && !string.IsNullOrWhiteSpace(modelCount.TableJson))
                    priceEvaluationTableDto =
                        JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCount.TableJson);
                if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
            }
            else
            {
                unitCost = 0M; //某个模组的单位成本
                ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p =>
                    p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partId) && p.Year.Equals(year));
                PriceEvaluationTableDto priceEvaluationTableDto = new();
                if (modelCountYear is not null && !string.IsNullOrWhiteSpace(modelCountYear.TableJson))
                    priceEvaluationTableDto =
                        JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
            }

            return unitCost / (1 - grossMargin / 100); //单价
        }
        else //如果传的就是 sop+N年   lastYearUnitPrice*(1-年将率)
        {
            Requirement requirement =
                await _resourceRequirement.FirstOrDefaultAsync(p =>
                    p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
            if (requirement is not null)
            {
                if (requirement == null)
                {
                    return 0.0M;
                }

                return lastYearUnitPrice * (1 - requirement.AnnualDeclineRate / 100);
            }
            else
            {
                return 0.0M;
            }
        }
    }

    /// <summary>
    /// 计算 全模组全生命周期 返利金额 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
    /// </summary>
    /// <returns></returns>
    private async Task<decimal> AllRebateMoney(long processId, decimal grossMargin)
    {
        //年份
        List<int> YearList = await GetYear(processId);
        List<long> AllProductId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id)
            .ToList();
        decimal AllRebateMoney = 0.0M;

        foreach (var ProductId in AllProductId) //循环每一个模组
        {
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var Year in YearList)
            {
                //返利比例
                decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId))
                    .Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ProductId, grossMargin, Year-1, 0.0M, 0.0M);
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ProductId, Year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ProductId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                //每一个模组的收入
                //var everyIncome = await Income(processId, ModelCount, grossMargin, Year, lastYearUnitPrice, 0.0M);
                var everyIncome = salesQuantity * sellingPrice;
                AllRebateMoney += (rebatePercentage / 100) * everyIncome;
            }
        }

        return AllRebateMoney;
    }

    /// <summary>
    /// 计算 前声明周期的  折让
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="grossMargin"></param>
    /// <returns></returns>
    private async Task<decimal> AllAllowance(long processId, decimal grossMargin)
    {
        //年份
        List<int> YearList = await GetYear(processId);
        List<long> AllProductId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id)
            .ToList();
        decimal ALLallowance = 0.0M;
        foreach (var ProductId in AllProductId) //循环每一个模组
        {
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var year in YearList)
            {
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                //每一个模组的收入
                //var everyIncome = await Income(processId, ModelCount, grossMargin, Year, lastYearUnitPrice, 0.0M);
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ProductId, year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ProductId, grossMargin, year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                var everyIncome = salesQuantity * sellingPrice;
                //折让
                ALLallowance += await Allowance(processId, year, everyIncome);
            }
        }

        return ALLallowance;
    }

    /// <summary>
    /// 折让  收入(Income()方法)*一次性销售折让率(取需求录入时候行销部录入的  一次性这让率)
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="year"></param>
    /// <param name="income"></param>
    /// <returns></returns>
    private async Task<decimal> Allowance(long processId, int year, decimal income)
    {
        //一次性这让率
        Requirement requirement =
            await _resourceRequirement.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
        var oneTimeDiscountRate = 0.0M;
        if (requirement is not null)
        {
            oneTimeDiscountRate = requirement.OneTimeDiscountRate;
        }

        return income * (oneTimeDiscountRate / 100);
    }

    /// <summary>
    /// 计算 全模组全生命周期 税费损失 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
    /// </summary>
    /// <returns></returns>
    private async Task<decimal> AllAddTaxRate(long processId, decimal grossMargin)
    {
        //年份
        List<int> YearList = await GetYear(processId);
        List<long> AllProductId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id)
            .ToList();
        decimal AddTaxRate = 0.0M;
        foreach (var ProductId in AllProductId) //循环每一个模组
        {
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var Year in YearList)
            {
                //返利比例
                decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId))
                    .Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                //每一个模组的收入
                //var everyIncome = await Income(processId, ModelCount, grossMargin, Year, lastYearUnitPrice, 0.0M);
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ProductId, Year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ProductId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                var everyIncome = salesQuantity * sellingPrice;
                //增值税率
                ManufacturingCostInfo manufacturingCostInfo =
                    await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(Year));
                if (manufacturingCostInfo is null)
                    manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year)
                        .FirstOrDefault();
                decimal addTaxRate = manufacturingCostInfo.VatRate;
                decimal TaxRate = ((rebatePercentage / 100) * everyIncome) * (addTaxRate / 100);
                AddTaxRate += TaxRate;
            }
        }

        return AddTaxRate;
    }
}