using Finance.DemandApplyAudit;
using Finance.EntityFrameworkCore;
using Finance.TradeCompliance;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Tests.DataSend
{
    public class DefaultEvalTableCreator
    {
        private readonly FinanceDbContext _context;
        public DefaultEvalTableCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEvalTable();
        }

        private void CreateEvalTable()
        {
            //添加solution表
            var solutionList = new List<Solution>
            {
                new Solution { Id = 1,  AuditFlowId = 1, Productld = 1, ModuleName = "广丰OMS", SolutionName = "AA", Product = "广丰OMS-AA", IsCOB = true, ElecEngineerId = 1, StructEngineerId = 1, IsFirst = true },
                new Solution { Id = 2,  AuditFlowId = 1, Productld = 1, ModuleName = "广丰OMS", SolutionName = "BB", Product = "广丰OMS-BB", IsCOB = true, ElecEngineerId = 1, StructEngineerId = 1, IsFirst = false },
            };
            var noDb = solutionList.Where(p => !_context.Solution.Contains(p));
            if (noDb.Any())
            {
                _context.Solution.AddRange(noDb);
                _context.SaveChanges();
            }
        }
    }
}
