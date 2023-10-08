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
using Abp.UI;
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
    /// 报价分析看板中的 汇总分析表  实体类
    /// </summary>
    private readonly IRepository<PooledAnalysisOffers, long> _resourcePooledAnalysisOffers;

    /// <summary>
    /// 产品信息表
    /// </summary>
    private readonly IRepository<ProductInformation, long> _resourceProductInformation;

    /// <summary>
    /// 营销部审核中方案表
    /// </summary>
    public readonly IRepository<Solution, long> _resourceSchemeTable;

    /// <summary>
    ///报价 项目看板实体类 实体类
    /// </summary>
    private readonly IRepository<ProjectBoardOffers, long> _resourceProjectBoardOffers;

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
    /// Nre
    /// </summary>
    private readonly IRepository<NreQuotation, long> _nreQuotation;

    /// <summary>
    /// 样品阶段
    /// </summary>
    private readonly IRepository<SampleQuotation, long> _sampleQuotation;

    private readonly IRepository<Sample, long> _sampleRepository;

    /// <summary>
    /// 报价分析看板中的 产品单价表 实体类
    /// </summary>
    private readonly IRepository<UnitPriceOffers, long> _resourceUnitPriceOffers;

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
    /// 报价毛利率测算-阶梯数量
    /// </summary>
    private readonly IRepository<ActualUnitPriceOffer, long> _actualUnitPriceOffer;

    /// <summary>
    /// 报价毛利率测算-实际数量
    /// </summary>
    private readonly IRepository<DynamicUnitPriceOffers, long> _dynamicUnitPriceOffers;


    /// <summary>
    /// 结构BOM录入接口类
    /// </summary>
    public readonly StructionBomAppService _structionBomAppService;

    private readonly IRepository<FinanceDictionary, string> _financeDictionaryRepository;
    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

    /// <summary>
    /// 审批流程主表
    /// </summary>
    private readonly IRepository<AuditFlow, long> _resourceAuditFlow;

    private readonly ProcessHoursEnterDeviceAppService _processHoursEnterDeviceAppService;

    public AnalysisBoardSecondMethod(IRepository<ModelCountYear, long> modelCountYear,
        IRepository<PriceEvaluation, long> priceEvaluationRepository,
        IRepository<ModelCount, long> modelCount,
        IRepository<DeviceQuotation, long> deviceQuotation,
        IRepository<UnitPriceOffers, long> resourceUnitPriceOffers,
        PriceEvaluationGetAppService priceEvaluationGetAppService,
        IRepository<SampleQuotation, long> sampleQuotation,
        IRepository<NreQuotation, long> nreQuotation,
        IRepository<ActualUnitPriceOffer, long> actualUnitPriceOffer,
        IRepository<SolutionQuotation, long> solutionQutation,
        PriceEvaluationAppService priceEvaluationAppService,
        IRepository<ExchangeRate, long> resourceExchangeRate,
        IRepository<PriceEvaluation, long> resourcePriceEvaluation,
        IRepository<AuditFlow, long> resourceAuditFlow,
        ElectronicBomAppService electronicBomAppService,
        StructionBomAppService structionBomAppService,
        IRepository<PooledAnalysisOffers, long> resourcePooledAnalysisOffers,
        IRepository<Sample, long> sampleRepository,
        IRepository<Solution, long> resourceSchemeTable,
        IRepository<FinanceDictionary, string> financeDictionaryRepository,
        IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
        IRepository<Gradient, long> gradientRepository,
        IRepository<ManufacturingCostInfo, long> manufacturingCostInfo,
        IRepository<StructureBomInfo, long> structureBomInfo,
        IRepository<Requirement, long> requirement,
        IRepository<ProjectBoardOffers, long> resourceProjectBoardOffers,
        ProcessHoursEnterDeviceAppService processHoursEnterDeviceAppService,
        IRepository<ProductInformation, long> productInformation,
        IRepository<DynamicUnitPriceOffers, long> dynamicUnitPriceOffers,
        NrePricingAppService nrePricingAppService,
        IRepository<GrossMarginForm, long> resourceGrossMarginForm)
    {
        _resourceRequirement = requirement;
        _resourceProjectBoardOffers = resourceProjectBoardOffers;
        _sampleRepository = sampleRepository;
        _dynamicUnitPriceOffers = dynamicUnitPriceOffers;
        _resourcePooledAnalysisOffers = resourcePooledAnalysisOffers;
        _resourceUnitPriceOffers = resourceUnitPriceOffers;
        _resourceSchemeTable = resourceSchemeTable;
        _financeDictionaryRepository = financeDictionaryRepository;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _priceEvaluationAppService = priceEvaluationAppService;
        _resourceAuditFlow = resourceAuditFlow;
        _solutionQutation = solutionQutation;
        _resourcePriceEvaluation = resourcePriceEvaluation;
        _gradientRepository = gradientRepository;
        _deviceQuotation = deviceQuotation;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;
        _actualUnitPriceOffer = actualUnitPriceOffer;
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

    public async Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new();
        var auditFlowId = analyseBoardSecondInputDto.auditFlowId;
        //获取方案88
        List<Solution> Solutions = analyseBoardSecondInputDto.solutionTables;
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(analyseBoardSecondInputDto.auditFlowId);
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId);
        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        //获取毛利率
        List<decimal> gross = await GetGrossMargin();
        //sop年份
        var soptime = priceEvaluationStartInputResult.SopTime;
        List<CreateSampleDto> sampleDtos = priceEvaluationStartInputResult.Sample;
        List<OnlySampleDto> samples = new List<OnlySampleDto>();

        //样品阶段
        foreach (var Solution in Solutions)
        {
            var productld = Solution.Productld;
            var gepr = new GetPriceEvaluationTableResultInput();
            gepr.AuditFlowId = auditFlowId;
            gepr.Year = soptime;
            gepr.UpDown = YearType.Year;
            gepr.GradientId = mintd.Id;
            gepr.ProductId = productld;
            //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
            //    var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);接口弃用
            ExcelPriceEvaluationTableDto ex = new ExcelPriceEvaluationTableDto();
            try
            {
                 ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = auditFlowId, GradientId = mintd.Id, InputCount = 0, SolutionId = Solution.Id,
                    Year = soptime, UpDown = YearType.FirstHalf
                });
            }catch (Exception e)
            {

                throw new UserFriendlyException("核价数据问题");

            }

            
           

            //最小梯度SOP年成本
            var totalcost = ex.TotalCost;
            //样品阶段
            if (priceEvaluationStartInputResult.IsHasSample == true)
            {
                OnlySampleDto onlySampleDto = new();
                List<SampleQuotation> onlySampleModels =
                    await getSample(sampleDtos, totalcost);
                onlySampleDto.SolutionName = Solution.SolutionName;
                onlySampleDto.SolutionId = Solution.Id;
                onlySampleDto.OnlySampleModels = onlySampleModels;
                samples.Add(onlySampleDto);
            }
        }

