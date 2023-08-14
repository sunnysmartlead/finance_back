﻿using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.Nre;
using Finance.NrePricing;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Method;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        /// Nre 产品部-电子工程师 实验费 实体类
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
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
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
        private static IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        private readonly IRepository<User, long> _userRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceModelCount"></param>
        /// <param name="resourceElectronicStructuralMethod"></param>
        /// <param name="resourceHandPieceCost"></param>
        /// <param name="resourceRestsCost"></param>
        /// <param name="resourceTravelExpense"></param>
        /// <param name="resourceMouldInventory"></param>
        /// <param name="resourceLaboratoryFee"></param>
        /// <param name="resourceNrePricingMethod"></param>
        /// <param name="resourceQADepartmentTest"></param>
        /// <param name="resourceQADepartmentQC"></param>
        /// <param name="resourceInitialResourcesManagement"></param>
        /// <param name="resourceEquipmentInfo"></param>
        /// <param name="resourceTraceInfo"></param>
        /// <param name="resourceWorkingHoursInfo"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        /// <param name="resourcePriceEvaluation"></param>
        /// <param name="flowAppService"></param>
        /// <param name="resourceNreIsSubmit"></param>
        /// <param name="resourceModelCountYear"></param>
        /// <param name="structBomDifferent"></param>
        /// <param name="user"></param>
        public NrePricingAppService(IRepository<ModelCount, long> resourceModelCount,
            ElectronicStructuralMethod resourceElectronicStructuralMethod,
            IRepository<HandPieceCost, long> resourceHandPieceCost,
            IRepository<RestsCost, long> resourceRestsCost,
            IRepository<TravelExpense, long> resourceTravelExpense,
            IRepository<MouldInventory, long> resourceMouldInventory,
            IRepository<LaboratoryFee, long> resourceLaboratoryFee,
            NrePricingMethod resourceNrePricingMethod,
            IRepository<EnvironmentalExperimentFee, long> resourceQADepartmentTest,
            IRepository<QADepartmentQC, long> resourceQADepartmentQC,
            IRepository<InitialResourcesManagement, long> resourceInitialResourcesManagement,
            IRepository<EquipmentInfo, long> resourceEquipmentInfo,
            IRepository<TraceInfo, long> resourceTraceInfo,
            IRepository<WorkingHoursInfo, long> resourceWorkingHoursInfo,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
            IRepository<PriceEvaluation, long> resourcePriceEvaluation, AuditFlowAppService flowAppService,
            IRepository<NreIsSubmit, long> resourceNreIsSubmit,
            IRepository<ModelCountYear, long> resourceModelCountYear,
            IRepository<StructBomDifferent, long> structBomDifferent,
            IRepository<User, long> user)
        {
            _resourceModelCount = resourceModelCount;
            _resourceElectronicStructuralMethod = resourceElectronicStructuralMethod;
            _resourceHandPieceCost = resourceHandPieceCost;
            _resourceRestsCost = resourceRestsCost;
            _resourceTravelExpense = resourceTravelExpense;
            _resourceMouldInventory = resourceMouldInventory;
            _resourceLaboratoryFee = resourceLaboratoryFee;
            _resourceNrePricingMethod = resourceNrePricingMethod;
            _resourceEnvironmentalExperimentFee = resourceQADepartmentTest;
            _resourceQADepartmentQC = resourceQADepartmentQC;
            _resourceInitialResourcesManagement = resourceInitialResourcesManagement;
            _resourceEquipmentInfo = resourceEquipmentInfo;
            _resourceTraceInfo = resourceTraceInfo;
            _resourceWorkingHoursInfo = resourceWorkingHoursInfo;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _resourcePriceEvaluation = resourcePriceEvaluation;
            _flowAppService = flowAppService;
            _resourceNreIsSubmit = resourceNreIsSubmit;
            _resourceModelCountYear = resourceModelCountYear;
            _configStructBomDifferent = structBomDifferent;
            _userRepository = user;
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
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId, Func<SolutionTable, bool> filter)
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
                //await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = projectManagementModel.SolutionId, EnumSole = NreIsSubmitDto.ProjectManagement.ToString() });
                #endregion
                //if (await this.GetProjectManagement(price.AuditFlowId))
                //{
                //    if (AbpSession.UserId is null)
                //    {
                //        throw new FriendlyException("请先登录");
                //    }
                //    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                //    {
                //        AuditFlowId = price.AuditFlowId,
                //        ProcessIdentifier = AuditFlowConsts.AF_NreInputOther,
                //        UserId = AbpSession.UserId.Value,
                //        Opinion = OPINIONTYPE.Submit_Agreee
                //    });
                //}
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
        /// 资源部模具费初始值(单个方案)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<MouldInventoryPartModel> GetInitialResourcesManagementSingle(long auditFlowId, long solutionId)
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
        public async Task PostResourcesManagementSingle(ResourcesManagementSingleDto price)        {

            ResourcesManagementModel resourcesManagementModel = new();
            resourcesManagementModel = price.ResourcesManagementModels;          
            MouldInventory MouldInventorys = ObjectMapper.Map<MouldInventory>(resourcesManagementModel.MouldInventory);              
            MouldInventorys.AuditFlowId = price.AuditFlowId;
            MouldInventorys.SolutionId = resourcesManagementModel.SolutionId;
            MouldInventorys.PeopleId = AbpSession.GetUserId();//提交人ID
            await _resourceMouldInventory.InsertOrUpdateAsync(MouldInventorys);//录入模具清单            
            #region 方案页面录入完成之后
            MouldInventoryPartModel mouldInventoryPartModel= await GetInitialResourcesManagementSingle(price.AuditFlowId, resourcesManagementModel.SolutionId);          
            long count = await _resourceMouldInventory.CountAsync(p => p.IsSubmit) + (MouldInventorys.IsSubmit?1 : 0);
            if (mouldInventoryPartModel.MouldInventoryModels.Count == count) {
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = resourcesManagementModel.SolutionId, EnumSole = NreIsSubmitDto.ResourcesManagement.ToString() });
                if (await this.GetResourcesManagement(price.AuditFlowId))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }

                }
            } 
            #endregion
           

        }
        /// <summary>
        /// 资源部录入  判断是否全部提交完毕  true 所有方案已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetResourcesManagement(long auditFlowId)
        {           
            //获取 总共的方案
            List<SolutionModel> partModels = await TotalSolution(auditFlowId);
            int AllCount = partModels.Count();
            //获取 已经提交的方案
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString())) + 1;
            return AllCount == Count; 
        }
        /// <summary>
        /// 资源部模具费录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        internal async Task GetResourcesManagementConfigurationState(long auditFlowId)
        {
            await _resourceNreIsSubmit.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));          
            List<MouldInventory> prop= await _resourceMouldInventory.GetAllListAsync(p=>p.AuditFlowId.Equals(auditFlowId));
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
        /// 产品部-电子工程师入  退回重置状态
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
        /// 产品部-电子工程师 录入(单个方案)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProductDepartmentSingle(ProductDepartmentSingleDto price)
        {
            ProductDepartmentModel productDepartmentModel = new();
            productDepartmentModel = price.ProductDepartmentModels;
            //判断 该方案 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(productDepartmentModel.SolutionId) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该方案方案id已经提交过了");
            }
            try
            {

                List<LaboratoryFee> laboratoryFees = ObjectMapper.Map<List<LaboratoryFee>>(productDepartmentModel.laboratoryFeeModels);
                //删除原数据
                await _resourceLaboratoryFee.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.SolutionId.Equals(productDepartmentModel.SolutionId));
                foreach (LaboratoryFee laboratoryFee in laboratoryFees)
                {
                    laboratoryFee.AuditFlowId = price.AuditFlowId;
                    laboratoryFee.SolutionId = productDepartmentModel.SolutionId;
                    await _resourceLaboratoryFee.InsertOrUpdateAsync(laboratoryFee);
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, SolutionId = productDepartmentModel.SolutionId, EnumSole = NreIsSubmitDto.ProductDepartment.ToString() });
                #endregion
                if (await this.GetProductDepartment(price.AuditFlowId))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }

                }
                if (price.IsSubmit)
                {
                    //流程流转
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
        public IActionResult PostProductDepartmentDownloadExcel(string FileName = "NRE产品部EMC+电性能实验费模版下载")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE产品部EMC+电性能实验费模版.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }




        /// <summary>
        /// Nre 产品部-电子工程师 导入数据(不提交)(Excel 单个方案解析数据)
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
                    List<LaboratoryFeeModel> rows = ObjectMapper.Map<List<LaboratoryFeeModel>>(rowExcls);
                    return rows;
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  产品部-电子工程师  录入过的值(单个方案)
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
        ///  Nre 产品部EMC 导出数据(传数据)
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
                    { "试验项目", item.TestItem },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.Coefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.DataThoroughly },
                    { "计数-DV", item.DataDV },
                    { "计数-PV", item.DataPV },
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
        ///  Nre 产品部=>EMC实验费 导出数据(不传数据)
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
                    { "试验项目", item.TestItem },
                    { "是否指定第三方", item.IsThirdParty },
                    { "单价", item.UnitPrice },
                    { "调整系数", item.Coefficient },
                    { "计价单位", item.Unit },
                    { "计数-摸底", item.DataThoroughly },
                    { "计数-DV", item.DataDV },
                    { "计数-PV", item.DataPV },
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
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = experimentItems.AuditFlowId, SolutionId = experimentItems.SolutionId, EnumSole = NreIsSubmitDto.EnvironmentalExperimentFee.ToString() });
                    #endregion
                    if (await this.GetExperimentItems(experimentItems.AuditFlowId))
                    {
                        if (AbpSession.UserId is null)
                        {
                            throw new FriendlyException("请先登录");
                        }
                        #region 流程流转
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
        public IActionResult PostExperimentItemsSingleDownloadExcel(string FileName = "NRE环境实验费模版下载")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE环境实验费模版.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
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
                    List<EnvironmentalExperimentFeeModel> rows = ObjectMapper.Map<List<EnvironmentalExperimentFeeModel>>(rowExcls);
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
        public async Task<IActionResult> GetExportOfEnvironmentalExperimentFeeForm(long auditFlowId, long solutionId)
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
        /// <param name="Id">流程表Id</param>
        /// <returns></returns>
        public async Task<List<ReturnSalesDepartmentDto>> GetInitialSalesDepartment(long Id)
        {

            List<ReturnSalesDepartmentDto> initialSalesDepartmentDtos = new();
            ReturnSalesDepartmentDto initialSalesDepartmentDto = new();
            List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //手板件费            
            List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += handPieceCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.SBJF;//这里就是写死的,因为手板费是一个表,我要从指定的表获取 费用的总和,这个不能配置             
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //模具费
            initialSalesDepartmentDto = new();
            List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += mouldInventories.Where(p => p.SolutionId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.MJF;
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //生产设备费  
            initialSalesDepartmentDto = new();
            initialSalesDepartmentDto.FormName = StaticName.SCSBF;
            List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id))
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
            List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
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
            List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.JJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += qADepartmentQCs.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Count * s.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //实验费
            initialSalesDepartmentDto = new();
            List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            List<EnvironmentalExperimentFee> qADepartmentTests = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.SYF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += laboratoryFees.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
                initialSalesDepartmentDto.PricingMoney += qADepartmentTests.Where(p => p.SolutionId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //测试软件费  (硬件费用+追溯软件费用+开图软件费用)
            initialSalesDepartmentDto = new();
            List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.CSRJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += workingHoursInfos.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.HardwareTotalPrice + s.TraceabilityDevelopmentFee + s.MappingDevelopmentFee)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //差旅费
            initialSalesDepartmentDto = new();
            List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.CLF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += travelExpenses.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //其他费用
            initialSalesDepartmentDto = new();
            List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.QTFY;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += restsCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            List<ReturnSalesDepartmentDto> returnSalesDepartmentDtos = await GetReturnInitialSalesDepartment(Id);
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
        /// <param name="processId"></param>
        /// <returns></returns>
        private async Task<int> GetYear(long processId)
        {
            List<ModelCountYear> modelCountYears = await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
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
        ///获取 Nre 核价表
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<PricingFormDto> GetPricingForm(long auditFlowId, long solutionId)
        {
            try
            {
                PriceEvaluation priceEvaluation = await _resourcePriceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
                List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                PricingFormDto pricingFormDto = new();
                if (priceEvaluation is not null)
                {
                    pricingFormDto.ProjectName = priceEvaluation.ProjectName;
                    pricingFormDto.ClientName = priceEvaluation.CustomerName;
                }
                //获取合计属性
                pricingFormDto.RequiredCapacity = modelCount.Sum(p => p.ModelTotal).ToString();
                //手板件费用
                List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                pricingFormDto.HandPieceCost = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                //模具费用
                List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                pricingFormDto.MouldInventory = ObjectMapper.Map<List<MouldInventoryModel>>(mouldInventories);
                //工装费用 (工装费用+测试线费用)              
                List<ToolingCostModel> workingHoursInfosGZ = new();
                //工装费用=>工装费用
                List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                workingHoursInfosGZ = workingHours.Where(p => p.ToolingName is not null).GroupBy(m => new { m.ToolingName, m.ToolingPrice }).Select(a => new ToolingCostModel
                {
                    WorkName = a.Key.ToolingName,
                    UnitPriceOfTooling = a.Key.ToolingPrice,
                    ToolingCount = a.Sum(m => m.ToolingNum),
                    Cost = a.Key.ToolingPrice * a.Sum(m => m.ToolingNum),
                }).ToList();
                pricingFormDto.ToolingCost = workingHoursInfosGZ;
                //工装费用=>测试线费用               
                List<ToolingCostModel> workingHoursInfosCSX = workingHours.Where(p => p.TestName is not null).GroupBy(m => new { m.TestName, m.TestPrice }).Select(a => new ToolingCostModel
                {
                    WorkName = a.Key.TestName,
                    UnitPriceOfTooling = a.Key.TestPrice,
                    ToolingCount = a.Sum(m => m.TestNum),
                    Cost = a.Key.TestPrice * a.Sum(m => m.TestNum),
                }).ToList();
                pricingFormDto.ToolingCost.AddRange(workingHoursInfosCSX);
                //治具费用               
                List<EquipmentInfo> equipmentInfosZj = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId))
                                                        join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Fixture)) on a.Id equals b.WorkHoursId
                                                        select new EquipmentInfo
                                                        {
                                                            WorkHoursId = b.WorkHoursId,
                                                            Part = b.Part,
                                                            EquipmentName = b.EquipmentName,
                                                            Status = b.Status,
                                                            Number = b.Number,
                                                            UnitPrice = b.UnitPrice,
                                                        }).ToList();
                List<FixtureCostModel> productionEquipmentCostModelsZj = equipmentInfosZj.GroupBy(m => new { m.EquipmentName, m.UnitPrice }).Select(
                     a => new FixtureCostModel
                     {
                         ToolingName = a.Key.EquipmentName,
                         UnitPrice = a.Key.UnitPrice,
                         Number = a.Sum(c => c.Number),
                         Cost = a.Key.UnitPrice * a.Sum(c => c.Number),
                     }).ToList();
                pricingFormDto.FixtureCost = productionEquipmentCostModelsZj;
                //检具费用(有变化,工装治具)
                List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                pricingFormDto.QAQCDepartments = ObjectMapper.Map<List<QADepartmentQCModel>>(qADepartmentQCs);
                //生产设备费用 
                List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId))
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
                List<ProductionEquipmentCostModel> productionEquipmentCostModels = equipmentInfos.GroupBy(m => new { m.EquipmentName, m.UnitPrice }).Select(
                    a => new ProductionEquipmentCostModel
                    {
                        EquipmentName = a.Key.EquipmentName,
                        UnitPrice = a.Key.UnitPrice,
                        Number = a.Sum(c => c.Number),
                        Cost = a.Key.UnitPrice * a.Sum(c => c.Number),
                    }).ToList();
                pricingFormDto.ProductionEquipmentCost = productionEquipmentCostModels;
                //实验费用
                {
                    //-产品部-电子工程师录入的试验费用
                    List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                    //-品保部录入的实验费用
                    List<EnvironmentalExperimentFee> qADepartmentTests = await _resourceEnvironmentalExperimentFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                    pricingFormDto.LaboratoryFeeModels = ObjectMapper.Map<List<LaboratoryFeeModel>>(laboratoryFees);
                    pricingFormDto.LaboratoryFeeModels.AddRange(ObjectMapper.Map<List<LaboratoryFeeModel>>(qADepartmentTests));
                }
                //测试软件费用
                List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                //测试软件费用=>硬件费用
                List<SoftwareTestingCotsModel> softwareTestingCots = new List<SoftwareTestingCotsModel>() { { new SoftwareTestingCotsModel() { SoftwareProject = "硬件费用", Cost = workingHoursInfos.Sum(p => p.HardwareTotalPrice) } } };
                pricingFormDto.SoftwareTestingCost = softwareTestingCots;
                //测试软件费用=>追溯软件费用
                pricingFormDto.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { SoftwareProject = "追溯软件费用", Cost = workingHoursInfos.Sum(p => p.TraceabilityDevelopmentFee) });
                //测试软件费用=>开图软件费用
                pricingFormDto.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { SoftwareProject = "开图软件费用", Cost = workingHoursInfos.Sum(p => p.MappingDevelopmentFee) });
                //差旅费
                List<TravelExpenseModel> travelExpenses = _resourceTravelExpense.GetAll().Where(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId))
                    .Join(_financeDictionaryDetailRepository.GetAll(), t => t.ReasonsId, p => p.Id, (t, p) => new TravelExpenseModel
                    {
                        ReasonsId = t.ReasonsId,
                        ReasonsName = p.DisplayName,
                        PeopleCount = t.PeopleCount,
                        CostSky = t.CostSky,
                        SkyCount = t.SkyCount,
                        Cost = t.Cost,
                        Remark = t.Remark,
                    }).ToList();
                pricingFormDto.TravelExpense = travelExpenses;
                //其他费用
                List<RestsCost> rests = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(solutionId));
                pricingFormDto.RestsCost = ObjectMapper.Map<List<RestsCostModel>>(rests);
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
    }
}
