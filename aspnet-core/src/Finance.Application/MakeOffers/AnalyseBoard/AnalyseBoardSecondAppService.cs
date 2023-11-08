using System;
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
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Microsoft.AspNetCore.Mvc;
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
    /// 营销部审核中方案表
    /// </summary>
    public readonly IRepository<Solution, long> _resourceSchemeTable;
    
    /// <summary>
    /// 流程流转服务
    /// </summary>
    private readonly AuditFlowAppService _flowAppService;
    private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

    /// <summary>
    /// 核价看板服务
    /// </summary>
    private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;

    /// <summary>
    /// NRE录入服务
    /// </summary>
    private readonly NrePricingAppService _nrePricingAppService;

    public AnalyseBoardSecondAppService(AnalysisBoardSecondMethod analysisBoardSecondMethod, IRepository<AuditQuotationList, long> financeAuditQuotationList, IRepository<Solution, long> resourceSchemeTable, AuditFlowAppService flowAppService, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, PriceEvaluationGetAppService priceEvaluationGetAppService, NrePricingAppService nrePricingAppService)
    {
        _analysisBoardSecondMethod = analysisBoardSecondMethod;
        _financeAuditQuotationList = financeAuditQuotationList;
        _resourceSchemeTable = resourceSchemeTable;
        _flowAppService = flowAppService;
        _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        _priceEvaluationGetAppService = priceEvaluationGetAppService;
        _nrePricingAppService = nrePricingAppService;
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
    /// 用于调试接口
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<List<FinanceDictionaryDetail>> getsample(
        AnalyseBoardSecondInputDto analyseBoardSecondInputDto)
    {
        List<FinanceDictionaryDetail> dics =  _financeDictionaryDetailRepository.GetAll()
            .Where(p => p.FinanceDictionaryId =="SampleName" ).ToList();
        return dics;
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
        return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond( auditFlowId,version);
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
    public async Task<GrossMarginSecondDto> PostGrossMarginForGradient( YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {

       
            return await _analysisBoardSecondMethod.PostGrossMarginForGradient( yearProductBoardProcessSecondDto);
       
        
    }
    /// <summary>
    /// 毛利率（实际数量）
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GrossMarginSecondDto> PostGrossMarginForactual( YearProductBoardProcessSecondDto yearProductBoardProcessSecondDto)
    {

       
        return await _analysisBoardSecondMethod.PostGrossMarginForactual( yearProductBoardProcessSecondDto);
       
        
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
    /// 查看 核心器件、Nre费用拆分
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    //public async virtual Task<CoreComponentAndNreDto> GetCoreComponentAndNreList(GetBomCostInput input)
    //{
   
    //    List<Material> malist = await _priceEvaluationGetAppService.GetBomCost(input);

    //    var 


    //    CoreComponentAndNreDto coreComponentAndNreDto = new();

    //    var BomMaterials=

    //    return await _analysisBoardSecondMethod.GetCoreComponentAndNreList(auditFlowId);
    //}

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
    /// 对外报价单查询
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ExternalQuotationDto> GetExternalQuotation(long auditFlowId, long solutionId, long numberOfQuotations)
    {
        //暂时注释 等报价看板完成之后放开
        //List<SolutionQuotation> solutionQuotations = await GeCatalogue(auditFlowId);
        //solutionQuotations= solutionQuotations.Where(p=>p.Id == solutionId).ToList();
        //if(solutionQuotations is null|| solutionQuotations.Count==0)
        //{
        //    throw new FriendlyException($"solutionId:{solutionId}报价方案ID不存在");
        //}
        List<ProductDto> productDtos = await GetProductList(auditFlowId, (int)numberOfQuotations);
        List<QuotationNreDto> quotationNreDtos = await GetNREList(auditFlowId, (int)numberOfQuotations);

        return await _analysisBoardSecondMethod.GetExternalQuotation(auditFlowId, solutionId, numberOfQuotations, productDtos, quotationNreDtos);
    }
    /// <summary>
    /// 对外报价单保存/提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task SaveExternalQuotation(ExternalQuotationDto externalQuotationDto)
    {
        //暂时注释 等报价看板完成之后放开
        //List<SolutionQuotation> solutionQuotations = await GeCatalogue(externalQuotationDto.AuditFlowId);
        //solutionQuotations = solutionQuotations.Where(p => p.Id == externalQuotationDto.SolutionId).ToList();
        //if (solutionQuotations is null || solutionQuotations.Count == 0)
        //{
        //    throw new FriendlyException($"solutionId:{externalQuotationDto.SolutionId}报价方案ID不存在");
        //}
        await _analysisBoardSecondMethod.SaveExternalQuotation(externalQuotationDto);
    }

    /// <summary>
    /// 根据流程号获取报价目录
    /// </summary>
    /// <param name="auditFlowId"></param>
    /// <returns></returns>
    public async Task<List<SolutionQuotation>> GeCatalogue(long auditFlowId)
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
    /// 报价分析看板 保存
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
    public async Task<AnalyseBoardSecondDto> GetQuotationFeedback(long auditFlowId,int version)
    {



        return await _analysisBoardSecondMethod.getStatementAnalysisBoardSecond(auditFlowId,version);
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
    /// <summary>
    /// 用于对外报价产品清单
    /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<ProductDto>> GetProductList(long auditFlowId,int version)
    {
        List<ProductDto> productDtos = new List<ProductDto>();
        productDtos.Add(new ProductDto()
        {
            ProductName="测试",
            Motion=1,
            Year="2023",
            UntilPrice="12"

        });

        return productDtos;
    }

    /// <summary>
    /// 用于对外报价NRE清单
    ///  /// <param name="auditFlowId"></param>
    /// <param name="version">报价方案版本</param>
    /// </summary>
    /// <returns></returns>
    public async Task<List<QuotationNreDto>> GetNREList(long auditFlowId,int version)
    {
        List<QuotationNreDto> productDtos = new List<QuotationNreDto>();
        productDtos.Add(new QuotationNreDto()
        {
            Product="测试",
            Pcs=1,
            shouban=12,
            moju=12,
            gzyj=12,
            sy=12,
            csrj=123,
            cl=122,
            qt=12
        });


        return productDtos;
    }

}