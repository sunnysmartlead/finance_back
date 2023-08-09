using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 制造成本（手动输入）
    /// </summary>
    [Table("Pe_AllManufacturingCost")]
    public class AllManufacturingCost : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 年份（全生命周期，此字段为0）
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 成本类型
        /// </summary>
        public virtual CostType CostType { get; set; }

        /// <summary>
        /// 直接制造成本：直接人工
        /// </summary>
        public virtual decimal? DirectLabor1 { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧
        /// </summary>
        public virtual decimal? EquipmentDepreciation1 { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual decimal? LineChangeCost1 { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用
        /// </summary>
        public virtual decimal? ManufacturingExpenses1 { get; set; }


        /// <summary>
        /// 直接制造成本：小计
        /// </summary>
        public virtual decimal? Subtotal1 { get; set; }

        /// <summary>
        /// 间接制造成本：直接人工
        /// </summary>
        public virtual decimal? DirectLabor2 { get; set; }

        /// <summary>
        /// 间接制造成本：设备折旧
        /// </summary>
        public virtual decimal? EquipmentDepreciation2 { get; set; }

        /// <summary>
        /// 间接制造成本：制造费用
        /// </summary>
        public virtual decimal? ManufacturingExpenses2 { get; set; }

        /// <summary>
        /// 间接制造成本：小计
        /// </summary>
        public virtual decimal? Subtotal2 { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Subtotal { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }

    /// <summary>
    /// 成本类型
    /// </summary>
    public enum CostType : byte
    {
        /// <summary>
        /// 组测
        /// </summary>
        GroupTest,

        /// <summary>
        /// SMT
        /// </summary>
        SMT,

        /// <summary>
        /// COB
        /// </summary>
        COB,

        /// <summary>
        /// 合计
        /// </summary>
        Total,

        /// <summary>
        /// 其他
        /// </summary>
        Other
        
    }

}
