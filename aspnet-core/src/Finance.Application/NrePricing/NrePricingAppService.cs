using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.EngineeringDepartment;
//using Finance.EntityFrameworkCore.Seed.Host;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Method;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using test;
using static Finance.Ext.FriendlyRequiredAttribute;
using File = System.IO.File;

namespace Finance.NerPricing
{
    /// <summary>
    /// Nre核价api
    /// </summary>
    public class NrePricingAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        private readonly ElectronicStructuralMethod _resourceElectronicStructuralMethod;
        /// <summary>
        /// Nre 项目管理部 手板件实体类
        /// </summary>
        private readonly IRepository<HandPieceCost, long> _resourceHandPieceCost;
        /// <summary>
        /// Nre 项目管理部 其他费用实体类
        /// </summary>
        private readonly IRepository<RestsCost, long> _resourceRestsCost;
        /// <summary>
        /// Nre 项目管理部 差旅费实体类
        /// </summary>
        private readonly IRepository<TravelExpense, long> _resourceTravelExpense;
        /// <summary> 
        /// Nre 资源部 模具清单实体类
        /// </summary>
        private readonly IRepository<MouldInventory, long> _resourceMouldInventory;
        /// <summary> 
        /// Nre 产品部-EMC电子工程师 实验费 实体类
        /// </summary>
        private readonly IRepository<LaboratoryFee, long> _resourceLaboratoryFee;
        /// <summary>
        /// Nre 方法 类
        /// </summary>
        private readonly NrePricingMethod _resourceNrePricingMethod;
        /// <summary>
        /// Nre 品保录入 环境实验费 实体类
        /// </summary>
        private readonly IRepository<EnvironmentalExperimentFee, long> _resourceEnvironmentalExperimentFee;
        /// <summary>
        ///Nre 品保录入  项目制程QC量检具表  实体类
        /// </summary>
        private readonly IRepository<QADepartmentQC, long> _resourceQADepartmentQC;
        /// <summary>
        ///Nre 营销部录入  总价表  实体类
        /// </summary>
        private readonly IRepository<InitialResourcesManagement, long> _resourceInitialResourcesManagement;
        /// <summary>
        /// 设备部分表
        /// </summary>
        private readonly IRepository<EquipmentInfo, long> _resourceEquipmentInfo;
        /// <summary>
        /// 追溯部分表(硬件及软件开发费用)
        /// </summary>
        private readonly IRepository<TraceInfo, long> _resourceTraceInfo;
        /// <summary>
        /// 工时工序静态字段表
        /// </summary>
        private readonly IRepository<WorkingHoursInfo, long> _resourceWorkingHoursInfo;
        /// <summary>
        /// 字典明细表
        /// </summary>
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        /// <summary>
        /// 核价需求录入表
        /// </summary>
        private readonly IRepository<PriceEvaluation, long> _resourcePriceEvaluation;
        ///// <summary>
        ///// 流程流转服务
        ///// </summary>
        //private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        ///  Nre  方案是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _resourceNreIsSubmit;
        /// <summary>
        /// 模组数量年份
        /// </summary>
        private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private readonly IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private readonly IRepository<StructBomDifferent, long> _configStructBomDifferent;
        private readonly IRepository<User, long> _userRepository;
        /// <summary>
        /// 
        /// </summary>
        private readonly IRepository<ProcessHoursEnter, long> _processHoursEnter;
        /// <summary>
        /// 二开新增治具部分表
        /// </summary>
        private readonly IRepository<ProcessHoursEnterFixture, long> _processHoursEnterFixture;
        /// <summary>
        /// 二开新增设备部分表
        /// </summary>
        private readonly IRepository<ProcessHoursEnterDevice, long> _processHoursEnterDevice;
        /// <summary>
        /// 二开新增 获取  线体数量和共线分摊率
        /// </summary>
        private readonly IRepository<ProcessHoursEnterLine, long> _processHoursEnterLine;
        /// <summary>
        /// 二开新增硬件部分表
        /// </summary>
        private readonly IRepository<ProcessHoursEnterFrock, long> _processHoursEnterFrock;
        /// <summary>
        /// 二开新增 基础库--实验库环境
        /// </summary>
        private readonly IRepository<Foundationreliable, long> _foundationreliable;
        /// <summary>
        /// Nre 项目管理部 手板件 修改项实体类
        /// </summary>
        private readonly IRepository<HandPieceCostModify, long> _handPieceCostModify;
        /// <summary>
        /// Nre 资源部 模具清单 修改项实体类
        /// </summary>
        private readonly IRepository<MouldInventoryModify, long> _mouldInventoryModify;
        /// <summary>
        /// 工装费用 修改项 实体类
        /// </summary>
        private readonly IRepository<ToolingCostsModify, long> _toolingCostsModify;
        /// <summary>
        /// 治具费用修改项实体类
        /// </summary>
        private readonly IRepository<FixtureCostsModify, long> _fixtureCostsModify;
        /// <summary>
        /// 检具费用  修改项实体类
        /// </summary>
        private readonly IRepository<InspectionToolCostModify, long> _inspectionToolCostModify;
        /// <summary>
        /// 生产设备费用  修改项实体类
        /// </summary>
        private readonly IRepository<ProductionEquipmentCostsModify, long> _productionEquipmentCostsModify;
        /// <summary>
        /// 实验费用 修改项 实体类
        /// </summary>
        private readonly IRepository<ExperimentalExpensesModify, long> _experimentalExpensesModify;
        /// <summary>
        /// 测试软件费用 修改项 实体类
        /// </summary>
        private readonly IRepository<TestingSoftwareCostsModify, long> _testingSoftwareCostsModify;
        /// <summary>
        /// 差旅费 修改项 实体类
        /// </summary>
        private readonly IRepository<TravelExpenseModify, long> _travelExpenseModify;
        /// <summary>
        /// 其他费 修改项 实体类
        /// </summary>
        private readonly IRepository<RestsCostModify, long> _restsCostModify;

        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        /// <summary>
        /// 环境实验费录入 服务
        /// </summary>
        private readonly FoundationreliableAppService _foundationreliableAppService;

        /// <summary>
        /// EMC实验费录入 服务
        /// </summary>
        private readonly FoundationEmcAppService _foundationEmcAppService;

        public NrePricingAppService(IRepository<ModelCount, long> resourceModelCount, ElectronicStructuralMethod resourceElectronicStructuralMethod, IRepository<HandPieceCost, long> resourceHandPieceCost, IRepository<RestsCost, long> resourceRestsCost, IRepository<TravelExpense, long> resourceTravelExpense, IRepository<MouldInventory, long> resourceMouldInventory, IRepository<LaboratoryFee, long> resourceLaboratoryFee, NrePricingMethod resourceNrePricingMethod, IRepository<EnvironmentalExperimentFee, long> resourceEnvironmentalExperimentFee, IRepository<QADepartmentQC, long> resourceQADepartmentQC, IRepository<InitialResourcesManagement, long> resourceInitialResourcesManagement, IRepository<EquipmentInfo, long> resourceEquipmentInfo, IRepository<TraceInfo, long> resourceTraceInfo, IRepository<WorkingHoursInfo, long> resourceWorkingHoursInfo, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<PriceEvaluation, long> resourcePriceEvaluation, IRepository<NreIsSubmit, long> resourceNreIsSubmit, IRepository<ModelCountYear, long> resourceModelCountYear, IRepository<ExchangeRate, long> configExchangeRate, IRepository<StructBomDifferent, long> configStructBomDifferent, IRepository<User, long> userRepository, IRepository<ProcessHoursEnter, long> processHoursEnter, IRepository<ProcessHoursEnterFixture, long> processHoursEnterFixture, IRepository<ProcessHoursEnterDevice, long> processHoursEnterDevice, IRepository<ProcessHoursEnterLine, long> processHoursEnterLine, IRepository<ProcessHoursEnterFrock, long> processHoursEnterFrock, IRepository<Foundationreliable, long> foundationreliable, IRepository<HandPieceCostModify, long> handPieceCostModify, IRepository<MouldInventoryModify, long> mouldInventoryModify, IRepository<ToolingCostsModify, long> toolingCostsModify, IRepository<FixtureCostsModify, long> fixtureCostsModify, IRepository<InspectionToolCostModify, long> inspectionToolCostModify, IRepository<ProductionEquipmentCostsModify, long> productionEquipmentCostsModify, IRepository<ExperimentalExpensesModify, long> experimentalExpensesModify, IRepository<TestingSoftwareCostsModify, long> testingSoftwareCostsModify, IRepository<TravelExpenseModify, long> travelExpenseModify, IRepository<RestsCostModify, long> restsCostModify, WorkflowInstanceAppService workflowInstanceAppService, FoundationreliableAppService foundationreliableAppService, FoundationEmcAppService foundationEmcAppService)
        {
            _resourceModelCount = resourceModelCount;
            _resourceElectronicStructuralMethod = resourceElectronicStructuralMethod;
            _resourceHandPieceCost = resourceHandPieceCost;
            _resourceRestsCost = resourceRestsCost;
            _resourceTravelExpense = resourceTravelExpense;
            _resourceMouldInventory = resourceMouldInventory;
            _resourceLaboratoryFee = resourceLaboratoryFee;
            _resourceNrePricingMethod = resourceNrePricingMethod;
            _resourceEnvironmentalExperimentFee = resourceEnvironmentalExperimentFee;
            _resourceQADepartmentQC = resourceQADepartmentQC;
            _resourceInitialResourcesManagement = resourceInitialResourcesManagement;
            _resourceEquipmentInfo = resourceEquipmentInfo;
            _resourceTraceInfo = resourceTraceInfo;
            _resourceWorkingHoursInfo = resourceWorkingHoursInfo;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _resourcePriceEvaluation = resourcePriceEvaluation;
            _resourceNreIsSubmit = resourceNreIsSubmit;
            _resourceModelCountYear = resourceModelCountYear;
            _configExchangeRate = configExchangeRate;
            _configStructBomDifferent = configStructBomDifferent;
            _userRepository = userRepository;
            _processHoursEnter = processHoursEnter;
            _processHoursEnterFixture = processHoursEnterFixture;
            _processHoursEnterDevice = processHoursEnterDevice;
            _processHoursEnterLine = processHoursEnterLine;
            _processHoursEnterFrock = processHoursEnterFrock;
            _foundationreliable = foundationreliable;
            _handPieceCostModify = handPieceCostModify;
            _mouldInventoryModify = mouldInventoryModify;
            _toolingCostsModify = toolingCostsModify;
            _fixtureCostsModify = fixtureCostsModify;
            _inspectionToolCostModify = inspectionToolCostModify;
            _productionEquipmentCostsModify = productionEquipmentCostsModify;
            _experimentalExpensesModify = experimentalExpensesModify;
            _testingSoftwareCostsModify = testingSoftwareCostsModify;
            _travelExpenseModify = travelExpenseModify;
            _restsCostModify = restsCostModify;
            _workflowInstanceAppService = workflowInstanceAppService;
            _foundationreliableAppService = foundationreliableAppService;
            _foundationEmcAppService = foundationEmcAppService;
        }



