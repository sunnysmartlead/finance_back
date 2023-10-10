using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.Authorization;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.FinanceMaintain;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.NrePricing.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Finance.MakeOffers.AnalyseBoard;

public class AnalyseBoardSecondAppService : FinanceAppServiceBase, IAnalyseBoardSecondAppService
{
    /// <summary>
    /// 分析看板方法
    /// </summary>
    public readonly AnalysisBoardSecondMethod _analysisBoardSecondMethod;

    /// <summary>
    /// 报价审核表
    /// </summary>
    private readonly IRepository<AuditQuotationList, long> _financeAuditQuotationList;

    /// <summary>
    /// 核价梯度相关
    /// </summary>
    private readonly IRepository<Gradient, long> _gradientRepository;

    protected readonly IRepository<ModelCount, long> _modelCountRepository;
    protected readonly IRepository<ModelCountYear, long> _modelCountYearRepository;

    /// <summary>
    /// 营销部审核中方案表
    /// </summary>
    public readonly IRepository<Solution, long> _resourceSchemeTable;

    /// <summary>
    /// 核价相关
    /// </summary>
    private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

    /// <summary>
    /// Nre核价api
    /// </summary>
    public readonly NrePricingAppService _nrePricingAppService;

    /// <summary>
    /// Nre核价api
    /// </summary>
    public readonly ProcessHoursEnterLineAppService _processHoursEnterLineAppService;

    public readonly PriceEvaluationAppService _priceEvaluationAppService;

    /// <summary>
    /// 归档文件列表实体类
    /// </summary>
    private readonly IRepository<DownloadListSave, long> _financeDownloadListSave;

    /// <summary>
    /// 报价分析看板中的 产品单价表 实体类
    /// </summary>
    private readonly IRepository<UnitPriceOffers, long> _resourceUnitPriceOffers;

    private readonly IRepository<GradientModel, long> _gradientModelRepository;
    private readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;
    private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
    private readonly IRepository<Sample, long> _sampleRepository;

    /// <summary>
    ///报价 项目看板实体类 实体类
    /// </summary>
    private readonly IRepository<ProjectBoardOffers, long> _resourceProjectBoardOffers;

    /// <summary>
    /// 报价分析看板中的 汇总分析表  实体类
    /// </summary>
    private readonly IRepository<PooledAnalysisOffers, long> _resourcePooledAnalysisOffers;

    /// <summary>
    ///  财务维护 毛利率方案
    /// </summary>
    private readonly IRepository<GrossMarginForm, long> _configGrossMarginForm;

    /// <summary>
    /// 报价分析看板中的 动态单价表 实体类
    /// </summary>
    private readonly IRepository<DynamicUnitPriceOffers, long> _resourceDynamicUnitPriceOffers;

    /// <summary>
    /// 流程流转服务
    /// </summary>
    private readonly AuditFlowAppService _flowAppService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AnalyseBoardSecondAppService(AnalysisBoardSecondMethod analysisBoardSecondMethod,
        IRepository<Gradient, long> gradientRepository, IRepository<GradientModel, long> gradientModelRepository,
        IRepository<UnitPriceOffers, long> unitPriceOffers,
        NrePricingAppService nrePricingAppService,
        PriceEvaluationAppService priceEvaluationAppService,
        ProcessHoursEnterLineAppService processHoursEnterLineAppService,
        IRepository<DynamicUnitPriceOffers, long> DynamicUnitPriceOffers,
        IRepository<ProjectBoardOffers, long> projectBoardOffers,
        AuditFlowAppService flowAppService,
        IRepository<AuditQuotationList, long> financeAuditQuotationList,
        PriceEvaluationGetAppService priceEvaluationGetAppService, IRepository<Sample, long> sampleRepository,
        IRepository<PriceEvaluation, long> priceEvaluationRepository,
        IRepository<PooledAnalysisOffers, long> pooledAnalysisOffers,
        IRepository<GrossMarginForm, long> configGrossMarginForm,
        IRepository<DownloadListSave, long> financeDownloadListSave,
        IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository,
        IRepository<GradientModelYear, long> gradientModelYearRepository,
        IRepository<Solution, long> resourceSchemeTable)
    {
        _priceEvaluationRepository = priceEvaluationRepository;
        _financeAuditQuotationList = financeAuditQuotationList;
        _financeDownloadListSave = financeDownloadListSave;

        _analysisBoardSecondMethod = analysisBoardSecondMethod;
        _resourceDynamicUnitPriceOffers = DynamicUnitPriceOffers;
        _resourceProjectBoardOffers = projectBoardOffers;
        _processHoursEnterLineAppService = processHoursEnterLineAppService;
        _flowAppService = flowAppService;
        _gradientRepository = gradientRepository;
        _resourceSchemeTable = resourceSchemeTable;

        _resourceUnitPriceOffers = unitPriceOffers;
        _resourcePooledAnalysisOffers = pooledAnalysisOffers;
        _priceEvaluationAppService = priceEvaluationAppService;
        _nrePricingAppService = nrePricingAppService;
        _configGrossMarginForm = configGrossMarginForm;
        _modelCountRepository = modelCountRepository;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;
        _modelCountYearRepository = modelCountYearRepository;
        _gradientModelRepository = gradientModelRepository;
        _sampleRepository = sampleRepository;
        _gradientModelYearRepository = gradientModelYearRepository;
    }

