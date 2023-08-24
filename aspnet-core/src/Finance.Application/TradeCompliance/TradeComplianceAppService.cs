using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Finance.Audit;
using Finance.Entering;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.Infrastructure.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.TradeCompliance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 贸易合规后端API接口类
    /// </summary>
    public class TradeComplianceAppService : ApplicationService
    {
        private readonly IRepository<TradeComplianceCheck, long> _tradeComplianceCheckRepository;
        private readonly IRepository<ProductMaterialInfo, long> _productMaterialInfoRepository;
        private readonly IRepository<EnteringElectronic, long> _enteringElectronicRepository;
        private readonly IRepository<StructureElectronic, long> _structureElectronicRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvalRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
        private readonly IObjectMapper _objectMapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeComplianceCheckRepository"></param>
        /// <param name="productMaterialInfoRepository"></param>
        /// <param name="enteringElectronicRepository"></param>
        /// <param name="structureElectronicRepository"></param>
        /// <param name="priceEvalRepository"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        /// <param name="priceEvaluationGetAppService"></param>
        /// <param name="objectMapper"></param>
        /// <param name="modelCountYearRepository"></param>
        public TradeComplianceAppService(IRepository<TradeComplianceCheck, long> tradeComplianceCheckRepository, IRepository<ProductMaterialInfo, long> productMaterialInfoRepository, IRepository<PriceEvaluation, long> priceEvalRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, PriceEvaluationGetAppService priceEvaluationGetAppService, IObjectMapper objectMapper, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<EnteringElectronic, long> enteringElectronicRepository, IRepository<StructureElectronic, long> structureElectronicRepository)
        {
            _tradeComplianceCheckRepository = tradeComplianceCheckRepository;
            _productMaterialInfoRepository = productMaterialInfoRepository;
            _priceEvalRepository = priceEvalRepository;
            _modelCountRepository = modelCountRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _priceEvaluationGetAppService = priceEvaluationGetAppService;
            _objectMapper = objectMapper;
            _modelCountYearRepository = modelCountYearRepository;
            _enteringElectronicRepository = enteringElectronicRepository;
            _structureElectronicRepository = structureElectronicRepository;
        }

        private async Task<int> GetFristSopYear(TradeComplianceInputDto input)
        {
            int sopYear = 0;
            var yearList = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId ==  input.AuditFlowId && p.ProductId == input.ProductId);
            if(yearList.Count > 0)
            {
                sopYear = yearList.Min(x => x.Year);
            }
            return sopYear;
        }

        /// <summary>
        /// 获取贸易合规界面数据(计算得出)
        /// </summary>
        public virtual async Task<TradeComplianceCheckDto> GetTradeComplianceCheckByCalc(TradeComplianceInputDto input)
        {
            TradeComplianceCheckDto tradeComplianceCheckDto = new ();

            var productInfos = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.ProductId);
            if (productInfos.Count > 0)
            {
                var tradeComplianceList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId);
                if (tradeComplianceList.Count > 0)
                {
                    tradeComplianceCheckDto.TradeComplianceCheck = tradeComplianceList.FirstOrDefault();
                }
                else
                {
                    tradeComplianceCheckDto.TradeComplianceCheck = new();
                }
                tradeComplianceCheckDto.TradeComplianceCheck.AuditFlowId = input.AuditFlowId;
                tradeComplianceCheckDto.TradeComplianceCheck.ProductId = input.ProductId;
                tradeComplianceCheckDto.TradeComplianceCheck.ProductName = productInfos.FirstOrDefault().Product;
                tradeComplianceCheckDto.TradeComplianceCheck.ProductType = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == productInfos.FirstOrDefault().ProductType).DisplayName;

                var countryList = (from a in await _priceEvalRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId)
                 join b in await _financeDictionaryDetailRepository.GetAllListAsync() on a.Country equals b.Id
                 select b.DisplayName).ToList();
                tradeComplianceCheckDto.TradeComplianceCheck.Country = countryList.Count > 0 ? countryList.FirstOrDefault(): null;
                //存入部分合规信息生成ID
                var tradeComplianceCheckId = await _tradeComplianceCheckRepository.InsertOrUpdateAndGetIdAsync(tradeComplianceCheckDto.TradeComplianceCheck);

                decimal sumEccns = 0m;
                decimal sumPending = 0m;
                GetPriceEvaluationTableInput priceTableByPart = new()
                {
                    AuditFlowId = input.AuditFlowId,
                    ProductId = input.ProductId,
                    InputCount = 0,
                    Year = await GetFristSopYear(input),
                };
                var priceTable = await _priceEvaluationGetAppService.GetPriceEvaluationTable(priceTableByPart);//取核价表数据
                tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue = priceTable.TotalCost * 1.1m;

                var countries = await _financeDictionaryDetailRepository.GetAllListAsync(p => p.DisplayName.Equals(tradeComplianceCheckDto.TradeComplianceCheck.Country));
                bool isControlCountry = false;//是否是受管制国家

                if (countries.Count > 0 && !countries.FirstOrDefault().Id.Equals(FinanceConsts.Country_Other))
                {
                    isControlCountry = true;
                }

                tradeComplianceCheckDto.ProductMaterialInfos = new();
                foreach (var material in priceTable.Material)
                {
                    ProductMaterialInfo materialinfo;
                    var materialList = await _productMaterialInfoRepository.GetAllListAsync(p => p.TradeComplianceCheckId == tradeComplianceCheckId && p.MaterialCode == material.Sap && p.MaterialName == material.TypeName);
                    if (materialList.Count > 0)
                    {
                        materialinfo = materialList.FirstOrDefault();
                    }
                    else
                    {
                        materialinfo = new();
                        materialinfo.AuditFlowId = input.AuditFlowId;
                        materialinfo.TradeComplianceCheckId = tradeComplianceCheckId;

                        materialinfo.MaterialCode = material.Sap;
                        materialinfo.MaterialName = material.TypeName;
                    }
                    materialinfo.MaterialIdInBom = material.Id;
                    materialinfo.MaterialDetailName = material.MaterialName;

                    materialinfo.Count = material.AssemblyCount;
                    materialinfo.UnitPrice = material.MaterialPriceCyn;
                    materialinfo.Amount = material.TotalMoneyCyn;

                    tradeComplianceCheckDto.ProductMaterialInfos.Add(materialinfo);

                    string MaterialIdPrefix = materialinfo.MaterialIdInBom[..1];
                    if(MaterialIdPrefix.Equals(PriceEvaluationGetAppService.ElectronicBomName))
                    {
                        long elecId = long.Parse(materialinfo.MaterialIdInBom.Remove(0, 1));
                        //查询是否涉及
                        EnteringElectronic enteringElectronic = await _enteringElectronicRepository.FirstOrDefaultAsync(p => p.ElectronicId.Equals(elecId) && p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.ProductId);
                        materialinfo.ControlStateType = enteringElectronic.MaterialControlStatus;
                    }
                    else
                    {
                        long structId = long.Parse(materialinfo.MaterialIdInBom.Remove(0, 1));
                        StructureElectronic structureElectronic = await _structureElectronicRepository.FirstOrDefaultAsync(p => p.StructureId.Equals(structId) && p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.ProductId);
                        materialinfo.ControlStateType = structureElectronic.MaterialControlStatus;
                    }
                   


                    if(materialinfo.ControlStateType == FinanceConsts.EccnCode_Eccn || (isControlCountry && materialinfo.ControlStateType == FinanceConsts.EccnCode_Ear99))
                    {
                        sumEccns += materialinfo.Amount;
                    }
                    if (materialinfo.ControlStateType == FinanceConsts.EccnCode_Pending)
                    {
                        sumPending += materialinfo.Amount;
                    }
                    await _productMaterialInfoRepository.InsertOrUpdateAsync(materialinfo);
                }
                tradeComplianceCheckDto.TradeComplianceCheck.EccnPricePercent = sumEccns / tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue;
                tradeComplianceCheckDto.TradeComplianceCheck.PendingPricePercent = sumPending / tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue;
                tradeComplianceCheckDto.TradeComplianceCheck.AmountPricePercent = tradeComplianceCheckDto.TradeComplianceCheck.EccnPricePercent + tradeComplianceCheckDto.TradeComplianceCheck.PendingPricePercent;

                if ((tradeComplianceCheckDto.TradeComplianceCheck.AmountPricePercent > 0.1m && isControlCountry)||(tradeComplianceCheckDto.TradeComplianceCheck.AmountPricePercent > 0.25m))
                {
                    tradeComplianceCheckDto.TradeComplianceCheck.AnalysisConclusion = GeneralDefinition.TRADE_COMPLIANCE_NOT_OK;
                }
                else
                {
                    tradeComplianceCheckDto.TradeComplianceCheck.AnalysisConclusion = GeneralDefinition.TRADE_COMPLIANCE_OK;
                }
                await _tradeComplianceCheckRepository.InsertOrUpdateAsync(tradeComplianceCheckDto.TradeComplianceCheck);
            }
            else
            {
                throw new FriendlyException("获取零件信息失败，请检查零件信息");
            }
            return tradeComplianceCheckDto;
        }

        internal virtual async Task<bool> IsProductsTradeComplianceOK(long flowId)
        {
            bool isOk = true;
            var productIdList = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
            foreach (var product in productIdList)
            {
                TradeComplianceInputDto tradeComplianceInput = new()
                {
                    AuditFlowId = flowId,
                    ProductId = product.Id
                };
                var tradeComplianceCheck = await GetTradeComplianceCheckByCalc(tradeComplianceInput);
                if (tradeComplianceCheck.TradeComplianceCheck.AnalysisConclusion == GeneralDefinition.TRADE_COMPLIANCE_NOT_OK)
                {
                    isOk = false;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 获取贸易合规界面数据(从数据库获取)
        /// </summary>
        public virtual async Task<TradeComplianceCheckDto> GetTradeComplianceCheckFromDateBase(TradeComplianceInputDto input)
        {
            var tradeComplianceCheckList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId);
            if(tradeComplianceCheckList.Count > 0)
            {
                TradeComplianceCheckDto tradeComplianceCheckDto = new();
                tradeComplianceCheckDto.TradeComplianceCheck = tradeComplianceCheckList.FirstOrDefault();
                tradeComplianceCheckDto.ProductMaterialInfos = new();
                var productMaterialList = await _productMaterialInfoRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.TradeComplianceCheckId == tradeComplianceCheckDto.TradeComplianceCheck.Id);
                foreach (var productMaterial in productMaterialList)
                {
                    tradeComplianceCheckDto.ProductMaterialInfos.Add(productMaterial);
                }
                return tradeComplianceCheckDto;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 是否贸易合规，合规返回true，不合规返回false
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> GetIsTradeCompliance(long auditFlowId)
        {
            var tradeComplianceCheckList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            foreach (var tradeComplianceCheck in tradeComplianceCheckList)
            {
                if(tradeComplianceCheck.AnalysisConclusion.Equals(GeneralDefinition.TRADE_COMPLIANCE_NOT_OK))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
