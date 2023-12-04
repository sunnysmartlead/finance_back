using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.PriceEval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class PanelJsonClearJob : AsyncBackgroundJob<long>, ITransientDependency
    {
        private readonly IRepository<PanelJson, long> _panelJsonRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PanelJsonClearJob(IRepository<PanelJson, long> panelJsonRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _panelJsonRepository = panelJsonRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }



        /// <summary>
        /// 清除核价表缓存
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async override Task ExecuteAsync(long auditFlowId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    await _panelJsonRepository.DeleteAsync(p => p.AuditFlowId == auditFlowId);
                }
                uow.Complete();
            }
        }
    }
}