//单价表
        List<SopAnalysisModel> sops = new List<SopAnalysisModel>();
        foreach (var gradient in gradients)
        {
            foreach (var Solution in Solutions)
            {
                SopAnalysisModel sopAnalysisModel = new();

                var productld = Solution.Productld;
                var gepr = new GetPriceEvaluationTableResultInput();
                gepr.AuditFlowId = auditFlowId;
                gepr.Year = soptime;
                gepr.UpDown = YearType.Year;
                gepr.GradientId = gradient.Id;
                gepr.ProductId = productld;
                //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
                //var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);
                var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = auditFlowId, GradientId = mintd.Id, InputCount = 0, SolutionId = Solution.Id,
                    Year = soptime, UpDown = YearType.FirstHalf
                });

                //最小梯度SOP年成本
                var totalcost = ex.TotalCost;
                //var totalcost = 100;

                sopAnalysisModel.Product = Solution.SolutionName;
                sopAnalysisModel.GradientValue = gradient.GradientValue + "K/Y";
                List<GrossValue> grosss = new List<GrossValue>();
                foreach (var gro in gross)
                {
                    GrossValue gr = new GrossValue();
                    gr.Grossvalue = Math.Round(totalcost / (1 - (gro / 100)), 2);
                    gr.Gross = gro.ToString();
                    grosss.Add(gr);
                }

                sopAnalysisModel.GrossValues = grosss;
                sops.Add(sopAnalysisModel);
            }
        }

