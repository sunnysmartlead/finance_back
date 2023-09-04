using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Authorization.Users;
using Finance.Dto;
using Finance.FinanceMaintain;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.Roles.Dto;
using Finance.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Authorization.Roles.StaticRoleNames;
using static Finance.MakeOffers.AnalyseBoard.Method.AnalysisBoardMethod;

namespace Finance.MakeOffers.AnalyseBoard
{
    /// <summary>
    /// 报价分析看板api
    /// </summary>
    [AbpAuthorize]
    public class AnalyseBoardAppService : FinanceAppServiceBase, IAnalyseBoardAppService
    {
        /// <summary>
        /// 分析看板方法
        /// </summary>
        public readonly AnalysisBoardMethod _analysisBoardMethod;
        /// <summary>
        /// Nre核价api
        /// </summary>
        public readonly NrePricingAppService _nrePricingAppService;
        /// <summary>
        /// 报价分析看板中的 产品单价表 实体类
        /// </summary>
        private readonly IRepository<UnitPriceOffers, long> _resourceUnitPriceOffers;
        /// <summary>
        /// 报价分析看板中的 汇总分析表  实体类
        /// </summary>
        private readonly IRepository<PooledAnalysisOffers, long> _resourcePooledAnalysisOffers;
        /// <summary>
        /// 报价分析看板中的 动态单价表 实体类
        /// </summary>
        private readonly IRepository<DynamicUnitPriceOffers, long> _resourceDynamicUnitPriceOffers;
        /// <summary>
        ///报价 项目看板实体类 实体类
        /// </summary>
        private readonly IRepository<ProjectBoardOffers, long> _resourceProjectBoardOffers;
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        /// 字典明细表
        /// </summary>
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        /// <summary>
        /// 报价审核表 中的  内部核价信息
        /// </summary>
        private readonly IRepository<InternalInformation, long> _financeInternalInformation;
        /// <summary>
        /// 报价审核表 中的 报价策略
        /// </summary>
        private readonly IRepository<BiddingStrategy, long> _financeBiddingStrategy;
        /// <summary>
        /// 报价审核表
        /// </summary>
        private readonly IRepository<AuditQuotationList, long> _financeAuditQuotationList;
        /// <summary>
        /// 审批流程主表
        /// </summary>
        private readonly IRepository<AuditFlow, long> _financeAuditFlow;
        /// <summary>
        /// 归档文件列表实体类
        /// </summary>
        private readonly IRepository<DownloadListSave, long> _financeDownloadListSave;
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        /// 文件管理接口
        /// </summary>
        private readonly FileCommonService _fileCommonService;
        /// <summary>
        /// 核价服务
        /// </summary>
        private readonly PriceEvaluationAppService _resourcePriceEvaluationAppService;
        private readonly IUserAppService _userAppService;
        /// 构造函数
        /// </summary>
        public AnalyseBoardAppService(AnalysisBoardMethod analysisBoardMethod,
            NrePricingAppService nrePricingAppService,
            IRepository<UnitPriceOffers, long> unitPriceOffers,
            IRepository<PooledAnalysisOffers, long> pooledAnalysisOffers,
            IRepository<DynamicUnitPriceOffers, long> DynamicUnitPriceOffers,
            IRepository<ProjectBoardOffers, long> projectBoardOffers,
            IRepository<ModelCount, long> resourceModelCount,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            IRepository<InternalInformation, long> financeInternalInformation,
            IRepository<BiddingStrategy, long> biddingStrategy,
            IRepository<AuditQuotationList, long> financeAuditQuotationList,
            AuditFlowAppService flowAppService,
            IRepository<AuditFlow, long> financeAuditFlow,
            IRepository<DownloadListSave, long> financeDownloadListSave,
            FileCommonService fileCommonService,
            PriceEvaluationAppService priceEvaluationAppService,
            IRepository<ModelCountYear, long> modelCountYear,
            IUserAppService userAppService)
        {
            _analysisBoardMethod = analysisBoardMethod;
            _nrePricingAppService = nrePricingAppService;
            _resourceUnitPriceOffers = unitPriceOffers;
            _resourcePooledAnalysisOffers = pooledAnalysisOffers;
            _resourceDynamicUnitPriceOffers = DynamicUnitPriceOffers;
            _resourceProjectBoardOffers = projectBoardOffers;
            _resourceModelCount = resourceModelCount;
            _financeDictionaryDetailRepository = financeDictionaryDetail;
            _financeInternalInformation = financeInternalInformation;
            _financeBiddingStrategy = biddingStrategy;
            _financeAuditQuotationList = financeAuditQuotationList;
            _flowAppService = flowAppService;
            _financeAuditFlow=financeAuditFlow;
            _financeDownloadListSave=financeDownloadListSave;
            _fileCommonService=fileCommonService;
            _resourcePriceEvaluationAppService=priceEvaluationAppService;
            _resourceModelCountYear=modelCountYear;
            _userAppService=userAppService;
        }
        /// <summary>
        /// 查看报表分析看板
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="GrossMarginId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AnalyseBoardDto> GetStatementAnalysisBoard(long Id, long? GrossMarginId)
        {
            AnalyseBoardDto analyseBoardDto = new AnalyseBoardDto();
            try
            {
                analyseBoardDto.NRE=await _nrePricingAppService.GetInitialSalesDepartment(Id);      
                List<UnitPriceCountModel> props = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id))
                                                   join b in await _financeDictionaryDetailRepository.GetAllListAsync() on a.ProductType equals b.Id
                                                   select new UnitPriceCountModel
                                                   {
                                                       ProductId=a.Id,
                                                       ProductType=a.ProductType,
                                                       ProductName=a.Product,
                                                       ProductNumber=a.SingleCarProductsQuantity,

                                                   }).ToList();
                foreach (UnitPriceCountModel item in props)
                {
                    ModelCountYear modelCountYear = new();
                    modelCountYear=_resourceModelCountYear.GetAllList(p => p.AuditFlowId.Equals(Id)&&p.ProductId.Equals(item.ProductId)).OrderBy(p => p.Year.ToString()).FirstOrDefault();
                    item.Unit=modelCountYear.TableJson!=null ? JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson).TotalCost : 0M;
                }
                analyseBoardDto.UnitPrice=await _analysisBoardMethod.CalculateUnitPriceModel(props, GrossMarginId);//计算单价表
                ProductBoardDtoOrEvery productBoardDtoOrEvery = await _analysisBoardMethod.BoardTwoFrom(props, Id);
                analyseBoardDto.ProductBoard= ObjectMapper.Map<DynamicUnitPrice>(productBoardDtoOrEvery);//计算 报价分析看板的 第二张表单           
                analyseBoardDto.PooledAnalysis=await _analysisBoardMethod.GetPooledAnalysis(GrossMarginId, Id);//汇总分析表
                var pro1p = await _analysisBoardMethod.GetProjectBoard(productBoardDtoOrEvery.AllInteriorGrossMargin/100, productBoardDtoOrEvery.AllClientGrossMargin/100, Id, productBoardDtoOrEvery.InteriorUnitPrice, productBoardDtoOrEvery.ClientUnitPrice, productBoardDtoOrEvery.OfferUnitPrice);//分析汇总表
                analyseBoardDto.ProjectBoard=pro1p;//分析汇总表
                return analyseBoardDto;            
            }
            catch (Exception e)
            {
                analyseBoardDto.IsSuccess=false;
                analyseBoardDto.Message="报价GetStatementAnalysisBoard方法"+e.Message;
                return analyseBoardDto;
            }

        }
        /// <summary>
        /// 报价分析看板 动态动态单价表单的计算
        /// </summary>
        /// <param name="productBoardDtos"></param>
        /// <returns></returns>
        public async Task<ProductBoardGrossMarginDto> PostCalculateFullGrossMargin(ProductBoardProcessDto productBoardDtos)
        {
            try
            {
                return await _analysisBoardMethod.CalculateFullGrossMargin(productBoardDtos);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
       /// <summary>
       /// 查看年份维度对比(全部模组)
       /// </summary>
       /// <param name="yearProductBoardProcessDto"></param>
       /// <returns></returns>
        public async Task<List<YearDimensionalityComparisonDto>> PostYearDimensionalityComparison(YearProductBoardProcessDto yearProductBoardProcessDto)
        {
            return await _analysisBoardMethod.YearDimensionalityComparison(yearProductBoardProcessDto.AuditFlowId, yearProductBoardProcessDto.GrossMargin, yearProductBoardProcessDto.ProductBoards);
        }
        /// <summary>
        /// 查看年份维度对比（各模组+整套）
        /// </summary>
        /// <param name = "productBoardProcessDto" ></param >
        /// <returns ></returns >
        public async Task<List<YearDimensionalityComparisonDto>> PostSomeYearDimensionalityComparison(YearSomeProductBoardProcessDto productBoardProcessDto)
        {
            return await _analysisBoardMethod.YearDimensionalityComparison(productBoardProcessDto.AuditFlowId, productBoardProcessDto.GrossMargin,new List<DynamicProductBoardModel>() { productBoardProcessDto.ProductBoard });
        }
        /// <summary>
        /// 汇总分析表 根据 整套 毛利率 计算(本次报价)
        /// </summary>
        public async Task<List<SpreadSheetCalculateDto>> PostSpreadSheetCalculate(ProductBoardProcessDto productBoardProcessDto)
        {
            try
            {
                return await _analysisBoardMethod.SpreadSheetCalculate(productBoardProcessDto.AuditFlowId, 1M, productBoardProcessDto.ProductBoards);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }

        }
        /// <summary>
        /// 下载成本信息表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetDownloadMessage(long Id, string FileName = "成本信息表下载")
        {
            try
            {
                return await _analysisBoardMethod.DownloadMessage(Id, FileName);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 报价接口
        /// </summary>
        /// <param name="isOfferDto"></param>
        /// <returns></returns>
        public async Task PostIsOffer(IsOfferDto isOfferDto)
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
                await PostIsOfferSave(isOfferDto);
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
        /// <summary>
        /// 报价 保存  接口
        /// </summary>
        /// <param name="isOfferDto"></param>
        /// <returns></returns>
        public async Task PostIsOfferSave(IsOfferDto isOfferDto)
        {
            //进行报价
            #region Nre 资源部录入 保存
            foreach (var item in isOfferDto.Nre)
            {
                item.AuditFlowId=isOfferDto.AuditFlowId;
            }
            await _nrePricingAppService.PostSalesDepartment(isOfferDto.Nre);
            #endregion
            #region 单价表添加           
            foreach (var unit in isOfferDto.UnitPrice)
            {
                UnitPriceOffers unitPriceOffer = await _resourceUnitPriceOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(isOfferDto.AuditFlowId)&&p.ProductId.Equals(unit.ProductId));
                if (unitPriceOffer is not null)
                {
                    unitPriceOffer.ProductName=unit.ProductName;
                    unitPriceOffer.ProductNumber=unit.ProductNumber;
                    unitPriceOffer.GrossMarginList=JsonConvert.SerializeObject(unit.GrossMarginList);
                    await _resourceUnitPriceOffers.UpdateAsync(unitPriceOffer);
                }
                else
                {
                    UnitPriceOffers unitPrice = new();
                    unitPrice.AuditFlowId=isOfferDto.AuditFlowId;
                    unitPrice.ProductId=unit.ProductId;
                    unitPrice.ProductName=unit.ProductName;
                    unitPrice.ProductNumber=unit.ProductNumber;
                    unitPrice.GrossMarginList=JsonConvert.SerializeObject(unit.GrossMarginList);
                    await _resourceUnitPriceOffers.InsertAsync(unitPrice);
                }
            }
            #endregion
            #region 汇总分析添加
            foreach (var pooled in isOfferDto.PooledAnalysis)
            {
                PooledAnalysisOffers pooledAnalysisOffers = await _resourcePooledAnalysisOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(isOfferDto.AuditFlowId)&&p.ProjectName.Equals(pooled.ProjectName));
                if (pooledAnalysisOffers is not null)
                {
                    pooledAnalysisOffers.GrossMarginList=JsonConvert.SerializeObject(pooled.GrossMarginList);
                    await _resourcePooledAnalysisOffers.UpdateAsync(pooledAnalysisOffers);
                }
                else
                {
                    PooledAnalysisOffers pool = new();
                    pool.AuditFlowId=isOfferDto.AuditFlowId;
                    pool.ProjectName=pooled.ProjectName;
                    pool.GrossMarginList=JsonConvert.SerializeObject(pooled.GrossMarginList);
                    await _resourcePooledAnalysisOffers.InsertAsync(pool);
                }
            }
            #endregion
            #region 动态单价表添加
            foreach (ProductBoardModel product in isOfferDto.ProductBoard.ProductBoard)
            {
                DynamicUnitPriceOffers dynamicUnitPriceOffers = await _resourceDynamicUnitPriceOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(isOfferDto.AuditFlowId)&&p.ProductId.Equals(product.ProductId));
                if (dynamicUnitPriceOffers is not null)
                {

                    dynamicUnitPriceOffers.ProductName=product.ProductName;
                    dynamicUnitPriceOffers.ProductNumber=product.ProductNumber;
                    dynamicUnitPriceOffers.InteriorTargetUnitPrice=product.InteriorTargetUnitPrice;
                    dynamicUnitPriceOffers.InteriorTargetGrossMargin=product.InteriorTargetGrossMargin;
                    dynamicUnitPriceOffers.ClientTargetUnitPrice=product.ClientTargetUnitPrice;
                    dynamicUnitPriceOffers.ClientTargetGrossMargin=product.ClientTargetGrossMargin;
                    dynamicUnitPriceOffers.OfferUnitPrice=product.OfferUnitPrice;
                    dynamicUnitPriceOffers.OffeGrossMargin=product.OffeGrossMargin;
                    dynamicUnitPriceOffers.AllInteriorGrossMargin=isOfferDto.ProductBoard.AllInteriorGrossMargin;
                    dynamicUnitPriceOffers.AllClientGrossMargin=isOfferDto.ProductBoard.AllClientGrossMargin;
                    await _resourceDynamicUnitPriceOffers.InsertOrUpdateAsync(dynamicUnitPriceOffers);
                }
                else
                {
                    DynamicUnitPriceOffers dynamic = new();
                    dynamic=ObjectMapper.Map<DynamicUnitPriceOffers>(product);
                    dynamic.AuditFlowId=isOfferDto.AuditFlowId;
                    dynamic.AllInteriorGrossMargin=isOfferDto.ProductBoard.AllInteriorGrossMargin;
                    dynamic.AllClientGrossMargin=isOfferDto.ProductBoard.AllClientGrossMargin;
                    await _resourceDynamicUnitPriceOffers.InsertAsync(dynamic);
                }
            }
            #endregion
            #region 项目看板添加
            foreach (var project1 in isOfferDto.ProjectBoard)
            {
                ProjectBoardOffers projectBoardOffers = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(isOfferDto.AuditFlowId)&&p.ProjectName.Equals(project1.ProjectName));
                if (projectBoardOffers is not null)
                {
                    projectBoardOffers.InteriorTarget=JsonConvert.SerializeObject(project1.InteriorTarget);
                    projectBoardOffers.ClientTarget=JsonConvert.SerializeObject(project1.ClientTarget);
                    projectBoardOffers.Offer=JsonConvert.SerializeObject(project1.Offer);
                    await _resourceProjectBoardOffers.UpdateAsync(projectBoardOffers);
                }
                else
                {
                    ProjectBoardOffers project = new();
                    project.AuditFlowId=isOfferDto.AuditFlowId;
                    project.ProjectName=project1.ProjectName;
                    project.InteriorTarget=JsonConvert.SerializeObject(project1.InteriorTarget);
                    project.ClientTarget=JsonConvert.SerializeObject(project1.ClientTarget);
                    project.Offer=JsonConvert.SerializeObject(project1.Offer);
                    await _resourceProjectBoardOffers.InsertAsync(project);
                }
            }
            #endregion
        }
        /// <summary>
        /// 查看 报价审核表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<QuotationListDto> GetQuotationList(long Id)
        {
            try
            {
                QuotationListDto quotationListDto = await _analysisBoardMethod.QuotationList(Id);
                return quotationListDto.RetainDecimals();
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 报价审核表 审批
        /// </summary>
        /// <param name="quotationListDto"></param>
        /// <returns></returns>
        public async Task PostAuditQuotationList(AuditQuotationListDto quotationListDto)
        {
            try
            {
                if (AbpSession.UserId is null)
                {
                    throw new FriendlyException("请先登录");
                }
                AuditFlowDetailDto flowDetailDto = new()
                {
                    AuditFlowId = quotationListDto.AuditFlowId,
                    ProcessIdentifier = AuditFlowConsts.AF_QuoteApproval,
                    UserId = AbpSession.UserId.Value,
                };
                if (quotationListDto.IsPass)
                {
                    flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
                    await _flowAppService.SetAuditFlowValid(new AuditFlowValidDto()
                    {
                        AuditFlowId = quotationListDto.AuditFlowId,
                        IsValid = true
                    });
                    await this.GetDownloadListSave(quotationListDto.AuditFlowId);
                }
                else
                {
                    if(quotationListDto.BackReason.IsNullOrEmpty())
                    {
                        throw new FriendlyException("拒绝原因不能为空");
                    }
                    //退回
                    flowDetailDto.Opinion = OPINIONTYPE.Reject;
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_GM_QuotationJudge + quotationListDto.BackReason;
                }
                ReturnDto returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 报价待审批审核表 的保存
        /// </summary>
        /// <param name="quotationListDto"></param>
        /// <returns></returns>
        public async Task PostAuditQuotationListSave(AuditQuotationListDto quotationListDto)
        {
            try
            {
                //将报价审核表 中的  内部核价信息 录入到数据库
                foreach (var pricing in quotationListDto.PricingMessage)
                {
                    InternalInformation internalInformation = await _financeInternalInformation.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(quotationListDto.AuditFlowId)&&p.ProductId.Equals(pricing.ProductId));
                    if (internalInformation is not null)
                    {
                        internalInformation.BOMCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.BOMCB)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.YieldCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZZCB)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.LossCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.SHCB)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.logisticsCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.WLCB)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.MOQCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.MOQFTCB)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.ElseCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.QT)).Select(p => p.CostValue).FirstOrDefault();
                        internalInformation.AllCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZCB)).Select(p => p.CostValue).FirstOrDefault();
                        await _financeInternalInformation.UpdateAsync(internalInformation);
                    }
                    else
                    {
                        await _financeInternalInformation.InsertAsync(new InternalInformation()
                        {
                            AuditFlowId=quotationListDto.AuditFlowId,
                            ProductId=pricing.ProductId,
                            BOMCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.BOMCB)).Select(p => p.CostValue).FirstOrDefault(),
                            YieldCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZZCB)).Select(p => p.CostValue).FirstOrDefault(),
                            LossCost=pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.SHCB)).Select(p => p.CostValue).FirstOrDefault(),
                            logisticsCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.WLCB)).Select(p => p.CostValue).FirstOrDefault(),
                            MOQCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.MOQFTCB)).Select(p => p.CostValue).FirstOrDefault(),
                            ElseCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.QT)).Select(p => p.CostValue).FirstOrDefault(),
                            AllCost = pricing.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZCB)).Select(p => p.CostValue).FirstOrDefault(),

                        });
                    }
                }
                //报价审核表 中的 报价策略
                foreach (BiddingStrategyModel bidding in quotationListDto.BiddingStrategy)
                {
                    BiddingStrategy biddingStrategy = await _financeBiddingStrategy.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(quotationListDto.AuditFlowId)&&p.ProductId.Equals(bidding.ProductId));
                    if (biddingStrategy is not null)
                    {
                        biddingStrategy.Product=bidding.Product;
                        biddingStrategy.Cost=bidding.Cost;
                        biddingStrategy.GrossMargin=bidding.GrossMargin;
                        biddingStrategy.Price=bidding.Price;
                        biddingStrategy.Commission=bidding.Commission;
                        biddingStrategy.GrossMarginCommission=bidding.GrossMarginCommission;
                        await _financeBiddingStrategy.UpdateAsync(biddingStrategy);
                    }
                    else
                    {
                        await _financeBiddingStrategy.InsertAsync(
                      new BiddingStrategy()
                      {
                          AuditFlowId=quotationListDto.AuditFlowId,
                          ProductId=bidding.ProductId,
                          Product=bidding.Product,
                          Cost=bidding.Cost,
                          GrossMargin=bidding.GrossMargin,
                          Price=bidding.Price,
                          Commission=bidding.Commission,
                          GrossMarginCommission=bidding.GrossMarginCommission,
                      });
                    }
                }
                //将整块数据打包成 josn 存入 数据库
                AuditQuotationList auditQuotationList = await _financeAuditQuotationList.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(quotationListDto.AuditFlowId));
                if (auditQuotationList is not null)
                {
                    auditQuotationList.AuditQuotationListJson=JsonConvert.SerializeObject(quotationListDto);
                    await _financeAuditQuotationList.UpdateAsync(auditQuotationList);
                }
                else
                {
                    await _financeAuditQuotationList.InsertAsync(new AuditQuotationList()
                    {
                        AuditFlowId = quotationListDto.AuditFlowId,
                        AuditQuotationListJson=JsonConvert.SerializeObject(quotationListDto),
                    });
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 报价审核表 下载
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDownloadAuditQuotationList(long Id, string FileName = "报价审核表下载")
        {
            try
            {
                return await _analysisBoardMethod.DownloadAuditQuotationList(Id, FileName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 报价审核表 下载--流
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetDownloadAuditQuotationListStream(long Id, string FileName = "报价审核表下载")
        {
            try
            {
                return await _analysisBoardMethod.DownloadAuditQuotationListStream(Id, FileName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 归档文件列表保存
        /// </summary>
        /// <returns></returns>
        public async Task GetDownloadListSave(long auditFlow)
        {
            string FileName = "";
            AuditFlow audit = await _financeAuditFlow.FirstOrDefaultAsync(p => p.Id.Equals(auditFlow));
            //循环模组
            List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlow));
            foreach (ModelCount modelCount in modelCounts)
            {
                #region 某个模组核价表全生命周期 文件保存
                //核价表全生命周期
                MemoryStream memoryStreamPricingAll = await _resourcePriceEvaluationAppService.PriceEvaluationTableDownloadStream(new PriceEvaluationTableDownloadStreamInput() { AuditFlowId=auditFlow, ProductId=modelCount.Id, IsAll=true });
                //将核价表全生命周期保存到硬盘中
                FileName="核价表全生命周期.xlsx";
                IFormFile fileAll = new FormFile(memoryStreamPricingAll, 0, memoryStreamPricingAll.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoAll = await _fileCommonService.UploadFile(fileAll);
                //核价表全生命周期的路径和名称保存到
                await _financeDownloadListSave.InsertAsync(new DownloadListSave() { AuditFlowId=auditFlow, QuoteProjectName=audit.QuoteProjectName, ProductName=modelCount.Product, ProductId=modelCount.Id, FileName=FileName, FilePath=fileUploadOutputDtoAll.FileUrl, FileId=fileUploadOutputDtoAll.FileId });
                #endregion
                #region 某个模组核价表全生命周期 文件保存
                //核价表
                MemoryStream memoryStreamPricing = await _resourcePriceEvaluationAppService.PriceEvaluationTableDownloadStream(new PriceEvaluationTableDownloadStreamInput() { AuditFlowId=auditFlow, ProductId=modelCount.Id, IsAll=false });
                //将核价表全保存到硬盘中
                FileName="核价表.xlsx";
                IFormFile file = new FormFile(memoryStreamPricing, 0, memoryStreamPricing.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDto = await _fileCommonService.UploadFile(file);
                //核价表全的路径和名称保存到
                await _financeDownloadListSave.InsertAsync(new DownloadListSave() { AuditFlowId=auditFlow, QuoteProjectName=audit.QuoteProjectName, ProductName=modelCount.Product, ProductId=modelCount.Id, FileName=FileName, FilePath=fileUploadOutputDto.FileUrl, FileId=fileUploadOutputDto.FileId });
                #endregion
                #region 某个模组 Nre 核价表 文件保存
                //核价表
                MemoryStream memoryStreamNre = null;// await _resourcePriceEvaluationAppService.NreTableDownloadStream(new NreTableDownloadInput() { AuditFlowId=auditFlow, ProductId=modelCount.Id });
                //将核价表保存到硬盘中
                FileName="Nre核价表.xlsx";
                IFormFile fileNre = new FormFile(memoryStreamNre, 0, memoryStreamNre.Length, FileName, FileName);
                FileUploadOutputDto fileUploadOutputDtoNre = await _fileCommonService.UploadFile(fileNre);
                //核价表的路径和名称保存到
                await _financeDownloadListSave.InsertAsync(new DownloadListSave() { AuditFlowId=auditFlow, QuoteProjectName=audit.QuoteProjectName, ProductName=modelCount.Product, ProductId=modelCount.Id, FileName=FileName, FilePath=fileUploadOutputDtoNre.FileUrl, FileId=fileUploadOutputDtoNre.FileId });
                #endregion
            }
            #region 报价审核表 文件保存
            //报价审核表
            MemoryStream memoryStreamOffer = await GetDownloadAuditQuotationListStream(auditFlow);
            //将报价审核表保存到硬盘中
            FileName="报价审核表.xlsx";
            IFormFile fileOffer = new FormFile(memoryStreamOffer, 0, memoryStreamOffer.Length, FileName, FileName);
            FileUploadOutputDto fileUploadOutputDtoOffer = await _fileCommonService.UploadFile(fileOffer);
            //报价审核表的路径和名称保存到
            if (audit is not null) await _financeDownloadListSave.InsertAsync(new DownloadListSave() { AuditFlowId=auditFlow, QuoteProjectName=audit.QuoteProjectName, ProductName="", ProductId=0, FileName=FileName, FilePath=fileUploadOutputDtoOffer.FileUrl, FileId=fileUploadOutputDtoOffer.FileId });
            #endregion
        }
        /// <summary>
        /// 归档文件列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<PigeonholeDownloadTableModel>> GetDownloadList(long? auditFlowId)
        {            
            ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());          
            try
            {
                List<PigeonholeDownloadTableModel> pigeonholeDownloadTableModels = new();
                List<DownloadListSave> downloadListSaves = new();
                List<RoleDto> generalManager = Roles.Items.Where(p => p.Name.Equals(Host.GeneralManager)).ToList();//总经理
                if (generalManager.Count is not 0)
                {
                    if (auditFlowId is not null)
                    {
                        downloadListSaves= await _financeDownloadListSave.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
                    }
                    else
                    {
                        downloadListSaves= await _financeDownloadListSave.GetAllListAsync();
                    }
                }
                else
                {
                    List<RoleDto> PM = Roles.Items.Where(p => p.Name.Equals(Host.ProjectManager)).ToList();//项目经理
                    if (PM.Count is not 0)
                    {
                        if (auditFlowId is not null)
                        {
                            downloadListSaves= await _financeDownloadListSave.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.FileName.Contains("核价表"));
                        }
                        else
                        {
                            downloadListSaves= await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("核价表"));
                        }
                    }
                    List<RoleDto> salesDepartment = Roles.Items.Where(p => p.Name.Equals(Host.SalesMan)).ToList();//营销部-业务员
                    if (salesDepartment.Count is not 0)
                    {
                        if (auditFlowId is not null)
                        {
                            downloadListSaves= await _financeDownloadListSave.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.FileName.Contains("报价审核表"));
                        }
                        else
                        {
                            downloadListSaves= await _financeDownloadListSave.GetAllListAsync(p => p.FileName.Contains("报价审核表"));
                        }
                    }
                }
                foreach (DownloadListSave item in downloadListSaves)
                {
                    PigeonholeDownloadTableModel pigeonholeDownloadTableModel = new();
                    pigeonholeDownloadTableModel.DownloadListSaveId=item.Id;// 归档文件列表id
                    pigeonholeDownloadTableModel.FileName=item.FileName;// 归档文件名称
                    pigeonholeDownloadTableModel.ProductName=item.ProductName;// 零件名称
                    pigeonholeDownloadTableModel.QuoteProjectName=item.QuoteProjectName;// 核报价项目名称
                    pigeonholeDownloadTableModels.Add(pigeonholeDownloadTableModel);
                }
                return pigeonholeDownloadTableModels;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

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
            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "归档文件下载.zip" };
        }
    }
}
