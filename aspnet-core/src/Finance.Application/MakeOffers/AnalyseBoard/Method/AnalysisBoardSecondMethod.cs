using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.ProductDevelopment.Dto;
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
    /// 核价梯度相关
    /// </summary>
    private readonly IRepository<Gradient, long> _gradientRepository;

    private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;

    /// <summary>
    /// 产品信息表
    /// </summary>
    private readonly IRepository<ProductInformation, long> _resourceProductInformation;

    /// <summary>
    /// 财务维护 毛利率方案
    /// </summary>
    private readonly IRepository<GrossMarginForm, long> _resourceGrossMarginForm;

    /// <summary>
    /// 核价表需求表
    /// </summary>
    private readonly IRepository<PriceEvaluation, long> _resourcePriceEvaluation;

    /// <summary>
    /// 制造成本里计算字段参数维护
    /// </summary>
    private readonly IRepository<ManufacturingCostInfo, long> _resourceManufacturingCostInfo;

    /// <summary>
    /// 产品开发部结构BOM输入信息
    /// </summary>
    private readonly IRepository<StructureBomInfo, long> _resourceStructureBomInfo;

    /// <summary>
    /// 报价设备
    /// </summary>
    private readonly IRepository<DeviceQuotation, long> _deviceQuotation;

    /// <summary>
    /// 报价方案
    /// </summary>
    private readonly IRepository<SolutionQuotation, long> _solutionQutation;

    /// <summary>
    /// 字典明细表
    /// </summary>
    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;


    /// <summary>
    /// Nre
    /// </summary>
    private readonly IRepository<NreQuotation, long> _nreQuotation;

    /// <summary>
    /// 样品阶段
    /// </summary>
    private readonly IRepository<SampleQuotation, long> _sampleQuotation;

    private readonly IRepository<Sample, long> _sampleRepository;

    private readonly NrePricingAppService _nrePricingAppService;
    public readonly PriceEvaluationAppService _priceEvaluationAppService;

    /// <summary>
    /// 电子BOM录入接口类
    /// </summary>
    public readonly ElectronicBomAppService _electronicBomAppService;

    /// <summary>
    /// 汇率录入表
    /// </summary>
    private readonly IRepository<ExchangeRate, long> _resourceExchangeRate;

    /// <summary>
    /// 结构BOM录入接口类
    /// </summary>
    public readonly StructionBomAppService _structionBomAppService;

    /// <summary>
    /// 审批流程主表
    /// </summary>
    private readonly IRepository<AuditFlow, long> _resourceAuditFlow;

    private readonly ProcessHoursEnterDeviceAppService _processHoursEnterDeviceAppService;

    public AnalysisBoardSecondMethod(IRepository<ModelCountYear, long> modelCountYear,
        IRepository<PriceEvaluation, long> priceEvaluationRepository,
        IRepository<ModelCount, long> modelCount,
        IRepository<DeviceQuotation, long> deviceQuotation,
        PriceEvaluationGetAppService priceEvaluationGetAppService,
        IRepository<SampleQuotation, long> sampleQuotation,
        IRepository<NreQuotation, long> nreQuotation,
        IRepository<SolutionQuotation, long> solutionQutation,
        PriceEvaluationAppService priceEvaluationAppService,
        IRepository<ExchangeRate, long> resourceExchangeRate,
        IRepository<PriceEvaluation, long> resourcePriceEvaluation,
        IRepository<AuditFlow, long> resourceAuditFlow,
        IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
        ElectronicBomAppService electronicBomAppService,
        StructionBomAppService structionBomAppService,
        IRepository<Sample, long> sampleRepository,
        IRepository<Gradient, long> gradientRepository,
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
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _priceEvaluationAppService = priceEvaluationAppService;
        _resourceAuditFlow = resourceAuditFlow;
        _solutionQutation = solutionQutation;
        _resourcePriceEvaluation = resourcePriceEvaluation;
        _gradientRepository = gradientRepository;
        _deviceQuotation = deviceQuotation;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;

        _sampleQuotation = sampleQuotation;
        _nreQuotation = nreQuotation;
        _resourceExchangeRate = resourceExchangeRate;
        _resourceModelCount = modelCount;
        _processHoursEnterDeviceAppService = processHoursEnterDeviceAppService;
        _nrePricingAppService = nrePricingAppService;
        _electronicBomAppService = electronicBomAppService;
        _structionBomAppService = structionBomAppService;
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
    public async Task<IActionResult> DownloadMessageSecond(AnalyseBoardSecondInputDto analyseBoardSecondInputDto,
        string fileName)
    {
        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\新成本信息表模板.xlsx";
        //获取核价相关
        var auditFlowId = analyseBoardSecondInputDto.auditFlowId;
        //获取方案
        List<Solution> Solutions = analyseBoardSecondInputDto.solutionTables;
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(analyseBoardSecondInputDto.auditFlowId);


        //  SOP 5年走量信息
        List<CreateRequirementDto> createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        List<CreateCarModelCountDto> cars = priceEvaluationStartInputResult.CarModelCount;
        List<CreateCarModelCountYearDto> yearDtos = (from car in cars
            from carModelCountYear in car.ModelCountYearList
            select carModelCountYear).ToList();
        List<int> YearList = yearDtos.Select(e => e.Year).Distinct().ToList();
        var yearmap = yearDtos.GroupBy(e => e.Year).ToDictionary(e => e.Key, e => e.Sum(v => v.Quantity));

        List<CreateColumnFormatProductInformationDto> productList = priceEvaluationStartInputResult.ProductInformation;
//年份
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var year in YearList)
        {
            CreateRequirementDto c = createRequirementDtos.Find(p => p.Year == year);
            SopSecondModel sop = new SopSecondModel();
            sop.Year = year;
            sop.Motion = yearmap[year];
            sop.YearDrop = c.AnnualDeclineRate; //年将率
            sop.RebateRequest = c.AnnualRebateRequirements; // 年度返利要求
            sop.DiscountRate = c.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = c.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }

//核心部件
        List<PartsSecondModel> partsModels = new();
        foreach (var solution in Solutions)
        {
            partsModels.Add(new PartsSecondModel()
            {
                SolutionName = "核心部件: " + solution.SolutionName, PartsName = "核心部件", Model = "型号", Type = "类型",
                Remark = "备注"
            });
            CreateColumnFormatProductInformationDto product =
                productList.Where(p => p.Product.Equals(solution.ModuleName)).FirstOrDefault();
            ProductDevelopmentInputDto p = new();
            p.AuditFlowId = auditFlowId;
            p.SolutionId = solution.Id;
            List<ElectronicBomInfo> eles = await _electronicBomAppService.FindElectronicBomByProcess(p); //电子BOM
            List<StructureBomInfo> strs = await _structionBomAppService.FindStructureBomByProcess(p); //结构BOM
            PartsSecondModel Sensor = new();
            Sensor.PartsName = "Sensor";
            Sensor.Type = product.SensorTypeSelect;
            var ss = eles.Where(e => e.TypeName.Equals("Sensor芯片")).FirstOrDefault();
            Sensor.Model = ss.SapItemName; //
            Sensor.Remark = ss.EncapsulationSize;
            partsModels.Add(Sensor);
            PartsSecondModel Lens = new();
            Lens.PartsName = "Lens";
            var ll = strs.Where(e => e.TypeName.Equals("镜头")).FirstOrDefault();
            Lens.Model = ll.DrawingNumName; //
            Lens.Remark = "/";
            Lens.Type = product.LensTypeSelect;
            partsModels.Add(Lens);

            PartsSecondModel ISP = new();
            ISP.PartsName = "ISP";
            var ips = eles.Where(e => e.TypeName.Equals("芯片IC—ISP")).FirstOrDefault();
            ISP.Model = ips.SapItemName; //
            ISP.Remark = ips.EncapsulationSize;
            ISP.Type = product.IspTypeSelect;
            partsModels.Add(ISP);
            PartsSecondModel cx = new();
            cx.PartsName = "串行芯片";
            var cxs = eles.Where(e => e.TypeName.Equals("串行芯片")).FirstOrDefault();
            cx.Model = cxs.SapItemName; //
            cx.Type = product.SerialChipTypeSelect;
            cx.Remark = cxs.EncapsulationSize;
            partsModels.Add(cx);

            PartsSecondModel xl = new();
            xl.PartsName = "线缆";
            var xls = strs.Where(e => e.TypeName.Equals("线束")).FirstOrDefault();
            xl.Model = xls.DrawingNumName; //
            xl.Remark = xls.DimensionalAccuracyRemark;
            xl.Type = product.CableTypeSelect;
            partsModels.Add(xl);
        }

//NRE费用信息
        List<NRESecondModel> nres = new();
        NRESecondModel tit = new NRESecondModel() { NreName = "NRE费用信息", CostName = "费用名称" };
        List<string> title = new List<string>();
        NRESecondModel shouban = new NRESecondModel()
        {
            CostName = "手板件费"
        };
        List<string> shoubans = new List<string>();

        NRESecondModel muju = new NRESecondModel()
        {
            CostName = "模具费"
        };
        List<string> mujus = new List<string>();

        NRESecondModel scsb = new NRESecondModel()
        {
            CostName = "生产设备费"
        };
        List<string> scsbs = new List<string>();

        NRESecondModel gz = new NRESecondModel()
        {
            CostName = "工装费"
        };
        List<string> gzs = new List<string>();

        NRESecondModel yj = new NRESecondModel()
        {
            CostName = "治具费"
        };
        List<string> yjs = new List<string>();

        NRESecondModel jj = new NRESecondModel()
        {
            CostName = "检具费"
        };
        List<string> jjs = new List<string>();

        NRESecondModel sy = new NRESecondModel()
        {
            CostName = "实验费"
        };
        List<string> sys = new List<string>();

        NRESecondModel cs = new NRESecondModel()
        {
            CostName = "测试软件费"
        };
        List<string> css = new List<string>();

        NRESecondModel cl = new NRESecondModel()
        {
            CostName = "差旅费"
        };
        List<string> cls = new List<string>();

        NRESecondModel qt = new NRESecondModel()
        {
            CostName = "其他费用"
        };
        List<string> qts = new List<string>();

        foreach (var solution in Solutions)
        {
            PricingFormDto pricingFormDto =
                await _nrePricingAppService.GetPricingFormDownload(auditFlowId, solution.Id);
            title.Add(solution.SolutionName);
            shoubans.Add(pricingFormDto.HandPieceCostTotal.ToString());
            mujus.Add(pricingFormDto.MouldInventoryTotal.ToString());
            scsbs.Add(pricingFormDto.ProductionEquipmentCostTotal.ToString());
            gzs.Add(pricingFormDto.ToolingCostTotal.ToString());
            yjs.Add(pricingFormDto.FixtureCostTotal.ToString());
            jjs.Add(pricingFormDto.QAQCDepartmentsTotal.ToString());
            sys.Add(pricingFormDto.LaboratoryFeeModelsTotal.ToString());
            css.Add(pricingFormDto.SoftwareTestingCostTotal.ToString());
            cls.Add(pricingFormDto.TravelExpenseTotal.ToString());
            qts.Add(pricingFormDto.RestsCostTotal.ToString());
        }

        tit.Costs = title;
        nres.Add(tit);
        shouban.Costs = shoubans;
        nres.Add(shouban);
        muju.Costs = mujus;
        nres.Add(muju);
        scsb.Costs = scsbs;
        nres.Add(scsb);
        gz.Costs = gzs;
        nres.Add(gz);
        yj.Costs = yjs;
        nres.Add(yj);
        sy.Costs = sys;
        nres.Add(sy);
        cs.Costs = css;
        nres.Add(cs);
        cl.Costs = cls;
        nres.Add(cl);
        qt.Costs = qts;
        nres.Add(qt);


        //成本单价信息
        List<PricingSecondModel> pricingModels = new List<PricingSecondModel>();

//获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId);
        var soptime = priceEvaluationStartInputResult.SopTime;
        foreach (var solution in Solutions)
        {
            PricingSecondModel pcstitle = new();
            pcstitle.Name = "成本单价信息: " + solution.SolutionName;
            pcstitle.pcs = "走量";
            List<string> pccs = new List<string>();
            PricingSecondModel bom = new PricingSecondModel() { pcs = "①BOM成本" };
            List<String> boms = new();
            PricingSecondModel sccb = new PricingSecondModel() { pcs = "②生产成本" };
            List<String> sccbs = new();
            PricingSecondModel shcb = new PricingSecondModel() { pcs = "③损耗成本" };
            List<String> shcbs = new();
            PricingSecondModel yf = new PricingSecondModel() { pcs = "④运费" };
            List<String> yfs = new();
            PricingSecondModel moqftcb = new PricingSecondModel() { pcs = "⑤MOQ分摊成本" };
            List<String> moqftcbs = new();

            PricingSecondModel zlcb = new PricingSecondModel() { pcs = "⑥质量成本" };
            List<String> zlcbs = new();

            PricingSecondModel ftcb = new PricingSecondModel() { pcs = "⑦分摊成本" };
            List<String> ftcbs = new();

            PricingSecondModel zcb = new PricingSecondModel() { pcs = "总成本" };
            List<String> zcbs = new();

            foreach (var gradient in gradients)
            {
                pccs.Add(gradient.GradientValue + "K/V" + "SOP年成本");
                pccs.Add(gradient.GradientValue + "K/V" + "全生命周期成本");
                GetBomCostInput getBomCostInput = new()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id
                };
                List<Material> malist = await _priceEvaluationAppService.GetBomCost(getBomCostInput);
                var sopbom = malist.Where(p => p.Year == new decimal(soptime)).Sum(p => p.TotalMoneyCyn);
                var quanbom = malist.Sum(p => p.TotalMoneyCyn);
                boms.Add(sopbom.ToString());
                boms.Add(quanbom.ToString());
                GetManufacturingCostInput manufacturingCostInput = new GetManufacturingCostInput()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id, Year = soptime,
                    UpDown = YearType.Year
                };
                List<ManufacturingCost> sops =
                    await _priceEvaluationAppService.GetManufacturingCost(manufacturingCostInput);
                var zzsop = sops.Sum(p => p.Subtotal);
                GetManufacturingCostInput quansccb = new GetManufacturingCostInput()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id
                };
                List<ManufacturingCost> quansccbs =
                    await _priceEvaluationAppService.GetManufacturingCost(quansccb);
                var quancb = quansccbs.Sum(p => p.Subtotal);
                sccbs.Add(zzsop.ToString());
                sccbs.Add(quancb.ToString());


                GetCostItemInput getCostItemInput = new GetCostItemInput()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id, Year = soptime,
                    UpDown = YearType.Year
                };
                List<LossCost> soplosss = await _priceEvaluationAppService.GetLossCost(getCostItemInput);
                var losssop = soplosss.Sum(p => p.WastageCost);
                var moqftcbsssop = soplosss.Sum(p => p.MoqShareCount);
                GetCostItemInput quanianloss = new GetCostItemInput()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id
                };
                List<LossCost> quannianlosss = await _priceEvaluationAppService.GetLossCost(getCostItemInput);
                var quannianloss = quannianlosss.Sum(p => p.WastageCost);
                var quannianmoqftcbs = quannianlosss.Sum(p => p.MoqShareCount);
                shcbs.Add(losssop.ToString());
                shcbs.Add(quannianloss.ToString());
                moqftcbs.Add(moqftcbsssop.ToString());
                moqftcbs.Add(quannianmoqftcbs.ToString());
                GetLogisticsCostInput getLogisticsCostInput = new()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id
                };
                List<ProductionControlInfoListDto> productionControlInfoListDtos =
                    await _priceEvaluationAppService.GetLogisticsCost(getLogisticsCostInput);
                var sopys = productionControlInfoListDtos.Where(p => p.Year.Equals(soptime.ToString()))
                    .Sum(p => p.PerTotalLogisticsCost);
                var quanys = productionControlInfoListDtos.Sum(p => p.PerTotalLogisticsCost);
                yfs.Add(sopys.ToString());
                yfs.Add(quanys.ToString());

                GetOtherCostItemInput getOtherCostItemInput = new()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id, Year = soptime
                };

                QualityCostListDto sopqu = await _priceEvaluationAppService.GetQualityCost(getOtherCostItemInput);
                var sooqu = sopqu.QualityCost;
                var quanqu = 0;
                zlcbs.Add(sooqu.ToString());
                zlcbs.Add(quanqu.ToString());
                GetOtherCostItemInput qtcb = new GetOtherCostItemInput()
                {
                    AuditFlowId = auditFlowId, GradientId = gradient.Id, SolutionId = solution.Id, Year = soptime
                };
                var sopqt = await _priceEvaluationAppService.GetOtherCostItem(qtcb);
                var sopvalue = sopqt.Total;
                var quanvalue = 0;

                ftcbs.Add(sopvalue.ToString());
                ftcbs.Add(quanvalue.ToString());

                zcbs.Add((sopbom + zzsop + losssop + moqftcbsssop + sopys + sooqu + sopvalue).ToString());
                zcbs.Add((quanbom + quancb + quannianloss + quannianmoqftcbs + quanys + quanqu + quanvalue).ToString());
            }


            pcstitle.Costs = pccs;
            pricingModels.Add(pcstitle);
            pricingModels.Add(bom);
            pricingModels.Add(sccb);
            pricingModels.Add(shcb);
            pricingModels.Add(yf);
            pricingModels.Add(moqftcb);
            pricingModels.Add(zlcb);
            pricingModels.Add(ftcb);
            pricingModels.Add(zcb);
        }


        var value = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
            RecordNumber = priceEvaluationStartInputResult.Number, //记录编号           
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            OfferForm = priceEvaluationStartInputResult.PriceEvalType, //报价形式
            SopTime = priceEvaluationStartInputResult.SopTime, //SOP时间
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目生命周期
            ForSale = priceEvaluationStartInputResult.SalesType, //销售类型
            modeOfTrade = priceEvaluationStartInputResult.TradeMode, //贸易方式
            PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
            //ExchangeRate = priceEvaluationStartInputResult.ExchangeRate, //汇率???
            Sop = Sop,
            ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
            Parts = partsModels,
            NRE = nres,
            Cost = pricingModels,
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

    /// <summary>
    /// 数据库获取样品阶段
    /// </summary>     
    /// <returns></returns>
    public async Task<List<OnlySampleDto>> getSampleForData(long processId)
    {
        List<SolutionQuotation> solutionQuotations =
            _solutionQutation.GetAll().Where(p => p.AuditFlowId == processId && p.status == 0).ToList();
        List<SampleQuotation> sampleQuotations =
            _sampleQuotation.GetAll().Where(p => p.AuditFlowId == processId).ToList();
        List<OnlySampleDto> OnlySampleDtos = (from solution in solutionQuotations
            select new OnlySampleDto()
            {
                SolutionName = solution.SolutionName,
                SolutionId = solution.SolutionId,
                OnlySampleModels = sampleQuotations.Where(p => p.SolutionId == solution.SolutionId).ToList()
            }).ToList();
        return OnlySampleDtos;
    }

    public async Task<List<SampleQuotation>> getSample(List<CreateSampleDto> samples,
        decimal totalcost)
    {
        List<SampleQuotation> onlySampleModels = new();
        //样品
        foreach (var createSampleDto in samples)
        {
            SampleQuotation onlySampleModel = new SampleQuotation();
            onlySampleModel.Pcs = createSampleDto.Pcs; //需求量
            onlySampleModel.Name = createSampleDto.Name; //样品阶段名称
            onlySampleModel.Cost = totalcost; //成本：核价看板的最小梯度第一年的成本

            onlySampleModels.Add(onlySampleModel);
        }

        return onlySampleModels;
    }

    /// <summary>
    /// 数据库获取NRE
    /// </summary>     
    /// <returns></returns>
    public async Task<List<AnalyseBoardNreDto>> getNteForData(long processId)
    {
        List<SolutionQuotation> solutionQuotations =
            _solutionQutation.GetAll().Where(p => p.AuditFlowId == processId && p.status == 0).ToList();
        List<AnalyseBoardNreDto> nres = new List<AnalyseBoardNreDto>();
        List<DeviceQuotation> deviceQuotations =
            _deviceQuotation.GetAll().Where(p => p.AuditFlowId == processId).ToList();
        List<NreQuotation> nreQuotations = _nreQuotation.GetAll().Where(p => p.AuditFlowId == processId).ToList();
        nres = (from solution in solutionQuotations
            select new AnalyseBoardNreDto
            {
                SolutionId = solution.SolutionId,
                solutionName = solution.SolutionName,
                numberLine = solution.numberLine,
                collinearAllocationRate = solution.collinearAllocationRate,
                models = nreQuotations.Where(p => p.SolutionId == solution.SolutionId).ToList(),
                devices = deviceQuotations.Where(p => p.SolutionId == solution.SolutionId).ToList()
            }).ToList();


        AnalyseBoardNreDto analyseBoardNreDto = new AnalyseBoardNreDto()
        {
            solutionName = "汇总",
            models = nreQuotations.Where(p => p.SolutionId is null).ToList(),
            devices = deviceQuotations.Where(p => p.SolutionId is null).ToList()
        };
        nres.Add(analyseBoardNreDto);

        return nres;
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
        List<DeviceQuotation> hzde = new();
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

            List<NreQuotation> models = new List<NreQuotation>();
            NreQuotation shouban = new();
            shouban.FormName = "手板件费";
            shouban.PricingMoney = pricingFormDto.HandPieceCostTotal;
            handPieceCostTotals += pricingFormDto.HandPieceCostTotal;
            models.Add(shouban);
            NreQuotation mouju = new();
            mouju.FormName = "模具费";
            mouju.PricingMoney = pricingFormDto.MouldInventoryTotal;
            mouldInventoryTotals += pricingFormDto.MouldInventoryTotal;
            models.Add(mouju);
            NreQuotation scsb = new();
            scsb.FormName = "生产设备费";
            scsb.PricingMoney = pricingFormDto.ProductionEquipmentCostTotal;
            productionEquipmentCostTotals += pricingFormDto.ProductionEquipmentCostTotal;
            models.Add(scsb);
            NreQuotation gzf = new();
            gzf.FormName = "工装费";
            gzf.PricingMoney = pricingFormDto.ToolingCostTotal;
            toolingCostTotals += pricingFormDto.ToolingCostTotal;
            models.Add(gzf);
            NreQuotation yjf = new();
            yjf.FormName = "治具费";
            yjf.PricingMoney = pricingFormDto.FixtureCostTotal;
            fixtureCostTotals += pricingFormDto.FixtureCostTotal;
            models.Add(yjf);
            NreQuotation jjf = new();
            jjf.FormName = "检具费";
            jjf.PricingMoney = pricingFormDto.QAQCDepartmentsTotal;
            qAQCDepartmentsTotals += pricingFormDto.QAQCDepartmentsTotal;
            models.Add(jjf);
            NreQuotation syf = new();
            syf.FormName = "实验费";
            syf.PricingMoney = pricingFormDto.LaboratoryFeeModelsTotal;
            laboratoryFeeModelsTotals += pricingFormDto.LaboratoryFeeModelsTotal;
            models.Add(syf);
            NreQuotation csrjf = new();
            csrjf.FormName = "测试软件费";
            csrjf.PricingMoney = pricingFormDto.SoftwareTestingCostTotal;
            softwareTestingCostTotals += pricingFormDto.SoftwareTestingCostTotal;
            models.Add(csrjf);
            NreQuotation clf = new();
            clf.FormName = "差旅费";
            clf.PricingMoney = pricingFormDto.TravelExpenseTotal;
            travelExpenseTotals += pricingFormDto.TravelExpenseTotal;
            models.Add(clf);
            NreQuotation qtfy = new();
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
            List<DeviceQuotation> deviceModels = new();
            foreach (var deviceDto in deviceDtos)
            {
                DeviceQuotation deviceModel = new();
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
        List<NreQuotation> hz = new List<NreQuotation>();
        NreQuotation shoubanhz = new();
        shoubanhz.FormName = "手板件费";
        shoubanhz.PricingMoney = handPieceCostTotals;
        hz.Add(shoubanhz);
        NreQuotation moujuhz = new();
        moujuhz.FormName = "模具费";
        moujuhz.PricingMoney = mouldInventoryTotals;

        hz.Add(moujuhz);
        NreQuotation scsbhz = new();
        scsbhz.FormName = "生产设备费";
        scsbhz.PricingMoney = productionEquipmentCostTotals;

        hz.Add(scsbhz);
        NreQuotation gzfhz = new();
        gzfhz.FormName = "工装费";
        gzfhz.PricingMoney = toolingCostTotals;
        hz.Add(gzfhz);
        NreQuotation yjfhz = new();
        yjfhz.FormName = "治具费";
        yjfhz.PricingMoney = fixtureCostTotals;
        hz.Add(yjfhz);
        NreQuotation jjfhz = new();
        jjfhz.FormName = "检具费";
        jjfhz.PricingMoney = qAQCDepartmentsTotals;
        hz.Add(jjfhz);
        NreQuotation syfhz = new();
        syfhz.FormName = "实验费";
        syfhz.PricingMoney = laboratoryFeeModelsTotals;
        hz.Add(syfhz);
        NreQuotation csrjfhz = new();
        csrjfhz.FormName = "测试软件费";
        csrjfhz.PricingMoney = softwareTestingCostTotals;
        hz.Add(csrjfhz);
        NreQuotation clfhz = new();
        clfhz.FormName = "差旅费";
        clfhz.PricingMoney = travelExpenseTotals;
        hz.Add(clfhz);
        NreQuotation qtfyhz = new();
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
    public async Task<List<PooledAnalysisModel>> GetPoolAnalysis(List<Gradient> gradients,
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<decimal> gross)
    {
        List<PooledAnalysisModel> result = new List<PooledAnalysisModel>();
        PooledAnalysisModel pooledAnalysisModelsl = new();
        var soptime = priceEvaluationStartInputResult.SopTime;
        pooledAnalysisModelsl.ProjectName = "数量";
       // var nsum = priceEvaluationStartInputResult.ModelCount.Sum(e => e.SumQuantity);

        var nsum = 100;
        decimal td = new();
        long tdid = 1;
        foreach (var gradient in gradients)
        {
            if (nsum <= gradient.GradientValue)
            {
                td = gradient.GradientValue;
                tdid = gradient.Id;
            }
        }

    //    _priceEvaluationGetAppService.GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput() {});
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


    /// <summary>
    /// Ner  营销部录入
    /// </summary>
    /// <param name="departmentDtos"></param>
    /// <returns></returns>
    public async Task PostSalesDepartment(List<AnalyseBoardNreDto> nres)
    {
        foreach (var analyseBoardNreDto in nres)
        {
            var solutionId = analyseBoardNreDto.SolutionId;
            var AuditFlowId = analyseBoardNreDto.AuditFlowId;
            List<DeviceQuotation> devices = analyseBoardNreDto.devices;
            foreach (var deviceQuotation in devices)
            {
                deviceQuotation.AuditFlowId = AuditFlowId;
                deviceQuotation.SolutionId = solutionId;
                _deviceQuotation.InsertOrUpdateAsync(deviceQuotation);
            }

            List<NreQuotation> re = analyseBoardNreDto.models;
            List<NreQuotation> res = ObjectMapper.Map<List<NreQuotation>>(re);
            foreach (var item in res)
            {
                item.AuditFlowId = AuditFlowId;
                item.SolutionId = solutionId;
                _nreQuotation.InsertOrUpdateAsync(item);
            }
        }
    }

    /// <summary>
    /// Ner  样品阶段
    /// </summary>
    /// <param name="departmentDtos"></param>
    /// <returns></returns>
    public async Task PostSample(List<OnlySampleDto> samples)
    {
        List<SampleQuotation> list = ObjectMapper.Map<List<SampleQuotation>>(samples);
        foreach (var item in list)
        {
            _sampleQuotation.InsertOrUpdate(item);
        }
    }

    /// <summary>
    /// 查看 营销部报价审核表
    /// </summary>
    public async Task<QuotationListSecondDto> QuotationListSecond(long processId)
    {
        List<FinanceDictionaryDetail> priceEvaluations = await _financeDictionaryDetailRepository.GetAllListAsync();
        QuotationListSecondDto pp = (from o in await _resourceAuditFlow.GetAllListAsync(p => p.Id.Equals(processId))
            join a in _resourcePriceEvaluation.GetAll() on o.Id equals a.AuditFlowId into oa
            from oa1 in oa.DefaultIfEmpty()
            join b in priceEvaluations on oa1.QuotationType equals b.Id into b1
            from b2 in b1.DefaultIfEmpty()
            join c in priceEvaluations on oa1.CustomerNature equals c.Id into c1
            from c2 in c1.DefaultIfEmpty()
            join d in priceEvaluations on oa1.TerminalNature equals d.Id into d1
            from d2 in d1.DefaultIfEmpty()
            join e in priceEvaluations on oa1.SalesType equals e.Id into e1
            from e2 in e1.DefaultIfEmpty()
            join f in _resourceExchangeRate.GetAll() on oa1.Currency equals f.Id into f1
            from f2 in f1.DefaultIfEmpty()
            join z in priceEvaluations on oa1.TradeMode equals z.Id into z1
            from z2 in z1.DefaultIfEmpty()
            select new QuotationListSecondDto
            {
                Date = DateTime.Now, //查询日期
                RecordNumber = oa1.Number, // 单据编号
                Versions = o.QuoteVersion, //版本
                OfferForm = b2.DisplayName, //报价形式
                DirectCustomerName = oa1.CustomerName, //直接客户名称
                ClientNature = c2.DisplayName, //客户性质
                TerminalCustomerName = oa1 is null ? "" : oa1.TerminalName, //终端客户名称
                TerminalClientNature = d2 is null ? "" : d2.DisplayName, //终端客户性质
                //开发计划 手工录入
                SopTime = oa1.SopTime, //Sop时间
                ProjectCycle = oa1.ProjectCycle, //项目周期
                ForSale = e2.DisplayName, //销售类型
                modeOfTrade = z2.DisplayName, //贸易方式
                PaymentMethod = oa1.PaymentMethod, //付款方式
                QuoteCurrency = f2.ExchangeRateKind, //报价币种
                ExchangeRate = oa1.ExchangeRate, //汇率
                ProjectName = oa1.ProjectName, //项目名称
            }).FirstOrDefault();

        return pp;
    }

    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long processId)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto = new();


        return managerApprovalOfferDto;
    }


    public async Task<AcceptanceBidDto> GetAcceptanceBid(long processId)
    {
        AcceptanceBidDto quotationListSecondDto = new();


        return quotationListSecondDto;
    }
    public async Task<AcceptanceBidDto> GetBidView(long processId)
    {
        AcceptanceBidDto quotationListSecondDto = new();


        return quotationListSecondDto;
    }
    public async Task<QuotationListSecondDto> GetFinancialArchive(long processId)
    {
        QuotationListSecondDto quotationListSecondDto = new();


        return quotationListSecondDto;
    }
}