using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.DataTableVersion;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rougamo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 报表版本服务
    /// </summary>
    [ParameterValidator]
    public class DataTableVersionAppService : FinanceAppServiceBase
    {
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly AnalyseBoardAppService _analyseBoardAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="auditFlowRepository"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        /// <param name="analyseBoardAppService"></param>
        public DataTableVersionAppService(IRepository<AuditFlow, long> auditFlowRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, AnalyseBoardAppService analyseBoardAppService)
        {
            _auditFlowRepository = auditFlowRepository;
            _modelCountRepository = modelCountRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _analyseBoardAppService = analyseBoardAppService;
        }


        #region 下拉框数据源接口

        /// <summary>
        /// 获取项目名称列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<string>> GetQuoteProjectNameList(GetQuoteProjectNameListInput input)
        {
            var data = _auditFlowRepository.GetAll().Select(p => p.QuoteProjectName).Distinct()
                .WhereIf(!input.QuoteProjectName.IsNullOrWhiteSpace(), p => p.Contains(input.QuoteProjectName));

            var count = await data.CountAsync();

            var result = await data.PageBy(input)
                .ToListAsync();

            return new PagedResultDto<string>(count, result);
        }

        /// <summary>
        /// 根据 项目名称 获取 核报价项目信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<QuoteProjectInfo>> GetQuoteProjectInfoListByQuoteProjectName(GetQuoteProjectInfoListByQuoteProjectNameInput input)
        {
            var data = _auditFlowRepository.GetAll()
                .WhereIf(!input.QuoteProjectName.IsNullOrWhiteSpace(), p => p.QuoteProjectName == input.QuoteProjectName);
            var count = await data.CountAsync();
            var result = await data.PageBy(input)
                .Select(p => new QuoteProjectInfo { Id = p.Id, QuoteProjectName = p.QuoteProjectName, IsValid = p.IsValid, QuoteVersion = p.QuoteVersion })
                .ToListAsync();
            return new PagedResultDto<QuoteProjectInfo>(count, result);
        }

        /// <summary>
        /// 根据两个流程号，获取产品列表（字典明细表的Id和DisplayName）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<ProductList>> GetProductListByAuditFlowIds(GetProductListByAuditFlowIdsInput input)
        {
            var data = _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.QuoteVersion1AuditFlowId || p.AuditFlowId == input.QuoteVersion2AuditFlowId)
                .Select(p => new ProductList { Id = p.Id, Product = p.Product });
            var count = await data.CountAsync();
            var result = await data.PageBy(input).ToListAsync();
            return new PagedResultDto<ProductList>(count, result);
        }

        #endregion

        #region 成本明细差异表

        /// <summary>
        /// 获取成本明细差异表-bom成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<CostDetailVarianceMaterial>> GetCostDetailVarianceMaterial(GetCostDetailVarianceMaterialInput input)
        {
            var version1 = await _modelCountRepository.GetAll().Where(p => p.Product == input.Product && p.AuditFlowId == input.QuoteVersion1AuditFlowId).Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();
            var version2 = await _modelCountRepository.GetAll().Where(p => p.Product == input.Product && p.AuditFlowId == input.QuoteVersion2AuditFlowId).Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();

            var vdto1 = ObjectMapper.Map<List<CostDetailVarianceMaterialInfo>>(JsonConvert.DeserializeObject<PriceEvaluationTableDto>(version1).Material);
            var vdto2 = ObjectMapper.Map<List<CostDetailVarianceMaterialInfo>>(JsonConvert.DeserializeObject<PriceEvaluationTableDto>(version2).Material);
            vdto1.ForEach(p => p.TotalMoney = p.AssemblyCount.To<decimal>() * p.MaterialPrice);
            vdto2.ForEach(p => p.TotalMoney = p.AssemblyCount.To<decimal>() * p.MaterialPrice);

            var data = vdto1.Select(p => new CostDetailVarianceMaterial { SuperType = p.SuperType, CategoryName = p.CategoryName, TypeName = p.TypeName })
                .Union(vdto2.Select(p => new CostDetailVarianceMaterial { SuperType = p.SuperType, CategoryName = p.CategoryName, TypeName = p.TypeName })).ToList();

            var v1 = vdto1.GroupBy(p => new { p.SuperType, p.CategoryName, p.TypeName }).Select(p => new CostDetailVarianceMaterialInfo
            {
                SuperType = p.Key.SuperType,
                CategoryName = p.Key.CategoryName,
                TypeName = p.Key.TypeName,
                AssemblyCount = p.Sum(o => o.AssemblyCount),
                MaterialPrice = p.Sum(o => o.MaterialPrice),
                MaterialPriceCyn = p.Sum(o => o.MaterialPriceCyn),
                TotalMoneyCyn = p.Sum(o => o.TotalMoneyCyn),
                TotalMoney = p.Sum(o => o.TotalMoney),
            });
            var v2 = vdto1.GroupBy(p => new { p.SuperType, p.CategoryName, p.TypeName }).Select(p => new CostDetailVarianceMaterialInfo
            {
                SuperType = p.Key.SuperType,
                CategoryName = p.Key.CategoryName,
                TypeName = p.Key.TypeName,
                AssemblyCount = p.Sum(o => o.AssemblyCount),
                MaterialPrice = p.Sum(o => o.MaterialPrice),
                MaterialPriceCyn = p.Sum(o => o.MaterialPriceCyn),
                TotalMoneyCyn = p.Sum(o => o.TotalMoneyCyn),
                TotalMoney = p.Sum(o => o.TotalMoney),
            });
            foreach (var item in data)
            {
                item.Version1 = v1.FirstOrDefault(p => p.TypeName == item.TypeName);
                item.Version2 = v2.FirstOrDefault(p => p.TypeName == item.TypeName);
                item.Variance = new VarianceCostDetailVarianceMaterialInfo
                {
                    AssemblyCount = VarianceExtensions.Variance(item?.Version1.AssemblyCount, item?.Version2.AssemblyCount),
                    MaterialPrice = VarianceExtensions.Variance(item?.Version1.MaterialPrice, item?.Version2.MaterialPrice),
                    MaterialPriceCyn = VarianceExtensions.Variance(item?.Version1.MaterialPriceCyn, item?.Version2.MaterialPriceCyn),
                    TotalMoneyCyn = VarianceExtensions.Variance(item?.Version1.TotalMoneyCyn, item?.Version2.TotalMoneyCyn),
                    TotalMoney = VarianceExtensions.Variance(item?.Version1.TotalMoney, item?.Version2.TotalMoney),
                };
            }
            return new ListResultDto<CostDetailVarianceMaterial>(data);
        }

        /// <summary>
        /// 获取成本明细差异表-制造成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<CostDetailVarianceManufacturingCost> GetCostDetailVarianceManufacturingCost(GetCostDetailVarianceManufacturingCostInput input)
        {
            var version1 = await _modelCountRepository.GetAll().Where(p => p.Product == input.Product && p.AuditFlowId == input.QuoteVersion1AuditFlowId).Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();
            var version2 = await _modelCountRepository.GetAll().Where(p => p.Product == input.Product && p.AuditFlowId == input.QuoteVersion2AuditFlowId).Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();

            var v1 = ObjectMapper.Map<CostDetailVarianceManufacturingCostInfo>(JsonConvert.DeserializeObject<PriceEvaluationTableDto>(version1).ManufacturingCost);
            var v2 = ObjectMapper.Map<CostDetailVarianceManufacturingCostInfo>(JsonConvert.DeserializeObject<PriceEvaluationTableDto>(version2).ManufacturingCost);


            var data = new CostDetailVarianceManufacturingCost
            {
                Version1 = v1,
                Version2 = v2,
                Variance = new VarianceCostDetailVarianceManufacturingCostInfo
                {
                    DirectLabor = VarianceExtensions.Variance(v1.DirectLabor, v2.DirectLabor),
                    EquipmentDepreciation = VarianceExtensions.Variance(v1.EquipmentDepreciation, v2.EquipmentDepreciation),
                    LineChangeCost = VarianceExtensions.Variance(v1.LineChangeCost, v2.LineChangeCost),
                    ManufacturingExpenses = VarianceExtensions.Variance(v1.ManufacturingExpenses, v2.ManufacturingExpenses)
                }
            };
            return data;
        }
        #endregion


        #region 整体差异报表

        /// <summary>
        /// 获取成本明细差异表-项目整体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<CostDetailVariance>> GetCostDetailVariance(GetCostDetailVarianceInput input)
        {
            var version1 = await _analyseBoardAppService.GetQuotationList(input.QuoteVersion1AuditFlowId);
            var version2 = await _analyseBoardAppService.GetQuotationList(input.QuoteVersion2AuditFlowId);

            var dto1 = version1.PricingMessage.SelectMany(p => p.PricingMessageModels).ToList();
            var dto2 = version2.PricingMessage.SelectMany(p => p.PricingMessageModels).ToList();

            dto1.Remove(dto1.FirstOrDefault(p => p.Name == "MOQ分摊成本"));
            dto2.Remove(dto1.FirstOrDefault(p => p.Name == "MOQ分摊成本"));

            dto1.Remove(dto1.FirstOrDefault(p => p.Name == "备注"));
            dto2.Remove(dto1.FirstOrDefault(p => p.Name == "备注"));

            dto1.FirstOrDefault(p => p.Name == "其他（质量+财务成本）").Name = "质量成本";
            dto2.FirstOrDefault(p => p.Name == "其他（质量+财务成本）").Name = "质量成本";

            var d1 = version1.BiddingStrategy;
            var d2 = version2.BiddingStrategy;

            dto1.Add(new PricingMessageModel { Name = "单位售价", CostValue = d1.Sum(p => p.Price) });
            dto1.Add(new PricingMessageModel { Name = "毛利率", CostValue = d1.Sum(p => p.GrossMargin) });

            dto2.Add(new PricingMessageModel { Name = "单位售价", CostValue = d2.Sum(p => p.Price) });
            dto2.Add(new PricingMessageModel { Name = "毛利率", CostValue = d2.Sum(p => p.GrossMargin) });

            var left = dto1.LeftJoin(dto2, p => p.Name, p => p.Name, (a, b) => new CostDetailVariance { Name = a.Name, Version1 = a.CostValue, Version2 = b.CostValue, Variance = VarianceExtensions.Variance(a?.CostValue, b?.CostValue) });
            var right = dto2.RightJoin(dto1, p => p.Name, p => p.Name, (a, b) => new CostDetailVariance { Name = a.Name, Version1 = a.CostValue, Version2 = b.CostValue, Variance = VarianceExtensions.Variance(a?.CostValue, b?.CostValue) });

            return new ListResultDto<CostDetailVariance>(left.Union(right).ToList());
        }

        #endregion
    }
}