//NRE
        analyseBoardSecondDto.nres = await getNre(analyseBoardSecondInputDto.auditFlowId,
            analyseBoardSecondInputDto.solutionTables);
        //样品阶段
        analyseBoardSecondDto.SampleOffer = samples;
        //sop单价表
        analyseBoardSecondDto.Sops = sops;

        analyseBoardSecondDto.FullLifeCycle =
            await GetPoolAnalysis(auditFlowId, gradients, priceEvaluationStartInputResult,
                gross, analyseBoardSecondInputDto.solutionTables, sops);
        analyseBoardSecondDto.GradientQuotedGrossMargins =
            await GetstepsNum(priceEvaluationStartInputResult, Solutions, gradients, sops)
            ;
        analyseBoardSecondDto.QuotedGrossMargins =
            await GetActual(priceEvaluationStartInputResult, Solutions);
        return analyseBoardSecondDto;
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

    public async Task PostIsOfferSaveSecond(IsOfferSecondDto isOfferDto)
    {
        List<Solution> solutions = isOfferDto.Solutions;
        int version = isOfferDto.version;
        var AuditFlowId = isOfferDto.AuditFlowId;
        await InsertSolution(solutions, version);
//获取报价方案
        List<SolutionQuotation> solutionQuotations =
            await _solutionQutation.GetAllListAsync(p => p.AuditFlowId == AuditFlowId && p.version == version);

        List<AnalyseBoardNreDto> nres = isOfferDto.nres;
        InsertNre(nres, solutionQuotations);
        List<OnlySampleDto> onlySampleDtos = isOfferDto.SampleOffer;
        InsertSample(onlySampleDtos, solutionQuotations);

        List<SopAnalysisModel> sops = isOfferDto.Sops;
        InsertSop(sops, version);
        List<PooledAnalysisModel> FullLifeCycle = isOfferDto.FullLifeCycle;
        InsertPool(AuditFlowId, FullLifeCycle, version);

        List<ProjectBoardModel> projectBoardModels = isOfferDto.ProjectBoard;
        Insertboard(AuditFlowId, projectBoardModels, version);
        List<GradientQuotedGrossMarginModel> jts = isOfferDto.GradientQuotedGrossMargins;
        InsertGradientQuotedGrossMargin(AuditFlowId, jts, version);
        List<QuotedGrossMarginProjectModel> models = isOfferDto.QuotedGrossMargins;
        InsertQuotedGrossMarginProject(AuditFlowId, models, version);
    }
    /// <summary>
    /// 报价毛利率测算 实际数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertQuotedGrossMarginProject(long AuditFlowId,
        List<QuotedGrossMarginProjectModel> grossMarginProjectModels, int version)
    {
        var list = new List<DynamicUnitPriceOffers>();
        foreach (var grossMargin in grossMarginProjectModels)
        {
            var project = grossMargin.project;
            var dys = (from gross in grossMargin.GrossMargins
                select new DynamicUnitPriceOffers()
                {
                    version = version,
                    AuditFlowId = AuditFlowId,
                    ProductName = gross.product,
                    InteriorTargetUnitPrice = gross.quotedGrossMarginSimple.Interior.Price,
                    AllInteriorGrossMargin = gross.quotedGrossMarginSimple.Interior.GrossMargin,
                    AllInteriorClientGrossMargin = gross.quotedGrossMarginSimple.Interior.ClientGrossMargin,
                    AllInteriorNreGrossMargin = gross.quotedGrossMarginSimple.Interior.NreGrossMargin,
                    ClientTargetUnitPrice = gross.quotedGrossMarginSimple.Client.Price,
                    AllClientGrossMargin = gross.quotedGrossMarginSimple.Client.GrossMargin,
                    AllClientClientGrossMargin = gross.quotedGrossMarginSimple.Client.ClientGrossMargin,
                    AllClientNreGrossMargin = gross.quotedGrossMarginSimple.Client.NreGrossMargin,
                    OfferUnitPrice = gross.quotedGrossMarginSimple.ThisQuotation.Price,
                    OffeGrossMargin = gross.quotedGrossMarginSimple.ThisQuotation.GrossMargin,
                    ClientGrossMargin = gross.quotedGrossMarginSimple.ThisQuotation.ClientGrossMargin,
                    NreGrossMargin = gross.quotedGrossMarginSimple.ThisQuotation.NreGrossMargin
                }).ToList();
            list.AddRange(dys);
        }

        foreach (var dynamicUnitPriceOffers in list)
        {
            _dynamicUnitPriceOffers.InsertAsync(dynamicUnitPriceOffers);
        }
    }
    /// <summary>
    /// 报价毛利率测算阶梯数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertGradientQuotedGrossMargin(long AuditFlowId,
        List<GradientQuotedGrossMarginModel> gradientQuotedGross, int version)
    {
        var list = (from grd in gradientQuotedGross
            select new ActualUnitPriceOffer()
            {
                AuditFlowId = AuditFlowId,
                version = version,
                Kv = grd.gradient,
                InteriorTarget = JsonConvert.SerializeObject(grd.QuotedGrossMarginSimple.Interior),
                ClientTarget = JsonConvert.SerializeObject(grd.QuotedGrossMarginSimple.Client),
                Offer = JsonConvert.SerializeObject(grd.QuotedGrossMarginSimple.ThisQuotation)
            }).ToList();

        foreach (var actual in list)
        {
            _actualUnitPriceOffer.InsertAsync(actual);
        }
    }
    /// <summary>
    /// 看板保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task Insertboard(long AuditFlowId, List<ProjectBoardModel> boards, int version)
    {
        var projectbords = (from board in boards
            select new ProjectBoardOffers()
            {
                AuditFlowId = AuditFlowId,
                ProjectName = board.ProjectName,
                version = version,
                InteriorTarget = JsonConvert.SerializeObject(board.InteriorTarget),
                ClientTarget = JsonConvert.SerializeObject(board.ClientTarget),
                Offer = JsonConvert.SerializeObject(board.Offer)
            }).ToList();

        foreach (var projectbord in projectbords)
        {
            await _resourceProjectBoardOffers.InsertAsync(projectbord);
        }
    }
    /// <summary>
    /// 汇总分析  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertPool(long AuditFlowId, List<PooledAnalysisModel> pooledAnalysisModels, int version)
    {
        var polls = (from pool in pooledAnalysisModels
            select new PooledAnalysisOffers()
            {
                AuditFlowId = AuditFlowId,
                ProjectName = pool.ProjectName,
                version = version,
                GrossMarginList = JsonConvert.SerializeObject(pool.GrossMarginList)
            }).ToList();
        foreach (var pool in polls)
        {
            await _resourcePooledAnalysisOffers.InsertAsync(pool);
        }
    }

    /// <summary>
    /// 单价表 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSop(List<SopAnalysisModel> unitPriceModels, int version)
    {
        var unitPriceOffersList = (from unit in unitPriceModels
            select new UnitPriceOffers()
            {
                AuditFlowId = unit.AuditFlowId,
                ProductName = unit.Product,
                GradientValue = unit.GradientValue,
                version = version,
                GrossMarginList = JsonConvert.SerializeObject(unit.GrossValues)
            }).ToList();
        foreach (var uns in unitPriceOffersList)
        {
            _resourceUnitPriceOffers.InsertAsync(uns);
        }
    }

    /// <summary>
    /// 样品 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSample(List<OnlySampleDto> onlySampleDtos, List<SolutionQuotation> solutionQuotations)
    {
        foreach (var onlySampleDto in onlySampleDtos)
        {
            var soltuion = solutionQuotations.Where(p => p.SolutionId == onlySampleDto.SolutionId).FirstOrDefault();
            List<SampleQuotation> sampleQuotations = onlySampleDto.OnlySampleModels;

            foreach (var sampleQuotation in sampleQuotations)
            {
                sampleQuotation.SolutionId = soltuion.Id;
                _sampleQuotation.InsertAsync(sampleQuotation);
            }
        }
    }

    /// <summary>
    /// 报价方案 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSolution(List<Solution> solutions, int version)
    {
        List<SolutionQuotation> solutionQuotations = (from solution in solutions
            select new SolutionQuotation()
            {
                SolutionId = solution.Id,
                AuditFlowId = solution.AuditFlowId,
                Productld = solution.Productld,
                ModuleName = solution.ModuleName,
                SolutionName = solution.SolutionName,
                Product = solution.Product,
                IsCOB = solution.IsCOB,
                ElecEngineerId = solution.ElecEngineerId,
                StructEngineerId = solution.StructEngineerId,
                IsFirst = solution.IsFirst,
                version = version
            }).ToList();
        foreach (var solutionQuotation in solutionQuotations)
        {
            _solutionQutation.InsertAsync(solutionQuotation);
        }
    }

    /// <summary>
    /// NRE 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertNre(List<AnalyseBoardNreDto> nres, List<SolutionQuotation> solutionQuotations)
    {
        foreach (var nre in nres)
        {
            var solutionId = nre.SolutionId;
            if (solutionId is not null)
            {
                var soltuion = solutionQuotations.Where(p => p.SolutionId == nre.SolutionId).FirstOrDefault();
                List<NreQuotation> nreQuotations = nre.models;
                List<DeviceQuotation> deviceQuotations = nre.devices;
                soltuion.numberLine = nre.numberLine;
                soltuion.collinearAllocationRate = nre.collinearAllocationRate;
                _solutionQutation.UpdateAsync(soltuion);
                foreach (var nreQuotation in nreQuotations)
                {
                    nreQuotation.SolutionId = soltuion.Id;
                    _nreQuotation.InsertAsync(nreQuotation);
                }

                foreach (var deviceQuotation in deviceQuotations)
                {
                    deviceQuotation.SolutionId = soltuion.Id;
                    _deviceQuotation.InsertAsync(deviceQuotation);
                }
            }
        }
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
        List<FinanceDictionaryDetail> dics = _financeDictionaryDetailRepository.GetAll()
            .Where(p => p.FinanceDictionaryId == "SampleName").ToList();
        //样品

        List<SampleQuotation> onlySampleModels = (from sample in samples
            join dic in dics on sample.Name equals dic.Id
            select new SampleQuotation()
            {
                Pcs = sample.Pcs, //需求量
                Name = dic.DisplayName, //样品阶段名称
                Cost = Math.Round(totalcost, 2) //成本：核价看板的最小梯度第一年的成本
            }).ToList();


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
            shouban.SolutionId = solutionTable.Id;
            shouban.PricingMoney = Math.Round(pricingFormDto.HandPieceCostTotal, 2);
            handPieceCostTotals += pricingFormDto.HandPieceCostTotal;
            models.Add(shouban);
            NreQuotation mouju = new();
            mouju.FormName = "模具费";
            mouju.SolutionId = solutionTable.Id;

            mouju.PricingMoney = Math.Round(pricingFormDto.MouldInventoryTotal, 2);
            mouldInventoryTotals += pricingFormDto.MouldInventoryTotal;
            models.Add(mouju);
            NreQuotation scsb = new();
            scsb.FormName = "生产设备费";
            scsb.SolutionId = solutionTable.Id;
            scsb.PricingMoney = Math.Round(pricingFormDto.ProductionEquipmentCostTotal, 2);
            productionEquipmentCostTotals += pricingFormDto.ProductionEquipmentCostTotal;
            models.Add(scsb);
            NreQuotation gzf = new();
            gzf.FormName = "工装费";
            gzf.SolutionId = solutionTable.Id;

            gzf.PricingMoney = Math.Round(pricingFormDto.ToolingCostTotal, 2);
            toolingCostTotals += pricingFormDto.ToolingCostTotal;
            models.Add(gzf);
            NreQuotation yjf = new();
            yjf.FormName = "治具费";
            yjf.SolutionId = solutionTable.Id;
            yjf.PricingMoney = Math.Round(pricingFormDto.FixtureCostTotal, 2);
            fixtureCostTotals += pricingFormDto.FixtureCostTotal;
            models.Add(yjf);
            NreQuotation jjf = new();
            jjf.FormName = "检具费";
            jjf.SolutionId = solutionTable.Id;
            jjf.PricingMoney = Math.Round(pricingFormDto.QAQCDepartmentsTotal, 2);
            qAQCDepartmentsTotals += pricingFormDto.QAQCDepartmentsTotal;
            models.Add(jjf);
            NreQuotation syf = new();
            syf.FormName = "实验费";
            syf.SolutionId = solutionTable.Id;
            syf.PricingMoney = Math.Round(pricingFormDto.LaboratoryFeeModelsTotal, 2);
            laboratoryFeeModelsTotals += pricingFormDto.LaboratoryFeeModelsTotal;
            models.Add(syf);
            NreQuotation csrjf = new();
            csrjf.FormName = "测试软件费";
            csrjf.SolutionId = solutionTable.Id;

            csrjf.PricingMoney = Math.Round(pricingFormDto.SoftwareTestingCostTotal, 2);
            softwareTestingCostTotals += pricingFormDto.SoftwareTestingCostTotal;
            models.Add(csrjf);
            NreQuotation clf = new();
            clf.FormName = "差旅费";
            clf.SolutionId = solutionTable.Id;

            clf.PricingMoney = Math.Round(pricingFormDto.TravelExpenseTotal, 2);
            travelExpenseTotals += pricingFormDto.TravelExpenseTotal;
            models.Add(clf);
            NreQuotation qtfy = new();
            qtfy.FormName = "其他费用";
            qtfy.SolutionId = solutionTable.Id;

            qtfy.PricingMoney = Math.Round(pricingFormDto.RestsCostTotal, 2);
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
                deviceModel.SolutionId = solutionTable.Id;
                deviceModel.Number = Math.Round(deviceDto.DeviceNumber.Value, 2);
                deviceModel.DevicePrice = Math.Round(deviceDto.DevicePrice.Value, 2);
                deviceModel.equipmentMoney = Math.Round(deviceModel.Number * deviceModel.DevicePrice, 2);
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
        shoubanhz.PricingMoney = Math.Round(handPieceCostTotals, 2);
        hz.Add(shoubanhz);
        NreQuotation moujuhz = new();
        moujuhz.FormName = "模具费";
        moujuhz.PricingMoney = Math.Round(mouldInventoryTotals, 2);

        hz.Add(moujuhz);
        NreQuotation scsbhz = new();
        scsbhz.FormName = "生产设备费";
        scsbhz.PricingMoney = Math.Round(productionEquipmentCostTotals, 2);

        hz.Add(scsbhz);
        NreQuotation gzfhz = new();
        gzfhz.FormName = "工装费";
        gzfhz.PricingMoney = Math.Round(toolingCostTotals, 2);
        hz.Add(gzfhz);
        NreQuotation yjfhz = new();
        yjfhz.FormName = "治具费";
        yjfhz.PricingMoney = Math.Round(fixtureCostTotals, 2);
        hz.Add(yjfhz);
        NreQuotation jjfhz = new();
        jjfhz.FormName = "检具费";
        jjfhz.PricingMoney = Math.Round(qAQCDepartmentsTotals, 2);
        hz.Add(jjfhz);
        NreQuotation syfhz = new();
        syfhz.FormName = "实验费";
        syfhz.PricingMoney = Math.Round(laboratoryFeeModelsTotals, 2);
        hz.Add(syfhz);
        NreQuotation csrjfhz = new();
        csrjfhz.FormName = "测试软件费";
        csrjfhz.PricingMoney = Math.Round(softwareTestingCostTotals, 2);
        hz.Add(csrjfhz);
        NreQuotation clfhz = new();
        clfhz.FormName = "差旅费";
        clfhz.PricingMoney = Math.Round(travelExpenseTotals, 2);
        hz.Add(clfhz);
        NreQuotation qtfyhz = new();
        qtfyhz.FormName = "其他费用";
        qtfyhz.PricingMoney = Math.Round(restsCostTotals, 2);
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
                grossMargin.product = solution.Product + "-" + solution.SolutionName;
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
            list.Add(grossMarginModel);
        }

        return list;
    }

    /// <summary>
    /// 报价毛利率测算阶梯数量
    /// </summary>     
    /// <returns></returns>
    public async Task<List<GradientQuotedGrossMarginModel>> GetstepsNum(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<Solution> solutions,
        List<Gradient> gradients, List<SopAnalysisModel> sops)
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
                grossMarginModel.product = solution.Product + "-" + solution.SolutionName;
                var smple = createCustomerTargetPriceDtos.Where(p => p.Kv == gradient.GradientValue).ToList();
                decimal unprice = smple.Count==0?0:smple.First().ExchangeRate * (decimal.Parse(smple.First().TargetPrice)); //单价
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
    public async Task<List<PooledAnalysisModel>> GetPoolAnalysis(long auditFlowId, List<Gradient> gradients,
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<decimal> gross, List<Solution> solutions,
        List<SopAnalysisModel> sops)
    {
        List<PooledAnalysisModel> result = new List<PooledAnalysisModel>();
        List<CreateRequirementDto> createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        PooledAnalysisModel pooledAnalysisModelsl = new();
        var soptime = priceEvaluationStartInputResult.SopTime;
        pooledAnalysisModelsl.ProjectName = "数量";
        var nsum = priceEvaluationStartInputResult.ModelCount.Sum(e => e.SumQuantity);

        var years = (from cmc in priceEvaluationStartInputResult.ModelCount
            from cmcy in cmc.ModelCountYearList
            select cmcy).Where(p => p.UpDown == YearType.FirstHalf).Select(p => p.Year).Distinct().ToList();
        if (nsum == 0)
        {
            nsum = 1;
        }

        decimal flxssr = 0;
        decimal td = new();
        long tdid = 1;
        decimal xscb = 0;
        foreach (var gradient in gradients.OrderBy(p => p.GradientValue))
        {
            if (nsum <= gradient.GradientValue*1000)
            {
                td = gradient.GradientValue;
                tdid = gradient.Id;
            }
        }

        foreach (var gradient in gradients)
        {
            SopAnalysisModel sop = sops.Where(p => p.GradientValue.Equals(td.ToString() + "K/Y")).ToList()
                .FirstOrDefault();
            var untiprice = sop.GrossValues.OrderBy(p => p.Gross).Select(p => p.Grossvalue).ToList().FirstOrDefault();
            foreach (var year in years)
            {
                var crms = createRequirementDtos.Where(p => p.Year == year && p.UpDown == YearType.FirstHalf)
                    .FirstOrDefault();
                foreach (var solution in solutions)
                {
                    var sjsl =
                        (from cmc in priceEvaluationStartInputResult.ModelCount.Where(p =>
                                p.Product.Equals(solution.Product))
                            from cmcy in cmc.ModelCountYearList
                            select cmcy).Where(p => p.UpDown == YearType.Year).ToList().Sum(e => e.Quantity);
                    var data = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                    {
                        AuditFlowId = auditFlowId, GradientId = tdid, InputCount = 0, SolutionId = solution.Id,
                        Year = year, UpDown = YearType.FirstHalf
                    });
                    var sum = data.Total * sjsl;
                    flxssr += untiprice * sjsl * (1 - crms.AnnualRebateRequirements / 100) *
                              (1 - crms.AnnualDeclineRate / 100);
                    xscb += sum;
                }
            }
        }


        //    _priceEvaluationGetAppService.GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput() {});
        List<GrossMarginModel> slsl = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = Math.Round(nsum / (1 - (gros / 100)), 2);

            slsl.Add(grossMarginModel);
        }

        pooledAnalysisModelsl.GrossMarginList = slsl;
        result.Add(pooledAnalysisModelsl);
        PooledAnalysisModel pooledAnalysisModelxscb = new();
        pooledAnalysisModelxscb.ProjectName = "销售成本";

        List<GrossMarginModel> xscbsl = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = Math.Round(xscb / (1 - (gros / 100)), 2);

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
            grossMarginModel.GrossMarginNumber = Math.Round(pjcb / (1 - (gros / 100)), 2);

            pjcbs.Add(grossMarginModel);
        }

        pooledAnalysisModelpjcb.GrossMarginList = pjcbs;
        result.Add(pooledAnalysisModelpjcb);


        PooledAnalysisModel pooledAnalysisModelfl = new();
        pooledAnalysisModelfl.ProjectName = "返利后销售收入";

        List<GrossMarginModel> fls = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = Math.Round(flxssr / (1 - (gros / 100)), 2);

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
            grossMarginModel.GrossMarginNumber = Math.Round(dj / (1 - (gros / 100)), 2);

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
            grossMarginModel.GrossMarginNumber = Math.Round(xsmls / (1 - (gros / 100)), 2);

            xsml.Add(grossMarginModel);
        }

        pooledAnalysisModelpjxsml.GrossMarginList = xsml;
        result.Add(pooledAnalysisModelpjxsml);


        PooledAnalysisModel pooledAnalysisModelpjxmll = new();
        pooledAnalysisModelpjxmll.ProjectName = "毛利率";
        if (flxssr == 0)
        {
            flxssr = 1;
        }

        decimal ml = xsmls / flxssr;
        List<GrossMarginModel> mlls = new List<GrossMarginModel>();
        foreach (var gros in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gros;
            grossMarginModel.GrossMarginNumber = Math.Round(ml / (1 - (gros / 100)), 2);

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
        /*List<SolutionQuotation> sol =
            await _solutionQutation.GetAllListAsync(p => p.AuditFlowId == processId && p.status == 0);
            List<Solution> solutions = ObjectMapper.Map<List<Solution>>(sol);
            */
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.Id == 115);
        QuotationListSecondDto pp = new QuotationListSecondDto
        {
            Date = DateTime.Now, //查询日期
            RecordNumber = priceEvaluationStartInputResult.Number, // 单据编号
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            //  OfferForm = priceEvaluationStartInputResult.DisplayName, //报价形式
            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            // ClientNature = priceEvaluationStartInputResult.DisplayName, //客户性质
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            // TerminalClientNature = priceEvaluationStartInputResult.DisplayName, //终端客户性质
            //开发计划 手工录入
            SopTime = priceEvaluationStartInputResult.SopTime, //Sop时间
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目周期
            ForSale = priceEvaluationStartInputResult.SalesType, //销售类型
            modeOfTrade = priceEvaluationStartInputResult.TradeMode, //贸易方式
            PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
            // QuoteCurrency = priceEvaluationStartInputResult.ExchangeRateKind, //报价币种
            //  ExchangeRate = priceEvaluationStartInputResult.ExchangeRate, //汇率
            ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
        };
        pp.nres = await getNre(processId, solutions);
        //获取核价营销相关数据
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);

        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        List<GradientGrossMarginModel> gradientGrossMarginModels = new();
        //获取毛利率
        List<decimal> gross = await GetGrossMargin();
        //sop年份
        //走量信息
        List<MotionMessageModel> messageModels = new List<MotionMessageModel>();

        foreach (var gradient in gradients)
        {
            MotionMessageModel motionMessageModel = new MotionMessageModel()
            {
                MessageName = gradient.GradientValue + "K/Y"
            };
            List<SopOrValueMode> sopOrValueModes = new();
            for (int i = 0; i < 5; i++)
            {
                SopOrValueMode sopOrValueMode = new()
                {
                };
                sopOrValueModes.Add(sopOrValueMode);
            }

            motionMessageModel.Sop = sopOrValueModes;

            messageModels.Add(motionMessageModel);
        }

        pp.MotionMessage = messageModels;

        //核心组件
        List<ComponenSocondModel> partsModels = new();
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "Sensor"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "Lens"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "ISP"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "串行芯片"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "线缆"
        });

        foreach (var parts in partsModels)
        {
            List<SolutionOrSpecification> Specifications = new();
            foreach (var solution in solutions)
            {
                SolutionOrSpecification solutionOrSpecification = new SolutionOrSpecification()
                {
                    solutionname = solution.SolutionName,
                    specification = "12"
                };
                Specifications.Add(solutionOrSpecification);
            }

            parts.Specifications = Specifications;
        }

        pp.componenSocondModels = partsModels;


        var soptime = priceEvaluationStartInputResult.SopTime;
        List<CreateSampleDto> sampleDtos = priceEvaluationStartInputResult.Sample;


        List<OnlySampleDto> samples = new List<OnlySampleDto>();
        //样品阶段
        foreach (var Solution in solutions)
        {
            var productld = Solution.Productld;
            var gepr = new GetPriceEvaluationTableResultInput();
            gepr.AuditFlowId = processId;
            gepr.Year = soptime;
            gepr.UpDown = YearType.Year;
            gepr.GradientId = mintd.Id;
            gepr.ProductId = productld;
            //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
            //    var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);接口弃用
            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = processId, GradientId = mintd.Id, InputCount = 0, SolutionId = Solution.Id,
                Year = soptime, UpDown = YearType.FirstHalf
            });

            //最小梯度SOP年成本
            var totalcost = ex.TotalCost;

            //样品阶段
            if (priceEvaluationStartInputResult.IsHasSample == true)
            {
                OnlySampleDto onlySampleDto = new();
                List<SampleQuotation> onlySampleModels =
                    await getSample(sampleDtos, totalcost);
                onlySampleDto.SolutionName = Solution.SolutionName;
                onlySampleDto.OnlySampleModels = onlySampleModels;
                samples.Add(onlySampleDto);
            }
        }

        pp.SampleOffer = samples;
