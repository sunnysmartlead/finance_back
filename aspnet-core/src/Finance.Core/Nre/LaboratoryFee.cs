using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 实验费  实体类
    /// </summary>
    public class LaboratoryFee : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }
        /// <summary>
        /// 实验项目
        /// </summary>
        public string TestItem { get; set; }
        /// <summary>
        /// 是否指定第三方
        /// </summary>
        public bool IsThirdParty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 调整系数
        /// </summary>
        public decimal Coefficient { get; set; }
        /// <summary>
        /// 计价单位
        /// </summary>
        public decimal Unit { get; set; }
        /// <summary>
        /// 时间-摸底
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DataThoroughly { get; set; }
        /// <summary>
        /// 时间-DV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DataDV { get; set; }
        /// <summary>
        /// 时间-PV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DataPV { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>
        public decimal AllCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
