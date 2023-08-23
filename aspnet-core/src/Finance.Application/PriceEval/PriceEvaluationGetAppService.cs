using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Json;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.NerPricing;
using Finance.NrePricing;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.AllManufacturingCost;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using Rougamo;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 获取核价
    /// </summary>
    //[ParameterValidator]
    public class PriceEvaluationGetAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 结构料Id前缀
        /// </summary>
        public const string StructureBomName = "s";

        /// <summary>
        /// 电子料Id前缀
        /// </summary>
        public const string ElectronicBomName = "e";
        #region 类初始化

        protected readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        protected readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

        protected readonly IRepository<Pcs, long> _pcsRepository;
        protected readonly IRepository<PcsYear, long> _pcsYearRepository;

        protected readonly IRepository<ModelCount, long> _modelCountRepository;
        protected readonly IRepository<ModelCountYear, long> _modelCountYearRepository;

        protected readonly IRepository<Requirement, long> _requirementRepository;




        protected readonly IRepository<ElectronicBomInfo, long> _electronicBomInfoRepository;
        protected readonly IRepository<StructureBomInfo, long> _structureBomInfoRepository;

        protected readonly IRepository<EnteringElectronic, long> _enteringElectronicRepository;
        protected readonly IRepository<StructureElectronic, long> _structureElectronicRepository;

        protected readonly IRepository<LossRateInfo, long> _lossRateInfoRepository;
        protected readonly IRepository<LossRateYearInfo, long> _lossRateYearInfoRepository;

        protected readonly IRepository<ExchangeRate, long> _exchangeRateRepository;

        protected readonly IRepository<ManufacturingCostInfo, long> _manufacturingCostInfoRepository;
        protected readonly IRepository<YearInfo, long> _yearInfoRepository;
        protected readonly IRepository<WorkingHoursInfo, long> _workingHoursInfoRepository;
        protected readonly IRepository<RateEntryInfo, long> _rateEntryInfoRepository;
        protected readonly IRepository<ProductionControlInfo, long> _productionControlInfoRepository;
        protected readonly IRepository<QualityRatioEntryInfo, long> _qualityCostProportionEntryInfoRepository;
        protected readonly IRepository<UserInputInfo, long> _userInputInfoRepository;
        protected readonly IRepository<QualityRatioYearInfo, long> _qualityCostProportionYearInfoRepository;
        protected readonly IRepository<UPHInfo, long> _uphInfoRepository;
        protected readonly IRepository<AllManufacturingCost, long> _allManufacturingCostRepository;

        protected readonly IRepository<Gradient, long> _gradientRepository;
        protected readonly IRepository<GradientModel, long> _gradientModelRepository;
        protected readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;

        protected readonly IRepository<EditItem, long> _editItemRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PriceEvaluationGetAppService(IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Pcs, long> pcsRepository, IRepository<PcsYear, long> pcsYearRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Requirement, long> requirementRepository, IRepository<ElectronicBomInfo, long> electronicBomInfoRepository, IRepository<StructureBomInfo, long> structureBomInfoRepository, IRepository<EnteringElectronic, long> enteringElectronicRepository, IRepository<StructureElectronic, long> structureElectronicRepository, IRepository<LossRateInfo, long> lossRateInfoRepository, IRepository<LossRateYearInfo, long> lossRateYearInfoRepository, IRepository<ExchangeRate, long> exchangeRateRepository, IRepository<ManufacturingCostInfo, long> manufacturingCostInfoRepository, IRepository<YearInfo, long> yearInfoRepository, IRepository<WorkingHoursInfo, long> workingHoursInfoRepository, IRepository<RateEntryInfo, long> rateEntryInfoRepository, IRepository<ProductionControlInfo, long> productionControlInfoRepository, IRepository<QualityRatioEntryInfo, long> qualityCostProportionEntryInfoRepository, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<QualityRatioYearInfo, long> qualityCostProportionYearInfoRepository, IRepository<UPHInfo, long> uphInfoRepository, IRepository<AllManufacturingCost, long> allManufacturingCostRepository,
          IRepository<Gradient, long> gradientRepository, IRepository<GradientModel, long> gradientModelRepository, IRepository<GradientModelYear, long> gradientModelYearRepository,
     IRepository<EditItem, long> editItemRepository)
        {
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _pcsRepository = pcsRepository;
            _pcsYearRepository = pcsYearRepository;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _requirementRepository = requirementRepository;
            _electronicBomInfoRepository = electronicBomInfoRepository;
            _structureBomInfoRepository = structureBomInfoRepository;
            _enteringElectronicRepository = enteringElectronicRepository;
            _structureElectronicRepository = structureElectronicRepository;
            _lossRateInfoRepository = lossRateInfoRepository;
            _lossRateYearInfoRepository = lossRateYearInfoRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _manufacturingCostInfoRepository = manufacturingCostInfoRepository;
            _yearInfoRepository = yearInfoRepository;
            _workingHoursInfoRepository = workingHoursInfoRepository;
            _rateEntryInfoRepository = rateEntryInfoRepository;
            _productionControlInfoRepository = productionControlInfoRepository;
            _qualityCostProportionEntryInfoRepository = qualityCostProportionEntryInfoRepository;
            _userInputInfoRepository = userInputInfoRepository;
            _qualityCostProportionYearInfoRepository = qualityCostProportionYearInfoRepository;
            _uphInfoRepository = uphInfoRepository;
            _allManufacturingCostRepository = allManufacturingCostRepository;

            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;

            _editItemRepository = editItemRepository;
        }



        #endregion

        #region 核价表

        /// <summary>
        /// 生成核价表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<CreatePriceEvaluationTableResult> CreatePriceEvaluationTable(CreatePriceEvaluationTableInput input)
        {
            var noneInputCountOrYearmodelAny = await _modelCountRepository.GetAll()
                .Where(p => p.AuditFlowId == input.AuditFlowId && !p.Year.HasValue && !p.InputCount.HasValue).AnyAsync();
            if (noneInputCountOrYearmodelAny)
            {
                throw new FriendlyException($"模组的投入量和年份全部正确设置才可生成核价表！");
                //return new CreatePriceEvaluationTableResult { IsSuccess = false, Message = "模组的投入量和年份全部正确设置才可生成核价表！" };
            }
            var dto = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId)
                .SelectAsync(async p => (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = p.AuditFlowId,
                    InputCount = p.InputCount.Value,
                    ProductId = p.Id,
                    Year = p.Year.Value
                })), p.Id, (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = p.AuditFlowId,
                    InputCount = p.InputCount.Value,
                    ProductId = p.Id,
                    Year = 0
                })))));

            foreach (var item in dto)
            {
                var entity = await _modelCountRepository.GetAsync(item.Id);
                entity.TableJson = item.Item1;
                entity.TableAllJson = item.Item3;
            }


            //获取所有年份核价表

            var dataYear = await (from m in _modelCountRepository.GetAll()
                                  join y in _modelCountYearRepository.GetAll() on m.Id equals y.ProductId
                                  where m.AuditFlowId == input.AuditFlowId
                                  select new { m.Id, m.InputCount, y.Year, m.AuditFlowId, YearId = y.Id }).SelectAsync(async p => (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                                  {
                                      AuditFlowId = p.AuditFlowId,
                                      InputCount = p.InputCount.Value,
                                      ProductId = p.Id,
                                      Year = p.Year
                                  })), p.YearId));

            foreach (var item in dataYear)
            {
                var entity = await _modelCountYearRepository.GetAsync(item.YearId);
                entity.TableJson = item.Item1;
            }

            return new CreatePriceEvaluationTableResult { IsSuccess = true, Message = "生成成功！" };

        }

        /// <summary>
        /// 查询已经生成的核价表（和GetPriceEvaluationTable接口的区别是，GetPriceEvaluationTable接口是实时数据，此接口是保存的数据，不会随着数据变化而变化）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTableResult(GetPriceEvaluationTableResultInput input)
        {
            var json = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.ProductId)
                .Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();
            if (json.IsNullOrWhiteSpace())
            {
                throw new FriendlyException("核价表尚未生成！");
            }
            var dto = JsonConvert.DeserializeObject<ExcelPriceEvaluationTableDto>(json);
            return dto;
        }

        /// <summary>
        /// 设置投入量和年份（用来控制生成的核价表的投入量和年份）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetPriceEvaluationTableInputCount(SetPriceEvaluationTableInputCount input)
        {
            foreach (var item in input.ModelCountInputCount)
            {
                await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.Id == item.ProductId)
                    .UpdateFromQueryAsync(p => new ModelCount
                    {
                        InputCount = item.InputCount,
                        Year = item.Year,
                    });
            }
        }

        /// <summary>
        /// 获取核价表模组的InputCount（投入量）和年份
        /// </summary>
        /// <param name="auditFlowId">流程Id</param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountInputCountEditDto>> GetPriceEvaluationTableInputCount(long auditFlowId)
        {
            var data = from m in _modelCountRepository.GetAll()
                       where m.AuditFlowId == auditFlowId
                       select new ModelCountInputCountEditDto
                       {
                           Id = m.Id,
                           ProductName = m.Product,
                           InputCount = m.InputCount,
                           Year = m.Year
                       };
            var result = await data.ToListAsync();
            return new ListResultDto<ModelCountInputCountEditDto>(result);
        }

        /// <summary>
        /// 获取项目核价表
        /// </summary>
        /// <param name="input">获取项目核价表的接口参数输入</param>
        /// <returns>项目核价表</returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTable(GetPriceEvaluationTableInput input)
        {
            //获取标题
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            //获取零件名
            var modelName = await (from mc in _modelCountRepository.GetAll()
                                   where mc.AuditFlowId == input.AuditFlowId && mc.Id == input.ProductId
                                   select mc.Product).FirstOrDefaultAsync();

            //物料成本
            var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, InputCount = input.InputCount, ProductId = input.ProductId, Year = input.Year });

            //电子料汇总信息
            var electronicSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.ElectronicName).GroupBy(p => p.CategoryName)
                .Select(p => new ElectronicSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //电子料合计
            var electronicSumValue = electronicSum.Sum(o => o.Value);

            //结构料汇总信息
            var structuralSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.StructuralName).GroupBy(p => p.CategoryName)
                .Select(p => new StructuralSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //胶水等辅材汇总信息
            var glueMaterialSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.GlueMaterialName).GroupBy(p => p.CategoryName)
                .Select(p => new GlueMaterialSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //SMT外协汇总信息
            var smtOutSourceSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.SMTOutSourceName).GroupBy(p => p.CategoryName)
                .Select(p => new SMTOutSourceSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //包材汇总信息
            var packingMaterialSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.PackingMaterialName).GroupBy(p => p.CategoryName)
                .Select(p => new PackingMaterialSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //制造成本
            var manufacturingCostAll = await this.GetManufacturingCost(new GetManufacturingCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = input.Year });

            //制造成本合计
            var manufacturingAllCost = manufacturingCostAll.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal;

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).Select(p => p.Year).ToListAsync();

                //计算需求量（模组总量）
                var moudelCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => yearCount.Contains(p.Year)).SumAsync(p => p.Quantity);

                #region 项目成本

                var costItem = await GetLossCost(new GetCostItemInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = input.Year });

                #endregion

                #region 其他成本项目

                var otherCostItem = await GetOtherCostItem(new GetOtherCostItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    ProductId = input.ProductId,
                    Year = input.Year
                });

                #endregion

                var result = new PriceEvaluationTableDto
                {
                    Title = $"{priceEvaluation?.Title}项目{modelName}核价表（量产/样品）（全生命周期）",
                    Date = DateTime.Now,
                    InputCount = input.InputCount,//项目经理填写
                    RequiredCount = moudelCount,
                    Material = electronicAndStructureList,
                    ElectronicSumValue = electronicSumValue,
                    ElectronicSum = electronicSum,
                    StructuralSum = structuralSum,
                    GlueMaterialSum = glueMaterialSum,
                    SMTOutSourceSum = smtOutSourceSum,
                    PackingMaterialSum = packingMaterialSum,
                    ManufacturingCost = manufacturingCostAll,
                    LossCost = costItem,
                    OtherCostItem = otherCostItem,
                    TotalCost = electronicAndStructureList.Sum(p => p.TotalMoneyCyn) + manufacturingAllCost + costItem.Sum(p => p.WastageCost) + costItem.Sum(p => p.MoqShareCount) + otherCostItem.Total,
                    PreparedDate = DateTime.Now,//编制日期
                    AuditDate = DateTime.Now, //工作流取    审核日期
                    ApprovalDate = DateTime.Now,//工作流取  批准日期
                };
                var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(result);
                DtoExcel(dto);
                return dto;
            }
            else
            {
                var result = await GetData(input.Year);
                var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(result);
                DtoExcel(dto);
                return dto;
            }

            async Task<PriceEvaluationTableDto> GetData(int year)
            {
                //质量成本比例
                var qualityCostProportionEntryInfo = await _qualityCostProportionYearInfoRepository.GetAll().FirstOrDefaultAsync(p => p.Year == year);
                if (qualityCostProportionEntryInfo is null)
                {
                    qualityCostProportionEntryInfo = await _qualityCostProportionYearInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
                }

                //获取终端走量数量
                var pcsCount = await _pcsYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => p.Year == year).SumAsync(p => p.Quantity);

                //计算需求量（模组总量）
                var moudelCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => p.Year == year).SumAsync(p => p.Quantity);

                //计算-成本项目
                var costItem = this.GetLossCostByMaterial(year, electronicAndStructureList);

                #region 其他成本项目

                var otherCostItem = await GetOtherCostItem(new GetOtherCostItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    ProductId = input.ProductId,
                    Year = year
                });

                #endregion

                return new PriceEvaluationTableDto
                {
                    Year = year,
                    Title = $"{priceEvaluation?.Title}项目{modelName}核价表（量产/样品）({year}年)",
                    Date = DateTime.Now,
                    InputCount = input.InputCount,//项目经理填写
                    RequiredCount = moudelCount,
                    Material = electronicAndStructureList,
                    ElectronicSumValue = electronicSumValue,
                    ElectronicSum = electronicSum,
                    StructuralSum = structuralSum,
                    GlueMaterialSum = glueMaterialSum,
                    SMTOutSourceSum = smtOutSourceSum,
                    PackingMaterialSum = packingMaterialSum,
                    ManufacturingCost = manufacturingCostAll,
                    LossCost = costItem,
                    OtherCostItem = otherCostItem,
                    TotalCost = electronicAndStructureList.Sum(p => p.TotalMoneyCyn) + manufacturingAllCost + costItem.Sum(p => p.WastageCost) + costItem.Sum(p => p.MoqShareCount) + otherCostItem.Total,
                    PreparedDate = DateTime.Now,//编制日期
                    AuditDate = DateTime.Now,//工作流取    审核日期
                    ApprovalDate = DateTime.Now,//工作流取  批准日期
                };
            }
        }

        #endregion

        #region 其他成本项目

        /// <summary>
        /// 获取物流费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<decimal> GetLogisticsFee(GetOtherCostItemInput input)
        {
            decimal logisticsFee;

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //Sum（每年的物流成本*每年的月度需求量）/Sum(月度需求量)
                var data = await (from m in _modelCountYearRepository.GetAll()
                                  join p in _productionControlInfoRepository.GetAll() on m.Year.ToString() equals p.Year
                                  where m.AuditFlowId == input.AuditFlowId && m.ProductId == input.ProductId
                                  && p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId
                                  select p.PerTotalLogisticsCost * m.Quantity).SumAsync();
                var monthlyDemand = await _modelCountYearRepository
                    .GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).SumAsync(p => p.Quantity);
                logisticsFee = data / monthlyDemand;
            }
            else
            {
                //物流成本
                var productionControlInfo = await _productionControlInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId && p.Year == input.Year.ToString());

                //物流费
                logisticsFee = productionControlInfo.PerTotalLogisticsCost;
            }
            return logisticsFee;
        }

        /// <summary>
        /// 获取其他成本项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<OtherCostItem> GetOtherCostItem(GetOtherCostItemInput input)
        {
            //物流费
            decimal logisticsFee = await GetLogisticsFee(input);

            //质量成本
            var qualityCost = await this.GetQualityCost(new GetOtherCostItemInput
            {
                AuditFlowId = input.AuditFlowId,
                ProductId = input.ProductId,
                Year = input.Year
            });

            //计算-其他成本项目
            var otherCostItem = new OtherCostItem
            {
                Id = input.Year,
                Fixture = null,//留白
                LogisticsFee = logisticsFee,
                ProductCategory = qualityCost.ProductCategory,
                CostProportion = qualityCost.CostProportion,
                QualityCost = qualityCost.QualityCost,
                AccountingPeriod = 60, //账期写死60天
                CapitalCostRate = null,//留白
                TaxCost = null,//留白
            };
            otherCostItem.Total = otherCostItem.LogisticsFee + otherCostItem.QualityCost;//物流费+质量成本（MAX)+财务成本+税务成本（财务成本、税务成本一般都为0）
            return otherCostItem;
        }

        #endregion

        #region 损耗成本

        /// <summary>
        /// 获取损耗成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<LossCost>> GetLossCost(GetCostItemInput input)
        {
            //物料成本
            var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = input.Year });
            return this.GetLossCostByMaterial(input.Year, electronicAndStructureList);
        }

        /// <summary>
        /// 根据物料 获取 损耗成本
        /// </summary>
        /// <param name="year"></param>
        /// <param name="electronicAndStructureList"></param>
        /// <returns></returns>
        private List<LossCost> GetLossCostByMaterial(int year, List<Material> electronicAndStructureList)
        {
            //计算-成本项目
            var costItem = electronicAndStructureList.GroupBy(x => x.SuperType)
                .Select(p => new LossCost { Id = year, Name = p.Key, WastageCost = p.Sum(o => o.Loss), MoqShareCount = p.Sum(o => o.MoqShareCount) }).ToList();//只有损耗，还要增加分摊
            return costItem;
        }

        /// <summary>
        /// 获取全生命周期 损耗成本 根据多个年份的损耗成本计算
        /// </summary>
        /// <param name="costItemList"></param>
        /// <returns></returns>
        private List<LossCost> GetCostItemAllPrivate(List<LossCost> costItemList)
        {
            var costItem = costItemList.GroupBy(p => p.Id).Select(p => new LossCost
            {
                Id = p.Key,
                Name = p.FirstOrDefault().Name,
                WastageCost = p.Sum(o => o.WastageCost),
                MoqShareCount = p.Sum(o => o.MoqShareCount),
            }).ToList();

            return costItem;
        }

        #endregion

        #region BOM成本

        /// <summary>
        /// 获取 bom成本（含损耗）汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<Material>> GetBomCost(GetBomCostInput input)
        {
            var gradient = await _gradientRepository.GetAsync(input.GradientId);
            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId)
                    .OrderBy(p => p.Year).Select(p => new { p.Year, p.Quantity }).ToListAsync();
                //var yearCount = await _gradientModelYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.GradientModelId == input.ProductId)
                //    .OrderBy(p => p.Year).Select(p => new { p.Year, Quantity = p.Count }).ToListAsync();

                //获取数据
                var material = await yearCount.SelectAsync(async p => await GetData(p.Year));

                //加总求平均以获取Dto
                var dto = material.SelectMany(p => p).Join(yearCount, p => p.Year, p => p.Year, (a, b) => { a.Quantity = b.Quantity; return a; })
                    .GroupBy(p => new
                    {
                        p.Id,
                        p.SuperType,
                        p.CategoryName,
                        p.TypeName,
                        p.Sap,
                        p.MaterialName,
                        p.AssemblyCount,
                        p.CurrencyText,
                        p.MoqShareCount,
                        p.Moq,
                        p.AvailableInventory,
                        p.Remarks,
                    }).Select(p => new Material
                    {
                        Id = p.Key.Id,
                        SuperType = p.Key.SuperType,
                        CategoryName = p.Key.CategoryName,
                        TypeName = p.Key.TypeName,
                        Sap = p.Key.Sap,
                        MaterialName = p.Key.MaterialName,
                        AssemblyCount = p.Key.AssemblyCount,
                        CurrencyText = p.Key.CurrencyText,
                        ExchangeRate = p.MinBy(o => o.Year).ExchangeRate,
                        SopExchangeRate = p.MinBy(o => o.Year).ExchangeRate,
                        Loss = p.Key.AssemblyCount == 0 ? 0 : p.Sum(o => o.Loss * o.Quantity) / p.Sum(o => p.Key.AssemblyCount.To<decimal>() * o.Quantity) * p.Key.AssemblyCount.To<decimal>(),
                        InputCount = p.Sum(o => o.InputCount),
                        PurchaseCount = p.Sum(o => o.PurchaseCount),
                        MoqShareCount = p.Key.MoqShareCount,
                        Moq = p.Key.Moq,
                        AvailableInventory = p.Key.AvailableInventory,
                        Remarks = p.Key.Remarks,
                        MaterialPriceCyn = p.Key.AssemblyCount == 0 ? 0 : p.Sum(o => o.MaterialPrice * o.ExchangeRate * o.AssemblyCount.To<decimal>() * o.Quantity) / p.Sum(o => p.Key.AssemblyCount.To<decimal>() * o.Quantity), //* p.Key.AssemblyCount.To<decimal>(),

                    }).ToList();
                foreach (var item in dto)
                {
                    item.TotalMoneyCyn = item.MaterialPriceCyn * item.AssemblyCount.To<decimal>();
                    item.MaterialPrice = item.MaterialPriceCyn / item.SopExchangeRate;
                    item.LossRate = item.TotalMoneyCyn == 0 ? 0 : item.Loss / item.TotalMoneyCyn;
                    item.MaterialCost = item.TotalMoneyCyn + item.Loss;
                }
                return dto.GroupBy(p => p.SuperType).Select(p => p.Select(o => o).OrderByDescending(o => o.TotalMoneyCyn).ToList())
                    .SelectMany(p => p).ToList();
            }
            else
            {
                return await GetData(input.Year);
            }

            async Task<List<Material>> GetData(int year)
            {
                //获取【电子料】表
                var electronic = from eb in _electronicBomInfoRepository.GetAll()
                                 join ec in _enteringElectronicRepository.GetAll() on eb.Id equals ec.ElectronicId

                                 join lri in _lossRateInfoRepository.GetAll()
                                 on new { eb.AuditFlowId, eb.ProductId, eb.CategoryName } equals new { lri.AuditFlowId, lri.ProductId, lri.CategoryName }

                                 join lriy in _lossRateYearInfoRepository.GetAll() on lri.Id equals lriy.LossRateInfoId

                                 join er in _exchangeRateRepository.GetAll() on ec.Currency equals er.ExchangeRateKind

                                 where eb.ProductId == input.ProductId &&
                                 ec.SolutionId == input.ProductId
                                 && eb.AuditFlowId == input.AuditFlowId && lriy.Year == year

                                 select new Material
                                 {
                                     Id = $"{PriceEvaluationGetAppService.ElectronicBomName}{eb.Id}",
                                     SuperType = lri.SuperType,
                                     CategoryName = eb.CategoryName,
                                     TypeName = eb.TypeName,
                                     Sap = eb.SapItemNum,
                                     MaterialName = eb.SapItemName,
                                     AssemblyCount = eb.AssemblyQuantity,//装配数量
                                     SystemiginalCurrency = ec.SystemiginalCurrency,//ec.SystemiginalCurrency,
                                     CurrencyText = ec.Currency,
                                     ExchangeRateValue = er.ExchangeRateValue,
                                     LossRate = lriy.Rate,//损耗率
                                     Moq = ec.MOQ,
                                     //AvailableInventory = ec.AvailableStock,
                                     StandardMoney = ec.StandardMoney,
                                     Remarks = ec.Remark
                                 };
                var electronicList = await electronic.ToListAsync();


                //获取【结构料】表（其他大类都在这）

                var structure = from sb in _structureBomInfoRepository.GetAll()
                                join se in _structureElectronicRepository.GetAll() on sb.Id equals se.StructureId

                                join lri in _lossRateInfoRepository.GetAll()
                                on new { sb.AuditFlowId, sb.ProductId, sb.CategoryName } equals new { lri.AuditFlowId, lri.ProductId, lri.CategoryName }

                                join lriy in _lossRateYearInfoRepository.GetAll() on lri.Id equals lriy.LossRateInfoId

                                join er in _exchangeRateRepository.GetAll() on se.Currency equals er.ExchangeRateKind

                                where sb.ProductId == input.ProductId
                                && se.SolutionId == input.ProductId
                                && sb.AuditFlowId == input.AuditFlowId && lriy.Year == year

                                select new Material
                                {
                                    Id = $"{PriceEvaluationGetAppService.StructureBomName}{sb.Id}",
                                    SuperType = lri.SuperType,
                                    CategoryName = sb.CategoryName,
                                    TypeName = sb.TypeName,
                                    Sap = sb.SapItemNum,
                                    MaterialName = sb.MaterialName,
                                    AssemblyCount = sb.AssemblyQuantity,//装配数量
                                    SystemiginalCurrency = se.SystemiginalCurrency,//se.Sop,
                                    CurrencyText = se.Currency,
                                    ExchangeRateValue = er.ExchangeRateValue,
                                    LossRate = lriy.Rate,//损耗率
                                    Moq = se.MOQ,
                                    //AvailableInventory = se.AvailableStock,
                                    StandardMoney = se.StandardMoney,
                                    Remarks = se.Remark
                                };
                var structureList = await structure.ToListAsync();

                var electronicAndStructureList = electronicList.Union(structureList).ToList();

                electronicAndStructureList.ForEach(item =>
                {
                    item.Year = year;
                    item.MaterialPrice = GetMaterialPrice(item.SystemiginalCurrency, year, gradient.GradientValue);
                    item.ExchangeRate = GetExchangeRate(item.ExchangeRateValue, year, gradient.GradientValue);
                    item.MaterialPriceCyn = GetYearValue(item.StandardMoney, year, gradient.GradientValue);
                    item.TotalMoneyCyn = (decimal)item.AssemblyCount * item.MaterialPriceCyn;//人民币合计金额=装配数量*人民币单价（诸年之和）
                    item.Loss = item.LossRate * item.TotalMoneyCyn;//等于合计金额*损耗率
                    item.MaterialCost = item.TotalMoneyCyn + item.Loss;//材料成本（含损耗）
                    item.InputCount = Math.Round((decimal)item.AssemblyCount * (1 + item.LossRate) * input.InputCount, 0).To<int>();//（装配数量*（1+损耗率）*投入量） ，四舍五入，取整
                    item.PurchaseCount = item.AvailableInventory > item.InputCount ? 0 : ((item.InputCount - item.AvailableInventory) > item.Moq ? (item.Moq == 0 ? 0 : (item.Moq * Math.Ceiling((item.InputCount - item.AvailableInventory) / item.Moq))) : item.Moq);
                    item.MoqShareCount = (item.Moq == 0 || item.InputCount == 0) ? 0 : ((item.PurchaseCount - item.InputCount) < 0 ? 0 : (item.PurchaseCount - item.InputCount) * item.MaterialPriceCyn / item.InputCount);
                });
                return electronicAndStructureList.GroupBy(p => p.SuperType).Select(p => p.Select(o => o).OrderByDescending(o => o.TotalMoneyCyn).ToList())
                    .SelectMany(p => p).ToList();
            }
        }

        /// <summary>
        /// 获取 bom成本（含损耗）汇总表 Dto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<BomCost> GetBomCostDto(GetBomCostInput input)
        {
            var data = await GetBomCost(input);
            var dto = new BomCost();
            dto.Material = data;
            dto.TotalMoneyCynCount = dto.Material.Sum(p => p.TotalMoneyCyn);
            dto.ElectronicCount = dto.Material.Where(p => p.SuperType == FinanceConsts.ElectronicName).Sum(p => p.TotalMoneyCyn);
            return dto;
        }

        #endregion

        #region 制造成本汇总表


        /// <summary>
        /// 获取 制造成本汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ManufacturingCost>> GetManufacturingCost(GetManufacturingCostInput input)
        {
            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                #region 组测

                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll()
                    .Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).Select(p => p.Year).ToListAsync();

                //获取数据
                var dtoList = await yearCount.SelectAsync(async p => await GetGroupTest(p));

                //所有年份的直接制造成本
                var manufacturingCostDirectList = dtoList.Select(p => p.ManufacturingCostDirect);

                //所有年份的间接制造成本
                var manufacturingCostIndirectList = dtoList.Select(p => p.ManufacturingCostIndirect);

                //月需求量的和
                var manufacturingCostDirectListMonthlyDemand = dtoList.Sum(p => p.MonthlyDemand);

                var manufacturingCostDirect = new ManufacturingCostDirect
                {
                    Id = 0,
                    DirectLabor = manufacturingCostDirectList.Sum(p => p.DirectLaborNo) / manufacturingCostDirectListMonthlyDemand,
                    EquipmentDepreciation = manufacturingCostDirectList.Sum(p => p.EquipmentDepreciationNo) / manufacturingCostDirectListMonthlyDemand,
                    LineChangeCost = manufacturingCostDirectList.Sum(p => p.LineChangeCost * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    ManufacturingExpenses = manufacturingCostDirectList.Sum(p => p.ManufacturingExpenses * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                };
                manufacturingCostDirect.Subtotal = manufacturingCostDirect.DirectLabor + manufacturingCostDirect.EquipmentDepreciation + manufacturingCostDirect.LineChangeCost + manufacturingCostDirect.ManufacturingExpenses;

                var manufacturingCostIndirect = new ManufacturingCostIndirect
                {
                    Id = 0,
                    DirectLabor = manufacturingCostIndirectList.Sum(p => p.DirectLabor * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    ManufacturingExpenses = manufacturingCostIndirectList.Sum(p => p.ManufacturingExpenses * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    EquipmentDepreciation = manufacturingCostIndirectList.Sum(p => p.EquipmentDepreciation * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                };
                manufacturingCostIndirect.Subtotal = manufacturingCostIndirect.DirectLabor + manufacturingCostIndirect.EquipmentDepreciation + manufacturingCostIndirect.ManufacturingExpenses;

                //manufacturingCostDirect.Round2();//保留2位小数
                //manufacturingCostIndirect.Round2();//保留2位小数
                var dto = new ManufacturingCost
                {
                    Id = input.Year,
                    CostType = CostType.GroupTest,
                    CostItem = PriceEvalConsts.GroupTest,
                    GradientKy = dtoList.Sum(p => p.GradientKy),
                    ManufacturingCostDirect = manufacturingCostDirect,
                    ManufacturingCostIndirect = manufacturingCostIndirect,
                    Subtotal = manufacturingCostDirect.Subtotal + manufacturingCostIndirect.Subtotal
                };
                //dto.Round2();//保留2位小数

                #endregion

                #region SMT和COB 其他
                List<ManufacturingCost> entity = await GetDbCost(input.Year, new List<CostType> { CostType.SMT, CostType.COB, CostType.Other });
                entity.Insert(2, dto);
                #endregion

                #region 合计
                var total = GetTotal(input.Year, entity);

                entity.Add(total);
                #endregion

                return entity;
            }
            else
            {
                var entity = await GetData(input.Year);
                return entity;
            }

            async Task<List<ManufacturingCost>> GetData(int year)
            {
                #region 获取组测

                var dto = await GetGroupTest(year);

                #endregion

                #region SMT和COB 其他
                List<ManufacturingCost> entity = await GetDbCost(year, new List<CostType> { CostType.SMT, CostType.COB, CostType.Other });
                entity.Insert(2, dto);

                #endregion

                #region 合计
                var total = GetTotal(input.Year, entity);
                entity.Add(total);
                #endregion

                return entity;
            }

            #region 获取数据库中存储的制造成本

            async Task<List<ManufacturingCost>> GetDbCost(int year, List<CostType> costType)
            {
                var data = await _allManufacturingCostRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.ProductId == input.ProductId
                                             && t.Year == year && costType.Contains(t.CostType))
                                    .Select(t => new ManufacturingCost
                                    {
                                        Id = year,
                                        CostType = t.CostType,
                                        GradientKy = 0,
                                        MonthlyDemand = 0,
                                        ManufacturingCostDirect = t.CostType == CostType.Other ? null : new ManufacturingCostDirect
                                        {
                                            Id = year,
                                            DirectLabor = t.DirectLabor1.Value,
                                            EquipmentDepreciation = t.EquipmentDepreciation1.Value,
                                            LineChangeCost = t.LineChangeCost1.Value,
                                            ManufacturingExpenses = t.ManufacturingExpenses1.Value,
                                            Subtotal = t.Subtotal1.Value,
                                        },
                                        ManufacturingCostIndirect = t.CostType == CostType.Other ? null : new ManufacturingCostIndirect
                                        {
                                            Id = year,
                                            DirectLabor = t.DirectLabor2.Value,
                                            EquipmentDepreciation = t.EquipmentDepreciation2.Value,
                                            ManufacturingExpenses = t.ManufacturingExpenses2.Value,
                                            Subtotal = t.Subtotal2.Value
                                        },
                                        Subtotal = t.Subtotal,
                                    }).ToListAsync();
                data.ForEach(p =>
                {
                    p.CostItem = p.CostType switch
                    {
                        CostType.GroupTest => PriceEvalConsts.GroupTest,
                        CostType.SMT => PriceEvalConsts.SMT,
                        CostType.COB => PriceEvalConsts.COB,
                        CostType.Total => PriceEvalConsts.Total,
                        CostType.Other => PriceEvalConsts.Other,
                        _ => throw new FriendlyException($"CostType输入参数不正确。参数为：{p.CostType}"),
                    };
                });
                return data;
            }
            #endregion

            #region 组测

            async Task<ManufacturingCost> GetGroupTest(int year)
            {
                //工序工时年份
                var yearInfo = await _yearInfoRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId && p.Year == year && p.Part == YearPart.WorkingHour);


                //获取制造成本参数
                var manufacturingCostInfo = await _manufacturingCostInfoRepository.FirstOrDefaultAsync(p => p.Year == year);
                if (manufacturingCostInfo is null)
                {
                    manufacturingCostInfo = await _manufacturingCostInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
                }

                //模组数量
                var modelCountYear = await _modelCountYearRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId && p.Year == year);

                //计算连续乘积的委托
                Func<List<decimal>, decimal> product = p =>
                {
                    var init = 1M;
                    p.ForEach(o => init *= o);
                    return init;
                };

                //要求
                var requirement = await _requirementRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).ToListAsync();

                //（1-累计降幅）
                var oneCumulativeDecline = product.Invoke(requirement.Select(p => 1 - (p.AnnualDeclineRate * 0.01M)).ToList());

                //年需求量(modelCountYear的数量)

                //月需求量
                var monthlyDemand = Math.Ceiling(modelCountYear.Quantity.To<decimal>() / 12M).To<int>();

                //UPH值
                var uph = (await _uphInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId)).UPH;

                //每班日产能
                var dailyCapacityPerShift = uph * manufacturingCostInfo.WorkingHours.To<decimal>() * manufacturingCostInfo.RateOfMobilization;

                //工时工序静态字段表
                var workingHoursInfo = await _workingHoursInfoRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).ToListAsync();


                //设备金额
                var equipmentMoney = workingHoursInfo.Sum(p => p.EquipmentTotalPrice);

                //产线数量
                var lineCount = Math.Ceiling(monthlyDemand / (dailyCapacityPerShift * (decimal)manufacturingCostInfo.MonthlyWorkingDays * (decimal)manufacturingCostInfo.DailyShift));

                //产线设备月折旧（删除稼动率）
                //var monthlyDepreciation = (equipmentMoney - 0) * lineCount * manufacturingCostInfo.RateOfMobilization / ((decimal)manufacturingCostInfo.UsefulLifeOfFixedAssets * 12) / oneCumulativeDecline;
                var monthlyDepreciation = (equipmentMoney - 0) * lineCount / ((decimal)manufacturingCostInfo.UsefulLifeOfFixedAssets * 12) / oneCumulativeDecline;


                //月产能
                var monthlyCapacity = dailyCapacityPerShift * 26 * 2;

                //产能利用率
                var capacityUtilization = monthlyDemand / (monthlyCapacity * lineCount);

                //分摊后折旧
                var allocatedDepreciation = monthlyDepreciation * capacityUtilization;

                //制造工时
                var manufacturingHours = (yearInfo.Sum(p => p.StandardLaborHours) + yearInfo.Sum(p => p.StandardMachineHours)).To<decimal>();

                //人员单价
                var personPrice = manufacturingCostInfo.AverageWage / oneCumulativeDecline;

                //财务费率
                var rateEntryInfo = await _rateEntryInfoRepository.FirstOrDefaultAsync(p => p.Year == year);
                if (rateEntryInfo is null)
                {
                    rateEntryInfo = await _rateEntryInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
                }

                //工时工序
                var workingHoursInputInfo = await _yearInfoRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId && p.Year == year && p.Part == YearPart.SwitchLine);

                //跟线工价
                var linePrice = personPrice / (decimal)manufacturingCostInfo.MonthlyWorkingDays / manufacturingCostInfo.WorkingHours.To<decimal>() / 3600;

                //跟线成本
                var lineCost = linePrice * workingHoursInputInfo.StandardLaborHours.To<decimal>();

                //切线成本
                var switchLineCost = workingHoursInputInfo.StandardMachineHours.To<decimal>() * (allocatedDepreciation / manufacturingCostInfo.MonthlyWorkingDays.To<decimal>() / (manufacturingCostInfo.WorkingHours.To<decimal>() * 2) / 3600);

                //换线成本
                var lineChangeCost = lineCost + switchLineCost;

                //直接人工=人员单价*人员数量*(月需求量/每班日产能)/月工作天数/(1-累积降幅）
                var directLaborNo = personPrice * (yearInfo.Sum(p => p.PersonCount).To<decimal>() + 1)
                    * Math.Ceiling(monthlyDemand / dailyCapacityPerShift) / (decimal)manufacturingCostInfo.MonthlyWorkingDays
                    / oneCumulativeDecline;

                //直接制造成本
                var manufacturingCost = new ManufacturingCostDirect
                {
                    MonthlyDemand = monthlyDemand,
                    Id = year,
                    //直接人工=人员单价*人员数量*(月需求量/每班日产能)/月工作天数/(1-累积降幅）/月需求量（新增：除以月需求量）
                    DirectLabor = directLaborNo / monthlyDemand,
                    DirectLaborNo = directLaborNo,
                    EquipmentDepreciation = allocatedDepreciation / monthlyDemand,
                    EquipmentDepreciationNo = allocatedDepreciation,
                    LineChangeCost = lineChangeCost,
                    ManufacturingExpenses = (rateEntryInfo.DirectManufacturingRate * manufacturingHours) / 3600,
                };
                manufacturingCost.Subtotal = manufacturingCost.DirectLabor + manufacturingCost.EquipmentDepreciation + manufacturingCost.LineChangeCost + manufacturingCost.ManufacturingExpenses;

                //间接制造成本
                var manufacturingCostIndirect = new ManufacturingCostIndirect
                {
                    MonthlyDemand = monthlyDemand,
                    Id = year,
                    DirectLabor = (rateEntryInfo.IndirectLaborRate * yearInfo.Sum(p => p.StandardLaborHours).To<decimal>()) / 3600,
                    EquipmentDepreciation = (rateEntryInfo.IndirectDepreciationRate * yearInfo.Sum(p => p.StandardMachineHours).To<decimal>()) / 3600,
                    ManufacturingExpenses = (rateEntryInfo.IndirectManufacturingRate * manufacturingHours) / 3600,
                };
                manufacturingCostIndirect.Subtotal = manufacturingCostIndirect.DirectLabor + manufacturingCostIndirect.EquipmentDepreciation + manufacturingCostIndirect.ManufacturingExpenses;



                //manufacturingCost.Round2();//保留两位小数
                //manufacturingCostIndirect.Round2();//保留两位小数

                var dto = new ManufacturingCost
                {
                    Id = year,
                    CostType = CostType.GroupTest,
                    CostItem = PriceEvalConsts.GroupTest,
                    GradientKy = modelCountYear.Quantity,
                    MonthlyDemand = monthlyDemand,
                    ManufacturingCostDirect = manufacturingCost,
                    ManufacturingCostIndirect = manufacturingCostIndirect,
                    Subtotal = manufacturingCost.Subtotal + manufacturingCostIndirect.Subtotal
                };
                //dto.Round2();//保留两位小数
                return dto;
            }


            #endregion

            #region 合计

            static ManufacturingCost GetTotal(int year, List<ManufacturingCost> entity)
            {
                var manufacturingCostDirectTotal = entity.Select(p => p.ManufacturingCostDirect).Where(p => p is not null);
                var manufacturingCostIndirectTotal = entity.Select(p => p.ManufacturingCostIndirect).Where(p => p is not null);

                var manufacturingCostDirectTotalDto = new ManufacturingCostDirect
                {
                    Id = year,
                    DirectLabor = manufacturingCostDirectTotal.Sum(p => p.DirectLabor),
                    EquipmentDepreciation = manufacturingCostDirectTotal.Sum(p => p.EquipmentDepreciation),
                    LineChangeCost = manufacturingCostDirectTotal.Sum(p => p.LineChangeCost),
                    ManufacturingExpenses = manufacturingCostDirectTotal.Sum(p => p.ManufacturingExpenses),
                    Subtotal = manufacturingCostDirectTotal.Sum(p => p.Subtotal),
                };
                var manufacturingCostIndirectTotalDto = new ManufacturingCostIndirect
                {
                    Id = year,
                    EquipmentDepreciation = manufacturingCostIndirectTotal.Sum(p => p.EquipmentDepreciation),
                    DirectLabor = manufacturingCostIndirectTotal.Sum(p => p.DirectLabor),
                    ManufacturingExpenses = manufacturingCostIndirectTotal.Sum(p => p.ManufacturingExpenses),
                    Subtotal = manufacturingCostIndirectTotal.Sum(p => p.Subtotal),
                };
                //manufacturingCostDirectTotalDto.Round2();//保留2位小数
                //manufacturingCostIndirectTotalDto.Round2();//保留2位小数

                var total = new ManufacturingCost
                {
                    Id = year,
                    CostItem = PriceEvalConsts.Total,
                    CostType = CostType.Total,
                    GradientKy = entity.Sum(p => p.GradientKy),
                    ManufacturingCostDirect = manufacturingCostDirectTotalDto,
                    ManufacturingCostIndirect = manufacturingCostIndirectTotalDto,
                    Subtotal = entity.Sum(p => p.Subtotal),
                };
                //total.Round2();//保留2位小数
                return total;
            }

            #endregion
        }

        #endregion

        #region 物流成本汇总表

        /// <summary>
        /// 获取 物流成本汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ProductionControlInfoListDto>> GetLogisticsCost(GetLogisticsCostInput input)
        {
            //物流成本
            var productionControlInfo = await _productionControlInfoRepository
                .GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId)
                .WhereIf(input.Year != PriceEvalConsts.AllYear, p => p.Year == input.Year.ToString())
                .ToListAsync();
            return ObjectMapper.Map<List<ProductionControlInfoListDto>>(productionControlInfo);
        }

        #endregion

        #region 质量成本

        /// <summary>
        /// 获取 质量成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<QualityCostListDto> GetQualityCost(GetOtherCostItemInput input)
        {
            //物流费
            decimal logisticsFee = await GetLogisticsFee(input);

            //制造成本
            var manufacturingCost = await GetManufacturingCost(new GetManufacturingCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = input.Year });

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).ToListAsync();

                var data = await yearCount.Select(p => p.Year).SelectAsync(async p =>
                {
                    var dto = new GetOtherCostItemInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = p };
                    var result = await GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = p });
                    return (dto, result);
                });

                var otherCostItemtList = (await data.SelectAsync(async p => await GetQualityCostPrivate(p.dto, p.result, logisticsFee, manufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal))).ToList();

                var qualityCost = (from o in otherCostItemtList
                                   join y in yearCount on o.Year equals y.Year
                                   select o.QualityCost * y.Quantity).Sum();

                return new QualityCostListDto
                {
                    ProductCategory = otherCostItemtList.FirstOrDefault().ProductCategory,
                    CostProportion = otherCostItemtList.Sum(p => p.CostProportion),
                    QualityCost = qualityCost / yearCount.Sum(p => p.Quantity),
                };
            }
            else
            {
                //物料成本
                var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Year = input.Year });
                return await this.GetQualityCostPrivate(input, electronicAndStructureList, logisticsFee, manufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal);
            }

        }

        /// <summary>
        /// 获取目标毛利率
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        private static decimal GetGrossProfitMargin(string productType)
        {
            switch (productType)
            {
                case FinanceConsts.ProductType_ExternalImaging: return 0.18M;
                case FinanceConsts.ProductType_EnvironmentalPerception: return 0.28M;
                case FinanceConsts.ProductType_CabinMonitoring: return 0.24M;
                default:
                    throw new FriendlyException($"营销部录入的产品小类有误，录入的产品小类为：{productType}");
            }
        }

        /// <summary>
        /// 获取 质量成本(内部使用)（单年份）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="electronicAndStructureList"></param>
        /// <param name="logisticsFee"></param>
        /// <param name="manufacturingCostSubtotal"></param>
        /// <returns></returns>
        private async Task<QualityCostListDto> GetQualityCostPrivate(GetOtherCostItemInput input, List<Material> electronicAndStructureList
         , decimal logisticsFee, decimal manufacturingCostSubtotal)
        {
            //项目管理部人员输入
            var userInputInfo = await _userInputInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            //产品类别
            var modelCount = await _modelCountRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.ProductId);
            //1-目标毛利率
            var grossProfitMargin = 1 - GetGrossProfitMargin(modelCount.ProductType);


            //成本比例
            var costProportion = await (from q in _qualityCostProportionEntryInfoRepository.GetAll()
                                        join qy in _qualityCostProportionYearInfoRepository.GetAll() on q.Id equals qy.QualityCostId
                                        join d in _financeDictionaryDetailRepository.GetAll() on q.Category equals d.Id
                                        join m in _modelCountRepository.GetAll() on d.Id equals m.ProductType
                                        where m.Id == input.ProductId && q.IsFirst == userInputInfo.IsFirst
                                        && qy.Year <= input.Year
                                        select qy).OrderByDescending(p => p.Year).Select(p => p.Rate).FirstOrDefaultAsync();

            //材料成本合计（质量成本（MAX)）
            var totalMaterialCost = (electronicAndStructureList.Sum(p => p.MaterialCost) + electronicAndStructureList.Sum(p => p.MoqShareCount)
                + logisticsFee + manufacturingCostSubtotal) / grossProfitMargin * costProportion;


            //产品小类名称
            var productTypeName = await (from m in _modelCountRepository.GetAll()
                                         join d in _financeDictionaryDetailRepository.GetAll() on m.ProductType equals d.Id
                                         where m.Id == input.ProductId && m.AuditFlowId == input.AuditFlowId
                                         select d.DisplayName).FirstOrDefaultAsync();

            return new QualityCostListDto
            {
                Year = input.Year,
                ProductCategory = productTypeName,
                CostProportion = costProportion,
                QualityCost = totalMaterialCost,
            };
        }


        #endregion

        #region 后端对接函数

        /// <summary>
        /// 根据流程表主键获取Pcs数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<PcsListDto>> GetPcsByPriceAuditFlowId(long auditFlowId) =>
            new ListResultDto<PcsListDto>((await _pcsRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
               .Join(_pcsYearRepository.GetAll(), p => p.Id, p => p.PcsId, (pcs, pcsYear) => new { pcs, pcsYear }).ToListAsync()).GroupBy(p => p.pcs).Select(p =>
               {
                   var dto = ObjectMapper.Map<PcsListDto>(p.Key);
                   dto.PcsYear = ObjectMapper.Map<IList<PcsYearListDto>>(p.Select(o => o.pcsYear));
                   return dto;
               }).ToList());

        /// <summary>
        /// 根据流程表主键获取模组数量
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountListDto>> GetModelCountByAuditFlowId(long auditFlowId)
        {
            var data = (await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                    .Join(_modelCountYearRepository.GetAll(), p => p.Id, p => p.ProductId, (modelCount, modelCountYear) => new { modelCount, modelCountYear }).ToListAsync()).GroupBy(p => p.modelCount).Select(p =>
                    {
                        var dto = ObjectMapper.Map<ModelCountListDto>(p.Key);
                        dto.ModelCountYearListDto = ObjectMapper.Map<IList<ModelCountYearListDto>>(p.Select(o => o.modelCountYear));
                        return dto;
                    }).ToList();
            return new ListResultDto<ModelCountListDto>(data);
        }



        #endregion

        #region Json读取
        /// <summary>
        /// 根据Json获取材料单价（原币），如果isAll为ture，根据公式，如果为false，根据选择的年份
        /// </summary>
        /// <param name="json"></param>
        /// <param name="isAll"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetMaterialPrice(string json, int year, decimal gradientValue)
        {
            return GetYearValue(json, year, gradientValue);
        }

        /// <summary>
        /// 根据Json获取汇率，如果isAll为ture，根据平均值，如果为false，根据选择的年份
        /// </summary>
        /// <param name="json"></param>
        /// <param name="isAll"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetExchangeRate(string json, int year, decimal gradientValue)
        {
            return GetYearValue(json, year, gradientValue);
        }

        /// <summary>
        /// 获取年份值，如果获取为不存在的年份，则取最后一年
        /// </summary>
        /// <param name="json"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetYearValue(string json, int year, decimal gradientValue)
        {
            //var list = EnteringMapper.JsonToList(json);
            var list = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(json).FirstOrDefault(p => p.Kv == gradientValue).YearOrValueModes;
            var query = list.Where(p => p.Year == year);
            if (query.Any())
            {
                return query.FirstOrDefault().Value;
            }
            else
            {
                return list.OrderByDescending(p => p.Year).FirstOrDefault().Value;
            }
        }

        #endregion

        #region 核价看板

        /// <summary>
        /// 核价看板-【产品选择】下拉框下拉数据接口
        /// </summary>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountSelectListDto>> GetPricingPanelProductSelectList(GetPricingPanelProductSelectListInput input)
        {
            var data = from m in _modelCountRepository.GetAll()
                       where m.AuditFlowId == input.AuditFlowId
                       select new ModelCountSelectListDto
                       {
                           Id = m.Id,
                           ProductName = m.Product
                       };
            var result = await data.ToListAsync();
            return new ListResultDto<ModelCountSelectListDto>(result);
        }

        /// <summary>
        /// 核价看板-时间选择下拉框下拉数据接口
        /// </summary>
        /// <returns></returns>
        public async virtual Task<ListResultDto<YearListDto>> GetPricingPanelTimeSelectList(GetPricingPanelTimeSelectListInput input)
        {
            var data = await _pcsYearRepository.GetAll()
                .Where(p => p.AuditFlowId == input.AuditFlowId)
                .Select(p => new YearListDto { Id = p.Year, Name = $"{p.Year}年" })
                .Distinct()
                .OrderBy(p => p.Id)
                .ToListAsync();
            if (data.Count > 0)
            {
                data.Add(new YearListDto { Id = PriceEvalConsts.AllYear, Name = "全生命周期" });
            }
            return new ListResultDto<YearListDto>(data);
        }

        /// <summary>
        /// 核价看板-产品成本占比图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ProportionOfProductCostListDto>> GetPricingPanelProportionOfProductCost(GetPricingPanelProportionOfProductCostInput input)
        {
            var data = await this.GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, InputCount = 0, ProductId = input.ProductId, Year = input.Year });

            //bom成本
            var bomCost = data.Material.Sum(p => p.TotalMoneyCyn);

            //损耗成本
            var costItemAll = data.Material.Sum(p => p.Loss);

            //制造成本
            var manufacturingCost = data.ManufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal; //data.ManufacturingCost.Where(p => p.CostType != CostType.Total).Sum(p => p.Subtotal); 

            //物流成本
            var logisticsFee = data.OtherCostItem.LogisticsFee;

            //质量成本
            var qualityCost = data.OtherCostItem.QualityCost;

            var sum = bomCost + costItemAll + manufacturingCost + logisticsFee + qualityCost;

            var list = new List<ProportionOfProductCostListDto>
            {
                new ProportionOfProductCostListDto{ Name="bom成本", Proportion= bomCost/sum},
                new ProportionOfProductCostListDto{ Name="损耗成本", Proportion= costItemAll/sum},
                new ProportionOfProductCostListDto{ Name="制造成本", Proportion= manufacturingCost/sum},
                new ProportionOfProductCostListDto{ Name="物流成本", Proportion= logisticsFee/sum},
                new ProportionOfProductCostListDto{ Name="质量成本", Proportion= qualityCost/sum},
            };
            return new ListResultDto<ProportionOfProductCostListDto>(list);
        }

        /// <summary>
        /// 添加核价表TR方案Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task AddPricingPanelTrProgrammeId(AddPricingPanelTrProgrammeIdInput input)
        {
            await _priceEvaluationRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).UpdateFromQueryAsync(p => new PriceEvaluation
            {
                TrProgramme = input.FileManagementId
            });
        }

        /// <summary>
        /// 获取推移图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<GoTable>> GetGoTable(GetGoTableInput input)
        {
            //获取总年数
            var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).Select(p => p.Year).OrderBy(p => p).ToListAsync();
            var dtoList = (await yearCount.SelectAsync(async p => await GetPriceEvaluationTable(new GetPriceEvaluationTableInput { Year = p, ProductId = input.ProductId, AuditFlowId = input.AuditFlowId, InputCount = input.InputCount }))).ToList();
            var dto = dtoList.Select(p => new GoTable { Year = p.Year, Value = p.TotalCost }).ToList();
            return new ListResultDto<GoTable>(dto);
        }


        /// <summary>
        /// 初版产品核价表下载-流
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal async virtual Task<MemoryStream> PriceEvaluationTableDownloadStream(PriceEvaluationTableDownloadStreamInput input)
        {
            if (input.IsAll)
            {
                var dtoAll = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = true }));
                DtoExcel(dtoAll);
                var memoryStream = new MemoryStream();
                await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/PriceEvaluationTable.xlsx", dtoAll);
                return memoryStream;
            }
            else
            {


                var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = false }));

                DtoExcel(dto);


                var memoryStream2 = new MemoryStream();

                await MiniExcel.SaveAsByTemplateAsync(memoryStream2, "wwwroot/Excel/PriceEvaluationTable.xlsx", dto);
                return memoryStream2;
            }
        }
        /// <summary>
        /// 初版产品核价表下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<FileResult> PriceEvaluationTableDownload(PriceEvaluationTableDownloadInput input)
        {
            var dtoAll = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = true }));
            var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = false }));

            DtoExcel(dtoAll);
            DtoExcel(dto);


            var memoryStream = new MemoryStream();
            var memoryStream2 = new MemoryStream();
            var memoryStream3 = new MemoryStream();

            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/PriceEvaluationTable.xlsx", dtoAll);
            await MiniExcel.SaveAsByTemplateAsync(memoryStream2, "wwwroot/Excel/PriceEvaluationTable.xlsx", dto);
            using (var zipArich = new ZipArchive(memoryStream3, ZipArchiveMode.Create, true))
            {

                var entry = zipArich.CreateEntry($"{dtoAll.Title.Replace("/", "")}.xlsx");
                using (System.IO.Stream stream = entry.Open())
                {
                    stream.Write(memoryStream.ToArray(), 0, memoryStream.Length.To<int>());
                }

                var entry2 = zipArich.CreateEntry($"{dto.Title.Replace("/", "")}.xlsx");
                using (System.IO.Stream stream = entry2.Open())
                {
                    stream.Write(memoryStream2.ToArray(), 0, memoryStream2.Length.To<int>());
                }
            }

            return new FileContentResult(memoryStream3.ToArray(), "application/octet-stream") { FileDownloadName = "产品核价表.zip" };

        }

        /// <summary>
        /// Dto Excel导出处理
        /// </summary>
        /// <param name="dtoAll"></param>
        private static void DtoExcel(ExcelPriceEvaluationTableDto dtoAll)
        {
            dtoAll.Material = dtoAll.Material.Select((a, b) => { a.Index = b + 1; return a; }).ToList();

            dtoAll.ManufacturingCostDto = dtoAll.ManufacturingCost.Select(p => new ManufacturingCostDto2
            {
                DirectLabor1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.DirectLabor,
                EquipmentDepreciation1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.EquipmentDepreciation,
                LineChangeCost1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.LineChangeCost,
                ManufacturingExpenses1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.ManufacturingExpenses,
                Subtotal1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.Subtotal,
                DirectLabor2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.DirectLabor,
                EquipmentDepreciation2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.EquipmentDepreciation,
                ManufacturingExpenses2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.ManufacturingExpenses,
                Subtotal2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.Subtotal,

                Subtotal = p.Subtotal,
                CostItem = p.CostItem,
                CostType = p.CostType
            }).ToList();

            dtoAll.Fixture = dtoAll.OtherCostItem.Fixture;
            dtoAll.LogisticsFee = dtoAll.OtherCostItem.LogisticsFee;
            dtoAll.ProductCategory = dtoAll.OtherCostItem.ProductCategory;
            dtoAll.CostProportion = dtoAll.OtherCostItem.CostProportion;
            dtoAll.CostProportionText = $"{dtoAll.OtherCostItem.CostProportion * 100}%";
            dtoAll.QualityCost = dtoAll.OtherCostItem.QualityCost;
            dtoAll.AccountingPeriod = dtoAll.OtherCostItem.AccountingPeriod;
            dtoAll.CapitalCostRate = dtoAll.OtherCostItem.CapitalCostRate;
            dtoAll.TaxCost = dtoAll.OtherCostItem.TaxCost;
            dtoAll.Total = dtoAll.OtherCostItem.Total;
            dtoAll.WastageCostCount = dtoAll.LossCost.Sum(p => p.WastageCost);
            dtoAll.MoqShareCountCount = dtoAll.LossCost.Sum(p => p.MoqShareCount);

            dtoAll.ShDzl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.ElectronicName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShJgl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.StructuralName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShJs = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.GlueMaterialName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShWxjg = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.SMTOutSourceName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShBzcl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.PackingMaterialName).Sum(p => p.WastageCost).ToString();


            dtoAll.MoqDzl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.ElectronicName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqJgl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.StructuralName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqJs = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.GlueMaterialName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqWxjg = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.SMTOutSourceName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqBzcl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.PackingMaterialName).Sum(p => p.MoqShareCount).ToString();


            dtoAll.TotalMoneyCynCount = dtoAll.Material.Sum(p => p.TotalMoneyCyn);
            dtoAll.LossCount = dtoAll.Material.Sum(p => p.Loss);
            dtoAll.LossRateCount = dtoAll.Material.Sum(p => p.LossRate);
        }


        /// <summary>
        /// 核价看板页面，设置是否客供。填写BOM的Id和是否客供
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task SetIsCustomerSupply(SetIsCustomerSupplyInput input)
        {
            var pe = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            pe.BomIsCustomerSupplyJson = input.BomIsCustomerSupplyList.ToJsonString();
        }

        /// <summary>
        /// 核价看板，梯度下拉框获取梯度接口
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<GradientListDto>> GetGradient(long auditFlowId)
        {
            var entity = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            return new PagedResultDto<GradientListDto>(entity.Count, ObjectMapper.Map<List<GradientListDto>>(entity));
        }

        /// <summary>
        /// 设置核价看板修改项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task SetEditItem(SetEditItemInput input)
        {
            var entity = await _editItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            if (entity is null)
            {
                await _editItemRepository.InsertAsync(new EditItem
                {
                    AuditFlowId = input.AuditFlowId,
                    EditItemJson = input.EditItem.ToJsonString(),
                });
            }
            else
            {
                entity.EditItemJson = input.EditItem.ToJsonString();
            }

        }
        #endregion



    }
}
