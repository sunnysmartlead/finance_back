using Abp.Authorization;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.Entering.Model;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
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
        internal readonly IRepository<ModelCount, long> _resourceModelCount;
        internal readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        internal readonly IRepository<FinanceDictionaryDetail, string> _resourceFinanceDictionaryDetail;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        internal readonly IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;
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
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceEnteringAppService(IRepository<ModelCount, long> modelCount,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            ElectronicStructuralMethod electronicStructuralMethod,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            AuditFlowAppService flowAppService,
            IRepository<EnteringElectronic, long> enteringElectronic,
            IRepository<StructureElectronic, long> structureElectronic,
            IRepository<ElecBomDifferent, long> elecBomDifferent,
            IRepository<StructBomDifferent, long> structBomDifferent)
        {
            _resourceModelCount = modelCount;
            _resourceModelCountYear = modelCountYear;
            _resourceFinanceDictionaryDetail = financeDictionaryDetail;
            _resourceElectronicStructuralMethod = electronicStructuralMethod;
            _resourceElectronicBomInfo = electronicBomInfo;
            _flowAppService = flowAppService;
            _configEnteringElectronic = enteringElectronic;
            _configStructureElectronic = structureElectronic;
            _configElecBomDifferent = elecBomDifferent;
            _configStructBomDifferent = structBomDifferent;
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
            initialElectronicDto.ElectronicBomList = await _resourceElectronicStructuralMethod.ElectronicBomList(solutionList, AuditFlowId);// 电子BOM单价表单           
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
                                electronicDto = await _resourceElectronicStructuralMethod.ElectronicBom(item.SolutionId, AuditFlowId, elecBom.ElectronicId);
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
                                    electronicDto = await _resourceElectronicStructuralMethod.ElectronicBom(item.SolutionId, auditFlowId, elecBom.ElectronicId);
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
            electronicDtos = electronicDtos.Where(a => enteringElectronics.Where(t => a.SolutionId == t.SolutionId && a.ElectronicId.Equals(t.ElectronicId)).Any()).ToList();
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
        internal async Task<int> InitialValueOfLoadingStructuralMaterials(long auditFlowId)
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
            initialStructuralDto.ConstructionBomList = await _resourceElectronicStructuralMethod.ConstructionBomList(partList, auditFlowId, structureBOMIdDeleted, structureBOMIdModify);// 结构料BOM单价表单
            return initialStructuralDto.ConstructionBomList.Count;
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
            List<SolutionModel> partList = await _resourceElectronicStructuralMethod.TotalSolution(auditFlowId,item=> item.Id.Equals(solutionId));
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
        public async Task<IsALLConstructionDto> GetBOMStructuralSingle(long auditFlowId, long solutionId)
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
            structuralMaterialModels = structuralMaterialModels.Where(a => structureElectronics.Where(t => a.SolutionId == t.SolutionId && a.StructureId.Equals(t.StructureId)).Any()).ToList();
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
        /// 结构/电子/BOM/单价审核
        /// </summary>
        /// <returns></returns>
        public async Task BOMUnitPriceReview(BomUnitPriceReviewToExamineDto toExamineDto)
        {
            
        }

    }

}