//内部核价
        List<PricingMessageSecondModel> pricingMessageSecondModels = new List<PricingMessageSecondModel>();


        foreach (var Solution in solutions)
        {
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "BOM成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "制造成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "损耗成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "物流成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "MOQ分摊成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "质量成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "其他成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "总成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "备注"
            });
            foreach (var model in pricingMessageSecondModels)
            {
                List<SopOrAll> sopOrAlls = new();
                foreach (var gradient in gradients)
                {
                    SopOrAll sop = new();
                    sop.sop = 12;
                    sop.all = 24;
                    sop.GradientValue = gradient.GradientValue;
                    sopOrAlls.Add(sop);
                }

                model.sops = sopOrAlls;
            }
        }

        pp.pricingMessageSecondModels = pricingMessageSecondModels;

        //报价策略
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        foreach (var gradient in gradients)
        {
            foreach (var solution in solutions)
            {
                BiddingStrategySecondModel biddingStrategySecondModel = new()
                {
                    GradientId = gradient.Id, Product = solution.Product, gradient = gradient.GradientValue + "K/Y",
                    SopCost = 1, FullLifeCyclecost = 2, Price = 1, SopGrossMargin = 23, TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23, NreGrossMargin = 10
                };
                BiddingStrategySecondModels.Add(biddingStrategySecondModel);
            }
        }


        pp.BiddingStrategySecondModels = BiddingStrategySecondModels;
        return pp;
    }

    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long processId)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto = new();
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.Id == 115);

        List<UnitPriceSumModel> unitPriceSumModels = new();
        List<NREUnitSumModel> NreUnitSumModels = new();

        List<string> products = solutions.Select(p => p.Product).ToList();

        List<ManagerApprovalOfferNre> ManagerApprovalOfferNres = new List<ManagerApprovalOfferNre>();
        foreach (var product in products)
        {
            UnitPriceSumModel unitPriceSumModel = new UnitPriceSumModel()
            {
                Product = product
            };

            NREUnitSumModel nreUnitSumModel = new()
            {
                Product = product
            };
            List<SolutuionAndValue> prs = new();
            List<SolutuionAndValue> nre = new();
            foreach (var solution in solutions)
            {
                prs.Add(new SolutuionAndValue()
                {
                    SolutionId = solution.Id,
                    value = 5
                });
                nre.Add(new SolutuionAndValue()
                {
                    SolutionId = solution.Id,
                    value = 5
                });
            }


            unitPriceSumModel.solutuionAndValues = prs;
            nreUnitSumModel.solutuionAndValues = nre;
            unitPriceSumModels.Add(unitPriceSumModel);
            NreUnitSumModels.Add(nreUnitSumModel);
        }

        var nres = await getNre(processId, solutions);

        foreach (var solution in solutions)
        {
            ManagerApprovalOfferNre managerApprovalOfferNre = new()
            {
                solutionName = solution.SolutionName, SolutionId = solution.Id
            };
            List<ManagerApprovalOfferModel> managerApprovalOfferModels = new List<ManagerApprovalOfferModel>();
            foreach (var product in products)
            {
                ManagerApprovalOfferModel model = new ManagerApprovalOfferModel()
                {
                    Product = product, SopCost = 4, FullLifeCyclecost = 5, SalesRevenue = 5, SellingCost = 6,
                    GrossMargin = 12, Price = 10, Commission = 1, SopGrossMargin = 11, GrossMarginCommission = 11,
                    ClientGrossMargin = 12, NreGrossMargin = 10
                };
                managerApprovalOfferModels.Add(model);
            }

            AnalyseBoardNreDto nr = nres.First();
            managerApprovalOfferNre.analyseBoardNreDto = nr;
            managerApprovalOfferNre.ManagerApprovalOfferModels = managerApprovalOfferModels;
            ManagerApprovalOfferNres.Add(managerApprovalOfferNre);
        }

        managerApprovalOfferDto.UnitPriceSum = unitPriceSumModels;
        managerApprovalOfferDto.NreUnitSumModels = NreUnitSumModels;
        managerApprovalOfferDto.ManagerApprovalOfferNres = ManagerApprovalOfferNres;
        return managerApprovalOfferDto;
    }

    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long processId)
    {
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.Id == 115);
        QuotationListSecondDto pp = new QuotationListSecondDto
        {
            Date = DateTime.Now, //查询日期

            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            // ClientNature = priceEvaluationStartInputResult.DisplayName, //客户性质
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            // TerminalClientNature = priceEvaluationStartInputResult.DisplayName, //终端客户性质
            //开发计划 手工录入

            //  ExchangeRate = priceEvaluationStartInputResult.ExchangeRate, //汇率
        };
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);

        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        List<GradientGrossMarginModel> gradientGrossMarginModels = new();
        //获取毛利率
        List<decimal> gross = await GetGrossMargin();
        //sop年份
        //走量信息
        List<MotionMessageModel> messageModels = new List<MotionMessageModel>();

        foreach (var gradient in gradients)
        {
            MotionMessageModel motionMessageModel = new MotionMessageModel()
            {
                MessageName = gradient.GradientValue + "K/Y"
            };
            List<SopOrValueMode> sopOrValueModes = new();
            for (int i = 0; i < 5; i++)
            {
                SopOrValueMode sopOrValueMode = new()
                {
                };
                sopOrValueModes.Add(sopOrValueMode);
            }

            motionMessageModel.Sop = sopOrValueModes;

            messageModels.Add(motionMessageModel);
        }

        //核心组件
        List<ComponenSocondModel> partsModels = new();
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "Sensor"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "Lens"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "ISP"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "串行芯片"
        });
        partsModels.Add(new ComponenSocondModel()
        {
            CoreComponent = "线缆"
        });

        foreach (var parts in partsModels)
        {
            List<SolutionOrSpecification> Specifications = new();
            foreach (var solution in solutions)
            {
                SolutionOrSpecification solutionOrSpecification = new SolutionOrSpecification()
                {
                    solutionname = solution.SolutionName,
                    specification = "12"
                };
                Specifications.Add(solutionOrSpecification);
            }

            parts.Specifications = Specifications;
        }

        pp.componenSocondModels = partsModels;
        pp.MotionMessage = messageModels;
        var soptime = priceEvaluationStartInputResult.SopTime;
        List<CreateSampleDto> sampleDtos = priceEvaluationStartInputResult.Sample;

        List<OnlySampleDto> samples = new List<OnlySampleDto>();
        //样品阶段
        foreach (var Solution in solutions)
        {
            var productld = Solution.Productld;
            var gepr = new GetPriceEvaluationTableResultInput();
            gepr.AuditFlowId = processId;
            gepr.Year = soptime;
            gepr.UpDown = YearType.Year;
            gepr.GradientId = mintd.Id;
            gepr.ProductId = productld;
            //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
            //    var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);接口弃用
            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = processId, GradientId = mintd.Id, InputCount = 0, SolutionId = Solution.Id,
                Year = soptime, UpDown = YearType.FirstHalf
            });

            //最小梯度SOP年成本
            var totalcost = ex.TotalCost;

            //样品阶段
            if (priceEvaluationStartInputResult.IsHasSample == true)
            {
                OnlySampleDto onlySampleDto = new();
                List<SampleQuotation> onlySampleModels =
                    await getSample(sampleDtos, totalcost);
                onlySampleDto.SolutionName = Solution.SolutionName;
                onlySampleDto.OnlySampleModels = onlySampleModels;
                samples.Add(onlySampleDto);
            }
        }

        pp.SampleOffer = samples;

        //报价策略梯度
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        foreach (var gradient in gradients)
        {
            foreach (var solution in solutions)
            {
                BiddingStrategySecondModel biddingStrategySecondModel = new()
                {
                    GradientId = gradient.Id, Product = solution.Product, gradient = gradient.GradientValue + "K/Y",
                    SopCost = 1, FullLifeCyclecost = 2, Price = 1, SopGrossMargin = 23, TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23, NreGrossMargin = 10
                };
                BiddingStrategySecondModels.Add(biddingStrategySecondModel);
            }
        }

        pp.BiddingStrategySecondModels = BiddingStrategySecondModels;


        //报价策略梯度
        List<BiddingStrategySecondModel> BiddingStrategySecondModelsAct = new();
        foreach (var gradient in gradients)
        {
            foreach (var solution in solutions)
            {
                BiddingStrategySecondModel biddingStrategySecondModel = new()
                {
                    Product = solution.Product,
                    SopCost = 1, FullLifeCyclecost = 2, Price = 1, SopGrossMargin = 23, TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23, NreGrossMargin = 10
                };
                BiddingStrategySecondModelsAct.Add(biddingStrategySecondModel);
            }
        }

        pp.BiddingStrategySecondModelsAct = BiddingStrategySecondModelsAct;

        //内部核价
        List<PricingMessageSecondModel> pricingMessageSecondModels = new List<PricingMessageSecondModel>();


        foreach (var Solution in solutions)
        {
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "BOM成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "制造成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "损耗成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "物流成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "MOQ分摊成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "质量成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "其他成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "总成本"
            });
            pricingMessageSecondModels.Add(new PricingMessageSecondModel()
            {
                SolutionName = Solution.SolutionName,
                SolutionId = Solution.Id,
                Name = "备注"
            });
            foreach (var model in pricingMessageSecondModels)
            {
                List<SopOrAll> sopOrAlls = new();
                foreach (var gradient in gradients)
                {
                    SopOrAll sop = new();
                    sop.sop = 12;
                    sop.all = 24;
                    sop.GradientValue = gradient.GradientValue;
                    sopOrAlls.Add(sop);
                }

                model.sops = sopOrAlls;
            }
        }

        pp.pricingMessageSecondModels = pricingMessageSecondModels;
        return pp;
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

    public async Task<ExternalQuotationDto> GetExternalQuotation(long processId)
    {
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);


        ExternalQuotationDto externalQuotationDto = new()
        {
            CustomerName = priceEvaluationStartInputResult.CustomerName,
            ProjectName = priceEvaluationStartInputResult.ProjectName,
            SopTime = priceEvaluationStartInputResult.SopTime,
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle
        };

        List<CreateColumnFormatProductInformationDto> createColumnFormatProductInformationDtos =
            priceEvaluationStartInputResult.ProductInformation;
        List<ExternalQuotationMxDto> mxs = (from crm in createColumnFormatProductInformationDtos
            select new ExternalQuotationMxDto()
            {
                productName = crm.Name, year = priceEvaluationStartInputResult.SopTime.ToString(),

                amout = priceEvaluationStartInputResult.ModelCount.Sum(p => p.SumQuantity)
            }).ToList();

        externalQuotationDto.mxs = mxs;

        List<SopOrValueMode> sopls = new List<SopOrValueMode>();
        sopls.Add(new SopOrValueMode()
        {
            Year = priceEvaluationStartInputResult.SopTime,
            Value = 11
        });
        sopls.Add(new SopOrValueMode()
        {
            Year = priceEvaluationStartInputResult.SopTime + 1,
            Value = 11
        });
        sopls.Add(new SopOrValueMode()
        {
            Year = priceEvaluationStartInputResult.SopTime + 2,
            Value = 11
        });

        externalQuotationDto.sopls = sopls;
        return externalQuotationDto;
    }

    public async Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(long processId)
    {
        CoreComponentAndNreDto coreComponentAndNreDto = new();
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        //梯度
        var gradients = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);
        List<Solution> solutions = await _resourceSchemeTable.GetAllListAsync(p => p.Id == 115);


        List<productAndGradient> ProductAndGradients = new();
        List<String> products = new()
        {
            "Sensor芯片", "串行芯片", " 镜头", "PCBA（除sensor芯片、串行芯片）", "结构件（除lens）", "质量成本", "损耗成本", "制造成本", "物流费用", "其他成本",
            "总成本"
        };

        foreach (var gradient in gradients)
        {
            foreach (var product in products)
            {
                ProductAndGradients.Add(new productAndGradient()
                {
                    GradientId = gradient.Id,
                    GradientValue = gradient.GradientValue,
                    Product = product,
                    solutionAndprices = (from solution in solutions
                        select new SolutionAndprice()
                        {
                            solutionName = solution.SolutionName,
                            SolutionId = solution.Id,
                            Number = 1,
                            Price = 100,
                            ExchangeRate = 1,
                            nsum = 12
                        }).ToList()
                });
            }
        }


        List<String> nree = new()
        {
            "手板件费用", "模具费用", " 工装费用", "治具费用", "检具费用", "生产设备费用", "专用生产设备", "非专用生产设备", "实验费用", "测试软件费用", "差旅费", "其他费用",
            "合计"
        };
        List<NreExpense> nres = (from nrerr in nree
            select new NreExpense()
            {
                nre = nrerr, price = 100, remark = "12"
            }).ToList();
        coreComponentAndNreDto.nres = nres;
        coreComponentAndNreDto.ProductAndGradients = ProductAndGradients;
        return coreComponentAndNreDto;
    }
}