    /// <summary>
    /// 查看报表分析看板  查看报价分析看板不含样品,查看报价分析看板含样品,查看报价分析看板仅含样品
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        try
        {
            return await _analysisBoardSecondMethod.PostStatementAnalysisBoardSecond(analyseBoardSecondInputDto);
        } catch (Exception e)
        {
            analyseBoardSecondDto.mes = e.Message;
            return analyseBoardSecondDto;
        }

    }


    /// <summary>
    /// 下载成本信息表二开
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PostDownloadMessageSecond(AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        try
        {
            string FileName = "成本信息表下载";
            return await _analysisBoardSecondMethod.DownloadMessageSecond(analyseBoardSecondInputDto, FileName);
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 汇总分析表 根据 整套 毛利率 计算(本次报价)
    /// </summary>
    public async Task<TargetPrice> PostSpreadSheetCalculate(SpreadSheetCalculateSecondDto productBoardProcessDto)
    {
        try
        {
            return null;
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 查看 核心器件、Nre费用拆分
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async virtual Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(long auditFlowId)
    {
        CoreComponentAndNreDto coreComponentAndNreDto = new();


        return await _analysisBoardSecondMethod.GetCoreComponentAndNreList(auditFlowId);
    }

    /// <summary>
    /// 下载核心器件、Nre费用拆分
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> GetDownloadCoreComponentAndNre(long Id, string FileName = "核心器件、Nre费用拆分下载")
    {
        try
        {
            // return await _analysisBoardSecondMethod.DownloadMessageSecond(Id, FileName);
            return null;
        }
        catch (Exception e)
        {
            throw new FriendlyException(e.Message);
        }
    }

    /// <summary>
    /// 查看年份维度对比(全部模组)
    /// </summary>
    /// <param name="yearProductBoardProcessDto"></param>
    /// <returns></returns>
    public async Task<YearDimensionalityComparisonSecondDto> PostYearDimensionalityComparison(
        YearProductBoardProcessSecondDto yearProductBoardProcessDto)
    {
        return await _analysisBoardSecondMethod.YearDimensionalityComparison(yearProductBoardProcessDto);
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
    /// 对外报价单
    /// </summary>
    /// <returns></returns>
    public async Task<ExternalQuotationDto> GetExternalQuotation(long auditFlowId)
    {
        return await _analysisBoardSecondMethod.GetExternalQuotation(auditFlowId);
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
    /// 报价分析看板 的保存
    /// </summary>
    /// <param name="quotationListDto"></param>
    /// <returns></returns>
    public async Task PostIsOfferSave(IsOfferSecondDto analyseBoardSecondDto)
    {
        AuditQuotationList auditQuotationList =
            await _financeAuditQuotationList.FirstOrDefaultAsync(p =>
                p.AuditFlowId.Equals(analyseBoardSecondDto.AuditFlowId));
        if (auditQuotationList is not null)
        {
            auditQuotationList.AuditQuotationListJson = JsonConvert.SerializeObject(analyseBoardSecondDto);
            await _financeAuditQuotationList.UpdateAsync(auditQuotationList);
        }
        else
        {
            await _financeAuditQuotationList.InsertAsync(new AuditQuotationList()
            {
                AuditFlowId = analyseBoardSecondDto.AuditFlowId,
                AuditQuotationListJson = JsonConvert.SerializeObject(analyseBoardSecondDto),
            });
        }
    }

    /// <summary>
    /// 报价接口
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostIsOfferSecond(IsOfferSecondDto isOfferDto)
    {
        if (AbpSession.UserId is null)
        {
            throw new FriendlyException("请先登录");
        }

        AuditFlowDetailDto flowDetailDto = new()
        {
            AuditFlowId = isOfferDto.AuditFlowId,
            ProcessIdentifier = AuditFlowConsts.AF_CostCheckNreFactor,
            UserId = AbpSession.UserId.Value
        };
        if (isOfferDto.IsOffer)
        {
            //进行报价
            await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
            flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
        }
        else
        {
            //不报价
            flowDetailDto.Opinion = OPINIONTYPE.Reject;
            flowDetailDto.OpinionDescription = OpinionDescription.OD_QuotationCheck;
        }

        ReturnDto returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);
    }

    public async Task PostIsOffer(IsOfferSecondDto isOfferDto)
    {
        await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
    }
    

 

    /// <summary>
    /// 总经理报价审批界面一
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long auditFlowId)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId);

        return managerApprovalOfferDto;
    }

    /// <summary>
    /// 总经理报价审批界面二
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long auditFlowId)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId);
        return quotationListSecondDto;
    }

    /// <summary>
    /// 营销部报价审批
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetQuotationApprovedMarketing(long auditFlowId)

    {
        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId);
        ;
    }

    /// <summary>
    /// 财务归档
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> FinancialFiling(long auditFlowId)

    {
        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId);
        ;
    }

    /// <summary>
    /// 报价反馈
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationFeedbackDto> GetQuotationFeedback(long auditFlowId)
    {
        QuotationFeedbackDto analyseBoardSecondDto = new();

        //获取方案
        List<Solution> Solutions = await _resourceSchemeTable.GetAllListAsync(p => p.Id == 115);
        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);

        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        //最小梯度值
        var mintd = gradients.OrderBy(e => e.GradientValue).First();

        List<GradientGrossMarginModel> gradientGrossMarginModels = new();
        //获取毛利率
        List<decimal> gross = await _analysisBoardSecondMethod.GetGrossMargin();
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
            var ex = await _priceEvaluationAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = auditFlowId, GradientId = mintd.Id, InputCount = 0, SolutionId = Solution.Id,
                Year = soptime, UpDown = YearType.FirstHalf
            });

            //最小梯度SOP年成本
            var totalcost = ex.TotalCost;

            //样品阶段
            if (priceEvaluationStartInputResult.IsHasSample == true)
            {
                OnlySampleDto onlySampleDto = new();
                List<SampleQuotation> onlySampleModels =
                    await _analysisBoardSecondMethod.getSample(sampleDtos, totalcost);
                onlySampleDto.SolutionName = Solution.SolutionName;
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
                    gr.Grossvalue = totalcost / (1 - (gro / 100));
                    gr.Gross = gro.ToString();
                    grosss.Add(gr);
                }

                sopAnalysisModel.GrossValues = grosss;
                sops.Add(sopAnalysisModel);
            }
        }


