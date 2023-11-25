using Abp.Domain.Entities;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 快速核报价使用的导入BOM成本表
    /// </summary>
    public class BomMaterial: Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 超级大种类
        /// </summary>
        public virtual string SuperType { get; set; }

        /// <summary>
        /// SAP（物理编码）
        /// </summary>
        public virtual string Sap { get; set; }

        /// <summary>
        /// 材料名称
        /// </summary>
        public virtual string MaterialName { get; set; }

        /// <summary>
        /// 是否客供
        /// </summary>
        public virtual bool IsCustomerSupply { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
        public virtual string TypeName { get; set; }

        /// <summary>
        /// 物料大类
        /// </summary>
        public virtual string CategoryName { get; set; }

        /// <summary>
        /// 装配数量
        /// </summary>
        public virtual double AssemblyCount { get; set; }

        /// <summary>
        /// 材料单价（原币）
        /// </summary>
        public virtual decimal MaterialPrice { get; set; }

        /// <summary>
        /// 币别文本
        /// </summary>
        public virtual string CurrencyText { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public virtual decimal ExchangeRate { get; set; }

        /// <summary>
        /// 材料单价（人民币）
        /// </summary>
        public virtual decimal MaterialPriceCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）
        /// </summary>
        public virtual decimal TotalMoneyCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）不含客供
        /// </summary>
        public virtual decimal TotalMoneyCynNoCustomerSupply { get; set; }

        /// <summary>
        /// 损耗
        /// </summary>
        public virtual decimal Loss { get; set; }

        /// <summary>
        /// 材料成本（含损耗）
        /// </summary>
        public virtual decimal MaterialCost { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual decimal InputCount { get; set; }

        /// <summary>
        /// 采购量
        /// </summary>
        public virtual decimal PurchaseCount { get; set; }

        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public virtual decimal MoqShareCount { get; set; }

        /// <summary>
        /// MOQ
        /// </summary>
        public virtual decimal Moq { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public virtual int AvailableInventory { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }
    }
}
