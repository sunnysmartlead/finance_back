using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Dto;
using Finance.PriceEval;
using Finance.ProjectManagement;
using Finance.TRSolution.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TRSolution
{
    /// <summary>
    /// TR主方案审核后端API类
    /// </summary>
    public class TRCheckAppService : FinanceAppServiceBase
    {
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<UserInputInfo, long> _userInputInfoRepository;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;

        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="priceEvaluationRepository"></param>
        public TRCheckAppService(IRepository<PriceEvaluation, long> priceEvaluationRepository, AuditFlowAppService flowAppService, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<FileManagement, long> fileManagementRepository)
        {
            _priceEvaluationRepository = priceEvaluationRepository;
            _flowAppService = flowAppService;
            _userInputInfoRepository = userInputInfoRepository;
            _fileManagementRepository = fileManagementRepository;
        }


        /// <summary>
        /// 获取TR主方案信息
        /// </summary>
        public async virtual Task<TRMainSolutionCheckDto> GetTRMainSolution(long flowId)
        {
            TRMainSolutionCheckDto tRMainSolutionCheckDto = new();
            try
            {
                var priceInfo = await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == flowId);
                var userInputInfo = await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == flowId);
                string createTime = priceInfo.CreationTime.ToString("yyyy年MM⽉dd⽇ HH:mm:ss");
                string department = priceInfo.DraftingDepartment;
                string customerName = priceInfo.CustomerName;
                string projectName = priceInfo.ProjectName;

                tRMainSolutionCheckDto.AuditFlowId = flowId;
                tRMainSolutionCheckDto.Title = priceInfo.Title;
                tRMainSolutionCheckDto.SolutionFileIdentifier = userInputInfo.FileId;//JsonConvert.DeserializeObject<List<long>>(priceInfo.SorFile).FirstOrDefault();
                tRMainSolutionCheckDto.IsSuccess = true;

                var fileName = await _fileManagementRepository.GetAllListAsync(p => p.Id == userInputInfo.FileId);
                if (fileName.Count > 0)
                {
                    tRMainSolutionCheckDto.SolutionFileName = fileName.FirstOrDefault().Name;
                    return tRMainSolutionCheckDto;
                }
                else
                {
                    tRMainSolutionCheckDto.IsSuccess = false;
                    tRMainSolutionCheckDto.Message = "文件找不到";
                    return tRMainSolutionCheckDto;
                }
            }
            catch (Exception ex)
            {
                tRMainSolutionCheckDto.IsSuccess = false;
                tRMainSolutionCheckDto.Message = ex.Message;
                return tRMainSolutionCheckDto;
            }
        }
        /// <summary>
        /// TR主方案审核
        /// </summary>
        public async virtual Task<ReturnDto> SetTRMainSolutionState(SetTRMainSolutionStateDto setTRMainSolutionState)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            AuditFlowDetailDto flowDetailDto = new()
            {
                AuditFlowId = setTRMainSolutionState.AuditFlowId,
                UserId = AbpSession.UserId.Value,
                OpinionDescription = setTRMainSolutionState.OpinionDescription
            };
            if (setTRMainSolutionState.IsAgree)
            {
                flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
                if (setTRMainSolutionState.TRCheckType == TRCHECKTYPE.MKTTRCheck)
                {
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT;
                }
                else if (setTRMainSolutionState.TRCheckType == TRCHECKTYPE.R_DTRCheck)
                {
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D;
                }
            }
            else
            {
                if (setTRMainSolutionState.OpinionDescription.IsNullOrEmpty())
                {
                    throw new FriendlyException("拒绝原因不能为空");
                }
                flowDetailDto.Opinion = OPINIONTYPE.Reject;
                if (setTRMainSolutionState.TRCheckType == TRCHECKTYPE.MKTTRCheck)
                {
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_MKT_TRMainCheck + flowDetailDto.OpinionDescription;
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT;
                }
                else if (setTRMainSolutionState.TRCheckType == TRCHECKTYPE.R_DTRCheck)
                {
                    flowDetailDto.OpinionDescription = OpinionDescription.OD_R_D_TRMainCheck + flowDetailDto.OpinionDescription;
                    flowDetailDto.ProcessIdentifier = AuditFlowConsts.AF_TRAuditR_D;
                }
            }
            returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);

            return returnDto;
        }
    }
}
