using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.WorkFlows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 友善的非空验证
    /// </summary>     
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FriendlyRequiredAttribute : ValidationAttribute
    {
        private IRepository<WorkflowInstance, long> _auditFlowRepository;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>   
        private IRepository<SolutionTable, long> _resourceSchemeTable;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eroName">异常属性名称</param>
        /// <param name="specialVerifica">该属性是否进行特殊验证</param>
        public FriendlyRequiredAttribute(string eroName, SpecialVerification specialVerifica = SpecialVerification.Nothing)
        {
            errorName = eroName;
            specialVerification = specialVerifica;
            _auditFlowRepository = IocManager.Instance.Resolve<IRepository<WorkflowInstance, long>>();
            _resourceSchemeTable = IocManager.Instance.Resolve<IRepository<SolutionTable, long>>();
        }
        /// <summary>
        /// 验证的属性名称
        /// </summary>
        private string errorName { get; set; }
        /// <summary>
        /// 特殊验证
        /// </summary>
        private SpecialVerification specialVerification { get; set; }
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>         
        public override bool IsValid(object value)
        {
            try
            {
                using (IUnitOfWorkCompleteHandle unitOfWork = IocManager.Instance.Resolve<IUnitOfWorkManager>().Begin())
                {                    
                    if (value == null) throw new FriendlyException($"{errorName}不能为空");
                    if (value is string str && string.IsNullOrWhiteSpace(str)) throw new FriendlyException($"{errorName}不能为空");
                    if ((value is int || value is long) && (long)value == 0) throw new FriendlyException($"{errorName}不能为0");
                    if (value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext()) throw new FriendlyException($"{errorName}不能为空");
                    if ((value is int || value is long) && specialVerification.Equals(SpecialVerification.AuditFlowIdVerification))
                    {
                        if (!_auditFlowRepository.GetAll().Any(p => p.Id == (long)value))
                        {
                            throw new FriendlyException($"流程Id【{(long)value}】不存在。");
                        }
                    }
                    else if ((value is int || value is long) && specialVerification.Equals(SpecialVerification.SolutionIdVerification))
                    {
                        if (!_resourceSchemeTable.GetAll().Any(p => p.Id == (long)value))
                        {
                            throw new FriendlyException($"方案Id【{(long)value}】不存在。");
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"{ex.Message},参数异常提示来自FriendlyRequiredAttribute");
            }
        }
        /// <summary>
        /// 特殊验证类型枚举
        /// </summary>
        public enum SpecialVerification
        {
            /// <summary>
            /// 不需要特殊验证
            /// </summary>
            Nothing,
            /// <summary>
            /// 流程ID验证
            /// </summary>
            AuditFlowIdVerification,
            /// <summary>
            /// 方案ID验证
            /// </summary>
            SolutionIdVerification,
        }
    }
}
