using Abp.Extensions;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Dto;
using Finance.Entering;
using Finance.ProductDevelopment.Dto;
using Finance.PropertyDepartment.Entering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// Boom审核接口
    /// </summary>
    public class BomCheckAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 界面流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;

        private readonly ElectronicBomAppService _electronicBomAppService;
        private readonly StructionBomAppService _structionBomAppService;
        /// <summary>
        /// 资源单价录入服务
        /// </summary>
        private readonly ResourceEnteringAppService _resourceEnteringAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="flowAppService"></param>
        /// <param name="resourceEnteringAppService"></param>
        /// <param name="electronicBomAppService"></param>
        /// <param name="structionBomAppService"></param>
        public BomCheckAppService(AuditFlowAppService flowAppService, ResourceEnteringAppService resourceEnteringAppService, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService)
        {
            _flowAppService = flowAppService;
            _resourceEnteringAppService = resourceEnteringAppService;
            _electronicBomAppService = electronicBomAppService;
            _structionBomAppService = structionBomAppService;
        }

        /// <summary>
        /// Bom审核
        /// </summary>
        public async virtual Task<ReturnDto> SetBomState(BomCheckDto bomCheck)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            AuditFlowDetailDto flowDetailDto = new()
            {
                AuditFlowId = bomCheck.AuditFlowId,
                UserId = AbpSession.UserId.Value,
                OpinionDescription = bomCheck.OpinionDescription
            };
            if (bomCheck.IsAgree)
            {
                flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
                if (bomCheck.BomCheckType == BOMCHECKTYPE.ElecBomCheck)
                {
                    await _electronicBomAppService.UpdateElecBomDiffAndElecBom(bomCheck.AuditFlowId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit;
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.StructBomCheck)
                {
                    await _structionBomAppService.UpdateStructionBomDiffAndStructionBom(bomCheck.AuditFlowId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_StructBomAudit;
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.ElecBomPriceCheck)
                {
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit;
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.StructBomPriceCheck)
                {
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit;
                }
            }
            else
            {
                if (bomCheck.OpinionDescription.IsNullOrEmpty())
                {
                    throw new FriendlyException("拒绝原因不能为空");
                }

                flowDetailDto.Opinion = OPINIONTYPE.Reject;
                if (bomCheck.BomCheckType == BOMCHECKTYPE.ElecBomCheck)
                {
                    await _electronicBomAppService.ClearElectronicBomDiff(bomCheck.AuditFlowId);
                    await _electronicBomAppService.ClearElecBomImportState(bomCheck.AuditFlowId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_ElectronicBomAudit;
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_ElecBomCheck + flowDetailDto.OpinionDescription;
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.StructBomCheck)
                {
                    await _structionBomAppService.ClearStructionBomDiff(bomCheck.AuditFlowId);
                    await _structionBomAppService.ClearStructBomImportState(bomCheck.AuditFlowId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_StructBomAudit;
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_StructBomCheck + flowDetailDto.OpinionDescription;
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.ElecBomPriceCheck)
                {
                    await _resourceEnteringAppService.GetElectronicConfigurationStateCertain(bomCheck.UnitPriceId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit;
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_ElecBomPriceCheck + flowDetailDto.OpinionDescription;
                    //移除电子单价提交记录中该流程ID的记录
                    ResourceSubmitLog.IsPostElectronic.Remove(bomCheck.AuditFlowId);
                }
                else if (bomCheck.BomCheckType == BOMCHECKTYPE.StructBomPriceCheck)
                {
                    await _resourceEnteringAppService.GetStructuralConfigurationStateCertain(bomCheck.UnitPriceId);
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit;
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_StructBomPriceCheck + flowDetailDto.OpinionDescription;
                    //移除结构单价提交记录中该流程ID的记录
                    ResourceSubmitLog.IsPostStructural.Remove(bomCheck.AuditFlowId);
                }
            }
            returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);

            return returnDto;
        }
    }
}
