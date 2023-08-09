using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 成本明细差异（成本项目）
    /// </summary>
    public class CostDetailVarianceManufacturingCost
    {
        /// <summary>
        /// 版本1
        /// </summary>
        public virtual CostDetailVarianceManufacturingCostInfo Version1 { get; set; }

        /// <summary>
        /// 版本2
        /// </summary>
        public virtual CostDetailVarianceManufacturingCostInfo Version2 { get; set; }

        /// <summary>
        /// 差异
        /// </summary>
        public virtual VarianceCostDetailVarianceManufacturingCostInfo Variance { get; set; }
    }

    /// <summary>
    /// 成本明细差异（物料）字段信息
    /// </summary>
    public class CostDetailVarianceManufacturingCostInfo : EntityDto<int>
    {
        /// <summary>
        /// 直接制造成本：直接人工（单位人工）
        /// </summary>
        public virtual decimal DirectLabor { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧（单位折旧）
        /// </summary>
        public virtual decimal EquipmentDepreciation { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual decimal LineChangeCost { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用（单位制造费用）
        /// </summary>
        public virtual decimal ManufacturingExpenses { get; set; }
    }

    public class VarianceCostDetailVarianceManufacturingCostInfo 
    {
        /// <summary>
        /// 直接制造成本：直接人工（单位人工）
        /// </summary>
        public virtual string DirectLabor { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧（单位折旧）
        /// </summary>
        public virtual string EquipmentDepreciation { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual string LineChangeCost { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用（单位制造费用）
        /// </summary>
        public virtual string ManufacturingExpenses { get; set; }
    }


}
