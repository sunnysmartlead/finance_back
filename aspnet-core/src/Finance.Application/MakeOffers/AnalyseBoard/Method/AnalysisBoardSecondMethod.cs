﻿using System;
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
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Interface.Expends;
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
        IRepository<NreQuotationList, long> nreQuotationList)
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
        _exchangeRate= exchangeRate;
        _gradientRepository = gradientRepository;
        _deviceQuotation = deviceQuotation;
        _actualUnitPriceOffer = actualUnitPriceOffer;
        _sampleQuotation = sampleQuotation;
        _nreQuotation = nreQuotation;
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
    }

    public async Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new();
        var auditFlowId = analyseBoardSecondInputDto.auditFlowId;
        //获取方案
        List<Solution> Solutions = analyseBoardSecondInputDto.solutionTables;
        var solutiondict = Solutions.ToDictionary(p => p.ModuleName);


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
                        await getSample(sampleDtos, totalcost);
                    onlySampleDto.SolutionName = Solution.Product;
                    onlySampleDto.SolutionId = Solution.Id;
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
                    await getSample(sampleDtos, totalcost);
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
        var nsum = priceEvaluationStartInputResult.ModelCount.Sum(e => e.SumQuantity);
        List<GrossMarginModel> xssl = new List<GrossMarginModel>();
        List<GrossMarginModel> xscbs = new List<GrossMarginModel>();
        PooledAnalysisModel pooledAnalysisModelxscb = new(); ////销售销售成本
        pooledAnalysisModelxscb.ProjectName = "销售成本";

        PooledAnalysisModel pooledAnalysisModelpjcb = new();
        pooledAnalysisModelpjcb.ProjectName = "单位平均成本";
        List<GrossMarginModel> pjcbs = new List<GrossMarginModel>();

        PooledAnalysisModel pooledAnalysisModelflxssr = new();
        pooledAnalysisModelflxssr.ProjectName = "返利后销售收入";
        List<GrossMarginModel> fls = new();
        PooledAnalysisModel pooledAnalysisModelpjdj = new();
        pooledAnalysisModelpjdj.ProjectName = "平均单价";
        List<GrossMarginModel> pjdjs = new List<GrossMarginModel>();

        PooledAnalysisModel pooledAnalysisModelyj = new();
        pooledAnalysisModelpjdj.ProjectName = "佣金";
        List<GrossMarginModel> yjs = new List<GrossMarginModel>();
        List<GrossMarginModel> yjss = new List<GrossMarginModel>();


        PooledAnalysisModel pooledAnalysisModelxsml = new();
        pooledAnalysisModelxsml.ProjectName = "销售毛利";
        List<GrossMarginModel> xsmls = new List<GrossMarginModel>();
        PooledAnalysisModel pooledAnalysisModelmll = new();
        pooledAnalysisModelmll.ProjectName = "毛利率";
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

                long gradientid = 0;
                foreach (var gradient in gradients)
                {
                    if (model.Quantity <= gradient.GradientValue)
                    {
                        gradientid = gradient.Id; //获取对应梯度id
                        break;
                    }
                }

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
                    long gradientid = 0;
                    foreach (var gradient in gradients)
                    {
                        if (mc.Quantity <= gradient.GradientValue)
                        {
                            gradientid = gradient.Id; //获取对应梯度id
                            break;
                        }
                    }

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


                CreateCustomerTargetPriceDto ctp = priceEvaluationStartInputResult.CustomerTargetPrice.FindFirst(p =>
                    p.Kv == gradient.GradientValue && p.Product.Equals(solution.ModuleName));
                var mbj = (Convert.ToDecimal(ctp.TargetPrice)) * ctp.ExchangeRate;
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
                    gradient = gradient.GradientValue + "k/y",
                    SolutionId = solution.Id,
                    product = solution.ModuleName,
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


                gradientQuotedGrossMarginModels.Add(model);
            }


            ProjectBoardSecondModel sl = new();
            sl.ProjectName = "数量";
            sl.InteriorTarget = mbnbsl;
            sl.ClientTarget = mbkhsl;

            projectBoardSecondModels.Add(sl);

            ProjectBoardSecondModel xscb = new();
            xscb.ProjectName = "销售成本";
            xscb.InteriorTarget = mbnbxscb;
            xscb.ClientTarget = mbkhxscb;

            projectBoardSecondModels.Add(xscb);

            ProjectBoardSecondModel dwpjcb = new();
            dwpjcb.ProjectName = "单位平均成本";
            dwpjcb.InteriorTarget = Math.Round(mbnbxscb / mbnbsl, 2);
            dwpjcb.ClientTarget = Math.Round(mbkhxscb / mbkhsl, 2);

            projectBoardSecondModels.Add(dwpjcb);

            ProjectBoardSecondModel xssr = new();
            xssr.ProjectName = "销售收入";
            xssr.InteriorTarget = Math.Round(mbnbxssr, 2);
            xssr.ClientTarget = Math.Round(mbkhxssr, 2);

            projectBoardSecondModels.Add(xssr);
            ProjectBoardSecondModel yj = new();
            yj.ProjectName = "佣金";
            yj.InteriorTarget = mbnbyj;
            yj.ClientTarget = mbkhyj;

            projectBoardSecondModels.Add(yj);
            ProjectBoardSecondModel pjdj = new();
            pjdj.ProjectName = "平均单价";
            pjdj.InteriorTarget = Math.Round(mbnbxssr / mbnbsl, 2);
            pjdj.ClientTarget = Math.Round(mbkhxssr / mbkhsl, 2);

            projectBoardSecondModels.Add(pjdj);

            ProjectBoardSecondModel xsml = new();
            xsml.ProjectName = "销售毛利";
            xsml.InteriorTarget = Math.Round(mbnbml, 2);
            xsml.ClientTarget = Math.Round(mbkhml, 2);

            projectBoardSecondModels.Add(xsml);
            ProjectBoardSecondModel mll = new();
            mll.ProjectName = "毛利率";
            mll.InteriorTarget = Math.Round((mbnbml / mbnbxssr) * 100, 2);
            mll.ClientTarget = Math.Round((mbkhml / mbkhxssr) * 100, 2);

            projectBoardSecondModels.Add(mll);
            boardModel.ProjectBoardModels = projectBoardSecondModels;
            boardModel.GradientId = gradient.Id;
            boardModel.title = gradient.GradientValue + "KV";
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
            foreach (var solution in Solutions)
            {
                var carnum = createCarModelCountDtos
                    .FindFirst(p => p.CarModel.Equals(key) && p.Product.Equals(solution.ModuleName))
                    .SingleCarProductsQuantity;

                var modelCountDto = modelcoutnlist.FindFirst(p => p.Product.Equals(solution.ModuleName)); //获取第一年的数量
                var modelCount = modelCountDto.ModelCountYearList.OrderBy(p => p.Year).ThenBy(p => p.UpDown).First();
                long gradientid = 0;
                foreach (var gradient in gradients)
                {
                    if (modelCount.Quantity < gradient.GradientValue)
                    {
                        gradientid = gradient.Id;
                        break;
                    }
                }

                var gradq = gradientQuotedGrossMarginModels.FindFirst(p =>
                    p.GradientId == gradientid && p.product.Equals(solution.ModuleName)); //第一年的单价

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
                    product = solution.ModuleName,
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

                var crm = createCarModelCountDtos.FindFirst(p => p.Product.Equals(solution.ModuleName));
                quotedGrossMarginActual.carNum = crm.SingleCarProductsQuantity;

                QuotedGrossMarginActualList.Add(quotedGrossMarginActual);
            }


            QuotedGrossMarginActual quotedGrossMarginActualqt = new QuotedGrossMarginActual()
            {
                carModel = key,
                AuditFlowId = auditFlowId,
                product = "齐套",
                InteriorPrice = qtmbnbdj,
                InteriorGrossMargin = (qtnbml / qtnbsr) * 100,
                InteriorNreGrossMargin = (qtnbftml / qtnbftss) * 100,
                InteriorClientGrossMargin = (qtnbkgml / qtnbkgsr) * 100,
                ClientPrice = qtmbkhdj,
                ClientGrossMargin = (qtkhml / qtkhsr) * 100,
                ClientClientGrossMargin = (qtkhkgml / qtkhkgsr) * 100,
                ClientNreGrossMargin = (qtkhftml / qtkhftss)
            };

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
            sjkhcb += khj.xscb;
            sjkhml += khj.xsml;
            QuotedGrossMarginActual quotedGrossMarginActualS = new()
            {
                product = solution.ModuleName,
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
        sjsl.InteriorTarget = sjnbsl;
        sjsl.ClientTarget = sjkhsl;

        projectBoardSecondModelssj.Add(sjsl);

        ProjectBoardSecondModel sjxscb = new();
        sjxscb.ProjectName = "销售成本";
        sjxscb.InteriorTarget = sjnbcb;
        sjxscb.ClientTarget = sjkhcb;

        projectBoardSecondModelssj.Add(sjxscb);

        ProjectBoardSecondModel sjdwpjcb = new();
        sjdwpjcb.ProjectName = "单位平均成本";
        sjdwpjcb.InteriorTarget = Math.Round(sjnbcb / sjnbsl, 2);
        sjdwpjcb.ClientTarget = Math.Round(sjkhcb / sjkhsl, 2);

        projectBoardSecondModelssj.Add(sjdwpjcb);

        ProjectBoardSecondModel sjxssr = new();
        sjxssr.ProjectName = "销售收入";
        sjxssr.InteriorTarget = Math.Round(sjnbxssr, 2);
        sjxssr.ClientTarget = Math.Round(sjkhxssr, 2);

        projectBoardSecondModelssj.Add(sjxssr);
        /*ProjectBoardSecondModel sjyj = new();
        sjyj.ProjectName = "佣金";
       // sjyj.InteriorTarget = mbnbyj;
       // sjyj.ClientTarget = mbkhyj;

        projectBoardSecondModelssj.Add(sjyj);*/
        ProjectBoardSecondModel sjpjdj = new();
        sjpjdj.ProjectName = "平均单价";
        sjpjdj.InteriorTarget = Math.Round(sjnbxssr / sjnbsl, 2);
        sjpjdj.ClientTarget = Math.Round(sjkhxssr / sjkhsl, 2);

        projectBoardSecondModelssj.Add(sjpjdj);

        ProjectBoardSecondModel sjxsml = new();
        sjxsml.ProjectName = "销售毛利";
        sjxsml.InteriorTarget = Math.Round(sjnbml, 2);
        sjxsml.ClientTarget = Math.Round(sjkhml, 2);

        projectBoardSecondModelssj.Add(sjxsml);
        ProjectBoardSecondModel sjmll = new();
        sjmll.ProjectName = "毛利率";
        sjmll.InteriorTarget = Math.Round((sjnbml / sjnbxssr) * 100, 2);
        sjmll.ClientTarget = Math.Round((sjkhml / sjkhxssr) * 100, 2);

        projectBoardSecondModelssj.Add(sjmll);
        sj.ProjectBoardModels = projectBoardSecondModelssj;
        boardModels.Add(sj);
        analyseBoardSecondDto.ProjectBoard = boardModels;

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
                AuditFlowId = AuditFlowId, GradientId = gradientId, InputCount = 0, SolutionId = solutionid,
                Year = crm.Year, UpDown = crm.UpDown
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
            kgSalesRevenue.Add(rev);
            //客供佣金（千元）
            YearValue kgcom = new();
            kgcom.value = kgup * gradient.GradientValue *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            kgcommission.Add(com);
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
        grossMarginSecondDto.GrossMargin = Math.Round((xsml / totalsale) * 100, 2);
        grossMarginSecondDto.ClientGrossMargin = Math.Round((kgxsml / kgtotalsale) * 100, 2);
        grossMarginSecondDto.NreGrossMargin = Math.Round((ftxsml / fttotalsale) * 100, 2);
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
            GrossMarginSecondDto grossmarin = new();
            if (carModel is not null)
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
            GrossMargin = (qtml / qtsr) * 100,
            ClientGrossMargin = (qtkgml / qtkgsr) * 100,
            NreGrossMargin = (qtftml / qtftss) * 100
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
            if (carModel is not null)
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
            long grad = 0;
            foreach (var gradient in gradients)
            {
                if (qu < gradient.GradientValue)
                {
                    grad = gradient.Id;
                    break;
                }
            }

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
                AuditFlowId = AuditFlowId, GradientId = grad, InputCount = 0, SolutionId = solutionid,
                Year = crm.Year, UpDown = crm.UpDown
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
            kgSalesRevenue.Add(rev);
            //客供佣金（千元）
            YearValue kgcom = new();
            kgcom.value = kgup * num.value *
                          (crm.CommissionRate / 100); //单价*数量*年度佣金比例
            kgcommission.Add(com);
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
        grossMarginSecondDto.GrossMargin = Math.Round((xsml / totalsale) * 100, 2);
        grossMarginSecondDto.ClientGrossMargin = Math.Round((kgxsml / kgtotalsale) * 100, 2);
        grossMarginSecondDto.kgxsml = kgxsml;
        grossMarginSecondDto.kgxssr = kgtotalsale;

        grossMarginSecondDto.NreGrossMargin = Math.Round((ftxsml / fttotalsale) * 100, 2);
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
            if (carModel is not null)
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

                Pricsenew=    (from price in Pricsenew
                    select new YearValue()
                    {
                        key = price.key,
                        value = price.value * carnum
                    }).ToList();
                Prices = Pricsenew;
                
                AverageCostnew=    (from price in AverageCostnew
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
                Pricsenew=    (from price in Pricsenew
                    select new YearValue()
                    {
                        key = price.key,
                        value = price.value * carnum
                    }).ToList();
                Prices = hebing(Prices, Pricsenew);
                AverageCostnew=    (from price in AverageCostnew
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
            gross.value = (smdict[SalesRevenues.key].value / SalesRevenues.value) * 100; //销售毛利/销售收入
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
            if (carModel is not null)
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
            long grad = 0;
            foreach (var gradient in gradients)
            {
                if (qu < gradient.GradientValue)
                {
                    grad = gradient.Id;
                    break;
                }
            }

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
                AuditFlowId = AuditFlowId, GradientId = grad, InputCount = 0, SolutionId = solutionid,
                Year = crm.Year, UpDown = crm.UpDown
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
            gross.value = (mar.value / rev.value) * 100; //销售毛利/销售收入
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
            gross.value = (mar.value / rev.value) * 100; //销售毛利/销售收入
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

    public async Task<AnalyseBoardSecondDto> getStatementAnalysisBoardSecond(long auditFlowId, int version)
    {
        List<SolutionQuotation> solutionQuotations =
            await _solutionQutation.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (solutionQuotations is null)
        {
            throw new UserFriendlyException("请选择报价方案");
        }

        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        List<AnalyseBoardNreDto> nres = await getNreForData(auditFlowId, version);
        List<OnlySampleDto> sampleDtos = await getSampleForData(auditFlowId, solutionQuotations);
        List<SopAnalysisModel> sops = await getSopForData(auditFlowId, version);
        List<PooledAnalysisModel> pools = await getPoolForData(auditFlowId, version);
        List<GradientQuotedGrossMarginModel> gradis = await getGradientQuotedGrossMargin(auditFlowId, version);
        List<QuotedGrossMarginProjectModel> quotedGrossMarginProjectModels =
            await getQuotedGrossMarginProject(auditFlowId, version);

        List<BoardModel> boards = await getboardForData(auditFlowId, version);

        analyseBoardSecondDto.nres = nres;
        analyseBoardSecondDto.SampleOffer = sampleDtos;
        analyseBoardSecondDto.Sops = sops;
        analyseBoardSecondDto.FullLifeCycle = pools;
        //analyseBoardSecondDto.QuotedGrossMargins = quotedGrossMarginProjectModels;
        // analyseBoardSecondDto.GradientQuotedGrossMargins = gradis;
        analyseBoardSecondDto.ProjectBoard = boards;
        return analyseBoardSecondDto;
    }

    public async Task PostIsOfferSaveSecond(IsOfferSecondDto isOfferDto)
    {
        List<Solution> solutions = isOfferDto.Solutions;
        int version = isOfferDto.version;
        var AuditFlowId = isOfferDto.AuditFlowId;
        var count = _solutionQutation.GetAllList(p => p.AuditFlowId == AuditFlowId).Count; //报价提交次数
        int time = 0;
        if (count == 0)
        {
            time = 1;
        }
        else
        {
            time = _solutionQutation.GetAllListAsync(p => p.AuditFlowId == AuditFlowId).Result.Max(p => p.ntime);
            if (time >= 3)
            {
                throw new UserFriendlyException("本流程报价提交次数已经到顶");
            }
        }

        await InsertSolution(solutions, version, time);

        List<AnalyseBoardNreDto> nres = isOfferDto.nres;
        InsertNre(nres, solutions);
        List<OnlySampleDto> onlySampleDtos = isOfferDto.SampleOffer;
        InsertSample(onlySampleDtos, solutions);

        List<SopAnalysisModel> sops = isOfferDto.Sops;
        InsertSop(sops, version);
        List<PooledAnalysisModel> FullLifeCycle = isOfferDto.FullLifeCycle;
        InsertPool(AuditFlowId, FullLifeCycle, version);

        List<BoardModel> projectBoardModels = isOfferDto.ProjectBoard;
        Insertboard(AuditFlowId, projectBoardModels, version);
        List<GradientGrossMarginCalculateModel> jts = isOfferDto.GradientQuotedGrossMargins;
        InsertGradientQuotedGrossMargin(AuditFlowId, jts, version);
        List<QuotedGrossMarginActualModel> models = isOfferDto.QuotedGrossMargins;
        InsertQuotedGrossMarginProject(AuditFlowId, models, version);
    }

    /// <summary>
    /// 报价毛利率测算 实际数量  从数据查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotedGrossMarginProjectModel>> getQuotedGrossMarginProject(long AuditFlowId, int version)
    {
        List<QuotedGrossMarginProjectModel> list = new();
        List<DynamicUnitPriceOffers> dynamicUnitPriceOffers = _dynamicUnitPriceOffers.GetAll()
            .Where(p => p.AuditFlowId == AuditFlowId && p.version == version).ToList();
        var dymap = from dy in dynamicUnitPriceOffers group dy by dy.CarModel;

        foreach (var d in dymap)
        {
            var carmodel = d.Key;
            QuotedGrossMarginProjectModel quotedGrossMarginProjectModel = new();
            quotedGrossMarginProjectModel.project = carmodel;
            List<GrossMargin> GrossMargins = new();
            foreach (var ds in d)
            {
                GrossMargin gross = new GrossMargin()
                {
                    version = ds.version,
                    Id = ds.Id,
                    product = ds.ProductName,
                    AuditFlowId = ds.AuditFlowId,
                    ProductNumber = ds.ProductNumber,
                    quotedGrossMarginSimple = new QuotedGrossMarginSimple()
                    {
                        Interior = new TargetPrice()
                        {
                            Price = ds.InteriorTargetUnitPrice,
                            GrossMargin = ds.AllInteriorGrossMargin,
                            ClientGrossMargin = ds.AllInteriorClientGrossMargin,
                            NreGrossMargin = ds.AllInteriorNreGrossMargin
                        },
                        Client = new TargetPrice()
                        {
                            Price = ds.ClientTargetUnitPrice,
                            GrossMargin = ds.AllClientGrossMargin,
                            ClientGrossMargin = ds.AllClientClientGrossMargin,
                            NreGrossMargin = ds.AllClientNreGrossMargin
                        },

                        ThisQuotation = new TargetPrice()
                        {
                            Price = ds.OfferUnitPrice,
                            GrossMargin = ds.OffeGrossMargin,
                            ClientGrossMargin = ds.ClientGrossMargin,
                            NreGrossMargin = ds.NreGrossMargin
                        }
                    }
                };
                GrossMargins.Add(gross);
            }


            quotedGrossMarginProjectModel.GrossMargins = GrossMargins;
            list.Add(quotedGrossMarginProjectModel);
        }

        return list;
    }

    /// <summary>
    /// 报价毛利率测算 实际数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertQuotedGrossMarginProject(long AuditFlowId,
        List<QuotedGrossMarginActualModel> grossMarginProjectModels, int version)
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
                    Id = gross.Id,
                    title = project,
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
            _dynamicUnitPriceOffers.InsertOrUpdateAsync(dynamicUnitPriceOffers);
        }
    }

    /// <summary>
    /// 报价毛利率测算 实际数量  从数据库查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<GradientQuotedGrossMarginModel>> getGradientQuotedGrossMargin(long AuditFlowId, int version)
    {
        List<GradientQuotedGrossMarginModel> gradientQuotedGross = new List<GradientQuotedGrossMarginModel>();
        /*select new GradientQuotedGrossMarginModel()
        {
            AuditFlowId = act.AuditFlowId,
            version = act.version,
            gradient = act.Kv,
            Id = act.Id,
            QuotedGrossMarginSimple = new QuotedGrossMarginSimple()
            {
                Interior = JsonConvert.DeserializeObject<TargetPrice>(act.InteriorTarget),
                Client = JsonConvert.DeserializeObject<TargetPrice>(act.ClientTarget),
                ThisQuotation = JsonConvert.DeserializeObject<TargetPrice>(act.Offer)
            }
        }).ToList();*/
        return gradientQuotedGross;
    }

    /// <summary>
    /// 报价毛利率测算阶梯数量  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertGradientQuotedGrossMargin(long AuditFlowId,
        List<GradientGrossMarginCalculateModel> gradientQuotedGross, int version)
    {
        var list = ObjectMapper.Map<List<GradientGrossCalculate>>(gradientQuotedGross);


        foreach (var actual in list)
        {
            _actualUnitPriceOffer.InsertAsync(actual);
        }
    }

    /// <summary>
    /// 看板保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task Insertboard(long AuditFlowId, List<BoardModel> boards, int version)
    {
        foreach (var bord in boards)
        {
            var projects = bord.ProjectBoardModels;
            var projectbords = (from board in projects
                select new ProjectBoardSecondOffers()
                {
                    AuditFlowId = AuditFlowId,
                    ProjectName = board.ProjectName,
                    version = version,
                    GradientId =board.GradientId,
                    InteriorTarget = board.InteriorTarget,
                    ClientTarget = board.ClientTarget,
                    Offer = board.Offer
                }).ToList();

            foreach (var projectbord in projectbords)
            {
                await _resourceProjectBoardSecondOffers.InsertOrUpdateAsync(projectbord);
            }
        }
    }

    /// <summary>
    /// 看板从数据库查询
    /// </summary>
    /// <returns></returns>
    public async Task<List<BoardModel>> getboardForData(long AuditFlowId, int version)
    {
        var boards = _resourceProjectBoardSecondOffers.GetAll()
            .Where(p => p.AuditFlowId == AuditFlowId && p.version == version).ToList();

        List<BoardModel> boardModels = new();

        var boardmap = from board in boards group board by board.title;
        foreach (var board in boardmap)
        {
            BoardModel boardModel = new();
            boardModel.title = board.Key;
            List<ProjectBoardSecondModel> ps = new();
            foreach (var p in board)
            {
                ProjectBoardSecondModel projectBoardModel = new ProjectBoardSecondModel()
                {
                    AuditFlowId = p.AuditFlowId,
                    ProjectName = p.ProjectName,
                    version = version,
                    Id = p.Id,
                    InteriorTarget = p.InteriorTarget,
                    ClientTarget = p.ClientTarget,
                    Offer = p.Offer
                };
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
    public async Task InsertPool(long AuditFlowId, List<PooledAnalysisModel> pooledAnalysisModels, int version)
    {
        var polls = (from pool in pooledAnalysisModels
            select new PooledAnalysisOffers()
            {
                Id = pool.Id,
                AuditFlowId = AuditFlowId,
                ProjectName = pool.ProjectName,
                version = version,
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
    public async Task<List<PooledAnalysisModel>> getPoolForData(long auditFlowId, int version)
    {
        List<PooledAnalysisOffers> pools = _resourcePooledAnalysisOffers.GetAll()
            .Where(p => p.AuditFlowId == auditFlowId && p.version == version)
            .ToList();
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
    public async Task<List<SopAnalysisModel>> getSopForData(long auditFlowId, int version)
    {
        List<UnitPriceOffers> ups = _resourceUnitPriceOffers.GetAll()
            .Where(p => p.AuditFlowId == auditFlowId && p.version == version).ToList();

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
    public async Task InsertSop(List<SopAnalysisModel> unitPriceModels, int version)
    {
        var unitPriceOffersList = (from unit in unitPriceModels
            select new UnitPriceOffers()
            {
                Id = unit.Id,
                AuditFlowId = unit.AuditFlowId,
                ProductName = unit.Product,
                GradientId = unit.GradientId,
                GradientValue = unit.GradientValue,
                version = version,
                GrossMarginList = JsonConvert.SerializeObject(unit.GrossValues)
            }).ToList();
        foreach (var uns in unitPriceOffersList)
        {
            _resourceUnitPriceOffers.InsertOrUpdateAsync(uns);
        }
    }

    /// <summary>
    /// 样品 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSample(List<OnlySampleDto> onlySampleDtos, List<Solution> solutionQuotations)
    {
        foreach (var onlySampleDto in onlySampleDtos)
        {
            var soltuion = solutionQuotations.Where(p => p.Id == onlySampleDto.SolutionId).FirstOrDefault();
            List<SampleQuotation> sampleQuotations = onlySampleDto.OnlySampleModels;

            foreach (var sampleQuotation in sampleQuotations)
            {
                sampleQuotation.SolutionId = soltuion.Id;
                _sampleQuotation.InsertOrUpdateAsync(sampleQuotation);
            }
        }
    }

    /// <summary>
    /// 获取样品阶段从数据库
    /// </summary>
    /// <returns></returns>
    public async Task<List<OnlySampleDto>> getSampleForData(long AuditFlowId,
        List<SolutionQuotation> solutionQuotations)
    {
        List<OnlySampleDto> onlySampleDtos = new();
        //样品阶段存的solutionQuotation.Id
        foreach (var solutionQuotation in solutionQuotations)
        {
            OnlySampleDto dto = new();
            dto.SolutionName = solutionQuotation.SolutionName;
            dto.AuditFlowId = AuditFlowId;
            List<SampleQuotation> sampleQuotations = _sampleQuotation.GetAll()
                .Where(p => p.AuditFlowId == AuditFlowId && p.SolutionId == solutionQuotation.Id)
                .ToList();
            dto.OnlySampleModels = sampleQuotations;


            onlySampleDtos.Add(dto);
        }

        return onlySampleDtos;
    }

    /// <summary>
    /// 报价方案 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertSolution(List<Solution> solutions, int version, int time)
    {
        SolutionQuotation solutionQuotation = new()
        {
            SolutionListJson = JsonConvert.SerializeObject(solutions),
            ntime = time,
            version = version
        };
        _solutionQutation.InsertAsync(solutionQuotation);
    }

    /// <summary>
    /// NRE 保存  接口
    /// </summary>
    /// <returns></returns>
    public async Task InsertNre(List<AnalyseBoardNreDto> nres, List<Solution> solutionQuotations)
    {
        foreach (var nre in nres)
        {
            var solutionId = nre.SolutionId;
            if (solutionId is not null)
            {
                var soltuion = _resourceSchemeTable.FirstOrDefault(p => p.Id == nre.SolutionId);
                List<NreQuotation> nreQuotations = nre.models;
                List<DeviceQuotation> deviceQuotations = nre.devices;
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
        List<CreateCarModelCountYearDto> yearDtos = (from car in cars
                                                     from carModelCountYear in car.ModelCountYearList
                                                     select carModelCountYear).ToList();
        List<int> YearList = yearDtos.Select(e => e.Year).Distinct().ToList();
        var yearmap = yearDtos.GroupBy(e => e.Year).ToDictionary(e => e.Key, e => e.Sum(v => v.Quantity));
        // 拿到产品信息
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
                SolutionName = "核心部件: " + solution.SolutionName,
                PartsName = "核心部件",
                Model = "型号",
                Type = "类型",
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
            // var ips = eles.Where(e => e.TypeName.Equals("芯片IC—ISP")).FirstOrDefault();
            var ips = eles.Where(e => e.TypeName.Equals("芯片IC——ISP")).FirstOrDefault();
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
        shouban.Cost = shoubans.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(shouban);
        muju.Costs = mujus;
        muju.Cost = mujus.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(muju);
        scsb.Costs = scsbs;
        scsb.Cost = scsbs.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(scsb);
        gz.Costs = gzs;
        gz.Cost = gzs.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(gz);
        yj.Costs = yjs;
        yj.Cost = yjs.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(yj);
        sy.Costs = sys;
        sy.Cost = sys.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(sy);
        cs.Costs = css;
        cs.Cost = css.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(cs);
        cl.Costs = cls;
        cl.Cost = cls.Sum(p => System.Convert.ToDecimal(p));
        nres.Add(cl);
        qt.Costs = qts;
        qt.Cost = qts.Sum(p => System.Convert.ToDecimal(p));
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
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id
                };
                List<Material> malist = await _priceEvaluationAppService.GetBomCost(getBomCostInput);
                var sopbom = malist.Where(p => p.Year == new decimal(soptime)).Sum(p => p.TotalMoneyCyn);
                var quanbom = malist.Sum(p => p.TotalMoneyCyn);
                boms.Add(sopbom.ToString());
                boms.Add(quanbom.ToString());
                GetManufacturingCostInput manufacturingCostInput = new GetManufacturingCostInput()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    Year = soptime,
                    UpDown = YearType.Year
                };
                List<ManufacturingCost> sops =
                    await _priceEvaluationAppService.GetManufacturingCost(manufacturingCostInput);
                var zzsop = sops.Sum(p => p.Subtotal);
                GetManufacturingCostInput quansccb = new GetManufacturingCostInput()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id
                };
                List<ManufacturingCost> quansccbs =
                    await _priceEvaluationAppService.GetManufacturingCost(quansccb);
                var quancb = quansccbs.Sum(p => p.Subtotal);
                sccbs.Add(zzsop.ToString());
                sccbs.Add(quancb.ToString());


                GetCostItemInput getCostItemInput = new GetCostItemInput()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    Year = soptime,
                    UpDown = YearType.Year
                };
                List<LossCost> soplosss = await _priceEvaluationAppService.GetLossCost(getCostItemInput);
                var losssop = soplosss.Sum(p => p.WastageCost);
                var moqftcbsssop = soplosss.Sum(p => p.MoqShareCount);
                GetCostItemInput quanianloss = new GetCostItemInput()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id
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
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id
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
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    Year = soptime
                };

                QualityCostListDto sopqu = await _priceEvaluationAppService.GetQualityCost(getOtherCostItemInput);
                var sooqu = sopqu.QualityCost;
                var quanqu = 0;
                zlcbs.Add(sooqu.ToString());
                zlcbs.Add(quanqu.ToString());
                GetOtherCostItemInput qtcb = new GetOtherCostItemInput()
                {
                    AuditFlowId = auditFlowId,
                    GradientId = gradient.Id,
                    SolutionId = solution.Id,
                    Year = soptime
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


        // var value = new
        // {
        //     Date = DateTime.Now.ToString("yyyy-MM-dd"), //日期
        //     RecordNumber = priceEvaluationStartInputResult.Number, //记录编号           
        //     Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
        //     DirectCustomerName = priceEvaluationStartInputResult.CustomerName, //直接客户名称
        //     TerminalCustomerName = priceEvaluationStartInputResult.TerminalName, //终端客户名称
        //     OfferForm = priceEvaluationStartInputResult.PriceEvalType, //报价形式
        //     SopTime = priceEvaluationStartInputResult.SopTime, //SOP时间
        //     ProjectCycle = priceEvaluationStartInputResult.ProjectCycle, //项目生命周期
        //     ForSale = priceEvaluationStartInputResult.SalesType, //销售类型
        //     modeOfTrade = priceEvaluationStartInputResult.TradeMode, //贸易方式
        //     PaymentMethod = priceEvaluationStartInputResult.PaymentMethod, //付款方式
        //     //ExchangeRate = priceEvaluationStartInputResult.ExchangeRate, //汇率???
        //     Sop = Sop,
        //     ProjectName = priceEvaluationStartInputResult.ProjectName, //项目名称
        //     Parts = partsModels,
        //     NRE = nres,
        //     Cost = pricingModels,
        // };
        //用MiniExcel读取数据

        try
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
            {
                { "日期", DateTime.Now.ToString("yyyy-MM-dd") },
                { "记录编号", priceEvaluationStartInputResult.Number },
                { "版本", priceEvaluationStartInputResult.QuoteVersion },
                { "直接客户名称", priceEvaluationStartInputResult.CustomerName },
                { "终端客户名称", priceEvaluationStartInputResult.TerminalName },
                { "报价形式", priceEvaluationStartInputResult.PriceEvalType },
                { "SOP时间", priceEvaluationStartInputResult.SopTime },
                { "项目生命周期", priceEvaluationStartInputResult.ProjectCycle },
                { "销售类型", priceEvaluationStartInputResult.SalesType },
                { "贸易方式", priceEvaluationStartInputResult.TradeMode },
                { "付款方式", priceEvaluationStartInputResult.PaymentMethod },
                { "项目名称", priceEvaluationStartInputResult.ProjectName },
            };
            var values = new List<Dictionary<string, object>>();
            var sheets = new Dictionary<string, object>
            {
                ["Sop"] = Sop,
                ["Parts"] = partsModels,
                ["NRE"] = nres,
                ["Cost"] = pricingModels,
            };
            // values.Add(keyValuePairs);
            values.Add(sheets);
            MemoryStream memoryStream = new MemoryStream();
            // return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            await MiniExcel.SaveAsAsync(memoryStream, sheets);
            return new FileContentResult(memoryStream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{fileName}.xlsx"
            };
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
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
            .Where(p => p.FinanceDictionaryId.Equals("SampleName")).ToList();
        //样品

        List<SampleQuotation> onlySampleModels = (from sample in samples
            join dic in dics on sample.Name equals (dic.Id)
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
    public async Task<List<AnalyseBoardNreDto>> getNreForData(long processId, int version)
    {
        List<SolutionQuotation> solutionQuotations =
            _solutionQutation.GetAll().Where(p => p.AuditFlowId == processId && p.version == version).ToList();
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
            var solutionName = solutionTable.Product;

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
        var solts = (from sol in sols
            select new SolutionQuotationDto()
            {
                version = sol.version,
                ntime = sol.ntime,
                AuditFlowId = auditFlowId,
                solutionList = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson)
            }).ToList();
        return solts;
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
    public async Task<QuotationListSecondDto> QuotationListSecond(long processId,int version )
    {
       var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
            List<Solution> solutions = ObjectMapper.Map<List<Solution>>(sol.SolutionListJson);
            
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);
       var customprice= priceEvaluationStartInputResult.CustomerTargetPrice;
       string bz = "";
       decimal hl= 0;
        if (customprice is not null)
        {
            hl=  customprice[0].ExchangeRate;
            bz=     _exchangeRate.FirstOrDefault(p => p.Id == customprice[0].Currency).ExchangeRateKind;
        }
        QuotationListSecondDto pp = new QuotationListSecondDto
        {
            Date = DateTime.Now, //查询日期
            RecordNumber = priceEvaluationStartInputResult.Number, // 单据编号
            Versions = priceEvaluationStartInputResult.QuoteVersion, //版本
            SampleQuotationType="",//样品报价类型
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
              ExchangeRate =hl, //汇率
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


     var crms=  priceEvaluationStartInputResult.Requirement;
     MotionMessageModel njl = new MotionMessageModel()
     {
         MessageName = "年降率(%)"
     };
     List<SopOrValueMode> njls = new();
     MotionMessageModel flyq = new MotionMessageModel()
     {
         MessageName = "年度返利要求(%)"
     };
     List<SopOrValueMode> flyqs = new();
     MotionMessageModel zrl = new MotionMessageModel()
     {
         MessageName = "一次性折让率(%)"
     };
     List<SopOrValueMode> zrls = new();
     MotionMessageModel yjbl = new MotionMessageModel()
     {
         MessageName = "佣金比例"
     };
     List<SopOrValueMode> yjbls = new();
     foreach (var crm in crms)
     {
         njls.Add(new SopOrValueMode()
         {
             
         } );
         
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
                AuditFlowId = processId,
                GradientId = mintd.Id,
                InputCount = 0,
                SolutionId = Solution.Id,
                Year = soptime,
                UpDown = YearType.FirstHalf
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
                    GradientId = gradient.Id,
                    Product = solution.Product,
                    gradient = gradient.GradientValue + "K/Y",
                    SopCost = 1,
                    FullLifeCyclecost = 2,
                    Price = 1,
                    SopGrossMargin = 23,
                    TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23,
                    NreGrossMargin = 10
                };
                BiddingStrategySecondModels.Add(biddingStrategySecondModel);
            }
        }


        pp.BiddingStrategySecondModels = BiddingStrategySecondModels;
        return pp;
    }

    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long processId,int version)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto = new();
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = ObjectMapper.Map<List<Solution>>(sol.SolutionListJson);

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
                solutionName = solution.SolutionName,
                SolutionId = solution.Id
            };
            List<ManagerApprovalOfferModel> managerApprovalOfferModels = new List<ManagerApprovalOfferModel>();
            foreach (var product in products)
            {
                ManagerApprovalOfferModel model = new ManagerApprovalOfferModel()
                {
                    Product = product,
                    SopCost = 4,
                    FullLifeCyclecost = 5,
                    SalesRevenue = 5,
                    SellingCost = 6,
                    GrossMargin = 12,
                    Price = 10,
                    Commission = 1,
                    SopGrossMargin = 11,
                    GrossMarginCommission = 11,
                    ClientGrossMargin = 12,
                    NreGrossMargin = 10
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

    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long processId,int version)
    {
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(processId);

        var sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == processId && p.version == version);
        List<Solution> solutions = ObjectMapper.Map<List<Solution>>(sol.SolutionListJson);
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
                AuditFlowId = processId,
                GradientId = mintd.Id,
                InputCount = 0,
                SolutionId = Solution.Id,
                Year = soptime,
                UpDown = YearType.FirstHalf
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
                    GradientId = gradient.Id,
                    Product = solution.Product,
                    gradient = gradient.GradientValue + "K/Y",
                    SopCost = 1,
                    FullLifeCyclecost = 2,
                    Price = 1,
                    SopGrossMargin = 23,
                    TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23,
                    NreGrossMargin = 10
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
                    SopCost = 1,
                    FullLifeCyclecost = 2,
                    Price = 1,
                    SopGrossMargin = 23,
                    TotallifeCyclegrossMargin = 12,
                    ClientGrossMargin = 23,
                    NreGrossMargin = 10
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
    internal async Task<List<long>> GetExternalQuotationNumberOfQuotations(long auditFlowId)
    {
        List<ExternalQuotation> externalQuotations =
           await _externalQuotation.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
        return externalQuotations.Select(p=>p.NumberOfQuotations).OrderBy(p=>p).ToList();
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
            externalQuotationDto.NreQuotationListDtos = ObjectMapper.Map<List<NreQuotationListDto>>(nreQuotationLists);
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
                    Year = long.Parse(a.Year),
                    TravelVolume = a.Motion,
                    UnitPrice = decimal.Parse(a.UntilPrice)
                }).ToList();
            externalQuotationDto.NreQuotationListDtos = quotationNreDtos.Select((a, index) => new NreQuotationListDto()
            {
                SerialNumber = index + 1,
                ProductName = a.Product,
                HandmadePartsFee = a.shouban,
                MyPropMoldCosterty = a.moju,
                CostOfToolingAndFixtures = a.gzyj,
                ExperimentalFees = a.sy,
                RDExpenses = a.qt + a.cl + a.csrj+a.jianju,
            }).ToList();
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

        long i = await _externalQuotation.CountAsync(p => p.AuditFlowId.Equals(externalQuotationDto.AuditFlowId));
        string year = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM");
        string iSttring = (i + 1).ToString("D4");
        externalQuotation.RecordNo = year + iSttring;
        externalQuotation.CreationTime = DateTime.Now;
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
    }

    /// <summary>
    ///  下载对外报价单
    /// </summary>
    /// <returns></returns>
    internal async Task<FileResult> DownloadExternalQuotation(long auditFlowId, long solutionId,
        long numberOfQuotations)
    {
        ExternalQuotationDto external =
            await GetExternalQuotation(auditFlowId, solutionId, numberOfQuotations, null, null);
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

        return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
            { FileDownloadName = "报价单下载.xlsx" };
    }

    public async Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(long processId,int version)
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
            "手板件费用", "模具费用", " 工装费用", "治具费用", "检具费用", "生产设备费用", "专用生产设备", "非专用生产设备", "实验费用", "测试软件费用", "差旅费", "其他费用",
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
}