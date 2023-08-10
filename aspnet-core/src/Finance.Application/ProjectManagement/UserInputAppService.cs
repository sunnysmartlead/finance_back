using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.ProjectManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProjectManagement
{
    /// <summary>
    /// 项目管理部录入
    /// </summary>
    public class UserInputAppService : FinanceAppServiceBase
    {
        private readonly IRepository<UserInputInfo, long> _userInputInfoRepository;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;

        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;

        public UserInputAppService(IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<FileManagement, long> fileManagementRepository, AuditFlowAppService flowAppService)
        {
            _userInputInfoRepository=userInputInfoRepository;
            _fileManagementRepository=fileManagementRepository;
            _flowAppService=flowAppService;
        }

        /// <summary>
        /// 项目经理确认录入
        /// </summary>
        /// <param name="userInputInfo"></param>
        /// <returns></returns>
        public async virtual Task<UserInputInfo> PostManagement(UserInputInfo userInputInfo)
        {
            //检查用户是否已经选择
            this.CheckUserId(userInputInfo.ElectronicEngineerId);
            this.CheckUserId(userInputInfo.StructureEngineerId);
            this.CheckUserId(userInputInfo.EngineerLossRateId);
            this.CheckUserId(userInputInfo.EngineerWorkHourId);
            this.CheckUserId(userInputInfo.QualityBenchId);
            this.CheckUserId(userInputInfo.QualityToolId);
            this.CheckUserId(userInputInfo.ProductManageId);
            this.CheckUserId(userInputInfo.ProjectAuditorId);
            UserInputInfo entity;
            var userInputs = await _userInputInfoRepository.GetAllListAsync(p => p.AuditFlowId == userInputInfo.AuditFlowId);
            if(userInputs.Count > 0)
            {
                entity = userInputs.FirstOrDefault();
                entity.ElectronicEngineerId = userInputInfo.ElectronicEngineerId;
                entity.StructureEngineerId = userInputInfo.StructureEngineerId;
                entity.ResourceElecId = userInputInfo.ResourceElecId;
                entity.ResourceStructId = userInputInfo.ResourceStructId; 
                entity.EngineerLossRateId = userInputInfo.EngineerLossRateId;
                entity.EngineerWorkHourId = userInputInfo.EngineerWorkHourId;
                entity.QualityBenchId = userInputInfo.QualityBenchId;
                entity.QualityToolId = userInputInfo.QualityToolId;
                entity.ProductManageId = userInputInfo.ProductManageId;
                entity.ProjectAuditorId = userInputInfo.ProjectAuditorId;
                entity.IsFirst = userInputInfo.IsFirst;
                entity.FileId = userInputInfo.FileId;

                entity.TRSubmitTime = userInputInfo.TRSubmitTime;
                entity.ElecEngineerTime = userInputInfo.ElecEngineerTime;
                entity.StructEngineerTime = userInputInfo.StructEngineerTime;
                entity.QualityBenchTime = userInputInfo.QualityBenchTime;
                entity.QualityToolTime = userInputInfo.QualityToolTime;
                entity.ResourceElecTime = userInputInfo.ResourceElecTime;
                entity.ResourceStructTime = userInputInfo.ResourceStructTime;
                entity.EngineerLossRateTime = userInputInfo.EngineerLossRateTime;
                entity.EngineerWorkHourTime = userInputInfo.EngineerWorkHourTime;
                entity.ProductManageTime = userInputInfo.ProductManageTime;
                entity.ProductCostInputTime = userInputInfo.ProductCostInputTime;
            }
            else
            {
                entity = new();
                entity = userInputInfo;
            }
            UserInputInfo entityRet = await _userInputInfoRepository.InsertOrUpdateAsync(entity);

            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }

            await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
            {
                AuditFlowId = userInputInfo.AuditFlowId,
                ProcessIdentifier = AuditFlowConsts.AF_PMInput,
                UserId = AbpSession.UserId.Value,
                Opinion = OPINIONTYPE.Submit_Agreee
            });
            return entityRet;
        }

        internal bool CheckUserId(long userId)
        {
            if(userId < 1)
            {
                throw new FriendlyException("用户有误，请检查用户选择是否正确！");
            }
            return true;
        }
        
        /// <summary>
        /// 根据流程号查询项目经理录入
        /// </summary>
        /// <param name="AuditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<UserInputDto> getManagement(long AuditFlowId)
        {
            List<UserInputInfo> userInputs = await _userInputInfoRepository.GetAllListAsync(p => p.AuditFlowId == AuditFlowId);
            if(userInputs.Count == 0)
            {
                return null;
            }
            else
            {
                UserInputDto result = ObjectMapper.Map<UserInputDto>(userInputs.FirstOrDefault());

                long FileId = result.FileId;

                List<FileManagement> fileList = await _fileManagementRepository.GetAllListAsync(p => p.Id == FileId);
                if (fileList.Count > 0)
                {
                    result.FileName = fileList.FirstOrDefault().Name;
                }
                else
                {
                    throw new FriendlyException("文件名找不到！");
                }
                return result;
            }
        }
    }
}


