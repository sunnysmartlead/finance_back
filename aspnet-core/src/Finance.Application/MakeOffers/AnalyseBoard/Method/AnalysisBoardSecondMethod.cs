using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.Ext;
using Finance.FinanceMaintain;
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
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.WorkFlows.Dto;
using Finance.WorkFlows;
using Interface.Expends;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.EventSource;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
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
    /// 核价梯度相关
    /// </summary>
    private readonly IRepository<Gradient, long> _gradientRepository;


    /// <summary>
    /// 报价分析看板中的 汇总分析表  实体类
    /// </summary>
    private readonly IRepository<PooledAnalysisOffers, long> _resourcePooledAnalysisOffers;


    /// <summary>
    /// 营销部审核中方案表
    /// </summary>
    public readonly IRepository<Solution, long> _resourceSchemeTable;

    /// <summary>
    ///报价 项目看板实体类 实体类
    /// </summary>
    private readonly IRepository<ProjectBoardOffers, long> _resourceProjectBoardOffers;

    /// <summary>
    ///报价 项目看板实体类 实体类
    /// </summary>
    private readonly IRepository<ProjectBoardSecondOffers, long> _resourceProjectBoardSecondOffers;

    /// <summary>
    /// 财务维护 毛利率方案
    /// </summary>
    private readonly IRepository<GrossMarginForm, long> _resourceGrossMarginForm;

    /// <summary>
    /// 财务维护 汇率录入表
    /// </summary>
    private readonly IRepository<ExchangeRate, long> _exchangeRate;

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
    /// 报价毛利率测算-阶梯数量
    /// </summary>
    private readonly IRepository<GradientGrossCalculate, long> _actualUnitPriceOffer;

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
    /// 报价审核表
    /// </summary>
    private readonly IRepository<AuditQuotationList, long> _financeAuditQuotationList;

    private readonly ProcessHoursEnterDeviceAppService _processHoursEnterDeviceAppService;

    /// <summary>
    /// 对外报价单
    /// </summary>
    private readonly IRepository<ExternalQuotation, long> _externalQuotation;

    /// <summary>
    /// 产品报价清单实体类
    /// </summary>
    private readonly IRepository<ProductExternalQuotationMx, long> _externalQuotationMx;

    /// <summary>
    /// NRE报价清单实体类
    /// </summary>
    private readonly IRepository<NreQuotationList, long> _NreQuotationList;

    private readonly WorkflowInstanceAppService _workflowInstanceAppService;


    public AnalysisBoardSecondMethod(IRepository<ModelCountYear, long> modelCountYear,
        IRepository<PriceEvaluation, long> priceEvaluationRepository,
        IRepository<ModelCount, long> modelCount,
        IRepository<DeviceQuotation, long> deviceQuotation,
        IRepository<UnitPriceOffers, long> resourceUnitPriceOffers,
        IRepository<SampleQuotation, long> sampleQuotation,
        IRepository<NreQuotation, long> nreQuotation,
        IRepository<ProjectBoardSecondOffers, long> resourceProjectBoardSecondOffers,
        IRepository<GradientGrossCalculate, long> actualUnitPriceOffer,
        IRepository<SolutionQuotation, long> solutionQutation,
        IRepository<ExchangeRate, long> exchangeRate,
        PriceEvaluationAppService priceEvaluationAppService,
        ElectronicBomAppService electronicBomAppService,
        StructionBomAppService structionBomAppService,
        IRepository<AuditQuotationList, long> financeAuditQuotationList,
        IRepository<PooledAnalysisOffers, long> resourcePooledAnalysisOffers,
        IRepository<Solution, long> resourceSchemeTable,
        IRepository<FinanceDictionary, string> financeDictionaryRepository,
        IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
        IRepository<Gradient, long> gradientRepository,
        IRepository<ProjectBoardOffers, long> resourceProjectBoardOffers,
        ProcessHoursEnterDeviceAppService processHoursEnterDeviceAppService,
        IRepository<DynamicUnitPriceOffers, long> dynamicUnitPriceOffers,
        NrePricingAppService nrePricingAppService,
        IRepository<GrossMarginForm, long> resourceGrossMarginForm,
        IRepository<ExternalQuotation, long> externalQuotation,
        IRepository<ProductExternalQuotationMx, long> externalQuotationMx,
        IRepository<NreQuotationList, long> nreQuotationList,
        WorkflowInstanceAppService workflowInstanceAppService)
    {
        _resourceProjectBoardOffers = resourceProjectBoardOffers;
        _dynamicUnitPriceOffers = dynamicUnitPriceOffers;
        _resourcePooledAnalysisOffers = resourcePooledAnalysisOffers;
        _resourceUnitPriceOffers = resourceUnitPriceOffers;
        _resourceSchemeTable = resourceSchemeTable;
        _financeDictionaryRepository = financeDictionaryRepository;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _priceEvaluationAppService = priceEvaluationAppService;
        _solutionQutation = solutionQutation;
        _exchangeRate = exchangeRate;
        _gradientRepository = gradientRepository;
        _deviceQuotation = deviceQuotation;
        _actualUnitPriceOffer = actualUnitPriceOffer;
        _sampleQuotation = sampleQuotation;
        _nreQuotation = nreQuotation;
        _financeAuditQuotationList = financeAuditQuotationList;
        _resourceProjectBoardSecondOffers = resourceProjectBoardSecondOffers;
        _processHoursEnterDeviceAppService = processHoursEnterDeviceAppService;
        _nrePricingAppService = nrePricingAppService;
        _electronicBomAppService = electronicBomAppService;
        _structionBomAppService = structionBomAppService;
        _priceEvaluationRepository = priceEvaluationRepository;
        _resourceGrossMarginForm = resourceGrossMarginForm;
        _resourceModelCountYear = modelCountYear;
        _externalQuotation = externalQuotation;
        _externalQuotationMx = externalQuotationMx;
        _NreQuotationList = nreQuotationList;
        _workflowInstanceAppService = workflowInstanceAppService;
    }

    public async Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new();
        var auditFlowId = analyseBoardSecondInputDto.auditFlowId;
        //获取方案
        List<Solution> Solutions = analyseBoardSecondInputDto.solutionTables;
        var solutiondict = Solutions.ToDictionary(p => p.ModuleName);
        var ntime = analyseBoardSecondInputDto.ntime; //报价次数
        var ntype = analyseBoardSecondInputDto.ntype; // 0 报价分析看板，1 报价反馈
        int last = ntime - 1; //上一个提交的版本


        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(analyseBoardSecondInputDto.auditFlowId);
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        var valuedict = gradients.ToDictionary(p => p.GradientValue);

        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        var yearList =
            await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId);
        var sopYear = yearList.MinBy(p => p.Year);
        var soptime = sopYear.Year;
        var sopTimeType = sopYear.UpDown;
        List<CreateSampleDto> sampleDtos = priceEvaluationStartInputResult.Sample;
        List<OnlySampleDto> samples = new List<OnlySampleDto>();
        //判断是否是仅含样品
        var pricetype = priceEvaluationStartInputResult.PriceEvalType;
        if ("PriceEvalType_Sample".Equals(pricetype))
        {
            foreach (var Solution in Solutions)
            {
                //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
                //    var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);接口弃用
                ExcelPriceEvaluationTableDto ex = new ExcelPriceEvaluationTableDto();
                try
                {
                    ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                    {
                        AuditFlowId = auditFlowId,
                        GradientId = mintd.Id,
                        InputCount = 0,
                        SolutionId = Solution.Id,
                        Year = soptime,
                        UpDown = sopTimeType
                    });
                }
                catch (Exception e)
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
                        await getSample(sampleDtos, totalcost, auditFlowId, Solution.Id);
                    onlySampleDto.SolutionName = Solution.Product;
                    onlySampleDto.SolutionId = Solution.Id;
                    onlySampleDto.AuditFlowId = auditFlowId;
                    onlySampleDto.OnlySampleModels = onlySampleModels;
                    samples.Add(onlySampleDto);
                }
            }

            analyseBoardSecondDto.SampleOffer = samples;

            return analyseBoardSecondDto;
        }


        //获取毛利率
        List<decimal> gross = await GetGrossMargin();
        //sop年份
        //var soptime = priceEvaluationStartInputResult.SopTime;

        //单价表
        List<SopAnalysisModel> sops = new List<SopAnalysisModel>();
        //样品阶段
        foreach (var Solution in Solutions)
        {
            //获取核价看板，sop年份数据,参数：年份、年份类型、梯度Id、模组Id,TotalCost为总成本,列表Material中，IsCustomerSupply为True的是客供料，TotalMoneyCyn是客供料的成本列表OtherCostItem2中，ItemName值等于【单颗成本】的项，Total是分摊成本
            //    var ex = await _priceEvaluationGetAppService.GetPriceEvaluationTableResult(gepr);接口弃用
            ExcelPriceEvaluationTableDto ex = new ExcelPriceEvaluationTableDto();
            try
            {
                ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = auditFlowId,
                    GradientId = mintd.Id,
                    InputCount = 0,
                    SolutionId = Solution.Id,
                    Year = soptime,
                    UpDown = sopTimeType
                });
            }
            catch (Exception e)
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
                    await getSample(sampleDtos, totalcost, auditFlowId, Solution.Id);
                onlySampleDto.SolutionName = Solution.Product;
                onlySampleDto.SolutionId = Solution.Id;
                onlySampleDto.OnlySampleModels = onlySampleModels;
                samples.Add(onlySampleDto);
            }


            foreach (var gradient in gradients)
            {
                ExcelPriceEvaluationTableDto sop = await _priceEvaluationAppService.GetPriceEvaluationTable(
                    new GetPriceEvaluationTableInput
                    {
                        AuditFlowId = auditFlowId,
                        GradientId = gradient.Id,
                        InputCount = 0,
                        SolutionId = Solution.Id,
                        Year = soptime,
                        UpDown = sopTimeType
                    });

                SopAnalysisModel sopAnalysisModel = new();
                sopAnalysisModel.Product = Solution.ModuleName;
                sopAnalysisModel.GradientId = gradient.Id;
                sopAnalysisModel.AuditFlowId = auditFlowId;
                sopAnalysisModel.GradientValue = gradient.GradientValue + "K/Y";
                List<GrossValue> grosss = new List<GrossValue>();
                var sopTotalCost = sop.TotalCost;
                foreach (var gro in gross)
                {
                    GrossValue gr = new GrossValue();
                    gr.Grossvalue = Math.Round(sopTotalCost / (1 - (gro / 100)), 2);
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
        //汇总分析
        List<PooledAnalysisModel> FullLifeCycle = new List<PooledAnalysisModel>();
        List<CreateRequirementDto> createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        PooledAnalysisModel pooledAnalysisModelsl = new(); //销售数量
        pooledAnalysisModelsl.ProjectName = "数量";
        pooledAnalysisModelsl.AuditFlowId = auditFlowId;

        var nsum = priceEvaluationStartInputResult.ModelCount.Sum(e => e.SumQuantity);
        List<GrossMarginModel> xssl = new List<GrossMarginModel>();
        List<GrossMarginModel> xscbs = new List<GrossMarginModel>();
        PooledAnalysisModel pooledAnalysisModelxscb = new(); ////销售销售成本
        pooledAnalysisModelxscb.ProjectName = "销售成本";
        pooledAnalysisModelxscb.AuditFlowId = auditFlowId;
        PooledAnalysisModel pooledAnalysisModelpjcb = new();
        pooledAnalysisModelpjcb.ProjectName = "单位平均成本";
        pooledAnalysisModelpjcb.AuditFlowId = auditFlowId;

        List<GrossMarginModel> pjcbs = new List<GrossMarginModel>();

        PooledAnalysisModel pooledAnalysisModelflxssr = new();
        pooledAnalysisModelflxssr.ProjectName = "返利后销售收入";
        pooledAnalysisModelflxssr.AuditFlowId = auditFlowId;

        List<GrossMarginModel> fls = new();
        PooledAnalysisModel pooledAnalysisModelpjdj = new();
        pooledAnalysisModelpjdj.ProjectName = "平均单价";
        pooledAnalysisModelpjdj.AuditFlowId = auditFlowId;

        List<GrossMarginModel> pjdjs = new List<GrossMarginModel>();

        PooledAnalysisModel pooledAnalysisModelyj = new();
        pooledAnalysisModelyj.ProjectName = "佣金";
        pooledAnalysisModelyj.AuditFlowId = auditFlowId;

        List<GrossMarginModel> yjs = new List<GrossMarginModel>();
        List<GrossMarginModel> yjss = new List<GrossMarginModel>();


        PooledAnalysisModel pooledAnalysisModelxsml = new();
        pooledAnalysisModelxsml.ProjectName = "销售毛利";
        pooledAnalysisModelxsml.AuditFlowId = auditFlowId;

        List<GrossMarginModel> xsmls = new List<GrossMarginModel>();
        PooledAnalysisModel pooledAnalysisModelmll = new();
        pooledAnalysisModelmll.ProjectName = "毛利率";
        pooledAnalysisModelmll.AuditFlowId = auditFlowId;
        List<GrossMarginModel> mlls = new List<GrossMarginModel>();

        //
        var modelcoutnlist = priceEvaluationStartInputResult.ModelCount;
        var cost = new decimal(0);

        foreach (var modelcount in modelcoutnlist)
        {
            //
            var modellist = modelcount.ModelCountYearList;
            var jg = solutiondict.ContainsKey(modelcount.Product);
            if (!jg)
            {
                continue;
            }

            var solution = solutiondict[modelcount.Product];


            var solutionid = solution.Id; //获取对应方案id


            foreach (var model in modellist)
            {
                var sop = new SopAnalysisModel();

                long gradientid = GetGradient(model.Quantity, gradients);


                //获取核价看板的值
                var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradientid,
                    InputCount = 0,
                    SolutionId = solutionid,
                    Year = model.Year,
                    UpDown = model.UpDown
                });

                var dwcb = ex.TotalCost; //每个成本

                cost += dwcb * model.Quantity; //将遍历的成本*数量加起来，得到成本
            }
        }


        foreach (var gro in gross)
        {
            GrossMarginModel grossMarginModel = new GrossMarginModel();
            grossMarginModel.GrossMargin = gro;
            grossMarginModel.GrossMarginNumber = nsum;
            xssl.Add(grossMarginModel);
            GrossMarginModel xscb = new GrossMarginModel();
            xscb.GrossMargin = gro;
            xscb.GrossMarginNumber = cost;
            xscbs.Add(xscb);

            GrossMarginModel pjcb = new GrossMarginModel();
            pjcb.GrossMargin = gro;
            pjcb.GrossMarginNumber = cost / nsum;
            pjcbs.Add(pjcb);


            GrossMarginModel fl = new GrossMarginModel();

            decimal zsr = 0;
            decimal yj = 0;

            //

            foreach (var modelCountDto in modelcoutnlist)
            {
                var jg = solutiondict.ContainsKey(modelCountDto.Product);
                if (!jg)
                {
                    continue;
                }

                var solution = solutiondict[modelCountDto.Product];
                var mcyl = modelCountDto.ModelCountYearList;
                decimal njl = 1;
                foreach (var mc in mcyl)
                {
                    long gradientid = GetGradient(mc.Quantity, gradients);
                    var sop = sops.FindFirst(p =>
                        p.Product.Equals(modelCountDto.Product) && p.GradientId == gradientid);

                    var dj = sop.GrossValues.FindFirst(p => p.Gross.Equals(gro.ToString()))
                        .Grossvalue; //获取产品实际数量落在的梯度的sop单价

                    var createRequirementDto =
                        createRequirementDtos.FindFirst(p =>
                            p.Year == mc.Year && p.UpDown.Equals(mc.UpDown)); //获取对应年份
                    njl = njl * (1 - createRequirementDto.AnnualDeclineRate / 100); //年降率
                    zsr += njl * dj * mc.Quantity * (1 - createRequirementDto.OneTimeDiscountRate / 100) *
                           (1 - createRequirementDto.AnnualRebateRequirements / 100);
                    yj += njl * dj * mc.Quantity * (createRequirementDto.CommissionRate / 100);
                }
            }

            fl.GrossMarginNumber = zsr;
            fls.Add(fl);

            GrossMarginModel pjdj = new GrossMarginModel();
            pjdj.GrossMargin = gro;
            pjdj.GrossMarginNumber = zsr / nsum;
            pjdjs.Add(pjdj);
            GrossMarginModel yjsss = new GrossMarginModel();
            yjsss.GrossMargin = gro;
            yjsss.GrossMarginNumber = yj;


            yjss.Add(yjsss);


            GrossMarginModel ml = new GrossMarginModel();
            ml.GrossMargin = gro;
            ml.GrossMarginNumber = zsr - cost - yj;
            xsmls.Add(ml);
            GrossMarginModel mll = new GrossMarginModel();
            mll.GrossMargin = gro;
            mll.GrossMarginNumber = ml.GrossMarginNumber / zsr;
            mlls.Add(mll);
        }


        pooledAnalysisModelsl.GrossMarginList = xssl;
        FullLifeCycle.Add(pooledAnalysisModelsl);
        pooledAnalysisModelxscb.GrossMarginList = xscbs;
        FullLifeCycle.Add(pooledAnalysisModelxscb);
        pooledAnalysisModelpjcb.GrossMarginList = pjcbs;
        FullLifeCycle.Add(pooledAnalysisModelpjcb);
        pooledAnalysisModelflxssr.GrossMarginList = fls;
        FullLifeCycle.Add(pooledAnalysisModelflxssr);
        pooledAnalysisModelpjdj.GrossMarginList = pjdjs;
        FullLifeCycle.Add(pooledAnalysisModelpjdj);
        pooledAnalysisModelyj.GrossMarginList = yjss;
        FullLifeCycle.Add(pooledAnalysisModelyj);
        pooledAnalysisModelxsml.GrossMarginList = xsmls;
        FullLifeCycle.Add(pooledAnalysisModelxsml);
        pooledAnalysisModelmll.GrossMarginList = mlls;
        FullLifeCycle.Add(pooledAnalysisModelmll);
        analyseBoardSecondDto.FullLifeCycle = FullLifeCycle;


        //阶梯数量&&看板阶梯数量
        List<GradientGrossMarginCalculateModel> gradientQuotedGrossMarginModels = new();
        //看板
        List<BoardModel> boardModels = new();
        foreach (var gradient in gradients)
        {
            BoardModel boardModel = new();

            List<ProjectBoardSecondModel> projectBoardSecondModels = new();
            decimal mbnbsl = 0; //目标内部
            decimal mbnbxscb = 0;
            decimal mbnbxssr = 0;
            decimal mbnbml = 0;
            decimal mbnbyj = 0;


            decimal mbkhsl = 0; //目标客户
            decimal mbkhxscb = 0;
            decimal mbkhxssr = 0;
            decimal mbkhml = 0;
            decimal mbkhyj = 0;
            foreach (var solution in Solutions)
            {
                //获取核价看板的值
                var jtex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    InputCount = 0,
                    SolutionId = solution.Id,
                    Year = soptime,
                    UpDown = sopTimeType
                });
                var jttotalcost = jtex.TotalCost;
                CreateModelCountDto cto =
                    priceEvaluationStartInputResult.ModelCount.FindFirst(p => p.Product.Equals(solution.ModuleName));
                var pt = cto.ProductTypeName;


                decimal ml = 0;
                if ("环境感知".Equals((pt)))
                {
                    ml = 25;
                }
                else if ("外摄显像".Equals((pt)))
                {
                    ml = 15;
                }
                else if ("舱内监测".Equals((pt)))
                {
                    ml = 20;
                }
                else
                {
                    ml = 20;
                }

                var InteriorPrice = Math.Round(jttotalcost / (1 - ml / 100), 2); //目标价内部单价

                var jtsl = await PostGrossMarginForGradient(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    UnitPrice = InteriorPrice
                });

                mbnbsl += jtsl.sl;
                mbnbxscb += jtsl.xscb;
                mbnbxssr += jtsl.xssr;
                mbnbml += jtsl.xsml;
                mbnbyj += jtsl.yj;
                decimal mbj = 0;
                if (priceEvaluationStartInputResult.CustomerTargetPrice is not null)
                {
                    CreateCustomerTargetPriceDto ctp = priceEvaluationStartInputResult.CustomerTargetPrice.FindFirst(
                        p =>
                            p.Kv == gradient.GradientValue && p.Product.Equals(solution.ModuleName));
                    mbj = ctp.ExchangeRate == null ? 0 : (Convert.ToDecimal(ctp.TargetPrice)) * ctp.ExchangeRate.Value;
                }

                var khmbjex = await PostGrossMarginForGradient(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    UnitPrice = mbj
                });
                mbkhsl += khmbjex.sl;
                mbkhxscb += khmbjex.xscb;
                mbkhxssr += khmbjex.xssr;
                mbkhml += khmbjex.xsml;
                mbkhyj += khmbjex.yj;


                GradientGrossMarginCalculateModel model = new()
                {
                    GradientId = gradient.Id,
                    gradient = gradient.GradientValue + (sopTimeType.Equals(YearType.Year) ? "K/Y" : "K/HY"),
                    SolutionId = solution.Id,
                    product = solution.Product,
                    AuditFlowId = auditFlowId,
                    InteriorPrice = InteriorPrice,
                    InteriorGrossMargin = ml, //目标价内部
                    InteriorClientGrossMargin = jtsl.ClientGrossMargin,
                    InteriorNreGrossMargin = jtsl.NreGrossMargin,
                    ClientPrice = mbj, //目标价（客户）
                    ClientGrossMargin = khmbjex.GrossMargin,
                    ClientClientGrossMargin = khmbjex.ClientGrossMargin,
                    ClientNreGrossMargin = khmbjex.NreGrossMargin
                };

                if (last > 0)
                {
                    var lastlist = (from lastjts in await _actualUnitPriceOffer.GetAllListAsync(p =>
                            p.AuditFlowId == auditFlowId && p.ntype == ntype && p.GradientId == gradient.Id &&
                            p.SolutionId.Equals(solution.Id))
                                    join sol in await _solutionQutation.GetAllListAsync(p =>
                                        p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                    select lastjts).ToList();
                    var lastjt = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    if (lastjt is not null)
                    {
                        model.LastRoundPrice = lastjt.OfferUnitPrice;
                        model.LastRoundGrossMargin = lastjt.OfferGrossMargin;
                        model.LastRoundClientGrossMargin = lastjt.OfferClientGrossMargin;
                        model.LastRoundNreGrossMargin = lastjt.OfferNreGrossMargin;
                    }
                }

                gradientQuotedGrossMarginModels.Add(model);
            }


            ProjectBoardSecondModel sl = new();
            sl.ProjectName = "数量";
            sl.AuditFlowId = auditFlowId;
            sl.GradientId = gradient.Id;
            sl.InteriorTarget = mbnbsl;
            sl.ClientTarget = mbkhsl;
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("数量") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    sl.OldOffer = lastbord.Offer;
                }
            }

            projectBoardSecondModels.Add(sl);

            ProjectBoardSecondModel xscb = new();
            xscb.ProjectName = "销售成本";
            xscb.AuditFlowId = auditFlowId;
            xscb.GradientId = gradient.Id;
            xscb.InteriorTarget = mbnbxscb;
            xscb.ClientTarget = mbkhxscb;
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("销售成本") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();

                if (lastbord is not null)
                {
                    xscb.OldOffer = lastbord.Offer;
                }
            }

            projectBoardSecondModels.Add(xscb);

            ProjectBoardSecondModel dwpjcb = new();
            dwpjcb.ProjectName = "单位平均成本";
            dwpjcb.GradientId = gradient.Id;
            dwpjcb.AuditFlowId = auditFlowId;

            dwpjcb.InteriorTarget = Math.Round(mbnbxscb / mbnbsl, 2);
            dwpjcb.ClientTarget = Math.Round(mbkhxscb / mbkhsl, 2);
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("单位平均成本") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();

                if (lastbord is not null)
                {
                    dwpjcb.OldOffer = lastbord.Offer;
                }
            }

            projectBoardSecondModels.Add(dwpjcb);

            ProjectBoardSecondModel xssr = new();
            xssr.ProjectName = "销售收入";
            xssr.AuditFlowId = auditFlowId;
            xssr.GradientId = gradient.Id;
            xssr.InteriorTarget = Math.Round(mbnbxssr, 2);
            xssr.ClientTarget = Math.Round(mbkhxssr, 2);
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("销售收入") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    xssr.OldOffer = lastbord.Offer.Value;
                }
            }

            projectBoardSecondModels.Add(xssr);
            ProjectBoardSecondModel yj = new();
            yj.ProjectName = "佣金";
            yj.AuditFlowId = auditFlowId;
            yj.GradientId = gradient.Id;
            yj.InteriorTarget = mbnbyj;
            yj.ClientTarget = mbkhyj;
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("佣金") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    yj.OldOffer = lastbord.Offer;
                }
            }

            projectBoardSecondModels.Add(yj);
            ProjectBoardSecondModel pjdj = new();
            pjdj.ProjectName = "平均单价";
            pjdj.AuditFlowId = auditFlowId;
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("平均单价") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    pjdj.OldOffer = lastbord.Offer;
                }
            }

            pjdj.GradientId = gradient.Id;
            pjdj.InteriorTarget = Math.Round(mbnbxssr / mbnbsl, 2);
            pjdj.ClientTarget = Math.Round(mbkhxssr / mbkhsl, 2);

            projectBoardSecondModels.Add(pjdj);

            ProjectBoardSecondModel xsml = new();
            xsml.ProjectName = "销售毛利";
            xsml.AuditFlowId = auditFlowId;
            xsml.GradientId = gradient.Id;
            xsml.InteriorTarget = Math.Round(mbnbml, 2);
            xsml.ClientTarget = Math.Round(mbkhml, 2);
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("销售毛利") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    xsml.OldOffer = lastbord.Offer;
                }
            }

            projectBoardSecondModels.Add(xsml);
            ProjectBoardSecondModel mll = new();
            mll.ProjectName = "毛利率";
            mll.AuditFlowId = auditFlowId;
            if (last > 0)
            {
                var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId && p.GradientId == gradient.Id && p.ProjectName.Equals("毛利率") &&
                        p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastbord is not null)
                {
                    mll.OldOffer = lastbord.Offer;
                }
            }

            mll.GradientId = gradient.Id;
            mll.InteriorTarget = Math.Round((mbnbml / mbnbxssr) * 100, 2);
            mll.ClientTarget = (mbkhxssr == 0) ? 0 : Math.Round((mbkhml / mbkhxssr) * 100, 2);

            projectBoardSecondModels.Add(mll);
            boardModel.ProjectBoardModels = projectBoardSecondModels;
            boardModel.GradientId = gradient.Id;
            boardModel.title = gradient.GradientValue + (sopTimeType.Equals(YearType.Year) ? "K/Y" : "K/HY");
            boardModels.Add(boardModel);
        }


        analyseBoardSecondDto.GradientQuotedGrossMargins = gradientQuotedGrossMarginModels;
        //报价实际数量测算
        var carmodelcouts = priceEvaluationStartInputResult.CarModelCount;
        List<QuotedGrossMarginActualModel> quotedGrossMarginActualModels = new();
        //存在多个车型的可能
        Dictionary<String, List<CreateCarModelCountDto>> cardicts = carmodelcouts.GroupBy(p => p.CarModel)
            .ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        foreach (var cardict in cardicts)
        {
            string key = cardict.Key;
            List<CreateCarModelCountDto> createCarModelCountDtos = cardict.Value;
            QuotedGrossMarginActualModel quotedGrossMarginActualModel = new QuotedGrossMarginActualModel();
            quotedGrossMarginActualModel.project = "报价毛利率测算-实际数量-" + key;
            decimal qtmbnbdj = 0; //目标价内部和客户单价
            decimal qtmbkhdj = 0;

            decimal qtnbsr = 0; //内部客供毛利和客供收入，分摊收入分毛利
            decimal qtnbml = 0;
            decimal qtnbkgsr = 0;
            decimal qtnbkgml = 0;
            decimal qtnbftss = 0;
            decimal qtnbftml = 0;
            //客户分摊毛利和客供收入分摊收入分毛利
            decimal qtkhsr = 0;
            decimal qtkhml = 0;
            decimal qtkhkgml = 0;
            decimal qtkhkgsr = 0;
            decimal qtkhftss = 0;
            decimal qtkhftml = 0;
            List<QuotedGrossMarginActual> QuotedGrossMarginActualList = new();
            int cnum = 0;
            foreach (var solution in Solutions)
            {
                var cdto = createCarModelCountDtos
                    .FindFirst(p => p.CarModel.Equals(key) && p.Product.Equals(solution.ModuleName));
                if (cdto is null)
                {
                    continue;
                }

                var carnum = cdto.SingleCarProductsQuantity;
                cnum += carnum;
                var modelCountDto = modelcoutnlist.FindFirst(p => p.Product.Equals(solution.ModuleName)); //获取第一年的数量
                var modelCount = modelCountDto.ModelCountYearList.OrderBy(p => p.Year).ThenBy(p => p.UpDown).First();

                long gradientid = GetGradient(modelCount.Quantity, gradients);

                var gradq = gradientQuotedGrossMarginModels.FindFirst(p =>
                    p.GradientId == gradientid && p.product.Equals(solution.Product)); //第一年的单价

                var nmj = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = auditFlowId,
                    CarModel = key,
                    SolutionId = solution.Id,
                    SoltionGradPrices = (from gradientqgs in gradientQuotedGrossMarginModels
                                         select new SoltionGradPrice()
                                         {
                                             Gradientid = gradientqgs.GradientId,
                                             SolutionId = gradientqgs.SolutionId,
                                             UnitPrice = gradientqgs.InteriorPrice
                                         }).ToList()
                });

                var khj = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = auditFlowId,
                    CarModel = key,
                    SolutionId = solution.Id,
                    SoltionGradPrices = (from gradientqgs in gradientQuotedGrossMarginModels
                                         select new SoltionGradPrice()
                                         {
                                             Gradientid = gradientqgs.GradientId,
                                             SolutionId = gradientqgs.SolutionId,
                                             UnitPrice = gradientqgs.ClientPrice
                                         }).ToList()
                });

                QuotedGrossMarginActual quotedGrossMarginActual = new QuotedGrossMarginActual()
                {
                    carModel = key,
                    AuditFlowId = auditFlowId,
                    carNum = carnum,
                    SolutionId = solution.Id,
                    product = solution.Product,
                    InteriorPrice = gradq.InteriorPrice,
                    InteriorGrossMargin = nmj.GrossMargin,
                    InteriorNreGrossMargin = nmj.NreGrossMargin,
                    InteriorClientGrossMargin = nmj.ClientGrossMargin,
                    ClientPrice = gradq.ClientPrice,
                    ClientGrossMargin = khj.GrossMargin,
                    ClientClientGrossMargin = khj.ClientGrossMargin,
                    ClientNreGrossMargin = khj.NreGrossMargin
                };
                qtmbnbdj += (carnum * gradq.InteriorPrice);
                qtmbkhdj += (carnum * gradq.ClientPrice);
                qtnbml += nmj.xsml;
                qtnbsr += nmj.xssr;
                qtnbkgsr += nmj.kgxssr;
                qtnbkgml += nmj.kgxsml;
                qtnbftss += nmj.ftxssr;
                qtnbftml += nmj.ftxsml;
                qtkhkgsr += khj.xssr;
                qtkhsr += khj.xssr;
                qtkhml += khj.xsml;
                qtkhkgml += khj.kgxsml;
                qtkhkgsr += khj.kgxssr;
                qtkhftss += khj.ftxssr;
                qtkhftml += khj.ftxsml;
                if (last > 0)
                {
                    var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                            p.AuditFlowId == auditFlowId &&
                            p.SolutionId.Equals(solution.Id) && p.carModel.Equals(key) && p.ntype == ntype)
                                    join sol in await _solutionQutation.GetAllListAsync(p =>
                                        p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                    select lastjts).ToList();
                    var lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    if (lastsj is not null)
                    {
                        quotedGrossMarginActual.LastRoundPrice = lastsj.OfferUnitPrice;
                        quotedGrossMarginActual.LastRoundGrossMargin = lastsj.OffeGrossMargin;
                        quotedGrossMarginActual.LastRoundClientGrossMargin = lastsj.ClientGrossMargin;
                        quotedGrossMarginActual.LastRoundNreGrossMargin = lastsj.NreGrossMargin;
                    }
                }

                var crm = createCarModelCountDtos.FindFirst(p => p.Product.Equals(solution.ModuleName));
                quotedGrossMarginActual.carNum = crm.SingleCarProductsQuantity;

                QuotedGrossMarginActualList.Add(quotedGrossMarginActual);
            }


            QuotedGrossMarginActual quotedGrossMarginActualqt = new QuotedGrossMarginActual()
            {
                carModel = key,
                AuditFlowId = auditFlowId,
                carNum = cnum,
                product = "齐套",
                InteriorPrice = qtmbnbdj,
                InteriorGrossMargin = (qtnbsr == 0) ? 0 : (qtnbml / qtnbsr) * 100,
                InteriorNreGrossMargin = (qtnbftss == 0) ? 0 : (qtnbftml / qtnbftss) * 100,
                InteriorClientGrossMargin = (qtnbkgsr == 0) ? 0 : (qtnbkgml / qtnbkgsr) * 100,
                ClientPrice = qtmbkhdj,
                ClientGrossMargin = (qtkhsr == 0) ? 0 : (qtkhml / qtkhsr) * 100,
                ClientClientGrossMargin = (qtkhkgsr == 0) ? 0 : (qtkhkgml / qtkhkgsr) * 100,
                ClientNreGrossMargin = (qtkhftss == 0) ? 0 : (qtkhftml / qtkhftss) * 100
            };
            if (last > 0)
            {
                var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId &&
                        p.ProductName.Equals("齐套") && p.carModel.Equals(key) && p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastsj is not null)
                {
                    quotedGrossMarginActualqt.LastRoundPrice = lastsj.OfferUnitPrice;
                    quotedGrossMarginActualqt.LastRoundGrossMargin = lastsj.OffeGrossMargin;
                    quotedGrossMarginActualqt.LastRoundClientGrossMargin = lastsj.ClientGrossMargin;
                    quotedGrossMarginActualqt.LastRoundNreGrossMargin = lastsj.NreGrossMargin;
                }
            }


            QuotedGrossMarginActualList.Add(quotedGrossMarginActualqt);
            quotedGrossMarginActualModel.QuotedGrossMarginActualList = QuotedGrossMarginActualList;
            quotedGrossMarginActualModels.Add(quotedGrossMarginActualModel);
        }


        QuotedGrossMarginActualModel quotedGrossMarginActualModelsj = new QuotedGrossMarginActualModel();
        quotedGrossMarginActualModelsj.project = "报价毛利率测算-实际数量合计";
        List<QuotedGrossMarginActual> QuotedGrossMarginActualListsj = new();


        decimal sjnbsl = 0;
        decimal sjkhsl = 0;
        decimal sjnbxssr = 0;
        decimal sjkhxssr = 0;
        decimal sjnbyj = 0;
        decimal sjkhyj = 0;
        decimal sjnbcb = 0;
        decimal sjkhcb = 0;
        decimal sjnbml = 0;
        decimal sjkhml = 0;
        foreach (var solution in Solutions)
        {
            var mcyl = modelcoutnlist.FindFirst(p => p.Product.Equals(solution.ModuleName)).ModelCountYearList;
            mcyl = mcyl.OrderBy(p => p.Year).ThenBy(P => P.UpDown).ToList();
            var cym = mcyl.First();

            var nmj = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,

                SolutionId = solution.Id,
                SoltionGradPrices = (from gradientqgs in gradientQuotedGrossMarginModels
                                     select new SoltionGradPrice()
                                     {
                                         Gradientid = gradientqgs.GradientId,
                                         SolutionId = gradientqgs.SolutionId,
                                         UnitPrice = gradientqgs.InteriorPrice
                                     }).ToList()
            });
            sjnbsl += nmj.sl;
            sjnbxssr += nmj.xssr;
            sjnbcb += nmj.xscb;
            sjnbyj += nmj.yj;
            sjnbml += nmj.xsml;
            var khj = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,

                SolutionId = solution.Id,
                SoltionGradPrices = (from gradientqgs in gradientQuotedGrossMarginModels
                                     select new SoltionGradPrice()
                                     {
                                         Gradientid = gradientqgs.GradientId,
                                         SolutionId = gradientqgs.SolutionId,
                                         UnitPrice = gradientqgs.ClientPrice
                                     }).ToList()
            });
            sjkhsl += khj.sl;
            sjkhxssr += khj.xssr;
            sjkhyj += khj.yj;
            sjkhcb += khj.xscb;
            sjkhml += khj.xsml;
            QuotedGrossMarginActual quotedGrossMarginActualS = new()
            {
                product = solution.Product,
                SolutionId = solution.Id,
                AuditFlowId = auditFlowId,
                InteriorPrice = nmj.unitPrice,
                InteriorGrossMargin = nmj.GrossMargin,
                InteriorClientGrossMargin = nmj.ClientGrossMargin,
                InteriorNreGrossMargin = nmj.NreGrossMargin,
                ClientPrice = khj.unitPrice,
                ClientGrossMargin = khj.GrossMargin,
                ClientClientGrossMargin = khj.ClientGrossMargin,
                ClientNreGrossMargin = khj.NreGrossMargin
            };
            if (last > 0)
            {
                var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                        p.AuditFlowId == auditFlowId &&
                        p.SolutionId.Equals(solution.Id) && string.IsNullOrEmpty(p.carModel) && p.ntype == ntype)
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastsj is not null)
                {
                    quotedGrossMarginActualS.LastRoundPrice = lastsj.OfferUnitPrice;
                    quotedGrossMarginActualS.LastRoundGrossMargin = lastsj.OffeGrossMargin;
                    quotedGrossMarginActualS.LastRoundClientGrossMargin = lastsj.ClientGrossMargin;
                    quotedGrossMarginActualS.LastRoundNreGrossMargin = lastsj.NreGrossMargin;
                }
            }

            QuotedGrossMarginActualListsj.Add(quotedGrossMarginActualS);
        }


        quotedGrossMarginActualModelsj.QuotedGrossMarginActualList = QuotedGrossMarginActualListsj;
        quotedGrossMarginActualModels.Add(quotedGrossMarginActualModelsj);


        analyseBoardSecondDto.QuotedGrossMargins = quotedGrossMarginActualModels;


        //项目看板-实际数量
        BoardModel sj = new()
        {
            title = "实际数量"
        };

        List<ProjectBoardSecondModel> projectBoardSecondModelssj = new();
        ProjectBoardSecondModel sjsl = new();
        sjsl.ProjectName = "数量";
        sjsl.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("数量") && p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjsl.OldOffer = lastbord.Offer;
            }
        }

        sjsl.InteriorTarget = sjnbsl;
        sjsl.ClientTarget = sjkhsl;

        projectBoardSecondModelssj.Add(sjsl);

        ProjectBoardSecondModel sjxscb = new();
        sjxscb.ProjectName = "销售成本";
        sjxscb.AuditFlowId = auditFlowId;

        sjxscb.InteriorTarget = sjnbcb;
        sjxscb.ClientTarget = sjkhcb;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("销售成本") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjxscb.OldOffer = lastbord.Offer;
            }
        }

        projectBoardSecondModelssj.Add(sjxscb);

        ProjectBoardSecondModel sjdwpjcb = new();
        sjdwpjcb.ProjectName = "单位平均成本";
        sjdwpjcb.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("单位平均成本") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjdwpjcb.OldOffer = lastbord.Offer;
            }
        }

        sjdwpjcb.InteriorTarget = Math.Round(sjnbcb / sjnbsl, 2);
        sjdwpjcb.ClientTarget = Math.Round(sjkhcb / sjkhsl, 2);

        projectBoardSecondModelssj.Add(sjdwpjcb);

        ProjectBoardSecondModel sjxssr = new();
        sjxssr.ProjectName = "销售收入";
        sjxssr.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("销售收入") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjxssr.OldOffer = lastbord.Offer;
            }
        }

        sjxssr.InteriorTarget = Math.Round(sjnbxssr, 2);
        sjxssr.ClientTarget = Math.Round(sjkhxssr, 2);

        projectBoardSecondModelssj.Add(sjxssr);
        ProjectBoardSecondModel sjyj = new();
        sjyj.ProjectName = "佣金";
        sjyj.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("佣金") && p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjyj.OldOffer = lastbord.Offer;
            }
        }

        sjyj.InteriorTarget = sjnbyj;
        sjyj.ClientTarget = sjkhyj;

        projectBoardSecondModelssj.Add(sjyj);
        ProjectBoardSecondModel sjpjdj = new();
        sjpjdj.ProjectName = "平均单价";
        sjpjdj.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("平均单价") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjpjdj.OldOffer = lastbord.Offer;
            }
        }

        sjpjdj.InteriorTarget = Math.Round(sjnbxssr / sjnbsl, 2);
        sjpjdj.ClientTarget = Math.Round(sjkhxssr / sjkhsl, 2);

        projectBoardSecondModelssj.Add(sjpjdj);

        ProjectBoardSecondModel sjxsml = new();
        sjxsml.ProjectName = "销售毛利";
        sjxsml.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("销售毛利") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjxsml.OldOffer = lastbord.Offer;
            }
        }

        sjxsml.InteriorTarget = Math.Round(sjnbml, 2);
        sjxsml.ClientTarget = Math.Round(sjkhml, 2);

        projectBoardSecondModelssj.Add(sjxsml);
        ProjectBoardSecondModel sjmll = new();
        sjmll.ProjectName = "毛利率";
        sjmll.AuditFlowId = auditFlowId;
        if (last > 0)
        {
            var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(p =>
                    p.AuditFlowId == auditFlowId && p.GradientId == 0 && p.ProjectName.Equals("毛利率") &&
                    p.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(p =>
                                p.AuditFlowId == auditFlowId && p.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
            var lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
            if (lastbord is not null)
            {
                sjmll.OldOffer = lastbord.Offer;
            }
        }

        sjmll.InteriorTarget = Math.Round((sjnbml / sjnbxssr) * 100, 2);
        sjmll.ClientTarget = (sjkhxssr == 0) ? 0 : Math.Round((sjkhml / sjkhxssr) * 100, 2);

        projectBoardSecondModelssj.Add(sjmll);
        sj.ProjectBoardModels = projectBoardSecondModelssj;
        boardModels.Add(sj);
        analyseBoardSecondDto.ProjectBoard = boardModels;

        return analyseBoardSecondDto;
    }

    /// <summary>
    /// 根据数量确定梯度
    /// </summary>
    /// <returns></returns>
    public long GetGradient(decimal Quantity, List<Gradient> gradients)
    {
        var maxgrad = gradients.OrderByDescending(p => p.GradientValue).FirstOrDefault();
        if (Quantity >= maxgrad.GradientValue)
        {
            return maxgrad.Id;
        }

        long gradientid = 0;
        foreach (var gradient in gradients)
        {
            if (Quantity <= gradient.GradientValue)
            {
                gradientid = gradient.Id; //获取对应梯度id
                break;
            }
        }

        return gradientid;
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
    /// 阶梯数量下查询毛利率
    /// </summary>
    /// <returns></returns>
    public async Task<GrossMarginSecondDto> PostGrossMarginForGradient(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        var gradientId = yearProductBoardProcessSecondDto.GradientId;
        var AuditFlowId = yearProductBoardProcessSecondDto.AuditFlowId;
        var unprice = yearProductBoardProcessSecondDto.UnitPrice;
        var solutionid = yearProductBoardProcessSecondDto.SolutionId;

        //获取梯度
        var gradient =
            await _gradientRepository.FirstOrDefaultAsync(p => p.AuditFlowId == AuditFlowId && p.Id == gradientId);
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(AuditFlowId);
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;

        List<YearValue> numk = new List<YearValue>(); //数量K
        List<YearValue> Prices = new List<YearValue>(); //单价
        List<YearValue> SellingCost = new List<YearValue>(); //销售成本
        List<YearValue> AverageCost = new List<YearValue>(); //单位平均成本
        List<YearValue> SalesRevenue = new List<YearValue>(); //销售收入
        List<YearValue> commission = new List<YearValue>(); //佣金
        List<YearValue> SalesMargin = new List<YearValue>(); //销售毛利

        List<YearValue> kgSellingCost = new List<YearValue>(); //销售成本
        List<YearValue> kgPrices = new List<YearValue>(); //客供单价
        List<YearValue> kgSalesRevenue = new List<YearValue>(); //客供销售收入
        List<YearValue> kgcommission = new List<YearValue>(); //客供佣金
        List<YearValue> kgSalesMargin = new List<YearValue>(); //客供销售毛利

        List<YearValue> ftSellingCost = new List<YearValue>(); //销售成本
        List<YearValue> ftPrices = new List<YearValue>(); //分摊单价
        List<YearValue> ftSalesRevenue = new List<YearValue>(); //分销售收入
        List<YearValue> ftcommission = new List<YearValue>(); //分佣金
        List<YearValue> ftSalesMargin = new List<YearValue>(); //分销售毛利


        for (int i = 0; i < createRequirementDtos.Count; i++)
        {
            var crm = createRequirementDtos[i];
            var ud = crm.UpDown;
            //数量K
            YearValue num = new();
            num.value = gradient.GradientValue;
            //单价
            YearValue price = new();
            if (i > 0)
            {
                price.value = Prices[i - 1].value * (1 - crm.AnnualDeclineRate / 100);
                Prices.Add(price);
            }
            else
            {
                price.value = unprice * (1 - crm.AnnualDeclineRate / 100);
                Prices.Add(price);
            }

            numk.Add(num);
            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = AuditFlowId,
                GradientId = gradientId,
                InputCount = 0,
                SolutionId = solutionid,
                Year = crm.Year,
                UpDown = crm.UpDown
            });
            //单位平均成本
            var totalcost = ex.TotalCost; //核价看板成本
            YearValue Average = new();
            Average.value = totalcost;
            AverageCost.Add(Average);


            //销售成本

            YearValue sell = new();
            sell.value = totalcost * num.value;
            SellingCost.Add(sell);

            //销售收入（千元）
            YearValue rev = new();
            rev.value = price.value * gradient.GradientValue *
                        (1 - crm.AnnualRebateRequirements / 100) *
                        (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            SalesRevenue.Add(rev);
            //佣金（千元）
            YearValue com = new();
            com.value = price.value * gradient.GradientValue *
                        (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            commission.Add(com);
            //销售毛利
            YearValue mar = new();
            mar.value = rev.value - sell.value - com.value; //销售收入-销售成本-佣金
            SalesMargin.Add(mar);


            var kg = ex.Material.Where(p => p.IsCustomerSupply).Sum(p => p.TotalMoneyCyn);
            //客供单价
            YearValue kgprice = new();
            decimal kgup = price.value + kg; //增加客供成本
            kgprice.value = kgup;
            kgPrices.Add(kgprice);

            //客供销售成本

            YearValue kgsell = new();
            kgsell.value = (totalcost + kg) * num.value;
            kgSellingCost.Add(kgsell);

            //客供销售收入（千元）
            YearValue kgrev = new();
            kgrev.value = (kgup) * gradient.GradientValue *
                          (1 - crm.AnnualRebateRequirements / 100) *
                          (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            kgSalesRevenue.Add(kgrev);
            //客供佣金（千元）
            YearValue kgcom = new();
            kgcom.value = kgup * gradient.GradientValue *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            kgcommission.Add(kgcom);
            //客供销售毛利
            YearValue kgmar = new();
            kgmar.value = kgrev.value - kgsell.value - kgcom.value; //销售收入-销售成本-佣金
            kgSalesMargin.Add(kgmar);


            var ft = ex.OtherCostItem2.Where(p => p.ItemName.Equals("单颗成本")).Sum(p => p.Total.Value);
            //分摊单价
            YearValue ftprice = new();
            decimal ftup = price.value - ft; //分摊成本
            ftprice.value = ftup;
            ftPrices.Add(ftprice);
            //分摊销售成本
            YearValue ftsell = new();
            ftsell.value = (totalcost - ft) * num.value;
            ftSellingCost.Add(ftsell);

            //分摊销售收入（千元）
            YearValue ftrev = new();
            ftrev.value = (ftup) * gradient.GradientValue *
                          (1 - crm.AnnualRebateRequirements / 100) *
                          (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            ftSalesRevenue.Add(ftrev);
            //分摊佣金（千元）
            YearValue ftcom = new();
            ftcom.value = ftup * gradient.GradientValue *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            ftcommission.Add(ftcom);
            //分摊销售毛利
            YearValue ftmar = new();
            ftmar.value = ftrev.value - ftsell.value - ftcom.value; //销售收入-销售成本-佣金
            ftSalesMargin.Add(ftmar);
        }

        var total = numk.Sum(p => p.value); //总数量

        var xszcb = SellingCost.Sum(p => p.value); //销售总成本


        var totalsale = SalesRevenue.Sum(p => p.value); //销售收入总和

        var xsml = SalesMargin.Sum(p => p.value); //销售毛利总和

        var yj = commission.Sum(p => p.value); //佣金
        var kgxszcb = SellingCost.Sum(p => p.value); //客供销售总成本

        var kgtotalsale = kgSalesRevenue.Sum(p => p.value); //客供销售收入总和

        var kgxsml = kgSalesMargin.Sum(p => p.value); //客供销售毛利总和

        var ftxszcb = SellingCost.Sum(p => p.value); //分摊销售总成本

        var fttotalsale = ftSalesRevenue.Sum(p => p.value); //分摊销售收入总和

        var ftxsml = ftSalesMargin.Sum(p => p.value); //分摊销售毛利总和

        GrossMarginSecondDto grossMarginSecondDto = new();
        grossMarginSecondDto.sl = total;
        grossMarginSecondDto.xscb = Math.Round(xszcb, 2);
        grossMarginSecondDto.xssr = Math.Round(totalsale, 2);
        grossMarginSecondDto.xsml = Math.Round(xsml, 2);
        grossMarginSecondDto.yj = Math.Round(yj, 2);
        if (unprice == 0)
        {
            grossMarginSecondDto.GrossMargin = 0;
            grossMarginSecondDto.ClientGrossMargin = 0;
            grossMarginSecondDto.NreGrossMargin = 0;
        }
        else
        {
            grossMarginSecondDto.GrossMargin = Math.Round((xsml / totalsale) * 100, 2);
            grossMarginSecondDto.ClientGrossMargin = Math.Round((kgxsml / kgtotalsale) * 100, 2);
            grossMarginSecondDto.NreGrossMargin = Math.Round((ftxsml / fttotalsale) * 100, 2);
        }

        grossMarginSecondDto.GradientId = gradientId;
        return grossMarginSecondDto;
    }

    /// <summary>
    /// 实际数量下查询毛利率  用于齐套
    /// </summary>
    /// <returns></returns>
    public async Task<GrossMarginSecondDto> PostGrossMarginForactualQt(
        YearProductBoardProcessQtSecondDto yearProductBoardProcessSecondDto)
    {
        var AuditFlowId = yearProductBoardProcessSecondDto.AuditFlowId;

        var solutionidscarnums = yearProductBoardProcessSecondDto.SolutionIdsAndcarNums;
        var carModel = yearProductBoardProcessSecondDto.CarModel; //车型
        var sgp = yearProductBoardProcessSecondDto.SoltionGradPrices;

        decimal unprice = 0;
        decimal qtsl = 0;
        decimal qtyj = 0;
        decimal qtcb = 0;
        decimal qtsr = 0;
        decimal qtml = 0;
        decimal qtkgsr = 0;
        decimal qtkgml = 0;
        decimal qtftss = 0;
        decimal qtftml = 0;

        foreach (var solutionidscarnum in solutionidscarnums)
        {
            if (solutionidscarnum.SolutionId == 0)
            {
                continue;
            }

            GrossMarginSecondDto grossmarin = new();
            if (!string.IsNullOrEmpty(carModel))
            {
                grossmarin = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = AuditFlowId,
                    SolutionId = solutionidscarnum.SolutionId,
                    CarModel = carModel,
                    SoltionGradPrices = sgp
                });
            }
            else
            {
                grossmarin = await PostGrossMarginForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = AuditFlowId,
                    SolutionId = solutionidscarnum.SolutionId,
                    SoltionGradPrices = sgp
                });
            }


            unprice += solutionidscarnum.carNum * grossmarin.unitPrice;
            qtsl += grossmarin.sl;
            qtml += grossmarin.xsml;
            qtcb += grossmarin.xscb;
            qtyj += grossmarin.yj;
            qtsr += grossmarin.xssr;
            qtkgsr += grossmarin.kgxssr;
            qtkgml += grossmarin.kgxsml;
            qtftss += grossmarin.ftxssr;
            qtftml += grossmarin.ftxsml;
        }

        GrossMarginSecondDto grossMarginSecondDto = new GrossMarginSecondDto()
        {
            unitPrice = unprice,
            sl = qtsl,
            yj = qtyj,
            xscb = qtcb,
            xssr = qtsr,
            xsml = qtml,
            GrossMargin = qtsr == 0 ? 0 : (qtml / qtsr) * 100,
            ClientGrossMargin = qtkgsr == 0 ? 0 : (qtkgml / qtkgsr) * 100,
            NreGrossMargin = qtftss == 0 ? 0 : (qtftml / qtftss) * 100
        };
        return grossMarginSecondDto;
    }

    /// <summary>
    /// 实际数量下查询毛利率
    /// </summary>
    /// <returns></returns>
    public async Task<GrossMarginSecondDto> PostGrossMarginForactual(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        var AuditFlowId = yearProductBoardProcessSecondDto.AuditFlowId;
        var solutionid = yearProductBoardProcessSecondDto.SolutionId;
        var carModel = yearProductBoardProcessSecondDto.CarModel; //车型
        var SoltionGradPrices = yearProductBoardProcessSecondDto.SoltionGradPrices;
        Solution solution = _resourceSchemeTable.FirstOrDefault(p => p.Id == solutionid);
        decimal unprice = 0;
        //获取梯度
        var gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == AuditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(AuditFlowId);
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        YearDimensionalityComparisonSecondDto yearDimensionalityComparisonSecondDto = new();

        var modelcount = priceEvaluationStartInputResult.ModelCount;
        var modelCountYearList = modelcount.FindFirst(p => p.Product.Equals(solution.ModuleName)).ModelCountYearList;
        List<YearValue> numk = new List<YearValue>(); //数量K
        List<YearValue> Prices = new List<YearValue>(); //单价
        List<YearValue> SellingCost = new List<YearValue>(); //销售成本
        List<YearValue> AverageCost = new List<YearValue>(); //单位平均成本
        List<YearValue> SalesRevenue = new List<YearValue>(); //销售收入
        List<YearValue> commission = new List<YearValue>(); //佣金
        List<YearValue> SalesMargin = new List<YearValue>(); //销售毛利

        List<YearValue> kgSellingCost = new List<YearValue>(); //销售成本
        List<YearValue> kgPrices = new List<YearValue>(); //客供单价
        List<YearValue> kgSalesRevenue = new List<YearValue>(); //客供销售收入
        List<YearValue> kgcommission = new List<YearValue>(); //客供佣金
        List<YearValue> kgSalesMargin = new List<YearValue>(); //客供销售毛利

        List<YearValue> ftSellingCost = new List<YearValue>(); //销售成本
        List<YearValue> ftPrices = new List<YearValue>(); //分摊单价
        List<YearValue> ftSalesRevenue = new List<YearValue>(); //分销售收入
        List<YearValue> ftcommission = new List<YearValue>(); //分佣金
        List<YearValue> ftSalesMargin = new List<YearValue>(); //分销售毛利
        decimal nj = 1;

        for (int i = 0; i < createRequirementDtos.Count; i++)
        {
            var crm = createRequirementDtos[i];

            var ud = crm.UpDown;

            var modelcountyear =
                modelCountYearList.FindFirst(p => p.Year == crm.Year && p.UpDown.Equals(crm.UpDown));
            decimal nnum = 0;
            if (!string.IsNullOrEmpty(carModel))
            {
                var cmc = priceEvaluationStartInputResult.CarModelCount.FindFirst(p =>
                    p.Product.Equals(solution.ModuleName) && p.CarModel.Equals(carModel));
                var carModelcount =
                    cmc.ModelCountYearList.Find(p =>
                        p.Year == crm.Year && p.UpDown.Equals(crm.UpDown)); //车型、方案产品名称一致的carmodel
                nnum = carModelcount.Quantity;
            }
            else
            {
                nnum = modelcountyear.Quantity;
            }

            //数量K
            YearValue num = new();
            num.value = nnum;
            numk.Add(num);

            var qu = modelcountyear.Quantity; //modelcount获取对应梯度,实际数量落在哪个阶梯

            long grad = GetGradient(qu, gradients);

            var sgprice =
                SoltionGradPrices.FindFirst(p => p.Gradientid == grad && p.SolutionId == solutionid); //相应梯度的单价
            if (i == 0)
            {
                unprice = sgprice.UnitPrice;
            }

            //单价
            YearValue price = new();
            nj = nj * (1 - crm.AnnualDeclineRate / 100);
            price.value = sgprice.UnitPrice * nj; //梯度对应的单价*当年的年降率
            Prices.Add(price);

            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = AuditFlowId,
                GradientId = grad,
                InputCount = 0,
                SolutionId = solutionid,
                Year = crm.Year,
                UpDown = crm.UpDown
            });
            //单位平均成本
            var totalcost = ex.TotalCost; //核价看板成本
            YearValue Average = new();
            Average.value = totalcost;
            AverageCost.Add(Average);


            //销售成本

            YearValue sell = new();
            sell.value = totalcost * num.value;
            SellingCost.Add(sell);

            //销售收入（千元）
            YearValue rev = new();
            rev.value = price.value * num.value *
                        (1 - crm.AnnualRebateRequirements / 100) *
                        (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            SalesRevenue.Add(rev);
            //佣金（千元）
            YearValue com = new();
            com.value = price.value * num.value *
                        (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            commission.Add(com);
            //销售毛利
            YearValue mar = new();
            mar.value = rev.value - sell.value - com.value; //销售收入-销售成本-佣金
            SalesMargin.Add(mar);


            var kg = ex.Material.Where(p => p.IsCustomerSupply).Sum(p => p.TotalMoneyCyn);
            //客供单价
            YearValue kgprice = new();
            decimal kgup = price.value + kg; //增加客供成本
            kgprice.value = kgup;
            kgPrices.Add(kgprice);

            //客供销售成本

            YearValue kgsell = new();
            kgsell.value = (totalcost + kg) * num.value;
            kgSellingCost.Add(kgsell);

            //客供销售收入（千元）
            YearValue kgrev = new();
            kgrev.value = (kgup) * num.value *
                          (1 - crm.AnnualRebateRequirements / 100) *
                          (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            kgSalesRevenue.Add(kgrev);
            //客供佣金（千元）
            YearValue kgcom = new();
            kgcom.value = kgup * num.value *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            kgcommission.Add(kgcom);
            //客供销售毛利
            YearValue kgmar = new();
            kgmar.value = kgrev.value - kgsell.value - kgcom.value; //销售收入-销售成本-佣金
            kgSalesMargin.Add(kgmar);


            var ft = ex.OtherCostItem2.Where(p => p.ItemName.Equals("单颗成本")).Sum(p => p.Total.Value);
            //分摊单价
            YearValue ftprice = new();
            decimal ftup = price.value - ft; //分摊成本
            ftprice.value = ftup;
            ftPrices.Add(ftprice);
            //分摊销售成本
            YearValue ftsell = new();
            ftsell.value = (totalcost - ft) * num.value;
            ftSellingCost.Add(ftsell);

            //分摊销售收入（千元）
            YearValue ftrev = new();
            ftrev.value = (ftup) * num.value *
                          (1 - crm.AnnualRebateRequirements / 100) *
                          (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            ftSalesRevenue.Add(ftrev);
            //分摊佣金（千元）
            YearValue ftcom = new();
            ftcom.value = ftup * num.value *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            ftcommission.Add(ftcom);
            //分摊销售毛利
            YearValue ftmar = new();
            ftmar.value = ftrev.value - ftsell.value - ftcom.value; //销售收入-销售成本-佣金
            ftSalesMargin.Add(ftmar);
        }

        var total = numk.Sum(p => p.value); //总数量
        var xszcb = SellingCost.Sum(p => p.value); //销售总成本
        var totalsale = SalesRevenue.Sum(p => p.value); //销售收入总和
        var xsml = SalesMargin.Sum(p => p.value); //销售毛利总和

        var yj = commission.Sum(p => p.value); //佣金
        var kgxszcb = SellingCost.Sum(p => p.value); //客供销售总成本

        var kgtotalsale = kgSalesRevenue.Sum(p => p.value); //客供销售收入总和

        var kgxsml = kgSalesMargin.Sum(p => p.value); //客供销售毛利总和

        var ftxszcb = SellingCost.Sum(p => p.value); //分摊销售总成本

        var fttotalsale = ftSalesRevenue.Sum(p => p.value); //分摊销售收入总和

        var ftxsml = ftSalesMargin.Sum(p => p.value); //分摊销售毛利总和

        GrossMarginSecondDto grossMarginSecondDto = new();
        grossMarginSecondDto.sl = total;
        grossMarginSecondDto.xscb = Math.Round(xszcb, 2);
        grossMarginSecondDto.xssr = Math.Round(totalsale, 2);
        grossMarginSecondDto.xsml = Math.Round(xsml, 2);
        grossMarginSecondDto.yj = Math.Round(yj, 2);
        grossMarginSecondDto.GrossMargin = totalsale == 0 ? 0 : Math.Round((xsml / totalsale) * 100, 2);
        grossMarginSecondDto.ClientGrossMargin = kgtotalsale == 0 ? 0 : Math.Round((kgxsml / kgtotalsale) * 100, 2);
        grossMarginSecondDto.kgxsml = kgxsml;
        grossMarginSecondDto.kgxssr = kgtotalsale;

        grossMarginSecondDto.NreGrossMargin = fttotalsale == 0 ? 0 : Math.Round((ftxsml / fttotalsale) * 100, 2);
        grossMarginSecondDto.ftxsml = ftxsml;
        grossMarginSecondDto.ftxssr = fttotalsale;
        grossMarginSecondDto.unitPrice = unprice;
        return grossMarginSecondDto;
    }

    /// <summary>
    /// 查看年度对比（实际数量）用于齐套
    /// </summary>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparisonForactualQt(
        YearProductBoardProcessQtSecondDto yearProductBoardProcessQtSecondDto)
    {
        var AuditFlowId = yearProductBoardProcessQtSecondDto.AuditFlowId;

        var carModel = yearProductBoardProcessQtSecondDto.CarModel; //车型
        var solutionidscarnums = yearProductBoardProcessQtSecondDto.SolutionIdsAndcarNums;
        var sgp = yearProductBoardProcessQtSecondDto.SoltionGradPrices;


        List<YearValue> numk = new List<YearValue>(); //数量K
        List<YearValue> Prices = new List<YearValue>(); //单价
        List<YearValue> SellingCost = new List<YearValue>(); //销售成本
        List<YearValue> AverageCost = new List<YearValue>(); //单位平均成本
        List<YearValue> SalesRevenue = new List<YearValue>(); //销售收入
        List<YearValue> commission = new List<YearValue>(); //佣金
        List<YearValue> SalesMargin = new List<YearValue>(); //销售毛利
        List<YearValue> GrossMargin = new List<YearValue>(); //毛利率
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(AuditFlowId);
        var crms = priceEvaluationStartInputResult.Requirement;

        for (int i = 0; i < solutionidscarnums.Count; i++)
        {
            var solutionidscarnum = solutionidscarnums[i];
            var carnum = solutionidscarnum.carNum;
            YearDimensionalityComparisonSecondDto nddb = new();
            if (!string.IsNullOrEmpty(carModel))
            {
                nddb = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = AuditFlowId,
                    SolutionId = solutionidscarnum.SolutionId,
                    CarModel = carModel,
                    SoltionGradPrices = sgp
                });
            }
            else
            {
                nddb = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
                {
                    AuditFlowId = AuditFlowId,
                    SolutionId = solutionidscarnum.SolutionId,

                    SoltionGradPrices = sgp
                });
            }

            var numknew = nddb.numk.Where(p => !p.key.Equals("全生命周期")).ToList();
            var Pricsenew = nddb.Prices.Where(p => !p.key.Equals("全生命周期")).ToList();
            var SellingCostnew = nddb.SellingCost.Where(p => !p.key.Equals("全生命周期")).ToList();
            var AverageCostnew = nddb.AverageCost.Where(p => !p.key.Equals("全生命周期")).ToList();
            var SalesRevenuenew = nddb.SalesRevenue.Where(p => !p.key.Equals("全生命周期")).ToList();
            var commissionnew = nddb.commission.Where(p => !p.key.Equals("全生命周期")).ToList();
            var SalesMarginnew = nddb.SalesMargin.Where(p => !p.key.Equals("全生命周期")).ToList();
            if (i == 0)
            {
                numk = numknew;
                SellingCost = SellingCostnew;

                SalesRevenue = SalesRevenuenew;
                commission = commissionnew;
                SalesMargin = SalesMarginnew;

                Pricsenew = (from price in Pricsenew
                             select new YearValue()
                             {
                                 key = price.key,
                                 value = price.value * carnum
                             }).ToList();
                Prices = Pricsenew;

                AverageCostnew = (from price in AverageCostnew
                                  select new YearValue()
                                  {
                                      key = price.key,
                                      value = price.value * carnum
                                  }).ToList();
                AverageCost = AverageCostnew;
            }
            else
            {
                numk = hebing(numk, numknew);
                SellingCost = hebing(SellingCost, SellingCostnew);

                SalesRevenue = hebing(SalesRevenue, SalesRevenuenew);
                commission = hebing(commission, commissionnew);
                SalesMargin = hebing(SalesMargin, SalesMarginnew);
                Pricsenew = (from price in Pricsenew
                             select new YearValue()
                             {
                                 key = price.key,
                                 value = price.value * carnum
                             }).ToList();
                Prices = hebing(Prices, Pricsenew);
                AverageCostnew = (from price in AverageCostnew
                                  select new YearValue()
                                  {
                                      key = price.key,
                                      value = price.value * carnum
                                  }).ToList();
                AverageCost = hebing(AverageCost, AverageCostnew);
            }
        }

        Dictionary<string, YearValue> smdict = SalesMargin.ToDictionary(p => p.key, p => p);

        foreach (var SalesRevenues in SalesRevenue)
        {
            //毛利率
            YearValue gross = new();
            gross.key = SalesRevenues.key;
            gross.value = SalesRevenues.value == 0
                ? 0
                : (smdict[SalesRevenues.key].value / SalesRevenues.value) * 100; //销售毛利/销售收入
            GrossMargin.Add(gross);
        }


        var total = numk.Sum(p => p.value); //总数量
        numk.Add(new YearValue()
        {
            key = "全生命周期",
            value = total
        });

        var xszcb = SellingCost.Sum(p => p.value); //销售总成本
        SellingCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = xszcb
        });

        AverageCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = xszcb / total //销售成本/数量
        });
        var totalsale = SalesRevenue.Sum(p => p.value); //销售收入总和
        SalesRevenue.Add(new YearValue()
        {
            key = "全生命周期",
            value = totalsale
        });
        Prices.Add(new YearValue() //单价总和=销售收入总和/数量总和
        {
            key = "全生命周期",
            value = totalsale / total
        });
        var yj = commission.Sum(p => p.value); //佣金总和
        commission.Add(new YearValue()
        {
            key = "全生命周期",
            value = yj
        });
        var xsml = SalesMargin.Sum(p => p.value); //销售毛利总和
        SalesMargin.Add(new YearValue()
        {
            key = "全生命周期",
            value = xsml
        });
        GrossMargin.Add(new YearValue() //销售毛利/销售收入
        {
            key = "全生命周期",
            value = (xsml / totalsale) * 100
        });

        YearDimensionalityComparisonSecondDto yearDimensionalityComparisonSecondDto =
            new YearDimensionalityComparisonSecondDto();

        yearDimensionalityComparisonSecondDto.numk = numk;
        yearDimensionalityComparisonSecondDto.Prices = Prices;
        yearDimensionalityComparisonSecondDto.SellingCost = SellingCost;
        yearDimensionalityComparisonSecondDto.AverageCost = AverageCost;
        yearDimensionalityComparisonSecondDto.SalesRevenue = SalesRevenue;
        yearDimensionalityComparisonSecondDto.commission = commission;
        yearDimensionalityComparisonSecondDto.SalesMargin = SalesMargin;
        yearDimensionalityComparisonSecondDto.GrossMargin = GrossMargin;
        return yearDimensionalityComparisonSecondDto;
    }

    /// <summary>
    /// List<YearValue>合并
    /// </summary>
    /// <returns></returns>
    public List<YearValue> hebing(List<YearValue> olds, List<YearValue> news)
    {
        List<YearValue> ones = new();
        Dictionary<String, YearValue> dict = news.ToDictionary(p => p.key, p => p);
        foreach (var old in olds)
        {
            var key = old.key;
            var value = old.value;
            value += dict[key].value;
            ones.Add(new YearValue()
            {
                key = key,
                value = value
            });
        }

        return ones;
    }

    /// <summary>
    /// 查看年度对比（实际数量）
    /// </summary>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparisonForactual(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        var AuditFlowId = yearProductBoardProcessSecondDto.AuditFlowId;

        var solutionid = yearProductBoardProcessSecondDto.SolutionId;
        var carModel = yearProductBoardProcessSecondDto.CarModel; //车型
        var SoltionGradPrices = yearProductBoardProcessSecondDto.SoltionGradPrices;
        Solution solution = _resourceSchemeTable.FirstOrDefault(p => p.Id == solutionid);

        //获取梯度
        var gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == AuditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(AuditFlowId);
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        YearDimensionalityComparisonSecondDto yearDimensionalityComparisonSecondDto = new();

        var modelcount = priceEvaluationStartInputResult.ModelCount;
        var modelCountYearList = modelcount.FindFirst(p => p.Product.Equals(solution.ModuleName)).ModelCountYearList;

        List<YearValue> numk = new List<YearValue>(); //数量K
        List<YearValue> Prices = new List<YearValue>(); //单价
        List<YearValue> SellingCost = new List<YearValue>(); //销售成本
        List<YearValue> AverageCost = new List<YearValue>(); //单位平均成本
        List<YearValue> SalesRevenue = new List<YearValue>(); //销售收入
        List<YearValue> commission = new List<YearValue>(); //佣金
        List<YearValue> SalesMargin = new List<YearValue>(); //销售毛利
        List<YearValue> GrossMargin = new List<YearValue>(); //毛利率
        decimal nj = 1;
        foreach (var crm in createRequirementDtos)
        {
            var modelcountyear =
                modelCountYearList.FindFirst(p => p.Year == crm.Year && p.UpDown.Equals(crm.UpDown));
            decimal nnum = 0;
            if (!string.IsNullOrEmpty(carModel))
            {
                var carModelcount =
                    priceEvaluationStartInputResult.CarModelCount.FindFirst(p =>
                        p.Product.Equals(solution.ModuleName) && p.CarModel.Equals(carModel)).ModelCountYearList.Find(
                        p =>
                            p.Year == crm.Year && p.UpDown.Equals(crm.UpDown)); //车型、方案产品名称一致的carmodel
                nnum = carModelcount.Quantity.GetDecimal();
            }
            else
            {
                nnum = modelcountyear.Quantity.GetDecimal();
            }


            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            //数量K
            YearValue num = new();
            num.key = key;
            num.value = nnum; //核价需求界面单个车型该模组的数量
            numk.Add(num);
            var qu = modelcountyear.Quantity; //modelcount获取对应梯度,实际数量落在哪个阶梯
            long grad = GetGradient(qu, gradients);


            var sgprice =
                SoltionGradPrices.FindFirst(p => p.Gradientid == grad && p.SolutionId == solutionid); //相应梯度的单价


            //单价
            YearValue price = new();
            nj = nj * (1 - crm.AnnualDeclineRate / 100);

            price.key = key;
            price.value = sgprice.UnitPrice * nj; //梯度对应的单价*当年的年降率
            Prices.Add(price);


            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = AuditFlowId,
                GradientId = grad,
                InputCount = 0,
                SolutionId = solutionid,
                Year = crm.Year,
                UpDown = crm.UpDown
            });

            var totalcost = ex.TotalCost; //核价看板成本
            //单位平均成本
            YearValue Average = new();
            Average.key = key;
            Average.value = totalcost;
            AverageCost.Add(Average);
            //销售成本
            YearValue sell = new();
            sell.key = key;
            sell.value = totalcost * num.value;
            SellingCost.Add(sell);

            //销售收入（千元）
            YearValue rev = new();
            rev.key = key;
            rev.value = price.value * nnum *
                        (1 - crm.AnnualRebateRequirements / 100) *
                        (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            SalesRevenue.Add(rev);
            //佣金（千元）
            YearValue com = new();
            com.key = key;
            com.value = price.value * nnum *
                        (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            commission.Add(com);
            //销售毛利
            YearValue mar = new();
            mar.key = key;
            mar.value = rev.value - sell.value - com.value; //销售收入-销售成本-佣金
            SalesMargin.Add(mar);

            //毛利率
            YearValue gross = new();
            gross.key = key;
            gross.value = (rev.value == 0) ? 0 : (mar.value / rev.value) * 100; //销售毛利/销售收入
            GrossMargin.Add(gross);
        }

        var total = numk.Sum(p => p.value); //总数量
        numk.Add(new YearValue()
        {
            key = "全生命周期",
            value = total
        });

        var xszcb = SellingCost.Sum(p => p.value); //销售总成本
        SellingCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = xszcb
        });

        AverageCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = total == 0 ? 0 : xszcb / total //销售成本/数量
        });
        var totalsale = SalesRevenue.Sum(p => p.value); //销售收入总和
        SalesRevenue.Add(new YearValue()
        {
            key = "全生命周期",
            value = totalsale
        });
        Prices.Add(new YearValue() //单价总和=销售收入总和/数量总和
        {
            key = "全生命周期",
            value = total == 0 ? 0 : totalsale / total
        });
        var yj = commission.Sum(p => p.value); //佣金总和
        commission.Add(new YearValue()
        {
            key = "全生命周期",
            value = yj
        });
        var xsml = SalesMargin.Sum(p => p.value); //销售毛利总和
        SalesMargin.Add(new YearValue()
        {
            key = "全生命周期",
            value = xsml
        });
        GrossMargin.Add(new YearValue() //销售毛利/销售收入
        {
            key = "全生命周期",
            value = totalsale == 0 ? 0 : (xsml / totalsale) * 100
        });
        yearDimensionalityComparisonSecondDto.numk = numk;
        yearDimensionalityComparisonSecondDto.Prices = Prices;
        yearDimensionalityComparisonSecondDto.SellingCost = SellingCost;
        yearDimensionalityComparisonSecondDto.AverageCost = AverageCost;
        yearDimensionalityComparisonSecondDto.SalesRevenue = SalesRevenue;
        yearDimensionalityComparisonSecondDto.commission = commission;
        yearDimensionalityComparisonSecondDto.SalesMargin = SalesMargin;
        yearDimensionalityComparisonSecondDto.GrossMargin = GrossMargin;


        return yearDimensionalityComparisonSecondDto;
    }

    /// <summary>
    /// 查看年度对比（阶梯数量）
    /// </summary>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> YearDimensionalityComparisonForGradient(
        YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {
        var gradientId = yearProductBoardProcessSecondDto.GradientId;
        var AuditFlowId = yearProductBoardProcessSecondDto.AuditFlowId;
        var unprice = yearProductBoardProcessSecondDto.UnitPrice;
        var solutionid = yearProductBoardProcessSecondDto.SolutionId;

        //获取梯度
        var gradient =
            await _gradientRepository.FirstOrDefaultAsync(p => p.AuditFlowId == AuditFlowId && p.Id == gradientId);
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(AuditFlowId);
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        YearDimensionalityComparisonSecondDto yearDimensionalityComparisonSecondDto = new();

        List<YearValue> numk = new List<YearValue>(); //数量K
        List<YearValue> Prices = new List<YearValue>(); //单价
        List<YearValue> SellingCost = new List<YearValue>(); //销售成本
        List<YearValue> AverageCost = new List<YearValue>(); //单位平均成本
        List<YearValue> SalesRevenue = new List<YearValue>(); //销售收入
        List<YearValue> commission = new List<YearValue>(); //佣金
        List<YearValue> SalesMargin = new List<YearValue>(); //销售毛利
        List<YearValue> GrossMargin = new List<YearValue>(); //毛利率


        for (int i = 0; i < createRequirementDtos.Count; i++)
        {
            var crm = createRequirementDtos[i];
            var ud = crm.UpDown;
            string key = crm.Year.ToString();
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }


            //数量K
            YearValue num = new();
            num.key = key;
            num.value = gradient.GradientValue;
            numk.Add(num);
            //单价
            YearValue price = new();
            if (i > 0)
            {
                price.key = key;
                price.value = Prices[i - 1].value * (1 - crm.AnnualDeclineRate / 100);
                Prices.Add(price);
            }
            else
            {
                price.key = key;
                price.value = unprice * (1 - crm.AnnualDeclineRate / 100);
                Prices.Add(price);
            }


            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = AuditFlowId,
                GradientId = gradientId,
                InputCount = 0,
                SolutionId = solutionid,
                Year = crm.Year,
                UpDown = crm.UpDown
            });
            //单位平均成本
            var totalcost = ex.TotalCost; //核价看板成本
            YearValue Average = new();
            Average.key = key;
            Average.value = totalcost;
            AverageCost.Add(Average);
            //销售成本

            YearValue sell = new();
            sell.key = key;
            sell.value = totalcost * num.value;
            SellingCost.Add(sell);

            //销售收入（千元）
            YearValue rev = new();
            rev.key = key;
            rev.value = price.value * gradient.GradientValue *
                        (1 - crm.AnnualRebateRequirements / 100) *
                        (1 - crm.OneTimeDiscountRate / 100); //单价*数量*（1-年度返利要求）*（1-一次性折让率）
            SalesRevenue.Add(rev);
            //佣金（千元）
            YearValue com = new();
            com.key = key;
            com.value = price.value * gradient.GradientValue *
                        (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            commission.Add(com);
            //销售毛利
            YearValue mar = new();
            mar.key = key;
            mar.value = rev.value - sell.value - com.value; //销售收入-销售成本-佣金
            SalesMargin.Add(mar);

            //毛利率
            YearValue gross = new();
            gross.key = key;
            gross.value = (rev.value == 0) ? 0 : (mar.value / rev.value) * 100; //销售毛利/销售收入
            GrossMargin.Add(gross);
        }

        var total = numk.Sum(p => p.value); //总数量
        numk.Add(new YearValue()
        {
            key = "全生命周期",
            value = total
        });

        var xszcb = SellingCost.Sum(p => p.value); //销售总成本
        SellingCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = xszcb
        });

        AverageCost.Add(new YearValue()
        {
            key = "全生命周期",
            value = xszcb / total //销售成本/数量
        });
        var totalsale = SalesRevenue.Sum(p => p.value); //销售收入总和
        SalesRevenue.Add(new YearValue()
        {
            key = "全生命周期",
            value = totalsale
        });
        Prices.Add(new YearValue() //单价总和=销售收入总和/数量总和
        {
            key = "全生命周期",
            value = totalsale / total
        });
        var yj = commission.Sum(p => p.value); //佣金总和
        commission.Add(new YearValue()
        {
            key = "全生命周期",
            value = yj
        });
        var xsml = SalesMargin.Sum(p => p.value); //销售毛利总和
        SalesMargin.Add(new YearValue()
        {
            key = "全生命周期",
            value = xsml
        });
        GrossMargin.Add(new YearValue() //销售毛利/销售收入
        {
            key = "全生命周期",
            value = totalsale == 0 ? 0 : (xsml / totalsale) * 100
        });
        yearDimensionalityComparisonSecondDto.numk = numk;
        yearDimensionalityComparisonSecondDto.Prices = Prices;
        yearDimensionalityComparisonSecondDto.SellingCost = SellingCost;
        yearDimensionalityComparisonSecondDto.AverageCost = AverageCost;
        yearDimensionalityComparisonSecondDto.SalesRevenue = SalesRevenue;
        yearDimensionalityComparisonSecondDto.commission = commission;
        yearDimensionalityComparisonSecondDto.SalesMargin = SalesMargin;
        yearDimensionalityComparisonSecondDto.GrossMargin = GrossMargin;


        return yearDimensionalityComparisonSecondDto;
    }

    public async Task<AnalyseBoardSecondDto> getStatementAnalysisBoardSecond(long auditFlowId, int version, int ntype)
    {
        SolutionQuotation solutionQuotation =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (solutionQuotation is null)
        {
            throw new UserFriendlyException("请选择报价方案");
        }

        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(solutionQuotation.SolutionListJson);

        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();

        List<AnalyseBoardNreDto> nres = await getNreForData(auditFlowId, version, ntype);
        List<OnlySampleDto> sampleDtos = await getSampleForData(auditFlowId, solutionList, version, ntype);
        List<SopAnalysisModel> sops = await getSopForData(auditFlowId, version, ntype);
        List<PooledAnalysisModel> pools = await getPoolForData(auditFlowId, version, ntype);
        List<GradientGrossMarginCalculateModel>
            gradis = await getGradientQuotedGrossMargin(auditFlowId, version, ntype);
        List<QuotedGrossMarginActualModel> quotedGrossMarginProjectModels =
            await getQuotedGrossMarginProject(auditFlowId, version, ntype);

        List<BoardModel> boards = await getboardForData(auditFlowId, version, ntype);

        analyseBoardSecondDto.nres = nres;
        analyseBoardSecondDto.SampleOffer = sampleDtos;
        analyseBoardSecondDto.Sops = sops;
        analyseBoardSecondDto.FullLifeCycle = pools;
        analyseBoardSecondDto.QuotedGrossMargins = quotedGrossMarginProjectModels;
        analyseBoardSecondDto.GradientQuotedGrossMargins = gradis;
        analyseBoardSecondDto.ProjectBoard = boards;
        return analyseBoardSecondDto;
    }

    public async Task delete(long AuditFlowId, int version, int ntype)
    {
        //应陈梦瑶要求增加可改变方案功能，先把老数据删除
        if (ntype == 0)
        {
             await _solutionQutation.DeleteAsync(p => p.version == version && p.AuditFlowId == AuditFlowId);
        }


        await  _nreQuotation.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
        await _deviceQuotation.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
        await _sampleQuotation.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
        await   _resourceUnitPriceOffers.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
      await   _resourcePooledAnalysisOffers.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
      await  _resourceProjectBoardSecondOffers.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
        await _actualUnitPriceOffer.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
        await  _dynamicUnitPriceOffers.DeleteAsync(p =>
            p.version == version && p.AuditFlowId == AuditFlowId && p.ntype == ntype);
    }

    public async Task PostIsOfferSaveSecond(IsOfferSecondDto isOfferDto)
    {
        List<Solution> solutions = isOfferDto.Solutions;
        int version = isOfferDto.version;
        var AuditFlowId = isOfferDto.AuditFlowId;
        int ntype = isOfferDto.ntype;
     
        int time = isOfferDto.ntime;
      
            if (ntype == 0 && time > 3)
            {
                throw new UserFriendlyException("本流程报价提交次数已经到顶");
            }
        


        if (ntype == 0)
        {
            await InsertSolution(solutions, version, time, AuditFlowId, isOfferDto.IsFirst);
        }

        List<AnalyseBoardNreDto> nres = isOfferDto.nres;
        InsertNre(nres, solutions, version, ntype);
        List<OnlySampleDto> onlySampleDtos = isOfferDto.SampleOffer;
        InsertSample(onlySampleDtos, solutions, version, ntype);

        List<SopAnalysisModel> sops = isOfferDto.Sops;
        InsertSop(sops, version, ntype);
        List<PooledAnalysisModel> FullLifeCycle = isOfferDto.FullLifeCycle;
        InsertPool(AuditFlowId, FullLifeCycle, version, ntype);

        List<BoardModel> projectBoardModels = isOfferDto.ProjectBoard;
        Insertboard(AuditFlowId, projectBoardModels, version, ntype);
        List<GradientGrossMarginCalculateModel> jts = isOfferDto.GradientQuotedGrossMargins;
        InsertGradientQuotedGrossMargin(AuditFlowId, jts, version, ntype);
        List<QuotedGrossMarginActualModel> models = isOfferDto.QuotedGrossMargins;
        InsertQuotedGrossMarginProject(AuditFlowId, models, version, ntype);
    }

    /// <summary>
    /// 报价毛利率测算 实际数量  从数据查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotedGrossMarginActualModel>> getQuotedGrossMarginProject(long AuditFlowId, int version,
        int ntype)
    {
        var ntime = _solutionQutation.FirstOrDefault(p => p.AuditFlowId == AuditFlowId && p.version == version).ntime;
        var last = ntime - 1;
        List<QuotedGrossMarginActualModel> list = new();
        List<DynamicUnitPriceOffers> dynamicUnitPriceOffers =await _dynamicUnitPriceOffers.GetAllListAsync(p => p.AuditFlowId == AuditFlowId && p.version == version && p.ntype == ntype);
        var dymap = from dy in dynamicUnitPriceOffers group dy by dy.title;

        foreach (var d in dymap)
        {
            var title = d.Key;
            QuotedGrossMarginActualModel quotedGrossMarginProjectModel = new();
            quotedGrossMarginProjectModel.project = title;
            List<QuotedGrossMarginActual> GrossMargins = new();
            foreach (var ds in d)
            {
                QuotedGrossMarginActual gross = new()
                {
                    version = ds.version,
                    Id = ds.Id,
                    SolutionId = ds.SolutionId,
                    carModel = ds.carModel,
                    product = ds.ProductName,
                    AuditFlowId = ds.AuditFlowId,
                    carNum = ds.carNum,
                    InteriorPrice = ds.InteriorTargetUnitPrice,
                    InteriorGrossMargin = ds.AllInteriorGrossMargin,
                    InteriorClientGrossMargin = ds.AllInteriorClientGrossMargin,
                    InteriorNreGrossMargin = ds.AllInteriorNreGrossMargin,
                    ClientPrice = ds.ClientTargetUnitPrice,
                    ClientGrossMargin = ds.AllClientGrossMargin,
                    ClientClientGrossMargin = ds.AllClientClientGrossMargin,
                    ClientNreGrossMargin = ds.AllClientNreGrossMargin,
                    ThisQuotationPrice = ds.OfferUnitPrice,
                    ThisQuotationGrossMargin = ds.OffeGrossMargin,
                    ThisQuotationClientGrossMargin = ds.ClientGrossMargin,
                    ThisQuotationNreGrossMargin = ds.NreGrossMargin
                };
                if (last > 0)
                {
                    DynamicUnitPriceOffers lastsj = new DynamicUnitPriceOffers();
                    if (!string.IsNullOrEmpty(ds.carModel) && !ds.ProductName.Equals("齐套"))
                    {
                        var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                                p.AuditFlowId == AuditFlowId && p.carModel.Equals(ds.carModel) && p.ntype == ntype &&
                                p.ProductName.Equals(ds.ProductName))
                                        join sol in await _solutionQutation.GetAllListAsync(p =>
                                            p.AuditFlowId == AuditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                        select lastjts).ToList();


                        lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(ds.carModel) && ds.ProductName.Equals("齐套"))
                    {
                        var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                                p.AuditFlowId == AuditFlowId && p.carModel.Equals(ds.carModel) && p.ntype == ntype &&
                                p.ProductName.Equals("齐套"))
                                        join sol in await _solutionQutation.GetAllListAsync(p =>
                                            p.AuditFlowId == AuditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                        select lastjts).ToList();


                        lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    }
                    else if (string.IsNullOrEmpty(ds.carModel))
                    {
                        var lastlist = (from lastjts in await _dynamicUnitPriceOffers.GetAllListAsync(p =>
                                p.AuditFlowId == AuditFlowId && string.IsNullOrEmpty(p.carModel) && p.ntype == ntype &&
                                p.SolutionId == ds.SolutionId)
                                        join sol in await _solutionQutation.GetAllListAsync(p =>
                                            p.AuditFlowId == AuditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                        select lastjts).ToList();


                        lastsj = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    }

                    if (lastsj is not null)
                    {
                        gross.LastRoundPrice = lastsj.OfferUnitPrice;
                        gross.LastRoundGrossMargin = lastsj.OffeGrossMargin;
                        gross.LastRoundClientGrossMargin = lastsj.ClientGrossMargin;
                        gross.LastRoundNreGrossMargin = lastsj.NreGrossMargin;
                    }
                }


                GrossMargins.Add(gross);
            }

            quotedGrossMarginProjectModel.QuotedGrossMarginActualList = GrossMargins;
            list.Add(quotedGrossMarginProjectModel);
        }

        return list;
    }

    /// <summary>
    /// 获取梯度
    /// </summary>
    /// <returns></returns>
    public async Task<List<Gradient>> getGradient(long AuditFlowId)
    {
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == AuditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        return gradients;
    }


    /// <summary>
    /// 报价毛利率测算 实际数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertQuotedGrossMarginProject(long AuditFlowId,
        List<QuotedGrossMarginActualModel> grossMarginProjectModels, int version, int ntype)
    {
        var list = new List<DynamicUnitPriceOffers>();
        foreach (var grossMargin in grossMarginProjectModels)
        {
            var project = grossMargin.project;
            var dys = (from gross in grossMargin.QuotedGrossMarginActualList
                       select new DynamicUnitPriceOffers()
                       {
                           version = version,
                           AuditFlowId = AuditFlowId,
                           ProductName = gross.product,
                           title = project,
                           ntype = ntype,
                           carModel = gross.carModel,
                           carNum = gross.carNum,
                           SolutionId = gross.SolutionId,
                           InteriorTargetUnitPrice = gross.InteriorPrice,
                           AllInteriorGrossMargin = gross.InteriorGrossMargin,
                           AllInteriorClientGrossMargin = gross.InteriorClientGrossMargin,
                           AllInteriorNreGrossMargin = gross.InteriorNreGrossMargin,
                           ClientTargetUnitPrice = gross.ClientPrice,
                           AllClientGrossMargin = gross.ClientGrossMargin,
                           AllClientClientGrossMargin = gross.ClientClientGrossMargin,
                           AllClientNreGrossMargin = gross.ClientNreGrossMargin,
                           OfferUnitPrice = gross.ThisQuotationPrice,
                           OffeGrossMargin = gross.ThisQuotationGrossMargin,
                           ClientGrossMargin = gross.ThisQuotationClientGrossMargin,
                           NreGrossMargin = gross.ThisQuotationNreGrossMargin
                       }).ToList();
            list.AddRange(dys);
        }

        foreach (var dynamicUnitPriceOffers in list)
        {
            _dynamicUnitPriceOffers.InsertAsync(dynamicUnitPriceOffers);
        }
    }

    /// <summary>
    /// 报价毛利率测算 阶梯数量 从数据库查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<GradientGrossMarginCalculateModel>> getGradientQuotedGrossMargin(long AuditFlowId,
        int version, int ntype)
    {
        var ntime = _solutionQutation.FirstOrDefault(p => p.AuditFlowId == AuditFlowId && p.version == version).ntime;
        var last = ntime - 1;
        var jtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == AuditFlowId && p.version == version && p.ntype == ntype);
        jtsls = jtsls.OrderBy(p => p.gradient).ToList();
        var gradientQuotedGross = (from jtsl in jtsls
                                   select new GradientGrossMarginCalculateModel()
                                   {
                                       AuditFlowId = jtsl.AuditFlowId,
                                       version = jtsl.version,
                                       GradientId = jtsl.GradientId,
                                       gradient = jtsl.gradient,
                                       SolutionId = jtsl.SolutionId,
                                       product = jtsl.product,
                                       InteriorPrice = jtsl.InteriorPrice,
                                       InteriorGrossMargin = jtsl.InteriorGrossMargin,
                                       InteriorClientGrossMargin = jtsl.InteriorClientGrossMargin,
                                       InteriorNreGrossMargin = jtsl.InteriorNreGrossMargin,
                                       ClientPrice = jtsl.ClientPrice,
                                       ClientGrossMargin = jtsl.ClientGrossMargin,
                                       ClientClientGrossMargin = jtsl.ClientClientGrossMargin,
                                       ClientNreGrossMargin = jtsl.ClientNreGrossMargin,
                                       ThisQuotationPrice = jtsl.OfferUnitPrice,
                                       ThisQuotationGrossMargin = jtsl.OfferGrossMargin,
                                       ThisQuotationClientGrossMargin = jtsl.OfferClientGrossMargin,
                                       ThisQuotationNreGrossMargin = jtsl.OfferNreGrossMargin,
                                       Id = jtsl.Id,
                                   }).ToList();
        foreach (var gradientGross in gradientQuotedGross)
        {
            if (last > 0)
            {
                var lastlist = (from lastjts in await _actualUnitPriceOffer.GetAllListAsync(p =>
                        p.AuditFlowId == AuditFlowId && p.ntype == ntype && p.GradientId == gradientGross.GradientId &&
                        p.SolutionId.Equals(gradientGross.SolutionId))
                                join sol in await _solutionQutation.GetAllListAsync(p =>
                                    p.AuditFlowId == AuditFlowId && p.ntime == last) on lastjts.version equals sol.version
                                select lastjts).ToList();
                var lastjt = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                if (lastjt is not null)
                {
                    gradientGross.LastRoundPrice = lastjt.OfferUnitPrice;
                    gradientGross.LastRoundGrossMargin = lastjt.OfferGrossMargin;
                    gradientGross.LastRoundClientGrossMargin = lastjt.OfferClientGrossMargin;
                    gradientGross.LastRoundNreGrossMargin = lastjt.OfferNreGrossMargin;
                }
            }
        }

        return gradientQuotedGross;
    }

    /// <summary>
    /// 报价毛利率测算阶梯数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertGradientQuotedGrossMargin(long AuditFlowId,
        List<GradientGrossMarginCalculateModel> gradientQuotedGross, int version, int ntype)
    {
        var list = (from gradientQuotedGro in gradientQuotedGross
                    select new GradientGrossCalculate()
                    {
                        gradient = gradientQuotedGro.gradient,
                        GradientId = gradientQuotedGro.GradientId,
                        SolutionId = gradientQuotedGro.SolutionId,
                        ntype = ntype,
                        product = gradientQuotedGro.product,
                        version = version,
                        AuditFlowId = gradientQuotedGro.AuditFlowId,
                        InteriorPrice = gradientQuotedGro.InteriorPrice,
                        InteriorGrossMargin = gradientQuotedGro.InteriorGrossMargin,
                        InteriorClientGrossMargin = gradientQuotedGro.InteriorClientGrossMargin,
                        InteriorNreGrossMargin = gradientQuotedGro.InteriorNreGrossMargin,
                        ClientPrice = gradientQuotedGro.ClientPrice,
                        ClientGrossMargin = gradientQuotedGro.ClientGrossMargin,
                        ClientClientGrossMargin = gradientQuotedGro.ClientClientGrossMargin,
                        ClientNreGrossMargin = gradientQuotedGro.ClientNreGrossMargin,
                        OfferUnitPrice = gradientQuotedGro.ThisQuotationPrice,
                        OfferGrossMargin = gradientQuotedGro.ThisQuotationGrossMargin,
                        OfferClientGrossMargin = gradientQuotedGro.ThisQuotationClientGrossMargin,
                        OfferNreGrossMargin = gradientQuotedGro.ThisQuotationNreGrossMargin
                    }).ToList();

        foreach (var actual in list)
        {
            actual.version = version;
            _actualUnitPriceOffer.InsertAsync(actual);
        }
    }

    /// <summary>
    /// 看板保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task Insertboard(long AuditFlowId, List<BoardModel> boards, int version, int ntype)
    {
        foreach (var bord in boards)
        {
            var projects = bord.ProjectBoardModels;
            var projectbords = (from board in projects
                                select new ProjectBoardSecondOffers()
                                {
                                    title = bord.title,
                                    GradientId = board.GradientId,
                                    AuditFlowId = AuditFlowId,
                                    ProjectName = board.ProjectName,
                                    version = version,
                                    ntype = ntype,
                                    InteriorTarget = board.InteriorTarget,
                                    ClientTarget = board.ClientTarget,
                                    Offer = board.Offer.Value
                                }).ToList();

            foreach (var projectbord in projectbords)
            {
                await _resourceProjectBoardSecondOffers.InsertAsync(projectbord);
            }
        }
    }

    /// <summary>
    /// 看板从数据库查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<BoardModel>> getboardForData(long AuditFlowId, int version, int ntype)
    {
        var boards =await _resourceProjectBoardSecondOffers.GetAllListAsync(p => p.AuditFlowId == AuditFlowId && p.version == version && p.ntype == ntype);
        var ntime = _solutionQutation.FirstOrDefault(p => p.AuditFlowId == AuditFlowId && p.version == version).ntime;
        var last = ntime - 1;
        List<BoardModel> boardModels = new();

        var boardmap = from board in boards group board by board.title;
        foreach (var board in boardmap)
        {
            BoardModel boardModel = new();
            boardModel.title = board.Key;
            List<ProjectBoardSecondModel> ps = new();
            foreach (var p in board)
            {
                boardModel.GradientId = p.GradientId;
                ProjectBoardSecondModel projectBoardModel = new ProjectBoardSecondModel()
                {
                    AuditFlowId = p.AuditFlowId,
                    ProjectName = p.ProjectName,
                    GradientId = p.GradientId,
                    version = version,
                    Id = p.Id,
                    InteriorTarget = p.InteriorTarget,
                    ClientTarget = p.ClientTarget,
                    Offer = p.Offer
                };

                if (last > 0)
                {
                    var lastbord = new ProjectBoardSecondOffers();
                    if (p.GradientId != 0)
                    {
                        var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(x =>
                                x.AuditFlowId == AuditFlowId && x.GradientId == p.GradientId &&
                                x.ProjectName.Equals(p.ProjectName) &&
                                x.ntype == ntype)
                            join sol in await _solutionQutation.GetAllListAsync(x =>
                                x.AuditFlowId == AuditFlowId && x.ntime == last) on lastjts.version equals sol.version
                            select lastjts).ToList();
                        lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    }
                    else if (p.GradientId == 0)
                    {
                        var lastlist = (from lastjts in await _resourceProjectBoardSecondOffers.GetAllListAsync(x =>
                                x.AuditFlowId == AuditFlowId && x.GradientId == 0 &&
                                x.ProjectName.Equals(p.ProjectName) &&
                                x.ntype == ntype)
                                        join sol in await _solutionQutation.GetAllListAsync(x =>
                                            x.AuditFlowId == AuditFlowId && x.ntime == last) on lastjts.version equals sol.version
                                        select lastjts).ToList();
                        lastbord = lastlist.OrderByDescending(p => p.version).FirstOrDefault();
                    }

                    if (lastbord is not null)
                    {
                        projectBoardModel.OldOffer = lastbord.Offer;
                    }
                }

                ps.Add(projectBoardModel);
            }

            boardModel.ProjectBoardModels = ps;
            boardModels.Add(boardModel);
        }

        return boardModels;
    }

    /// <summary>
    /// 汇总分析  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertPool(long AuditFlowId, List<PooledAnalysisModel> pooledAnalysisModels, int version,
        int ntype)
    {
        var polls = (from pool in pooledAnalysisModels
                     select new PooledAnalysisOffers()
                     {
                         Id = pool.Id,
                         AuditFlowId = AuditFlowId,
                         ProjectName = pool.ProjectName,
                         version = version,
                         ntype = ntype,
                         GrossMarginList = JsonConvert.SerializeObject(pool.GrossMarginList)
                     }).ToList();
        foreach (var pool in polls)
        {
            await _resourcePooledAnalysisOffers.InsertOrUpdateAsync(pool);
        }
    }

    /// <summary>
    /// 从数据库查询汇总分析
    /// </summary>
    /// <returns></returns>
    public async Task<List<PooledAnalysisModel>> getPoolForData(long auditFlowId, int version, int ntype)
    {
        List<PooledAnalysisOffers> pools =await _resourcePooledAnalysisOffers.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype);
        List<PooledAnalysisModel> PooledAnalysisModels = (from pool in pools
                                                          select new PooledAnalysisModel()
                                                          {
                                                              AuditFlowId = pool.AuditFlowId,
                                                              ProjectName = pool.ProjectName,
                                                              version = version,
                                                              GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(pool.GrossMarginList)
                                                          }).ToList();

        return PooledAnalysisModels;
    }

    /// <summary>
    /// 单价表数据库查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<SopAnalysisModel>> getSopForData(long auditFlowId, int version, int ntype)
    {
        List<UnitPriceOffers> ups = await _resourceUnitPriceOffers.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype);
        ups=ups.OrderBy(p => p.gradient).ToList();

        List<SopAnalysisModel> sops = (from up in ups
                                       select new SopAnalysisModel()
                                       {
                                           AuditFlowId = up.AuditFlowId,
                                           Id = up.Id,
                                           Product = up.ProductName,
                                           GradientValue = up.GradientValue,
                                           version = up.version,
                                           GrossValues = JsonConvert.DeserializeObject<List<GrossValue>>(up.GrossMarginList)
                                       }).ToList();
        return sops;
    }

    /// <summary>
    /// 单价表 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSop(List<SopAnalysisModel> unitPriceModels, int version, int ntype)
    {
        var unitPriceOffersList = (from unit in unitPriceModels
                                   select new UnitPriceOffers()
                                   {
                                       AuditFlowId = unit.AuditFlowId,
                                       ProductName = unit.Product,
                                       GradientId = unit.GradientId,
                                       GradientValue = unit.GradientValue,
                                       version = version,
                                       ntype = ntype,
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
    public async Task InsertSample(List<OnlySampleDto> onlySampleDtos, List<Solution> solutionQuotations, int version,
        int ntype)
    {
        foreach (var onlySampleDto in onlySampleDtos)
        {
            var soltuion = solutionQuotations.FirstOrDefault(p => p.Id == onlySampleDto.SolutionId);
            List<SampleQuotation> sampleQuotations = onlySampleDto.OnlySampleModels;

            foreach (var sampleQuotation in sampleQuotations)
            {
                sampleQuotation.version = version;
                sampleQuotation.ntype = ntype;
                _sampleQuotation.InsertAsync(sampleQuotation);
            }
        }
    }

    /// <summary>
    /// 获取样品阶段从数据库用于Excel
    /// </summary>
    /// <returns></returns>
    public async Task<List<SampleExcel>> getSampleExcel(long AuditFlowId,
        List<Solution> solutionQuotations, int version, int ntype)
    {
        List<SampleExcel> list = new();
        foreach (var solutionQuotation in solutionQuotations)
        {
            list.Add(new SampleExcel()
            {
                SolutionName = "方案名",
                Name = "样品阶段",
                Pcs = "需求量(pcs)",
                Cost = "成本",
                UnitPrice = "单价",
                GrossMargin = "毛利率",
                SalesRevenue = "销售收入"
            })
                ;
            SampleExcel dto = new();
            dto.SolutionName = solutionQuotation.ModuleName;

            List<SampleQuotation> sampleQuotations = _sampleQuotation.GetAll()
                .Where(p => p.AuditFlowId == AuditFlowId && p.SolutionId == solutionQuotation.Id &&
                            p.version == version && p.ntype == ntype)
                .ToList();
            var samples = (from sampleQuotation in sampleQuotations
                           select new SampleExcel()
                           {
                               SolutionName = solutionQuotation.SolutionName,
                               Name = sampleQuotation.Name,
                               Pcs = sampleQuotation.Pcs.ToString(),
                               Cost = sampleQuotation.Cost.ToString(),
                               UnitPrice = sampleQuotation.UnitPrice.ToString(),
                               GrossMargin = sampleQuotation.GrossMargin.ToString(),
                               SalesRevenue = sampleQuotation.SalesRevenue.ToString()
                           }).ToList();

            list.AddRange(samples);
        }

        return list;
    }

    /// <summary>
    /// 获取样品阶段从数据库
    /// </summary>
    /// <returns></returns>
    public async Task<List<OnlySampleDto>> getSampleForData(long AuditFlowId,
        List<Solution> solutionQuotations, int version, int ntype)
    {
        List<OnlySampleDto> onlySampleDtos = new();
        //样品阶段存的solutionQuotation.Id
        foreach (var solutionQuotation in solutionQuotations)
        {
            OnlySampleDto dto = new();
            dto.SolutionName = solutionQuotation.ModuleName;
            dto.AuditFlowId = AuditFlowId;
            List<SampleQuotation> sampleQuotations =await _sampleQuotation.GetAllListAsync(p => p.AuditFlowId == AuditFlowId && p.SolutionId == solutionQuotation.Id &&
                            p.version == version && p.ntype == ntype)
                ;
            dto.OnlySampleModels = sampleQuotations;


            onlySampleDtos.Add(dto);
        }

        return onlySampleDtos;
    }

    /// <summary>
    /// 报价方案 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSolution(List<Solution> solutions, int version, int time, long processId, bool isfirst)
    {
        SolutionQuotation solutionQuotation = new()
        {
            AuditFlowId = processId,
            SolutionListJson = JsonConvert.SerializeObject(solutions),
            ntime = time,
            IsFirst = isfirst,
            version = version
        };
        _solutionQutation.InsertAsync(solutionQuotation);
    }

    /// <summary>
    /// NRE 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertNre(List<AnalyseBoardNreDto> nres, List<Solution> solutionQuotations, int version,
        int ntype)
    {
        foreach (var nre in nres)
        {
            List<NreQuotation> nreQuotations = nre.models;
            List<DeviceQuotation> deviceQuotations = nre.devices;
            var collinearAllocationRate = nre.collinearAllocationRate;
            var numberLine = nre.numberLine;
            foreach (var nreQuotation in nreQuotations)
            {
                nreQuotation.version = version;
                nreQuotation.ntype = ntype;
                nreQuotation.numberLine = numberLine;
                nreQuotation.collinearAllocationRate = collinearAllocationRate;
                _nreQuotation.InsertAsync(nreQuotation);
            }

            foreach (var deviceQuotation in deviceQuotations)
            {
                deviceQuotation.version = version;
                deviceQuotation.ntype = ntype;
                _deviceQuotation.InsertAsync(deviceQuotation);
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
    /// 获取成本单价
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<List<PricingSecondModel>> GetPriceCost(
        List<Solution> Solutions, long auditFlowId)
    {
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        var yearList =
            await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        var sopYear = yearList.MinBy(p => p.Year);
        var soptime = sopYear.Year;
        var sopTimeType = sopYear.UpDown;
        //成本单价信息
        List<PricingSecondModel> pricingModels = new List<PricingSecondModel>();
        foreach (var solution in Solutions)
        {
            foreach (var gradient in gradients)
            {
                //Bom成本
                GetBomCostInput SopgetBomCostInput = new()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    InputCount = 0,
                    Year = soptime,
                    UpDown = sopTimeType
                };
                List<Material> sopmalist = await _priceEvaluationAppService.GetBomCost(SopgetBomCostInput);
                //全生命周期
                GetBomCostInput FullgetBomCostInput = new()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    InputCount = 0,
                    Year = 0,
                    UpDown = YearType.Year
                };
                List<Material> Fullmalist = await _priceEvaluationAppService.GetBomCost(FullgetBomCostInput);
                var bomsop = sopmalist.Sum(p => p.TotalMoneyCynNoCustomerSupply);
                var Bomfull = Fullmalist.Sum(p => p.TotalMoneyCynNoCustomerSupply);
                GetPriceEvaluationTableInput sop = new()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    InputCount = 0,
                    SolutionId = solution.Id,
                    Year = soptime,
                    UpDown = sopTimeType
                };
                var sopex = await _priceEvaluationAppService.GetPriceEvaluationTable(
                    sop);


                GetPriceEvaluationTableInput full = new()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    InputCount = 0,
                    SolutionId = solution.Id,
                    Year = 0,
                    UpDown = YearType.Year
                };
                var fullex = await _priceEvaluationAppService.GetPriceEvaluationTable(
                    full);
                var ScSop = sopex.ManufacturingCost.FirstOrDefault(p => p.CostType.Equals(CostType.Total))
                    .Subtotal; //生产成本
                var Scfull = fullex.ManufacturingCost.FirstOrDefault(p => p.CostType.Equals(CostType.Total)).Subtotal;
                var LsSop = sopex.LossCost.Sum(partsModels => partsModels.WastageCost); //损耗成本
                var Lsfull = fullex.LossCost.Sum(partsModels => partsModels.WastageCost);
                var YfSop = sopex.LogisticsFee; //运费
                var Yffull = fullex.LogisticsFee;
                var MoqSop = sopex.MoqShareCountCount; //MOQ分摊成本
                var Moqfull = fullex.MoqShareCountCount;
                var QuSop = sopex.QualityCost; //质量成本
                var Qufull = fullex.QualityCost;
                var FtSop = sopex.OtherCostItem2.Where(p => p.ItemName.Equals("单颗成本"))
                    .Sum(partsModels => partsModels.Total.Value); //分摊成本
                var Ftfull = fullex.OtherCostItem2.Where(p => p.ItemName.Equals("单颗成本"))
                    .Sum(partsModels => partsModels.Total.Value);
                var AllSop = bomsop + ScSop + LsSop + YfSop + MoqSop + QuSop + FtSop; //总成本
                var Allfull = Bomfull + Scfull + Lsfull + Yffull + Moqfull + Qufull + Ftfull;
                PricingSecondModel p = new PricingSecondModel()
                {
                    SolutionName = solution.Product,
                    SolutionId = solution.Id,
                    GradientId = gradient.Id,
                    Gradient = gradient.GradientValue.ToString(),
                    BomSop = Math.Round(bomsop, 2),
                    Bomfull = Math.Round(Bomfull, 2),
                    ScSop = Math.Round(ScSop, 2),
                    Scfull = Math.Round(Scfull, 2),
                    LsSop = Math.Round(LsSop, 2),
                    Lsfull = Math.Round(Lsfull, 2),
                    YfSop = Math.Round(YfSop, 2),
                    Yffull = Math.Round(Yffull, 2),
                    MoqSop = Math.Round(MoqSop, 2),
                    Moqfull = Math.Round(Moqfull, 2),
                    QuSop = Math.Round(QuSop, 2),
                    Qufull = Math.Round(Qufull, 2),
                    FtSop = Math.Round(FtSop, 2),
                    Ftfull = Math.Round(Ftfull, 2),
                    AllSop = Math.Round(AllSop, 2),
                    Allfull = Math.Round(Allfull, 2)
                };

                pricingModels.Add(p);
            }
        }

        return pricingModels;
    }

    /// <summary>
    /// 获取核心部件
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<List<PartsSecondModel>> GetCoreComponent(
        PriceEvaluationStartInputResult priceEvaluationStartInputResult, List<Solution> Solutions, long auditFlowId)
    {
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();
        var yearList =
            await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        var sopYear = yearList.MinBy(p => p.Year);
        var soptime = sopYear.Year;
        var sopTimeType = sopYear.UpDown;
        // 拿到产品信息
        List<CreateColumnFormatProductInformationDto> productList = priceEvaluationStartInputResult.ProductInformation;
        //获取核价相关
        List<PartsSecondModel> partsModels = new();
        foreach (var solution in Solutions)
        {
            //核心部件
            var productInformation = productList.FindFirst(p => p.Product.Equals(solution.ModuleName));


            GetBomCostInput getBomCostInput = new()
            {
                AuditFlowId = auditFlowId,
                GradientId = gradients[0].Id,
                SolutionId = solution.Id,
                InputCount = 0,
                Year = soptime,
                UpDown = sopTimeType
            };
            List<Material> malist = await _priceEvaluationAppService.GetBomCost(getBomCostInput);
            foreach (var ma in malist)
            {
                if (ma.TypeName.Equals("Sensor芯片"))
                {
                    PartsSecondModel Sensor = new();
                    Sensor.SolutionName = solution.Product;
                    Sensor.PartsName = "Sensor芯片"; //核心部件
                    Sensor.Model = ma.MaterialName; //型号（材料名称）
                    Sensor.Type = _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(productInformation.SensorTypeSelect))
                        .DisplayName; //类型（SELECT,  数据字典获取）
                    partsModels.Add(Sensor);
                }

                if (ma.TypeName.Equals("镜头"))
                {
                    PartsSecondModel Sensor = new();
                    Sensor.SolutionName = solution.Product;
                    Sensor.PartsName = "镜头"; //核心部件
                    Sensor.Model = ma.MaterialName; //型号（材料名称）
                    Sensor.Type = _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(productInformation.LensTypeSelect))
                        .DisplayName; //类型（SELECT,  数据字典获取）
                    partsModels.Add(Sensor);
                }

                if (ma.TypeName.Equals("芯片IC——ISP"))
                {
                    PartsSecondModel Sensor = new();
                    Sensor.SolutionName = solution.Product;
                    Sensor.PartsName = "芯片IC——ISP"; //核心部件
                    Sensor.Model = ma.MaterialName; //型号（材料名称）
                    Sensor.Type = _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(productInformation.IspTypeSelect))
                        .DisplayName; //类型（SELECT,  数据字典获取）
                    partsModels.Add(Sensor);
                }

                if (ma.TypeName.Equals("串行芯片"))
                {
                    PartsSecondModel Sensor = new();
                    Sensor.SolutionName = solution.Product;

                    Sensor.PartsName = "串行芯片"; //核心部件
                    Sensor.Model = ma.MaterialName; //型号（材料名称）
                    Sensor.Type = _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(productInformation.SerialChipTypeSelect))
                        .DisplayName; //类型（SELECT,  数据字典获取）
                    partsModels.Add(Sensor);
                }

                if (ma.TypeName.Equals("线束"))
                {
                    PartsSecondModel Sensor = new();
                    Sensor.SolutionName = solution.Product;

                    Sensor.PartsName = "线束"; //核心部件
                    Sensor.Model = ma.MaterialName; //型号（材料名称）
                    Sensor.Type = _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(productInformation.CableTypeSelect))
                        .DisplayName; //类型（SELECT,  数据字典获取）
                    partsModels.Add(Sensor);
                }
            }
        }

        return partsModels;
    }

    /// <summary>
    /// 报价审核表 下载--流
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="fileName"></param>
    public async Task<MemoryStream> DownloadAuditQuotationListStream(long auditFlowId, int version, int ntype,
        string fileName)
    {
        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);


        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
        var customprice = priceEvaluationStartInputResult.CustomerTargetPrice;
        string bz = "";
        decimal hl = 0;
        if (customprice is not null)
        {
            hl = customprice[0].ExchangeRate == null ? 0 : customprice[0].ExchangeRate.Value;
            bz = customprice[0].ExchangeRate == null
                ? ""
                : _exchangeRate.FirstOrDefault(p => p.Id == customprice[0].Currency).ExchangeRateKind;
        }
        else
        {
            hl = 0;
            bz = "";
        }


        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();


        //梯度走量
        var gml = priceEvaluationStartInputResult.GradientModel;
        //sop年份
        //走量信息
        List<MotionMessageSecondModel> messageModels = new List<MotionMessageSecondModel>();

        var gmlmaps = gml.GroupBy(p => p.GradientValue).ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        foreach (var gmlmap in gmlmaps)
        {
            MotionMessageSecondModel motionMessageModel = new MotionMessageSecondModel()
            {
                MessageName = gmlmap.Key + "K/Y"
            };
            var ygs = gmlmap.Value;

            List<YearValue> sopOrValueModes = new();
            for (int i = 0; i < ygs.Count; i++)
            {
                var gmys = ygs[i].GradientModelYear;

                if (i == 0)
                {
                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModes.Add(yearValue);
                    }
                }
                else
                {
                    List<YearValue> sopOrValueModesnew = new();

                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModesnew.Add(yearValue);
                    }

                    sopOrValueModes = hebing(sopOrValueModes, sopOrValueModesnew);
                }
            }


            motionMessageModel.YearValues = sopOrValueModes;


            messageModels.Add(motionMessageModel);
        }

        //实际数量
        var modelcounts = priceEvaluationStartInputResult.ModelCount;
        List<YearValue> sjsls = new();
        for (int i = 0; i < modelcounts.Count; i++)
        {
            var modelcount = modelcounts[i];
            var mcyls = modelcount.ModelCountYearList;
            List<YearValue> sjslsnew = new();
            foreach (var mcyl in mcyls)
            {
                string key = mcyl.Year.ToString();
                var ud = mcyl.UpDown;
                if (ud.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (ud.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (ud.Equals(YearType.Year))
                {
                    key += "年";
                }

                YearValue yearValue = new()
                {
                    key = key,
                    value = mcyl.Quantity
                };
                sjslsnew.Add(yearValue);
            }

            if (i == 0)
            {
                sjsls = sjslsnew;
            }
            else
            {
                sjsls = hebing(sjsls, sjslsnew);
            }
        }

        MotionMessageSecondModel sjsl = new MotionMessageSecondModel()
        {
            MessageName = "实际数量(K)"
        };
        sjsl.YearValues = sjsls;
        messageModels.Add(sjsl);

        List<MotionGradientSecondModel> mess = new List<MotionGradientSecondModel>();

        foreach (var message in messageModels)
        {
            var yvs = message.YearValues;
            foreach (var yv in yvs)
            {
                MotionGradientSecondModel mes = new()
                {
                    gradient = message.MessageName,
                    key = yv.key,
                    value = yv.value.ToString()
                };

                mess.Add(mes);
            }
        }

        List<CreateCarModelCountDto> cars = priceEvaluationStartInputResult.CarModelCount;


        List<CreateCarModelCountYearDto> yearDtos = (from car in cars
                                                     from carModelCountYear in car.ModelCountYearList
                                                     select carModelCountYear).ToList();
        // 拿到产品信息
        List<CreateColumnFormatProductInformationDto> productList = priceEvaluationStartInputResult.ProductInformation;
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        //年份
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var crm in createRequirementDtos)
        {
            SopSecondModel sop = new SopSecondModel();
            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            sop.Year = key;
            sop.Motion = yearDtos.Where(p => p.Year == crm.Year && p.UpDown.Equals(crm.UpDown))
                .Sum(p => p.Quantity);
            sop.AnnualDeclineRate = crm.AnnualDeclineRate; //年将率
            sop.AnnualRebateRequirements = crm.AnnualRebateRequirements; // 年度返利要求
            sop.OneTimeDiscountRate = crm.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = crm.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }


        //核心组件
        List<PartsSecondModel> partsModels =
            await GetCoreComponent(priceEvaluationStartInputResult, solutions, auditFlowId);


        //样品阶段
        var samples = await getSampleExcel(auditFlowId, solutions, version, ntype);

        //NRE
        var nres = await getNreForExcel(auditFlowId, version, ntype);


        //内部核价

        List<PricingSecondModel> pricingMessageSecondModels = await GetPriceCost(solutions, auditFlowId);
        //报价策略
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        var gtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype);
        gtsls = gtsls.OrderBy(p => p.GradientId).ToList();
        var soltionGradPrices = (from gtsl in gtsls
                                 select new SoltionGradPrice()
                                 {
                                     Gradientid = gtsl.GradientId,
                                     SolutionId = gtsl.SolutionId,
                                     UnitPrice = gtsl.OfferUnitPrice
                                 }).ToList();

        foreach (var gtsl in gtsls)
        {
            BiddingStrategySecondModel biddingStrategySecondModel = new BiddingStrategySecondModel()
            {
                GradientId = gtsl.GradientId,
                Product = gtsl.product,
                gradient = gtsl.gradient,
                Price = gtsl.OfferUnitPrice,
                TotallifeCyclegrossMargin = gtsl.OfferGrossMargin,
                ClientGrossMargin = gtsl.OfferClientGrossMargin,
                NreGrossMargin = gtsl.OfferNreGrossMargin
            };
            var niandu = await YearDimensionalityComparisonForGradient(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,
                GradientId = gtsl.GradientId,
                UnitPrice = gtsl.OfferUnitPrice,
                SolutionId = gtsl.SolutionId
            });
            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopCost = AverageCost[0].value;
            biddingStrategySecondModel.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            biddingStrategySecondModel.TotallifeCyclegrossMargin =
                GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(biddingStrategySecondModel);
        }


        var sjslsss = await _dynamicUnitPriceOffers.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && string.IsNullOrEmpty(p.carModel) &&
            p.ntype == ntype);
        foreach (var sjslss in sjslsss)
        {
            BiddingStrategySecondModel model = new()
            {
                Product = sjslss.ProductName,
                gradient = "实际数量",
                Price = sjslss.OfferUnitPrice,
                TotallifeCyclegrossMargin = sjslss.OffeGrossMargin,
                ClientGrossMargin = sjslss.ClientGrossMargin,
                NreGrossMargin = sjslss.NreGrossMargin
            };
            var niandu = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,
                SoltionGradPrices = soltionGradPrices,
                UnitPrice = sjslss.OfferUnitPrice,
                SolutionId = sjslss.SolutionId
            });


            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            model.SopCost = AverageCost[0].value;
            model.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            model.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            model.TotallifeCyclegrossMargin = GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(model);
        }


        var value = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
            RecordNumber = priceEvaluationStartInputResult.Number, //记录编号           
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            OfferForm = (string.IsNullOrEmpty(priceEvaluationStartInputResult.PriceEvalType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.PriceEvalType)).DisplayName, //报价形式
            SopTime = priceEvaluationStartInputResult.SopTime, //SOP时间
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目生命周期
            ForSale = (string.IsNullOrEmpty(priceEvaluationStartInputResult.SalesType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.SalesType)).DisplayName, //销售类型
            modeOfTrade = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TradeMode))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TradeMode)).DisplayName, //贸易方式
            PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
            ExchangeRate = hl, //汇率
            QuoteCurrency = bz, //报价币种
            ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
            nres = nres, //NRE费用信息
            componenSocondModels = partsModels, //核心部件
            pricingMessageSecondModels = pricingMessageSecondModels,
            samples = samples, //样品阶段
            mess = mess, //梯度走量
            BiddingStrategySecondModels = BiddingStrategySecondModels, //报价策略
            Sop = Sop
        };
        var pricetype = priceEvaluationStartInputResult.PriceEvalType;
        string templatePath = "";


        if ("PriceEvalType_Sample".Equals(pricetype))
        {
            templatePath = "wwwroot/Excel/报价审批表模板—仅含样品.xlsx";
        }
        else
        {
            templatePath = "wwwroot/Excel/报价审批表模板—含样品.xlsx";
        }

        var memoryStream = new MemoryStream();
        await MiniExcel.SaveAsByTemplateAsync(memoryStream, templatePath, value);
        return memoryStream;
    }

    /// <summary>
    /// 营销部报价审批    报价审核表 下载
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<IActionResult> DownloadAuditQuotationList(long auditFlowId, int version, int ntype)
    {
        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);


        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
        var customprice = priceEvaluationStartInputResult.CustomerTargetPrice;
        string bz = "";
        decimal hl = 0;
        if (customprice is not null)
        {
            hl = customprice[0].ExchangeRate == null ? 0 : customprice[0].ExchangeRate.Value;
            bz = customprice[0].ExchangeRate == null
                ? ""
                : _exchangeRate.FirstOrDefault(p => p.Id == customprice[0].Currency).ExchangeRateKind;
        }
        else
        {
            hl = 0;
            bz = "";
        }


        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();


        //梯度走量
        var gml = priceEvaluationStartInputResult.GradientModel;
        //sop年份
        //走量信息
        List<MotionMessageSecondModel> messageModels = new List<MotionMessageSecondModel>();

        var gmlmaps = gml.GroupBy(p => p.GradientValue).ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        foreach (var gmlmap in gmlmaps)
        {
            MotionMessageSecondModel motionMessageModel = new MotionMessageSecondModel()
            {
                MessageName = gmlmap.Key + "K/Y"
            };
            var ygs = gmlmap.Value;

            List<YearValue> sopOrValueModes = new();
            for (int i = 0; i < ygs.Count; i++)
            {
                var gmys = ygs[i].GradientModelYear;

                if (i == 0)
                {
                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModes.Add(yearValue);
                    }
                }
                else
                {
                    List<YearValue> sopOrValueModesnew = new();

                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModesnew.Add(yearValue);
                    }

                    sopOrValueModes = hebing(sopOrValueModes, sopOrValueModesnew);
                }
            }


            motionMessageModel.YearValues = sopOrValueModes;


            messageModels.Add(motionMessageModel);
        }

        //实际数量
        var modelcounts = priceEvaluationStartInputResult.ModelCount;
        List<YearValue> sjsls = new();
        for (int i = 0; i < modelcounts.Count; i++)
        {
            var modelcount = modelcounts[i];
            var mcyls = modelcount.ModelCountYearList;
            List<YearValue> sjslsnew = new();
            foreach (var mcyl in mcyls)
            {
                string key = mcyl.Year.ToString();
                var ud = mcyl.UpDown;
                if (ud.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (ud.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (ud.Equals(YearType.Year))
                {
                    key += "年";
                }

                YearValue yearValue = new()
                {
                    key = key,
                    value = mcyl.Quantity
                };
                sjslsnew.Add(yearValue);
            }

            if (i == 0)
            {
                sjsls = sjslsnew;
            }
            else
            {
                sjsls = hebing(sjsls, sjslsnew);
            }
        }

        MotionMessageSecondModel sjsl = new MotionMessageSecondModel()
        {
            MessageName = "实际数量(K)"
        };
        sjsl.YearValues = sjsls;
        messageModels.Add(sjsl);

        List<MotionGradientSecondModel> mess = new List<MotionGradientSecondModel>();

        foreach (var message in messageModels)
        {
            var yvs = message.YearValues;
            foreach (var yv in yvs)
            {
                MotionGradientSecondModel mes = new()
                {
                    gradient = message.MessageName,
                    key = yv.key,
                    value = yv.value.ToString()
                };

                mess.Add(mes);
            }
        }

        List<CreateCarModelCountDto> cars = priceEvaluationStartInputResult.CarModelCount;


        List<CreateCarModelCountYearDto> yearDtos = (from car in cars
            from carModelCountYear in car.ModelCountYearList
            select carModelCountYear).ToList();
        // 拿到产品信息
        List<CreateColumnFormatProductInformationDto> productList = priceEvaluationStartInputResult.ProductInformation;
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        //年份
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var crm in createRequirementDtos)
        {
            SopSecondModel sop = new SopSecondModel();
            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            sop.Year = key;
            sop.Motion = yearDtos.Where(p => p.Year == crm.Year && p.UpDown.Equals(crm.UpDown))
                .Sum(p => p.Quantity);
            sop.AnnualDeclineRate = crm.AnnualDeclineRate; //年将率
            sop.AnnualRebateRequirements = crm.AnnualRebateRequirements; // 年度返利要求
            sop.OneTimeDiscountRate = crm.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = crm.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }


        //核心组件
        List<PartsSecondModel> partsModels =
            await GetCoreComponent(priceEvaluationStartInputResult, solutions, auditFlowId);


        //样品阶段
        var samples = await getSampleExcel(auditFlowId, solutions, version, ntype);

        //NRE
        var nres = await getNreForExcel(auditFlowId, version, ntype);


        //内部核价

        List<PricingSecondModel> pricingMessageSecondModels = await GetPriceCost(solutions, auditFlowId);
        //报价策略
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        var gtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype);
        gtsls = gtsls.OrderBy(p => p.GradientId).ToList();
        var soltionGradPrices = (from gtsl in gtsls
            select new SoltionGradPrice()
            {
                Gradientid = gtsl.GradientId,
                SolutionId = gtsl.SolutionId,
                UnitPrice = gtsl.OfferUnitPrice
            }).ToList();

        foreach (var gtsl in gtsls)
        {
            BiddingStrategySecondModel biddingStrategySecondModel = new BiddingStrategySecondModel()
            {
                GradientId = gtsl.GradientId,
                Product = gtsl.product,
                gradient = gtsl.gradient,
                Price = gtsl.OfferUnitPrice,
                TotallifeCyclegrossMargin = gtsl.OfferGrossMargin,
                ClientGrossMargin = gtsl.ClientGrossMargin,
                NreGrossMargin = gtsl.OfferNreGrossMargin
            };
            var niandu = await YearDimensionalityComparisonForGradient(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,
                GradientId = gtsl.GradientId,
                UnitPrice = gtsl.OfferUnitPrice,
                SolutionId = gtsl.SolutionId
            });
            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopCost = AverageCost[0].value;
            biddingStrategySecondModel.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            biddingStrategySecondModel.TotallifeCyclegrossMargin =
                GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(biddingStrategySecondModel);
        }


        var sjslsss = await _dynamicUnitPriceOffers.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && string.IsNullOrEmpty(p.carModel) &&
            p.ntype == ntype);
        foreach (var sjslss in sjslsss)
        {
            BiddingStrategySecondModel model = new()
            {
                Product = sjslss.ProductName,
                gradient = "实际数量",
                Price = sjslss.OfferUnitPrice,
                TotallifeCyclegrossMargin = sjslss.OffeGrossMargin,
                ClientGrossMargin = sjslss.ClientGrossMargin,
                NreGrossMargin = sjslss.NreGrossMargin
            };
            var niandu = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = auditFlowId,
                SoltionGradPrices = soltionGradPrices,
                UnitPrice = sjslss.OfferUnitPrice,
                SolutionId = sjslss.SolutionId
            });


            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            model.SopCost = AverageCost[0].value;
            model.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            model.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            model.TotallifeCyclegrossMargin = GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(model);
        }


        var value = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
            RecordNumber = priceEvaluationStartInputResult.Number, //记录编号           
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            OfferForm = (string.IsNullOrEmpty(priceEvaluationStartInputResult.PriceEvalType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.PriceEvalType)).DisplayName, //报价形式
            SopTime = priceEvaluationStartInputResult.SopTime, //SOP时间
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目生命周期
            ForSale = (string.IsNullOrEmpty(priceEvaluationStartInputResult.SalesType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.SalesType)).DisplayName, //销售类型
            modeOfTrade = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TradeMode))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TradeMode)).DisplayName, //贸易方式
            PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
            ExchangeRate = hl, //汇率

            ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
            nres = nres, //NRE费用信息
            componenSocondModels = partsModels, //核心部件
            pricingMessageSecondModels = pricingMessageSecondModels,
            samples = samples, //样品阶段
            mess = mess, //梯度走量
            BiddingStrategySecondModels = BiddingStrategySecondModels, //报价策略
            Sop = Sop
        };


        try
        {
            using (var memoryStream = new MemoryStream())
            {
                //判断是否是仅含样品
                var pricetype = priceEvaluationStartInputResult.PriceEvalType;
                if ("PriceEvalType_Sample".Equals(pricetype))
                {
                    MiniExcel.SaveAsByTemplate(memoryStream, "wwwroot/Excel/报价审批表模板—仅含样品.xlsx", value);
                    return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = "报价审批表" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
                    };
                }


                MiniExcel.SaveAsByTemplate(memoryStream, "wwwroot/Excel/报价审批表模板—含样品.xlsx", value);
                return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = "报价审批表" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
                };
            }
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 下载成本信息表二开
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
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
        var yearList =
            await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId == analyseBoardSecondInputDto.auditFlowId);
        var sopYear = yearList.MinBy(p => p.Year);
        var soptime = sopYear.Year;
        var sopTimeType = sopYear.UpDown;
        List<CreateCarModelCountYearDto> yearDtos = (from car in cars
            from carModelCountYear in car.ModelCountYearList
            select carModelCountYear).ToList();
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();

        // 拿到产品信息
        List<CreateColumnFormatProductInformationDto> productList = priceEvaluationStartInputResult.ProductInformation;

        //年份
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var crm in createRequirementDtos)
        {
            SopSecondModel sop = new SopSecondModel();
            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            sop.Year = key;
            sop.Motion = yearDtos.Where(p => p.Year == crm.Year && p.UpDown.Equals(crm.UpDown))
                .Sum(p => p.Quantity);
            sop.AnnualDeclineRate = crm.AnnualDeclineRate; //年将率
            sop.AnnualRebateRequirements = crm.AnnualRebateRequirements; // 年度返利要求
            sop.OneTimeDiscountRate = crm.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = crm.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }


        List<PartsSecondModel> partsModels =
            await GetCoreComponent(priceEvaluationStartInputResult, Solutions, auditFlowId);
        //成本单价信息
        List<PricingSecondModel> pricingModels = await GetPriceCost(Solutions, auditFlowId);

        List<NRESecondModel> nres = new();
        //NRE费用信息
        foreach (var solution in Solutions)
        {
            PricingFormDto pricingFormDto =
                await _nrePricingAppService.GetPricingFormDownload(auditFlowId, solution.Id);
            NRESecondModel nreSecondModel = new()
            {
                SolutionName = solution.Product,
                shouban = pricingFormDto.HandPieceCostTotal,
                moju = pricingFormDto.MouldInventoryTotal,
                scsb = pricingFormDto.ProductionEquipmentCostTotal,
                gz = pricingFormDto.ToolingCostTotal,
                yj = pricingFormDto.FixtureCostTotal,
                sy = pricingFormDto.LaboratoryFeeModelsTotal,
                csrj = pricingFormDto.SoftwareTestingCostTotal,
                cl = pricingFormDto.TravelExpenseTotal,
                jianju = pricingFormDto.QAQCDepartmentsTotal,
                qt = pricingFormDto.RestsCostTotal
            };
            nres.Add(nreSecondModel);
        }

        var customprice = priceEvaluationStartInputResult.CustomerTargetPrice;
        string bz = "";
        decimal hl = 0;
        if (customprice is not null)
        {
            hl = customprice[0].ExchangeRate == null ? 0 : customprice[0].ExchangeRate.Value;
            bz = customprice[0].ExchangeRate == null
                ? ""
                : _exchangeRate.FirstOrDefault(p => p.Id == customprice[0].Currency).ExchangeRateKind;
        }
        else
        {
            hl = 0;
            bz = "";
        }

        try
        {
            var value = new
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
                RecordNumber = priceEvaluationStartInputResult.Number, //记录编号           
                Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
                DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
                TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
                OfferForm = (string.IsNullOrEmpty(priceEvaluationStartInputResult.PriceEvalType))
                    ? ""
                    : _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.PriceEvalType))
                        .DisplayName, //报价形式
                SopTime = priceEvaluationStartInputResult.SopTime, //SOP时间
                ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目生命周期
                ForSale = (string.IsNullOrEmpty(priceEvaluationStartInputResult.SalesType))
                    ? ""
                    : _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.SalesType)).DisplayName, //销售类型
                modeOfTrade = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TradeMode))
                    ? ""
                    : _financeDictionaryDetailRepository
                        .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TradeMode)).DisplayName, //贸易方式
                PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
                ExchangeRate = hl, //汇率
                Sop = Sop,
                ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
                NRE = nres, //NRE费用信息
                Parts = partsModels, //
                PriceCost = pricingModels
            };
            // values.Add(keyValuePairs);
            using (var memoryStream = new MemoryStream())
            {
                MiniExcel.SaveAsByTemplate(memoryStream, "wwwroot/Excel/新成本信息表模板.xlsx", value);
                return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = "成本信息表" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".xlsx"
                };
            }
            //  MemoryStream memoryStream = new MemoryStream();
            // return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //   await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/新成本信息表模板.xlsx", value);
            //   await memoryStream.SaveAsByTemplateAsync(templatePath, value);
            //   memoryStream.Seek(0, SeekOrigin.Begin);
            /*return new FileContentResult(memoryStream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{fileName}.xlsx"
            };*/
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
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
        decimal totalcost, long auditFlowId, long solutionid)
    {
        List<FinanceDictionaryDetail> dics = _financeDictionaryDetailRepository.GetAll()
            .Where(p => p.FinanceDictionaryId.Equals("SampleName")).ToList();
        //样品

        List<SampleQuotation> onlySampleModels = (from sample in samples
            join dic in dics on sample.Name equals (dic.Id)
            select new SampleQuotation()
            {
                AuditFlowId = auditFlowId,
                SolutionId = solutionid,
                Pcs = sample.Pcs, //需求量
                Name = dic.DisplayName, //样品阶段名称
                Cost = Math.Round(totalcost, 2) //成本：核价看板的最小梯度第一年的成本
            }).ToList();


        return onlySampleModels;
    }

    /// <summary>
    /// 数据库获取NR用于Excel
    /// </summary>     
    /// <returns></returns>
    public async Task<List<NreExcel>> getNreForExcel(long processId, int version, int ntype)
    {
        List<NreExcel> list = new();
        SolutionQuotation solutionQuotation =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(solutionQuotation.SolutionListJson);
        List<DeviceQuotation> deviceQuotations =
            _deviceQuotation.GetAll().Where(p => p.AuditFlowId == processId && p.version == version && p.ntype == ntype)
                .ToList();
        List<NreQuotation> nreQuotations = _nreQuotation.GetAll()
            .Where(p => p.AuditFlowId == processId && p.version == version && p.ntype == ntype).ToList();
        foreach (var solution in solutionList)
        {
            var nres = nreQuotations.Where(p => p.SolutionId == solution.Id).ToList();
            list.Add(new NreExcel()
            {
                PricingMoney = "线体数量：",
                OfferCoefficient = nres[0].numberLine.ToString(),
                OfferMoney = "共线分摊率：",
                Remark = nres[0].collinearAllocationRate.ToString(),
            });
            list.Add(new NreExcel()
            {
                solutionName = "方案名",
                Index = "序号",
                CostName = "费用名称",
                PricingMoney = "核价金额",
                OfferCoefficient = "报价系数",
                OfferMoney = "报价金额",
                Remark = "备注"
            });
            for (int i = 0; i < nres.Count; i++)
            {
                var nre = nres[i];
                list.Add(new NreExcel()
                {
                    solutionName = solution.Product,
                    Index = i + "",
                    CostName = nre.FormName,
                    PricingMoney = nre.PricingMoney.ToString(),
                    OfferCoefficient = nre.OfferCoefficient.ToString(),
                    OfferMoney = nre.OfferCoefficient.ToString(),
                    Remark = nre.Remark
                });
            }

            list.Add(new NreExcel()
            {
                PricingMoney = "以上之和：",
                OfferCoefficient = nres.Sum(p => p.OfferCoefficient).ToString(),
                OfferMoney = "以上之和：",
                Remark = nres.Sum(p => p.OfferMoney).ToString()
            });
            list.Add(new NreExcel()
            {
                solutionName = "方案名",
                Index = "设备名称",
                CostName = "设备单价",
                PricingMoney = "设备数量",
                OfferCoefficient = "设备金额"
            });
            var devs = deviceQuotations.Where(p => p.SolutionId == solution.Id).ToList();
            foreach (var dev in devs)
            {
                list.Add(new NreExcel()
                {
                    solutionName = solution.SolutionName,
                    Index = dev.DeviceName,
                    CostName = dev.DevicePrice.ToString(),
                    PricingMoney = dev.Number.ToString(),
                    OfferCoefficient = dev.equipmentMoney.ToString()
                });
            }
        }

        var hzre = nreQuotations.Where(p => p.SolutionId is null).ToList();

        list.Add(new NreExcel()
        {
            solutionName = "方案名",
            Index = "序号",
            CostName = "费用名称",
            PricingMoney = "核价金额",
            OfferCoefficient = "报价系数",
            OfferMoney = "报价金额",
            Remark = "备注"
        });

        for (int i = 0; i < hzre.Count; i++)
        {
            var nre = hzre[i];
            list.Add(new NreExcel()
            {
                solutionName = "汇总",
                Index = i + "",
                CostName = nre.FormName,
                PricingMoney = nre.PricingMoney.ToString(),
                OfferCoefficient = nre.OfferCoefficient.ToString(),
                OfferMoney = nre.OfferCoefficient.ToString(),
                Remark = nre.Remark
            });
        }

        list.Add(new NreExcel()
        {
            PricingMoney = "以上之和：",
            OfferCoefficient = hzre.Sum(p => p.OfferCoefficient).ToString(),
            OfferMoney = "以上之和：",
            Remark = hzre.Sum(p => p.OfferMoney).ToString()
        });


        var hzdevs = deviceQuotations.Where(p => p.SolutionId is null).ToList();

        list.Add(new NreExcel()
        {
            solutionName = "方案名",
            Index = "设备名称",
            CostName = "设备单价",
            PricingMoney = "设备数量",
            OfferCoefficient = "设备金额"
        });
        foreach (var dev in hzdevs)
        {
            list.Add(new NreExcel()
            {
                solutionName = "汇总",
                Index = dev.DeviceName,
                CostName = dev.DevicePrice.ToString(),
                PricingMoney = dev.Number.ToString(),
                OfferCoefficient = dev.equipmentMoney.ToString()
            });
        }

        return list;
    }

    /// <summary>
    /// 数据库获取NRE
    /// </summary>     
    /// <returns></returns>
    public async Task<List<AnalyseBoardNreDto>> getNreForData(long processId, int version, int ntype)
    {
        SolutionQuotation solutionQuotation =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(solutionQuotation.SolutionListJson);
        List<AnalyseBoardNreDto> nres = new List<AnalyseBoardNreDto>();
        List<DeviceQuotation> deviceQuotations =
            await _deviceQuotation.GetAllListAsync(p => p.AuditFlowId == processId && p.version == version && p.ntype == ntype)
                ;
        List<NreQuotation> nreQuotations =await _nreQuotation.GetAllListAsync(p => p.AuditFlowId == processId && p.version == version && p.ntype == ntype);
        if (nreQuotations is null || nreQuotations.Count == 0)
        {
            return nres;
        }

        nres = (from solution in solutionList
            select new AnalyseBoardNreDto
            {
                SolutionId = solution.Id,
                solutionName = solution.Product,
                numberLine = nreQuotations.FirstOrDefault(p => p.SolutionId == solution.Id).numberLine,
                collinearAllocationRate = nreQuotations.FirstOrDefault(p => p.SolutionId == solution.Id)
                    .collinearAllocationRate,
                models = nreQuotations.Where(p => p.SolutionId == solution.Id).ToList(),
                devices = deviceQuotations.Where(p => p.SolutionId == solution.Id).ToList()
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
            var solutionName = solutionTable.Product;

            GetProcessHoursEntersInput getProcessHoursEntersInput = new();
            getProcessHoursEntersInput.AuditFlowId = processId;
            getProcessHoursEntersInput.SolutionId = solutionTable.Id;

            //NRE
            AnalyseBoardNreDto analyseBoardNreDto = new();
            analyseBoardNreDto.solutionName = "NRE " + solutionName;
            PricingFormDto pricingFormDto =
                await _nrePricingAppService.GetPricingFormDownload(processId, solutionTable.Id);
            var pecs = pricingFormDto.ProductionEquipmentCost;
            pecs = pecs.Where(p => p.DeviceStatus is not null && p.DeviceStatus.Equals(FinanceConsts.Sbzt_Zy)).ToList();
            List<NreQuotation> models = new List<NreQuotation>();
            NreQuotation shouban = new();
            shouban.FormName = "手板件费";
            shouban.SolutionId = solutionTable.Id;
            shouban.AuditFlowId = processId;
            shouban.PricingMoney = Math.Round(pricingFormDto.HandPieceCostTotal, 2);
            handPieceCostTotals += pricingFormDto.HandPieceCostTotal;
            models.Add(shouban);
            NreQuotation mouju = new();
            mouju.FormName = "模具费";
            mouju.SolutionId = solutionTable.Id;
            mouju.AuditFlowId = processId;
            mouju.PricingMoney = Math.Round(pricingFormDto.MouldInventoryTotal, 2);
            mouldInventoryTotals += pricingFormDto.MouldInventoryTotal;
            models.Add(mouju);
            NreQuotation scsb = new();
            scsb.FormName = "生产设备费";
            scsb.AuditFlowId = processId;
            scsb.SolutionId = solutionTable.Id;
            scsb.PricingMoney = Math.Round(pricingFormDto.ProductionEquipmentCostTotal, 2);
            productionEquipmentCostTotals += pricingFormDto.ProductionEquipmentCostTotal;
            models.Add(scsb);
            NreQuotation gzf = new();
            gzf.FormName = "工装费";
            gzf.SolutionId = solutionTable.Id;
            gzf.AuditFlowId = processId;
            gzf.PricingMoney = Math.Round(pricingFormDto.ToolingCostTotal, 2);
            toolingCostTotals += pricingFormDto.ToolingCostTotal;
            models.Add(gzf);
            NreQuotation yjf = new();
            yjf.FormName = "治具费";
            yjf.AuditFlowId = processId;
            yjf.SolutionId = solutionTable.Id;
            yjf.PricingMoney = Math.Round(pricingFormDto.FixtureCostTotal, 2);
            fixtureCostTotals += pricingFormDto.FixtureCostTotal;
            models.Add(yjf);
            NreQuotation jjf = new();
            jjf.FormName = "检具费";
            jjf.AuditFlowId = processId;
            jjf.SolutionId = solutionTable.Id;
            jjf.PricingMoney = Math.Round(pricingFormDto.QAQCDepartmentsTotal, 2);
            qAQCDepartmentsTotals += pricingFormDto.QAQCDepartmentsTotal;
            models.Add(jjf);
            NreQuotation syf = new();
            syf.FormName = "实验费";
            syf.AuditFlowId = processId;
            syf.SolutionId = solutionTable.Id;
            syf.PricingMoney = Math.Round(pricingFormDto.LaboratoryFeeModelsTotal, 2);
            laboratoryFeeModelsTotals += pricingFormDto.LaboratoryFeeModelsTotal;
            models.Add(syf);
            NreQuotation csrjf = new();
            csrjf.FormName = "测试软件费";
            csrjf.SolutionId = solutionTable.Id;
            csrjf.AuditFlowId = processId;
            csrjf.PricingMoney = Math.Round(pricingFormDto.SoftwareTestingCostTotal, 2);
            softwareTestingCostTotals += pricingFormDto.SoftwareTestingCostTotal;
            models.Add(csrjf);
            NreQuotation clf = new();
            clf.FormName = "差旅费";
            clf.SolutionId = solutionTable.Id;
            clf.AuditFlowId = processId;

            clf.PricingMoney = Math.Round(pricingFormDto.TravelExpenseTotal, 2);
            travelExpenseTotals += pricingFormDto.TravelExpenseTotal;
            models.Add(clf);
            NreQuotation qtfy = new();
            qtfy.FormName = "其他费用";
            qtfy.SolutionId = solutionTable.Id;
            qtfy.AuditFlowId = processId;

            qtfy.PricingMoney = Math.Round(pricingFormDto.RestsCostTotal, 2);
            restsCostTotals += pricingFormDto.RestsCostTotal;
            models.Add(qtfy);


            List<UphAndValue> UphAndValues = pricingFormDto.UphAndValues;
            foreach (var uphAndValue in UphAndValues)
            {
                if ((OperateTypeCode.xtsl.ToString()).Equals(uphAndValue.Uph))
                {
                    analyseBoardNreDto.numberLine = uphAndValue.Value; //线体数量
                }

                if ((OperateTypeCode.gxftl.ToString()).Equals(uphAndValue.Uph))
                {
                    analyseBoardNreDto.collinearAllocationRate = uphAndValue.Value; //共线分摊率
                }
            }

            //获取设备
            List<DeviceQuotation> deviceModels = new();
            foreach (var pec in pecs)
            {
                DeviceQuotation deviceModel = new();
                deviceModel.DeviceName = pec.EquipmentName;
                deviceModel.SolutionId = solutionTable.Id;
                deviceModel.Number = pec.Number;
                deviceModel.AuditFlowId = processId;
                deviceModel.DevicePrice = Math.Round(pec.UnitPrice, 2);
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
        shoubanhz.AuditFlowId = processId;
        shoubanhz.PricingMoney = Math.Round(handPieceCostTotals, 2);
        hz.Add(shoubanhz);
        NreQuotation moujuhz = new();
        moujuhz.FormName = "模具费";
        moujuhz.AuditFlowId = processId;

        moujuhz.PricingMoney = Math.Round(mouldInventoryTotals, 2);

        hz.Add(moujuhz);
        NreQuotation scsbhz = new();
        scsbhz.FormName = "生产设备费";
        scsbhz.AuditFlowId = processId;

        scsbhz.PricingMoney = Math.Round(productionEquipmentCostTotals, 2);

        hz.Add(scsbhz);
        NreQuotation gzfhz = new();
        gzfhz.FormName = "工装费";
        gzfhz.AuditFlowId = processId;
        gzfhz.PricingMoney = Math.Round(toolingCostTotals, 2);
        hz.Add(gzfhz);
        NreQuotation yjfhz = new();
        yjfhz.FormName = "治具费";
        yjfhz.AuditFlowId = processId;
        yjfhz.PricingMoney = Math.Round(fixtureCostTotals, 2);
        hz.Add(yjfhz);
        NreQuotation jjfhz = new();
        jjfhz.FormName = "检具费";
        jjfhz.AuditFlowId = processId;
        jjfhz.PricingMoney = Math.Round(qAQCDepartmentsTotals, 2);
        hz.Add(jjfhz);
        NreQuotation syfhz = new();
        syfhz.FormName = "实验费";
        syfhz.AuditFlowId = processId;
        syfhz.PricingMoney = Math.Round(laboratoryFeeModelsTotals, 2);
        hz.Add(syfhz);
        NreQuotation csrjfhz = new();
        csrjfhz.FormName = "测试软件费";
        csrjfhz.AuditFlowId = processId;
        csrjfhz.PricingMoney = Math.Round(softwareTestingCostTotals, 2);
        hz.Add(csrjfhz);
        NreQuotation clfhz = new();
        clfhz.FormName = "差旅费";
        clfhz.AuditFlowId = processId;
        clfhz.PricingMoney = Math.Round(travelExpenseTotals, 2);
        hz.Add(clfhz);
        NreQuotation qtfyhz = new();
        qtfyhz.FormName = "其他费用";
        qtfyhz.AuditFlowId = processId;
        qtfyhz.PricingMoney = Math.Round(restsCostTotals, 2);
        hz.Add(qtfyhz);
        Dictionary<string, List<DeviceQuotation>> demaps =
            hzde.GroupBy(p => p.DeviceName).ToDictionary(x => x.Key, x => x.Select(e => e).ToList());
        List<DeviceQuotation> decs = new();
        foreach (var demap in demaps)
        {
            string devicename = demap.Key;
            var des = demap.Value;
            DeviceQuotation dev = new()
            {
                AuditFlowId = processId,
                DeviceName = devicename,
                Number = des.Sum(p => p.Number),
                DevicePrice = des[0].DevicePrice,
                equipmentMoney = des.Sum(p => p.equipmentMoney)
            };

            decs.Add(dev);
        }

        analyseBoardNreDto1.devices = decs;
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
    /// 获取报价目录
    /// </summary>
    /// <param name="departmentDtos"></param>
    /// <returns></returns>
    public async Task<List<SolutionQuotationDto>> GeCatalogue(long auditFlowId)
    {
        List<SolutionQuotation> sols =
            await _solutionQutation.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        List<SolutionQuotationDto> solts = new();
        foreach (var sol in sols)
        {
            var isQuotation = await getQuotation(auditFlowId, sol.version);
            solts.Add(new SolutionQuotationDto()
            {
                version = sol.version,
                Id = sol.Id,
                ntime = sol.ntime,
                AuditFlowId = auditFlowId,
                isQuotation = isQuotation,
                IsFirst = sol.IsFirst,
                IsCOB = sol.IsCOB,
                solutionList = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson)
            });
        }

        return solts;
    }

    /// <summary>
    /// 获取是否生成报价反馈
    /// </summary>
    /// <param name="departmentDtos"></param>
    /// <returns></returns>
    public async Task<bool> getQuotation(long auditFlowId, int version)
    {
        var nrecount =
            await _nreQuotation.CountAsync(p => p.AuditFlowId == auditFlowId && p.version == version && p.ntype == 1);
        if (nrecount > 0)
        {
            return true;
        }

        var samplecount =
            await _sampleQuotation.CountAsync(p =>
                p.AuditFlowId == auditFlowId && p.version == version && p.ntype == 1);
        if (samplecount > 0)
        {
            return true;
        }

        return false;
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
    public async Task<QuotationListSecondDto> QuotationListSecond(long processId, int version, int type)
    {
        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);


        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);
        var customprice = priceEvaluationStartInputResult.CustomerTargetPrice;
        string bz = "";
        decimal hl = 0;
        if (customprice is not null)
        {
            hl = customprice[0].ExchangeRate == null ? 0 : customprice[0].ExchangeRate.Value;
            bz = customprice[0].ExchangeRate == null
                ? ""
                : _exchangeRate.FirstOrDefault(p => p.Id == customprice[0].Currency).ExchangeRateKind;
        }
        else
        {
            hl = 0;
            bz = "";
        }


        QuotationListSecondDto pp = new QuotationListSecondDto
        {
            Date = DateTime.Now, //查询日期
            RecordNumber = priceEvaluationStartInputResult.Number, // 单据编号
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            OfferForm = (string.IsNullOrEmpty(priceEvaluationStartInputResult.PriceEvalType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.PriceEvalType)).DisplayName, //报价形式
            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            ClientNature = priceEvaluationStartInputResult.CustomerNature, //客户性质
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            TerminalClientNature = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TerminalNature))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TerminalNature))
                    .DisplayName, //终端客户性质
            //开发计划 手工录入
            SopTime = priceEvaluationStartInputResult.SopTime, //Sop时间
            ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目周期
            ForSale = (string.IsNullOrEmpty(priceEvaluationStartInputResult.SalesType))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.SalesType)).DisplayName, //销售类型
            modeOfTrade = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TradeMode))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TradeMode)).DisplayName, //贸易方式
            PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
            QuoteCurrency = bz, //报价币种
            ExchangeRate = hl, //汇率
            ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
            AuditFlowId = processId,
            version = version
        };
        pp.nres = await getNreForData(processId, version, type);
        //获取核价营销相关数据
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);

        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();


        //梯度走量
        var gml = priceEvaluationStartInputResult.GradientModel;
        //sop年份
        //走量信息
        List<MotionMessageSecondModel> messageModels = new List<MotionMessageSecondModel>();

        var gmlmaps = gml.GroupBy(p => p.GradientValue).ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        foreach (var gmlmap in gmlmaps)
        {
            MotionMessageSecondModel motionMessageModel = new MotionMessageSecondModel()
            {
                MessageName = gmlmap.Key + "K/Y"
            };
            var ygs = gmlmap.Value;

            List<YearValue> sopOrValueModes = new();
            for (int i = 0; i < ygs.Count; i++)
            {
                var gmys = ygs[i].GradientModelYear;

                if (i == 0)
                {
                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModes.Add(yearValue);
                    }
                }
                else
                {
                    List<YearValue> sopOrValueModesnew = new();

                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModesnew.Add(yearValue);
                    }

                    sopOrValueModes = hebing(sopOrValueModes, sopOrValueModesnew);
                }
            }


            motionMessageModel.YearValues = sopOrValueModes;


            messageModels.Add(motionMessageModel);
        }


        //实际数量
        var modelcounts = priceEvaluationStartInputResult.ModelCount;
        List<YearValue> sjsls = new();
        for (int i = 0; i < modelcounts.Count; i++)
        {
            var modelcount = modelcounts[i];
            var mcyls = modelcount.ModelCountYearList;
            List<YearValue> sjslsnew = new();
            foreach (var mcyl in mcyls)
            {
                string key = mcyl.Year.ToString();
                var ud = mcyl.UpDown;
                if (ud.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (ud.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (ud.Equals(YearType.Year))
                {
                    key += "年";
                }

                YearValue yearValue = new()
                {
                    key = key,
                    value = mcyl.Quantity
                };
                sjslsnew.Add(yearValue);
            }

            if (i == 0)
            {
                sjsls = sjslsnew;
            }
            else
            {
                sjsls = hebing(sjsls, sjslsnew);
            }
        }

        MotionMessageSecondModel sjsl = new MotionMessageSecondModel()
        {
            MessageName = "实际数量(K)"
        };
        sjsl.YearValues = sjsls;
        messageModels.Add(sjsl);

        List<MotionGradientSecondModel> mess = new List<MotionGradientSecondModel>();

        foreach (var message in messageModels)
        {
            var yvs = message.YearValues;
            foreach (var yv in yvs)
            {
                MotionGradientSecondModel mes = new()
                {
                    gradient = message.MessageName,
                    key = yv.key,
                    value = yv.value.ToString()
                };

                mess.Add(mes);
            }
        }

        pp.motion = mess;


        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var crm in createRequirementDtos)
        {
            SopSecondModel sop = new SopSecondModel();
            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            sop.Year = key;

            sop.AnnualDeclineRate = crm.AnnualDeclineRate; //年将率
            sop.AnnualRebateRequirements = crm.AnnualRebateRequirements; // 年度返利要求
            sop.OneTimeDiscountRate = crm.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = crm.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }

        pp.sops = Sop;


        //核心组件
        List<PartsSecondModel> partsModels =
            await GetCoreComponent(priceEvaluationStartInputResult, solutions, processId);
        pp.componenSocondModels = partsModels;


        //样品阶段
        var samples = await getSampleForData(processId, solutions, version, type);
        var pricetype = priceEvaluationStartInputResult.PriceEvalType;
        pp.SampleOffer = samples;
        if ("PriceEvalType_Sample".Equals(pricetype))
        {
            return pp;
        }


        //NRE
        var nres = await getNreForData(processId, version, type);
        pp.nres = nres;

        //内部核价

        List<PricingSecondModel> pricingMessageSecondModels = await GetPriceCost(solutions, processId);


        pp.pricingMessageSecondModels = pricingMessageSecondModels;


        //报价策略
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        var gtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == processId && p.version == version && p.ntype == type);
        gtsls = gtsls.OrderBy(p => p.GradientId).ToList();
        var soltionGradPrices = (from gtsl in gtsls
            select new SoltionGradPrice()
            {
                Gradientid = gtsl.GradientId,
                SolutionId = gtsl.SolutionId,
                UnitPrice = gtsl.OfferUnitPrice
            }).ToList();

        foreach (var gtsl in gtsls)
        {
            BiddingStrategySecondModel biddingStrategySecondModel = new BiddingStrategySecondModel()
            {
                GradientId = gtsl.GradientId,
                Product = gtsl.product,
                gradient = gtsl.gradient,
                Price = gtsl.OfferUnitPrice,
                TotallifeCyclegrossMargin = gtsl.OfferGrossMargin,
                ClientGrossMargin = gtsl.OfferClientGrossMargin,
                NreGrossMargin = gtsl.OfferNreGrossMargin
            };
            var niandu = await YearDimensionalityComparisonForGradient(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = processId,
                GradientId = gtsl.GradientId,
                UnitPrice = gtsl.OfferUnitPrice,
                SolutionId = gtsl.SolutionId
            });
            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopCost = AverageCost[0].value;
            biddingStrategySecondModel.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            biddingStrategySecondModel.TotallifeCyclegrossMargin =
                GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(biddingStrategySecondModel);
        }


        var sjslsss = await _dynamicUnitPriceOffers.GetAllListAsync(p =>
            p.AuditFlowId == processId && p.version == version && string.IsNullOrEmpty(p.carModel) && p.ntype == type);
        foreach (var sjslss in sjslsss)
        {
            BiddingStrategySecondModel model = new()
            {
                Product = sjslss.ProductName,
                gradient = "实际数量",
                Price = sjslss.OfferUnitPrice,
                TotallifeCyclegrossMargin = sjslss.OffeGrossMargin,
                ClientGrossMargin = sjslss.ClientGrossMargin,
                NreGrossMargin = sjslss.NreGrossMargin
            };
            var niandu = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = processId,
                SoltionGradPrices = soltionGradPrices,
                UnitPrice = sjslss.OfferUnitPrice,
                SolutionId = sjslss.SolutionId
            });


            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            model.SopCost = AverageCost[0].value;
            model.FullLifeCyclecost =
                AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            model.SopGrossMargin = GrossMargin[0].value; //sop毛利率
            model.TotallifeCyclegrossMargin = GrossMargin.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModels.Add(model);
        }


        pp.BiddingStrategySecondModels = BiddingStrategySecondModels;
        return pp;
    }

    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long processId, int version, int ntype)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto = new();

        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);
        //样品阶段

        var pricetype = priceEvaluationStartInputResult.PriceEvalType;


        List<UnitPriceSumModel> unitPriceSumModels = new();
        List<NREUnitSumModel> NreUnitSumModels = new();

        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        var gtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == processId && p.version == version && p.ntype == ntype);
        gtsls = gtsls.OrderBy(p => p.GradientId).ToList();
        var soltionGradPrices = (from gtsl in gtsls
            select new SoltionGradPrice()
            {
                Gradientid = gtsl.GradientId,
                SolutionId = gtsl.SolutionId,
                UnitPrice = gtsl.OfferUnitPrice
            }).ToList();


        List<ManagerApprovalOfferNre> ManagerApprovalOfferNres = new List<ManagerApprovalOfferNre>();
        foreach (var solution in solutions)
        {
            //实际数量(合计)
            var heji = await _dynamicUnitPriceOffers.FirstOrDefaultAsync(p =>
                p.version == version && p.AuditFlowId == processId && p.SolutionId == solution.Id &&
                string.IsNullOrEmpty(p.carModel) && p.ntype == ntype);
            var value = heji.OfferUnitPrice;

            var ma = new ManagerApprovalOfferNre()
            {
                solutionName = heji.ProductName,
                SolutionId = heji.SolutionId,
                OfferUnitPrice = heji.OfferUnitPrice,
                OfferGrossMargin = heji.OffeGrossMargin,
                OfferClientGrossMargin = heji.ClientGrossMargin,
                OfferNreGrossMargin = heji.NreGrossMargin
            };
            var niandu = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = processId,
                SoltionGradPrices = soltionGradPrices,
                UnitPrice = heji.OfferUnitPrice,
                SolutionId = heji.SolutionId
            });
            ma.SopCost = niandu.AverageCost[0].value;
            ma.fullCost = niandu.AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            ma.SalesRevenue = niandu.SalesRevenue.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            ma.SellingCost = niandu.SellingCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            ManagerApprovalOfferNres.Add(ma);


            //NRE报价汇总
            var nrelist = await _nreQuotation.GetAllListAsync(p =>
                p.version == version && p.AuditFlowId == processId && p.SolutionId == solution.Id && p.ntype == ntype);
            var number = nrelist.Sum(p => p.OfferMoney); //报价金额

            var cost = nrelist.Sum(p => p.PricingMoney); //核价成本
            UnitPriceSumModel unitPriceSumModel = new UnitPriceSumModel()
            {
                Product = solution.ModuleName,
                price = value
            };


            NREUnitSumModel nreUnitSumModel = new()
            {
                Product = solution.ModuleName,
                cost = cost,
                number = number
            };

            unitPriceSumModels.Add(unitPriceSumModel);
            NreUnitSumModels.Add(nreUnitSumModel);
        }

        var nres = await getNreForData(processId, version, ntype);

        managerApprovalOfferDto.nre = nres.FirstOrDefault(p => p.solutionName.Equals("汇总"));

        managerApprovalOfferDto.UnitPriceSum = unitPriceSumModels;
        managerApprovalOfferDto.NreUnitSumModels = NreUnitSumModels;
        managerApprovalOfferDto.ManagerApprovalOfferNres = ManagerApprovalOfferNres;
        return managerApprovalOfferDto;
    }

    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long processId, int version, int ntype)
    {
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);


        QuotationListSecondDto pp = new QuotationListSecondDto
        {
            Date = DateTime.Now, //查询日期

            DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
            ClientNature = priceEvaluationStartInputResult.CustomerNature, //客户性质
            TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
            TerminalClientNature = (string.IsNullOrEmpty(priceEvaluationStartInputResult.TerminalNature))
                ? ""
                : _financeDictionaryDetailRepository
                    .FirstOrDefault(p => p.Id.Equals(priceEvaluationStartInputResult.TerminalNature))
                    .DisplayName, //终端客户性质 //终端客户性质
            //开发计划 手工录入

            //  ExchangeRate = priceEvaluationStartInputResult.ExchangeRate, //汇率
        };

        if ("PriceEvalType_Sample".Equals(priceEvaluationStartInputResult.PriceEvalType))
        {
            var samples = await getSampleForData(processId, solutions, version, ntype);
            pp.SampleOffer = samples;
            return pp;
        }

        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);

        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        List<GradientGrossMarginModel> gradientGrossMarginModels = new();
        //获取毛利率
        List<decimal> gross = await GetGrossMargin();
        var gml = priceEvaluationStartInputResult.GradientModel;
        //走量信息
        List<MotionMessageSecondModel> messageModels = new List<MotionMessageSecondModel>();

        var gmlmaps = gml.GroupBy(p => p.GradientValue).ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        foreach (var gmlmap in gmlmaps)
        {
            MotionMessageSecondModel motionMessageModel = new MotionMessageSecondModel()
            {
                MessageName = gmlmap.Key + "K/Y"
            };
            var ygs = gmlmap.Value;

            List<YearValue> sopOrValueModes = new();
            for (int i = 0; i < ygs.Count; i++)
            {
                var gmys = ygs[i].GradientModelYear;

                if (i == 0)
                {
                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModes.Add(yearValue);
                    }
                }
                else
                {
                    List<YearValue> sopOrValueModesnew = new();

                    foreach (var gmy in gmys)
                    {
                        string key = gmy.Year.ToString();
                        var ud = gmy.UpDown;
                        if (ud.Equals(YearType.FirstHalf))
                        {
                            key += "上半年";
                        }

                        if (ud.Equals(YearType.SecondHalf))
                        {
                            key += "下半年";
                        }

                        if (ud.Equals(YearType.Year))
                        {
                            key += "年";
                        }

                        YearValue yearValue = new YearValue()
                        {
                            key = key,
                            value = gmy.Count
                        };
                        sopOrValueModesnew.Add(yearValue);
                    }

                    sopOrValueModes = hebing(sopOrValueModes, sopOrValueModesnew);
                }
            }


            motionMessageModel.YearValues = sopOrValueModes;


            messageModels.Add(motionMessageModel);
        }


        //实际数量
        var modelcounts = priceEvaluationStartInputResult.ModelCount;
        List<YearValue> sjsls = new();
        for (int i = 0; i < modelcounts.Count; i++)
        {
            var modelcount = modelcounts[i];
            var mcyls = modelcount.ModelCountYearList;
            List<YearValue> sjslsnew = new();
            foreach (var mcyl in mcyls)
            {
                string key = mcyl.Year.ToString();
                var ud = mcyl.UpDown;
                if (ud.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (ud.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (ud.Equals(YearType.Year))
                {
                    key += "年";
                }

                YearValue yearValue = new()
                {
                    key = key,
                    value = mcyl.Quantity
                };
                sjslsnew.Add(yearValue);
            }

            if (i == 0)
            {
                sjsls = sjslsnew;
            }
            else
            {
                sjsls = hebing(sjsls, sjslsnew);
            }
        }

        MotionMessageSecondModel sjsl = new MotionMessageSecondModel()
        {
            MessageName = "实际数量(K)"
        };
        sjsl.YearValues = sjsls;
        messageModels.Add(sjsl);
        List<MotionGradientSecondModel> mess = new List<MotionGradientSecondModel>();
        foreach (var message in messageModels)
        {
            var yvs = message.YearValues;
            foreach (var yv in yvs)
            {
                MotionGradientSecondModel mes = new()
                {
                    gradient = message.MessageName,
                    key = yv.key,
                    value = yv.value.ToString()
                };

                mess.Add(mes);
            }
        }

        pp.motion = mess;
        var crms = priceEvaluationStartInputResult.Requirement;
        var createRequirementDtos = priceEvaluationStartInputResult.Requirement;
        List<SopSecondModel> Sop = new List<SopSecondModel>();
        foreach (var crm in createRequirementDtos)
        {
            SopSecondModel sop = new SopSecondModel();
            string key = crm.Year.ToString();
            var ud = crm.UpDown;
            if (ud.Equals(YearType.FirstHalf))
            {
                key += "上半年";
            }

            if (ud.Equals(YearType.SecondHalf))
            {
                key += "下半年";
            }

            if (ud.Equals(YearType.Year))
            {
                key += "年";
            }

            sop.Year = key;

            sop.AnnualDeclineRate = crm.AnnualDeclineRate; //年将率
            sop.AnnualRebateRequirements = crm.AnnualRebateRequirements; // 年度返利要求
            sop.OneTimeDiscountRate = crm.OneTimeDiscountRate; //一次性折让率（%）
            sop.CommissionRate = crm.CommissionRate; //年度佣金比例（%）
            Sop.Add(sop);
        }

        pp.sops = Sop;


        //核心组件
        List<PartsSecondModel> partsModels =
            await GetCoreComponent(priceEvaluationStartInputResult, solutions, processId);
        pp.componenSocondModels = partsModels;


        var nres = await getNreForData(processId, version, ntype);

        pp.nres = nres.FindAll(p => p.solutionName.Equals("汇总"));


        pp.pricingMessageSecondModels = await GetPriceCost(solutions, processId);


        //报价策略梯度
        List<BiddingStrategySecondModel> BiddingStrategySecondModels = new();
        var gtsls = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == processId && p.version == version && p.ntype == ntype);
        gtsls = gtsls.OrderBy(p => p.GradientId).ToList();
        var soltionGradPrices = (from gtsl in gtsls
                                 select new SoltionGradPrice()
                                 {
                                     Gradientid = gtsl.GradientId,
                                     SolutionId = gtsl.SolutionId,
                                     UnitPrice = gtsl.OfferUnitPrice
                                 }).ToList();

        foreach (var gtsl in gtsls)
        {
            BiddingStrategySecondModel biddingStrategySecondModel = new BiddingStrategySecondModel()
            {
                GradientId = gtsl.GradientId,
                Product = gtsl.product,
                gradient = gtsl.gradient,
                Price = gtsl.OfferUnitPrice,
                TotallifeCyclegrossMargin = gtsl.OfferGrossMargin,
                ClientGrossMargin = gtsl.OfferClientGrossMargin,
                NreGrossMargin = gtsl.OfferNreGrossMargin
            };
            var niandu = await YearDimensionalityComparisonForGradient(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = processId,
                GradientId = gtsl.GradientId,
                UnitPrice = gtsl.OfferUnitPrice,
                SolutionId = gtsl.SolutionId
            });
            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            biddingStrategySecondModel.SopCost = AverageCost[0].value;
            biddingStrategySecondModel.FullLifeCyclecost =
                niandu.AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            biddingStrategySecondModel.SopGrossMargin = niandu.GrossMargin[0].value; //sop毛利率
            BiddingStrategySecondModels.Add(biddingStrategySecondModel);
        }


        pp.BiddingStrategySecondModelsGradent = BiddingStrategySecondModels;


        //  报价策略-实际数量(合计)
        List<BiddingStrategySecondModel> BiddingStrategySecondModelsAct = new();

        var sjslsss = await _dynamicUnitPriceOffers.GetAllListAsync(p =>
            p.AuditFlowId == processId && p.version == version && string.IsNullOrEmpty(p.carModel) && p.ntype == ntype);
        foreach (var sjslss in sjslsss)
        {
            BiddingStrategySecondModel model = new()
            {
                Product = sjslss.ProductName,
                gradient = "实际数量",
                Price = sjslss.OfferUnitPrice,
                TotallifeCyclegrossMargin = sjslss.OffeGrossMargin,
                ClientGrossMargin = sjslss.ClientGrossMargin,
                NreGrossMargin = sjslss.NreGrossMargin
            };
            var niandu = await PostYearDimensionalityComparisonForactual(new YearProductBoardProcessSecondDto()
            {
                AuditFlowId = processId,
                SoltionGradPrices = soltionGradPrices,
                UnitPrice = sjslss.OfferUnitPrice,
                SolutionId = sjslss.SolutionId
            });
            var AverageCost = niandu.AverageCost.OrderBy(p => p.key).ToList();
            model.SopCost = AverageCost[0].value;
            var GrossMargin = niandu.GrossMargin.OrderBy(p => p.key).ToList();
            model.SopGrossMargin = GrossMargin[0].value;
            model.FullLifeCyclecost = niandu.AverageCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            model.SalesRevenue = niandu.SalesRevenue.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            model.SellingCost = niandu.SellingCost.FirstOrDefault(p => p.key.Equals("全生命周期")).value;
            BiddingStrategySecondModelsAct.Add(model);
        }

        var sampless = await getSampleForData(processId, solutions, version, ntype);
        pp.SampleOffer = sampless;
        pp.BiddingStrategySecondModelsAct = BiddingStrategySecondModelsAct;


        return pp;
    }

    /// <summary>
    /// 获取审批
    /// </summary>
    /// <returns></returns>
    public async Task<AuditQuotationList> GetfinanceAuditQuotationList(long auditFlowId, int version, int ntype,
        int nsource)
    {
        return _financeAuditQuotationList.FirstOrDefault(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype && p.nsource == nsource);
    }

    /// <summary>
    /// 保存审批
    /// </summary>
    /// <returns></returns>
    public async Task InsertfinanceAuditQuotationList(string content, long auditFlowId, int version, int ntype,
        int nsource)
    {
        _financeAuditQuotationList.InsertAsync(new AuditQuotationList()
        {
            AuditFlowId = auditFlowId,
            ntype = ntype,
            version = version,
            nsource = nsource,
            AuditQuotationListJson = content
        });
    }

    /// <summary>
    /// 编辑审批
    /// </summary>
    /// <returns></returns>
    public async Task UpadatefinanceAuditQuotationList(string content, long auditFlowId, int version, int ntype,
        int source)
    {
        var auditQuotationList = _financeAuditQuotationList.FirstOrDefault(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.ntype == ntype && p.nsource == source);
        auditQuotationList.AuditQuotationListJson = content;
        _financeAuditQuotationList.UpdateAsync(auditQuotationList);
    }

    internal async Task<List<long>> GetExternalQuotationNumberOfQuotations(long auditFlowId, long solutionId)
    {
        List<ExternalQuotation> externalQuotations =
            await _externalQuotation.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
        List<long> prop = externalQuotations.Select(p => p.NumberOfQuotations).OrderBy(p => p).ToList();
        long ii = await _externalQuotation.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.IsSubmit);
        List<SolutionQuotationDto> solutionQuotations = await GeCatalogue(auditFlowId);
        if (solutionQuotations.Count() == 0) throw new FriendlyException("报价看板的组合方案未查询到");
        var icount = Convert.ToDecimal(ii / solutionQuotations.Count());
        int icount2 = Convert.ToInt32(Math.Floor(icount).ToString());
        if (prop.Count == 0)
        {
            prop = new();
            prop.Add(1);
        }

        for (int i = 0; i < icount2; i++)
        {
            long pp = prop.Last();
            if (pp < 3)
            {
                prop.Add(pp + 1);
            }
        }

        return prop;
    }

    internal async Task<ExternalQuotationDto> GetExternalQuotation(long auditFlowId, long solutionId,
        long numberOfQuotations, List<ProductDto> productDtos, List<QuotationNreDto> quotationNreDtos)
    {
        ExternalQuotationDto externalQuotationDto = new ExternalQuotationDto();
        List<ExternalQuotation> externalQuotations =
            await _externalQuotation.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));

        if (externalQuotations.Count != 0 &&
            externalQuotations.Max(p => p.NumberOfQuotations) + 1 < numberOfQuotations || numberOfQuotations == 0)
        {
            throw new FriendlyException($"version:{numberOfQuotations}版本号有误!");
        }

        if (externalQuotations.Count == 0 && numberOfQuotations != 1)
        {
            throw new FriendlyException($"version:{numberOfQuotations}版本号有误!");
        }

        if (numberOfQuotations > 3)
        {
            throw new FriendlyException($"version:{numberOfQuotations}版本号最大为3!");
        }

        ExternalQuotation externalQuotation = externalQuotations.FirstOrDefault(p =>
            p.SolutionId.Equals(solutionId) && p.NumberOfQuotations.Equals(numberOfQuotations));
        if (externalQuotation is not null)
        {
            List<ProductExternalQuotationMx> externalQuotationMxs =
                await _externalQuotationMx.GetAllListAsync(p => p.ExternalQuotationId.Equals(externalQuotation.Id));
            List<NreQuotationList> nreQuotationLists =
                await _NreQuotationList.GetAllListAsync(p => p.ExternalQuotationId.Equals(externalQuotation.Id));
            externalQuotationDto = ObjectMapper.Map<ExternalQuotationDto>(externalQuotation);
            externalQuotationDto.ProductQuotationListDtos = new List<ProductQuotationListDto>();
            externalQuotationDto.ProductQuotationListDtos =
                ObjectMapper.Map<List<ProductQuotationListDto>>(externalQuotationMxs);
            externalQuotationDto.NreQuotationListDtos = new List<NreQuotationListDto>();
            externalQuotationDto.NreQuotationListDtos =
                ObjectMapper.Map<List<NreQuotationListDto>>(nreQuotationLists);
        }
        else
        {
            //获取核价营销相关数据
            PriceEvaluationStartInputResult priceEvaluationStartInputResult =
                await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
            externalQuotationDto = new()
            {
                //CustomerName = priceEvaluationStartInputResult.CustomerName,
                ProjectName = priceEvaluationStartInputResult.ProjectName,
                SopTime = priceEvaluationStartInputResult.SopTime,
                ProjectCycle = priceEvaluationStartInputResult.ProjectCycle,
                SolutionId = solutionId,
                AuditFlowId = auditFlowId,
                CreationTime = DateTime.Now,
                NumberOfQuotations = externalQuotations.Count == 0
                    ? 1
                    : externalQuotations.Max(p => p.NumberOfQuotations) + 1
            };
            externalQuotationDto.ProductQuotationListDtos = productDtos.Select((a, index) =>
                new ProductQuotationListDto()
                {
                    SerialNumber = index + 1,
                    ProductName = a.ProductName,
                    Year = a.Year,
                    TravelVolume = a.Motion,
                    UnitPrice = decimal.Parse(a.UntilPrice)
                }).ToList();
            externalQuotationDto.NreQuotationListDtos = quotationNreDtos.Select((a, index) =>
                new NreQuotationListDto()
                {
                    SerialNumber = index + 1,
                    ProductName = a.Product,
                    HandmadePartsFee = a.shouban,
                    MyPropMoldCosterty = a.moju,
                    CostOfToolingAndFixtures = a.gzyj,
                    ExperimentalFees = a.sy,
                    RDExpenses = a.qt + a.cl + a.csrj + a.jianju + a.scsb,
                }).ToList();
            var now = DateTime.Now;
            var year1 = now.ToString("yy");
            var month = now.ToString("MM");

            var ppp = await _externalQuotation.GetAllListAsync();
            long i = ppp.Where(p => p.CreationTime.Year.ToString().EndsWith(year1)
                && p.CreationTime.Month.ToString().EndsWith(month)).Count();                 
            string year = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM");
            string iSttring = (i + 1).ToString("D4");
            externalQuotationDto.RecordNo = year + iSttring;
            externalQuotationDto.AccountName = "浙江舜宇智领技术有限公司";
            externalQuotationDto.DutyParagraph = "91330281MA2816W038";
            externalQuotationDto.BankOfDeposit = "中国农业银行余姚市环城支行";
            externalQuotationDto.AccountNumber = "39603001040014366";
            externalQuotationDto.Address = "浙江省余姚市阳明街道丰乐路67-69号";
        }

        return externalQuotationDto;
    }

    internal async Task SaveExternalQuotation(ExternalQuotationDto externalQuotationDto)
    {
        List<ExternalQuotation> externalQuotations =
            await _externalQuotation.GetAllListAsync(p => p.AuditFlowId.Equals(externalQuotationDto.AuditFlowId));

        if (externalQuotations.Count != 0 && externalQuotationDto.NumberOfQuotations == 0 &&
            externalQuotations.Max(p => p.NumberOfQuotations) + 1 < externalQuotationDto.NumberOfQuotations)
        {
            throw new FriendlyException($"version:{externalQuotationDto.NumberOfQuotations}版本号有误!");
        }

        if ((externalQuotations.Count == 0 && externalQuotationDto.NumberOfQuotations != 1) ||
            externalQuotationDto.NumberOfQuotations < 1)
        {
            throw new FriendlyException($"version:{externalQuotationDto.NumberOfQuotations}版本号有误!");
        }

        ExternalQuotation external = externalQuotations.FirstOrDefault(p =>
            p.SolutionId.Equals(externalQuotationDto.SolutionId) &&
            p.NumberOfQuotations.Equals(externalQuotationDto.NumberOfQuotations));
        //将报价单存入库中
        ExternalQuotation externalQuotation = ObjectMapper.Map<ExternalQuotation>(externalQuotationDto);
        if (external != null && external.NumberOfQuotations == externalQuotationDto.NumberOfQuotations &&
            external.IsSubmit)
        {
            throw new FriendlyException("已提交不可操作");
        }

        if (externalQuotation.NumberOfQuotations > 3)
        {
            throw new FriendlyException("报价已经超过三次,不可继续流转");
        }

        long i = await _externalQuotation.CountAsync(p =>
            p.AuditFlowId.Equals(externalQuotationDto.AuditFlowId) && p.IsSubmit &&
            p.NumberOfQuotations.Equals(externalQuotationDto.NumberOfQuotations));     
        ExternalQuotation prop = await _externalQuotation.BulkInsertOrUpdateAsync(externalQuotation);
        long id = prop.Id;
        await _externalQuotationMx.HardDeleteAsync(p => p.ExternalQuotationId.Equals(id));
        List<ProductExternalQuotationMx> productExternalQuotationMxes =
            ObjectMapper.Map<List<ProductExternalQuotationMx>>(externalQuotationDto.ProductQuotationListDtos);
        productExternalQuotationMxes.ForEach(p => p.ExternalQuotationId = id);
        await _externalQuotationMx.BulkInsertAsync(productExternalQuotationMxes);

        await _NreQuotationList.HardDeleteAsync(p => p.ExternalQuotationId.Equals(id));
        List<NreQuotationList> nreQuotationLists =
            ObjectMapper.Map<List<NreQuotationList>>(externalQuotationDto.NreQuotationListDtos);
        nreQuotationLists.ForEach(p => p.ExternalQuotationId = id);
        await _NreQuotationList.BulkInsertAsync(nreQuotationLists);

        List<SolutionQuotationDto> solutionQuotations = await GeCatalogue(externalQuotationDto.AuditFlowId);
        //流程流转
        if (solutionQuotations.Count == i + 1 && externalQuotationDto.IsSubmit)
        {
            //嵌入工作流
            await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
            {
                NodeInstanceId = externalQuotationDto.NodeInstanceId,
                FinanceDictionaryDetailId = externalQuotationDto.Opinion,
                Comment = externalQuotationDto.Comment,
            });
        }
    }

    /// <summary>
    ///  下载对外报价单-流
    /// </summary>
    /// <returns></returns>
    internal async Task<MemoryStream> DownloadExternalQuotationStream(long auditFlowId, long solutionId,
        long numberOfQuotations, bool ntype = false)
    {
        List<ProductDto> productDtos = await GetProductList(auditFlowId, solutionId, (int)numberOfQuotations, 1);//为了流测试成功，待明天改正
        List<QuotationNreDto> quotationNreDtos = await GetNREList(auditFlowId, solutionId, (int)numberOfQuotations, 1);//为了流测试成功，待明天改正
        ExternalQuotationDto external =
            await GetExternalQuotation(auditFlowId, solutionId, numberOfQuotations, productDtos, quotationNreDtos);
        if (ntype)
        {
            
            external.ProductQuotationListDtos = productDtos.Select((a, index) =>
               new ProductQuotationListDto()
               {
                   SerialNumber = index + 1,
                   ProductName = a.ProductName,
                   Year = a.Year,
                   TravelVolume = a.Motion,
                   UnitPrice = decimal.Parse(a.UntilPrice)
               }).ToList();
            external.NreQuotationListDtos = quotationNreDtos.Select((a, index) =>
                    new NreQuotationListDto()
                    {
                        SerialNumber = index + 1,
                        ProductName = a.Product,
                        HandmadePartsFee = a.shouban,
                        MyPropMoldCosterty = a.moju,
                        CostOfToolingAndFixtures = a.gzyj,
                        ExperimentalFees = a.sy,
                        RDExpenses = a.qt + a.cl + a.csrj + a.jianju + a.scsb,
                    }).ToList();
        }
        external.ProductQuotationListDtos = external.ProductQuotationListDtos.Select((p, index) =>
        {
            p.SerialNumber = index + 1;
            return p;
        }).ToList();
        external.NreQuotationListDtos = external.NreQuotationListDtos.Select((p, index) =>
        {
            p.SerialNumber = index + 1;
            return p;
        }).ToList();
        var memoryStream = new MemoryStream();

        await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/报价单下载.xlsx", external);

        return memoryStream;
    }

    /// <summary>
    ///  下载对外报价单
    /// </summary>
    /// <returns></returns>
    internal async Task<FileResult> DownloadExternalQuotation(long auditFlowId, long solutionId,
        long numberOfQuotations)
    {
        MemoryStream memoryStream = await DownloadExternalQuotationStream(auditFlowId, solutionId, numberOfQuotations);

        return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
        { FileDownloadName = "报价单下载.xlsx" };
    }

    /// <summary>
    /// 下载对外报价单
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public async Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(long processId, int version)
    {
        CoreComponentAndNreDto coreComponentAndNreDto = new();
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        //梯度
        var gradients = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == processId);
        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = ObjectMapper.Map<List<Solution>>(sol.SolutionListJson);

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
            "手板件费用", "模具费用", " 工装费用", "治具费用", "检具费用", "生产设备费用", "专用生产设备", "非专用生产设备", "实验费用", "测试软件费用", "差旅费",
            "其他费用",
            "合计"
        };
        List<NreExpense> nres = (from nrerr in nree
                                 select new NreExpense()
                                 {
                                     nre = nrerr,
                                     price = 100,
                                     remark = "12"
                                 }).ToList();
        coreComponentAndNreDto.nres = nres;
        coreComponentAndNreDto.ProductAndGradients = ProductAndGradients;
        return coreComponentAndNreDto;
    }


    public async Task<List<SoltionGradPrice>> GetSoltionGradPriceList(long auditFlowId, int version, int type)
    {
        var sps = await _actualUnitPriceOffer.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.ntype == type);
        sps = sps.OrderBy(p => p.gradient).ToList();
        var list = (from sp in sps
                    select new SoltionGradPrice()
                    {
                        UnitPrice = sp.OfferUnitPrice,
                        Gradientid = sp.GradientId,
                        SolutionId = sp.SolutionId
                    }).ToList();
        return list;
    }


    /// <summary>
    /// 用于对外报价产品清单
    /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    ///  <param name="version">0报价看板数据，1报价反馈数据</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<ProductDto>> GetProductList(long auditFlowId, long id, int ntime, int ntype)
    {
        var sos = await _solutionQutation.FirstOrDefaultAsync(p =>
            p.AuditFlowId == auditFlowId && p.Id == id);
        int version = sos.version;
        List<SoltionGradPrice> gsp = await GetSoltionGradPriceList(auditFlowId, version, ntype);
        Dictionary<long, List<SoltionGradPrice>> gsmap = gsp.GroupBy(p => p.Gradientid)
            .ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        var gradients = await getGradient(auditFlowId);

        var solutionList = JsonConvert.DeserializeObject<List<Solution>>(sos.SolutionListJson);


        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
        var modelcouts = priceEvaluationStartInputResult.ModelCount;
        List<ProductDto> productDtos = new List<ProductDto>();
        foreach (var modelcout in modelcouts)
        {
            var sol = solutionList.FirstOrDefault(p => p.ModuleName.Equals(modelcout.Product));
            if (sol is null)
            {
                continue;
            }

            var mcys = modelcout.ModelCountYearList;

            foreach (var mcy in mcys)
            {
                String key = mcy.Year + "";


                if (mcy.UpDown.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (mcy.UpDown.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (mcy.UpDown.Equals(YearType.Year))
                {
                    key += "年";
                }

                string UntilPrice = "";
                foreach (var gradient in gradients)
                {
                    UntilPrice = gsp.FirstOrDefault(p => p.SolutionId == sol.Id && p.Gradientid == gradient.Id)
                        .UnitPrice.ToString();
                    productDtos.Add(new ProductDto()
                    {
                        ProductName = sol.Product,
                        Motion = gradient.GradientValue,
                        Year = key,
                        UntilPrice = UntilPrice
                    });
                }
            }
        }

        productDtos = productDtos.OrderBy(p => p.Year).ToList();
        return productDtos;
    }

    /// <summary>
    /// 用于对外报价NRE清单
    ///<param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotationNreDto>> GetNREList(long auditFlowId, long id, int ntime, int ntype)
    {
        List<QuotationNreDto> productDtos = new List<QuotationNreDto>();
        var sos = await _solutionQutation.FirstOrDefaultAsync(p =>
            p.AuditFlowId == auditFlowId && p.Id == id);
        int version = sos.version;
        var nres = await _nreQuotation.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.SolutionId != null && p.ntype == ntype);
        var nresmap = nres.GroupBy(p => p.SolutionId).ToDictionary(r => r.Key, x => x.Select(e => e).ToList());
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
        var modelcouts = priceEvaluationStartInputResult.ModelCount;
        foreach (var nremap in nresmap)
        {
            long solutionid = nremap.Key.Value;
            var nresList = nremap.Value;
            var solution = _resourceSchemeTable.FirstOrDefault(p => p.Id == solutionid);
            var mcs = modelcouts.FirstOrDefault(p => p.Product.Equals(solution.ModuleName));

            productDtos.Add(new QuotationNreDto()
            {
                Product = solution.Product,
                Pcs = mcs is not null ? mcs.SumQuantity : 0,
                shouban = nresList.Where(p => p.FormName.Equals("手板件费")).Sum(p => p.OfferMoney),
                moju = nresList.Where(p => p.FormName.Equals("模具费")).Sum(p => p.OfferMoney),
                gzyj = nresList.Where(p => p.FormName.Equals("工装费") || p.FormName.Equals("治具费"))
                        .Sum(p => p.OfferMoney),
                sy = nresList.Where(p => p.FormName.Equals("实验费")).Sum(p => p.OfferMoney),
                csrj = nresList.Where(p => p.FormName.Equals("测试软件费")).Sum(p => p.OfferMoney),
                cl = nresList.Where(p => p.FormName.Equals("差旅费")).Sum(p => p.OfferMoney),
                qt = nresList.Where(p => p.FormName.Equals("其他费用")).Sum(p => p.OfferMoney),
                scsb = nresList.Where(p => p.FormName.Equals("生产设备费")).Sum(p => p.OfferMoney),
                jianju = nresList.Where(p => p.FormName.Equals("检具费")).Sum(p => p.OfferMoney)
            }
            );
        }

        return productDtos;
    }
}