        /// <summary>
        /// 获取 方案
        /// </summary>
        /// <param name="auditFlowId">流程id(long类型)</param>
        /// <returns></returns>
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId)
        {
            //总共的方案
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            return partList;
        }
        /// <summary>
        /// 根据筛选条件获取方案列表
        /// </summary>
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId, Func<Solution, bool> filter)
        {
            //总共的方案
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId, filter);
            return partList;
        }
        /// <summary>
        /// 项目管理部录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProjectManagement(ProjectManagementDto price)
        {
            //录入手板件费用
            foreach (ProjectManagementModel item in price.projectManagements)
            {
                //判断 该方案 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                if (nreIsSubmits.Count is not 0)
                {
                    throw new FriendlyException(item.ProductId + ":该方案id已经提交过了");
                }
                try
                {
                    List<HandPieceCost> handPieceCosts = ObjectMapper.Map<List<HandPieceCost>>(item.HandPieceCost);
                    //删除原数据
                    await _resourceHandPieceCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (HandPieceCost handPieceCost in handPieceCosts)
                    {
                        handPieceCost.AuditFlowId = price.AuditFlowId;
                        handPieceCost.ProductId = item.ProductId;
                        await _resourceHandPieceCost.InsertOrUpdateAsync(handPieceCost); //录入手板费用
                    }
                    List<RestsCost> restsCosts = ObjectMapper.Map<List<RestsCost>>(item.RestsCost);
                    //删除原数据
                    await _resourceRestsCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (RestsCost restsCost in restsCosts)
                    {
                        restsCost.AuditFlowId = price.AuditFlowId;
                        restsCost.ProductId = item.ProductId;
                        await _resourceRestsCost.InsertOrUpdateAsync(restsCost);//录入其他费用
                    }
                    List<TravelExpense> travelExpenses = ObjectMapper.Map<List<TravelExpense>>(item.TravelExpense);
                    //删除原数据
                    await _resourceTravelExpense.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (TravelExpense travel in travelExpenses)
                    {
                        travel.AuditFlowId = price.AuditFlowId;
                        travel.ProductId = item.ProductId;
                        await _resourceTravelExpense.InsertOrUpdateAsync(travel);//录入差旅费
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = item.ProductId, EnumSole = NreIsSubmitDto.ProjectManagement.ToString() });
                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 项目管理部录入(单个方案)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProjectManagementSingle(ProjectManagementDtoSingle price)
        {
            ProjectManagementModel projectManagementModel = new();
            projectManagementModel = price.projectManagement;
            //判断 该方案 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(projectManagementModel.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该方案已经提交过了");
            }
            try
            {

                List<HandPieceCost> handPieceCosts = ObjectMapper.Map<List<HandPieceCost>>(projectManagementModel.HandPieceCost);
                //删除原数据
                await _resourceHandPieceCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(projectManagementModel.SolutionId));
                foreach (HandPieceCost handPieceCost in handPieceCosts)
                {
                    handPieceCost.AuditFlowId = price.AuditFlowId;
                    handPieceCost.SolutionId = projectManagementModel.SolutionId;
                    await _resourceHandPieceCost.InsertOrUpdateAsync(handPieceCost); //录入手板费用
                }
                List<RestsCost> restsCosts = ObjectMapper.Map<List<RestsCost>>(projectManagementModel.RestsCost);
                //删除原数据
                await _resourceRestsCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(projectManagementModel.SolutionId));
                foreach (RestsCost restsCost in restsCosts)
                {
                    restsCost.AuditFlowId = price.AuditFlowId;
                    restsCost.SolutionId = projectManagementModel.SolutionId;
                    await _resourceRestsCost.InsertOrUpdateAsync(restsCost);//录入其他费用
                }
                List<TravelExpense> travelExpenses = ObjectMapper.Map<List<TravelExpense>>(projectManagementModel.TravelExpense);
                //删除原数据
                await _resourceTravelExpense.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(projectManagementModel.SolutionId));
                foreach (TravelExpense travel in travelExpenses)
                {
                    travel.AuditFlowId = price.AuditFlowId;
                    travel.SolutionId = projectManagementModel.SolutionId;
                    await _resourceTravelExpense.InsertOrUpdateAsync(travel);//录入差旅费
                }
                #region 录入完成之后
                if (price.Opinion == FinanceConsts.Done)
                {
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = projectManagementModel.SolutionId, EnumSole = NreIsSubmitDto.ProjectManagement.ToString() });
                }
                #endregion
                if (await this.GetProjectManagement(price.AuditFlowId))
                {
                    //嵌入工作流
                    await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                    {
                        NodeInstanceId = price.NodeInstanceId,
                        FinanceDictionaryDetailId = price.Opinion,
                        Comment = price.Comment,
                    });
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 项目管理部录入  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetProjectManagement(long Id)
        {
            //获取 总共的方案
            List<SolutionModel> partModels = await TotalSolution(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的方案
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// 项目管理部录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetProjectManagementConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
            foreach (NreIsSubmit item in nreAre)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre项目管理部 获取版本录入过的值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<ProjectManagementModel>> GetReturnProjectManagement(long Id)
        {
            try
            {
                List<ProjectManagementModel> projectManagementModels = new();
                //所有的方案
                List<SolutionModel> partModels = new();
                partModels = await TotalSolution(Id);
                foreach (SolutionModel partModel in partModels)
                {
                    ProjectManagementModel projectManagementModel = new();
                    projectManagementModel.SolutionId = partModel.SolutionId;

                    List<HandPieceCostModel> handPieceCostModels = new();
                    List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(partModel.SolutionId));
                    if (handPieceCosts is not null) handPieceCostModels = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                    projectManagementModel.HandPieceCost = new();
                    projectManagementModel.HandPieceCost = handPieceCostModels;//手板费用

                    List<RestsCostModel> restsCostModels = new();
                    List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(partModel.SolutionId));
                    if (restsCosts is not null) restsCostModels = ObjectMapper.Map<List<RestsCostModel>>(restsCosts);
                    projectManagementModel.RestsCost = new();
                    projectManagementModel.RestsCost = restsCostModels;//其他费用

                    List<TravelExpenseModel> travelExpenseModels = new();
                    List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(partModel.SolutionId));
                    if (travelExpenses is not null) travelExpenseModels = ObjectMapper.Map<List<TravelExpenseModel>>(travelExpenses);
                    projectManagementModel.TravelExpense = new();
                    projectManagementModel.TravelExpense = travelExpenseModels;//差旅费
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.SolutionId.Equals(partModel.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                    projectManagementModel.IsSubmit = Count > 0;
                    projectManagementModels.Add(projectManagementModel);
                }
                return projectManagementModels;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre项目管理部 获取版本录入过的值(单个方案)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<ProjectManagementModel> GetReturnProjectManagementSingle(long auditFlowId, long solutionId)
        {
            try
            {
                List<ProjectManagementModel> projectManagementModels = new();
                //所有的方案
                List<SolutionModel> partModels = new();
                partModels = await TotalSolution(auditFlowId);
                partModels = partModels.Where(p => p.SolutionId.Equals(solutionId)).ToList();
                foreach (SolutionModel partModel in partModels)
                {
                    ProjectManagementModel projectManagementModel = new();
                    projectManagementModel.SolutionId = partModel.SolutionId;

                    List<HandPieceCostModel> handPieceCostModels = new();
                    List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId));
                    if (handPieceCosts is not null) handPieceCostModels = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                    projectManagementModel.HandPieceCost = new();
                    projectManagementModel.HandPieceCost = handPieceCostModels;//手板费用

                    List<RestsCostModel> restsCostModels = new();
                    List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId));
                    if (restsCosts is not null) restsCostModels = ObjectMapper.Map<List<RestsCostModel>>(restsCosts);
                    projectManagementModel.RestsCost = new();
                    projectManagementModel.RestsCost = restsCostModels;//其他费用

                    List<TravelExpenseModel> travelExpenseModels = new();
                    List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId));
                    if (travelExpenses is not null) travelExpenseModels = ObjectMapper.Map<List<TravelExpenseModel>>(travelExpenses);
                    projectManagementModel.TravelExpense = new();
                    projectManagementModel.TravelExpense = travelExpenseModels;//差旅费
                    //判断 该方案 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                    projectManagementModel.IsSubmit = Count > 0;
                    projectManagementModels.Add(projectManagementModel);
                }
                return projectManagementModels.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 资源部模具费初始值数量
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>     
        public async Task<long> GetInitialResourcesManagementCount(long auditFlowId)
        {
            long count = 0;
            List<SolutionModel> partModels = await TotalSolution(auditFlowId);// 获总方案       

            //循环每一个方案
            foreach (SolutionModel part in partModels)
            {
                MouldInventoryPartModel mould = await GetInitialResourcesManagementSingle(auditFlowId, part.SolutionId);
                count += mould.MouldInventoryModels.Count;
            }
            return count;
        }
        /// <summary>
        /// 资源部模具费初始值(单个方案)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<MouldInventoryPartModel> GetInitialResourcesManagementSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            List<SolutionModel> partModels = await TotalSolution(auditFlowId, item => item.Id.Equals(solutionId));// 获取指定的方案         
            List<MouldInventoryPartModel> mouldInventoryPartModels = new();// Nre核价 带 方案 id 的模具清单 模型  
            //循环每一个方案
            foreach (SolutionModel part in partModels)
            {
                MouldInventoryPartModel mouldInventoryPartModel = new();//  Nre核价 模组清单模型
                mouldInventoryPartModel.SolutionId = part.SolutionId;//方案的 Id             
                //获取初始值的时候如果数据库里有,直接拿数据库中的
                //删除的结构BOMid
                List<long> longs = new();
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(part.SolutionId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _resourceMouldInventory.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(part.SolutionId) && p.StructuralId.Equals(structBom.StructureId));
                            longs.Add(structBom.StructureId);
                        }
                    }
                }
                //获取初始值的时候如果数据库里有,直接拿数据库中的
                List<MouldInventory> mouldInventory = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(part.SolutionId));
                List<MouldInventory> mouldInventoryEquals = (from a in mouldInventory
                                                             join b in longs on a.StructuralId equals b
                                                             select a).ToList();//相等的

                mouldInventory = mouldInventory.Except(mouldInventoryEquals).Distinct().ToList();//差集
                mouldInventoryPartModel.MouldInventoryModels = await _resourceNrePricingMethod.MouldInventoryModels(auditFlowId, part.SolutionId);//传流程id和方案号的id
                foreach (MouldInventoryModel item in mouldInventoryPartModel.MouldInventoryModels)
                {
                    MouldInventory mouldInventory1 = mouldInventory.FirstOrDefault(p => p.StructuralId.Equals(item.StructuralId));
                    if (mouldInventory1 is not null)
                    {
                        item.Id = mouldInventory1.Id;
                        item.MoldCavityCount = mouldInventory1.MoldCavityCount;//摸穴数
                        item.ModelNumber = mouldInventory1.ModelNumber;//摸次数
                        item.Count = mouldInventory1.Count;//数量
                        item.UnitPrice = mouldInventory1.UnitPrice;//单价
                        item.Cost = mouldInventory1.Cost;//费用
                        item.PeopleId = mouldInventory1.PeopleId;//提交人id
                        item.IsSubmit = mouldInventory1.IsSubmit;//是否提交
                        item.Remark = mouldInventory1.Remark;//备注
                        User user = await _userRepository.FirstOrDefaultAsync(p => p.Id == item.PeopleId);
                        if (user is not null) item.PeopleName = user.Name;//提交人名称
                    }
                }
                mouldInventoryPartModels.Add(mouldInventoryPartModel);
            }
            return mouldInventoryPartModels.FirstOrDefault();
        }

        /// <summary>
        /// 计算 模具费的 数量以及费用(单个方案)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public async Task<ResourcesManagementModel> PostCalculateMouldInventorySingle(ResourcesManagementModel resources)
        {
            resources.MouldInventory.Count = await _resourceNrePricingMethod.CalculateCount(resources.SolutionId, resources.MouldInventory);//计算数量
            double count = resources.MouldInventory.Count;
            resources.MouldInventory.Cost = count != 0 ? (decimal)count : 0 * resources.MouldInventory.UnitPrice;//计算费用
            return resources;
        }
        /// <summary>
        /// 资源部 模具费录入(单个方案)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task PostResourcesManagementSingle(ResourcesManagementSingleDto price)
        {

            ResourcesManagementModel resourcesManagementModel = new();
            resourcesManagementModel = price.ResourcesManagementModels;
            MouldInventory MouldInventorys = ObjectMapper.Map<MouldInventory>(resourcesManagementModel.MouldInventory);
            MouldInventorys.AuditFlowId = price.AuditFlowId;
            MouldInventorys.SolutionId = resourcesManagementModel.SolutionId;
            MouldInventorys.PeopleId = AbpSession.GetUserId();//提交人ID
            await _resourceMouldInventory.InsertOrUpdateAsync(MouldInventorys);//录入模具清单            
            #region 方案页面录入完成之后
            //long ResourcesManagementCount = await GetInitialResourcesManagementCount(price.AuditFlowId);
            //long count = await _resourceMouldInventory.CountAsync(p => p.IsSubmit&&p.AuditFlowId.Equals(price.AuditFlowId)) + (MouldInventorys.IsSubmit ? 1 : 0);
            if (await GetResourcesManagement(price.AuditFlowId, MouldInventorys.IsSubmit ? 1 : 0))
            {
                // _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = resourcesManagementModel.SolutionId, EnumSole = NreIsSubmitDto.ResourcesManagement.ToString() });
                //if (await this.GetResourcesManagement(price.AuditFlowId))
                //{                  
                //嵌入工作流
                await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                {
                    NodeInstanceId = price.NodeInstanceId,
                    FinanceDictionaryDetailId = price.Opinion,
                    Comment = price.Comment,
                });
                //}
            }
            #endregion          
        }
        /// <summary>
        /// 资源部模具费录入  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetResourcesManagement(long auditFlowId, long cun)
        {
            long ResourcesManagementCount = await GetInitialResourcesManagementCount(auditFlowId);
            long count = await _resourceMouldInventory.CountAsync(p => p.IsSubmit && p.AuditFlowId.Equals(auditFlowId)) + cun;
            return ResourcesManagementCount == count;
        }
        /// <summary>
        /// 资源部模具费录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        internal async Task GetResourcesManagementConfigurationState(long auditFlowId)
        {
            if (auditFlowId == 0) throw new FriendlyException("资源部模具费录入退回重置状态流程id不能为0");
            //await _resourceNreIsSubmit.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
            List<MouldInventory> prop = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            foreach (var item in prop)
            {
                item.IsSubmit = false;
                await _resourceMouldInventory.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 资源部模具费录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        internal async Task GetResourcesManagementConfigurationState(List<long> NreId)
        {
            if (NreId is null || NreId.Count == 0) throw new FriendlyException("资源部模具费录入id进行重置,传值不能为空或者数量为0");
            //await _resourceNreIsSubmit.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
            List<MouldInventory> prop = await _resourceMouldInventory.GetAllListAsync(p => NreId.Contains(p.Id));
            foreach (var item in prop)
            {
                item.IsSubmit = false;
                await _resourceMouldInventory.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 产品部-电子工程师 录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProductDepartment(ProductDepartmentDto price)
        {

            foreach (ProductDepartmentModel Product in price.ProductDepartmentModels)
            {
                List<LaboratoryFee> laboratoryFees = ObjectMapper.Map<List<LaboratoryFee>>(Product.laboratoryFeeModels);
                //判断 该方案 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(Product.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
                if (nreIsSubmits.Count is not 0)
                {

                    throw new FriendlyException(Product.ProductId + "该方案id已经提交过了");
                }
                try
                {
                    //删除原数据
                    await _resourceLaboratoryFee.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(Product.SolutionId));
                    foreach (LaboratoryFee laboratoryFee in laboratoryFees)
                    {
                        laboratoryFee.AuditFlowId = price.AuditFlowId;
                        laboratoryFee.SolutionId = Product.SolutionId;
                        await _resourceLaboratoryFee.InsertOrUpdateAsync(laboratoryFee);
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = Product.SolutionId, EnumSole = NreIsSubmitDto.ProductDepartment.ToString() });
                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        ///产品部-电子工程师录入  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetProductDepartment(long Id)
        {
            //获取 总共的方案
            List<SolutionModel> partModels = await TotalSolution(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的方案
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        ///  Nre 产品部EMC+电性能实验费  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetProductDepartmentConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
            foreach (NreIsSubmit item in nreAre)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        ///  Nre 产品部EMC+电性能实验费 录入(单个方案)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProductDepartmentSingle(ProductDepartmentSingleDto price)
        {
            //判断 该方案 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(price.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该方案方案id已经提交过了");
            }
            try
            {

                List<LaboratoryFee> laboratoryFees = ObjectMapper.Map<List<LaboratoryFee>>(price.ProductDepartmentModels);
                //删除原数据
                await _resourceLaboratoryFee.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(price.SolutionId));
                foreach (LaboratoryFee laboratoryFee in laboratoryFees)
                {
                    laboratoryFee.AuditFlowId = price.AuditFlowId;
                    laboratoryFee.SolutionId = price.SolutionId;
                    await _resourceLaboratoryFee.InsertOrUpdateAsync(laboratoryFee);
                }
                if (price.IsSubmit)
                {
                    #region 录入完成之后
                    if (price.Opinion == FinanceConsts.Done)
                    {
                        await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = price.SolutionId, EnumSole = NreIsSubmitDto.ProductDepartment.ToString() });
                    }
                    #endregion
                    if (await this.GetProductDepartment(price.AuditFlowId))
                    {
                        if (AbpSession.UserId is null)
                        {
                            throw new FriendlyException("请先登录");
                        }
                        #region 流程流转

                        //嵌入工作流
                        await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                        {
                            NodeInstanceId = price.NodeInstanceId,
                            FinanceDictionaryDetailId = price.Opinion,
                            Comment = price.Comment,
                        });
                        #endregion

                    }
                }


            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }

        /// <summary>
        ///  Nre 产品部EMC+电性能实验费 下载 Excel 解析数据模板
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<IActionResult> PostProductDepartmentDownloadExcel(string FileName = "NRE产品部EMC电性能实验费模版下载")
        {
            //try
            //{
            //    string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE产品部EMC电性能实验费模版.xlsx";
            //    return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //    {
            //        FileDownloadName = $"{FileName}.xlsx"
            //    };
            //}
            //catch (Exception e)
            //{
            //    throw new UserFriendlyException(e.Message);
            //}
            try
            {
                List<FoundationEmcDto> foundationEmcliables = await _foundationEmcAppService.GetListAllAsync(new GetFoundationEmcsInput() { MaxResultCount = 9999 });

                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE产品部EMC电性能实验费模版.xlsx";
                //创建Excel工作簿
                var workbook = new XSSFWorkbook(templatePath);
                ISheet sheet = workbook.GetSheetAt(0);
                //列数据约束
                workbook.SetConstraint(sheet, 0, 2, foundationEmcliables.Select(p => p.Name).ToArray());
                ISheet sheet2 = workbook.CreateSheet("EMC实验库");
                // 添加表头
                IRow headerRow = sheet2.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("实验分类");
                headerRow.CreateCell(1).SetCellValue("实验名称");
                headerRow.CreateCell(2).SetCellValue("单价");
                headerRow.CreateCell(3).SetCellValue("计价单位");
                headerRow.CreateCell(4).SetCellValue("工序维护人");
                headerRow.CreateCell(5).SetCellValue("工序维护时间");
                int index = 1;
                ICellStyle dateStyle = workbook.CreateCellStyle();
                IDataFormat dateFormat = workbook.CreateDataFormat();
                dateStyle.DataFormat = dateFormat.GetFormat("yyyy-MM-dd");
                // 设置第一列宽度为 20
                sheet2.SetColumnWidth(5, 20 * 256);
                foreach (FoundationEmcDto item in foundationEmcliables)
                {   // 添加数据
                    IRow dataRow = sheet2.CreateRow(index++);
                    dataRow.CreateCell(0).SetCellValue(item.Classification);
                    dataRow.CreateCell(1).SetCellValue(item.Name);
                    dataRow.CreateCell(2).SetCellValue(item.Price.ToString());
                    dataRow.CreateCell(3).SetCellValue(item.Unit);
                    dataRow.CreateCell(4).SetCellValue(item.LastModifierUserName);
                    dataRow.CreateCell(5).SetCellValue(item.CreationTime);
                    ICell cell = dataRow.CreateCell(5);
                    cell.CellStyle = dateStyle;
                    cell.SetCellValue(item.CreationTime);
                }
                // 保存工作簿
                using (MemoryStream fileStream = new MemoryStream())
                {
                    workbook.Write(fileStream);
                    return new FileContentResult(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{FileName}.xlsx"
                    };
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }




        /// <summary>
        ///  Nre 产品部EMC+电性能实验费 导入数据(不提交)(Excel 单个方案解析数据)
        /// </summary>
        /// <returns></returns>
        public async Task<List<LaboratoryFeeModel>> PostProductDepartmentSingleExcel(IFormFile filename)
        {
            try
            {
                if (System.IO.Path.GetExtension(filename.FileName) is not ".xlsx") throw new FriendlyException("模板文件类型不正确");
                using (var memoryStream = new MemoryStream())
                {
                    await filename.CopyToAsync(memoryStream);
                    List<LaboratoryFeeExcelModel> rowExcls = memoryStream.Query<LaboratoryFeeExcelModel>(startCell: "A2").ToList();
                    if (rowExcls.Count is 0) throw new FriendlyException("模板数据为空/未使用标准模板");
                    List<FoundationEmcDto> foundationEmcliables = await _foundationEmcAppService.GetListAllAsync(new GetFoundationEmcsInput() { MaxResultCount = 9999 });
                    List<LaboratoryFeeModel> rows = ObjectMapper.Map<List<LaboratoryFeeModel>>(rowExcls);
                    rows.RemoveAll(p => p.ProjectName == "" || p.ProjectName == null);
                    int index = 3;
                    rows.ForEach((row) =>
                    {
                        FoundationEmcDto foundationreliableDto = foundationEmcliables.FirstOrDefault(p => p.Name == row.ProjectName);
                        if (foundationreliableDto is null) throw new FriendlyException($"{index}行,您填写的值为{row.ProjectName},实验项目为请根据下拉选择填写!");
                        row.UnitPrice = (decimal)foundationreliableDto.Price;
                        row.Unit = foundationreliableDto.Unit;
                        row.AdjustmentCoefficient = 1;//调整系数默认为1
                        index++;
                    });
                    return rows;
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        ///   Nre 产品部EMC+电性能实验费  录入过的值(单个方案)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<IsSubmitLaboratoryFeeModel> GetProductDepartmentSingle(long auditFlowId, long solutionId)
        {
            IsSubmitLaboratoryFeeModel isSubmitLaboratoryFeeModel = new();
            isSubmitLaboratoryFeeModel.SolutionId = solutionId;
            //判断 该方案 是否已经录入
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
            isSubmitLaboratoryFeeModel.IsSubmit = Count > 0;
            List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
            List<LaboratoryFeeModel> laboratoryFeeModels = ObjectMapper.Map<List<LaboratoryFeeModel>>(laboratoryFees);
            isSubmitLaboratoryFeeModel.laboratoryFeeModels = laboratoryFeeModels;
            return isSubmitLaboratoryFeeModel;
        }
        /// <summary>
        ///  Nre 产品部EMC+电性能实验费 导出数据(传数据)
        /// </summary>
        /// <param name="laboratoryFeeModels"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<IActionResult> PostExportOfProductDepartmentForm(List<LaboratoryFeeModel> laboratoryFeeModels)
        {
            try
            {
                //用MiniExcel读取数据
                var values = new List<Dictionary<string, object>>();
                foreach (LaboratoryFeeModel item in laboratoryFeeModels)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                    { "试验项目", item.ProjectName },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.AdjustmentCoefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.CountBottomingOut },
                    { "计数-DV", item.CountDV },
                    { "计数-PV", item.CountPV },
                    { "总费用", item.AllCost },
                    { "备注", item.Remark },
                    };
                    values.Add(keyValuePairs);
                }
                MemoryStream memoryStream = new MemoryStream();
                await MiniExcel.SaveAsAsync(memoryStream, values);
                return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"EMC实验费{DateTime.Now}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }

        /// <summary>
        ///   Nre 产品部EMC+电性能实验费 导出数据(不传数据)
        /// </summary>      
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<IActionResult> GetExportOfProductDepartmentFeeForm(long auditFlowId, long solutionId)
        {
            try
            {
                List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                //用MiniExcel读取数据
                var values = new List<Dictionary<string, object>>();
                foreach (LaboratoryFee item in laboratoryFees)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                    { "试验项目", item.ProjectName },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.AdjustmentCoefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.CountBottomingOut },
                    { "计数-DV", item.CountDV },
                    { "计数-PV", item.CountPV },
                    { "总费用", item.AllCost },
                    { "备注", item.Remark },
                    };
                    values.Add(keyValuePairs);
                }
                MemoryStream memoryStream = new MemoryStream();
                await MiniExcel.SaveAsAsync(memoryStream, values);
                return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"环境实验费{DateTime.Now}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }



        /// <summary>
        /// Nre 品保部 录入
        /// </summary>
        /// <param name="qADepartmentDto"></param>
        /// <returns></returns>
        public async Task PostQADepartment(QADepartmentDto qADepartmentDto)
        {
            foreach (QADepartmentPartModel qad in qADepartmentDto.QADepartments)
            {
                //判断 该方案 是否已经录入
                List<NreIsSubmit> nreIsSubmitsQRA1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.SolutionId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
                List<NreIsSubmit> nreIsSubmitsQRA2 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.SolutionId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
                if (nreIsSubmitsQRA1.Count is not 0 && nreIsSubmitsQRA2.Count is not 0)
                {

                    throw new FriendlyException(qad.ProductId + "该方案id已经提交过了");
                }
                try
                {
                    List<EnvironmentalExperimentFee> qADepartments = ObjectMapper.Map<List<EnvironmentalExperimentFee>>(qad.EnvironmentalExperimentFees);
                    //删除原数据
                    await _resourceEnvironmentalExperimentFee.DeleteAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.SolutionId.Equals(qad.ProductId));
                    //环境实验费表 模型
                    foreach (var item in qADepartments)
                    {
                        item.AuditFlowId = qADepartmentDto.AuditFlowId;
                        item.SolutionId = qad.ProductId;
                        await _resourceEnvironmentalExperimentFee.InsertOrUpdateAsync(item);
                    }
                    List<QADepartmentQC> qADepartmentQCs = ObjectMapper.Map<List<QADepartmentQC>>(qad.QAQCDepartments);
                    //删除原数据
                    await _resourceQADepartmentQC.DeleteAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId));
                    //项目制程QC量检具表 模型
                    foreach (var item in qADepartmentQCs)
                    {
                        item.AuditFlowId = qADepartmentDto.AuditFlowId;
                        item.ProductId = qad.ProductId;
                        await _resourceQADepartmentQC.InsertOrUpdateAsync(item);
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qADepartmentDto.AuditFlowId, SolutionId = qad.ProductId, EnumSole = NreIsSubmitDto.EnvironmentalExperimentFee.ToString() });
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qADepartmentDto.AuditFlowId, SolutionId = qad.ProductId, EnumSole = NreIsSubmitDto.QRA2.ToString() });
                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        ///Nre 品保部  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetQADepartment(long auditFlowId)
        {
            //获取 总共的方案
            List<SolutionModel> partModels = await TotalSolution(auditFlowId);
            int AllCount = partModels.Count();
            //获取 已经提交的方案
            int Count1 = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
            int Count2 = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            return AllCount <= Count1 && AllCount <= Count2;
        }
        /// <summary>
        /// Nre 品保部  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetQADepartmentConfigurationState(long auditFlowId)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
            List<NreIsSubmit> nreAre2 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            foreach (NreIsSubmit item in nreAre2)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        ///Nre 品保部=>环境实验费 录入  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetExperimentItems(long auditFlowId)
        {
            //获取 总共的方案
            List<SolutionModel> partModels = await TotalSolution(auditFlowId);
            int AllCount = partModels.Count();
            //获取 已经提交的方案
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// Nre 品保部=>环境实验费 录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetExperimentItemsConfigurationState(long auditFlowId)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre 品保部=>环境实验费 录入(单个方案)
        /// </summary>
        /// <returns></returns>
        public async Task PostExperimentItemsSingle(ExperimentItemsSingleDto experimentItems)
        {
            //判断 该方案 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.SolutionId.Equals(experimentItems.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
            if (nreIsSubmits.Count is not 0)
            {
                throw new FriendlyException("该方案id已经提交过了");
            }
            try
            {
                List<EnvironmentalExperimentFee> qADepartments = ObjectMapper.Map<List<EnvironmentalExperimentFee>>(experimentItems.EnvironmentalExperimentFeeModels);
                //删除原数据
                await _resourceEnvironmentalExperimentFee.DeleteAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.SolutionId.Equals(experimentItems.SolutionId));
                //环境实验费表 模型
                foreach (EnvironmentalExperimentFee item in qADepartments)
                {
                    item.AuditFlowId = experimentItems.AuditFlowId;
                    item.SolutionId = experimentItems.SolutionId;
                    await _resourceEnvironmentalExperimentFee.InsertOrUpdateAsync(item);
                }
                if (experimentItems.IsSubmit)
                {
                    #region 录入完成之后
                    if (experimentItems.Opinion == FinanceConsts.Done)
                    {
                        await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = experimentItems.AuditFlowId, SolutionId = experimentItems.SolutionId, EnumSole = NreIsSubmitDto.EnvironmentalExperimentFee.ToString() });
                    }
                    #endregion
                    if (await this.GetExperimentItems(experimentItems.AuditFlowId))
                    {
                        //嵌入工作流
                        await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                        {
                            NodeInstanceId = experimentItems.NodeInstanceId,
                            FinanceDictionaryDetailId = experimentItems.Opinion,
                            Comment = experimentItems.Comment,
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>环境实验费 录入过的值(单个方案)
        /// </summary>
        /// <returns></returns>
        public async Task<ExperimentItemsModel> GetReturnExperimentItemsSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                List<ExperimentItemsModel> experimentItemsModels = new();
                //所有的方案
                List<SolutionModel> partModels = new();
                partModels = await TotalSolution(auditFlowId);
                partModels = partModels.Where(p => p.SolutionId.Equals(solutionId)).ToList();
                foreach (SolutionModel partModel in partModels)
                {
                    ExperimentItemsModel experimentItemsModel = new();
                    //判断 该方案 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.EnvironmentalExperimentFee.ToString()));
                    experimentItemsModel.IsSubmit = Count > 0;
                    experimentItemsModel.SolutionId = partModel.SolutionId;
                    experimentItemsModel.EnvironmentalExperimentFeeModels = new();
                    List<EnvironmentalExperimentFee> qADepartmentTests = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(partModel.SolutionId));
                    if (qADepartmentTests is not null) experimentItemsModel.EnvironmentalExperimentFeeModels = ObjectMapper.Map<List<EnvironmentalExperimentFeeModel>>(qADepartmentTests);
                    experimentItemsModels.Add(experimentItemsModel);
                }
                return experimentItemsModels.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  Nre 品保部=>环境实验费 产品开发部-NRE 下载 Excel 解析数据模板
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<IActionResult> PostExperimentItemsSingleDownloadExcel(string FileName = "NRE环境实验费模版下载")
        {
            try
            {
                List<FoundationreliableDto> foundationreliables = await _foundationreliableAppService.GetListAllAsync(new GetFoundationreliablesInput() { MaxResultCount = 9999 });

                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE环境实验费模版.xlsx";
                //创建Excel工作簿
                var workbook = new XSSFWorkbook(templatePath);
                ISheet sheet = workbook.GetSheetAt(0);
                //列数据约束
                workbook.SetConstraint(sheet, 0, 2, foundationreliables.Select(p => p.Name).ToArray());
                ISheet sheet2 = workbook.CreateSheet("环境实验库");
                // 添加表头
                IRow headerRow = sheet2.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("实验分类");
                headerRow.CreateCell(1).SetCellValue("实验名称");
                headerRow.CreateCell(2).SetCellValue("单价");
                headerRow.CreateCell(3).SetCellValue("计价单位");
                headerRow.CreateCell(4).SetCellValue("工序维护人");
                headerRow.CreateCell(5).SetCellValue("工序维护时间");
                int index = 1;
                ICellStyle dateStyle = workbook.CreateCellStyle();
                IDataFormat dateFormat = workbook.CreateDataFormat();
                dateStyle.DataFormat = dateFormat.GetFormat("yyyy-MM-dd");
                // 设置第一列宽度为 20
                sheet2.SetColumnWidth(5, 20 * 256);
                foreach (FoundationreliableDto item in foundationreliables)
                {   // 添加数据
                    IRow dataRow = sheet2.CreateRow(index++);
                    dataRow.CreateCell(0).SetCellValue(item.Classification);
                    dataRow.CreateCell(1).SetCellValue(item.Name);
                    dataRow.CreateCell(2).SetCellValue(item.Price.ToString());
                    dataRow.CreateCell(3).SetCellValue(item.Unit);
                    dataRow.CreateCell(4).SetCellValue(item.LastModifierUserName);
                    ICell cell = dataRow.CreateCell(5);
                    cell.CellStyle = dateStyle;
                    cell.SetCellValue(item.CreationTime);
                }
                // 保存工作簿
                using (MemoryStream fileStream = new MemoryStream())
                {
                    workbook.Write(fileStream);
                    return new FileContentResult(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{FileName}.xlsx"
                    };
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>环境实验费 导入数据(不提交)(Excel 单个方案解析数据)
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnvironmentalExperimentFeeModel>> PostExperimentItemsSingleExcel(IFormFile filename)
        {
            try
            {
                if (System.IO.Path.GetExtension(filename.FileName) is not ".xlsx") throw new FriendlyException("模板文件类型不正确");
                using (var memoryStream = new MemoryStream())
                {
                    await filename.CopyToAsync(memoryStream);
                    List<QADepartmentTestExcelModel> rowExcls = memoryStream.Query<QADepartmentTestExcelModel>(startCell: "A2").ToList();
                    if (rowExcls.Count is 0) throw new FriendlyException("模板数据为空/未使用标准模板");
                    List<FoundationreliableDto> foundationreliables = await _foundationreliableAppService.GetListAllAsync(new GetFoundationreliablesInput() { MaxResultCount = 9999 });
                    List<EnvironmentalExperimentFeeModel> rows = ObjectMapper.Map<List<EnvironmentalExperimentFeeModel>>(rowExcls);
                    rows.RemoveAll(p => p.ProjectName == "" || p.ProjectName == null);
                    int index = 3;
                    rows.ForEach((row) =>
                    {
                        FoundationreliableDto foundationreliableDto = foundationreliables.FirstOrDefault(p => p.Name == row.ProjectName);
                        if (foundationreliableDto is null) throw new FriendlyException($"{index}行,您填写的值为{row.ProjectName},实验项目为请根据下拉选择填写!");
                        row.UnitPrice = (decimal)foundationreliableDto.Price;
                        row.Unit = foundationreliableDto.Unit;
                        row.AdjustmentCoefficient = 1;//调整系数默认为1
                        index++;
                    });
                    return rows;
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  Nre 品保部=>环境实验费 导出数据(传数据)
        /// </summary>
        /// <param name="environmentalExperimentFeeModels"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<IActionResult> PostExportOfEnvironmentalExperimentFeeForm(List<EnvironmentalExperimentFeeModel> environmentalExperimentFeeModels)
        {
            try
            {
                //用MiniExcel读取数据
                var values = new List<Dictionary<string, object>>();
                foreach (EnvironmentalExperimentFeeModel item in environmentalExperimentFeeModels)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                    { "试验项目", item.ProjectName },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.AdjustmentCoefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.CountBottomingOut },
                    { "计数-DV", item.CountDV },
                    { "计数-PV", item.CountPV },
                    { "总费用", item.AllCost },
                    { "备注", item.Remark },
                    };
                    values.Add(keyValuePairs);
                }
                MemoryStream memoryStream = new MemoryStream();
                await MiniExcel.SaveAsAsync(memoryStream, values);
                return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"环境实验费{DateTime.Now}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  Nre 品保部=>环境实验费 导出数据(不传数据)
        /// </summary>      
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<IActionResult> GetExportOfEnvironmentalExperimentFeeForm([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                List<EnvironmentalExperimentFee> environmentalExperimentFees = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                //用MiniExcel读取数据
                var values = new List<Dictionary<string, object>>();
                foreach (EnvironmentalExperimentFee item in environmentalExperimentFees)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                    { "试验项目", item.ProjectName },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.AdjustmentCoefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.CountBottomingOut },
                    { "计数-DV", item.CountDV },
                    { "计数-PV", item.CountPV },
                    { "总费用", item.AllCost },
                    { "备注", item.Remark },
                    };
                    values.Add(keyValuePairs);
                }
                MemoryStream memoryStream = new MemoryStream();
                await MiniExcel.SaveAsAsync(memoryStream, values);
                return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"环境实验费{DateTime.Now}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }

        /// <summary>
        /// Ner  营销部录入初始值
        /// </summary>
        /// <param name="auditFlowId">流程表Id</param>
        /// <returns></returns>
        public async Task<List<ReturnSalesDepartmentDto>> GetInitialSalesDepartment([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId)
        {

            List<ReturnSalesDepartmentDto> initialSalesDepartmentDtos = new();
            ReturnSalesDepartmentDto initialSalesDepartmentDto = new();
            List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            //手板件费            
            List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += handPieceCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.SBJF;//这里就是写死的,因为手板费是一个表,我要从指定的表获取 费用的总和,这个不能配置             
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //模具费
            initialSalesDepartmentDto = new();
            List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += mouldInventories.Where(p => p.SolutionId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.MJF;
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //生产设备费  
            initialSalesDepartmentDto = new();
            initialSalesDepartmentDto.FormName = StaticName.SCSBF;
            List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId))
                                                  join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Equipment)) on a.Id equals b.WorkHoursId
                                                  select new EquipmentInfo
                                                  {
                                                      Number = b.Number,
                                                      UnitPrice = b.UnitPrice,
                                                      Id = a.ProductId,
                                                  }).ToList();
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += equipmentInfos.Where(p => p.Id.Equals(item.Id)).Sum(item => item.Number * item.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //工装治具费
            initialSalesDepartmentDto = new();
            initialSalesDepartmentDto.FormName = StaticName.GZZJF;//工装+治具+测试线         
            //测试线+工装
            List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            //治具
            List<EquipmentInfo> equipment = (from a in workingHours
                                             join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Fixture)) on a.Id equals b.WorkHoursId
                                             select new EquipmentInfo
                                             {
                                                 Number = b.Number,
                                                 UnitPrice = b.UnitPrice,
                                                 Id = a.ProductId,
                                             }).ToList();
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += workingHours.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.TestNum * s.TestPrice + s.ToolingPrice * s.ToolingNum)/* * item.SingleCarProductsQuantity*/;
                initialSalesDepartmentDto.PricingMoney += equipment.Where(p => p.Id.Equals(item.Id)).Sum(s => s.Number * s.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //检具费
            initialSalesDepartmentDto = new();
            List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            initialSalesDepartmentDto.FormName = StaticName.JJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += qADepartmentQCs.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Count * s.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //实验费
            initialSalesDepartmentDto = new();
            List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<EnvironmentalExperimentFee> qADepartmentTests = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            initialSalesDepartmentDto.FormName = StaticName.SYF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += laboratoryFees.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
                initialSalesDepartmentDto.PricingMoney += qADepartmentTests.Where(p => p.SolutionId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //测试软件费  (硬件费用+追溯软件费用+开图软件费用)
            initialSalesDepartmentDto = new();
            List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            initialSalesDepartmentDto.FormName = StaticName.CSRJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += workingHoursInfos.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.HardwareTotalPrice + s.TraceabilityDevelopmentFee + s.MappingDevelopmentFee)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //差旅费
            initialSalesDepartmentDto = new();
            List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            initialSalesDepartmentDto.FormName = StaticName.CLF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += travelExpenses.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //其他费用
            initialSalesDepartmentDto = new();
            List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            initialSalesDepartmentDto.FormName = StaticName.QTFY;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += restsCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            List<ReturnSalesDepartmentDto> returnSalesDepartmentDtos = await GetReturnInitialSalesDepartment(auditFlowId);
            if (returnSalesDepartmentDtos is not null)
            {
                foreach (var returnSales in returnSalesDepartmentDtos)
                {
                    foreach (ReturnSalesDepartmentDto nitialSales in initialSalesDepartmentDtos)
                    {
                        if (returnSales.FormName.Equals(nitialSales.FormName))
                        {
                            nitialSales.Id = returnSales.Id;
                            nitialSales.OfferCoefficient = returnSales.OfferCoefficient;
                            nitialSales.OfferMoney = returnSales.OfferMoney;
                            nitialSales.Remark = returnSales.Remark;
                        }
                    }
                }
            }
            return initialSalesDepartmentDtos;
        }
        /// <summary>
        /// Ner  营销部录入
        /// </summary>
        /// <param name="departmentDtos"></param>
        /// <returns></returns>
        public async Task PostSalesDepartment(List<InitialSalesDepartmentDto> departmentDtos)
        {
            try
            {
                List<InitialResourcesManagement> initialResourcesManagements = ObjectMapper.Map<List<InitialResourcesManagement>>(departmentDtos);
                foreach (var item in initialResourcesManagements)
                {
                    await _resourceInitialResourcesManagement.InsertOrUpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Ner  营销部 录入过的值
        /// </summary>
        /// <param name="Id">流程表Id</param>
        /// <returns></returns>
        public async Task<List<ReturnSalesDepartmentDto>> GetReturnInitialSalesDepartment(long Id)
        {
            List<InitialResourcesManagement> initialResourcesManagements = await _resourceInitialResourcesManagement.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            List<ReturnSalesDepartmentDto> returnSalesDepartmentDtos = new List<ReturnSalesDepartmentDto>();
            returnSalesDepartmentDtos = ObjectMapper.Map<List<ReturnSalesDepartmentDto>>(initialResourcesManagements);
            return returnSalesDepartmentDtos;
        }
        /// <summary>
        ///整个NRE  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetNreConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// 获取 第一个页面最初的年份
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        private async Task<int> GetYear(long auditFlowId)
        {
            List<ModelCountYear> modelCountYears = await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<int> yearList = modelCountYears.Select(p => p.Year).Distinct().ToList();
            int year = yearList.Min();
            return year;
        }
        /// <summary>
        /// 将json 转成  List YearOrValueMode>
        /// </summary>
        internal static List<YearOrValueMode> JsonExchangeRateValue(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
        /// <summary>
        ///获取 Nre 核价表 下载
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<PricingFormDto> GetPricingFormDownload([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                PriceEvaluation priceEvaluation = await _resourcePriceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
                List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                PricingFormDto pricingFormDto = new();
                pricingFormDto = ObjectMapper.Map<PricingFormDto>(await GetPricingForm(auditFlowId, solutionId));
                //替换被修改项的值
                //手板件费用
                List<HandPieceCostModify> handPieceCostModifies = _handPieceCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (HandPieceCostModel item in pricingFormDto.HandPieceCost)
                {
                    HandPieceCostModify modify = handPieceCostModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.PartName = modify.PartName;
                        item.PartNumber = modify.PartNumber;
                        item.UnitPrice = modify.UnitPrice;
                        item.Quantity = modify.Quantity;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //手板件新增项
                List<HandPieceCostModify> handPieceCostModifiesprop = handPieceCostModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (HandPieceCostModify modify in handPieceCostModifiesprop)
                {
                    pricingFormDto.HandPieceCost.Add(new HandPieceCostModel()
                    {
                        PartName = modify.PartName,
                        PartNumber = modify.PartNumber,
                        UnitPrice = modify.UnitPrice,
                        Quantity = modify.Quantity,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                //模具费用
                List<MouldInventoryModify> mouldInventoryModifies = _mouldInventoryModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (MouldInventoryModel item in pricingFormDto.MouldInventory)
                {
                    MouldInventoryModify modify = mouldInventoryModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.StructuralId = modify.StructuralId;
                        item.ModelName = modify.ModelName;
                        item.UnitPrice = modify.UnitPrice;
                        item.MoldCavityCount = modify.MoldCavityCount;
                        item.ModelNumber = modify.ModelNumber;
                        item.Count = modify.Count;
                        item.UnitPrice = modify.UnitPrice;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;

                    }
                }
                //模具费用新增项
                List<MouldInventoryModify> mouldInventoryModifiesprop = mouldInventoryModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (MouldInventoryModify modify in mouldInventoryModifiesprop)
                {
                    pricingFormDto.MouldInventory.Add(new()
                    {
                        StructuralId = modify.StructuralId,
                        ModelName = modify.ModelName,
                        UnitPrice = modify.UnitPrice,
                        MoldCavityCount = modify.MoldCavityCount,
                        ModelNumber = modify.ModelNumber,
                        Count = modify.Count,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                //工装费用
                List<ToolingCostsModify> toolingCostsModifies = _toolingCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (ToolingCostModel item in pricingFormDto.ToolingCost)
                {
                    ToolingCostsModify modify = toolingCostsModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.WorkName = modify.WorkName;
                        item.UnitPriceOfTooling = modify.UnitPriceOfTooling;
                        item.ToolingCount = modify.ToolingCount;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //工装费用新增项
                List<ToolingCostsModify> toolingCostsModifiesprop = toolingCostsModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (ToolingCostsModify modify in toolingCostsModifiesprop)
                {
                    pricingFormDto.ToolingCost.Add(new()
                    {
                        WorkName = modify.WorkName,
                        UnitPriceOfTooling = modify.UnitPriceOfTooling,
                        ToolingCount = modify.ToolingCount,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                //治具费用
                List<FixtureCostsModify> fixtureCostsModifies = _fixtureCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (FixtureCostModel item in pricingFormDto.FixtureCost)
                {
                    FixtureCostsModify modify = fixtureCostsModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.ToolingName = modify.ToolingName;
                        item.UnitPrice = modify.UnitPrice;
                        item.Number = modify.Number;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //治具费用新增项
                List<FixtureCostsModify> fixtureCostsModifiesprop = fixtureCostsModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (FixtureCostsModify modify in fixtureCostsModifiesprop)
                {
                    pricingFormDto.FixtureCost.Add(new()
                    {
                        ToolingName = modify.ToolingName,
                        UnitPrice = modify.UnitPrice,
                        Number = modify.Number,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                //检具费用
                List<InspectionToolCostModify> inspectionToolCostModifies = _inspectionToolCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (QADepartmentQCModel item in pricingFormDto.QAQCDepartments)
                {
                    InspectionToolCostModify modify = inspectionToolCostModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.Qc = modify.Qc;
                        item.UnitPrice = modify.UnitPrice;
                        item.Count = modify.Count;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //检具费用新增项
                List<InspectionToolCostModify> inspectionToolCostModifiesprop = inspectionToolCostModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (InspectionToolCostModify modify in inspectionToolCostModifiesprop)
                {
                    pricingFormDto.QAQCDepartments.Add(new()
                    {
                        Qc = modify.Qc,
                        UnitPrice = modify.UnitPrice,
                        Count = modify.Count,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                //生产设备费用
                List<ProductionEquipmentCostsModify> productionEquipmentCostsModifies = _productionEquipmentCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (ProductionEquipmentCostModel item in pricingFormDto.ProductionEquipmentCost)
                {
                    ProductionEquipmentCostsModify modify = productionEquipmentCostsModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.EquipmentName = modify.EquipmentName;
                        item.Number = modify.Number;
                        item.UnitPrice = modify.UnitPrice;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                        item.DeviceStatus = modify.DeviceStatus;
                        item.DeviceStatusName = await GetDisplayName(modify.DeviceStatus);
                    }
                }
                //生产设备费用新增项
                List<ProductionEquipmentCostsModify> productionEquipmentCostsModifiesprop = productionEquipmentCostsModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (var modify in productionEquipmentCostsModifiesprop)
                {
                    pricingFormDto.ProductionEquipmentCost.Add(new()
                    {
                        EquipmentName = modify.EquipmentName,
                        Number = modify.Number,
                        UnitPrice = modify.UnitPrice,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                        DeviceStatus = modify.DeviceStatus,
                        DeviceStatusName = await GetDisplayName(modify.DeviceStatus),
                    });
                }
                //实验费用
                List<ExperimentalExpensesModify> experimentalExpensesModifies = _experimentalExpensesModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (LaboratoryFeeModel item in pricingFormDto.LaboratoryFeeModels)
                {
                    ExperimentalExpensesModify modify = experimentalExpensesModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.ProjectName = modify.ProjectName;
                        item.IsThirdParty = modify.IsThirdParty;
                        item.UnitPrice = modify.UnitPrice;
                        item.AdjustmentCoefficient = modify.AdjustmentCoefficient;
                        item.Unit = modify.Unit;
                        item.CountBottomingOut = modify.CountBottomingOut;
                        item.CountDV = modify.CountDV;
                        item.CountPV = modify.CountPV;
                        item.AllCost = modify.AllCost;
                        item.Remark = modify.Remark;
                    }
                }
                //实验费用新增项
                List<ExperimentalExpensesModify> experimentalExpensesModifiesprop = experimentalExpensesModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (ExperimentalExpensesModify modify in experimentalExpensesModifiesprop)
                {
                    pricingFormDto.LaboratoryFeeModels.Add(new()
                    {
                        ProjectName = modify.ProjectName,
                        IsThirdParty = modify.IsThirdParty,
                        UnitPrice = modify.UnitPrice,
                        AdjustmentCoefficient = modify.AdjustmentCoefficient,
                        Unit = modify.Unit,
                        CountBottomingOut = modify.CountBottomingOut,
                        CountDV = modify.CountDV,
                        CountPV = modify.CountPV,
                        AllCost = modify.AllCost,
                        Remark = modify.Remark,
                    });
                }
                //测试软件费用
                List<TestingSoftwareCostsModify> testingSoftwareCostsModifies = _testingSoftwareCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (SoftwareTestingCotsModel item in pricingFormDto.SoftwareTestingCost)
                {
                    TestingSoftwareCostsModify modify = testingSoftwareCostsModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.SoftwareProject = modify.SoftwareProject;
                        item.CostH = modify.CostH;
                        item.Hour = modify.Hour;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //测试软件费用新增项
                List<TestingSoftwareCostsModify> testingSoftwareCostsModifiesprop = testingSoftwareCostsModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (TestingSoftwareCostsModify modify in testingSoftwareCostsModifiesprop)
                {
                    pricingFormDto.SoftwareTestingCost.Add(new()
                    {
                        SoftwareProject = modify.SoftwareProject,
                        CostH = modify.CostH,
                        Hour = modify.Hour,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                // 差旅费用
                List<TravelExpenseModify> travelExpenseModifies = _travelExpenseModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (TravelExpenseModel item in pricingFormDto.TravelExpense)
                {
                    TravelExpenseModify modify = travelExpenseModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.ReasonsId = modify.ReasonsId;
                        item.ReasonsName = await GetDisplayName(modify.ReasonsId);
                        item.PeopleCount = modify.PeopleCount;
                        item.CostSky = modify.CostSky;
                        item.SkyCount = modify.SkyCount;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //差旅费用新增项
                List<TravelExpenseModify> travelExpenseModifiesprop = travelExpenseModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (TravelExpenseModify modify in travelExpenseModifiesprop)
                {
                    pricingFormDto.TravelExpense.Add(new()
                    {
                        ReasonsId = modify.ReasonsId,
                        ReasonsName = await GetDisplayName(modify.ReasonsId),
                        PeopleCount = modify.PeopleCount,
                        CostSky = modify.CostSky,
                        SkyCount = modify.SkyCount,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                // 其他费用
                List<RestsCostModify> restsCostModifies = _restsCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (RestsCostModel item in pricingFormDto.RestsCost)
                {
                    RestsCostModify modify = restsCostModifies.FirstOrDefault(p => p.ModifyId.Equals(item.Id));
                    if (modify != null)
                    {
                        item.ConstName = modify.ConstName;
                        item.Cost = modify.Cost;
                        item.Remark = modify.Remark;
                    }
                }
                //其他费用新增项
                List<RestsCostModify> restsCostModifiesprop = restsCostModifies.Where(p => p.ModifyId == 0).ToList();
                foreach (RestsCostModify modify in restsCostModifiesprop)
                {
                    pricingFormDto.RestsCost.Add(new()
                    {
                        ConstName = modify.ConstName,
                        Cost = modify.Cost,
                        Remark = modify.Remark,
                    });
                }
                pricingFormDto.HandPieceCostTotal = pricingFormDto.HandPieceCost.Sum(p => p.Cost);
                pricingFormDto.MouldInventoryTotal = pricingFormDto.MouldInventory.Sum(p => p.Cost);
                pricingFormDto.ToolingCostTotal = pricingFormDto.ToolingCost.Sum(p => p.Cost);
                pricingFormDto.FixtureCostTotal = pricingFormDto.FixtureCost.Sum(p => p.Cost);
                pricingFormDto.QAQCDepartmentsTotal = pricingFormDto.QAQCDepartments.Sum(p => p.Cost);
                pricingFormDto.ProductionEquipmentCostTotal = pricingFormDto.ProductionEquipmentCost.Sum(p => p.Cost);
                pricingFormDto.LaboratoryFeeModelsTotal = pricingFormDto.LaboratoryFeeModels.Sum(p => p.AllCost);
                pricingFormDto.SoftwareTestingCostTotal = pricingFormDto.SoftwareTestingCost.Sum(p => p.Cost);
                pricingFormDto.TravelExpenseTotal = pricingFormDto.TravelExpense.Sum(p => p.Cost);
                pricingFormDto.RestsCostTotal = pricingFormDto.RestsCost.Sum(p => p.Cost);
                //(不含税人民币) NRE 总费用
                pricingFormDto.RMBAllCost = pricingFormDto.HandPieceCost.Sum(p => p.Cost)//手板件总费用
                                         + pricingFormDto.MouldInventory.Sum(p => p.Cost)//模具清单总费用
                                         + pricingFormDto.ToolingCost.Sum(p => p.Cost)//工装费用总费用
                                         + pricingFormDto.FixtureCost.Sum(p => p.Cost)//治具费用总费用
                                         + pricingFormDto.QAQCDepartments.Sum(p => p.Cost)//检具费用总费用
                                         + pricingFormDto.ProductionEquipmentCost.Sum(p => p.Cost)//生产设备总费用
                                         + pricingFormDto.LaboratoryFeeModels.Sum(p => p.AllCost)//实验费用总费用
                                         + pricingFormDto.SoftwareTestingCost.Sum(p => p.Cost)//测试软件总费用
                                         + pricingFormDto.TravelExpense.Sum(p => p.Cost)//差旅费总费用
                                         + pricingFormDto.RestsCost.Sum(p => p.Cost);//其他费用总费用
                int year = await GetYear(auditFlowId);
                //获取汇率
                ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals("USD"));
                List<YearOrValueMode> yearOrValueModes = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                YearOrValueMode exchangeRateModel = new();
                if (yearOrValueModes.Count is not 0) exchangeRateModel = yearOrValueModes.FirstOrDefault(p => p.Year.Equals(year));
                //(不含税美金) NRE 总费用
                pricingFormDto.USDAllCost = 0.0M;
                if (exchangeRateModel is not null)
                {
                    pricingFormDto.USDAllCost = pricingFormDto.RMBAllCost / exchangeRateModel.Value;
                }
                else
                {
                    pricingFormDto.USDAllCost = pricingFormDto.RMBAllCost;
                }
                return pricingFormDto;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        ///获取 Nre 核价表
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<ModifyItemPricingFormDto> GetPricingForm([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                PriceEvaluation priceEvaluation = await _resourcePriceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
                List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                List<ModelCountYear> modelCountYears = await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                ModifyItemPricingFormDto modify = new ModifyItemPricingFormDto();
                if (priceEvaluation is not null)
                {
                    modify.ProjectName = priceEvaluation.ProjectName;
                    modify.ClientName = priceEvaluation.CustomerName;
                }
                //线体数量和共线分摊率的 乘积
                decimal UphAndValuesd = 1M;
                //线体数量和共线分摊率的值
                List<ProcessHoursEnterLine> processHoursEnterLines = await _processHoursEnterLine.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                List<UphAndValue> result = (from a in processHoursEnterLines
                                            join b in modelCountYears on a.ModelCountYearId equals b.Id
                                            select new UphAndValue
                                            {

                                                Uph = a.Uph,
                                                Value = (decimal)a.Value,
                                                Year = (int)b.Year,
                                                Description = a.Uph.ParseEnum<OperateTypeCode>().GetDescription()
                                            }).ToList();
                if (result.Count is not 0)
                {
                    //先判断线体数量是否一致 如果一致 则继续判断共线分摊率
                    var xtsl = result.Where(a => a.Uph.Equals(OperateTypeCode.xtsl.ToString())).ToList();
                    if (xtsl.Select(x => x.Value).Distinct().Count() == 1)
                    {
                        //每一年的值都一样  
                        var gxftl = result.Where(a => a.Uph.Equals(OperateTypeCode.gxftl.ToString())).ToList();
                        //继续判断共线分摊率
                        if (gxftl.Select(x => x.Value).Distinct().Count() == 1)
                        {
                            //获取值最大年份的那一年
                            var maxYear = gxftl.Max(p => p.Year);
                            result = result.Where(p => p.Year.Equals(maxYear)).ToList();
                        }
                        else
                        {
                            //获取值最大的那年
                            var maxGxftl = gxftl.OrderByDescending(p => p.Value).FirstOrDefault();
                            result = result.Where(p => p.Year.Equals(maxGxftl.Year)).ToList();
                        }
                    }
                    else
                    {
                        //获取值最大的那年
                        var maxXtsl = xtsl.OrderByDescending(p => p.Value).FirstOrDefault();
                        result = result.Where(p => p.Year.Equals(maxXtsl.Year)).ToList();
                    }
                }

                decimal NumberOfLines = result
               .FirstOrDefault(a => a.Uph.Equals(OperateTypeCode.xtsl.ToString()))?.Value ?? 0;


                modify.UphAndValues = result;
                foreach (UphAndValue item in modify.UphAndValues)
                {
                    //if (item.Uph.Equals(OperateTypeCode.gxftl.ToString())) item.Value=item.Value / 100;
                    UphAndValuesd *= item.Value;
                }
                //获取产能需求
                modify.RequiredCapacity = modelCount.Sum(p => p.SumQuantity).ToString();
                //手板件费用
                List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                modify.HandPieceCost = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                modify.HandPieceCostTotal = modify.HandPieceCost.Sum(p => p.Cost);
                //模具费用
                //List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                modify.MouldInventory =(await GetInitialResourcesManagementSingle(auditFlowId, solutionId)).MouldInventoryModels;
                foreach (MouldInventoryModel item in modify.MouldInventory)
                {
                    User user = await _userRepository.FirstOrDefaultAsync(p => p.Id == item.PeopleId);
                    if (user is not null) item.PeopleName = user.Name;//提交人名称              
                }
                modify.MouldInventoryTotal = modify.MouldInventory.Sum(p => p.Cost);
                List<ProcessHoursEnter> processHours = await _processHoursEnter.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                //工装费用 (工装费用+测试线费用)              
                List<ToolingCostModel> workingHoursInfosGZ = new();
                //工装费用=>工装费用             
                workingHoursInfosGZ = processHours.Where(p => p.FrockName is not null).GroupBy(m => new { m.FrockName, m.FrockPrice }).Select(a => new ToolingCostModel
                {
                    Id = processHours.Where(p => p.FrockName == a.Key.FrockName && p.FrockPrice == a.Key.FrockPrice).Select(p => p.Id).FirstOrDefault(),
                    WorkName = a.Key.FrockName,
                    UnitPriceOfTooling = a.Key.FrockPrice,
                    ToolingCount = (int)a.Sum(m => m.FrockNumber),
                    Cost = a.Key.FrockPrice * a.Sum(m => m.FrockNumber) //* UphAndValuesd
                }).ToList();
                modify.ToolingCost = workingHoursInfosGZ;
                //工装费用=>测试线费用               
                List<ToolingCostModel> workingHoursInfosCSX = processHours.Where(p => p.TestLineName is not null).GroupBy(m => new { m.TestLineName, m.TestLinePrice }).Select(a => new ToolingCostModel
                {
                    Id = processHours.Where(p => p.TestLineName == a.Key.TestLineName && p.TestLinePrice == a.Key.TestLinePrice).Select(p => p.Id).FirstOrDefault() + 1,
                    WorkName = a.Key.TestLineName,
                    UnitPriceOfTooling = (decimal)a.Key.TestLinePrice,
                    ToolingCount = (int)a.Sum(m => m.TestLineNumber),
                    Cost = (decimal)(a.Key.TestLinePrice * a.Sum(m => m.TestLineNumber)) //* UphAndValuesd,
                }).ToList();
                modify.ToolingCost.AddRange(workingHoursInfosCSX);
                modify.ToolingCostTotal = modify.ToolingCost.Sum(p => p.Cost);
                //治具费用               
                List<ProcessHoursEnterFixture> processHoursEnterFixtures = (from a in processHours
                                                                            join b in await _processHoursEnterFixture.GetAllListAsync() on a.Id equals b.ProcessHoursEnterId
                                                                            select new ProcessHoursEnterFixture
                                                                            {
                                                                                Id = b.Id,
                                                                                ProcessHoursEnterId = b.ProcessHoursEnterId,
                                                                                FixtureName = b.FixtureName,
                                                                                FixturePrice = b.FixturePrice,
                                                                                FixtureNumber = b.FixtureNumber
                                                                            }).ToList();
                List<FixtureCostModel> productionEquipmentCostModelsZj = processHoursEnterFixtures.GroupBy(m => new { m.FixtureName, m.FixturePrice }).Select(
                     a => new FixtureCostModel
                     {
                         Id = processHoursEnterFixtures.Where(p => p.FixtureName == a.Key.FixtureName && p.FixturePrice == a.Key.FixturePrice).Select(p => p.Id).FirstOrDefault(),
                         ToolingName = a.Key.FixtureName,
                         UnitPrice = (decimal)a.Key.FixturePrice,
                         Number = (int)a.Sum(c => c.FixtureNumber),
                         Cost = (decimal)(a.Key.FixturePrice * a.Sum(c => c.FixtureNumber)) //* UphAndValuesd
                     }).ToList();
                modify.FixtureCost = productionEquipmentCostModelsZj;
                modify.FixtureCostTotal = modify.FixtureCost.Sum(p => p.Cost);
                //检具费用(有变化,工装治具)
                //List<ProcessHoursEnter> processHours= await _processHoursEnter.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));            
                //modify.QAQCDepartments = (from a in processHours
                //                          select new QADepartmentQCModel
                //                          {
                //                              Qc = a.FixtureName,
                //                              UnitPrice = a.FixturePrice,
                //                              Count = (int)a.FixtureNumber,
                //                              Cost = a.FixturePrice * a.FixtureNumber* UphAndValuesd
                //                          }).ToList();

                modify.QAQCDepartments = processHours.GroupBy(m => new { m.FixtureName, m.FixturePrice }).Select(
                     a => new QADepartmentQCModel
                     {
                         Id = processHoursEnterFixtures.Where(p => p.FixtureName == a.Key.FixtureName && p.FixturePrice == a.Key.FixturePrice).Select(p => p.Id).FirstOrDefault(),
                         Qc = a.Key.FixtureName,
                         UnitPrice = (decimal)a.Key.FixturePrice,
                         Count = (int)a.Sum(c => c.FixtureNumber),
                         Cost = ((decimal)(a.Key.FixturePrice * a.Sum(c => c.FixtureNumber)) * UphAndValuesd) / 100
                     }).ToList();
                modify.QAQCDepartmentsTotal = modify.QAQCDepartments.Sum(p => p.Cost);
                //生产设备费用 
                List<ProcessHoursEnterDevice> processHoursEnterDevices = (from a in processHours
                                                                          join b in await _processHoursEnterDevice.GetAllListAsync() on a.Id equals b.ProcessHoursEnterId
                                                                          select new ProcessHoursEnterDevice
                                                                          {
                                                                              Id = b.Id,
                                                                              DeviceName = b.DeviceName,
                                                                              DeviceNumber = b.DeviceNumber,
                                                                              DevicePrice = b.DevicePrice,
                                                                              DeviceStatus = b.DeviceStatus,
                                                                              ProcessHoursEnterId = b.ProcessHoursEnterId
                                                                          }).ToList();
                List<ProductionEquipmentCostModel> productionEquipmentCostModels = processHoursEnterDevices.GroupBy(m => new { m.DeviceName, m.DevicePrice, m.DeviceStatus }).Select(
                    a => new ProductionEquipmentCostModel
                    {
                        Id = processHoursEnterDevices.Where(p => p.DeviceName == a.Key.DeviceName && p.DevicePrice == a.Key.DevicePrice && p.DeviceStatus == a.Key.DeviceStatus).Select(p => p.Id).FirstOrDefault(),
                        EquipmentName = a.Key.DeviceName,
                        DeviceStatus = a.Key.DeviceStatus,
                        UnitPrice = (decimal)a.Key.DevicePrice,
                        Number = (int)a.Sum(c => c.DeviceNumber),
                        Cost = a.Key.DeviceStatus == Status.zy.ToString() ? (decimal)(a.Key.DevicePrice * a.Sum(c => c.DeviceNumber) * NumberOfLines) : (decimal)(a.Key.DevicePrice * a.Sum(c => c.DeviceNumber) * UphAndValuesd),
                    }).ToList();
                List<ProductionEquipmentCostModel> productionEquipmentCostModelsjoinedList = (from t in productionEquipmentCostModels
                                                                                              join p in _financeDictionaryDetailRepository.GetAll()
                                                                                              on t.DeviceStatus equals p.Id into temp
                                                                                              from p in temp.DefaultIfEmpty()
                                                                                              select new ProductionEquipmentCostModel
                                                                                              {
                                                                                                  Id = t.Id,
                                                                                                  EquipmentName = t.EquipmentName,
                                                                                                  DeviceStatus = t.DeviceStatus,
                                                                                                  UnitPrice = t.UnitPrice,
                                                                                                  Number = t.Number,
                                                                                                  Cost = t.Cost / 100,
                                                                                                  DeviceStatusName = p != null ? p.DisplayName : ""
                                                                                              }).ToList();
                modify.ProductionEquipmentCost = productionEquipmentCostModelsjoinedList;
                modify.ProductionEquipmentCostTotal = modify.ProductionEquipmentCost.Sum(p => p.Cost);
                //实验费用
                {
                    //-产品部-电子工程师录入的试验费用
                    List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                    //-品保部录入的实验费用
                    List<EnvironmentalExperimentFee> qADepartmentTests = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                    modify.LaboratoryFeeModels = ObjectMapper.Map<List<LaboratoryFeeModel>>(laboratoryFees);
                    modify.LaboratoryFeeModels.AddRange(ObjectMapper.Map<List<LaboratoryFeeModel>>(qADepartmentTests));
                    modify.LaboratoryFeeModelsTotal = modify.LaboratoryFeeModels.Sum(p => p.AllCost);
                }
                //测试软件费用                 
                //测试软件费用=>硬件费用               
                List<ProcessHoursEnterFrock> processHoursEnterFrocks = (from a in processHours
                                                                        join b in await _processHoursEnterFrock.GetAllListAsync() on a.Id equals b.ProcessHoursEnterId
                                                                        select new ProcessHoursEnterFrock
                                                                        {
                                                                            Id = b.Id,
                                                                            HardwareDeviceName = b.HardwareDeviceName,
                                                                            HardwareDeviceNumber = b.HardwareDeviceNumber,
                                                                            HardwareDevicePrice = b.HardwareDevicePrice,
                                                                            ProcessHoursEnterId = b.ProcessHoursEnterId
                                                                        }).ToList();
                List<SoftwareTestingCotsModel> softwareTestingCots = new List<SoftwareTestingCotsModel>() { { new SoftwareTestingCotsModel() { Id = processHours.FirstOrDefault().Id, SoftwareProject = "硬件费用", Count = (int)processHoursEnterFrocks.Sum(p => p.HardwareDeviceNumber), Cost = (decimal)processHoursEnterFrocks.Sum(p => p.HardwareDevicePrice) } } };
                modify.SoftwareTestingCost = softwareTestingCots;
                //测试软件费用=>追溯软件费用
                modify.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { Id = processHours.FirstOrDefault().Id + 1, SoftwareProject = "追溯软件费用", Cost = processHours.Sum(p => p.TraceabilitySoftwareCost) });
                //测试软件费用=>开图软件费用
                modify.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { Id = processHours.FirstOrDefault().Id + 2, SoftwareProject = "开图软件费用", Cost = processHours.Sum(p => p.SoftwarePrice) });
                modify.SoftwareTestingCostTotal = modify.SoftwareTestingCost.Sum(p => p.Cost);
                //差旅费
                List<TravelExpenseModel> travelExpenses = _resourceTravelExpense.GetAll().Where(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId))
                    .Join(_financeDictionaryDetailRepository.GetAll(), t => t.ReasonsId, p => p.Id, (t, p) => new TravelExpenseModel
                    {
                        Id = t.Id,
                        ReasonsId = t.ReasonsId,
                        ReasonsName = p.DisplayName,
                        PeopleCount = t.PeopleCount,
                        CostSky = t.CostSky,
                        SkyCount = t.SkyCount,
                        Cost = t.Cost,
                        Remark = t.Remark,
                    }).ToList();
                modify.TravelExpense = travelExpenses;
                modify.TravelExpenseTotal = modify.TravelExpense.Sum(p => p.Cost);
                //其他费用
                List<RestsCost> rests = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                modify.RestsCost = ObjectMapper.Map<List<RestsCostModel>>(rests);
                modify.RestsCostTotal = modify.RestsCost.Sum(p => p.Cost);
                //(不含税人民币) NRE 总费用
                modify.RMBAllCost = modify.HandPieceCost.Sum(p => p.Cost)//手板件总费用
                                         + modify.MouldInventory.Sum(p => p.Cost)//模具清单总费用
                                         + modify.ToolingCost.Sum(p => p.Cost)//工装费用总费用
                                         + modify.FixtureCost.Sum(p => p.Cost)//治具费用总费用
                                         + modify.QAQCDepartments.Sum(p => p.Cost)//检具费用总费用
                                         + modify.ProductionEquipmentCost.Sum(p => p.Cost)//生产设备总费用
                                         + modify.LaboratoryFeeModels.Sum(p => p.AllCost)//实验费用总费用
                                         + modify.SoftwareTestingCost.Sum(p => p.Cost)//测试软件总费用
                                         + modify.TravelExpense.Sum(p => p.Cost)//差旅费总费用
                                         + modify.RestsCost.Sum(p => p.Cost);//其他费用总费用
                int year = await GetYear(auditFlowId);
                //获取汇率
                ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals("USD"));
                List<YearOrValueMode> yearOrValueModes = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                YearOrValueMode exchangeRateModel = new();
                if (yearOrValueModes.Count is not 0) exchangeRateModel = yearOrValueModes.FirstOrDefault(p => p.Year.Equals(year));
                //(不含税美金) NRE 总费用
                modify.USDAllCost = 0.0M;
                if (exchangeRateModel is not null)
                {
                    modify.USDAllCost = modify.RMBAllCost / exchangeRateModel.Value;
                }
                else
                {
                    modify.USDAllCost = modify.RMBAllCost;
                }
                //手板件费用修改项
                modify.HandPieceCostModifyDtos = ObjectMapper.Map<List<HandPieceCostModifyDto>>(_handPieceCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //模具费用修改项
                modify.MouldInventoryModifyDtos = ObjectMapper.Map<List<MouldInventoryModifyDto>>(_mouldInventoryModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //工装费用修改项
                modify.ToolingCostsModifyDtos = ObjectMapper.Map<List<ToolingCostsModifyDto>>(_toolingCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //治具费用修改项
                modify.FixtureCostsModifyDtos = ObjectMapper.Map<List<FixtureCostsModifyDto>>(_fixtureCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //检具费用修改项
                modify.InspectionToolCostModifyDtos = ObjectMapper.Map<List<InspectionToolCostModifyDto>>(_inspectionToolCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //生产设备费用修改项
                modify.ProductionEquipmentCostsModifyDtos = ObjectMapper.Map<List<ProductionEquipmentCostsModifyDto>>(_productionEquipmentCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //实验费用修改项
                modify.ExperimentalExpensesModifyDtos = ObjectMapper.Map<List<ExperimentalExpensesModifyDto>>(_experimentalExpensesModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                //测试软件费用修改项
                modify.TestingSoftwareCostsModifyDtos = ObjectMapper.Map<List<TestingSoftwareCostsModifyDto>>(_testingSoftwareCostsModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                // 差旅费用修改项
                modify.TravelExpenseModifyDtos = ObjectMapper.Map<List<TravelExpenseModifyDto>>(_travelExpenseModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));
                // 其他费用修改项
                modify.RestsCostModifyDtos = ObjectMapper.Map<List<RestsCostModifyDto>>(_restsCostModify.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId)));

                return modify;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 根据ID从字典表里取值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal async Task<string> GetDisplayName(string Id)
        {
            FinanceDictionaryDetail prop = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id.Equals(Id));
            return prop?.DisplayName;
        }
        /// <summary>
        /// 手板件费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AdditionOfCostModificationItemsForHandBoards(HandPieceCostModifyDto handPieceCostModifyDto)
        {
            HandPieceCostModify handPieceCostModify = ObjectMapper.Map<HandPieceCostModify>(handPieceCostModifyDto);
            await _handPieceCostModify.BulkInsertOrUpdateAsync(handPieceCostModify);
        }
        /// <summary>
        /// 手板件费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelitionOfCostModificationItemsForHandBoards(long Id)
        {                         
            await _handPieceCostModify.HardDeleteAsync(p=>p.Id.Equals(Id));
        }
        /// <summary>
        /// 模具费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddMoldCostModificationItem(MouldInventoryModifyDto mouldInventoryModifyDto)
        {
            MouldInventoryModify mouldInventoryModify = ObjectMapper.Map<MouldInventoryModify>(mouldInventoryModifyDto);
            await _mouldInventoryModify.BulkInsertOrUpdateAsync(mouldInventoryModify);
        }
        /// <summary>
        /// 模具费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelMoldCostModificationItem(long Id)
        {            
            await _mouldInventoryModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 工装费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddToolingCostModificationItem(ToolingCostsModifyDto toolingCostsModifyDto)
        {
            ToolingCostsModify toolingCostsModify = ObjectMapper.Map<ToolingCostsModify>(toolingCostsModifyDto);
            await _toolingCostsModify.BulkInsertOrUpdateAsync(toolingCostsModify);
        }
        /// <summary>
        /// 工装费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelToolingCostModificationItem(long Id)
        {          
            await _toolingCostsModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 治具费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AdditionOfFixtureCostModificationItem(FixtureCostsModifyDto fixtureCostsModifyDto)
        {
            FixtureCostsModify fixtureCostsModify = ObjectMapper.Map<FixtureCostsModify>(fixtureCostsModifyDto);
            await _fixtureCostsModify.BulkInsertOrUpdateAsync(fixtureCostsModify);
        }
        /// <summary>
        /// 治具费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelitionOfFixtureCostModificationItem(long Id)
        {         
            await _fixtureCostsModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 检具费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddInspectionToolCostModificationItem(InspectionToolCostModifyDto inspectionToolCostModifyDto)
        {
            InspectionToolCostModify inspectionToolCostModify = ObjectMapper.Map<InspectionToolCostModify>(inspectionToolCostModifyDto);
            await _inspectionToolCostModify.BulkInsertOrUpdateAsync(inspectionToolCostModify);
        }
        /// <summary>
        /// 检具费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelInspectionToolCostModificationItem(long Id)
        {           
            await _inspectionToolCostModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 生产设备费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddProductionEquipmentCostModificationItem(ProductionEquipmentCostsModifyDto productionEquipmentCostsModifyDto)
        {
            ProductionEquipmentCostsModify productionEquipmentCostsModify = ObjectMapper.Map<ProductionEquipmentCostsModify>(productionEquipmentCostsModifyDto);
            await _productionEquipmentCostsModify.BulkInsertOrUpdateAsync(productionEquipmentCostsModify);
        }
        /// <summary>
        /// 生产设备费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelProductionEquipmentCostModificationItem(long Id)
        {         
            await _productionEquipmentCostsModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 实验费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddExperimentalFeeModificationItem(ExperimentalExpensesModifyDto experimentalExpensesModifyDto)
        {
            ExperimentalExpensesModify experimentalExpensesModify = ObjectMapper.Map<ExperimentalExpensesModify>(experimentalExpensesModifyDto);
            await _experimentalExpensesModify.BulkInsertOrUpdateAsync(experimentalExpensesModify);
        }
        /// <summary>
        /// 实验费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelExperimentalFeeModificationItem(long Id)
        {            
            await _experimentalExpensesModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 测试软件费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddingModificationItemsForTestingSoftwareCosts(TestingSoftwareCostsModifyDto testingSoftwareCostsModifyDto)
        {
            TestingSoftwareCostsModify testingSoftwareCostsModify = ObjectMapper.Map<TestingSoftwareCostsModify>(testingSoftwareCostsModifyDto);
            await _testingSoftwareCostsModify.BulkInsertOrUpdateAsync(testingSoftwareCostsModify);
        }
        /// <summary>
        /// 测试软件费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelingModificationItemsForTestingSoftwareCosts(long Id)
        {         
            await _testingSoftwareCostsModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 差旅费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task AddTravelExpenseModificationItem(TravelExpenseModifyDto travelExpenseModifyDto)
        {
            TravelExpenseModify travelExpenseModify = ObjectMapper.Map<TravelExpenseModify>(travelExpenseModifyDto);
            await _travelExpenseModify.BulkInsertOrUpdateAsync(travelExpenseModify);
        }
        /// <summary>
        /// 差旅费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelTravelExpenseModificationItem(long Id)
        {            
            await _travelExpenseModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// 其他费用修改项添加
        /// </summary>
        /// <returns></returns>
        public async Task OtherExpenseModificationItemsAdded(RestsCostModifyDto restsCostModifyDto)
        {
            RestsCostModify restsCostModify = ObjectMapper.Map<RestsCostModify>(restsCostModifyDto);
            await _restsCostModify.BulkInsertOrUpdateAsync(restsCostModify);
        }
        /// <summary>
        /// 其他费用修改项删除
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task DelOtherExpenseModificationItemsAdded(long Id)
        {            
            await _restsCostModify.HardDeleteAsync(p => p.Id.Equals(Id));
        }
        /// <summary>
        /// NRE审核接口
        /// </summary>
        /// <returns></returns>
        public async Task NREToExamine(NREToExamineToExamineDto toExamineDto)
        {
            //模具费审核 并且拒绝
            if (toExamineDto.NreCheckType == NRECHECKTYPE.MoldCost && toExamineDto.Opinion == FinanceConsts.YesOrNo_No)
            {
                //重置状态
                await GetResourcesManagementConfigurationState(toExamineDto.NreId);
            }
            //环境实验费审核 并且拒绝
            if (toExamineDto.NreCheckType == NRECHECKTYPE.EnvironmentalTestingFee && toExamineDto.Opinion == FinanceConsts.YesOrNo_No)
            {
                //重置状态
                await GetExperimentItemsConfigurationState(toExamineDto.AuditFlowId);
            }
            //EMC试验费审核 并且拒绝
            if (toExamineDto.NreCheckType == NRECHECKTYPE.EMCTestFee && toExamineDto.Opinion == FinanceConsts.YesOrNo_No)
            {
                //重置状态
                await GetProductDepartmentConfigurationState(toExamineDto.AuditFlowId);
            }
            //嵌入工作流
            await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
            {
                NodeInstanceId = toExamineDto.NodeInstanceId,
                FinanceDictionaryDetailId = toExamineDto.Opinion,
                Comment = toExamineDto.Comment,
            });
        }
    }
}
