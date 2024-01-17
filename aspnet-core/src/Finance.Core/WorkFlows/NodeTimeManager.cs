using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    public class NodeTimeManager : DomainService
    {
        private readonly IRepository<NodeTime, long> _nodeTimeRepository;

        public NodeTimeManager(IRepository<NodeTime, long> nodeTimeRepository)
        {
            _nodeTimeRepository = nodeTimeRepository;
        }

        /// <summary>
        /// 节点开始
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <returns></returns>
        public async Task Start(long workFlowInstanceId, long nodeInstance)
        {
            await _nodeTimeRepository.InsertAsync(new NodeTime
            {
                WorkFlowInstanceId = workFlowInstanceId,
                NodeInstance = nodeInstance,
                StartTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 节点更新
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <returns></returns>
        public async Task Update(long workFlowInstanceId, long nodeInstance)
        {
            var nodeTime = await _nodeTimeRepository.GetAll()
                .Where(p => p.WorkFlowInstanceId == workFlowInstanceId && p.NodeInstance == nodeInstance && p.UpdateTime == null)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            
            if (nodeTime is null)
            {
                await _nodeTimeRepository.InsertAsync(new NodeTime
                {
                    WorkFlowInstanceId = workFlowInstanceId,
                    NodeInstance = nodeInstance,
                    UpdateTime = DateTime.Now,
                });
            }
            else
            {
                nodeTime.UpdateTime = DateTime.Now;
                await _nodeTimeRepository.UpdateAsync(nodeTime);
            }
        }

    }
}
