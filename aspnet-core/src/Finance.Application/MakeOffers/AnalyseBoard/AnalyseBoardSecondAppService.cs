﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
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
using Finance.Processes;
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

    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

    /// <summary>
    /// 报价Nre
    /// </summary>
    private readonly IRepository<NreQuotation, long> _nreQuotation;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AnalyseBoardSecondAppService(AnalysisBoardSecondMethod analysisBoardSecondMethod,
        AuditFlowAppService flowAppService,
        IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
        IRepository<AuditQuotationList, long> financeAuditQuotationList,
        IRepository<Gradient, long> gradientRepository,
        IRepository<NreQuotation, long> nreQuotation,
        IRepository<Solution, long> resourceSchemeTable)
    {
        _financeAuditQuotationList = financeAuditQuotationList;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _analysisBoardSecondMethod = analysisBoardSecondMethod;
        _nreQuotation = nreQuotation;
        _flowAppService = flowAppService;
        _gradientRepository = gradientRepository;
        _flowAppService = flowAppService;
        _resourceSchemeTable = resourceSchemeTable;
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
            return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond(auditFlowId, version);
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
    public async virtual Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(long auditFlowId,int version)
    {
        CoreComponentAndNreDto coreComponentAndNreDto = new();
        return await _analysisBoardSecondMethod.GetCoreComponentAndNreList(auditFlowId,version);
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
    /// 总经理报价审批界面一
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<ManagerApprovalOfferDto> GetManagerApprovalOfferOne(long auditFlowId,int version)
    {
        ManagerApprovalOfferDto managerApprovalOfferDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferOne(auditFlowId,version);

        return managerApprovalOfferDto;
    }

    /// <summary>
    /// 总经理报价审批界面二
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetManagerApprovalOfferTwo(long auditFlowId ,int version)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId,version);
        return quotationListSecondDto;
    }

    /// <summary>
    /// 营销部报价审批
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetQuotationApprovedMarketing(long auditFlowId,int version )

    {
        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId,version);
        ;
    }

    /// <summary>
    /// 财务归档
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> FinancialFiling(long auditFlowId,int version 
        )

    {
        return await _analysisBoardSecondMethod.QuotationListSecond(auditFlowId,version);
        ;
    }

    /// <summary>
    /// 报价反馈
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<AnalyseBoardSecondDto> GetQuotationFeedback(long auditFlowId, int version)
    {
        return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond(auditFlowId, version);
    }

    /// <summary>
    /// 中标确认
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetAcceptanceBid(long auditFlowId,int version)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId,version);
        return quotationListSecondDto;
    }

    /// <summary>
    /// 总经理中标查看
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<QuotationListSecondDto> GetBidView(long auditFlowId,int version)
    {
        QuotationListSecondDto quotationListSecondDto =
            await _analysisBoardSecondMethod.GetManagerApprovalOfferTwo(auditFlowId,version);
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

    /// <summary>
    /// 用于对外报价产品清单
    /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<ProductDto>> GetProductList(long auditFlowId, int version)
    {
        List<ProductDto> productDtos = new List<ProductDto>();
        productDtos.Add(new ProductDto()
        {
            ProductName = "测试",
            Motion = 1,
            Year = "2023",
            UntilPrice = "12"
        });

        return productDtos;
    }

    /// <summary>
    /// 用于对外报价NRE清单
    ///  /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotationNreDto>> GetNREList(long auditFlowId, int version)
    {
        List<QuotationNreDto> productDtos = new List<QuotationNreDto>();

        var nres = await _nreQuotation.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.version == version);
        var nresmap = nres.GroupBy(p => p.SolutionId).ToDictionary(r => r.Key, r => r.ToList());
        foreach (var nremap in nresmap)
        {
            long solutionid = nremap.Key.Value;
            var nresList = nremap.Value;
          var solution=  _resourceSchemeTable.FirstOrDefault(p => p.Id == solutionid);
            productDtos.Add(new QuotationNreDto()
                {
                    Product = solution.ModuleName,
                    Pcs = 1,
                    shouban = nresList.Where(p=>p.FormName.Equals("手板件费")).Sum(p=>p.OfferMoney),
                    moju = nresList.Where(p=>p.FormName.Equals("模具费")).Sum(p=>p.OfferMoney),
                    gzyj = nresList.Where(p=>p.FormName.Equals("工装费")||p.FormName.Equals("治具费")).Sum(p=>p.OfferMoney),
                    sy = nresList.Where(p=>p.FormName.Equals("实验费")).Sum(p=>p.OfferMoney),
                    csrj = nresList.Where(p=>p.FormName.Equals("测试软件费")).Sum(p=>p.OfferMoney),
                    cl = nresList.Where(p=>p.FormName.Equals("差旅费")).Sum(p=>p.OfferMoney),
                    qt = nresList.Where(p=>p.FormName.Equals("其他费用")).Sum(p=>p.OfferMoney),
                    scsb=nresList.Where(p=>p.FormName.Equals("生产设备费")).Sum(p=>p.OfferMoney),
                    jianju=nresList.Where(p=>p.FormName.Equals("检具费")).Sum(p=>p.OfferMoney)
                }
            );
        }
        return productDtos;
    }
}