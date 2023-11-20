using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    public class AfterUpdateSumInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }
        /// <summary>
        /// 年份
        /// </summary>

        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 质量成本修改后合计
        /// </summary>
        public virtual decimal QualityCostAfterSum { get; set; }
        /// <summary>
        /// 损耗成本修改后合计
        /// </summary>
        public virtual decimal LossCostAfterSum { get; set; }
        /// <summary>
        /// 制造成本修改后合计
        /// </summary>
        public virtual decimal ManufacturingAfterSum { get; set; }
        /// <summary>
        /// 物流成本修改后合计
        /// </summary>
        public virtual decimal LogisticsAfterSum { get; set; }
        /// <summary>
        /// 其他流成本修改后合计
        /// </summary>
        public virtual decimal OtherCosttAfterSum { get; set; }
    }
}
