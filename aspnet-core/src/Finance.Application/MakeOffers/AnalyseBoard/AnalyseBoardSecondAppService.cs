﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.Nre;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HPSF;

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

    private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;

    /// <summary>
    /// 核价部分服务
    /// </summary>
    public readonly PriceEvaluationAppService _priceEvaluationAppService;

    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

    /// <summary>
    /// 报价方案组合
    /// </summary>
    private readonly IRepository<SolutionQuotation, long> _solutionQutation;

    /// <summary>
    /// 归档文件列表实体类
    /// </summary>
    private readonly IRepository<DownloadListSave, long> _financeDownloadListSave;

    /// <summary>
    /// 报价Nre
    /// </summary>
    private readonly IRepository<NreQuotation, long> _nreQuotation;

    private readonly IRepository<AfterUpdateSumInfo, long> _afterUpdateSumInfoRepository;

    public AnalyseBoardSecondAppService(AnalysisBoardSecondMethod analysisBoardSecondMethod, IRepository<Gradient, long> gradientRepository, IRepository<AuditQuotationList, long> financeAuditQuotationList, IRepository<Solution, long> resourceSchemeTable, AuditFlowAppService flowAppService, FileCommonService fileCommonService, PriceEvaluationGetAppService priceEvaluationGetAppService, PriceEvaluationAppService priceEvaluationAppService, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<SolutionQuotation, long> solutionQutation, IRepository<DownloadListSave, long> financeDownloadListSave, IRepository<NreQuotation, long> nreQuotation, IRepository<AfterUpdateSumInfo, long> afterUpdateSumInfoRepository)
    {
        _analysisBoardSecondMethod = analysisBoardSecondMethod;
        _gradientRepository = gradientRepository;
        _financeAuditQuotationList = financeAuditQuotationList;
        _resourceSchemeTable = resourceSchemeTable;
        _flowAppService = flowAppService;
        _fileCommonService = fileCommonService;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;
        _priceEvaluationAppService = priceEvaluationAppService;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _solutionQutation = solutionQutation;
        _financeDownloadListSave = financeDownloadListSave;
        _nreQuotation = nreQuotation;
        _afterUpdateSumInfoRepository = afterUpdateSumInfoRepository;
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
    /// 用于调试接口
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<List<Gradient>> getInterface(
        long auid)
    {
        //获取梯度
        List<Gradient> gradients =
            await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auid);
        gradients = gradients.OrderBy(p => p.GradientValue).ToList();

        return gradients;
    }

    /// <summary>
    /// 根据流程id,版本version 查看报表分析看板  查看报价分析看板不含样品,查看报价分析看板含样品,查看报价分析看板仅含样品
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<AnalyseBoardSecondDto> getStatementAnalysisBoardSecond(long auditFlowId, int version)
    {
        AnalyseBoardSecondDto analyseBoardSecondDto = new AnalyseBoardSecondDto();
        try
        {
            return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond(auditFlowId, version, 0);
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
        var electronicAndStructureList = await _priceEvaluationAppService.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, InputCount = input.InputCount, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

        List<CoreDevice> CoreDeviclist = new List<CoreDevice>();
        decimal moq = 0;
        foreach (var material in electronicAndStructureList)
        {
            moq = moq + material.MoqShareCount;
            if (material.SuperType.Equals("电子料") && material.TypeName.Equals("Sensor芯片")
                || material.SuperType.Equals("电子料") && material.TypeName.Equals("串行芯片")
                || material.SuperType.Equals("结构料") && material.TypeName.Equals("镜头")
                )
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = material.MaterialName;
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);

            }

            else if (material.SuperType.Equals("电子料") && !material.TypeName.Equals("Sensor芯片") || material.SuperType.Equals("电子料") && !material.TypeName.Equals("串行芯片"))
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = "PCBA";
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);
            }

            else if (!material.SuperType.Equals("电子料") && !material.TypeName.Equals("镜头"))
            {
                CoreDevice CoreDevice = new CoreDevice();
                CoreDevice.ProjectName = "结构件（除lens）";
                CoreDevice.UnitPrice = material.MaterialPrice;
                CoreDevice.Number = material.AssemblyCount;
                CoreDevice.Rate = material.ExchangeRate;
                CoreDevice.Sum = material.TotalMoneyCynNoCustomerSupply;

                CoreDeviclist.Add(CoreDevice);
            }

        }


        //质量成本
        var QualityList = await _afterUpdateSumInfoRepository.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.SolutionId.Equals(input.SolutionId) && p.GradientId.Equals(input.GradientId) && p.Year.Equals(input.Year) && p.UpDown.Equals(input.UpDown));
        
        CoreDevice zhiliangCoreDevice = new CoreDevice();
        zhiliangCoreDevice.ProjectName = "质量成本";
        zhiliangCoreDevice.Sum = QualityList.FirstOrDefault().ManufacturingAfterSum;
        CoreDeviclist.Add(zhiliangCoreDevice);

        //损耗成本
        CoreDevice lossCostCoreDevice = new CoreDevice();
        lossCostCoreDevice.ProjectName = "损耗成本";
        lossCostCoreDevice.Sum = QualityList.FirstOrDefault().LossCostAfterSum;
        CoreDeviclist.Add(lossCostCoreDevice);

        //制造成本
        CoreDevice qualityCosDevice = new CoreDevice();
        qualityCosDevice.ProjectName = "制造成本";
        qualityCosDevice.Sum = QualityList.FirstOrDefault().QualityCostAfterSum;
        CoreDeviclist.Add(qualityCosDevice);

        //物流成本
        CoreDevice logisticsCosDevice = new CoreDevice();
        logisticsCosDevice.ProjectName = "物流成本";
        logisticsCosDevice.Sum = QualityList.FirstOrDefault().LogisticsAfterSum;
        CoreDeviclist.Add(logisticsCosDevice);

        //其他成本
        CoreDevice OtherCostDevice = new CoreDevice();
        OtherCostDevice.ProjectName = "其他成本";
        OtherCostDevice.Sum = QualityList.FirstOrDefault().OtherCosttAfterSum;
        CoreDeviclist.Add(OtherCostDevice);


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
    /// 根据流程号查询对外报价单的版本号
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetExternalQuotationNumberOfQuotations(long auditFlowId)
    {
        return await _analysisBoardSecondMethod.GetExternalQuotationNumberOfQuotations(auditFlowId);
    }

    /// <summary>
    /// 对外报价单查询
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ExternalQuotationDto> GetExternalQuotation(long auditFlowId, long solutionId,
        long numberOfQuotations)
    {
        //暂时注释 等报价看板完成之后放开
        List<SolutionQuotationDto> solutionQuotations = await GeCatalogue(auditFlowId);
        solutionQuotations = solutionQuotations.Where(p => p.Id == solutionId).ToList();
        if (solutionQuotations is null || solutionQuotations.Count == 0)
        {
            throw new FriendlyException($"solutionId:{solutionId}报价方案ID不存在");
        }

        List<ProductDto> productDtos = await GetProductList(auditFlowId, (int)numberOfQuotations);
        List<QuotationNreDto> quotationNreDtos = await GetNREList(auditFlowId, (int)numberOfQuotations);

        return await _analysisBoardSecondMethod.GetExternalQuotation(auditFlowId, solutionId, numberOfQuotations,
            productDtos, quotationNreDtos);
    }

    /// <summary>
    /// 对外报价单保存/提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task SaveExternalQuotation(ExternalQuotationDto externalQuotationDto)
    {
        //暂时注释 等报价看板完成之后放开
        List<SolutionQuotationDto> solutionQuotations = await GeCatalogue(externalQuotationDto.AuditFlowId);
        solutionQuotations = solutionQuotations.Where(p => p.Id == externalQuotationDto.SolutionId).ToList();
        if (solutionQuotations is null || solutionQuotations.Count == 0)
        {
            throw new FriendlyException($"solutionId:{externalQuotationDto.SolutionId}报价方案ID不存在");
        }

        await _analysisBoardSecondMethod.SaveExternalQuotation(externalQuotationDto);
    }

    /// <summary>
    /// 根据流程号获取报价目录
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<SolutionQuotationDto>> GeCatalogue(long auditFlowId)
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

    ///// <summary>
    ///  审核的保存
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
    /// 报价分析看板 仅保存
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostIsOfferSecondOnlySave(IsOfferSecondDto isOfferDto)
    {
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
        /*if (AbpSession.UserId is null)
        {
            throw new FriendlyException("请先登录");
        }*/
        if (isOfferDto.IsOffer)
        {
            //进行报价
            await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
        }


        /*
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
        */
    }

    public async Task PostIsOffer(IsOfferSecondDto isOfferDto)
    {
        await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
    }

    /// <summary>
    /// 获取对应单价  ntype 报价分析看板0 ，报价反馈1
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<SoltionGradPrice>> GetSoltionGradPriceList(long auditFlowId, int version, int ntype)
    {
        return await _analysisBoardSecondMethod.GetSoltionGradPriceList(auditFlowId, version, ntype);
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

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 1, 1);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId, version, 0);
        }
        else
        {
            return JsonConvert.DeserializeObject<ManagerApprovalOfferDto>(au.AuditQuotationListJson);
        }
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

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 2, 0);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 0);
        }
        else
        {
            return JsonConvert.DeserializeObject<QuotationListSecondDto>(au.AuditQuotationListJson);
        }
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
            return await _analysisBoardSecondMethod.DownloadAuditQuotationList(auditFlowId, version, 0);
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
    public async Task<QuotationListSecondDto> GetQuotationApprovedMarketing(long auditFlowId, int version)

    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }


        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 0, 0);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId, version, 0);
        }
        else
        {
            return JsonConvert.DeserializeObject<QuotationListSecondDto>(au.AuditQuotationListJson);
        }
    }

    /// <summary>
    /// 营销部报价保存/修改
    /// </summary>
    /// <param name="auditFlowId">必输</param>
    ///  /// <param name="version">必输</param>
    /// <returns></returns>
    public async Task PostQuotationApprovedMarketingSave(QuotationListSecondDto quotationListSecondDto)

    {
        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(quotationListSecondDto.AuditFlowId,
            quotationListSecondDto.version, 1, 0);
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
    /// 财务归档
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> FinancialFiling(long auditFlowId, int version
    )

    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"流程ID:{auditFlowId}的报价方案组合{version}不存在");
        }

        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId, version, 1);
        ;
    }

    /// <summary>
    /// 报价反馈 保存
    /// </summary>
    /// <param name="isOfferDto"></param>
    /// <returns></returns>
    public async Task PostQuotationFeedback(IsOfferSecondDto isOfferDto)
    {
        /*if (AbpSession.UserId is null)
        {
            throw new FriendlyException("请先登录");
        }*/
        if (isOfferDto.IsOffer)
        {
            isOfferDto.ntype = 1;
            //进行报价
            await _analysisBoardSecondMethod.PostIsOfferSaveSecond(isOfferDto);
        }
    }

    /// <summary>
    /// 报价反馈
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<AnalyseBoardSecondDto> GetQuotationFeedback(AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
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
    public async Task<ManagerApprovalOfferDto> GeQuotationFeedbacktManagerOne(long auditFlowId, int version)
    {
        SolutionQuotation sol =
            await _solutionQutation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        if (sol is null)
        {
            throw new FriendlyException($"solutionId:{auditFlowId}报价方案组合{version}不存在");
        }

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 1, 1);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId, version, 1);
        }
        else
        {
            return JsonConvert.DeserializeObject<ManagerApprovalOfferDto>(au.AuditQuotationListJson);
        }
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

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 2, 1);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 1);
        }
        else
        {
            return JsonConvert.DeserializeObject<QuotationListSecondDto>(au.AuditQuotationListJson);
        }
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

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 2, 1);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 1);
        }
        else
        {
            return JsonConvert.DeserializeObject<QuotationListSecondDto>(au.AuditQuotationListJson);
        }
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

        var au = await _analysisBoardSecondMethod.GetfinanceAuditQuotationList(auditFlowId, version, 2, 1);
        if (au is null)
        {
            return await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId, version, 1);
        }
        else
        {
            return JsonConvert.DeserializeObject<QuotationListSecondDto>(au.AuditQuotationListJson);
        }
    }

    /// <summary>
    /// 报价审核表 审批
    /// </summary>
    /// <param name="quotationListDto"></param>
    /// <returns></returns>
    public async Task PostAuditQuotationList(AuditQuotationListDto quotationListDto)
    {
        await this.GetDownloadListSave(quotationListDto.AuditFlowId);
    }

    /// <summary>
    /// 归档文件列表保存
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
            // 报价审核表 文件保存

            var time = _solutionQutation.GetAllList(p => p.AuditFlowId == auditFlow).Max(p => p.ntime);
            var sols = _solutionQutation.GetAllList(p => p.AuditFlowId == auditFlow && p.ntime == time);
            foreach (var sol in sols)
            {
                MemoryStream memoryStreamOffer =
                    await _analysisBoardSecondMethod.DownloadAuditQuotationListStream(auditFlow, sol.version, 1,
                        "报价审批表" + sol.version);
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
                    FileName = "版本" + sol.version + "报价审批表", FilePath = fileUploadOutputDtoOffer.FileUrl,
                    FileId = fileUploadOutputDtoOffer.FileId
                });


                List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(sol.SolutionListJson);
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


                        FileName = "产品" + solution.ModuleName + "梯度" + gradient.GradientValue + "核价表.xlsx";
                        IFormFile fileOfferhejia = new FormFile(hejia, 0, memoryStreamOffer.Length, FileName, FileName);
                        FileUploadOutputDto fileUploadOutputDtoOfferhejia =
                            await _fileCommonService.UploadFile(fileOfferhejia);
                        //核价表的路径和名称保存到


                        await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                        {
                            AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                            ProductName = "", ProductId = 0,
                            FileName = "产品" + solution.ModuleName + "梯度" + gradient.GradientValue + "核价表",
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
                        new FormFile(nrehejia, 0, memoryStreamOffer.Length, FileName, FileName);
                    FileUploadOutputDto fileUploadOutputDtoOffernrehejia =
                        await _fileCommonService.UploadFile(fileOffernrehejia);
                    //NRE核价表的路径和名称保存到


                    await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                    {
                        AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                        ProductName = "", ProductId = 0,
                        FileName = solution.ModuleName + "NRE核价表", FilePath = fileUploadOutputDtoOffernrehejia.FileUrl,
                        FileId = fileUploadOutputDtoOffernrehejia.FileId
                    });


                    //对外报价单
                    MemoryStream dwbjd =
                        await _analysisBoardSecondMethod.DownloadExternalQuotationStream(auditFlow, solution.Id,
                            sol.version);
                    FileName = solution.ModuleName + "对外报价单.xlsx";
                    IFormFile fileOfferdwbj = new FormFile(dwbjd, 0, memoryStreamOffer.Length, FileName, FileName);
                    FileUploadOutputDto fileUploadOutputDtoOfferdwbj =
                        await _fileCommonService.UploadFile(fileOfferdwbj);
                    //对外报价单的路径和名称保存到


                    await _financeDownloadListSave.InsertAsync(new DownloadListSave()
                    {
                        AuditFlowId = auditFlow, QuoteProjectName = priceEvaluationStartInputResult.ProjectName,
                        ProductName = "", ProductId = 0,
                        FileName = solution.ModuleName + "对外报价单", FilePath = fileUploadOutputDtoOfferdwbj.FileUrl,
                        FileId = fileUploadOutputDtoOfferdwbj.FileId
                    });
                }
            }
        }
    }

    /// <summary>
    /// 归档文件列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<PigeonholeDownloadTableModel>> GetDownloadList(long? auditFlowId)
    {
        List<PigeonholeDownloadTableModel> pigeonholeDownloadTableModels = new();
        List<DownloadListSave> downloadListSaves =
            await _financeDownloadListSave.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));


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
    /// 归档文件下载 传 DownloadListSaveId
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
    /// </summary>
    /// <returns></returns>
    public async Task<List<ProductDto>> GetProductList(long auditFlowId, int version)
    {
        List<SoltionGradPrice> gsp = await _analysisBoardSecondMethod.GetSoltionGradPriceList(auditFlowId, version, 1);
        Dictionary<long, List<SoltionGradPrice>> gsmap = gsp.GroupBy(p => p.Gradientid)
            .ToDictionary(x => x.Key, x => x.Select(e => e).ToList());

        var gradients = await _analysisBoardSecondMethod.getGradient(auditFlowId);


        //获取核价营销相关数据
        var priceEvaluationStartInputResult =
            await _priceEvaluationAppService.GetPriceEvaluationStartData(auditFlowId);
        var modelcouts = priceEvaluationStartInputResult.ModelCount;
        List<ProductDto> productDtos = new List<ProductDto>();
        foreach (var modelcout in modelcouts)
        {
            var mcys = modelcout.ModelCountYearList;

            foreach (var mcy in mcys)
            {
                String key = "";


                if (mcy.Equals(YearType.FirstHalf))
                {
                    key += "上半年";
                }

                if (mcy.Equals(YearType.SecondHalf))
                {
                    key += "下半年";
                }

                if (mcy.Equals(YearType.Year))
                {
                    key += "年";
                }

                string UntilPrice = "";
                foreach (var gradient in gradients)
                {
                    if (mcy.Quantity <= gradient.GradientValue)
                    {
                        var list = gsmap[gradient.Id];
                        UntilPrice = list.FirstOrDefault(p => p.Equals(modelcout.Product)).UnitPrice.ToString();
                    }
                }

                productDtos.Add(new ProductDto()
                {
                    ProductName = modelcout.Product,
                    Motion = mcy.Quantity,
                    Year = key,
                    UntilPrice = UntilPrice
                });
            }
        }


        return productDtos;
    }

    /// <summary>
    /// 用于对外报价NRE清单
    ///<param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotationNreDto>> GetNREList(long auditFlowId, int version)
    {
        List<QuotationNreDto> productDtos = new List<QuotationNreDto>();

        var nres = await _nreQuotation.GetAllListAsync(p =>
            p.AuditFlowId == auditFlowId && p.version == version && p.SolutionId != null);
        var nresmap = nres.GroupBy(p => p.SolutionId).ToDictionary(r => r.Key, x => x.Select(e => e).ToList());
        foreach (var nremap in nresmap)
        {
            long solutionid = nremap.Key.Value;
            var nresList = nremap.Value;
            var solution = _resourceSchemeTable.FirstOrDefault(p => p.Id == solutionid);
            productDtos.Add(new QuotationNreDto()
                {
                    Product = solution.ModuleName,
                    Pcs = 1,
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