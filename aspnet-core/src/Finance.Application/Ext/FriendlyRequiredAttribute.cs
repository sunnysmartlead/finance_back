using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.DemandApplyAudit;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        private IRepository<Solution, long> _resourceSchemeTable;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string Opinion { get; set; } = "Opinion";
        public bool _skip { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eroName">异常属性名称</param>
        /// <param name="specialVerifica">该属性是否进行特殊验证</param>
        /// <param name="skip">如果是true的话,无论流程是保存还是提交,都需要校验</param>
        public FriendlyRequiredAttribute(string eroName, SpecialVerification specialVerifica = SpecialVerification.Nothing,bool skip=false)
        {
            errorName = eroName;
            specialVerification = specialVerifica;
            _skip = skip;
            _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
            _auditFlowRepository = IocManager.Instance.Resolve<IRepository<WorkflowInstance, long>>();
            _resourceSchemeTable = IocManager.Instance.Resolve<IRepository<Solution, long>>();
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
        /// 判断参数是否需要校验
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //如果是true的话,无论流程是保存还是提交,都需要校验
            if(_skip)
            {
                // 需要验证
                return base.IsValid(value, validationContext);
            }
            var parameterValue = GetParameterValue(Opinion, validationContext.ObjectInstance);
            var yes = new List<string> { FinanceConsts.YesOrNo_Yes,
                FinanceConsts.EvalFeedback_Js,
                FinanceConsts.StructBomEvalSelect_Yes,
                FinanceConsts.BomEvalSelect_Yes,
                FinanceConsts.MybhgSelect_No,
                FinanceConsts.HjkbSelect_Yes,
                FinanceConsts.ElectronicBomEvalSelect_Yes,FinanceConsts.Done};
            if (parameterValue != null &&!yes.Contains(parameterValue.ToString()))
            {
                _httpContextAccessor.HttpContext.Items["Skip"] = true;
                // 不需要验证
                return ValidationResult.Success;
            }
            else
            {
                var cacheEntry =_httpContextAccessor.HttpContext.Items["Skip"];                
                if (cacheEntry is not null&&(bool)cacheEntry)
                {
                    // 不需要验证
                    return ValidationResult.Success;
                }
                // 需要验证
                return base.IsValid(value, validationContext);
            }
        }
        // 在对象中查找指定的参数值
        private object GetParameterValue(string parameterName, object obj)
        {
            var type = obj.GetType();
            var property = type.GetProperty(parameterName);
            if (property != null)
            {
                return property.GetValue(obj);
            }
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.Name == parameterName)
                {
                    return field.GetValue(obj);
                }
            }
            return null;
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