//NRE
        analyseBoardSecondDto.nres = await _analysisBoardSecondMethod.getNre(auditFlowId,
            Solutions);
        //样品阶段
        analyseBoardSecondDto.SampleOffer = samples;
        //sop单价表
        analyseBoardSecondDto.Sops = sops;
        analyseBoardSecondDto.FullLifeCycle =
            await _analysisBoardSecondMethod.GetPoolAnalysis(auditFlowId, gradients, priceEvaluationStartInputResult,
                gross, Solutions, sops);
        analyseBoardSecondDto.GradientQuotedGrossMargins =
            await _analysisBoardSecondMethod.GetstepsNum(priceEvaluationStartInputResult, Solutions, gradients, sops)
            ;
        analyseBoardSecondDto.QuotedGrossMargins =
            await _analysisBoardSecondMethod.GetActual(priceEvaluationStartInputResult, Solutions);


        return analyseBoardSecondDto;
    }

    /// <summary>
    /// 中标确认
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetAcceptanceBid(long auditFlowId)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId);
        return quotationListSecondDto;
    }

    /// <summary>
    /// 总经理中标查看
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetBidView(long auditFlowId)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId);
        return quotationListSecondDto;
    }

    /// <summary>
    /// 归档文件列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<PigeonholeDownloadTableModel>> GetDownloadList(long? auditFlowId)
    {
        List<PigeonholeDownloadTableModel> pigeonholeDownloadTableModels = new();
        PigeonholeDownloadTableModel pg = new PigeonholeDownloadTableModel()
        {
            DownloadListSaveId = 129, // 归档文件列表id
            FileName = "测试", // 归档文件名称
            ProductName = "测试", // 零件名称
            QuoteProjectName = "测试1232" // 核报价项目名称
        };
        pigeonholeDownloadTableModels.Add(pg);
        return pigeonholeDownloadTableModels;
    }
}