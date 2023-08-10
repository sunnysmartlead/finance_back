using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 获取
    /// </summary>
    public class GetCostDetailVarianceInput 
    {
        /// <summary>
        /// 版本1：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion1AuditFlowId { get; set; }

        /// <summary>
        /// 版本2：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion2AuditFlowId { get; set; }
    }

    /// <summary>
    /// 获取成本明细差异方法输入
    /// </summary>
    public class GetCostDetailVarianceMaterialInput
    {
        /// <summary>
        /// 版本1：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion1AuditFlowId { get; set; }

        /// <summary>
        /// 版本2：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion2AuditFlowId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 是否为全生命周期
        /// </summary>
        public virtual bool IsAll { get; set; }
    }

    /// <summary>
    /// 获取成本项目差异方法输入
    /// </summary>
    public class GetCostDetailVarianceManufacturingCostInput
    {
        /// <summary>
        /// 版本1：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion1AuditFlowId { get; set; }

        /// <summary>
        /// 版本2：审批流程主表Id
        /// </summary>
        [Required]
        public virtual long QuoteVersion2AuditFlowId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 是否为全生命周期
        /// </summary>
        public virtual bool IsAll { get; set; }
    }
}
