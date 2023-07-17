using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 成本明细差异（物料）
    /// </summary>
    public class CostDetailVarianceMaterial
    {
        /// <summary>
        /// 超级大种类
        /// </summary>
        public virtual string SuperType { get; set; }

        /// <summary>
        /// 物料大类
        /// </summary>

        public virtual string CategoryName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>

        public virtual string TypeName { get; set; }

        /// <summary>
        /// 版本1
        /// </summary>
        public virtual CostDetailVarianceMaterialInfo Version1 { get; set; }

        /// <summary>
        /// 版本2
        /// </summary>
        public virtual CostDetailVarianceMaterialInfo Version2{ get; set; }

        /// <summary>
        /// 差异
        /// </summary>
        public virtual VarianceCostDetailVarianceMaterialInfo Variance { get; set; }
    }

    /// <summary>
    /// 成本明细差异（物料）字段信息
    /// </summary>
    public class CostDetailVarianceMaterialInfo //: EntityDto<string>
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 超级大种类
        /// </summary>
        public virtual string SuperType { get; set; }

        /// <summary>
        /// 物料大类
        /// </summary>

        public virtual string CategoryName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>

        public virtual string TypeName { get; set; }

        /// <summary>
        /// 装配数量
        /// </summary>
        public virtual double AssemblyCount { get; set; }

        /// <summary>
        /// 材料单价（原币）
        /// </summary>
        public virtual decimal MaterialPrice { get; set; }

        /// <summary>
        /// 材料单价（人民币）
        /// </summary>
        public virtual decimal MaterialPriceCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）
        /// </summary>
        public virtual decimal TotalMoneyCyn { get; set; }

        /// <summary>
        /// 合计金额（原币）（核价表中无，用数量*原币单价计算得到）
        /// </summary>
        public virtual decimal TotalMoney { get; set; }
    }

    public class VarianceCostDetailVarianceMaterialInfo 
    {
        /// <summary>
        /// 装配数量
        /// </summary>
        public virtual string AssemblyCount { get; set; }

        /// <summary>
        /// 材料单价（原币）
        /// </summary>
        public virtual string MaterialPrice { get; set; }

        /// <summary>
        /// 材料单价（人民币）
        /// </summary>
        public virtual string MaterialPriceCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）
        /// </summary>
        public virtual string TotalMoneyCyn { get; set; }

        /// <summary>
        /// 合计金额（原币）（核价表中无，用数量*原币单价计算得到）
        /// </summary>
        public virtual string TotalMoney { get; set; }
    }

}
