using Abp;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Finance.Audit;
using Finance.EngineeringDepartment;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NerPricing;
using Finance.Nre;
using Finance.NrePricing;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Model;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Method
{
    /// <summary>
    /// 分析看板方法
    /// </summary>
    public class AnalysisBoardMethod : AbpServiceBase, ISingletonDependency
    {
        /// <summary>
        /// 财务维护 毛利率方案
        /// </summary>
        private readonly IRepository<GrossMarginForm, long> _resourceGrossMarginForm;
        /// <summary>
        /// 要求
        /// </summary>
        private readonly IRepository<Requirement, long> _resourceRequirement;
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 模组数量年份
        /// </summary>
        private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        /// 字典明细表
        /// </summary>
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        /// <summary>
        /// 核价表需求表
        /// </summary>
        private readonly IRepository<PriceEvaluation, long> _resourcePriceEvaluation;
        /// <summary>
        /// 汇率录入表
        /// </summary>
        private readonly IRepository<ExchangeRate, long> _resourceExchangeRate;
        /// <summary>
        /// 审批流程主表
        /// </summary>
        private readonly IRepository<AuditFlow, long> _resourceAuditFlow;
        /// <summary> 
        /// Nre 资源部 模具清单实体类
        /// </summary>
        private readonly IRepository<MouldInventory, long> _resourceMouldInventory;
        /// <summary>
        /// 设备部分表
        /// </summary>
        private readonly IRepository<EquipmentInfo, long> _resourceEquipmentInfo;
        /// <summary>
        /// 追溯部分表(硬件及软件开发费用)
        /// </summary>
        private readonly IRepository<WorkingHoursInfo, long> _resourceWorkingHoursInfo;
        /// <summary>
        /// Nre 项目管理部 其他费用实体类
        /// </summary>
        private readonly IRepository<RestsCost, long> _resourceRestsCost;
        /// <summary> 
        /// Nre 产品部-电子工程师 实验费 实体类
        /// </summary>
        private readonly IRepository<LaboratoryFee, long> _resourceLaboratoryFee;
        /// <summary>
        /// Nre 品保录入 实验项目 实体类
        /// </summary>
        private readonly IRepository<QADepartmentTest, long> _resourceQADepartmentTest;
        /// <summary>
        /// Nre 项目管理部 差旅费实体类
        /// </summary>
        private readonly IRepository<TravelExpense, long> _resourceTravelExpense;
        /// <summary>
        /// Nre 营销部录入 实体类
        /// </summary>
        private readonly IRepository<InitialResourcesManagement, long> _resourceResourcesManagement;
        /// <summary>
        /// 产品信息表
        /// </summary>
        private readonly IRepository<ProductInformation, long> _resourceProductInformation;
        /// <summary>
        /// 制造成本里计算字段参数维护
        /// </summary>
        private readonly IRepository<ManufacturingCostInfo, long> _resourceManufacturingCostInfo;
        /// <summary>
        /// 产品开发部结构BOM输入信息
        /// </summary>
        private readonly IRepository<StructureBomInfo, long> _resourceStructureBomInfo;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        private readonly IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;
        /// <summary>
        /// 报价分析看板中的 动态单价表 实体类
        /// </summary>
        private readonly IRepository<DynamicUnitPriceOffers, long> _resourceDynamicUnitPriceOffers;
        /// <summary>
        ///报价 项目看板实体类 实体类
        /// </summary>
        private readonly IRepository<ProjectBoardOffers, long> _resourceProjectBoardOffers;
        /// <summary>
        /// 报价审核表 中的 报价策略
        /// </summary>
        private readonly IRepository<BiddingStrategy, long> _financeBiddingStrategy;
        /// <summary>
        /// 报价审核表
        /// </summary>
        private readonly IRepository<AuditQuotationList, long> _financeAuditQuotationList;
        /// <summary>
        ///Nre 品保录入  项目制程QC量检具表  实体类
        /// </summary>
        private readonly IRepository<QADepartmentQC, long> _resourceQADepartmentQC;
        /// <summary>
        /// Nre 项目管理部 手板件实体类
        /// </summary>
        private readonly IRepository<HandPieceCost, long> _resourceHandPieceCost;    
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnalysisBoardMethod(
            IRepository<GrossMarginForm, long> grossMarginForm,
            IRepository<Requirement, long> requirement,
            IRepository<ModelCount, long> modelCount,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            IRepository<PriceEvaluation, long> priceEvaluation,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<ExchangeRate, long> exchangeRate,
            IRepository<AuditFlow, long> auditFlow,
            IRepository<MouldInventory, long> mouldInventory,
            IRepository<EquipmentInfo, long> equipmentInfo,
            IRepository<WorkingHoursInfo, long> workingHoursInfo,
            IRepository<RestsCost, long> restsCost,
            IRepository<LaboratoryFee, long> laboratoryFee,
            IRepository<QADepartmentTest, long> qADepartmentTest,
            IRepository<TravelExpense, long> travelExpense,
            IRepository<InitialResourcesManagement, long> initialResourcesManagement,
            IRepository<ProductInformation, long> productInformation,
            IRepository<ManufacturingCostInfo, long> manufacturingCostInfo,
            IRepository<StructureBomInfo, long> structureBomInfo,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            IRepository<DynamicUnitPriceOffers, long> dynamicUnitPriceOffers,
            IRepository<ProjectBoardOffers, long> resourceProjectBoardOffers,
            IRepository<BiddingStrategy, long> financeBiddingStrategy,
            IRepository<AuditQuotationList, long> financeAuditQuotationList,
            IRepository<QADepartmentQC, long> resourceQADepartmentQC,
            IRepository<HandPieceCost, long> handPieceCost)
        {
            _resourceGrossMarginForm = grossMarginForm;
            _resourceRequirement = requirement;
            _resourceModelCount = modelCount;
            _financeDictionaryDetailRepository = financeDictionaryDetail;
            _resourcePriceEvaluation = priceEvaluation;
            _resourceModelCountYear = modelCountYear;
            _resourceExchangeRate = exchangeRate;
            _resourceAuditFlow = auditFlow;
            _resourceMouldInventory = mouldInventory;
            _resourceEquipmentInfo = equipmentInfo;
            _resourceWorkingHoursInfo = workingHoursInfo;
            _resourceRestsCost = restsCost;
            _resourceLaboratoryFee = laboratoryFee;
            _resourceQADepartmentTest = qADepartmentTest;
            _resourceTravelExpense = travelExpense;
            _resourceResourcesManagement = initialResourcesManagement;
            _resourceProductInformation = productInformation;
            _resourceManufacturingCostInfo = manufacturingCostInfo;
            _resourceStructureBomInfo = structureBomInfo;
            _resourceElectronicBomInfo = electronicBomInfo;
            _resourceDynamicUnitPriceOffers = dynamicUnitPriceOffers;
            _resourceProjectBoardOffers = resourceProjectBoardOffers;
            _financeBiddingStrategy = financeBiddingStrategy;
            _financeAuditQuotationList = financeAuditQuotationList;
            _resourceQADepartmentQC = resourceQADepartmentQC;
            _resourceHandPieceCost= handPieceCost;          
        }

        /// <summary>
        /// 计算单价表
        /// </summary>
        /// <param name="unitPrice">单价表</param>
        /// <param name="GrossMarginId">毛利率表的id</param>
        /// <returns></returns>
        public async Task<List<UnitPriceModel>> CalculateUnitPriceModel(List<UnitPriceCountModel> unitPrice, long? GrossMarginId)
        {

            List<UnitPriceModel> unitPriceModels = new List<UnitPriceModel>();
            GrossMarginForm price = new();
            if (GrossMarginId.Equals(null))//判断 前端 时候传 毛利率id  如果不传 则取默认
            {
                price = await _resourceGrossMarginForm.FirstOrDefaultAsync(p => p.IsDefaultn);
            }
            else
            {
                price = await _resourceGrossMarginForm.FirstOrDefaultAsync(p => p.Id.Equals(GrossMarginId));
            }
            GrossMarginDto gross = ObjectMapper.Map<GrossMarginDto>(price);
            foreach (var unitPriceItem in unitPrice)
            {
                //返回类型
                UnitPriceModel unitPriceModel = new UnitPriceModel();
                unitPriceModel.ModelCountId = unitPriceItem.ModelCountId;
                unitPriceModel.ProductName = unitPriceItem.ProductName;
                unitPriceModel.ProductNumber = unitPriceItem.ProductNumber;
                unitPriceModel.GrossMarginList = GrossMarginList(gross.GrossMarginPrice, unitPriceItem);//毛利率的计算
                unitPriceModels.Add(unitPriceModel);
            }
            return unitPriceModels;
        }
        /// <summary>
        /// 计算 UnitPriceModel.GrossMarginList 毛利率
        /// </summary>
        private List<GrossMarginModel> GrossMarginList(List<decimal> p, UnitPriceCountModel m)
        {
            List<GrossMarginModel> grossMarginList = new List<GrossMarginModel>();
            foreach (decimal item in p)
            {
                GrossMarginModel grossMarginModel = new GrossMarginModel();
                grossMarginModel.GrossMargin = item;
                decimal deci = m.Unit / (1 - item / 100);
                grossMarginModel.GrossMarginNumber = deci;
                grossMarginList.Add(grossMarginModel);
            }
            return grossMarginList;
        }
        /// <summary>
        /// 报价分析看板的 第二张表单
        /// </summary>
        /// <param name="unitPrice">单价</param>      
        ///  <param name="processId">单价</param>   
        public async Task<ProductBoardDtoOrEvery> BoardTwoFrom(List<UnitPriceCountModel> unitPrice, long processId)
        {
            ProductBoardDtoOrEvery productBoardDto = new ProductBoardDtoOrEvery();
            List<int> yearList = await GetYear(processId);
            List<ProductBoardModel> productBoardModels = new List<ProductBoardModel>();
            //每个模组全生命周期  返利后销售收入与收入 总和
            decimal AllRebateSalesRevenueFull = 0.0M;
            //每个模组全生命周期 本次报价  返利后销售收入与收入 总和
            decimal AllOfferRebateSalesRevenueFull = 0.0M;
            //每个模组全生命周期  销售毛利 总和
            decimal AllTradingProfitFull = 0.0M;
            //每个模组全生命周期 本次报价  销售毛利 总和
            decimal AllOfferProfitFull = 0.0M;
            //全生命周期的单价
            decimal AllUnitInteriorTargetUnitPrice = 0.0M;
            // 目标价(内部) 整套 单价
            decimal AllInteriorUnitPrice = 0.0M;
            // 目标价(客户) 整套 单价
            decimal AllClientUnitPrice = 0.0M;
            List<DynamicProductBoardModel> InteriorUnitPrice = new List<DynamicProductBoardModel>();
            List<DynamicProductBoardModel> ClientUnitPrice = new List<DynamicProductBoardModel>();
            List<DynamicProductBoardModel> OfferUnitPrice = new List<DynamicProductBoardModel>();
            foreach (UnitPriceCountModel unit in unitPrice)
            {
                ProductBoardModel productBoardModel = new ProductBoardModel();
                productBoardModel.ProductName = unit.ProductName;// 产品名称
                productBoardModel.ProductNumber = unit.ProductNumber;//单车产品数量
                productBoardModel.ModelCountId = unit.ModelCountId;//模组数量主键
                //报价分析看板中的 动态单价表 
                DynamicUnitPriceOffers dynamicUnitPriceOffer = await _resourceDynamicUnitPriceOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(productBoardModel.ModelCountId));
                decimal GrossMargin = 0.0M;
                if (unit.ProductType == FinanceConsts.ProductType_ExternalImaging)
                {
                    GrossMargin = (decimal)TargetPriceRule.peripheralDisplays / 100;
                }
                else if (unit.ProductType == FinanceConsts.ProductType_EnvironmentalPerception)
                {
                    GrossMargin = (decimal)TargetPriceRule.lookAround / 100;
                }
                else if (unit.ProductType == FinanceConsts.ProductType_CabinMonitoring)
                {
                    GrossMargin = (decimal)TargetPriceRule.cabinMonitor / 100;
                }
                else
                {
                    GrossMargin = 0.0M;
                }
                productBoardModel.InteriorTargetGrossMargin = GrossMargin * 100;//目标毛利率(内部)
                productBoardModel.InteriorTargetUnitPrice = unit.Unit / (1 - GrossMargin);//目标单价(内部)
                InteriorUnitPrice.Add(new DynamicProductBoardModel() { ModelCountId = unit.ModelCountId, UnitPrice = unit.Unit / (1 - GrossMargin) });
                var prop = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(unit.ModelCountId))
                            join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                            select new
                            {
                                id = a.Id,
                                CustomerTargetPrice = b.CustomerTargetPrice
                            }).FirstOrDefault();
                #region 目标价客户               
                if (dynamicUnitPriceOffer is not null)
                {
                    productBoardModel.ClientTargetUnitPrice= dynamicUnitPriceOffer.ClientTargetUnitPrice;//取数据库中的 客户目标价的 单价
                }
                else
                {
                    productBoardModel.ClientTargetUnitPrice = prop.CustomerTargetPrice != 0 ? prop.CustomerTargetPrice : 0M;//取第一张表的 单颗产品目标价
                }
                #endregion                
                ClientUnitPrice.Add(new DynamicProductBoardModel()
                {
                    ModelCountId = unit.ModelCountId,
                    UnitPrice = productBoardModel.ClientTargetUnitPrice
                });
                //计算全生命周期的 返利后销售收入与收入
                decimal AllRebateSalesRevenue = 0.0M;
                //计算全生命周期的 销售毛利
                decimal AllTradingProfit = 0.0M;
                AllUnitInteriorTargetUnitPrice += productBoardModel.InteriorTargetUnitPrice;
                #region          
                decimal lastYearUnitPrice = productBoardModel.ClientTargetUnitPrice;
                AllClientUnitPrice += productBoardModel.ClientTargetUnitPrice;
                AllInteriorUnitPrice += productBoardModel.InteriorTargetUnitPrice;
                //循环所有的年份               
                foreach (int item in yearList)
                {
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, productBoardModel.ModelCountId, 1M, item, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    decimal RebateSalesRevenueValue = await RebateSalesRevenue(processId, productBoardModel.ModelCountId, 0.0M, item, lastYearUnitPrice, lastYearUnitPrice);
                    //返利后销售收入与收入
                    AllRebateSalesRevenue += RebateSalesRevenueValue;
                    AllTradingProfit += await TradingProfit(processId, productBoardModel.ModelCountId, 0.0M, item, 0.0M, lastYearUnitPrice);
                }
                #endregion
                #region 本次报价       
                //计算全生命周期的 本次报价 返利后销售收入与收入
                decimal AllOfferRebateSalesRevenue = 0.0M;
                //计算全生命周期的 本次报价 销售毛利
                decimal AllOfferTradingProfit = 0.0M;
                if (dynamicUnitPriceOffer is not null)
                {
                    productBoardModel.OfferUnitPrice = dynamicUnitPriceOffer.OfferUnitPrice;//本次报价的单价
                    productBoardModel.OffeGrossMargin = dynamicUnitPriceOffer.OffeGrossMargin;//本次报价的毛利率
                    OfferUnitPrice.Add(new DynamicProductBoardModel() { UnitPrice = dynamicUnitPriceOffer.OfferUnitPrice, ModelCountId = unit.ModelCountId });
                    decimal OfferlastYearUnitPrice = dynamicUnitPriceOffer.OfferUnitPrice;
                    //循环所有的年份               
                    foreach (int item in yearList)
                    {
                        //售价(单价)
                        var sellingPrice = await SellingPrice(processId, productBoardModel.ModelCountId, 1M, item, OfferlastYearUnitPrice, 0.0M);
                        OfferlastYearUnitPrice = sellingPrice;
                        decimal RebateSalesRevenueValue = await RebateSalesRevenue(processId, productBoardModel.ModelCountId, 0.0M, item, OfferlastYearUnitPrice, OfferlastYearUnitPrice);
                        //返利后销售收入与收入
                        AllOfferRebateSalesRevenue += RebateSalesRevenueValue;
                        AllOfferTradingProfit += await TradingProfit(processId, productBoardModel.ModelCountId, 0.0M, item, 0.0M, OfferlastYearUnitPrice);
                    }
                }
                #endregion
                productBoardModel.ClientTargetGrossMargin = (AllTradingProfit / AllRebateSalesRevenue) * 100;

                #region 前几个版本
                List<OneGoNOffer> oneGoNOffers = new List<OneGoNOffer>();
                List<AuditFlow> auditFlows = (from a in await _resourceAuditFlow.GetAllListAsync(p => p.Id.Equals(processId))
                                              join c in await _resourceAuditFlow.GetAllListAsync(p =>p.Id<processId) on a.QuoteProjectName equals c.QuoteProjectName
                                              select new AuditFlow
                                              {
                                                  Id = c.Id,
                                                  QuoteVersion = c.QuoteVersion,
                                                  QuoteProjectNumber = c.QuoteProjectNumber,
                                                  QuoteProjectName = c.QuoteProjectName,
                                                  IsValid = c.IsValid,
                                                  Remarks = c.Remarks,
                                              }).OrderBy(p=>p.Id).ToList();
                foreach (AuditFlow audit in auditFlows)
                {
                    OneGoNOffer oneGoNOffer = new OneGoNOffer();
                    DynamicUnitPriceOffers dynamicUnitPriceOffers = await _resourceDynamicUnitPriceOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProductName.Equals(unit.ProductName));
                    if (dynamicUnitPriceOffers is not null) oneGoNOffer.UnitPrice = dynamicUnitPriceOffers.OfferUnitPrice;
                    if (dynamicUnitPriceOffers is not null) oneGoNOffer.GrossMargin = dynamicUnitPriceOffers.OffeGrossMargin;
                    oneGoNOffers.Add(oneGoNOffer);
                }
                productBoardModel.OldOffer = oneGoNOffers;
                #endregion
                AllRebateSalesRevenueFull += AllRebateSalesRevenue;

                AllOfferRebateSalesRevenueFull += AllOfferRebateSalesRevenue;

                AllTradingProfitFull += AllTradingProfit;
                AllOfferProfitFull += AllOfferTradingProfit;
                productBoardModels.Add(productBoardModel);
            }
            decimal AllUnit = 0M;//全生命周期的单位成本
            List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
            foreach (ModelCount modelCount in modelCounts)
            {
                PriceEvaluationTableDto priceEvaluationTableDto = new();
                if (!string.IsNullOrWhiteSpace(modelCount.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCount.TableJson);
                if (priceEvaluationTableDto.TotalCost is not 0.0M) AllUnit += priceEvaluationTableDto.TotalCost;
            }

            if (AllUnitInteriorTargetUnitPrice is not 0.0M) productBoardDto.AllInteriorGrossMargin = (1 - AllUnit / AllUnitInteriorTargetUnitPrice) * 100;
            if (AllRebateSalesRevenueFull is not 0.0M) productBoardDto.AllClientGrossMargin = (AllTradingProfitFull / AllRebateSalesRevenueFull) * 100;         
            if(AllOfferRebateSalesRevenueFull is not  0.0M)productBoardDto.AllOfferGrossMargin= (AllOfferProfitFull / AllOfferRebateSalesRevenueFull) * 100;         
            productBoardDto.ProductBoard = productBoardModels;
            productBoardDto.AllRebateSalesRevenueFull = AllRebateSalesRevenueFull;
            productBoardDto.AllTradingProfitFull = AllTradingProfitFull;         
            // 动态单价表目标价客户的单价
            productBoardDto.ClientUnitPrice = ClientUnitPrice;
            // 动态单价表目标价内部的单价
            productBoardDto.InteriorUnitPrice = InteriorUnitPrice;
            // 动态单价表本次报价的单价
            productBoardDto.OfferUnitPrice = OfferUnitPrice;
            return productBoardDto;
        }
        /// <summary>
        /// 汇总分析表
        /// </summary>     
        /// <param name="GrossMarginId"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public async Task<List<PooledAnalysisModel>> GetPooledAnalysis(long? GrossMarginId, long processId)
        {
            //获取毛利率
            #region
            List<UnitPriceModel> unitPriceModels = new List<UnitPriceModel>();
            GrossMarginForm price = new();
            if (GrossMarginId.Equals(null))//判断 前端 时候传 毛利率id  如果不传 则取默认
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
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                grossMarginModel.GrossMarginNumber = _resourceModelCount.GetAllList().Where(p => p.AuditFlowId.Equals(processId)).Sum(p => p.ModelTotal);
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
            pooledAnalysisModel.ProjectName = "销售成本";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                grossMarginModel.GrossMarginNumber = await AllsellingCost(processId);
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
            pooledAnalysisModel.ProjectName = "单位平均成本";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                //该 毛利率的 销售数量
                decimal GrossMarginNumberSL = result[0].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                //该 毛利率的 销售成本
                decimal GrossMarginNumberxSCB = result[1].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                if (GrossMarginNumberSL is not 0.0M) grossMarginModel.GrossMarginNumber = GrossMarginNumberxSCB / GrossMarginNumberSL;
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
            pooledAnalysisModel.ProjectName = "返利后销售收入";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
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
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
            pooledAnalysisModel.ProjectName = "平均单价";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                //该 毛利率的  销售收入
                decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                //该 毛利率的 数量
                decimal GrossMarginNumberSL = result[0].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();

                if (GrossMarginNumberSL is not 0.0M) grossMarginModel.GrossMarginNumber = GrossMarginNumberSR / GrossMarginNumberSL;
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });

            pooledAnalysisModel.ProjectName = "销售毛利";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                //该 毛利率的  销售收入
                decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                //该 毛利率的 销售成本
                decimal GrossMarginNumberCB = result[1].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                //该 毛利率 全生命周期的返利金额
                decimal ALLRebateMoney = await AllRebateMoney(processId, item);
                //该 毛利率 全生命周期的税费损失
                decimal ALLAddTaxRate = await AllAddTaxRate(processId, item);
                grossMarginModel.GrossMarginNumber = GrossMarginNumberSR - GrossMarginNumberCB - ALLRebateMoney - ALLAddTaxRate;
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });

            pooledAnalysisModel.ProjectName = "毛利率";
            grossMarginModels.Clear();
            foreach (decimal item in gross.GrossMarginPrice)//循环毛利率
            {
                GrossMarginModel grossMarginModel = new();
                grossMarginModel.GrossMargin = item;
                //该 毛利率的  销售毛利
                decimal GrossMarginNumberML = result[5].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();
                //该 毛利率的 销售收入
                decimal GrossMarginNumberSR = result[3].GrossMarginList.Where(p => p.GrossMargin.Equals(item)).Select(p => p.GrossMarginNumber).First();

                if (GrossMarginNumberSR is not 0.0M) grossMarginModel.GrossMarginNumber = (GrossMarginNumberML / GrossMarginNumberSR) * 100;
                grossMarginModels.Add(grossMarginModel);
            }
            pooledAnalysisModel.GrossMarginList = JsonConvert.DeserializeObject<List<GrossMarginModel>>(JsonConvert.SerializeObject(grossMarginModels));
            result.Add(new PooledAnalysisModel() { ProjectName = pooledAnalysisModel.ProjectName, GrossMarginList = pooledAnalysisModel.GrossMarginList });
            return result;
        }
        /// <summary>
        /// 分析汇总表
        /// </summary>
        /// <param name="InteriorTargetGrossMargin">内部价(内部)整套毛利率</param>
        /// <param name="ClientTargetGrossMargin">目标价(客户)整套毛利率</param>
        /// <param name="processId">流程id</param>
        /// <returns></returns>
        public async Task<List<ProjectBoardModel>> GetProjectBoard(decimal InteriorTargetGrossMargin, decimal ClientTargetGrossMargin, long processId, List<DynamicProductBoardModel> InteriorUnitPrice, List<DynamicProductBoardModel> ClientUnitPrice, List<DynamicProductBoardModel> OfferUnitPrice)
        {
            List<ProjectBoardModel> projectBoardModels = new();
            ProjectBoardModel projectBoardModel = new();
            List<AuditFlow> auditFlows = (from a in await _resourceAuditFlow.GetAllListAsync(p => p.Id.Equals(processId))
                                          join c in await _resourceAuditFlow.GetAllListAsync(p => p.Id < processId) on a.QuoteProjectName equals c.QuoteProjectName
                                          select new AuditFlow
                                          {
                                              Id = c.Id,
                                              QuoteVersion = c.QuoteVersion,
                                              QuoteProjectNumber = c.QuoteProjectNumber,
                                              QuoteProjectName = c.QuoteProjectName,
                                              IsValid = c.IsValid,
                                              Remarks = c.Remarks,
                                          }).OrderBy(p => p.Id).ToList();
            projectBoardModel.ProjectName = "数量";
            decimal modelTotal = _resourceModelCount.GetAllList().Where(p => p.AuditFlowId.Equals(processId)).Sum(p => p.ModelTotal);
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = modelTotal };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = ClientTargetGrossMargin, GrossMarginNumber = modelTotal };
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer= new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = modelTotal };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  数量  
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if(projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }               
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));

            projectBoardModel.ProjectName = "销售成本";
            decimal Targetvalue = await AllsellingCost(processId);
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = Targetvalue };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = ClientTargetGrossMargin, GrossMarginNumber = Targetvalue };
            if(OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = Targetvalue };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  销售成本        
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));
            projectBoardModel.ProjectName = "单位平均成本";
            decimal InteriorTargetValue = 0.0M;
            if (projectBoardModels[0].InteriorTarget.GrossMarginNumber is not 0.0M) InteriorTargetValue = projectBoardModels[1].InteriorTarget.GrossMarginNumber / projectBoardModels[0].InteriorTarget.GrossMarginNumber;
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = InteriorTargetValue };
            decimal ClientTargetValue = 0.0M;
            if (projectBoardModels[0].ClientTarget.GrossMarginNumber is not 0.0M) ClientTargetValue = projectBoardModels[1].ClientTarget.GrossMarginNumber / projectBoardModels[0].ClientTarget.GrossMarginNumber;
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = ClientTargetGrossMargin, GrossMarginNumber = ClientTargetValue };
            decimal OfferTargetValue = 0.0M;
            if (projectBoardModels[0].Offer is not null&&projectBoardModels[0].Offer.GrossMarginNumber is not 0.0M) OfferTargetValue = projectBoardModels[1].Offer.GrossMarginNumber / projectBoardModels[0].Offer.GrossMarginNumber;
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = OfferTargetValue };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  单位平均成本      
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));

            projectBoardModel.ProjectName = "销售收入";
            //该 毛利率 全生命周期的 收入
            decimal InteriorALLincome = Targetvalue * (1 + InteriorTargetGrossMargin);
            //该 毛利率 全生命周期的 收入
            decimal ClientALLincome = await AllincomeDynamic(processId, 1M, ClientUnitPrice);
            //该 毛利率 全生命周期的 收入
            decimal OfferALLincome = await AllincomeDynamic(processId, 1M, OfferUnitPrice);
            //该 毛利率 全生命周期的 折让
            decimal InteriorALLAllowance = await SomeAllAllowance(processId, 1M, InteriorUnitPrice);
            //该 毛利率 全生命周期的 折让
            decimal ClientALLAllowance = await SomeAllAllowance(processId, 1M, ClientUnitPrice);
            //该 毛利率 全生命周期的 折让
            decimal OfferALLAllowance = await SomeAllAllowance(processId, 1M, OfferUnitPrice);
            //目标价内部 返利后收入
            decimal InteriorSRValue = InteriorALLincome - InteriorALLAllowance;
            //目标价客户 返利后收入
            decimal ClientSRValue = ClientALLincome - ClientALLAllowance;
            //本次报价 返利后收入
            decimal OfferSRValue = OfferALLincome - OfferALLAllowance;
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = InteriorSRValue };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = ClientSRValue };
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = OfferSRValue };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  销售收入      
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));

            projectBoardModel.ProjectName = "平均单价";
            //目标价 内部 平均单价
            decimal InteriorPJDJValue = 0.0M;
            if (modelTotal is not 0.0M) InteriorPJDJValue = InteriorSRValue / modelTotal;
            //目标价 客户 平均单价
            decimal ClientPJDJValue = 0.0M;
            if (modelTotal is not 0.0M) ClientPJDJValue = ClientSRValue / modelTotal;
            //目标价 客户 平均单价
            decimal OfferPJDJValue = 0.0M;
            if (modelTotal is not 0.0M) OfferPJDJValue = OfferSRValue / modelTotal;
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = InteriorPJDJValue };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = ClientPJDJValue };
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = OfferPJDJValue };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  平均单价      
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));

            projectBoardModel.ProjectName = "销售毛利";
            //该 毛利率 全生命周期的返利金额 内部
            decimal InteriorTargetGrossMarginALLRebateMoney = await SomeAllRebateMoney(processId, 1M, InteriorUnitPrice);
            //该 毛利率 全生命周期的税费损失 内部
            decimal InteriorTargetALLAddTaxRate = await SomeAllAddTaxRate(processId, 1M, InteriorUnitPrice);
            //该 毛利率 全生命周期的返利金额 客户
            decimal ClientGrossMarginALLRebateMoney = await SomeAllRebateMoney(processId, 1M, ClientUnitPrice);
            //该 毛利率 全生命周期的税费损失 客户
            decimal ClientTargetALLAddTaxRate = await SomeAllAddTaxRate(processId, 1M, ClientUnitPrice);
            //该 毛利率 全生命周期的返利金额 本次报价
            decimal OfferTargetGrossMarginALLRebateMoney = await SomeAllRebateMoney(processId, 1M, OfferUnitPrice);
            //该 毛利率 全生命周期的税费损失 本次报价
            decimal OfferTargetALLAddTaxRate = await SomeAllAddTaxRate(processId, 1M, OfferUnitPrice);
            //目标价 内部 销售毛利
            decimal InteriorXXMLValue = InteriorSRValue - Targetvalue - InteriorTargetGrossMarginALLRebateMoney - InteriorTargetALLAddTaxRate;
            //目标价 客户 销售毛利
            decimal ClientXXMLValue = ClientSRValue - Targetvalue - ClientGrossMarginALLRebateMoney - ClientTargetALLAddTaxRate;
            //本次报价 销售毛利
            decimal OfferXXMLValue = OfferSRValue - Targetvalue - OfferTargetGrossMarginALLRebateMoney - OfferTargetALLAddTaxRate;
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = InteriorXXMLValue };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = ClientXXMLValue };
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = OfferXXMLValue };
            #region 前几个版本 的  销售毛利      
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));

            projectBoardModel.ProjectName = "毛利率";
            //目标价 内部 毛利率
            decimal InteriorMLLValue = 0.0M;
            if (InteriorSRValue is not 0.0M) InteriorMLLValue = (InteriorXXMLValue / InteriorSRValue) * 100;
            //目标价 客户 毛利率
            decimal ClientMLLValue = 0.0M;
            if (ClientSRValue is not 0.0M) ClientMLLValue = (ClientXXMLValue / ClientSRValue) * 100;
            //本次报价 毛利率
            decimal OfferMLLValue = 0.0M;
            if (OfferSRValue is not 0.0M) OfferMLLValue = (OfferXXMLValue / OfferSRValue) * 100;
            projectBoardModel.InteriorTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = InteriorTargetGrossMargin * 100 };
            projectBoardModel.ClientTarget = new GrossMarginModel() { GrossMargin = InteriorTargetGrossMargin, GrossMarginNumber = ClientMLLValue };
            if (OfferUnitPrice.Count is not 0) projectBoardModel.Offer = new GrossMarginModel() { GrossMargin = 1, GrossMarginNumber = OfferMLLValue };
            projectBoardModel.OldOffer = null;//之前几轮的 分析汇总
            #region 前几个版本 的  毛利率      
            projectBoardModel.OldOffer = new List<GrossMarginModel>();//之前几轮的 分析汇总
            foreach (AuditFlow audit in auditFlows)
            {
                ProjectBoardOffers projectBoard = await _resourceProjectBoardOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(audit.Id) && p.ProjectName.Equals(projectBoardModel.ProjectName));
                if (projectBoard is not null)
                {
                    projectBoardModel.OldOffer.Add(JsonConvert.DeserializeObject<GrossMarginModel>(projectBoard.Offer));
                }
                else
                {
                    projectBoardModel.OldOffer.Add(new GrossMarginModel());
                }
            }
            #endregion
            projectBoardModels.Add(JsonConvert.DeserializeObject<ProjectBoardModel>(JsonConvert.SerializeObject(projectBoardModel)));
            return projectBoardModels;
        }
        /// <summary>
        /// 报价分析看板 动态动态单价表单的计算
        /// </summary>
        /// <param name="productBoardDtos"></param>
        /// <returns></returns>
        public async Task<ProductBoardGrossMarginDto> CalculateFullGrossMargin(ProductBoardProcessDto productBoardDtos)
        {
            ProductBoardGrossMarginDto productBoardGrossMargin = new();
            List<ProductBoardGrossMarginModel> dynamicProductBoardModels = new List<ProductBoardGrossMarginModel>();
            List<int> yearList = await GetYear(productBoardDtos.AuditFlowId);
            //每个模组全生命周期  返利后销售收入与收入 总和
            decimal AllRebateSalesRevenueFull = 0.0M;
            //每个模组全生命周期  销售毛利 总和
            decimal AllTradingProfitFull = 0.0M;
            foreach (DynamicProductBoardModel productBoardModel in productBoardDtos.ProductBoards)
            {
                //计算全生命周期的 返利后销售收入与收入
                decimal AllRebateSalesRevenue = 0.0M;
                //计算 销售毛利
                decimal AllTradingProfit = 0.0M;
                ProductBoardGrossMarginModel dynamicProductBoardModel = new ProductBoardGrossMarginModel();
                dynamicProductBoardModel.UnitPrice = productBoardModel.UnitPrice;
                //上一年的单价                
                decimal lastYearUnitPrice = productBoardModel.UnitPrice;
                //循环所有的年份               
                foreach (int item in yearList)
                {
                    //售价(单价)
                    var sellingPrice = await SellingPrice(productBoardDtos.AuditFlowId, productBoardModel.ModelCountId, 1M, item, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    decimal RebateSalesRevenueValue = await RebateSalesRevenue(productBoardDtos.AuditFlowId, productBoardModel.ModelCountId, 0.0M, item, lastYearUnitPrice, lastYearUnitPrice);
                    //返利后销售收入与收入
                    AllRebateSalesRevenue += RebateSalesRevenueValue;
                    decimal TradingProfitValue = await TradingProfit(productBoardDtos.AuditFlowId, productBoardModel.ModelCountId, 0.0M, item, 0.0M, lastYearUnitPrice);
                    //单年份的销售毛利
                    AllTradingProfit += TradingProfitValue;
                }
                if (AllRebateSalesRevenue is not 0.0M) dynamicProductBoardModel.GrossMargin = (AllTradingProfit / AllRebateSalesRevenue) * 100;
                dynamicProductBoardModels.Add(dynamicProductBoardModel);
                AllRebateSalesRevenueFull += AllRebateSalesRevenue;
                AllTradingProfitFull += AllTradingProfit;
            }
            productBoardGrossMargin.ProductBoardGrosses = dynamicProductBoardModels;
            if (AllRebateSalesRevenueFull is not 0.0M) productBoardGrossMargin.AllGrossMargin = (AllTradingProfitFull / AllRebateSalesRevenueFull) * 100;
            return productBoardGrossMargin;
        }
        /// <summary>
        /// 计算年份维度
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="grossMargin"></param>
        /// <param name="unitPrice"></param>
        /// <returns></returns>
        public async Task<List<YearDimensionalityComparisonDto>> YearDimensionalityComparison(long processId, decimal grossMargin, List<DynamicProductBoardModel> dynamicProductBoardModels)
        {
            List<YearDimensionalityComparisonDto> yearDimensionalityComparisonDtos = new List<YearDimensionalityComparisonDto>();
            YearDimensionalityComparisonDto yearDimensionalityComparisonDto = new YearDimensionalityComparisonDto();
            List<SopOrValueMode> sopOrValueModes = new List<SopOrValueMode>();
            //年份
            List<int> YearList = await GetYear(processId);
            List<long> AllModelCountId = dynamicProductBoardModels.Select(p=>p.ModelCountId).ToList();
            yearDimensionalityComparisonDto.GrossMargin = grossMargin;
            yearDimensionalityComparisonDto.Project = "数量";
            foreach (var Year in YearList)
            {
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                sopOrValueMode.Year = Year;
                foreach (var boardModel in dynamicProductBoardModels)
                {                  
                   sopOrValueMode.Value += await SalesQuantity(processId, boardModel.ModelCountId, Year);
                }                
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);//各个年份数量的总和
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));

            yearDimensionalityComparisonDto.Project = "单价";
            Dictionary<long, decimal> DictiolastYearUnitPrice = new Dictionary<long, decimal>();
            foreach (var Year in YearList)
            {
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                foreach (var boardModel in dynamicProductBoardModels)
                {
                    if (!DictiolastYearUnitPrice.ContainsKey(boardModel.ModelCountId)) DictiolastYearUnitPrice[boardModel.ModelCountId] = boardModel.UnitPrice;
                    decimal ALLYearPrice = await SellingPrice(processId, boardModel.ModelCountId, 1M, Year, DictiolastYearUnitPrice[boardModel.ModelCountId], 0.0M);
                    DictiolastYearUnitPrice[boardModel.ModelCountId] = ALLYearPrice;
                    sopOrValueMode.Value += ALLYearPrice;
                }
                sopOrValueMode.Year = Year;
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));
            yearDimensionalityComparisonDto.Project = "销售成本";
            foreach (var Year in YearList)
            {
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                decimal ALLsellingCost = 0.0M;
                foreach (var ModelCountId in AllModelCountId)//循环每一个模组
                {
                    //每一个模组每年的单位成本
                    decimal unitCost = 0.0M;
                    ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(ModelCountId) && p.Year.Equals(Year));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (!string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                    if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                    decimal Count = await SalesQuantity(processId, ModelCountId, Year);
                    ALLsellingCost += unitCost * Count;
                }             
                sopOrValueMode.Year = Year;
                sopOrValueMode.Value = ALLsellingCost;
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));
            yearDimensionalityComparisonDto.Project = "单位平均成本";
            foreach (var Year in YearList)
            {
                //该 年所有 模组的成本
                decimal cost = yearDimensionalityComparisonDtos[2].YearList.Where(p => p.Year.Equals(Year)).Select(p => p.Value).FirstOrDefault();
                //该  年所有 模组的数量
                decimal count = yearDimensionalityComparisonDtos[0].YearList.Where(p => p.Year.Equals(Year)).Select(p => p.Value).FirstOrDefault();
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                decimal modelTotal = cost / count;
                sopOrValueMode.Year = Year;
                sopOrValueMode.Value = modelTotal;
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));
            DictiolastYearUnitPrice.Clear();
            yearDimensionalityComparisonDto.Project = "销售收入(元)";
            foreach (var Year in YearList)
            {
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                sopOrValueMode.Year = Year;
                foreach (var item in dynamicProductBoardModels)
                {
                    //上一年的单价
                    if (!DictiolastYearUnitPrice.ContainsKey(item.ModelCountId)) DictiolastYearUnitPrice[item.ModelCountId] = item.UnitPrice;
                    decimal ALLYearPrice = await SellingPrice(processId, item.ModelCountId, 1M, Year, DictiolastYearUnitPrice[item.ModelCountId], 0.0M);
                    DictiolastYearUnitPrice[item.ModelCountId] = ALLYearPrice;
                    if (!DictiolastYearUnitPrice.ContainsKey(item.ModelCountId)) DictiolastYearUnitPrice[item.ModelCountId] = item.UnitPrice;
                    //销售数量
                    decimal Count = await SalesQuantity(processId, item.ModelCountId, Year);
                    var prop = await AllincomeYear(processId, Year, 1M, DictiolastYearUnitPrice[item.ModelCountId]);
                    //该 年份 毛利率 全生命周期的 收入
                    decimal InteriorALLincome = ALLYearPrice * Count;
                    //该 毛利率 全生命周期的 折让
                    decimal InteriorALLAllowance = await Allowance(processId, Year, InteriorALLincome);
                    decimal modelTotal = InteriorALLincome - InteriorALLAllowance;
                    sopOrValueMode.Value += modelTotal;
                }
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));
            DictiolastYearUnitPrice.Clear();
            yearDimensionalityComparisonDto.Project = "销售毛利(元)";
            foreach (var Year in YearList)
            {
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                sopOrValueMode.Year = Year;

                foreach (var dynamicProductBoardModel in dynamicProductBoardModels)
                {
                    //上一年的单价
                    if (!DictiolastYearUnitPrice.ContainsKey(dynamicProductBoardModel.ModelCountId)) DictiolastYearUnitPrice[dynamicProductBoardModel.ModelCountId] = dynamicProductBoardModel.UnitPrice;
                    decimal ALLYearPrice = await SellingPrice(processId, dynamicProductBoardModel.ModelCountId, 1M, Year, DictiolastYearUnitPrice[dynamicProductBoardModel.ModelCountId], 0.0M);
                    DictiolastYearUnitPrice[dynamicProductBoardModel.ModelCountId] = ALLYearPrice;
                    //销售数量
                    decimal Count = await SalesQuantity(processId, dynamicProductBoardModel.ModelCountId, Year);
                    var prop = await AllincomeYear(processId, Year, 1M, DictiolastYearUnitPrice[dynamicProductBoardModel.ModelCountId]);
                    //该 年份 毛利率 全生命周期的 收入
                    decimal InteriorALLincome = ALLYearPrice * Count;
                    //某一年 某一个模组 返利金额
                    decimal TargetGrossMarginALLRebateMoney = await SomeModelCountRebateMoneyYear(processId, dynamicProductBoardModel.ModelCountId, Year, 1M, InteriorALLincome);
                    //该 毛利率 全生命周期的税费损失
                    decimal GrossMarginALLRebateMoney = await SomeModelCountAddTaxRateYear(TargetGrossMarginALLRebateMoney, Year);
                    //返利后销售成本
                    decimal unitCost = 0M;
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(dynamicProductBoardModel.ModelCountId) && p.Year.Equals(Year));
                    if (!string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                    if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                    decimal cost = unitCost * Count;
                    //该 毛利率 全生命周期的 折让
                    decimal InteriorALLAllowance = await Allowance(processId, Year, InteriorALLincome);
                    decimal modelTotal = InteriorALLincome - cost - TargetGrossMarginALLRebateMoney - GrossMarginALLRebateMoney- InteriorALLAllowance;
                    sopOrValueMode.Value +=modelTotal;
                }
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = yearDimensionalityComparisonDto.YearList.Sum(p => p.Value);
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));

            yearDimensionalityComparisonDto.Project = "毛利率(%)";
            decimal ALLgrossmargin = 0.0M;
            decimal ALLincome = 0.0M;
            foreach (var Year in YearList)
            {
                //该 年份 的 毛利
                decimal grossmargin = yearDimensionalityComparisonDtos[5].YearList.Where(p => p.Year.Equals(Year)).Select(p => p.Value).FirstOrDefault();
                ALLgrossmargin+= grossmargin;
                //该 年份 的 返利后的收入
                decimal income = yearDimensionalityComparisonDtos[4].YearList.Where(p => p.Year.Equals(Year)).Select(p => p.Value).FirstOrDefault();
                ALLincome += income;
                SopOrValueMode sopOrValueMode = new SopOrValueMode();
                decimal modelTotal = 0.0M;
                if (income is not 0.0M) modelTotal = (grossmargin / income)*100;
                sopOrValueMode.Year = Year;
                sopOrValueMode.Value = modelTotal;
                sopOrValueModes.Add(sopOrValueMode);
            }
            yearDimensionalityComparisonDto.YearList = JsonConvert.DeserializeObject<List<SopOrValueMode>>(JsonConvert.SerializeObject(sopOrValueModes));
            sopOrValueModes.Clear();
            yearDimensionalityComparisonDto.Totak = (ALLgrossmargin / ALLincome)*100;
            yearDimensionalityComparisonDtos.Add(JsonConvert.DeserializeObject<YearDimensionalityComparisonDto>(JsonConvert.SerializeObject(yearDimensionalityComparisonDto)));
            return yearDimensionalityComparisonDtos;
        }
        /// <summary>
        /// 汇总分析表 根据 整套 毛利率 计算(本次报价)
        /// </summary>
        public async Task<List<SpreadSheetCalculateDto>> SpreadSheetCalculate(long processId, decimal GrossMargin, List<DynamicProductBoardModel> AllUnitPrice)
        {
            List<SpreadSheetCalculateDto> spreadSheetCalculateDtos = new List<SpreadSheetCalculateDto>();
            SpreadSheetCalculateDto spreadSheetCalculateDto = new SpreadSheetCalculateDto();
            spreadSheetCalculateDto.ProjectName = "数量";
            decimal modelTotal = _resourceModelCount.GetAllList().Where(p => p.AuditFlowId.Equals(processId)).Sum(p => p.ModelTotal);
            spreadSheetCalculateDto.Value = modelTotal;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "销售成本";
            decimal Targetvalue = await AllsellingCost(processId);
            spreadSheetCalculateDto.Value = Targetvalue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "单位平均成本";
            decimal InteriorTargetValue = 0.0M;
            if (spreadSheetCalculateDtos[0].Value is not 0.0M) InteriorTargetValue = spreadSheetCalculateDtos[1].Value / spreadSheetCalculateDtos[0].Value;
            spreadSheetCalculateDto.Value = InteriorTargetValue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "返利后销售收入";
            //该 毛利率 全生命周期的 收入
            decimal ALLincome = await AllincomeDynamic(processId, 1M, AllUnitPrice);
            //该 毛利率 全生命周期的 折让
            decimal ALLAllowance = await SomeAllAllowance(processId, GrossMargin, AllUnitPrice);
            //目标价内部 返利后收入
            decimal SRValue = ALLincome - ALLAllowance;
            spreadSheetCalculateDto.Value = SRValue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "平均单价";
            //目标价 内部 平均单价
            decimal PJDJValue = 0.0M;
            if (modelTotal is not 0.0M) PJDJValue = SRValue / modelTotal;
            spreadSheetCalculateDto.Value = PJDJValue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "销售毛利";
            //全生命周期的返利金额 
            decimal TargetGrossMarginALLRebateMoney = await SomeAllRebateMoney(processId, GrossMargin, AllUnitPrice);
            //全生命周期的税费损失
            decimal TargetALLAddTaxRate = await SomeAllAddTaxRate(processId, GrossMargin, AllUnitPrice);
            decimal XXMLValue = SRValue - Targetvalue - TargetGrossMarginALLRebateMoney - TargetALLAddTaxRate;
            spreadSheetCalculateDto.Value = XXMLValue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            spreadSheetCalculateDto.ProjectName = "毛利率";
            //目标价 内部 毛利率
            decimal InteriorMLLValue = 0.0M;
            if (SRValue is not 0.0M) InteriorMLLValue = (XXMLValue / SRValue) * 100;
            spreadSheetCalculateDto.Value = InteriorMLLValue;
            spreadSheetCalculateDtos.Add(JsonConvert.DeserializeObject<SpreadSheetCalculateDto>(JsonConvert.SerializeObject(spreadSheetCalculateDto)));
            return spreadSheetCalculateDtos;
        }
        /// <summary>
        /// 下载
        /// 表
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadMessage(long processId, string fileName)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\成本信息表模板.xlsx";
            QuotationListDto quotationListDto = await QuotationList(processId);
            //年份
            List<int> YearList = await GetYear(processId);
            List<SopModel> Sop = new List<SopModel>();
            foreach (int year in YearList)
            {
                SopModel sopModel = new();
                foreach (MotionMessageModel item in quotationListDto.MotionMessage)
                {
                    sopModel.Year = year;
                    if (item.MessageName.Contains(StaticName.ZL))
                    {
                        sopModel.Motion = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NJL))
                    {
                        sopModel.YearDrop = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NDFLYQ))
                    {
                        sopModel.RebateRequest = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.YCXZRL))
                    {
                        sopModel.DiscountRate = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                }
                Sop.Add(sopModel);
            }
            //核心部件
            List<PartsModel> partsModels = new List<PartsModel>();
            foreach (CoreComponentModel item in quotationListDto.CoreComponent)
            {
                partsModels.Add(new PartsModel() { PartsName = "核心部件:" + item.ComponentName, Parts = "核心部件", Model = "型号", Type = "类型", Remark = "备注" });
                foreach (ComponenModel prod in item.ProductSubclass)
                {
                    partsModels.Add(new PartsModel() { PartsName = "", Parts = prod.CoreComponent, Model = prod.Model, Type = prod.Type, Remark = prod.Remark });
                }
                partsModels.Add(new PartsModel() { Model = "", Parts = "", PartsName = "", Type = "", Remark = "" });
            }
            //NRE
            List<NREModel> nREModels = new();
            foreach (NreCostMessageModel item in quotationListDto.NreCost)
            {
                nREModels.Add(new NREModel() { NreName = "NRE费用信息:" + item.NreCostModuleName, CostName = "", Cost = "成本" });
                foreach (NreCostModel nre in item.NreCostModels)
                {
                    nREModels.Add(new NREModel() { NreName = "", CostName = nre.Name, Cost = nre.Cost.ToString() });
                }
                nREModels.Add(new NREModel() { NreName = "", CostName = "", Cost = "" });
            }
            //成本单价信息
            List<PricingModel> pricingModels = new();
            foreach (PricingMessage item in quotationListDto.PricingMessage)
            {
                decimal BOM = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.BOMCB)).Select(p => p.CostValue).First();
                decimal ProduceCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZZCB)).Select(p => p.CostValue).First();
                decimal Yield = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.SHCB)).Select(p => p.CostValue).First();
                decimal Freight = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.WLCB)).Select(p => p.CostValue).First();
                decimal MOQ = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.MOQFTCB)).Select(p => p.CostValue).First();
                decimal QualityCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.QT)).Select(p => p.CostValue).First();
                decimal SumCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZCB)).Select(p => p.CostValue).First();
                pricingModels.Add(new PricingModel() { Name = item.PricingMessageName, BOM = BOM, ProduceCost = ProduceCost, Yield = Yield, Freight = Freight, MOQ = MOQ, QualityCost = QualityCost, SumCost = SumCost, Remark = "" });
            }
            var value = new
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),//日期
                RecordNumber = quotationListDto.RecordNumber,//记录编号           
                Versions = quotationListDto.Versions,//版本
                DirectCustomerName = quotationListDto.DirectCustomerName,//直接客户名称
                TerminalCustomerName = quotationListDto.TerminalCustomerName,//终端客户名称
                OfferForm = quotationListDto.OfferForm,//报价形式
                SopTime = quotationListDto.SopTime,//SOP时间
                ProjectCycle = quotationListDto.ProjectCycle,//项目生命周期
                ForSale = quotationListDto.ForSale,//销售类型
                modeOfTrade = quotationListDto.modeOfTrade,//贸易方式
                PaymentMethod = quotationListDto.PaymentMethod,//付款方式
                ExchangeRate = quotationListDto.ExchangeRate,//汇率
                Sop = Sop,
                ProjectName = quotationListDto.ProjectName,//项目名称
                Parts = partsModels,
                NRE = nREModels,
                Cost = pricingModels,
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
        /// 下载
        /// 表
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadCostInformationExcel(long processId, string fileName)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\新成本信息表模板.xlsx";
            QuotationListDto quotationListDto = await QuotationList(processId);
            //年份
            List<int> YearList = await GetYear(processId);
            List<SopModel> Sop = new List<SopModel>();
            foreach (int year in YearList)
            {
                SopModel sopModel = new();
                foreach (MotionMessageModel item in quotationListDto.MotionMessage)
                {
                    sopModel.Year = year;
                    if (item.MessageName.Contains(StaticName.ZL))
                    {
                        sopModel.Motion = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NJL))
                    {
                        sopModel.YearDrop = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NDFLYQ))
                    {
                        sopModel.RebateRequest = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.YCXZRL))
                    {
                        sopModel.DiscountRate = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    //年度佣金比例(%),字段还未知
                    if (item.MessageName.Contains(StaticName.YCXZRL))
                    {
                        sopModel.DiscountRate = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                }
                Sop.Add(sopModel);
            }
            //核心部件
            List<PartsModel> partsModels = new List<PartsModel>();
            foreach (CoreComponentModel item in quotationListDto.CoreComponent)
            {
                partsModels.Add(new PartsModel() { PartsName = "核心部件:" + item.ComponentName, Parts = "核心部件", Model = "型号", Type = "类型", Remark = "备注" });
                foreach (ComponenModel prod in item.ProductSubclass)
                {
                    partsModels.Add(new PartsModel() { PartsName = "", Parts = prod.CoreComponent, Model = prod.Model, Type = prod.Type, Remark = prod.Remark });
                }
                partsModels.Add(new PartsModel() { Model = "", Parts = "", PartsName = "", Type = "", Remark = "" });
            }
            //NRE
            List<NREModel> nREModels = new();
            foreach (NreCostMessageModel item in quotationListDto.NreCost)
            {
                nREModels.Add(new NREModel() { NreName = "NRE费用信息:" + item.NreCostModuleName, CostName = "", Cost = "成本" });
                foreach (NreCostModel nre in item.NreCostModels)
                {
                    nREModels.Add(new NREModel() { NreName = "", CostName = nre.Name, Cost = nre.Cost.ToString() });
                }
                nREModels.Add(new NREModel() { NreName = "", CostName = "", Cost = "" });
            }
            //成本单价信息
            List<PricingModel> pricingModels = new();
            foreach (PricingMessage item in quotationListDto.PricingMessage)
            {
                decimal BOM = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.BOMCB)).Select(p => p.CostValue).First();
                decimal ProduceCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZZCB)).Select(p => p.CostValue).First();
                decimal Yield = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.SHCB)).Select(p => p.CostValue).First();
                decimal Freight = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.WLCB)).Select(p => p.CostValue).First();
                decimal MOQ = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.MOQFTCB)).Select(p => p.CostValue).First();
                decimal QualityCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.QT)).Select(p => p.CostValue).First();
                decimal SumCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZCB)).Select(p => p.CostValue).First();
                pricingModels.Add(new PricingModel() { Name = item.PricingMessageName, BOM = BOM, ProduceCost = ProduceCost, Yield = Yield, Freight = Freight, MOQ = MOQ, QualityCost = QualityCost, SumCost = SumCost, Remark = "" });
            }
            var value = new
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),//日期
                RecordNumber = quotationListDto.RecordNumber,//记录编号           
                Versions = quotationListDto.Versions,//版本
                DirectCustomerName = quotationListDto.DirectCustomerName,//直接客户名称
                TerminalCustomerName = quotationListDto.TerminalCustomerName,//终端客户名称
                OfferForm = quotationListDto.OfferForm,//报价形式
                SopTime = quotationListDto.SopTime,//SOP时间
                ProjectCycle = quotationListDto.ProjectCycle,//项目生命周期
                ForSale = quotationListDto.ForSale,//销售类型
                modeOfTrade = quotationListDto.modeOfTrade,//贸易方式
                PaymentMethod = quotationListDto.PaymentMethod,//付款方式
                ExchangeRate = quotationListDto.ExchangeRate,//汇率
                Sop = Sop,
                ProjectName = quotationListDto.ProjectName,//项目名称
                Parts = partsModels,
                NRE = nREModels,
                Cost = pricingModels,
            };
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsByTemplateAsync(templatePath, value);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{fileName}.xlsx"
            };
        }
        
        private async Task<DownloadAuditQuotationListModel> DownloadAuditQuotation(long processId)
        {
            QuotationListDto quotationListDto = await QuotationList(processId);
            //年份
            List<int> YearList = await GetYear(processId);
            List<SopModel> Sop = new List<SopModel>();
            foreach (int year in YearList)
            {
                SopModel sopModel = new();
                foreach (MotionMessageModel item in quotationListDto.MotionMessage)
                {
                    sopModel.Year = year;
                    if (item.MessageName.Contains(StaticName.ZL))
                    {
                        sopModel.Motion = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NJL))
                    {
                        sopModel.YearDrop = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.NDFLYQ))
                    {
                        sopModel.RebateRequest = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                    if (item.MessageName.Contains(StaticName.YCXZRL))
                    {
                        sopModel.DiscountRate = item.Sop.Where(p => p.Year.Equals(year)).Select(p => p.Value).First();
                    }
                }
                Sop.Add(sopModel);
            }
            //核心部件
            List<PartsModel> partsModels = new List<PartsModel>();
            foreach (CoreComponentModel item in quotationListDto.CoreComponent)
            {
                partsModels.Add(new PartsModel() { PartsName = "核心部件:" + item.ComponentName, Parts = "核心部件", Model = "型号", Type = "类型", Remark = "备注" });
                foreach (ComponenModel prod in item.ProductSubclass)
                {
                    partsModels.Add(new PartsModel() { PartsName = "", Parts = prod.CoreComponent, Model = prod.Model, Type = prod.Type, Remark = prod.Remark });
                }
                partsModels.Add(new PartsModel() { Model = "", Parts = "", PartsName = "", Type = "", Remark = "" });
            }
            //NRE
            List<NREModel> nREModels = new();
            foreach (NreCostMessageModel item in quotationListDto.NreCost)
            {
                nREModels.Add(new NREModel() { NreName = "NRE费用信息:" + item.NreCostModuleName, CostName = "", Cost = "成本" });
                foreach (NreCostModel nre in item.NreCostModels)
                {
                    nREModels.Add(new NREModel() { NreName = "", CostName = nre.Name, Cost = nre.Cost.ToString() });
                }
                nREModels.Add(new NREModel() { NreName = "", CostName = "", Cost = "" });
            }
            //成本单价信息
            List<PricingModel> pricingModels = new();
            foreach (PricingMessage item in quotationListDto.PricingMessage)
            {
                decimal BOM = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.BOMCB)).Select(p => p.CostValue).First();
                decimal ProduceCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZZCB)).Select(p => p.CostValue).First();
                decimal Yield = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.SHCB)).Select(p => p.CostValue).First();
                decimal Freight = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.WLCB)).Select(p => p.CostValue).First();
                decimal MOQ = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.MOQFTCB)).Select(p => p.CostValue).First();
                decimal QualityCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.QT)).Select(p => p.CostValue).First();
                decimal SumCost = item.PricingMessageModels.Where(p => p.Name.Contains(StaticName.ZCB)).Select(p => p.CostValue).First();
                pricingModels.Add(new PricingModel() { Name = item.PricingMessageName, BOM = BOM, ProduceCost = ProduceCost, Yield = Yield, Freight = Freight, MOQ = MOQ, QualityCost = QualityCost, SumCost = SumCost, Remark = "" });
            }
            //报价策略         
            List<BiddingStrategy> biddingStrategies = new();
            biddingStrategies = await _financeBiddingStrategy.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
            //费用表
            List<ExpensesStatementModel> expensesStatementModels = quotationListDto.ExpensesStatement;
            //
            DownloadAuditQuotationListModel value = new DownloadAuditQuotationListModel()
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),//日期
                RecordNumber = quotationListDto.RecordNumber,//记录编号           
                Versions = quotationListDto.Versions,//版本
                DirectCustomerName = quotationListDto.DirectCustomerName,//直接客户名称
                TerminalCustomerName = quotationListDto.TerminalCustomerName,//终端客户名称
                OfferForm = quotationListDto.OfferForm,//报价形式
                SopTime = quotationListDto.SopTime,//SOP时间
                ProjectCycle = quotationListDto.ProjectCycle,//项目生命周期
                ForSale = quotationListDto.ForSale,//销售类型
                modeOfTrade = quotationListDto.modeOfTrade,//贸易方式
                PaymentMethod = quotationListDto.PaymentMethod,//付款方式
                ExchangeRate = quotationListDto.ExchangeRate,//汇率
                Sop = Sop,
                ProjectName = quotationListDto.ProjectName,//项目名称
                Parts = partsModels,
                NRE = nREModels,
                Cost = pricingModels,
                Strategy = biddingStrategies,//报价策略
                Offer = expensesStatementModels,//费用表
            };
            return value;
        }
        /// <summary>
        /// 下载报价审核表
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadAuditQuotationList(long processId, string fileName)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\报价审核表模板.xlsx";
            DownloadAuditQuotationListModel value = await DownloadAuditQuotation(processId);
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsByTemplateAsync(templatePath, value);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{fileName}.xlsx"
            };
        }
        /// <summary>
        /// 报价审核表 下载--流
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="fileName"></param>
        public async Task<MemoryStream> DownloadAuditQuotationListStream(long processId, string fileName)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\报价审核表模板.xlsx";
            DownloadAuditQuotationListModel value = await DownloadAuditQuotation(processId);
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsByTemplateAsync(templatePath, value);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        /// <summary>
        ///获取总共 零件
        /// </summary>
        private async Task<List<PartModel>> PartDtoList(long processId)
        {
            //根据流程表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
            List<PartModel> partModel = (from a in modelCount
                                         select new PartModel
                                         {
                                             ProductId = a.Id,
                                             ParName = a.Product,
                                         }).ToList();
            return partModel;
        }
        /// <summary>
        /// 查看 报价审核表
        /// </summary>
        public async Task<QuotationListDto> QuotationList(long processId)
        {
            try
            {
                List<InitialResourcesManagement> initialResourcesManagement = await _resourceResourcesManagement.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
                List<ExpensesStatementModel> expensesStatements = ObjectMapper.Map<List<ExpensesStatementModel>>(initialResourcesManagement);
                QuotationListDto quotationListDto = new QuotationListDto();
                List<FinanceDictionaryDetail> priceEvaluations = await _financeDictionaryDetailRepository.GetAllListAsync();
                QuotationListDto pp = (from o in await _resourceAuditFlow.GetAllListAsync(p => p.Id.Equals(processId))
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
                                       select new QuotationListDto
                                       {
                                           Date = DateTime.Now,//查询日期
                                           RecordNumber = oa1.Number,// 单据编号
                                           Versions = o.QuoteVersion,//版本
                                           OfferForm = b2.DisplayName,//报价形式
                                           DirectCustomerName = oa1.CustomerName,//直接客户名称
                                           ClientNature = c2.DisplayName,//客户性质
                                           TerminalCustomerName = oa1 is null ? "" : oa1.TerminalName,//终端客户名称
                                           TerminalClientNature = d2 is null ? "" : d2.DisplayName,//终端客户性质
                                                                                                   //开发计划 手工录入
                                           SopTime = oa1.SopTime,//Sop时间
                                           ProjectCycle = oa1.ProjectCycle,//项目周期
                                           ForSale = e2.DisplayName,//销售类型
                                           modeOfTrade = z2.DisplayName,//贸易方式
                                           PaymentMethod = oa1.PaymentMethod,//付款方式
                                           QuoteCurrency = f2.ExchangeRateKind,//报价币种
                                           ExchangeRate = oa1.ExchangeRate,//汇率
                                           ProjectName = oa1.ProjectName,//项目名称
                                       }).FirstOrDefault();
                if (pp is not null)
                {
                    quotationListDto = pp;
                }
                AuditQuotationList auditQuotationList = await _financeAuditQuotationList.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId));
                if (auditQuotationList is not null)
                {
                    AuditQuotationListDto auditQuotationListDto = JsonConvert.DeserializeObject<AuditQuotationListDto>(auditQuotationList.AuditQuotationListJson);
                    //开发计划
                    quotationListDto.DevelopmentPlan =auditQuotationListDto.DevelopmentPlan;
                }
                  
                //年份
                List<int> YearList = await GetYear(processId);
                //sop走量信息
                #region 走量信息
                MotionMessageModel motionMessageModel = new MotionMessageModel();
                motionMessageModel.MessageName = StaticName.ZL;
                motionMessageModel.Sop = new();
                foreach (var year in YearList)
                {
                    int quantity = _resourceModelCountYear.GetAllList(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year)).Sum(p => p.Quantity);
                    motionMessageModel.Sop.Add(new SopOrValueMode() { Year = year, Value = quantity });
                }
                quotationListDto.MotionMessage = new List<MotionMessageModel>();
                quotationListDto.MotionMessage.Add(new MotionMessageModel() { MessageName = motionMessageModel.MessageName, Sop = motionMessageModel.Sop });
                motionMessageModel = new MotionMessageModel();
                motionMessageModel.Sop = new();
                motionMessageModel.MessageName = StaticName.NJL;
                foreach (var year in YearList)
                {
                    decimal annualDeclineRate = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year)).Sum(p => p.AnnualDeclineRate);
                    motionMessageModel.Sop.Add(new SopOrValueMode() { Year = year, Value = annualDeclineRate });
                }
                quotationListDto.MotionMessage.Add(new MotionMessageModel() { MessageName = motionMessageModel.MessageName, Sop = motionMessageModel.Sop });
                motionMessageModel = new MotionMessageModel();
                motionMessageModel.Sop = new();
                motionMessageModel.MessageName = StaticName.NDFLYQ;
                foreach (var year in YearList)
                {
                    decimal annualRebateRequirements = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year)).Sum(p => p.AnnualRebateRequirements);
                    motionMessageModel.Sop.Add(new SopOrValueMode() { Year = year, Value = annualRebateRequirements });
                }
                quotationListDto.MotionMessage.Add(new MotionMessageModel() { MessageName = motionMessageModel.MessageName, Sop = motionMessageModel.Sop });
                motionMessageModel = new MotionMessageModel();
                motionMessageModel.Sop = new();
                motionMessageModel.MessageName = StaticName.YCXZRL;
                foreach (var year in YearList)
                {
                    decimal oneTimeDiscountRate = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year)).Sum(p => p.OneTimeDiscountRate);
                    motionMessageModel.Sop.Add(new SopOrValueMode() { Year = year, Value = oneTimeDiscountRate });
                }
                quotationListDto.MotionMessage.Add(new MotionMessageModel() { MessageName = motionMessageModel.MessageName, Sop = motionMessageModel.Sop });
                #endregion
                List<PartModel> partModels = await PartDtoList(processId);
                //核心部件
                #region 核心部件
                List<CoreComponentModel> coreComponentModels = new List<CoreComponentModel>();

                foreach (PartModel partModel in partModels)
                {
                    CoreComponentModel coreComponentModel = new CoreComponentModel();
                    coreComponentModel.ModelCountId = partModel.ProductId;
                    coreComponentModel.ComponentName = partModel.ParName;
                    List<ElectronicBomInfo> electronicBomInfos = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    var SensorTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                            join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                            select new
                                            {
                                                SensorTypeSelect = AnalysisBoardAttribute.TypeTransition(b.SensorTypeSelect)
                                            }).FirstOrDefault();
                    string SensorModel = electronicBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.Sensor)).Select(p => p.TypeName).FirstOrDefault();
                    coreComponentModel.ProductSubclass = new();
                    if (!string.IsNullOrEmpty(SensorModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.Sensor_CoreComponent, Model = SensorModel, Type = SensorTypeSelect != null ? SensorTypeSelect.SensorTypeSelect : "" });
                    var IspTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                         join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                         select new
                                         {
                                             IspTypeSelect = AnalysisBoardAttribute.TypeTransition(b.IspTypeSelect)
                                         }).FirstOrDefault();
                    string LSPModel = electronicBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.ICISP)).Select(p => p.TypeName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(LSPModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.ICISP_CoreComponent, Model = LSPModel, Type = IspTypeSelect != null ? IspTypeSelect.IspTypeSelect : "" });
                    var SerialChipTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                                join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                                select new
                                                {
                                                    SerialChipTypeSelect = AnalysisBoardAttribute.TypeTransition(b.SerialChipTypeSelect)
                                                }).FirstOrDefault();
                    string CXXPModel = electronicBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.SerialChip)).Select(p => p.SapItemName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(CXXPModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.SerialChip_CoreComponent, Model = CXXPModel, Type = SerialChipTypeSelect != null ? SerialChipTypeSelect.SerialChipTypeSelect : "" });
                    var LedTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                         join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                         select new
                                         {
                                             LedTypeSelect = AnalysisBoardAttribute.TypeTransition(b.LedTypeSelect)
                                         }).FirstOrDefault();
                    string LEDModel = electronicBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.LEDVCSEL)).Select(p => p.SapItemName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(LEDModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.LEDVCSEL_CoreComponent, Model = LEDModel, Type = LedTypeSelect != null ? LedTypeSelect.LedTypeSelect : "" });


                    List<StructureBomInfo> structureBomInfos = await _resourceStructureBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));

                    string LensModel = structureBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.Shot)).Select(p => p.DrawingNumName).FirstOrDefault();
                    var LensTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                          join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                          select new
                                          {
                                              LensTypeSelect = AnalysisBoardAttribute.TypeTransition(b.LensTypeSelect)
                                          }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(LensModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.Shot_CoreComponent, Model = LensModel, Type = LensTypeSelect != null ? LensTypeSelect.LensTypeSelect : "" });
                    //线缆
                    string XSModel = structureBomInfos.Where(p => p.TypeName != null && p.TypeName.Contains(StaticName.Harness)).Select(p => p.DrawingNumName).FirstOrDefault();
                    var CableTypeSelect = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId))
                                           join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                                           select new
                                           {
                                               CableTypeSelect = AnalysisBoardAttribute.TypeTransition(b.CableTypeSelect)
                                           }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(XSModel)) coreComponentModel.ProductSubclass.Add(new ComponenModel() { CoreComponent = StaticName.Harness_CoreComponent, Model = XSModel, Type = CableTypeSelect != null ? CableTypeSelect.CableTypeSelect : "" });
                    coreComponentModels.Add(coreComponentModel);
                }
                quotationListDto.CoreComponent = coreComponentModels;
                #endregion
                //Nre费用
                #region Nre费用
                //所有的模组
                List<NreCostMessageModel> nreCostMessageModels = new List<NreCostMessageModel>();

                foreach (PartModel partModel in partModels)
                {
                    NreCostMessageModel nreCostMessageModel = new();
                    nreCostMessageModel.ModelCountId = partModel.ProductId;
                    nreCostMessageModel.NreCostModuleName = partModel.ParName;
                    //手板件费
                    nreCostMessageModel.NreCostModels = new();
                    List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal handPieceCostsCost = handPieceCosts.Sum(p => p.Cost);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.SBJF, Cost = handPieceCostsCost });
                    //模具费
                    List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal mouldInventoriesCost = mouldInventories.Sum(p => p.Cost);                    
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.MJF, Cost = mouldInventoriesCost });
                    //检具费
                    List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal qADepartmentQCsCost = qADepartmentQCs.Sum(p => p.Count * p.UnitPrice);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.JJF, Cost = qADepartmentQCsCost });
                    //生产设备费用 
                    List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId))
                                                          join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Equipment)) on a.Id equals b.WorkHoursId
                                                          select new EquipmentInfo
                                                          {
                                                              WorkHoursId = b.WorkHoursId,
                                                              Part = b.Part,
                                                              EquipmentName = b.EquipmentName,
                                                              Status = b.Status,
                                                              Number = b.Number,
                                                              UnitPrice = b.UnitPrice,
                                                          }).ToList();
                    decimal equipmentInfosCost = equipmentInfos.Sum(p => p.UnitPrice * p.Number);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.SCSBF, Cost = equipmentInfosCost });
                    //工装治具费(工装费用(包括 工装费用+测试线费用)+治具费用)
                    List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(processId)&&p.ProductId.Equals(partModel.ProductId));
                    //==>测试线费用+工装费用
                    decimal frockCost = workingHours.Sum(s => s.TestNum * s.TestPrice + s.ToolingPrice * s.ToolingNum);
                    List<EquipmentInfo> equipment = (from a in workingHours
                                                     join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Fixture)) on a.Id equals b.WorkHoursId
                                                     select new EquipmentInfo
                                                     {
                                                         Number = b.Number,
                                                         UnitPrice = b.UnitPrice,
                                                     }).ToList();
                    //==>治具费用
                    decimal gaugeCost = equipment.Sum(s => s.Number * s.UnitPrice);
                    decimal toolingFixtureInfosCost = frockCost + gaugeCost;
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.GZZJF, Cost = toolingFixtureInfosCost });
                    //可靠性费(实验费,包含(产品部-电子工程师录入的试验费用&&品保部录入的实验费用))
                    //==>-产品部-电子工程师录入的试验费用
                    List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    //==>-品保部录入的实验费用
                    List<QADepartmentTest> qADepartmentTests = await _resourceQADepartmentTest.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal testCost = laboratoryFees.Sum(p => p.AllCost) + qADepartmentTests.Sum(p => p.AllCost);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.SYF, Cost = testCost });
                    //测试软件费用(硬件费用+追溯软件费用+开图软件费用)
                    List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal workingHoursInfosCost = workingHoursInfos.Sum(p => p.HardwareTotalPrice + p.TraceabilityDevelopmentFee + p.MappingDevelopmentFee);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.CSRJF , Cost = workingHoursInfosCost });
                    //研发费(差旅费)
                    List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal travelExpensesCost = travelExpenses.Sum(p => p.Cost);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.CLF, Cost = travelExpensesCost });
                    //其他费用
                    List<RestsCost> rests = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.ProductId.Equals(partModel.ProductId));
                    decimal restsCost = rests.Sum(s => s.Cost);
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.QTFY, Cost = restsCost });
                    //总成本
                    decimal allCost = handPieceCostsCost+qADepartmentQCsCost + mouldInventoriesCost + equipmentInfosCost + toolingFixtureInfosCost + testCost + workingHoursInfosCost + travelExpensesCost + restsCost;
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.ZCB, Cost = allCost });
                    nreCostMessageModel.NreCostModels.Add(new NreCostModel() { Name = StaticName.BZ, Cost = 0.0M });
                    nreCostMessageModels.Add(nreCostMessageModel);
                }
                quotationListDto.NreCost = nreCostMessageModels;
                #endregion
                //内部核价信息
                #region 内部核价信息
                //取正义 核价表 某个模组的核价 
                List<PricingMessage> pricingMessages = new();
                foreach (PartModel partModel in partModels)
                {
                    ModelCount priceEvaluationTableDtos = await _resourceModelCount.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (!string.IsNullOrEmpty(priceEvaluationTableDtos.TableAllJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(priceEvaluationTableDtos.TableAllJson);
                    PricingMessage pricingMessageModel = new PricingMessage();
                    var CostValue = 0.0M;
                    pricingMessageModel.ModelCountId = partModel.ProductId;
                    pricingMessageModel.PricingMessageName = partModel.ParName;
                    if (priceEvaluationTableDto.Material is null) priceEvaluationTableDto.Material = new();
                    CostValue = priceEvaluationTableDto.Material.Sum(p => p.TotalMoneyCyn);
                    if (pricingMessageModel.PricingMessageModels is null) pricingMessageModel.PricingMessageModels = new();
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.BOMCB, CostValue = CostValue });
                    if (priceEvaluationTableDto.ManufacturingCost is null) priceEvaluationTableDto.ManufacturingCost = new();//手动给值附上实例 避免没值报错问题                    
                    //直接制造成本+间接制造成本
                    CostValue = priceEvaluationTableDto.ManufacturingCost.Where(p => p.CostType.Equals(CostType.Total)).Sum(p => p.Subtotal);
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.ZZCB, CostValue = CostValue });
                    if (priceEvaluationTableDto.LossCost is null) priceEvaluationTableDto.LossCost = new();
                    CostValue = priceEvaluationTableDto.LossCost.Sum(p => p.WastageCost);
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.SHCB, CostValue = CostValue });
                    if (priceEvaluationTableDto.OtherCostItem is null) priceEvaluationTableDto.OtherCostItem = new();
                    CostValue = priceEvaluationTableDto.OtherCostItem.LogisticsFee;
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.WLCB, CostValue = CostValue });
                    CostValue = priceEvaluationTableDto.LossCost.Sum(p => p.MoqShareCount);
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.MOQFTCB, CostValue = CostValue });
                    CostValue = priceEvaluationTableDto.OtherCostItem.QualityCost;
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.QT, CostValue = CostValue });
                    CostValue = priceEvaluationTableDto.TotalCost;
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.ZCB, CostValue = CostValue });
                    pricingMessageModel.PricingMessageModels.Add(new PricingMessageModel() { Name = StaticName.BZ });
                    pricingMessages.Add(pricingMessageModel);
                }
                quotationListDto.PricingMessage = pricingMessages;
                #endregion
                //报价策略
                #region 报价策略
                List<BiddingStrategyModel> biddingStrategyModels = new List<BiddingStrategyModel>();
                foreach (PartModel partModel in partModels)
                {
                    BiddingStrategy biddingStrategy = await _financeBiddingStrategy.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(partModel.ProductId));
                    if (biddingStrategy is null) biddingStrategy = new();
                    ModelCount modelCount = new();
                    modelCount = await _resourceModelCount.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partModel.ProductId));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (!string.IsNullOrWhiteSpace(modelCount.TableAllJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCount.TableAllJson);
                    DynamicUnitPriceOffers dynamicUnitPriceOffers = new();
                    dynamicUnitPriceOffers = await _resourceDynamicUnitPriceOffers.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(partModel.ProductId));
                    if (dynamicUnitPriceOffers is null) dynamicUnitPriceOffers = new();
                    biddingStrategyModels.Add(new BiddingStrategyModel() { ModelCountId = partModel.ProductId, Product = partModel.ParName, Cost = priceEvaluationTableDto.TotalCost, GrossMargin = dynamicUnitPriceOffers.OffeGrossMargin, Price = dynamicUnitPriceOffers.OfferUnitPrice, Commission = biddingStrategy.Commission, GrossMarginCommission = biddingStrategy .GrossMarginCommission});
                }
                quotationListDto.BiddingStrategy = biddingStrategyModels;
                #endregion
                //费用表
                #region 费用汇总(费用表)  
                quotationListDto.ExpensesStatement = expensesStatements;
                #endregion                
                return quotationListDto;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        /// <summary>
        /// 获取 第一个页面最初的年份
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private async Task<List<int>> GetYear(long processId)
        {
            List<ModelCountYear> modelCountYears = await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
            List<int> yearList = modelCountYears.Select(p => p.Year).Distinct().OrderBy(p => p).ToList();
            return yearList;
        }
        /// <summary>
        /// 计算销售毛利 返利后销售收入-销售成本-返利金额-税费损失
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> TradingProfit(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            //返利后销售收入
            decimal rebateSalesRevenue = await RebateSalesRevenue(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            //销售成本
            decimal sellingCost = await SellingCost(processId, partId, year);
            //返利金额
            decimal rebateMoney = await RebateMoney(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            //税费损失
            decimal taxesduesLoss = await TaxesduesLoss(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            return rebateSalesRevenue - sellingCost - rebateMoney - taxesduesLoss;
        }
        /// <summary>
        /// 返利后销售收入与收入   收入(Income方法)-折让(Allowance方法)
        /// </summary>
        /// <param name="processId">流程id</param>
        /// <param name="partId">零件id</param>
        /// <param name="year">年份</param>
        /// <param name="lastYearUnitPrice">上一年单价 如果为0 获取今年单价</param>
        /// <param name="grossMargin">毛利率</param>
        ///  <param name="YearUnitPrice">单价</param>
        /// <returns></returns>
        private async Task<decimal> RebateSalesRevenue(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            //收入
            decimal income = await Income(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            //折让
            var allowance = await Allowance(processId, year, income);
            return income - allowance;
        }
        /// <summary>
        /// 收入  销售数量(SalesQuantity方法)*售价(也就是单价)(SellingPrice方法)
        /// </summary>
        /// <param name="processId">流程id</param>
        /// <param name="partId">零件id</param>
        /// <param name="year">年份</param>
        /// <param name="lastYearUnitPrice">上一年单价 如果为0 获取今年单价</param>
        /// <param name="grossMargin">毛利率</param>
        /// <param name="YearUnitPrice">单价</param>
        /// <returns></returns>
        private async Task<decimal> Income(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            //销售数量
            var salesQuantity = await SalesQuantity(processId, partId, year);
            //售价(单价)
            var sellingPrice = await SellingPrice(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            return salesQuantity * sellingPrice;
        }
        /// <summary>
        /// 销售数量  终端走量*车辆的份额比率*模组配比数量 (需求录入中的模组总量)
        /// </summary>
        /// <param name="processId">流程id</param>
        /// <param name="partId">零件id</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        private async Task<int> SalesQuantity(long processId, long partId, int year)
        {
            ModelCountYear modelCount = new ModelCountYear();
            if (partId is 0)
            {
                modelCount = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
            }
            else
            {
                //销售数量=模组总量  取模组数量中的模组总量(某一个年份)
                modelCount = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(partId) && p.Year.Equals(year));
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
        /// 售价(单价)  sop(当年)直接取 sop年的单价    sop+N(上一年单价*(1-年将率))
        /// </summary>
        /// <param name="processId">流程id</param>
        /// <param name="partId">零件id</param>
        /// <param name="year">年份</param>
        /// <param name="lastYearUnitPrice">上一年单价 如果为0 获取今年单价</param>
        /// <param name="grossMargin">毛利率</param>
        /// <param name="YearUnitPrice">单价</param>
        /// <returns></returns>
        private async Task<decimal> SellingPrice(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            if (YearUnitPrice is not 0.0M)//如果传入单价直接返回
            {
                return YearUnitPrice;
            }
            if (grossMargin is 0.0M)//如果毛利率等于0  则直接取  营销部录入(第一张表的)  产品信息的 单颗产品目标价
            {
                var prop = (from a in await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partId))
                            join b in await _resourceProductInformation.GetAllListAsync(p => p.AuditFlowId.Equals(processId)) on a.Product equals b.Product
                            select new
                            {
                                CustomerTargetPrice = b.CustomerTargetPrice
                            }).FirstOrDefault();
                return prop.CustomerTargetPrice;
            }
            if (lastYearUnitPrice is 0.0M)//如果不传上一年的单价  就证明 是获取今年的单价  
            {
                //单位价格调用正义的接口 拿到单位成本算单价 传的参数有  流程id  和零件 id
                decimal unitCost = 0M; //先写死 单位成本
                if (year is 0)
                {
                    unitCost = 0M; //全模组的单位成本
                    ModelCount modelCount = await _resourceModelCount.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Id.Equals(partId));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (modelCount is not null && !string.IsNullOrWhiteSpace(modelCount.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCount.TableJson);
                    if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                }
                else
                {
                    unitCost = 0M; //某个模组的单位成本
                    ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(partId) && p.Year.Equals(year));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (modelCountYear is not null && !string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                    if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                }

                return unitCost / (1 - grossMargin / 100);//单价
            }
            else //如果传的就是 sop+N年   lastYearUnitPrice*(1-年将率)
            {
                Requirement requirement = await _resourceRequirement.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
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
        /// 计算某一年的全模组的单价
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private async Task<decimal> SomeSellingPrice(long processId, long year)
        {
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal AllTotalCost = 0.0M;
            foreach (var modelcountid in AllModelCountId)
            {
                ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(modelcountid) && p.Year == year);
                PriceEvaluationTableDto priceEvaluationTableDto = new();
                if (modelCountYear is not null && !string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                if (priceEvaluationTableDto.TotalCost is not 0.0M) AllTotalCost += priceEvaluationTableDto.TotalCost;
            }
            return AllTotalCost;
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
            Requirement requirement = await _resourceRequirement.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.Year.Equals(year));
            var oneTimeDiscountRate = 0.0M;
            if (requirement is not null)
            {
                oneTimeDiscountRate = requirement.OneTimeDiscountRate;
            }
            return income * (oneTimeDiscountRate / 100);
        }
        /// <summary>
        /// 计算销售成本 单位成本(取sop年的单中的单位成本)*销售数量(需求录入的模组总量  思路修改  要获取单年的 方法如下SalesQuantity)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> SellingCost(long processId, long partId, int year)
        {
            //单位成本 某一年
            decimal unitCost = 151.19M;
            ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(partId) && p.Year.Equals(year));
            PriceEvaluationTableDto priceEvaluationTableDto = new();
            if (!string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
            if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
            //销售数量  某一年
            int salesQuantity = await SalesQuantity(processId, partId, year); ;
            return unitCost * salesQuantity;
        }
        /// <summary>
        /// 计算返利金额  收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> RebateMoney(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            //返利比例
            decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
            //收入
            var income = await Income(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            return (rebatePercentage / 100) * income;
        }
        /// <summary>
        /// 计算 全模组全生命周期 返利金额 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllRebateMoney(long processId, decimal grossMargin)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal AllRebateMoney = 0.0M;

            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = 0.0M;
                foreach (var Year in YearList)
                {
                    //返利比例
                    decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                    //上一年的单价
                    //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCountId, grossMargin, Year-1, 0.0M, 0.0M);
                    //销售数量
                    var salesQuantity = await SalesQuantity(processId, ModelCountId, Year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
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
        /// 计算 某个单价 全模组全生命周期 返利金额 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> SomeAllRebateMoney(long processId, decimal grossMargin, List<DynamicProductBoardModel> AllUnitPrice)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            decimal AllRebateMoney = 0.0M;

            foreach (var unitPrice in AllUnitPrice)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = unitPrice.UnitPrice;
                foreach (var Year in YearList)
                {
                    //返利比例
                    decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                    //销售数量
                    var salesQuantity = await SalesQuantity(processId, unitPrice.ModelCountId, Year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, unitPrice.ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    var everyIncome = salesQuantity * sellingPrice;
                    AllRebateMoney += (rebatePercentage / 100) * everyIncome;
                }
            }
            return AllRebateMoney;
        }
        /// <summary>
        /// 计算 某一年 全模组全生命周期 返利金额 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllRebateMoneyYear(long processId, int year, decimal grossMargin)
        {

            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal AllRebateMoney = 0.0M;
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //返利比例
                decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ModelCountId, year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                //某一年的
                var everyIncome = salesQuantity * sellingPrice;
                AllRebateMoney += rebatePercentage * everyIncome;
            }
            return AllRebateMoney;
        }
        /// <summary>
        /// 计算 某一年 某个模组 全模组全生命周期 返利金额 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> SomeModelCountRebateMoneyYear(long processId, long ModelCountId, int year, decimal grossMargin, decimal income)
        {

         
            decimal AllRebateMoney = 0.0M;
            //返利比例
            decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
          
            
            AllRebateMoney += rebatePercentage/100 * income;

            return AllRebateMoney;
        }
        /// <summary>
        /// 计算 全模组全生命周期 税费损失 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllAddTaxRate(long processId, decimal grossMargin)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal AddTaxRate = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = 0.0M;
                foreach (var Year in YearList)
                {
                    //返利比例
                    decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                    //上一年的单价
                    //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                    //每一个模组的收入
                    //var everyIncome = await Income(processId, ModelCount, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    //销售数量
                    var salesQuantity = await SalesQuantity(processId, ModelCountId, Year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    var everyIncome = salesQuantity * sellingPrice;
                    //增值税率
                    ManufacturingCostInfo manufacturingCostInfo = await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(Year));
                    if (manufacturingCostInfo is null) manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year).FirstOrDefault();
                    decimal addTaxRate = manufacturingCostInfo.VatRate;
                    decimal TaxRate = ((rebatePercentage / 100) * everyIncome) * (addTaxRate / 100);
                    AddTaxRate += TaxRate;
                }
            }
            return AddTaxRate;
        }
        /// <summary>
        /// 计算 某个单价 全模组全生命周期 税费损失 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> SomeAllAddTaxRate(long processId, decimal grossMargin, List<DynamicProductBoardModel> AllUnitPrice)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            decimal AddTaxRate = 0.0M;
            foreach (var unitPrice in AllUnitPrice)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = unitPrice.UnitPrice;
                foreach (var Year in YearList)
                {
                    //返利比例
                    decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(Year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                    //上一年的单价
                    //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                    //每一个模组的收入
                    //var everyIncome = await Income(processId, ModelCount, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    //销售数量
                    var salesQuantity = await SalesQuantity(processId, unitPrice.ModelCountId, Year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, unitPrice.ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    var everyIncome = salesQuantity * sellingPrice;
                    //增值税率
                    ManufacturingCostInfo manufacturingCostInfo = await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(Year));
                    if (manufacturingCostInfo is null) manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year).FirstOrDefault();
                    decimal addTaxRate = manufacturingCostInfo.VatRate;
                    decimal TaxRate = ((rebatePercentage / 100) * everyIncome) * (addTaxRate / 100);
                    AddTaxRate += TaxRate;
                }
            }
            return AddTaxRate;
        }
        /// <summary>
        /// 计算 某一年 全模组全生命周期 税费损失 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllAddTaxRateYear(long processId, int year, decimal grossMargin)
        {

            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal AddTaxRate = 0.0M;
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //返利比例
                decimal rebatePercentage = _resourceRequirement.GetAllList(p => p.AuditFlowId.Equals(processId)).Where(p => p.Year.Equals(year)).Select(p => p.AnnualRebateRequirements).FirstOrDefault();
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, year-1, 0.0M, 0.0M);
                //每一个模组的收入
                //var everyIncome = await Income(processId, ModelCount, grossMargin, year, lastYearUnitPrice, 0.0M);
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ModelCountId, year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                var everyIncome = salesQuantity * sellingPrice;
                //增值税率
                ManufacturingCostInfo manufacturingCostInfo = await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(year));
                if (manufacturingCostInfo is null) manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year).FirstOrDefault();
                decimal addTaxRate = manufacturingCostInfo.VatRate;
                AddTaxRate += (rebatePercentage * everyIncome) * addTaxRate / 100;
            }
            return AddTaxRate;
        }
        /// <summary>
        /// 计算 某一年 某个模组 全生命周期 税费损失 之和   收入(Income方法)*返利比例(取需求录入 要求中的  年度返利要求)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> SomeModelCountAddTaxRateYear(decimal FL,int year)
        {

           
            decimal AddTaxRate = 0.0M;
           
            ManufacturingCostInfo manufacturingCostInfo = await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(year));
            if (manufacturingCostInfo is null) manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year).FirstOrDefault();
            decimal addTaxRate = manufacturingCostInfo.VatRate;
            AddTaxRate += FL * addTaxRate / 100;
            return AddTaxRate;
        }
        /// <summary>
        /// 计算 全生命周期的  收入
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> Allincome(long processId, decimal grossMargin)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLYearIncome = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = 0.0M;
                foreach (var Year in YearList)
                {
                    //上一年的单价
                    //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year-1, 0.0M, 0.0M);
                    //每一年的单价
                    decimal ALLYearPrice = await SellingPrice(processId, ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = ALLYearPrice;
                    //销售数量
                    decimal Count = await SalesQuantity(processId, ModelCountId, Year);
                    decimal prop = ALLYearPrice * Count;
                    ALLYearIncome += prop;
                }
            }
            return ALLYearIncome;
        }
        /// <summary>
        /// 计算 动态单价汇总表 全生命周期的  收入
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllincomeDynamic(long processId, decimal grossMargin, List<DynamicProductBoardModel> unitPrice)
        {
            //年份
            List<int> YearList = await GetYear(processId);

            decimal ALLYearIncome = 0.0M;
            foreach (DynamicProductBoardModel boardModel in unitPrice)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = boardModel.UnitPrice;
                foreach (var Year in YearList)
                {
                    //每一年的单价
                    decimal ALLYearPrice = await SellingPrice(processId, boardModel.ModelCountId, grossMargin, Year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = ALLYearPrice;
                    //销售数量
                    decimal Count = await SalesQuantity(processId, boardModel.ModelCountId, Year);
                    decimal prop = ALLYearPrice * Count;
                    ALLYearIncome += prop;
                }
            }
            return ALLYearIncome;
        }
        /// <summary>
        /// 计算 某一年 全生命周期的  收入
        /// </summary>
        /// <returns></returns>
        private async Task<(decimal, decimal)> AllincomeYear(long processId, int year, decimal grossMargin, decimal unitPrice)
        {
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLYearIncome = 0.0M;
            //上一年的单价
            decimal lastYearUnitPrice = unitPrice;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, year-1, 0.0M, 0.0M);
                //每一年的单价
                decimal ALLYearPrice = await SellingPrice(processId, ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = ALLYearPrice;
                //销售数量
                decimal Count = await SalesQuantity(processId, ModelCountId, year);
                ALLYearIncome += ALLYearPrice * Count;
            }
            return (ALLYearIncome, lastYearUnitPrice);
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
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLallowance = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
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
                    var salesQuantity = await SalesQuantity(processId, ModelCountId, year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    var everyIncome = salesQuantity * sellingPrice;
                    //折让
                    ALLallowance += await Allowance(processId, year, everyIncome);
                }
            }
            return ALLallowance;
        }
        /// <summary>
        /// 计算 某个单价  前声明周期的  折让
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="grossMargin"></param>
        /// <returns></returns>
        private async Task<decimal> SomeAllAllowance(long processId, decimal grossMargin, List<DynamicProductBoardModel> AllUnitPrice)
        {
            //年份
            List<int> YearList = await GetYear(processId);

            decimal ALLallowance = 0.0M;
            foreach (var unitPrice in AllUnitPrice)//循环每一个模组
            {
                //上一年的单价
                decimal lastYearUnitPrice = unitPrice.UnitPrice;
                foreach (var year in YearList)
                {
                    //销售数量
                    var salesQuantity = await SalesQuantity(processId, unitPrice.ModelCountId, year);
                    //售价(单价)
                    var sellingPrice = await SellingPrice(processId, unitPrice.ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                    lastYearUnitPrice = sellingPrice;
                    var everyIncome = salesQuantity * sellingPrice;
                    //折让
                    ALLallowance += await Allowance(processId, year, everyIncome);
                }
            }
            return ALLallowance;
        }
        /// <summary>
        /// 计算 某一年 全生命周期的  折让
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="grossMargin"></param>
        /// <returns></returns>
        private async Task<decimal> AllAllowanceYear(long processId, int year, decimal grossMargin)
        {

            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLallowance = 0.0M;
            //上一年的单价
            decimal lastYearUnitPrice = 0.0M;
            foreach (var ModelCountId in AllModelCountId)//循环每一个模组
            {
                //上一年的单价
                //decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, year-1, 0.0M, 0.0M);
                //每一个模组的收入
                //var everyIncome = await Income(processId, ModelCount, grossMargin, year, lastYearUnitPrice, 0.0M);
                //销售数量
                var salesQuantity = await SalesQuantity(processId, ModelCountId, year);
                //售价(单价)
                var sellingPrice = await SellingPrice(processId, ModelCountId, grossMargin, year, lastYearUnitPrice, 0.0M);
                lastYearUnitPrice = sellingPrice;
                var everyIncome = salesQuantity * sellingPrice;
                //折让
                ALLallowance += await Allowance(processId, year, everyIncome);
            }
            return ALLallowance;
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
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLsellingCost = 0.0M;
            foreach (var Year in YearList)
            {
                foreach (var ModelCountId in AllModelCountId)//循环每一个模组
                {
                    //每一个模组每年的单位成本
                    decimal unitCost = 0.0M;//todo 正义接口
                    ModelCountYear modelCountYear = await _resourceModelCountYear.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(processId) && p.ModelCountId.Equals(ModelCountId) && p.Year.Equals(Year));
                    PriceEvaluationTableDto priceEvaluationTableDto = new();
                    if (!string.IsNullOrWhiteSpace(modelCountYear.TableJson)) priceEvaluationTableDto = JsonConvert.DeserializeObject<PriceEvaluationTableDto>(modelCountYear.TableJson);
                    if (priceEvaluationTableDto.TotalCost is not 0.0M) unitCost = priceEvaluationTableDto.TotalCost;
                    //每一个模组每年的 销售数量
                    decimal Count = await SalesQuantity(processId, ModelCountId, Year);
                    decimal prop = unitCost * Count;
                    ALLsellingCost += prop;
                }
            }
            return ALLsellingCost;
        }      
        /// <summary>
        /// 计算税费损失 返利金额(RebateMoney方法)*增值税率(取增值税率表)
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> TaxesduesLoss(long processId, long partId, decimal grossMargin, int year, decimal lastYearUnitPrice, decimal YearUnitPrice)
        {
            //返利金额
            decimal rebateMoney = await RebateMoney(processId, partId, grossMargin, year, lastYearUnitPrice, YearUnitPrice);
            //增值税率
            ManufacturingCostInfo manufacturingCostInfo = await _resourceManufacturingCostInfo.FirstOrDefaultAsync(p => p.Year.Equals(year));
            if(manufacturingCostInfo is  null) manufacturingCostInfo = _resourceManufacturingCostInfo.GetAllList().OrderByDescending(p => p.Year).FirstOrDefault();          
            decimal addTaxRate = (manufacturingCostInfo.VatRate) / 100;
            return rebateMoney * addTaxRate;
        }
        #region 暂时报废的方法
        /// <summary>
        /// 计算 全生命周期的  售价
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> AllPrice(long processId, decimal grossMargin)
        {
            //年份
            List<int> YearList = await GetYear(processId);
            List<long> AllModelCountId = _resourceModelCount.GetAllList(p => p.AuditFlowId.Equals(processId)).Select(p => p.Id).ToList();
            decimal ALLYearUnitPrice = 0.0M;
            foreach (var Year in YearList)
            {
                foreach (var ModelCount in AllModelCountId)//循环每一个模组
                {
                    //上一年的单价
                    decimal lastYearUnitPrice = await SellingPrice(processId, ModelCount, grossMargin, Year - 1, 0.0M, 0.0M);
                    //每一年的单价
                    ALLYearUnitPrice += await SellingPrice(processId, ModelCount, grossMargin, Year - 1, lastYearUnitPrice, 0.0M);
                }
            }
            return ALLYearUnitPrice;
        }
        #endregion

    }
    /// <summary>
    /// 内部目标价规则
    /// </summary>
    public enum TargetPriceRule
    {
        /// <summary>
        /// 外摄显象(毛利率)
        /// </summary>
        peripheralDisplays = 18,
        /// <summary>
        /// 环视感知(毛利率)
        /// </summary>
        lookAround = 28,
        /// <summary>
        /// 舱内监测(毛利率)
        /// </summary>
        cabinMonitor = 24,
    }
}
