using Abp.Domain.Repositories;
using Finance.PriceEval.Dto;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 数据录入API，获取录入的依据数据
    /// </summary>
    public class DataInputAppService : FinanceAppServiceBase
    {
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<GradientModel, long> _gradientModelRepository;
        private readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;

        public DataInputAppService(IRepository<Gradient, long> gradientRepository, IRepository<GradientModel, long> gradientModelRepository, IRepository<GradientModelYear, long> gradientModelYearRepository)
        {
            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;
        }

        /// <summary>
        /// 根据模组Id获取梯度信息录入的依据数据
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<List<GradientModelYearListDto>> GetGradientModelYearByProductId(long productId)
        {
            var data = from m in _gradientModelRepository.GetAll()
                       join y in _gradientModelYearRepository.GetAll() on m.Id equals y.GradientModelId
                       join g in _gradientRepository.GetAll() on m.GradientId equals g.Id
                       where y.ProductId == productId
                       select new GradientModelYearListDto
                       {
                           Id = y.Id,
                           AuditFlowId = y.AuditFlowId,
                           PriceEvaluationId = y.PriceEvaluationId,
                           GradientModelId = y.GradientModelId,
                           ProductId = y.ProductId,
                           GradientValue = g.GradientValue,
                           Year = y.Year,
                           UpDown = y.UpDown,
                           Count = y.Count
                       };
            return await data.ToListAsync();
        }

        /// <summary>
        /// 根据流程Id获取梯度
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public virtual async Task<List<Gradient>> GetGradientByAuditFlowId(long auditFlowId) 
        {
            return await _gradientRepository.GetAllListAsync(p=>p.AuditFlowId == auditFlowId);
        }
    }
}
