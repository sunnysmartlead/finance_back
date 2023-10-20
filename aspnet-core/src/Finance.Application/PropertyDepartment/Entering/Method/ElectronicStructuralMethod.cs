using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.Entering;
using Finance.Entering.Model;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Finance.Roles.Dto;
using Finance.Users;
using Finance.Users.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Finance.Authorization.Roles.StaticRoleNames;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Finance.PropertyDepartment.Entering.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class ElectronicStructuralMethod : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, ISingletonDependency
    {
        /// <summary>
        /// 是否涉及选项
        /// </summary>
        private static string IsInvolveItem = "是";
        /// <summary>
        /// 财务字典表明细
        /// </summary>
        private static IRepository<FinanceDictionaryDetail, string> _resourceFinanceDictionaryDetail;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        private static IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;
        /// <summary>
        /// 产品开发部结构BOM输入信息
        /// </summary>
        private static IRepository<StructureBomInfo, long> _resourceStructureBomInfo;
        /// <summary>
        /// 模组数量
        /// </summary>
        private static IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 模组数量年份
        /// </summary>
        private static IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        /// 基础单价库实体类
        /// </summary>
        private static IRepository<UInitPriceForm, long> _configUInitPriceForm;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private static IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 资源部电子物料录入
        /// </summary>
        private static IRepository<EnteringElectronic, long> _configEnteringElectronic;
        /// <summary>
        /// 资源部结构物料录入
        /// </summary>
        private static IRepository<StructureElectronic, long> _configStructureElectronic;
        /// <summary>
        /// 实体映射
        /// </summary>
        private static new IObjectMapper ObjectMapper;
        /// <summary>
        /// 
        /// </summary>
        private static UserManager _userManager;
        /// <summary>
        /// 电子BOM两次上传差异化表
        /// </summary>
        private static IRepository<ElecBomDifferent, long> _configElecBomDifferent;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        private readonly IUserAppService _userAppService;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;
        /// <summary>
        /// 共用物料库
        /// </summary>
        private readonly IRepository<SharedMaterialWarehouse, long> _sharedMaterialWarehouse;
        /// <summary>
        /// 梯度
        /// </summary>
        private readonly IRepository<Gradient, long> _gradient;
        /// <summary>
        /// 梯度模组
        /// </summary>
        private readonly IRepository<GradientModel, long> _gradientModel;
        /// <summary>
        /// 梯度模组年份
        /// </summary>
        private readonly IRepository<GradientModelYear, long> _gradientModelYear;
        /// <summary>
        /// 客户目标价
        /// </summary>
        private readonly IRepository<CustomerTargetPrice, long> _customerTargetPrice;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceFinanceDictionaryDetail"></param>
        /// <param name="electronicBomInfo"></param>
        /// <param name="structureBomInfo"></param>
        /// <param name="objectMapper"></param>
        /// <param name="modelCount"></param>
        /// <param name="modelCountYear"></param>
        /// <param name="uInitPriceForm"></param>
        /// <param name="exchangeRate"></param>
        /// <param name="enteringElectronic"></param>
        /// <param name="userManager"></param>
        /// <param name="repository"></param>
        /// <param name="structureElectronic"></param>
        /// <param name="elecBomDifferent"></param>
        /// <param name="structBomDifferent"></param>
        /// <param name="userAppService"></param>
        /// <param name="resourceSchemeTable"></param>
        /// <param name="sharedMaterialWarehouse"></param>
        /// <param name="gradientRepository"></param>
        /// <param name="gradientModelRepository"></param>
        /// <param name="gradientModelYearRepository"></param>
        /// <param name="customerTargetPrice"></param>
        public ElectronicStructuralMethod(
            IRepository<FinanceDictionaryDetail,
            string> resourceFinanceDictionaryDetail,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            IRepository<StructureBomInfo, long> structureBomInfo,
            IObjectMapper objectMapper,
            IRepository<ModelCount, long> modelCount,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<UInitPriceForm, long> uInitPriceForm,
            IRepository<ExchangeRate, long> exchangeRate,
            IRepository<EnteringElectronic, long> enteringElectronic,
            UserManager userManager,
            IRepository<User, long> repository,
            IRepository<StructureElectronic, long> structureElectronic,
            IRepository<ElecBomDifferent, long> elecBomDifferent,
            IRepository<StructBomDifferent, long> structBomDifferent,
            IUserAppService userAppService,
            IRepository<Solution, long> resourceSchemeTable,
            IRepository<SharedMaterialWarehouse, long> sharedMaterialWarehouse,
            IRepository<Gradient, long> gradientRepository,
            IRepository<GradientModel, long> gradientModelRepository,
            IRepository<GradientModelYear, long> gradientModelYearRepository,
            IRepository<CustomerTargetPrice, long> customerTargetPrice) : base(repository)
        {
            _resourceFinanceDictionaryDetail = resourceFinanceDictionaryDetail;
            _resourceElectronicBomInfo = electronicBomInfo;
            ObjectMapper = objectMapper;
            _resourceStructureBomInfo = structureBomInfo;
            _resourceModelCount = modelCount;
            _resourceModelCountYear = modelCountYear;
            _configUInitPriceForm = uInitPriceForm;
            _configExchangeRate = exchangeRate;
            _configEnteringElectronic = enteringElectronic;
            _userManager = userManager;
            _configStructureElectronic = structureElectronic;
            _configElecBomDifferent = elecBomDifferent;
            _configStructBomDifferent = structBomDifferent;
            _userAppService = userAppService;
            _resourceSchemeTable = resourceSchemeTable;
            _sharedMaterialWarehouse = sharedMaterialWarehouse;
            _gradient = gradientRepository;
            _gradientModel = gradientModelRepository;
            _gradientModelYear = gradientModelYearRepository;
            _customerTargetPrice = customerTargetPrice;
        }

        /// <summary>
        /// 总的方案
        /// </summary>
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId)
        {
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => auditFlowId == p.AuditFlowId);
            result = result.OrderBy(p => p.ModuleName).ToList();
            List<SolutionModel> partModel = (from a in result
                                             select new SolutionModel
                                             {
                                                 SolutionId = a.Id,
                                                 SolutionName = a.SolutionName,
                                                 ProductId = a.Productld,
                                             }).ToList();
            return partModel;
        }
        /// <summary>
        /// 根据筛选条件获取方案列表
        /// </summary>
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId, Func<Solution, bool> filter)
        {
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => auditFlowId == p.AuditFlowId);
            result = result.OrderBy(p => p.ModuleName).ToList();
            List<SolutionModel> partModel = (from a in result
                                             where filter(a)
                                             select new SolutionModel
                                             {
                                                 SolutionId = a.Id,
                                                 SolutionName = a.SolutionName,
                                                 ProductId = a.Productld,
                                             }).ToList();
            return partModel;
        }
        //总的梯度
        internal async Task<List<GradientValueModel>> TotalGradient(long auditFlowId)
        {
            List<GradientValueModel> gradientModels = (from gradient in await _gradient.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId))
                                                       select new GradientValueModel
                                                       {
                                                           AuditFlowId = gradient.AuditFlowId,
                                                           Kv = gradient.SystermGradientValue
                                                       }).ToList();
            return gradientModels;
        }
        /// <summary>
        /// 根据流程号 和梯度  获取 模组走量
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="kv"></param>
        /// <returns></returns>
        internal async Task<List<GradientModelYear>> NumberOfModules(long auditFlowId, decimal kv)
        {
            List<Gradient> gradients = await _gradient.GetAllListAsync(g => g.AuditFlowId == auditFlowId && g.SystermGradientValue.Equals(kv));
            List<GradientModel> gradientModels = await _gradientModel.GetAllListAsync(gm => gm.AuditFlowId == auditFlowId);
            List<GradientModelYear> gradientModelYears = await _gradientModelYear.GetAllListAsync(gmy => gmy.AuditFlowId == auditFlowId);
            List<GradientModelYear> gradientModelsAll = (from gradient in gradients
                                                         join gradientModel in gradientModels
                                                         on gradient.Id equals gradientModel.GradientId
                                                         join gradientModelYear in gradientModelYears
                                                         on gradientModel.Id equals gradientModelYear.GradientModelId
                                                         select new GradientModelYear
                                                         {
                                                             AuditFlowId = gradientModelYear.AuditFlowId,
                                                             GradientModelId = gradientModelYear.GradientModelId,
                                                             Year = gradientModelYear.Year,
                                                             Count = gradientModelYear.Count,
                                                             UpDown = gradientModelYear.UpDown,
                                                             ProductId = gradientModelYear.ProductId,
                                                         }).ToList();
            return gradientModelsAll;
        }
        /// <summary>
        /// 电子BOM单价录入
        /// </summary>
        /// <param name="solution">总共的方案</param>
        /// <param name="auditFlowId">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ElectronicDto>> ElectronicBomList(List<SolutionModel> solution, long auditFlowId)
        {
            try
            {
                //查询PCS中的梯度
                List<GradientValueModel> gradient = await TotalGradient(auditFlowId);
                List<ElectronicDto> electronicBomList = new List<ElectronicDto>();
                //总共的零件/方案
                foreach (SolutionModel item in solution)
                {
                    //通过零件号获取 模组数量中的 年度模组数量以及年份               
                    List<ModelCountYear> modelCountYearList = (await _resourceModelCountYear.GetAllListAsync(p => p.ProductId.Equals(item.ProductId)))
                        .Select(a => new ModelCountYear
                        {
                            ProductId = a.ProductId,
                            Year = a.Year,
                            Quantity = a.Quantity,
                            UpDown = a.UpDown
                        }).ToList();
                    List<ElectronicBomInfo> electronicBomInfo = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Equals(IsInvolveItem));
                    //循环查询到的 电子料BOM表单
                    foreach (ElectronicBomInfo BomInfo in electronicBomInfo)
                    {
                        //重新计算装配数量  SAP相同的料号装配数量需要相加
                        BomInfo.AssemblyQuantity = electronicBomInfo.Where(p => p.SapItemNum.Equals(BomInfo.SapItemNum)).Sum(p => p.AssemblyQuantity);
                        ElectronicDto electronicDto = new();
                        //将电子料BOM映射到ElectronicDto
                        electronicDto = ObjectMapper.Map<ElectronicDto>(BomInfo);
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(BomInfo.Id));
                        if (enteringElectronic != null)
                        {
                            //将电子料BOM映射到ElectronicDto
                            electronicDto = ObjectMapper.Map<ElectronicDto>(enteringElectronic);
                            electronicDto.CategoryName = BomInfo.CategoryName;//物料大类
                            electronicDto.TypeName = BomInfo.TypeName;//物料种类
                            electronicDto.SapItemNum = BomInfo.SapItemNum;//物料编号
                            electronicDto.SapItemName = BomInfo.SapItemName;//材料名称
                            electronicDto.AssemblyQuantity = BomInfo.AssemblyQuantity;//装配数量
                            //获取某个ID的人员信息
                            var user = GetAsync(new EntityDto<long> { Id = enteringElectronic.PeopleId });
                            if (user.Result is not null) electronicDto.PeopleName = user.Result.Name;
                            electronicBomList.Add(electronicDto);
                            continue;//直接进行下一个循环
                        }
                        //查询共用物料库
                        List<SharedMaterialWarehouse> sharedMaterialWarehouses = await _sharedMaterialWarehouse.GetAllListAsync(p => p.MaterialCode.Equals(BomInfo.SapItemNum));
                        //项目物料的使用量
                        electronicDto.MaterialsUseCount = new List<YearOrValueKvMode>();
                        foreach (GradientValueModel gradientItem in gradient)
                        {
                            List<GradientModelYear> gradientModelYears = await NumberOfModules(auditFlowId, gradientItem.Kv);
                            YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                            yearOrValueKvMode.YearOrValueModes = new();
                            yearOrValueKvMode.Kv = gradientItem.Kv;
                            foreach (ModelCountYear modelCountYear in modelCountYearList)
                            {
                                //公共物料库中装配数量乘以每年的走量
                                decimal sharedMaterialWarehousesModeCount = sharedMaterialWarehouses
                                    .SelectMany(sharedMaterial => JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(sharedMaterial.ModuleThroughputs)
                                    .Where(p => p.Year.Equals(modelCountYear.Year))
                                    .Select(yearOrValueModeCanNull => sharedMaterial.AssemblyQuantity * (yearOrValueModeCanNull.Value ?? 0)))
                                    .Sum();
                                decimal bomAssemblyQuantity = (decimal)BomInfo.AssemblyQuantity;
                                List<GradientModelYear> gradientModels = gradientModelYears.Where(p => p.ProductId.Equals(item.ProductId) && p.Year.Equals(modelCountYear.Year) && p.UpDown.Equals(modelCountYear.UpDown)).ToList();
                                if (gradientModels.Count is not 1) throw new FriendlyException("获取项目物料使用量时候,梯度走量不唯一");
                                decimal modelCountYearQuantity = gradientModels.FirstOrDefault().Count * 1000;
                                decimal value = bomAssemblyQuantity * modelCountYearQuantity + sharedMaterialWarehousesModeCount;
                                YearOrValueMode yearOrValueMode = new YearOrValueMode { Year = modelCountYear.Year, UpDown = modelCountYear.UpDown, Value = value };
                                yearOrValueKvMode.YearOrValueModes.Add(yearOrValueMode);
                            }
                            electronicDto.MaterialsUseCount.Add(yearOrValueKvMode);
                        }
                        //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
                        List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.MaterialCode.Equals(BomInfo.SapItemNum) && p.FreezeOrNot.Equals(FreezeOrNot.Thaw) && p.EffectiveDate < DateTime.Now && p.ExpirationDate > DateTime.Now);//&&!p.FrozenState&&p.EffectiveEndDate>DateTime.Now
                        List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.SupplierPriority.Equals(SupplierPriority.Core)).ToList();
                        List<UInitPriceForm> uInitPriceForm = uInitPriceFormsPriority.Count > 0 ? uInitPriceFormsPriority : uInitPriceForms;
                        //返回值分别是 系统单价（原币） 项目物料的年将率  物料返利金额   MOQ 货币编码  物料管制状态
                        var (systemiginalCurrency, inTheRate, rebateMoney, Moq, CurrencyCode, MaterialControlStatus) = await CalculateUnitPrice(gradient, modelCountYearList, uInitPriceForm, electronicDto.MaterialsUseCount, sharedMaterialWarehouses, item.SolutionId, auditFlowId);
                        electronicDto.Currency = CurrencyCode;//获取货币编码
                        electronicDto.SystemiginalCurrency = systemiginalCurrency;//系统单价（原币）
                        electronicDto.InTheRate = inTheRate;//项目物料的年将率
                        electronicDto.StandardMoney = await CalculateStandardMoney(electronicDto);//本位币
                        electronicDto.RebateMoney = rebateMoney;//物料返利金额
                        electronicDto.MOQ = Moq;//MOQ
                        electronicDto.MaterialControlStatus = MaterialControlStatus;//物料管制状态
                        //判断本位币是否值全部为空如果为空  则可以直接提交                  
                        //true 全部不会空                      
                        electronicDto.IsEntering = IsAllNullOrZero(electronicDto.StandardMoney.SelectMany(p => p.YearOrValueModes).ToList(), p => p.Value);
                        electronicDto.IsSystemiginal = electronicDto.IsEntering;
                        electronicBomList.Add(electronicDto);
                    }
                    //将项目物料使用量 SAP相同的料号项目物料使用量需要相加
                    //List<ElectronicDto> electroniprop = electronicBomList.DeepClone();
                    //foreach (ElectronicDto electronic in electronicBomList)
                    //{
                    //    List<ElectronicDto> electronicDtos = electroniprop.Where(p => p.SapItemNum.Equals(electronic.SapItemNum)).ToList();
                    //    List<YearOrValueKvMode> m = new();
                    //    electronicDtos.ForEach(p => m.AddRange(p.MaterialsUseCount));
                    //    foreach (var MaterialsUse in electronic.MaterialsUseCount)
                    //    {
                    //        foreach (YearOrValueMode YearOrValueMode in MaterialsUse.YearOrValueModes)
                    //        {
                    //            YearOrValueMode.Value = (m.Where(o => o.Kv.Equals(MaterialsUse.Kv)).SelectMany(o => o.YearOrValueModes)).Where(m => m.Year
                    //            .Equals(YearOrValueMode.Year) && m.UpDown.Equals(YearOrValueMode.UpDown)).Sum(o => o.Value);
                    //        }
                    //    }
                    //}
                }     
                return electronicBomList;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 电子BOM单价录入数量
        /// </summary>
        /// <param name="solution">总共的方案</param>
        /// <param name="auditFlowId">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ElectronicDto>> ElectronicBomListCount(List<SolutionModel> solution, long auditFlowId)
        {
            try
            {
                //查询PCS中的梯度
                List<GradientValueModel> gradient = await TotalGradient(auditFlowId);
                List<ElectronicDto> electronicBomList = new List<ElectronicDto>();
                //总共的零件/方案
                foreach (SolutionModel item in solution)
                {
                    List<ElectronicBomInfo> electronicBomInfo = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Equals(IsInvolveItem));
                    //循环查询到的 电子料BOM表单
                    foreach (ElectronicBomInfo BomInfo in electronicBomInfo)
                    {
                        ElectronicDto electronicDto = new();
                        //将电子料BOM映射到ElectronicDto
                        electronicDto = ObjectMapper.Map<ElectronicDto>(BomInfo);
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(BomInfo.Id));
                        if (enteringElectronic != null)
                        {
                            //将电子料BOM映射到ElectronicDto
                            electronicDto = ObjectMapper.Map<ElectronicDto>(enteringElectronic);
                            electronicBomList.Add(electronicDto);
                            continue;//直接进行下一个循环
                        }
                        electronicBomList.Add(electronicDto);
                    }
                }
                return electronicBomList;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 单个 物料编号的计算
        /// </summary>
        /// <param name="SolutionId"></param>
        /// <param name="ProductId"></param>
        /// <param name="auditFlowId"></param>
        /// <param name="ElectronicId"></param>
        /// <returns></returns>
        internal async Task<ElectronicDto> ElectronicBom(long SolutionId, long ProductId, long auditFlowId, long ElectronicId)
        {
            //查询PCS中的梯度
            List<GradientValueModel> gradient = await TotalGradient(auditFlowId);
            ElectronicBomInfo electronicBomInfo = await _resourceElectronicBomInfo.FirstOrDefaultAsync(p => p.Id.Equals(ElectronicId) && p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(SolutionId) && p.IsInvolveItem.Equals(IsInvolveItem));
            //通过零件号获取 模组数量中的 年度模组数量以及年份               
            List<ModelCountYear> modelCountYearList = (await _resourceModelCountYear.GetAllListAsync(p => p.ProductId.Equals(ProductId)))
                .Select(a => new ModelCountYear
                {
                    ProductId = a.ProductId,
                    Year = a.Year,
                    Quantity = a.Quantity,
                    UpDown = a.UpDown
                }).ToList();
            ElectronicDto electronicDto = new();
            //将电子料BOM映射到ElectronicDto
            electronicDto = ObjectMapper.Map<ElectronicDto>(electronicBomInfo);
            //查询共用物料库
            List<SharedMaterialWarehouse> sharedMaterialWarehouses = await _sharedMaterialWarehouse.GetAllListAsync(p => p.MaterialCode.Equals(electronicDto.SapItemNum));
            //项目物料的使用量
            electronicDto.MaterialsUseCount = new List<YearOrValueKvMode>();
            foreach (GradientValueModel gradientItem in gradient)
            {
                YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                yearOrValueKvMode.YearOrValueModes = new();
                yearOrValueKvMode.Kv = gradientItem.Kv;
                foreach (ModelCountYear modelCountYear in modelCountYearList)
                {
                    decimal sharedMaterialWarehousesModeCount = sharedMaterialWarehouses
                        .SelectMany(sharedMaterial => JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(sharedMaterial.ModuleThroughputs)
                        .Where(p => p.Year.Equals(modelCountYear.Year))
                        .Select(yearOrValueModeCanNull => sharedMaterial.AssemblyQuantity * (yearOrValueModeCanNull.Value ?? 0)))
                        .Sum();
                    decimal bomAssemblyQuantity = (decimal)electronicDto.AssemblyQuantity;
                    decimal modelCountYearQuantity = modelCountYear.Quantity;
                    decimal value = bomAssemblyQuantity * modelCountYearQuantity + sharedMaterialWarehousesModeCount;
                    YearOrValueMode yearOrValueMode = new YearOrValueMode { Year = modelCountYear.Year, UpDown = modelCountYear.UpDown, Value = value };
                    yearOrValueKvMode.YearOrValueModes.Add(yearOrValueMode);
                }
                electronicDto.MaterialsUseCount.Add(yearOrValueKvMode);
            }
            //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
            List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.MaterialCode.Equals(electronicDto.SapItemNum) && p.FreezeOrNot.Equals(FreezeOrNot.Thaw) && p.EffectiveDate < DateTime.Now && p.ExpirationDate > DateTime.Now);//&&!p.FrozenState&&p.EffectiveEndDate>DateTime.Now
            //通过优先级筛选
            List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.SupplierPriority.Equals(SupplierPriority.Core)).ToList();
            List<UInitPriceForm> uInitPriceForm = uInitPriceFormsPriority.Count > 0 ? uInitPriceFormsPriority : uInitPriceForms;
            //返回值分别是 系统单价（原币） 项目物料的年将率  物料返利金额   MOQ 货币编码  物料管制状态
            var (systemiginalCurrency, inTheRate, rebateMoney, Moq, CurrencyCode, MaterialControlStatus) = await CalculateUnitPrice(gradient, modelCountYearList, uInitPriceForm, electronicDto.MaterialsUseCount, sharedMaterialWarehouses, SolutionId, auditFlowId);
            electronicDto.Currency = CurrencyCode;//获取货币编码
            electronicDto.SystemiginalCurrency = systemiginalCurrency;//系统单价（原币）
            electronicDto.InTheRate = inTheRate;//项目物料的年将率
            electronicDto.StandardMoney = await CalculateStandardMoney(electronicDto);//本位币
            electronicDto.RebateMoney = rebateMoney;//物料返利金额
            electronicDto.MOQ = Moq;//MOQ
            electronicDto.MaterialControlStatus = MaterialControlStatus;//物料管制状态
            //判断本位币是否值全部为空如果为空  则可以直接提交                  
            //true 全部不会空     
            electronicDto.IsEntering = IsAllNullOrZero(electronicDto.StandardMoney.SelectMany(p => p.YearOrValueModes).ToList(), p => p.Value);
            return electronicDto;
        }
        /// <summary>
        /// 电子BOM单价审核
        /// </summary>
        /// <param name="price">总共的零件</param>
        /// <param name="auditFlowId">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ElectronicDto>> BOMElectronicBomList(List<SolutionModel> price, long auditFlowId)
        {

            ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());
            List<ElectronicDto> electronicBomList = new List<ElectronicDto>();
            //p总共的零件
            foreach (SolutionModel item in price)
            {
                //获取电子料bom表单  根据流程主键id 和 每一个零件的id  
                List<ElectronicBomInfo> electronicBomInfo = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Equals(IsInvolveItem));
                //循环查询到的 电子料BOM表单
                foreach (ElectronicBomInfo BomInfo in electronicBomInfo)
                {
                    //重新计算装配数量  SAP相同的料号装配数量需要相加
                    BomInfo.AssemblyQuantity = electronicBomInfo.Where(p => p.SapItemNum.Equals(BomInfo.SapItemNum)).Sum(p => p.AssemblyQuantity);
                    ElectronicDto electronicDto = new ElectronicDto();
                    //将电子料BOM映射到ElectronicDto
                    electronicDto = ObjectMapper.Map<ElectronicDto>(BomInfo);
                    //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(BomInfo.Id) && p.IsSubmit);
                    if (enteringElectronic != null)
                    {
                        //将电子料BOM映射到ElectronicDto
                        electronicDto = ObjectMapper.Map<ElectronicDto>(enteringElectronic);
                        electronicDto.CategoryName = BomInfo.CategoryName;//物料大类
                        electronicDto.TypeName = BomInfo.TypeName;//物料种类
                        electronicDto.SapItemNum = BomInfo.SapItemNum;//物料编号
                        electronicDto.SapItemName = BomInfo.SapItemName;//材料名称
                        electronicDto.AssemblyQuantity = BomInfo.AssemblyQuantity;//装配数量
                        var user = GetAsync(new EntityDto<long> { Id = enteringElectronic.PeopleId });
                        if (user.Result is not null) electronicDto.PeopleName = user.Result.Name;
                        //返利金额不给项目经理看
                        if (Roles.Items.Where(p => p.Name.Equals(Host.ProjectManager)).ToList().Count is not 0)
                        {
                            electronicDto.RebateMoney = new List<KvMode>();
                        }
                        electronicBomList.Add(electronicDto);
                        continue;//直接进行下一个循环
                    }

                }
            }
            return electronicBomList;
        }
        /// <summary>
        /// 结构件单价录入
        /// </summary>
        /// <param name="price"></param>
        /// <param name="auditFlowId"></param>
        /// <param name="structureBOMIdDeleted"></param>
        /// <param name="structureBOMIdModify"></param>
        /// <returns></returns>
        internal async Task<List<ConstructionDto>> ConstructionBomList(List<SolutionModel> price, long auditFlowId, List<long> structureBOMIdDeleted, List<long> structureBOMIdModify)
        {
            List<ConstructionDto> constructionDtos = new List<ConstructionDto>();
            //查询PCS中的梯度
            List<GradientValueModel> gradient = await TotalGradient(auditFlowId);
            foreach (SolutionModel item in price)//循环方案
            {
                //通过零件号获取 模组数量中的 年度模组数量以及年份               
                List<ModelCountYear> modelCountYearList = (await _resourceModelCountYear.GetAllListAsync(p => p.ProductId.Equals(item.ProductId)))
                    .Select(a => new ModelCountYear
                    {
                        ProductId = a.ProductId,
                        Year = a.Year,
                        Quantity = a.Quantity,
                        UpDown = a.UpDown
                    }).ToList();
                List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Contains(IsInvolveItem));
                List<string> structureBomInfosGr = structureBomInfos.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重
                // 按照结构料、胶水、包材顺序排序
                structureBomInfosGr = structureBomInfosGr.OrderBy(m =>
                {

                    if (m.Contains("结构料"))
                    {
                        return 1;
                    }
                    else if (m.Contains("胶水"))
                    {
                        return 2;
                    }
                    else if (m.Contains("包材"))
                    {
                        return 3;
                    }

                    return 4;
                }).ToList();
                foreach (string SuperTypeName in structureBomInfosGr)//超级大种类  结构料 胶水等辅材 SMT外协 包材
                {
                    List<StructureBomInfo> StructureMaterialnfp = structureBomInfos.Where(p => p.SuperTypeName.Equals(SuperTypeName)).ToList(); //查找属于这一超级大类的
                    List<ConstructionModel> constructionModels = ObjectMapper.Map<List<ConstructionModel>>(StructureMaterialnfp);// 结构BOM表单 模型

                    foreach (ConstructionModel construction in constructionModels)
                    {
                        //重新计算装配数量  SAP相同的料号装配数量需要相加
                        construction.AssemblyQuantity = constructionModels.Where(p => p.SapItemNum.Equals(construction.SapItemNum)).Sum(p => p.AssemblyQuantity);
                        //查询共用物料库
                        List<SharedMaterialWarehouse> sharedMaterialWarehouses = await _sharedMaterialWarehouse.GetAllListAsync(p => p.MaterialCode.Equals(construction.SapItemNum));
                        int count = structureBOMIdDeleted.Where(p => p.Equals(construction.StructureId)).Count();//如果改id删除了就跳过
                        if (count != 0)
                        {
                            continue;//直接进行下一个循环
                        }
                        #region 获取物料使用量                  
                        //项目物料的使用量
                        construction.MaterialsUseCount = new List<YearOrValueKvMode>();
                        foreach (GradientValueModel gradientItem in gradient)
                        {
                            List<GradientModelYear> gradientModelYears = await NumberOfModules(auditFlowId, gradientItem.Kv);
                            YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                            yearOrValueKvMode.YearOrValueModes = new List<YearOrValueMode>();
                            yearOrValueKvMode.Kv = gradientItem.Kv;
                            foreach (ModelCountYear modelCountYear in modelCountYearList)
                            {
                                decimal sharedMaterialWarehousesModeCount = sharedMaterialWarehouses
                                    .SelectMany(sharedMaterial => JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(sharedMaterial.ModuleThroughputs)
                                    .Where(p => p.Year.Equals(modelCountYear.Year))
                                    .Select(yearOrValueModeCanNull => sharedMaterial.AssemblyQuantity * (yearOrValueModeCanNull.Value ?? 0)))
                                    .Sum();
                                decimal bomAssemblyQuantity = (decimal)construction.AssemblyQuantity;
                                List<GradientModelYear> gradientModels = gradientModelYears.Where(p => p.ProductId.Equals(item.ProductId) && p.Year.Equals(modelCountYear.Year) && p.UpDown.Equals(modelCountYear.UpDown)).ToList();
                                if (gradientModels.Count is not 1) throw new FriendlyException("获取项目物料使用量时候,梯度走量不唯一");
                                decimal modelCountYearQuantity = gradientModels.FirstOrDefault().Count * 1000;
                                decimal value = bomAssemblyQuantity * modelCountYearQuantity + sharedMaterialWarehousesModeCount;
                                YearOrValueMode yearOrValueMode = new YearOrValueMode { Year = modelCountYear.Year, UpDown = modelCountYear.UpDown, Value = value };
                                yearOrValueKvMode.YearOrValueModes.Add(yearOrValueMode);
                            }
                            construction.MaterialsUseCount.Add(yearOrValueKvMode);
                        }
                        #endregion
                        #region 返回值分别是 系统单价（原币） 项目物料的年将率  物料返利金额   MOQ 货币编码  物料管制状态
                        //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
                        List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.MaterialCode.Equals(construction.SapItemNum) && p.FreezeOrNot.Equals(FreezeOrNot.Thaw) && p.EffectiveDate < DateTime.Now && p.ExpirationDate > DateTime.Now);
                        //通过优先级筛选
                        List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.SupplierPriority.Equals(SupplierPriority.Core)).ToList();
                        List<UInitPriceForm> uInitPriceForm = uInitPriceFormsPriority.Count > 0 ? uInitPriceFormsPriority : uInitPriceForms;
                        //返回值分别是 系统单价（原币） 项目物料的年将率  物料返利金额   MOQ 货币编码  物料管制状态
                        var (systemiginalCurrency, inTheRate, rebateMoney, Moq, CurrencyCode, MaterialControlStatus) = await CalculateUnitPrice(gradient, modelCountYearList, uInitPriceForm, construction.MaterialsUseCount, sharedMaterialWarehouses, item.SolutionId, auditFlowId);

                        #endregion
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(construction.StructureId));
                        if (structureElectronic != null)
                        {
                            construction.Id = structureElectronic.Id;
                            construction.MaterialControlStatus = structureElectronic.MaterialControlStatus;//物料管制状态
                            construction.Currency = structureElectronic.Currency;//币种                       
                            construction.SolutionId = item.SolutionId;//方案ID
                            construction.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.StandardMoney);//本位币                           
                            construction.RebateMoney = JsonConvert.DeserializeObject<List<KvMode>>(structureElectronic.RebateMoney);//物料可返回金额
                            construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.InTheRate);//年降
                            construction.SystemiginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.SystemiginalCurrency);//原币
                            construction.PeopleId = structureElectronic.PeopleId;//确认人
                            construction.IsSubmit = structureElectronic.IsSubmit;//是否提交
                            construction.IsEntering = structureElectronic.IsEntering;//是否录入
                            construction.MOQ = structureElectronic.MOQ;//MOQ
                            construction.Remark = structureElectronic.Remark;//备注
                            construction.IsSystemiginal= structureElectronic.IsSystemiginal;// 系统单价是否从单价库中带出
                            var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = structureElectronic.PeopleId });
                            if (user.Result is not null) construction.PeopleName = user.Result.Name;
                            int countUp = structureBOMIdModify.Where(p => p.Equals(construction.StructureId)).Count();//如果修改了,重置参数
                            if (countUp != 0)
                            {
                                construction.MOQ = Moq;//MOQ
                                construction.StandardMoney = systemiginalCurrency;//本位币
                                construction.RebateMoney = rebateMoney;//物料可返利金额
                                construction.InTheRate = inTheRate;//年降
                                construction.SystemiginalCurrency = systemiginalCurrency;//原币
                                construction.Currency = CurrencyCode;//获取币种
                                construction.MaterialControlStatus = MaterialControlStatus;//物料管制状态     
                                construction.IsEntering = false;//确认重置
                                construction.IsSubmit = false;//提交重置
                                construction.IsSystemiginal= IsAllNullOrZero(construction.StandardMoney.SelectMany(p => p.YearOrValueModes).ToList(), p => p.Value);
                                continue;//直接进行下一个循环
                            }
                            continue;//直接进行下一个循环
                        }
                        construction.SystemiginalCurrency = systemiginalCurrency;//系统单价（原币）
                        construction.Currency = CurrencyCode;//获取币种
                        construction.MaterialControlStatus = MaterialControlStatus;//物料管制状态                     
                        construction.RebateMoney = rebateMoney;//物料返利金额
                        construction.InTheRate = inTheRate;//年降
                        construction.StandardMoney = await CalculateStandardMoney(construction);//本位币
                        //判断本位币是否值全部为空如果为空  则可以直接提交                  
                        //true 全部不会空     
                        construction.IsEntering = IsAllNullOrZero(construction.StandardMoney.SelectMany(p => p.YearOrValueModes).ToList(), p => p.Value);
                        construction.RebateMoney = rebateMoney;//物料可返利金额
                        construction.MOQ = Moq;//MOQ
                        construction.SolutionId = item.SolutionId;//方案ID
                        construction.IsSystemiginal = construction.IsEntering;
                    }
                    

                    //将项目物料使用量 SAP相同的料号项目物料使用量需要相加
                    //List<ConstructionModel> constructionprop = constructionModels.DeepClone();
                    //foreach (ConstructionModel electronic in constructionModels)
                    //{
                    //    List<ConstructionModel> electronicDtos = constructionprop.Where(p => p.SapItemNum.Equals(electronic.SapItemNum)).ToList();
                    //    List<YearOrValueKvMode> m = new();
                    //    electronicDtos.ForEach(p => m.AddRange(p.MaterialsUseCount));
                    //    foreach (var MaterialsUse in electronic.MaterialsUseCount)
                    //    {
                    //        foreach (YearOrValueMode YearOrValueMode in MaterialsUse.YearOrValueModes)
                    //        {
                    //            YearOrValueMode.Value = (m.Where(o => o.Kv.Equals(MaterialsUse.Kv)).SelectMany(o => o.YearOrValueModes)).Where(m => m.Year
                    //            .Equals(YearOrValueMode.Year) && m.UpDown.Equals(YearOrValueMode.UpDown)).Sum(o => o.Value);
                    //        }
                    //    }
                    //}

                    ConstructionDto constructionDto = new ConstructionDto()
                    {
                        SuperTypeName = SuperTypeName,
                        StructureMaterial = constructionModels,
                    };

                    constructionDtos.Add(constructionDto);
                }
            }
            return constructionDtos;
        }
        /// <summary>
        /// 结构件单价录入数量
        /// </summary>
        /// <param name="price"></param>
        /// <param name="auditFlowId"></param>
        /// <param name="structureBOMIdDeleted"></param>
        /// <param name="structureBOMIdModify"></param>
        /// <returns></returns>
        internal async Task<List<ConstructionDto>> ConstructionBomListCount(List<SolutionModel> price, long auditFlowId, List<long> structureBOMIdDeleted, List<long> structureBOMIdModify)
        {
            List<ConstructionDto> constructionDtos = new List<ConstructionDto>();
            //查询PCS中的梯度
            List<GradientValueModel> gradient = await TotalGradient(auditFlowId);
            foreach (SolutionModel item in price)//循环方案
            {

                List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Contains(IsInvolveItem));
                List<string> structureBomInfosGr = structureBomInfos.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重
                foreach (string SuperTypeName in structureBomInfosGr)//超级大种类  结构料 胶水等辅材 SMT外协 包材
                {
                    List<StructureBomInfo> StructureMaterialnfp = structureBomInfos.Where(p => p.SuperTypeName.Equals(SuperTypeName)).ToList(); //查找属于这一超级大类的
                    List<ConstructionModel> constructionModels = ObjectMapper.Map<List<ConstructionModel>>(StructureMaterialnfp);// 结构BOM表单 模型                     
                    ConstructionDto constructionDto = new ConstructionDto()
                    {
                        SuperTypeName = SuperTypeName,
                        StructureMaterial = constructionModels,
                    };
                    constructionDtos.Add(constructionDto);
                }
            }
            return constructionDtos;
        }
        /// <summary>
        /// 结构料BOM单价审核
        /// </summary>
        /// <param name="price">总共的零件</param>
        /// <param name="Id">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ConstructionDto>> BOMConstructionBomList(List<SolutionModel> price, long Id)
        {
            ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());
            List<ConstructionDto> constructionDtos = new List<ConstructionDto>();
            //循环模组
            foreach (SolutionModel item in price)
            {
                List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(item.SolutionId) && p.IsInvolveItem.Contains(IsInvolveItem));
                List<string> structureBomInfosGr = structureBomInfos.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重
                //超级大种类  结构料 胶水等辅材 SMT外协 包材
                // 按照结构料、胶水、包材顺序排序
                structureBomInfosGr = structureBomInfosGr.OrderBy(m =>
                {
                    if (m.Contains("结构料"))
                    {
                        return 1;
                    }
                    else if (m.Contains("胶水"))
                    {
                        return 2;
                    }
                    else if (m.Contains("包材"))
                    {
                        return 3;
                    }

                    return 4;
                }).ToList();
                foreach (string SuperTypeName in structureBomInfosGr)
                {
                    List<StructureBomInfo> StructureMaterialnfp = structureBomInfos.Where(p => p.SuperTypeName.Equals(SuperTypeName)).ToList(); //查找属于这一超级大类的
                    List<ConstructionModel> constructionModels = ObjectMapper.Map<List<ConstructionModel>>(StructureMaterialnfp);// 结构BOM表单 模型

                    List<ConstructionModel> RemoveconstructionModels = new List<ConstructionModel>();
                    foreach (ConstructionModel construction in constructionModels)
                    {
                        //重新计算装配数量  SAP相同的料号装配数量需要相加
                        construction.AssemblyQuantity = constructionModels.Where(p => p.SapItemNum.Equals(construction.SapItemNum)).Sum(p => p.AssemblyQuantity);
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(construction.StructureId) && p.IsSubmit);
                        if (structureElectronic != null)
                        {
                            construction.Id = structureElectronic.Id;//id
                            construction.MaterialControlStatus = structureElectronic.MaterialControlStatus;//物料管制状态
                            construction.Currency = structureElectronic.Currency;//币种                                                 
                            construction.MaterialsUseCount = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.MaterialsUseCount);//项目物料的使用量 
                            construction.SystemiginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.SystemiginalCurrency);//系统单价（原币）
                            construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.InTheRate);//项目物料的年降率 
                            construction.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.StandardMoney);//本位币 
                            construction.RebateMoney = Roles.Items.Where(p => p.Name.Equals(Host.ProjectManager)).ToList().Count is 0 ? JsonConvert.DeserializeObject<List<KvMode>>(structureElectronic.RebateMoney) : new List<KvMode>();//物料可返回金额(不给项目经理看)
                            construction.MOQ = structureElectronic.MOQ;//MOQ
                            construction.PeopleId = structureElectronic.PeopleId;//确认人
                            construction.Remark = structureElectronic.Remark;//备注
                            construction.SolutionId = item.SolutionId;//方案ID
                            construction.IsSystemiginal = structureElectronic.IsSystemiginal;// 系统单价是否从单价库中带出
                            var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = structureElectronic.PeopleId });
                            if (user.Result is not null) construction.PeopleName = user.Result.Name;
                            continue;//直接进行下一个循环
                        }
                        else
                        {
                            RemoveconstructionModels.Add(construction);
                        }
                    }
                    foreach (var Removeconstruction in RemoveconstructionModels)
                    {
                        constructionModels.Remove(Removeconstruction);
                    }
                    ConstructionDto constructionDto = new ConstructionDto()
                    {
                        SuperTypeName = SuperTypeName,
                        StructureMaterial = constructionModels,
                    };
                    constructionDtos.Add(constructionDto);
                }
            }
            return constructionDtos;
        }
        /// <summary>
        /// 计算单价  返回值分别是 系统单价（原币） 项目物料的年将率  物料返利金额   MOQ 货币编码  物料管制状态
        /// </summary>
        /// <param name="gradient">PCS中的梯度</param>
        /// <param name="modelCountYears">模组数量中的 年度模组数量以及年份</param>
        /// <param name="uInitPrice">基础单价库数据</param>
        /// <param name="materialsUseCount">项目物料的使用量</param>
        /// <param name="sharedMaterialWarehouses">共用物料库</param>
        /// <param name="solutionId">方案ID</param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        internal async Task<(List<YearOrValueKvMode>, List<YearOrValueKvMode>, List<KvMode>, decimal, string, string)> CalculateUnitPrice(List<GradientValueModel> gradient, List<ModelCountYear> modelCountYears, List<UInitPriceForm> uInitPrice, List<YearOrValueKvMode> materialsUseCount, List<SharedMaterialWarehouse> sharedMaterialWarehouses, long solutionId, long auditFlowId)
        {
            try
            {
                List<YearOrValueKvMode> yearOrValueModesOriginal = new List<YearOrValueKvMode>();
                List<YearOrValueKvMode> yearOrValueModesAnnualDecline = new List<YearOrValueKvMode>();
                List<KvMode> kvModes = new List<KvMode>();
                decimal Moq = 0;//MOQ
                string CurrencyCode = "";//币种
                string MaterialControlStatus = "待定";//物料管制状态
                //判断是否存在阶梯价
                if (uInitPrice.Count > 0)
                {
                    //判断报价类型
                    UInitPriceForm priceForm = uInitPrice.First();
                    Moq = priceForm.Moq;//MOQ
                    CurrencyCode = priceForm.CurrencyCode;//币种
                    MaterialControlStatus = priceForm.MaterialControlStatus.GetDescription();//物料管制状态
                    //如果是/ 的话
                    if (priceForm.QuotationType.Equals(QuotationType.Nothing))
                    {
                        //循环梯度
                        foreach (GradientValueModel gradientItem in gradient)
                        {
                            //原币对象
                            YearOrValueKvMode yearOrValueKvModeOriginal = new YearOrValueKvMode();
                            yearOrValueKvModeOriginal.YearOrValueModes = new();
                            yearOrValueKvModeOriginal.Kv = gradientItem.Kv;//梯度

                            //毛利率对象
                            YearOrValueKvMode yearOrValueKvModeAnnualDecline = new YearOrValueKvMode();
                            yearOrValueKvModeAnnualDecline.YearOrValueModes = new();
                            yearOrValueKvModeAnnualDecline.Kv = gradientItem.Kv;//梯度

                            KvMode kvMode = new KvMode();
                            kvMode.Kv = gradientItem.Kv;//梯度
                            //循环年份
                            foreach (ModelCountYear item in modelCountYears)
                            {
                                YearOrValueMode yearOrValueModeOriginal = new YearOrValueMode();
                                yearOrValueModeOriginal.Year = item.Year;
                                List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(priceForm.UInitPriceFormYearOrValueModes);
                                if (item.Year == DateTime.Now.Year)
                                {
                                    yearOrValueModeOriginal.Value = priceForm.BulkUntaxedPrice / 1000;
                                }
                                else
                                {
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)
                                    && p.Year.Equals(item.Year));
                                    if (uInitPriceFormYearOrValueMode is null)
                                    {
                                        uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)).OrderByDescending(p => p.Year).FirstOrDefault();
                                    }
                                    yearOrValueModeOriginal.Value = uInitPriceFormYearOrValueMode.Value / 1000;
                                }
                                yearOrValueModeOriginal.UpDown = item.UpDown;
                                yearOrValueKvModeOriginal.YearOrValueModes.Add(yearOrValueModeOriginal);

                                //创建年降对象
                                UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate) && p.Year.Equals(item.Year));
                                if (uInitPriceFormYearOrValueModeAnnualDecline is null)
                                {
                                    uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                YearOrValueMode yearOrValueModeAnnualDecline = new YearOrValueMode();
                                yearOrValueModeAnnualDecline.Year = item.Year;
                                yearOrValueModeAnnualDecline.UpDown = item.UpDown;
                                yearOrValueModeAnnualDecline.Value = uInitPriceFormYearOrValueModeAnnualDecline.Value;
                                yearOrValueKvModeAnnualDecline.YearOrValueModes.Add(yearOrValueModeAnnualDecline);


                                //当前梯度和当前年份的物料使用量
                                decimal sharedMaterialWarehousesModeCount = materialsUseCount.Where(p => p.Kv.Equals(gradientItem.Kv))
                                                         .SelectMany(p => p.YearOrValueModes)
                                                         .Where(p => p.Year.Equals(item.Year) && p.UpDown.Equals(item.UpDown))
                                                         .Select(p => p.Value)
                                                         .Sum();

                                //获取汇率值                    
                                decimal exchangeRateModelValue = await ObtainExchangeRate(auditFlowId, priceForm.CurrencyCode, item.Year, solutionId, gradientItem.Kv);
                                //返点率
                                UInitPriceFormYearOrValueMode rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)
                               && p.Year.Equals(item.Year));
                                if (rebateRateYearOrValueMode is null)
                                {
                                    rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                kvMode.Value += sharedMaterialWarehousesModeCount * yearOrValueModeOriginal.Value * exchangeRateModelValue * rebateRateYearOrValueMode.Value;
                            }
                            //将年份和价格对象和物料返利金额添加到列表中
                            yearOrValueModesOriginal.Add(yearOrValueKvModeOriginal);
                            yearOrValueModesAnnualDecline.Add(yearOrValueKvModeAnnualDecline);
                            kvModes.Add(kvMode);
                        }
                    }
                    else if (priceForm.QuotationType.Equals(QuotationType.Batch))//批量
                    {
                        //循环梯度
                        foreach (GradientValueModel gradientItem in gradient)
                        {
                            //原币对象
                            YearOrValueKvMode yearOrValueKvModeOriginal = new YearOrValueKvMode();
                            yearOrValueKvModeOriginal.YearOrValueModes = new();
                            yearOrValueKvModeOriginal.Kv = gradientItem.Kv;//梯度

                            //毛利率对象
                            YearOrValueKvMode yearOrValueKvModeAnnualDecline = new YearOrValueKvMode();
                            yearOrValueKvModeAnnualDecline.YearOrValueModes = new();
                            yearOrValueKvModeAnnualDecline.Kv = gradientItem.Kv;//梯度

                            KvMode kvMode = new KvMode();
                            kvMode.Kv = gradientItem.Kv;//梯度
                            //循环年份
                            foreach (ModelCountYear item in modelCountYears)
                            {
                                YearOrValueMode yearOrValueModeOriginal = new YearOrValueMode();
                                yearOrValueModeOriginal.Year = item.Year;
                                //decimal sharedMaterialWarehousesModeCount = sharedMaterialWarehouses
                                //           .SelectMany(sharedMaterial => JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(sharedMaterial.ModuleThroughputs)
                                //           .Where(p => p.Year.Equals(item.Year))
                                //           .Select(yearOrValueModeCanNull => sharedMaterial.AssemblyQuantity * (yearOrValueModeCanNull.Value ?? 0) + priceForm.AccumulatedProcurementQuantity))
                                //           .Sum();

                                decimal sharedMaterialWarehousesModeCount = materialsUseCount.Where(p => p.Kv.Equals(gradientItem.Kv))
                                                         .SelectMany(p => p.YearOrValueModes)
                                                         .Where(p => p.Year <= item.Year && ((item.UpDown == YearType.FirstHalf) ? !(p.UpDown.Equals(YearType.SecondHalf) && p.Year == item.Year) : true))
                                                         .Select(p => p.Value)
                                                         .Sum() + priceForm.AccumulatedProcurementQuantity;
                                //获取区间的一条
                                UInitPriceForm uInitPriceForm = SelectNearest(uInitPrice, sharedMaterialWarehousesModeCount, item => item.BatchStartQuantity);
                                //如果没有符合的取价格最低的哪条
                                if (uInitPriceForm is null)
                                {
                                    uInitPriceForm = uInitPrice.OrderByDescending(p => p.BulkUntaxedPrice).FirstOrDefault();
                                }
                                List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(uInitPriceForm.UInitPriceFormYearOrValueModes);
                                if (item.Year == DateTime.Now.Year)
                                {
                                    yearOrValueModeOriginal.Value = uInitPriceForm.BulkUntaxedPrice / 1000;
                                }
                                else
                                {
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)
                                    && p.Year.Equals(item.Year));
                                    if (uInitPriceFormYearOrValueMode is null)
                                    {
                                        uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)).OrderByDescending(p => p.Year).FirstOrDefault();
                                    }
                                    yearOrValueModeOriginal.Value = uInitPriceFormYearOrValueMode.Value / 1000;
                                }
                                yearOrValueModeOriginal.UpDown = item.UpDown;
                                yearOrValueKvModeOriginal.YearOrValueModes.Add(yearOrValueModeOriginal);


                                //创建年降对象
                                UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate) && p.Year.Equals(item.Year));
                                if (uInitPriceFormYearOrValueModeAnnualDecline is null)
                                {
                                    uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                YearOrValueMode yearOrValueModeAnnualDecline = new YearOrValueMode();
                                yearOrValueModeAnnualDecline.Year = item.Year;
                                yearOrValueModeAnnualDecline.UpDown = item.UpDown;
                                yearOrValueModeAnnualDecline.Value = uInitPriceFormYearOrValueModeAnnualDecline.Value;
                                yearOrValueKvModeAnnualDecline.YearOrValueModes.Add(yearOrValueModeAnnualDecline);


                                //当前梯度和当前年份的物料使用量
                                decimal materialsUseCountModeCount = materialsUseCount.Where(p => p.Kv.Equals(gradientItem.Kv))
                                                         .SelectMany(p => p.YearOrValueModes)
                                                         .Where(p => p.Year.Equals(item.Year) && p.UpDown.Equals(item.UpDown))
                                                         .Select(p => p.Value)
                                                         .Sum();
                                //获取汇率值                    
                                decimal exchangeRateModelValue = await ObtainExchangeRate(auditFlowId, priceForm.CurrencyCode, item.Year, solutionId, gradientItem.Kv);
                                //返点率
                                UInitPriceFormYearOrValueMode rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)
                               && p.Year.Equals(item.Year));
                                if (rebateRateYearOrValueMode is null)
                                {
                                    rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                kvMode.Value += materialsUseCountModeCount * yearOrValueModeOriginal.Value * exchangeRateModelValue * rebateRateYearOrValueMode.Value;
                            }
                            //将年份和价格对象和物料返利金额添加到列表中
                            yearOrValueModesOriginal.Add(yearOrValueKvModeOriginal);
                            yearOrValueModesAnnualDecline.Add(yearOrValueKvModeAnnualDecline);
                            kvModes.Add(kvMode);
                        }
                    }
                    else if (priceForm.QuotationType.Equals(QuotationType.Ladder))//阶梯
                    {
                        //循环梯度
                        foreach (GradientValueModel gradientItem in gradient)
                        {
                            //原币对象
                            YearOrValueKvMode yearOrValueKvModeOriginal = new YearOrValueKvMode();
                            yearOrValueKvModeOriginal.YearOrValueModes = new();
                            yearOrValueKvModeOriginal.Kv = gradientItem.Kv;//梯度

                            //毛利率对象
                            YearOrValueKvMode yearOrValueKvModeAnnualDecline = new YearOrValueKvMode();
                            yearOrValueKvModeAnnualDecline.YearOrValueModes = new();
                            yearOrValueKvModeAnnualDecline.Kv = gradientItem.Kv;//梯度

                            KvMode kvMode = new KvMode();
                            kvMode.Kv = gradientItem.Kv;//梯度
                            //循环年份
                            foreach (var item in modelCountYears)
                            {
                                YearOrValueMode yearOrValueModeOriginal = new YearOrValueMode();
                                yearOrValueModeOriginal.Year = item.Year;

                                decimal sharedMaterialWarehousesModeCount = materialsUseCount.Where(p => p.Kv.Equals(gradientItem.Kv))
                                                         .SelectMany(p => p.YearOrValueModes)
                                                         .Where(p => p.Year.Equals(item.Year))
                                                         .Select(p => p.Value)
                                                         .Sum();
                                //获取区间的一条
                                UInitPriceForm uInitPriceForm = SelectNearest(uInitPrice, sharedMaterialWarehousesModeCount, item => item.BatchStartQuantity);
                                //如果没有符合的取价格最低的哪条
                                if (uInitPriceForm is null)
                                {
                                    uInitPriceForm = uInitPrice.OrderByDescending(p => p.BulkUntaxedPrice).FirstOrDefault();
                                }
                                List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(uInitPriceForm.UInitPriceFormYearOrValueModes);
                                if (item.Year == DateTime.Now.Year)
                                {
                                    yearOrValueModeOriginal.Value = uInitPriceForm.BulkUntaxedPrice / 1000;
                                }
                                else
                                {
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)
                                    && p.Year.Equals(item.Year));
                                    if (uInitPriceFormYearOrValueMode is null)
                                    {
                                        uInitPriceFormYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualUntaxedPrice)).OrderByDescending(p => p.Year).FirstOrDefault();
                                    }
                                    yearOrValueModeOriginal.Value = uInitPriceFormYearOrValueMode.Value / 1000;
                                }
                                yearOrValueModeOriginal.UpDown = item.UpDown;
                                yearOrValueKvModeOriginal.YearOrValueModes.Add(yearOrValueModeOriginal);

                                //创建年降对象
                                UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate) && p.Year.Equals(item.Year));
                                if (uInitPriceFormYearOrValueModeAnnualDecline is null)
                                {
                                    uInitPriceFormYearOrValueModeAnnualDecline = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.AnnualDeclineRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                YearOrValueMode yearOrValueModeAnnualDecline = new YearOrValueMode();
                                yearOrValueModeAnnualDecline.Year = item.Year;
                                yearOrValueModeAnnualDecline.UpDown = item.UpDown;
                                yearOrValueModeAnnualDecline.Value = uInitPriceFormYearOrValueModeAnnualDecline.Value;
                                yearOrValueKvModeAnnualDecline.YearOrValueModes.Add(yearOrValueModeAnnualDecline);

                                //当前梯度和当前年份的物料使用量
                                decimal materialsUseCountModeCount = materialsUseCount.Where(p => p.Kv.Equals(gradientItem.Kv))
                                                         .SelectMany(p => p.YearOrValueModes)
                                                         .Where(p => p.Year.Equals(item.Year) && p.UpDown.Equals(item.UpDown))
                                                         .Select(p => p.Value)
                                                         .Sum();
                                //获取汇率值                    
                                decimal exchangeRateModelValue = await ObtainExchangeRate(auditFlowId, priceForm.CurrencyCode, item.Year, solutionId, gradientItem.Kv);
                                //返点率
                                UInitPriceFormYearOrValueMode rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)
                               && p.Year.Equals(item.Year));
                                if (rebateRateYearOrValueMode is null)
                                {
                                    rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                                }
                                kvMode.Value += materialsUseCountModeCount * yearOrValueModeOriginal.Value * exchangeRateModelValue * rebateRateYearOrValueMode.Value;
                            }
                            //将年份和价格对象和物料返利金额添加到列表中
                            yearOrValueModesOriginal.Add(yearOrValueKvModeOriginal);
                            yearOrValueModesAnnualDecline.Add(yearOrValueKvModeAnnualDecline);
                            kvModes.Add(kvMode);
                        }
                    }
                }
                else
                {
                    //循环梯度
                    foreach (GradientValueModel gradientItem in gradient)
                    {
                        YearOrValueKvMode yearOrValueKvModeOriginal = new YearOrValueKvMode();
                        yearOrValueKvModeOriginal.YearOrValueModes = new();
                        yearOrValueKvModeOriginal.Kv = gradientItem.Kv;//梯度

                        //毛利率对象
                        YearOrValueKvMode yearOrValueKvModeAnnualDecline = new YearOrValueKvMode();
                        yearOrValueKvModeAnnualDecline.YearOrValueModes = new();
                        yearOrValueKvModeAnnualDecline.Kv = gradientItem.Kv;//梯度


                        KvMode kvMode = new KvMode();
                        kvMode.Kv = gradientItem.Kv;//梯度
                        //循环年份
                        foreach (ModelCountYear item in modelCountYears)
                        {
                            YearOrValueMode yearOrValueMode = new YearOrValueMode();
                            yearOrValueMode.Year = item.Year;
                            yearOrValueMode.Value = 0;
                            yearOrValueMode.UpDown = item.UpDown;
                            yearOrValueKvModeOriginal.YearOrValueModes.Add(yearOrValueMode);

                            YearOrValueMode yearOrValueModeAnnualDecline = new YearOrValueMode();
                            yearOrValueModeAnnualDecline.Year = item.Year;
                            yearOrValueModeAnnualDecline.UpDown = item.UpDown;
                            yearOrValueModeAnnualDecline.Value = 0;
                            yearOrValueKvModeAnnualDecline.YearOrValueModes.Add(yearOrValueModeAnnualDecline);

                            kvMode.Value = 0;
                        }
                        //将年份和价格对象和物料返利金额添加到列表中
                        yearOrValueModesOriginal.Add(yearOrValueKvModeOriginal);
                        yearOrValueModesAnnualDecline.Add(yearOrValueKvModeAnnualDecline);
                        kvModes.Add(kvMode);
                    }
                }
                //重新计算年降
                yearOrValueModesAnnualDecline = CalculateAnnualDecline(yearOrValueModesAnnualDecline, yearOrValueModesOriginal);
                return (yearOrValueModesOriginal, yearOrValueModesAnnualDecline, kvModes, Moq, CurrencyCode, MaterialControlStatus);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        //计算本位币
        [Obsolete("优化前")]
        internal async Task<List<YearOrValueKvMode>> CalculateStandardMoneyObsolete(ElectronicDto electronicDto)
        {
            List<YearOrValueKvMode> yearOrValueKvModes = new List<YearOrValueKvMode>();
            //梯度
            foreach (YearOrValueKvMode KV in electronicDto.SystemiginalCurrency)
            {
                YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                yearOrValueKvMode.YearOrValueModes = new List<YearOrValueMode>();
                yearOrValueKvMode.Kv = KV.Kv;
                //年份
                foreach (YearOrValueMode item in KV.YearOrValueModes)
                {
                    //获取汇率值                    
                    decimal exchangeRateModelValue = await ObtainExchangeRate(electronicDto.AuditFlowId, electronicDto.Currency, item.Year, electronicDto.SolutionId, KV.Kv);
                    YearOrValueMode yearOrValueMode = new YearOrValueMode();
                    yearOrValueMode.Year = item.Year;
                    yearOrValueMode.Value = (decimal)item.Value * exchangeRateModelValue * (decimal)electronicDto.AssemblyQuantity;

                    yearOrValueKvMode.YearOrValueModes.Add(yearOrValueMode);
                }
                yearOrValueKvModes.Add(yearOrValueKvMode);
            }
            return yearOrValueKvModes;
        }
        /// <summary>
        /// 根据物料编码获取 单价库中的数据
        /// </summary>
        /// <param name="MaterialCode">物料编码</param>
        /// <param name="materialsUseCount">项目物料的使用量</param>
        /// <param name="year">年份</param>
        /// <param name="kv">梯度</param>
        /// <returns></returns>
        internal async Task<UInitPriceForm> GetUInitPriceForm(string MaterialCode, List<YearOrValueKvMode> materialsUseCount, YearOrValueMode year, decimal kv, List<UInitPriceForm> uInitPrice)
        {
            //判断是否存在阶梯价
            if (uInitPrice.Count > 0)
            {
                //判断报价类型
                UInitPriceForm priceForm = uInitPrice.First();
                if (priceForm.QuotationType.Equals(QuotationType.Nothing))
                {
                    return priceForm;
                }
                else if (priceForm.QuotationType.Equals(QuotationType.Batch))//批量
                {
                    //查询共用物料库
                    List<SharedMaterialWarehouse> sharedMaterialWarehouses = await _sharedMaterialWarehouse.GetAllListAsync(p => p.MaterialCode.Equals(MaterialCode));

                    decimal sharedMaterialWarehousesModeCount = sharedMaterialWarehouses
                                       .SelectMany(sharedMaterial => JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(sharedMaterial.ModuleThroughputs)
                                       .Where(p => p.Year.Equals(year.Year))
                                       .Select(yearOrValueModeCanNull => sharedMaterial.AssemblyQuantity * (yearOrValueModeCanNull.Value ?? 0) + priceForm.AccumulatedProcurementQuantity))
                                       .Sum();
                    //获取区间的一条
                    UInitPriceForm uInitPriceForm = SelectNearest(uInitPrice, sharedMaterialWarehousesModeCount, item => item.BatchStartQuantity);
                    //如果没有符合的取价格最低的哪条
                    if (uInitPriceForm is null)
                    {
                        uInitPriceForm = uInitPrice.OrderByDescending(p => p.BulkUntaxedPrice).FirstOrDefault();
                    }
                    return uInitPriceForm;
                }
                else if (priceForm.QuotationType.Equals(QuotationType.Ladder))//阶梯
                {
                    decimal sharedMaterialWarehousesModeCount = materialsUseCount.Where(p => p.Kv.Equals(kv))
                                             .SelectMany(p => p.YearOrValueModes)
                                             .Where(p => p.Year.Equals(year.Year) && p.UpDown.Equals(year.UpDown))
                                             .Select(p => p.Value)
                                             .Sum();
                    //获取区间的一条
                    UInitPriceForm uInitPriceForm = SelectNearest(uInitPrice, sharedMaterialWarehousesModeCount, item => item.BatchStartQuantity);
                    //如果没有符合的取价格最低的哪条
                    if (uInitPriceForm is null)
                    {
                        uInitPriceForm = uInitPrice.OrderByDescending(p => p.BulkUntaxedPrice).FirstOrDefault();
                    }
                    return uInitPriceForm;
                }
            }
            else
            {
                return null;
            }
            return null;
        }
        /// <summary>
        /// 获取汇率
        /// </summary>
        /// <param name="auditFlowId">x</param>
        /// <param name="Currency">币种</param>
        /// <param name="Year">年份</param>
        /// <param name="solutionId">零件</param>
        /// <param name="Kv">梯度</param>
        /// <returns></returns>
        internal async Task<decimal> ObtainExchangeRate(long auditFlowId, string Currency, int Year, long solutionId, decimal Kv)
        {

            List<CustomerTargetPrice> customerTargetPrices = (from a in await _resourceSchemeTable.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.Id.Equals(solutionId))
                                                              join b in await _customerTargetPrice.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)) on a.Productld equals b.ProductId
                                                              select b).ToList();
            CustomerTargetPrice customer = customerTargetPrices.FirstOrDefault(p => p.Kv.Equals(Kv));

            ExchangeRate ExchangeRateKind = customer is null ? null : await _configExchangeRate.FirstOrDefaultAsync(p => p.Id.Equals(customer.Currency));
            if (customer is not null && customer.ExchangeRate is not 0M && ExchangeRateKind is not null && ExchangeRateKind.ExchangeRateKind == Currency)
            {
                return customer.ExchangeRate;
            }
            else
            {
                //获取汇率
                ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals(Currency));
                //获取汇率值
                List<YearOrValueMode> exchangeRateValues = JsonExchangeRateValue(exchangeRate?.ExchangeRateValue);
                //获取汇率值
                YearOrValueMode exchangeRateModel = exchangeRateValues.FirstOrDefault(p => p.Year.Equals(Year));
                if (exchangeRateModel is null)
                {
                    exchangeRateModel = exchangeRateValues.OrderByDescending(p => p.Year).FirstOrDefault();
                }
                return exchangeRateModel != null ? (decimal)(exchangeRateModel.Value) : 0M;
            }
        }
        //计算电子本位币
        internal async Task<List<YearOrValueKvMode>> CalculateStandardMoney(ElectronicDto electronicDto)
        {
            //计算本位币
            List<YearOrValueKvMode> yearOrValueKvModes = new List<YearOrValueKvMode>();
            foreach (var KV in electronicDto.SystemiginalCurrency)
            {
                YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                yearOrValueKvMode.Kv = KV.Kv;
                yearOrValueKvMode.YearOrValueModes = new List<YearOrValueMode>();
                foreach (var item in KV.YearOrValueModes)
                {
                    //获取汇率值                    
                    decimal exchangeRateModelValue = await ObtainExchangeRate(electronicDto.AuditFlowId, electronicDto.Currency, item.Year, electronicDto.SolutionId, KV.Kv);
                    //计算本位币
                    YearOrValueMode yearOrValueMode = new YearOrValueMode();
                    yearOrValueMode.Year = item.Year;
                    yearOrValueMode.Value = (decimal)item.Value * exchangeRateModelValue * (decimal)electronicDto.AssemblyQuantity;
                    yearOrValueMode.UpDown = item.UpDown;
                    yearOrValueKvMode.YearOrValueModes.Add(yearOrValueMode);
                }
                yearOrValueKvModes.Add(yearOrValueKvMode);
            }
            return yearOrValueKvModes;
        }
        //计算电子物料返利金额
        internal async Task<List<KvMode>> CalculateMaterialRebateAmount(ElectronicDto electronicDto, List<UInitPriceForm> uInitPrice)
        {
            //计算物料返利金额
            List<KvMode> yearOrValueKvModes = electronicDto.MaterialsUseCount.Select(KV =>
            {
                KvMode yearOrValueKvMode = new KvMode();
                yearOrValueKvMode.Kv = KV.Kv;
                yearOrValueKvMode.Value = KV.YearOrValueModes.Sum(item =>
                {
                    //获取汇率值                    
                    decimal exchangeRateModelValue = ObtainExchangeRate(electronicDto.AuditFlowId, electronicDto.Currency, item.Year, electronicDto.SolutionId, KV.Kv).Result;
                    //单价
                    decimal unitPrice = electronicDto.SystemiginalCurrency.FirstOrDefault(p =>
                    p.Kv.Equals(KV.Kv)).YearOrValueModes.FirstOrDefault(p => p.Year.Equals(item.Year))?.Value ?? 0.0m;
                    UInitPriceForm uInitPriceForm = GetUInitPriceForm(electronicDto.SapItemNum, electronicDto.MaterialsUseCount, item, KV.Kv, uInitPrice).Result;
                    //返点率
                    List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(uInitPriceForm.UInitPriceFormYearOrValueModes);
                    UInitPriceFormYearOrValueMode rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)
                   && p.Year.Equals(item.Year));
                    if (rebateRateYearOrValueMode is null)
                    {
                        rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                    }
                    return item.Value * unitPrice * exchangeRateModelValue * rebateRateYearOrValueMode.Value;
                });
                return yearOrValueKvMode;
            }).ToList();
            return yearOrValueKvModes;
        }
        //计算结构本位币
        internal async Task<List<YearOrValueKvMode>> CalculateStandardMoney(ConstructionModel construction)
        {
            //计算本位币
            List<YearOrValueKvMode> yearOrValueKvModes = new List<YearOrValueKvMode>();
            foreach (var KV in construction.SystemiginalCurrency)
            {
                YearOrValueKvMode yearOrValueKvMode = new YearOrValueKvMode();
                yearOrValueKvMode.Kv = KV.Kv;
                List<YearOrValueMode> yearOrValueModes = new List<YearOrValueMode>();
                foreach (var item in KV.YearOrValueModes)
                {
                    //获取汇率值
                    decimal exchangeRateModelValue = await ObtainExchangeRate(construction.AuditFlowId, construction.Currency, item.Year, construction.SolutionId, KV.Kv);
                    //计算本位币
                    YearOrValueMode yearOrValueMode = new YearOrValueMode();
                    yearOrValueMode.Year = item.Year;
                    yearOrValueMode.Value = (decimal)item.Value * exchangeRateModelValue * (decimal)construction.AssemblyQuantity;
                    yearOrValueMode.UpDown = item.UpDown;
                    yearOrValueModes.Add(yearOrValueMode);
                }
                yearOrValueKvMode.YearOrValueModes = yearOrValueModes;
                yearOrValueKvModes.Add(yearOrValueKvMode);
            }
            return yearOrValueKvModes;
        }
        //计算结构物料返利金额
        internal async Task<List<KvMode>> CalculateMaterialRebateAmount(ConstructionModel construction, List<UInitPriceForm> uInitPrice)
        {
            //计算物料返利金额
            List<KvMode> yearOrValueKvModes = construction.MaterialsUseCount.Select(KV =>
            {
                KvMode yearOrValueKvMode = new KvMode();
                yearOrValueKvMode.Kv = KV.Kv;
                yearOrValueKvMode.Value = KV.YearOrValueModes.Sum(item =>
                {
                    //获取汇率值
                    decimal exchangeRateModelValue = ObtainExchangeRate(construction.AuditFlowId, construction.Currency, item.Year, construction.SolutionId, KV.Kv).Result;
                    //单价
                    decimal unitPrice = construction.SystemiginalCurrency.FirstOrDefault(p =>
                    p.Kv.Equals(KV.Kv)).YearOrValueModes.FirstOrDefault(p => p.Year.Equals(item.Year))?.Value ?? 0.0m;
                    UInitPriceForm uInitPriceForm = GetUInitPriceForm(construction.SapItemNum, construction.MaterialsUseCount, item, KV.Kv, uInitPrice).Result;
                    //返点率
                    List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(uInitPriceForm.UInitPriceFormYearOrValueModes);
                    UInitPriceFormYearOrValueMode rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.FirstOrDefault(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)
                   && p.Year.Equals(item.Year));
                    if (rebateRateYearOrValueMode is null)
                    {
                        rebateRateYearOrValueMode = uInitPriceFormYearOrValueModes.Where(p => p.UInitPriceFormType.Equals(UInitPriceFormType.RebateRate)).OrderByDescending(p => p.Year).FirstOrDefault();
                    }
                    return item.Value * unitPrice * exchangeRateModelValue * rebateRateYearOrValueMode.Value;
                });
                return yearOrValueKvMode;
            }).ToList();
            return yearOrValueKvModes;
        }
        /// <summary>
        /// 电子料单价录入
        /// </summary>
        internal async Task ElectronicMaterialEntering(SubmitElectronicDto submitElectronicDto)
        {
            foreach (ElectronicDto item in submitElectronicDto.ElectronicDtoList)
            {
                try
                {
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.SolutionId.Equals(item.SolutionId) && p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.ElectronicId.Equals(item.ElectronicId));
                    if (enteringElectronic is null)
                    {
                        //添加
                        enteringElectronic = new();
                        enteringElectronic = ObjectMapper.Map<EnteringElectronic>(item);
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        enteringElectronic.IsEntering = true;//确认录入           
                        await _configEnteringElectronic.InsertAsync(enteringElectronic);
                    }
                    else
                    {
                        enteringElectronic.MOQ = item.MOQ;//MOQ
                        enteringElectronic.ElectronicId = item.ElectronicId;//电子BOM表单的Id
                        enteringElectronic.SolutionId = item.SolutionId; //零件的id
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.RebateMoney = JsonConvert.SerializeObject(item.RebateMoney);//物料可返利金额
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id                       
                        enteringElectronic.IsEntering = true;//确认录入          
                        enteringElectronic.Currency = item.Currency;//币种
                        enteringElectronic.MaterialControlStatus = item.MaterialControlStatus;//物料管制状态
                        enteringElectronic.Remark = item.Remark;//备注
                        enteringElectronic.IsSystemiginal = item.IsSystemiginal;//系统单价是否从单价库中带出
                        enteringElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//物料使用量
                        enteringElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//年将率         
                        enteringElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                        enteringElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(item.SystemiginalCurrency);//系统单价(原币)
                        await _configEnteringElectronic.UpdateAsync(enteringElectronic);
                    }
                    //删除电子差异表中的数据                 
                    await _configElecBomDifferent.DeleteAsync(p => p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(item.ElectronicId));
                }
                catch (Exception e)
                {
                    throw new FriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 电子单价录入 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        internal async Task SubmitElectronicMaterialEntering(SubmitElectronicDto submitElectronicDto)
        {
            try
            {
                //循环 资源部 填写的 电子BOM 表达那实体类
                foreach (ElectronicDto electronic in submitElectronicDto.ElectronicDtoList)
                {
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.SolutionId.Equals(electronic.SolutionId) && p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.ElectronicId.Equals(electronic.ElectronicId));
                    if (enteringElectronic is null)
                    {
                        //添加
                        enteringElectronic = new();
                        enteringElectronic = ObjectMapper.Map<EnteringElectronic>(electronic);
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        enteringElectronic.IsSubmit = true;//确认提交           
                        await _configEnteringElectronic.InsertAsync(enteringElectronic);
                    }
                    else
                    {
                        enteringElectronic.MOQ = electronic.MOQ;//MOQ
                        enteringElectronic.ElectronicId = electronic.ElectronicId;//电子BOM表单的Id
                        enteringElectronic.SolutionId = electronic.SolutionId; //零件的id
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.RebateMoney = JsonConvert.SerializeObject(electronic.RebateMoney);//物料可返利金额
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id                       
                        enteringElectronic.IsSubmit = true;//确认提交          
                        enteringElectronic.Currency = electronic.Currency;//币种
                        enteringElectronic.MaterialControlStatus = electronic.MaterialControlStatus;//物料管制状态
                        enteringElectronic.Remark = electronic.Remark;//备注
                        enteringElectronic.IsSystemiginal = electronic.IsSystemiginal;//系统单价是否从单价库中带出
                        enteringElectronic.MaterialsUseCount = JsonConvert.SerializeObject(electronic.MaterialsUseCount);//物料使用量
                        enteringElectronic.InTheRate = JsonConvert.SerializeObject(electronic.InTheRate);//年将率         
                        enteringElectronic.StandardMoney = JsonConvert.SerializeObject(electronic.StandardMoney);//本位币
                        enteringElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(electronic.SystemiginalCurrency);//系统单价(原币)
                        await _configEnteringElectronic.UpdateAsync(enteringElectronic);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 电子单价录入计算
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        internal async Task<ElectronicDto> ElectronicMaterialUnitPriceInputCalculation(ElectronicDto electronicDto)
        {
            //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
            List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.MaterialCode.Equals(electronicDto.SapItemNum) && p.FreezeOrNot.Equals(FreezeOrNot.Thaw) && p.EffectiveDate < DateTime.Now && p.ExpirationDate > DateTime.Now);
            //通过优先级筛选
            List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.SupplierPriority.Equals(SupplierPriority.Core)).ToList();
            List<UInitPriceForm> uInitPrice = uInitPriceFormsPriority.Count > 0 ? uInitPriceFormsPriority : uInitPriceForms;
            electronicDto.StandardMoney = await CalculateStandardMoney(electronicDto);//本位币
            //如果单价库没有取到值直接跳过
            if (uInitPrice.Count > 0) electronicDto.RebateMoney = await CalculateMaterialRebateAmount(electronicDto, uInitPrice);//物料返利金额
            foreach (YearOrValueKvMode inTheRate in electronicDto.InTheRate)
            {
                List<YearOrValueMode> yearOrValueKvModes = electronicDto.SystemiginalCurrency.FirstOrDefault(p => p.Kv.Equals(inTheRate.Kv)).YearOrValueModes;
                //第一年的年降不需要,默认为0
                inTheRate.YearOrValueModes[0].Value = 0;
                for (int i = 1; i < inTheRate.YearOrValueModes.Count; i++)
                {
                    if (yearOrValueKvModes[i - 1].Value != 0)
                    {
                        inTheRate.YearOrValueModes[i].Value = (1 - yearOrValueKvModes[i].Value / yearOrValueKvModes[i - 1].Value) * 100;
                    }
                }
            }
            return electronicDto;
        }
        /// <summary>
        /// 结构单价录入计算
        /// </summary>
        /// <param name="structural"></param>
        /// <returns></returns>
        internal async Task<ConstructionModel> CalculationOfStructuralMaterials(ConstructionModel structural)
        {
            //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
            List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.MaterialCode.Equals(structural.SapItemNum) && p.FreezeOrNot.Equals(FreezeOrNot.Thaw) && p.EffectiveDate < DateTime.Now && p.ExpirationDate > DateTime.Now);
            //通过优先级筛选
            List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.SupplierPriority.Equals(SupplierPriority.Core)).ToList();
            List<UInitPriceForm> uInitPrice = uInitPriceFormsPriority.Count > 0 ? uInitPriceFormsPriority : uInitPriceForms;
            structural.StandardMoney = await CalculateStandardMoney(structural);//本位币   
            //如果单价库没有取到值直接跳过
            if (uInitPrice.Count > 0) structural.RebateMoney = await CalculateMaterialRebateAmount(structural, uInitPrice);//物料返利金额
            foreach (YearOrValueKvMode inTheRate in structural.InTheRate)
            {
                List<YearOrValueMode> yearOrValueKvModes = structural.SystemiginalCurrency.FirstOrDefault(p => p.Kv.Equals(inTheRate.Kv)).YearOrValueModes;
                //第一年的年降不需要,默认为0
                inTheRate.YearOrValueModes[0].Value = 0;
                for (int i = 1; i < inTheRate.YearOrValueModes.Count; i++)
                {
                    if (yearOrValueKvModes[i - 1].Value != 0)
                    {
                        inTheRate.YearOrValueModes[i].Value = (1 - yearOrValueKvModes[i].Value / yearOrValueKvModes[i - 1].Value) * 100;
                    }
                }
            }
            return structural;
        }
        /// <summary>
        /// 计算年降
        /// </summary>
        /// <param name="InTheRate">项目物料的年降率</param>
        /// <param name="systemiginalCurrency">系统单价(原币)</param>
        /// <returns></returns>
        internal List<YearOrValueKvMode> CalculateAnnualDecline(List<YearOrValueKvMode> InTheRate, List<YearOrValueKvMode> systemiginalCurrency)
        {
            foreach (YearOrValueKvMode inTheRate in InTheRate)
            {
                List<YearOrValueMode> yearOrValueKvModes = systemiginalCurrency.FirstOrDefault(p => p.Kv.Equals(inTheRate.Kv)).YearOrValueModes;
                //第一年的年降不需要,默认为0
                inTheRate.YearOrValueModes[0].Value = 0;
                for (int i = 1; i < inTheRate.YearOrValueModes.Count; i++)
                {
                    if (yearOrValueKvModes[i - 1].Value != 0)
                    {
                        inTheRate.YearOrValueModes[i].Value = (1 - yearOrValueKvModes[i].Value / yearOrValueKvModes[i - 1].Value) * 100;
                    }
                }
            }
            return InTheRate;
        }
        /// <summary>
        /// 结构件单价录入提交 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        internal async Task StructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEntering)
        {
            foreach (StructuralMaterialModel item in structuralMemberEntering.StructuralMaterialEntering)
            {
                try
                {
                    StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.SolutionId.Equals(item.SolutionId) && p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.StructureId.Equals(item.StructureId));
                    if (structureElectronic is null)
                    {
                        //添加
                        structureElectronic = new();
                        structureElectronic = ObjectMapper.Map<StructureElectronic>(item);
                        structureElectronic.AuditFlowId = structuralMemberEntering.AuditFlowId;//流程的id
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.IsEntering = true; //确认录入
                        await _configStructureElectronic.InsertAsync(structureElectronic);
                    }
                    else
                    {
                        structureElectronic.RebateMoney = JsonConvert.SerializeObject(item.RebateMoney);//物料返利金额
                        structureElectronic.MOQ = item.MOQ;//MOQ
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.Currency = item.Currency;//币种      
                        structureElectronic.IsEntering = true; //确认录入
                        structureElectronic.MaterialControlStatus = item.MaterialControlStatus;//ECCN
                        structureElectronic.Remark = item.Remark; //备注                  
                        structureElectronic.IsSystemiginal = item.IsSystemiginal;// 系统单价是否从单价库中带出
                        structureElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                        structureElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//项目物料的使用量
                        structureElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(item.SystemiginalCurrency);//系统单价（原币）
                        structureElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//项目物料的年降率
                        await _configStructureElectronic.UpdateAsync(structureElectronic);
                    }
                    //删除结构差异表中的数据  
                    await _configStructBomDifferent.DeleteAsync(p => p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(item.StructureId));
                }
                catch (Exception e)
                {
                    throw new FriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 结构件单价提交 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        internal async Task SubmitStructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEntering)
        {
            try
            {
                foreach (StructuralMaterialModel item in structuralMemberEntering.StructuralMaterialEntering)
                {
                    StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.SolutionId.Equals(item.SolutionId) && p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.StructureId.Equals(item.StructureId));
                    if (structureElectronic is null)
                    {
                        //添加
                        structureElectronic = new();
                        structureElectronic = ObjectMapper.Map<StructureElectronic>(item);
                        structureElectronic.AuditFlowId = structuralMemberEntering.AuditFlowId;//流程的id
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.IsSubmit = true; //确认提交
                        await _configStructureElectronic.InsertAsync(structureElectronic);
                    }
                    else
                    {
                        structureElectronic.RebateMoney = JsonConvert.SerializeObject(item.RebateMoney);//物料返利金额
                        structureElectronic.MOQ = item.MOQ;//MOQ
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.Currency = item.Currency;//币种      
                        structureElectronic.IsSubmit = true; //确认提交
                        structureElectronic.MaterialControlStatus = item.MaterialControlStatus;//ECCN
                        structureElectronic.Remark = item.Remark; //备注                  
                        structureElectronic.IsSystemiginal = item.IsSystemiginal;// 系统单价是否从单价库中带出
                        structureElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                        structureElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//项目物料的使用量
                        structureElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(item.SystemiginalCurrency);//系统单价（原币）
                        structureElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//项目物料的年降率
                        await _configStructureElectronic.UpdateAsync(structureElectronic);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 将json 转成  List YearOrValueMode>
        /// </summary>
        internal static List<YearOrValueMode> JsonExchangeRateValue(string price)
        {
            if (string.IsNullOrEmpty(price)) return new List<YearOrValueMode>();
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
        /// <summary>
        /// 在集合中通过一个参数的范围取最接近的一条 比如 number值为20   集合中有 10 15 25  则取15
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="number">对比的数字</param>
        /// <param name="selector">根据那个属性对比</param>
        /// <returns></returns>
        internal T SelectNearest<T>(IEnumerable<T> source, decimal number, Func<T, decimal> selector)
        {
            // 筛选出小于等于目标值的元素
            List<T> filteredList = source.Where(x => selector(x) <= number).ToList();
            var closestValue = default(T);
            if (!filteredList.Any())
            {
                //有一种可能 值都比  他大  ,那就取最小的那个
                if (source.Any()) closestValue = source.MinBy(x => selector(x));
            }
            else
            {
                // 获取最接近目标值的元素
                closestValue = filteredList.MaxBy(x => selector(x));
            }
            return closestValue;
        }

        /// <summary>
        /// 检查数组对象中某个属性值是否全部为 null 或 0。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="source">数组对象。</param>
        /// <param name="selector">属性选择器。</param>       
        internal bool IsAllNullOrZero<T>(IEnumerable<T> source, Func<T, object> selector)
        {
            foreach (T item in source)
            {
                object value = selector(item);
                if (value != null)
                {
                    Type type = value.GetType();
                    if (type.IsValueType)
                    {
                        if (type == typeof(decimal) && (decimal)value == 0m)
                        {
                            return false;
                        }
                        else if (type == typeof(double) && (double)value == 0d)
                        {
                            return false;
                        }
                        else if (type == typeof(float) && (float)value == 0f)
                        {
                            return false;
                        }
                        else if (value.Equals(Activator.CreateInstance(type))) // 如果 value 是值类型且等于默认值
                        {
                            return false;
                        }
                    }
                    else // 如果 value 是引用类型
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
