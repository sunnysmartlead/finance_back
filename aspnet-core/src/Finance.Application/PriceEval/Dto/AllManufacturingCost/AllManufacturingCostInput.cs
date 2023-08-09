using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.AllManufacturingCost
{
    public interface IManufacturingCostInput
    {
        /// <summary>
        /// 年份（全生命周期，此字段为0）
        /// </summary>
        [Required]
        public int Year { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        [Required]
        public decimal Subtotal { get; set; }
    }
    public class OtherManufacturingCostInput : IManufacturingCostInput
    {
        /// <summary>
        /// 年份（全生命周期，此字段为0）
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        [Required]
        public virtual decimal Subtotal { get; set; }

    }
    public class AllManufacturingCostInput: IManufacturingCostInput
    {
        ///// <summary>
        ///// 审批流程主表ID
        ///// </summary>
        //[Required]
        //public virtual long AuditFlowId { get; set; }

        ///// <summary>
        ///// 主表 模组数量（ModelCount） 的Id
        ///// </summary>
        //[Required]
        //public virtual long ProductId { get; set; }

        /// <summary>
        /// 年份（全生命周期，此字段为0）
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        ///// <summary>
        ///// 成本类型
        ///// </summary>
        //[Required]
        //public virtual CostType CostType { get; set; }

        /// <summary>
        /// 直接制造成本：直接人工
        /// </summary>
        [Required]
        public virtual decimal DirectLabor1 { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧
        /// </summary>
        [Required]
        public virtual decimal EquipmentDepreciation1 { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        [Required]
        public virtual decimal LineChangeCost1 { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用
        /// </summary>
        [Required]
        public virtual decimal ManufacturingExpenses1 { get; set; }


        /// <summary>
        /// 直接制造成本：小计
        /// </summary>
        [Required]
        public virtual decimal Subtotal1 { get; set; }

        /// <summary>
        /// 间接制造成本：直接人工
        /// </summary>
        [Required]
        public virtual decimal DirectLabor2 { get; set; }

        /// <summary>
        /// 间接制造成本：设备折旧
        /// </summary>
        [Required]
        public virtual decimal EquipmentDepreciation2 { get; set; }

        /// <summary>
        /// 间接制造成本：制造费用
        /// </summary>
        [Required]
        public virtual decimal ManufacturingExpenses2 { get; set; }

        /// <summary>
        /// 间接制造成本：小计
        /// </summary>
        [Required]
        public virtual decimal Subtotal2 { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        [Required]
        public virtual decimal Subtotal { get; set; }
    }
}
