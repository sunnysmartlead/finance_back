using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Finance.Audit.Dto;
using Finance.Authorization.Roles;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    public class RuleAppService : FinanceAppServiceBase
    {
        private readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<PricingTeam, long> _pricingTeamRepository;
        private readonly IRepository<PriceEvaluationStartData, long> _priceEvaluationStartDataRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public RuleAppService(IRepository<Solution, long> solutionRepository, IRepository<PricingTeam, long> pricingTeamRepository, IRepository<PriceEvaluationStartData, long> priceEvaluationStartDataRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Role> roleRepository, IRepository<UserRole, long> userRoleRepository)
        {
            _solutionRepository = solutionRepository;
            _pricingTeamRepository = pricingTeamRepository;
            _priceEvaluationStartDataRepository = priceEvaluationStartDataRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// 过滤待办项
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<AuditFlowRightDetailDto>> FilteTask(long auditFlowId, List<AuditFlowRightDetailDto> list, long? userId = null)
        {
            if (userId == null) 
            {
                userId = AbpSession.UserId;
            }
            //获取当前流程方案列表
            var solutionList = await _solutionRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);

            //获取核价团队
            var pricingTeam = await _pricingTeamRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);

            //获取项目经理
            var projectPm = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);

            //获取核价需求录入保存项
            var priceEvaluationStartData = await _priceEvaluationStartDataRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);


            //项目经理控制的页面
            var pmPage = new List<string> { FinanceConsts.PriceDemandReview,
                FinanceConsts.NRE_ManualComponentInput, FinanceConsts.UnitPriceInputReviewToExamine,
                FinanceConsts.PriceEvaluationBoard };

            //拥有能看归档的角色的用户
            var role = await _roleRepository.GetAllListAsync(p =>
                            p.Name == StaticRoleNames.Host.FinanceTableAdmin
                            || p.Name == StaticRoleNames.Host.EvalTableAdmin
                    || p.Name == StaticRoleNames.Host.Bjdgdgly);
            var userIds = await _userRoleRepository.GetAll().Where(p => role.Select(p => p.Id).Contains(p.RoleId)).Select(p => p.UserId).ToListAsync();


            var dto = list

                //如果当前用户不是电子工程师，就把电子BOM录入页面过滤掉
                .WhereIf(!solutionList.Any(p => p.ElecEngineerId == userId), p => p.ProcessIdentifier != FinanceConsts.ElectronicsBOM || p.IsReset)

                //如果当前用户不是结构工程师，就把结构BOM录入页面过滤掉
                .WhereIf(!solutionList.Any(p => p.StructEngineerId == userId), p => p.ProcessIdentifier != FinanceConsts.StructureBOM || p.IsReset)

                //工序工时
                .WhereIf(pricingTeam == null || pricingTeam.EngineerId != userId, p => p.ProcessIdentifier != FinanceConsts.FormulaOperationAddition || p.IsReset)

                //环境实验费
                .WhereIf(pricingTeam == null || pricingTeam.QualityBenchId != userId, p => p.ProcessIdentifier != FinanceConsts.NRE_ReliabilityExperimentFeeInput || p.IsReset)

                //EMC+电性能实验费录入
                .WhereIf(pricingTeam == null || pricingTeam.EMCId != userId, p => p.ProcessIdentifier != FinanceConsts.NRE_EMCExperimentalFeeInput || p.IsReset)

                //制造成本录入
                .WhereIf(pricingTeam == null || pricingTeam.ProductCostInputId != userId, p => p.ProcessIdentifier != FinanceConsts.COBManufacturingCostEntry || p.IsReset)

                //物流成本录入
                .WhereIf(pricingTeam == null || pricingTeam.ProductManageTimeId != userId, p => p.ProcessIdentifier != FinanceConsts.LogisticsCostEntry || p.IsReset)

                //项目核价审核
                .WhereIf(pricingTeam == null || pricingTeam.AuditId != userId, p => p.ProcessIdentifier != FinanceConsts.ProjectChiefAudit || p.IsReset)

                //项目经理
                .WhereIf(projectPm == null || projectPm.ProjectManager != userId, p => ((!pmPage.Contains(p.ProcessIdentifier)) || p.ProcessName == FinanceConsts.Bomcbsh) || p.IsReset)

                //生成报价分析界面选择报价方案、选择是否报价，必须是发起核价需求录入的人才能看到
                .WhereIf(projectPm == null || projectPm.CreatorUserId != userId, p => p.ProcessIdentifier != FinanceConsts.QuoteAnalysis || p.IsReset)

                //核价需求录入，必须是自己录入才可见
                .WhereIf((priceEvaluationStartData != null && priceEvaluationStartData.CreatorUserId != null && priceEvaluationStartData.CreatorUserId != userId)

                || (projectPm != null && projectPm.CreatorUserId != userId), p => p.ProcessIdentifier != FinanceConsts.PricingDemandInput || p.IsReset)

                //报价单，必须是发起核价需求录入的人才能看到
                .WhereIf(projectPm == null || projectPm.CreatorUserId != userId, p => p.ProcessIdentifier != "ExternalQuotation" || p.IsReset)

                //报价审批表、【报价反馈】，必须是发起核价需求录入的人才能看到
                .WhereIf(projectPm == null || projectPm.CreatorUserId != userId,
                p => (p.ProcessIdentifier != "QuotationApprovalForm" && p.ProcessIdentifier != "QuoteFeedback") || p.IsReset)

                //如果当前用户是本流程的项目经理，就把【审批报价策略与核价表】、【报价反馈】、【确认中标金额】页面过滤掉
                .WhereIf(projectPm == null || projectPm.ProjectManager == userId,
                p => p.ProcessIdentifier != "QuoteApproval" || p.ProcessIdentifier != "QuoteFeedback" || p.ProcessIdentifier != "BidWinningConfirmation"
                || p.IsReset
                )

                //归档，如果当前用户不是该流程的发起人、项目经理，或归档管理员，就过滤
                .WhereIf(projectPm == null || (projectPm.CreatorUserId != userId && projectPm.ProjectManager != userId)
                && (!userIds.Contains(userId.Value)),

                p => p.ProcessIdentifier != "ArchiveEnd" || p.IsReset)

                .ToList();
            return dto;
        }
    }
}
