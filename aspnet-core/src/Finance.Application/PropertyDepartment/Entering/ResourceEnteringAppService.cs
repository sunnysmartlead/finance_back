using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.DemandApplyAudit;
using Finance.Entering.Model;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.ProductDevelopment.Dto;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.Entering
{
    /// <summary>
    /// 资源部录入API
    /// </summary>
    [AbpAuthorize]
    public class ResourceEnteringAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 是否涉及选项
        /// </summary>
        private static string IsInvolveItem = "是";
        internal readonly IRepository<ModelCount, long> _resourceModelCount;
        internal readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        internal readonly IRepository<FinanceDictionaryDetail, string> _resourceFinanceDictionaryDetail;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        internal readonly IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;

        /// <summary>
        /// 产品开发部结构BOM输入信息
        /// </summary>
        private static IRepository<StructureBomInfo, long> _resourceStructureBomInfo;
        internal readonly ElectronicStructuralMethod _resourceElectronicStructuralMethod;
        /// <summary>
        /// 资源部电子物料录入
        /// </summary>
        internal static IRepository<EnteringElectronic, long> _configEnteringElectronic;
        /// <summary>
        /// 资源部结构物料录入
        /// </summary>
        internal static IRepository<StructureElectronic, long> _configStructureElectronic;
        /// <summary>
        /// 资源部电子物料录入复制项
        /// </summary>
        internal static IRepository<EnteringElectronicCopy, long> _configEnteringElectronicCopy;
        /// <summary>
        /// 资源部结构物料录入复制项
        /// </summary>
        internal static IRepository<StructureElectronicCopy, long> _configStructureElectronicCopy;
        /// <summary>
        /// 电子BOM两次上传差异化表
        /// </summary>
        internal static IRepository<ElecBomDifferent, long> _configElecBomDifferent;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        internal static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        /// <summary>
        /// 流程流转服务
        /// </summary>
        internal readonly AuditFlowAppService _flowAppService;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<User, long> _userRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceEnteringAppService(IRepository<ModelCount, long> modelCount,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            ElectronicStructuralMethod electronicStructuralMethod,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            IRepository<StructureBomInfo, long> structureBomInfo,
            AuditFlowAppService flowAppService,
            IRepository<EnteringElectronic, long> enteringElectronic,
            IRepository<StructureElectronic, long> structureElectronic,
             IRepository<EnteringElectronicCopy, long> enteringElectronicCopy,
            IRepository<StructureElectronicCopy, long> structureElectronicCopy,
            IRepository<ElecBomDifferent, long> elecBomDifferent,
            IRepository<StructBomDifferent, long> structBomDifferent,
            WorkflowInstanceAppService workflowInstanceAppService,
            IRepository<User, long> user)
        {
            _resourceModelCount = modelCount;
            _resourceModelCountYear = modelCountYear;
            _resourceFinanceDictionaryDetail = financeDictionaryDetail;
            _resourceElectronicStructuralMethod = electronicStructuralMethod;
            _resourceElectronicBomInfo = electronicBomInfo;
            _resourceStructureBomInfo = structureBomInfo;
            _flowAppService = flowAppService;
            _configEnteringElectronic = enteringElectronic;
            _configStructureElectronic = structureElectronic;
            _configEnteringElectronicCopy = enteringElectronicCopy;
            _configStructureElectronicCopy = structureElectronicCopy;
            _configElecBomDifferent = elecBomDifferent;
            _configStructBomDifferent = structBomDifferent;
            _workflowInstanceAppService = workflowInstanceAppService;
            _userRepository = user;
        }

        /// <summary>
        /// 资源部输入时,电子料初始值数量
        /// </summary>
        /// <param name="AuditFlowId">流程表主键</param>
        /// <returns></returns>         

        internal async Task<int> InitialValueQuantityOfElectronicMaterials(long AuditFlowId)
        {
            //总共的方案
            List<SolutionModel> solutionList = await _resourceElectronicStructuralMethod.TotalSolution(AuditFlowId);
            InitialElectronicDto initialElectronicDto = new();
            //从字典明细表取 零件名称和id
            initialElectronicDto.PartDtoList = solutionList;
            initialElectronicDto.ElectronicBomList = await _resourceElectronicStructuralMethod.ElectronicBomListCount(solutionList, AuditFlowId);// 电子BOM单价表单           
            #region 电子BOM退回差异处理
            foreach (var item in solutionList)//循环所有方案
            {
                List<ElecBomDifferent> elecBomDifferents = await _configElecBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.SolutionId.Equals(item.SolutionId));
                if (elecBomDifferents.Count is not 0)//有差异
                {
                    foreach (ElecBomDifferent elecBom in elecBomDifferents)
                    {
                        //判断差异类型
                        if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configEnteringElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(elecBom.ElectronicId));
                            //2.删除返回数据库中的数据
                            ElectronicDto electronicDto = initialElectronicDto.ElectronicBomList.Where(p => p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                            initialElectronicDto.ElectronicBomList.Remove(electronicDto);
                        }
                        else if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            //重新加载项目使用量和系统单价
                            ElectronicDto electronicDto = initialElectronicDto.ElectronicBomList.Where(p => p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                            //索引
                            if (electronicDto is not null)
                            {
                                int index = initialElectronicDto.ElectronicBomList.FindIndex(p => p.ElectronicId.Equals(elecBom.ElectronicId));
                                initialElectronicDto.ElectronicBomList[index] = electronicDto;
                            }
                        }
                    }
                }
            }
            #endregion
            return initialElectronicDto.ElectronicBomList.Count;
        }
        /// <summary>
        /// 资源部输入时,加载电子料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<List<ElectronicDto>> GetElectronicSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                List<ElectronicDto> electronicDtos = new();
                //指定的方案          
                List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId, item => item.Id.Equals(solutionId));
                electronicDtos = await _resourceElectronicStructuralMethod.ElectronicBomList(partList, auditFlowId);// 电子BOM单价表单;
                #region 电子BOM退回差异处理
                foreach (SolutionModel item in partList)//循环所有方案          
                {
                    List<ElecBomDifferent> elecBomDifferents = await _configElecBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId));
                    if (elecBomDifferents.Count is not 0)//有差异
                    {
                        foreach (ElecBomDifferent elecBom in elecBomDifferents)
                        {
                            //判断差异类型
                            if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                            {
                                //删除存在数据库里的数据和返回数据中的数据即可
                                //1.删除数据库中的数据
                                await _configEnteringElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(elecBom.ElectronicId));
                                //2.删除返回数据库中的数据
                                ElectronicDto electronicDto = electronicDtos.Where(p => p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                                electronicDtos.Remove(electronicDto);
                            }
                            else if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                            {
                                //重新加载项目使用量和系统单价
                                ElectronicDto electronicDto = electronicDtos.Where(p => p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();//索引
                                if (electronicDto is not null)
                                {
                                    int index = electronicDtos.FindIndex(p => p.ElectronicId.Equals(elecBom.ElectronicId));
                                    electronicDto = await _resourceElectronicStructuralMethod.ElectronicBom(item.SolutionId, item.ProductId, auditFlowId, elecBom.ElectronicId);
                                    electronicDtos[index] = electronicDto;
                                }
                            }
                        }
                    }
                }
                #endregion
                return electronicDtos;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        ///  BOM单价审核 加载电子料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<IsALLElectronicDto> GetBOMElectronicSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            IsALLElectronicDto isALLElectronicDto = new();
            //指定的方案
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId, item => item.Id.Equals(solutionId));
            isALLElectronicDto.ElectronicDtos = await _resourceElectronicStructuralMethod.BOMElectronicBomList(partList, auditFlowId);// 电子BOM单价表单          
            isALLElectronicDto.isAll = await this.GetElectronicIsAllEntering(auditFlowId);
            return isALLElectronicDto;
        }
        /// <summary>
        /// 电子料单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完(仅仅审核页面查询方法)
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> GetElectronicIsAllEntering(long auditFlowId)
        {
            int AllCount = await InitialValueQuantityOfElectronicMaterials(auditFlowId);//应该录入数据库这么多条数据         
            //总共的方案
            List<SolutionModel> solutionModel = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            int Count = 0;
            foreach (SolutionModel item in solutionModel)
            {
                Count += await _configEnteringElectronic.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsSubmit);//数据库实际提交的条数
            }
            return AllCount == Count;
        }
        /// <summary>
        /// 电子料单价录入确认/提交 有则添加无则修改
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        public async Task PostElectronicMaterialEntering(SubmitElectronicDto electronicDto)
        {
            if (electronicDto.IsSubmit)
            {
                await _resourceElectronicStructuralMethod.SubmitElectronicMaterialEntering(electronicDto);
                //判断是否全部提交
                if (await this.GetElectronicIsAllEntering(electronicDto.AuditFlowId, electronicDto))
                {
                    //嵌入工作流
                    await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                    {
                        NodeInstanceId = electronicDto.NodeInstanceId,
                        FinanceDictionaryDetailId = electronicDto.Opinion,
                        Comment = electronicDto.Comment,
                    });
                }
            }
            else
            {
                //电子料单价录入确认
                await _resourceElectronicStructuralMethod.ElectronicMaterialEntering(electronicDto);
            }

        }
        /// <summary>
        /// 电子料单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> GetElectronicIsAllEntering(long auditFlowId, SubmitElectronicDto electronicDto)
        {
            int AllCount = await InitialValueQuantityOfElectronicMaterials(auditFlowId);//应该录入数据库这么多条数据          
            //总共的方案
            List<SolutionModel> solutionModel = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            int Count = 0;
            foreach (SolutionModel item in solutionModel)
            {
                Count += await _configEnteringElectronic.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsSubmit);//数据库实际提交的条数
            }
            List<ElectronicDto> electronicDtos = electronicDto.ElectronicDtoList;
            List<EnteringElectronic> enteringElectronics = new();
            foreach (ElectronicDto item in electronicDtos)
            {
                EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.IsEntering && p.SolutionId.Equals(item.SolutionId) && p.ElectronicId.Equals(item.ElectronicId));
                if (enteringElectronic is not null) enteringElectronics.Add(enteringElectronic);
            }
            electronicDtos = electronicDtos.Where(a => enteringElectronics.Where(t => a.SolutionId == t.SolutionId && a.ElectronicId.Equals(t.ElectronicId)).Any() || a.IsEntering).ToList();
            Count += electronicDtos.Count();
            return AllCount == Count;
        }
        /// <summary>
        ///  电子料单价录入  退回重置状态
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task GetElectronicConfigurationState(long auditFlowId)
        {
            if (auditFlowId == 0) throw new FriendlyException("电子单价录入退回重置状态流程id不能为0");
            List<EnteringElectronic> enteringElectronics = await _configEnteringElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            foreach (EnteringElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configEnteringElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 电子料单价录入  退回重置状态 根据电子单价录入表id进行重置
        /// </summary>
        /// <param name="ElectronicId">电子单价录入表id</param>
        /// <returns></returns>
        internal async Task GetElectronicConfigurationStateCertain(List<long> ElectronicId)
        {
            if (ElectronicId is null || ElectronicId.Count==0) throw new FriendlyException("电子料单价录入  退回重置状态 根据电子单价录入表id进行重置,传值不能为空或者数量为0");
            List<EnteringElectronic> enteringElectronics = await _configEnteringElectronic.GetAllListAsync(p => ElectronicId.Contains(p.Id));
            foreach (EnteringElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configEnteringElectronic.UpdateAsync(item);
            }
        }     
        /// <summary>
        /// 资源部输入时,加载结构料初始值数量
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task<int> InitialValueOfLoadingStructuralMaterials([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId)
        {
            //总共的零件
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            //删除的结构BOMid
            List<long> structureBOMIdDeleted = new();
            //修改的结构BOMid
            List<long> structureBOMIdModify = new();
            #region 结构BOM退回差异处理
            foreach (var item in partList)//循环所有零件
            {
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configStructureElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(structBom.StructureId));
                            structureBOMIdDeleted.Add(structBom.StructureId);
                        }
                        else if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            structureBOMIdModify.Add(structBom.StructureId);
                        }
                    }
                }
            }
            #endregion
            InitialStructuralDto initialStructuralDto = new();
            initialStructuralDto.PartDtoList = partList;
            initialStructuralDto.ConstructionBomList = await _resourceElectronicStructuralMethod.ConstructionBomListCount(partList, auditFlowId, structureBOMIdDeleted, structureBOMIdModify);// 结构料BOM单价表单

            int AllCount = 0;//应该录入数据库这么多条数据
            initialStructuralDto.ConstructionBomList.ForEach(p => { AllCount += p.StructureMaterial.Count(); });
            return AllCount;
        }
        /// <summary>
        ///  资源部输入时,加载结构料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<List<ConstructionDto>> GetStructuralSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            //删除的结构BOMid
            List<long> structureBOMIdDeleted = new();
            //修改的结构BOMid
            List<long> structureBOMIdModify = new();
            //指定方案
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId, item => item.Id.Equals(solutionId));
            #region 结构BOM退回差异处理
            foreach (var item in partList)//循环所有方案
            {
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configStructureElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(structBom.StructureId));
                            structureBOMIdDeleted.Add(structBom.StructureId);
                        }
                        else if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            structureBOMIdModify.Add(structBom.StructureId);
                        }
                    }
                }
            }
            #endregion
            List<ConstructionDto> prop = await _resourceElectronicStructuralMethod.ConstructionBomList(partList, auditFlowId, structureBOMIdDeleted, structureBOMIdModify);// 结构料BOM单价表单           

            return prop;
        }
        /// <summary>
        ///  结构料BOM单价审核加载初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<IsALLConstructionDto> GetBOMStructuralSingle([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            IsALLConstructionDto isALLConstructionDto = new();
            isALLConstructionDto.isAll = await GetStructuralIsAllEntering(auditFlowId);
            //指定方案
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId, item => item.Id.Equals(solutionId));
            List<ConstructionDto> prop = await _resourceElectronicStructuralMethod.BOMConstructionBomList(partList, auditFlowId);// 结构料BOM单价审核
            isALLConstructionDto.ConstructionDtos = prop;
            return isALLConstructionDto;
        }
        /// <summary>
        /// 结构件单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完(仅仅加载结构料初始值(单个零件)可用)
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> GetStructuralIsAllEntering(long auditFlowId)
        {
            int AllCount = await InitialValueOfLoadingStructuralMaterials(auditFlowId);//应该录入数据库这么多条数据           
            //总共的零件
            List<SolutionModel> solutionModels = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            int Count = 0;
            foreach (var item in solutionModels)
            {
                Count += await _configStructureElectronic.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsSubmit);//数据库实际提交的条数
            }
            return AllCount == Count;
        }
        /// <summary>
        /// 结构件单价录入确认/提交 有则添加无则修改
        /// </summary>
        /// <param name="structuralMemberEnteringModel"></param>
        /// <returns></returns>        
        public async Task PostStructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEnteringModel)
        {
            if (structuralMemberEnteringModel.IsSubmit)
            {
                await _resourceElectronicStructuralMethod.SubmitStructuralMemberEntering(structuralMemberEnteringModel);
                //判断有没有全部提交完
                if (await this.GetStructuralIsAllEntering(structuralMemberEnteringModel.AuditFlowId, structuralMemberEnteringModel))
                {
                    //嵌入工作流
                    await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                    {
                        NodeInstanceId = structuralMemberEnteringModel.NodeInstanceId,
                        FinanceDictionaryDetailId = structuralMemberEnteringModel.Opinion,
                        Comment = structuralMemberEnteringModel.Comment,
                    });
                }
            }
            else
            {
                await _resourceElectronicStructuralMethod.StructuralMemberEntering(structuralMemberEnteringModel);
            }
        }
        /// <summary>
        /// 结构件单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> GetStructuralIsAllEntering(long auditFlowId, StructuralMemberEnteringModel structuralMemberEnteringModel)
        {
            int AllCount = await InitialValueOfLoadingStructuralMaterials(auditFlowId);//应该录入数据库这么多条数据         
            //总共的零件
            List<SolutionModel> solutionModels = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId);
            int Count = 0;
            foreach (SolutionModel item in solutionModels)
            {
                Count += await _configStructureElectronic.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(item.SolutionId) && p.IsSubmit);//数据库实际提交的条数
            }
            List<StructuralMaterialModel> structuralMaterialModels = structuralMemberEnteringModel.StructuralMaterialEntering;
            List<StructureElectronic> structureElectronics = new();
            foreach (StructuralMaterialModel item in structuralMaterialModels)
            {
                StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.IsEntering && p.SolutionId.Equals(item.SolutionId) && p.StructureId.Equals(item.StructureId));
                if (structureElectronic is not null) structureElectronics.Add(structureElectronic);
            }
            structuralMaterialModels = structuralMaterialModels.Where(a => structureElectronics.Where(t => a.SolutionId == t.SolutionId && a.StructureId.Equals(t.StructureId)).Any() || a.IsEntering).ToList();
            Count += structuralMaterialModels.Count();
            return AllCount == Count;
        }
        /// <summary>
        ///  结构件单价录入  退回重置状态
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task GetStructuralConfigurationState(long auditFlowId)
        {
            if (auditFlowId == 0) throw new FriendlyException("结构件单价录入退回重置状态流程id不能为0");
            List<StructureElectronic> enteringElectronics = await _configStructureElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            foreach (StructureElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configStructureElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 结构件单价录入  退回重置状态  根据结构单价表的id 进行重置
        /// </summary>
        /// <param name="StructuralId"></param>
        /// <returns></returns>
        internal async Task GetStructuralConfigurationStateCertain(List<long> StructuralId)
        {
            if (StructuralId is null || StructuralId.Count == 0) throw new FriendlyException("结构件单价录入  退回重置状态 根据电子单价录入表id进行重置,传值不能为空或者数量为0");
            List<StructureElectronic> enteringElectronics = await _configStructureElectronic.GetAllListAsync(p => StructuralId.Contains(p.Id));
            foreach (StructureElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configStructureElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 电子单价录入计算
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        public async Task<ElectronicDto> ElectronicMaterialUnitPriceInputCalculation(ElectronicDto electronicDto)
        {
            return await _resourceElectronicStructuralMethod.ElectronicMaterialUnitPriceInputCalculation(electronicDto);
        }
        /// <summary>
        /// 结构单价录入计算
        /// </summary>
        /// <param name="structural"></param>
        /// <returns></returns>
        public async Task<ConstructionModel> CalculationOfStructuralMaterials(ConstructionModel structural)
        {
            return await _resourceElectronicStructuralMethod.CalculationOfStructuralMaterials(structural);
        }
        /// <summary>
        /// 获取电子单价物料返利金额
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task<List<RebateAmountKvModeElectronic>> ElectronicUnitPriceMaterialRebateAmount(long auditFlowId)
        {
            List<EnteringElectronic> prop = await _configEnteringElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<RebateAmountKvModeElectronic> rebateAmountKvModes = (from a in prop
                                                                      where a != null
                                                                      select new RebateAmountKvModeElectronic
                                                                      {
                                                                          AuditFlowId = a.AuditFlowId,
                                                                          SolutionId = a.SolutionId,
                                                                          ElectronicId = a.ElectronicId,
                                                                          ElectronicUnitPriceId = a.Id,
                                                                          KvModes = EnteringMapper.JsonToKvMode(a.RebateMoney),
                                                                      }).ToList();
            return rebateAmountKvModes;
        }
        /// <summary>
        /// 获取电子单价物料返利金额
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal async Task<List<RebateAmountKvModeElectronic>> ElectronicUnitPriceMaterialRebateAmount(long auditFlowId, Func<EnteringElectronic, bool> filter)
        {
            List<EnteringElectronic> prop = await _configEnteringElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<RebateAmountKvModeElectronic> rebateAmountKvModes = (from a in prop
                                                                      where filter(a) && a != null
                                                                      select new RebateAmountKvModeElectronic
                                                                      {
                                                                          AuditFlowId = a.AuditFlowId,
                                                                          SolutionId = a.SolutionId,
                                                                          ElectronicId = a.ElectronicId,
                                                                          ElectronicUnitPriceId = a.Id,
                                                                          KvModes = EnteringMapper.JsonToKvMode(a.RebateMoney),
                                                                      }).ToList();
            return rebateAmountKvModes;
        }
        /// <summary>
        /// 获取结构单价物料返利金额
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task<List<RebateAmountKvModeElectronicStructure>> StructureUnitPriceMaterialRebateAmount(long auditFlowId)
        {
            List<StructureElectronic> prop = await _configStructureElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<RebateAmountKvModeElectronicStructure> rebateAmountKvModes = (from a in prop
                                                                               where a != null
                                                                               select new RebateAmountKvModeElectronicStructure
                                                                               {
                                                                                   AuditFlowId = a.AuditFlowId,
                                                                                   SolutionId = a.SolutionId,
                                                                                   StructureId = a.StructureId,
                                                                                   StructuralUnitPriceId = a.Id,
                                                                                   KvModes = EnteringMapper.JsonToKvMode(a.RebateMoney),
                                                                               }).ToList();
            return rebateAmountKvModes;
        }
        /// <summary>
        /// 获取结构单价物料返利金额
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal async Task<List<RebateAmountKvModeElectronicStructure>> StructureUnitPriceMaterialRebateAmount(long auditFlowId, Func<StructureElectronic, bool> filter)
        {
            List<StructureElectronic> prop = await _configStructureElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<RebateAmountKvModeElectronicStructure> rebateAmountKvModes = (from a in prop
                                                                               where filter(a) && a != null
                                                                               select new RebateAmountKvModeElectronicStructure
                                                                               {
                                                                                   AuditFlowId = a.AuditFlowId,
                                                                                   SolutionId = a.SolutionId,
                                                                                   StructureId = a.StructureId,
                                                                                   StructuralUnitPriceId = a.Id,
                                                                                   KvModes = EnteringMapper.JsonToKvMode(a.RebateMoney),
                                                                               }).ToList();
            return rebateAmountKvModes;
        }
        /// <summary>
        /// 结构/电子/BOM/单价审核
        /// </summary>
        /// <returns></returns>
        public async Task BOMUnitPriceReview(BomUnitPriceReviewToExamineDto toExamineDto)
        {
            //电子bom单价审核 并且是拒绝
            if (toExamineDto.BomCheckType == BOMCHECKTYPE.ElecBomPriceCheck && toExamineDto.Opinion != FinanceConsts.ElectronicBomEvalSelect_Yes)
            {
                //重置状态
                await GetElectronicConfigurationStateCertain(toExamineDto.ElectronicsUnitPriceId);
            }
            //结构bom单价审核 并且是拒绝
            if (toExamineDto.BomCheckType == BOMCHECKTYPE.StructBomPriceCheck && toExamineDto.Opinion != FinanceConsts.StructBomEvalSelect_Yes)
            {
                //重置状态
                await GetStructuralConfigurationStateCertain(toExamineDto.StructureUnitPriceId);
            }
            //bom单价审核 并且是拒绝
            if (toExamineDto.BomCheckType == BOMCHECKTYPE.BomPriceCheck && toExamineDto.Opinion != FinanceConsts.BomEvalSelect_Yes)
            {
                //重置状态
                await GetElectronicConfigurationStateCertain(toExamineDto.ElectronicsUnitPriceId);
                //重置状态
                await GetStructuralConfigurationStateCertain(toExamineDto.StructureUnitPriceId);
            }
            //嵌入工作流
            await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
            {
                NodeInstanceId = toExamineDto.NodeInstanceId,
                FinanceDictionaryDetailId = toExamineDto.Opinion,
                Comment = toExamineDto.Comment,
            });
        }
        /// <summary>
        /// 电子单价复制
        /// </summary>
        /// <param name="auditFlowId">流程号</param>
        /// <returns></returns>
        public async Task ElectronicBOMUnitPriceCopying(long auditFlowId)
        {
            await _configEnteringElectronicCopy.HardDeleteAsync(p=>p.AuditFlowId.Equals(auditFlowId));
            List<EnteringElectronic> enterings = await _configEnteringElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<EnteringElectronicCopy> enteringsCopy = ObjectMapper.Map<List<EnteringElectronicCopy>>(enterings);
            await _configEnteringElectronicCopy.BulkInsertAsync(enteringsCopy);
        }
        /// <summary>
        /// 结构单价复制
        /// </summary>
        /// <param name="auditFlowId">流程号</param>
        /// <returns></returns>
        internal async Task StructureBOMUnitPriceCopying(long auditFlowId)
        {
            await _configStructureElectronicCopy.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<StructureElectronic> structures = await _configStructureElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<StructureElectronicCopy> structuresCopy = ObjectMapper.Map<List<StructureElectronicCopy>>(structures);
            await _configStructureElectronicCopy.BulkInsertAsync(structuresCopy);
        }
        /// <summary>
        /// 电子单价复制清除
        /// </summary>
        /// <param name="auditFlowId">流程号</param>
        /// <returns></returns>
        public async Task ElectronicBOMUnitPriceEliminate(long auditFlowId)
        {
            await _configEnteringElectronicCopy.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId));
        }
        /// <summary>
        /// 结构单价复制清除
        /// </summary>
        /// <param name="auditFlowId">流程号</param>
        /// <returns></returns>
        internal async Task StructureBOMUnitPriceEliminate(long auditFlowId)
        {
            await _configStructureElectronicCopy.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId));
        }
        /// <summary>
        ///  电子单价复制信息获取接口
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<List<ElectronicDtoCopy>> ElectronicUnitPriceCopyingInformationAcquisition([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            try
            {
                List<EnteringElectronicCopy> enteringElectronicCopies = await _configEnteringElectronicCopy.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                List<ElectronicDtoCopy> electronicDtoCopies = new();
                electronicDtoCopies = ObjectMapper.Map<List<ElectronicDtoCopy>>(enteringElectronicCopies);
                foreach (ElectronicDtoCopy item in electronicDtoCopies)
                {
                    ElectronicBomInfo BomInfo = await _resourceElectronicBomInfo.FirstOrDefaultAsync(p => p.Id.Equals(item.ElectronicId));
                    item.CategoryName = BomInfo.CategoryName;//物料大类
                    item.TypeName = BomInfo.TypeName;//物料种类
                    item.SapItemNum = BomInfo.SapItemNum;//物料编号
                    item.SapItemName = BomInfo.SapItemName;//材料名称
                    item.AssemblyQuantity = BomInfo.AssemblyQuantity;//装配数量
                    //获取某个ID的人员信息
                    User user = await _userRepository.FirstOrDefaultAsync(p => p.Id == item.PeopleId);
                    if (user is not null) item.PeopleName = user.Name;//提交人名称
                    user = await _userRepository.FirstOrDefaultAsync(p => p.Id == item.ModifierId);
                    if (user is not null) item.ModifierName = user.Name;//修改人名称
                }

                return electronicDtoCopies;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 电子料单价复制信息录入确认/提交  
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        public async Task PostElectronicMaterialEnteringCopy(SubmitElectronicDtoCopy electronicDto)
        {
            //循环 资源部 填写的 电子BOM 表达那实体类
            foreach (ElectronicDto electronic in electronicDto.ElectronicDtoList)
            {
                EnteringElectronicCopy enteringElectronic = await _configEnteringElectronicCopy.FirstOrDefaultAsync(p => p.SolutionId.Equals(electronic.SolutionId) && p.AuditFlowId.Equals(electronicDto.AuditFlowId) && p.ElectronicId.Equals(electronic.ElectronicId));
                enteringElectronic.MOQ = electronic.MOQ;//MOQ
                enteringElectronic.ElectronicId = electronic.ElectronicId;//电子BOM表单的Id
                enteringElectronic.SolutionId = electronic.SolutionId; //零件的id
                enteringElectronic.AuditFlowId = electronicDto.AuditFlowId;//流程的id
                enteringElectronic.RebateMoney = JsonConvert.SerializeObject(electronic.RebateMoney);//物料可返利金额
                enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id                       
                if (electronicDto.IsSubmit)
                {
                    enteringElectronic.IsSubmit = electronicDto.IsSubmit;//确认提交 
                }
                else
                {
                    enteringElectronic.IsEntering = electronicDto.IsSubmit;//确认提交 
                }
                enteringElectronic.Currency = electronic.Currency;//币种
                enteringElectronic.MaterialControlStatus = electronic.MaterialControlStatus;//物料管制状态
                enteringElectronic.Remark = electronic.Remark;//备注
                enteringElectronic.MaterialsUseCount = JsonConvert.SerializeObject(electronic.MaterialsUseCount);//物料使用量
                enteringElectronic.InTheRate = JsonConvert.SerializeObject(electronic.InTheRate);//年将率         
                enteringElectronic.StandardMoney = JsonConvert.SerializeObject(electronic.StandardMoney);//本位币
                await _configEnteringElectronicCopy.UpdateAsync(enteringElectronic);
            }
        }
        /// <summary>
        ///  结构单价复制信息获取接口
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<List<ConstructionDtoCopy>> StructureUnitPriceCopyingInformationAcquisition([FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)] long auditFlowId, [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)] long solutionId)
        {
            List<ConstructionDtoCopy> constructionDtos = new List<ConstructionDtoCopy>();
            List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId) && p.IsInvolveItem.Contains(IsInvolveItem));
            List<StructureElectronicCopy> structureElectronicCopies = await _configStructureElectronicCopy.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));

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
                List<ConstructionModelCopy> constructionModels = ObjectMapper.Map<List<ConstructionModelCopy>>(StructureMaterialnfp);// 结构BOM表单 模型
                // //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                List<StructureElectronicCopy> structureElectronicCopy = await _configStructureElectronicCopy.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.SolutionId.Equals(solutionId));
                foreach (ConstructionModelCopy construction in constructionModels)
                {
                    //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                    StructureElectronicCopy structureElectronic = structureElectronicCopy.FirstOrDefault(p => p.StructureId.Equals(construction.StructureId));
                    construction.Id = structureElectronic.Id;
                    construction.MaterialControlStatus = structureElectronic.MaterialControlStatus;//物料管制状态
                    construction.Currency = structureElectronic.Currency;//币种                       
                    construction.SolutionId = solutionId;//方案ID
                    construction.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.StandardMoney);//本位币                           
                    construction.RebateMoney = JsonConvert.DeserializeObject<List<KvMode>>(structureElectronic.RebateMoney);//物料可返回金额
                    construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.InTheRate);//年降
                    construction.SystemiginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(structureElectronic.SystemiginalCurrency);//原币
                    construction.PeopleId = structureElectronic.PeopleId;//确认人
                    construction.IsSubmit = structureElectronic.IsSubmit;//是否提交
                    construction.IsEntering = structureElectronic.IsEntering;//是否录入
                    construction.MOQ = structureElectronic.MOQ;//MOQ
                    construction.Remark = structureElectronic.Remark;//备注
                    var user = await _userRepository.FirstOrDefaultAsync(p => p.Id == structureElectronic.PeopleId);
                    if (user is not null) construction.PeopleName = user.Name;
                    user = await _userRepository.FirstOrDefaultAsync(p => p.Id == structureElectronic.ModifierId);
                    if (user is not null) construction.ModifierName = user.Name;//修改人名称
                }
                ConstructionDtoCopy constructionDtoCopy = new ConstructionDtoCopy()
                {
                    SuperTypeName = SuperTypeName,
                    StructureMaterial = constructionModels,
                };
                constructionDtos.Add(constructionDtoCopy);
            }
            return constructionDtos;
        }

        /// <summary>
        /// 结构件单价复制信息录入确认/提交 
        /// </summary>
        /// <param name="structuralMemberEnteringModel"></param>
        /// <returns></returns>        
        public async Task PostStructuralMemberEnteringCopy(StructuralMemberEnteringModelCopy structuralMemberEnteringModel)
        {
            try
            {
                foreach (ConstructionModelCopy item in structuralMemberEnteringModel.StructuralMaterialEntering)
                {
                    StructureElectronicCopy structureElectronic = await _configStructureElectronicCopy.FirstOrDefaultAsync(p => p.SolutionId.Equals(item.SolutionId) && p.AuditFlowId.Equals(structuralMemberEnteringModel.AuditFlowId) && p.StructureId.Equals(item.StructureId));

                    structureElectronic.RebateMoney = JsonConvert.SerializeObject(item.RebateMoney);//物料返利金额
                    structureElectronic.MOQ = item.MOQ;//MOQ
                    structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                    structureElectronic.Currency = item.Currency;//币种              
                    if (structuralMemberEnteringModel.IsSubmit)
                    {
                        structureElectronic.IsSubmit = structuralMemberEnteringModel.IsSubmit;//确认提交 
                    }
                    else
                    {
                        structureElectronic.IsEntering = structuralMemberEnteringModel.IsSubmit;//确认提交 
                    }
                    structureElectronic.MaterialControlStatus = item.MaterialControlStatus;//ECCN
                    structureElectronic.Remark = item.Remark; //备注                  
                    structureElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                    structureElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//项目物料的使用量
                    structureElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(item.SystemiginalCurrency);//系统单价（原币）
                    structureElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//项目物料的年降率
                    await _configStructureElectronicCopy.UpdateAsync(structureElectronic);

                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
    }

}
