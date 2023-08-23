using Abp.Domain.Repositories;
using Finance.PriceEval.Dto;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.PriceEval.Dto.DataInput;

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
        private readonly IRepository<Requirement, long> _requirementRepository;
        private readonly IRepository<CustomerTargetPrice, long> _customerTargetPriceRepository;
        
        public DataInputAppService(IRepository<Gradient, long> gradientRepository, IRepository<GradientModel, long> gradientModelRepository, IRepository<GradientModelYear, long> gradientModelYearRepository, IRepository<Requirement, long> requirementRepository)
        {
            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;
            _requirementRepository = requirementRepository;
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
            return await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
        }

        /// <summary>
        /// 根据年份和流程Id获取核价需求页面录入的要求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<IList<Requirement>> GetRequirement(GetRequirementInput input)
        {
            return await _requirementRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Year == input.Year && p.UpDown == input.UpDown);
        }

        /// <summary>
        /// 根据年份和流程Id获取核价需求页面录入的要求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<IList<CustomerTargetPrice>> GetCustomerTargetPrice(GetCustomerTargetPriceInput input)
        {
            return await _customerTargetPriceRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Kv == input.Kv && p.Product == input.Product);
        }
    }
}
