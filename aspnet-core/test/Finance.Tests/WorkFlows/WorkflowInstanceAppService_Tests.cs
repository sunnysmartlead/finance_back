using Finance.EntityFrameworkCore.Seed.Host;
using Finance.PriceEval;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.WorkFlows
{
    public class WorkflowInstanceAppService_Tests : FinanceTestBase
    {
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        public WorkflowInstanceAppService_Tests()
        {
            _workflowInstanceAppService = Resolve<WorkflowInstanceAppService>();
        }

        /// <summary>
        /// 测试核价开始函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task StartWorkflowInstance_Test()
        {
            var output = await _workflowInstanceAppService.StartWorkflowInstance(new StartWorkflowInstanceInput
            {
                WorkflowId = WorkFlowCreator.MainFlowId,
                FinanceDictionaryDetailId = FinanceConsts.Done,
                Title = "这是一个测试的流程实例"
            });
            output.CompareTo(1);
        }

        /// <summary>
        /// 测试核价开始函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SubmitNode_Test()
        {
          var erer=  await _workflowInstanceAppService.StartWorkflowInstance(new StartWorkflowInstanceInput
            {
                WorkflowId = WorkFlowCreator.MainFlowId,
                FinanceDictionaryDetailId = FinanceConsts.EvalReason_Bb1,
                Title = "这是一个测试的流程实例"
            });

            var result =await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(erer);

            await _workflowInstanceAppService.SubmitNode(new SubmitNodeInput
            {
                NodeInstanceId = 12,
                FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
            });
        }
    }
}
