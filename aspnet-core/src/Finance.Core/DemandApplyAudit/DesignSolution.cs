using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.DemandApplyAudit
{
    /// <summary>
    /// 营销部审核中项目设计方案
    /// </summary>
    public class DesignSolution : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SolutionName { get; set; }
        /// <summary>
        /// sensor
        /// </summary>
        public string Sensor { get; set; }
        /// <summary>
        /// serial
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// lens
        /// </summary>
        public string Lens { get; set; }
        /// <summary>
        /// lsp
        /// </summary>
        public string ISP { get; set; }
        /// <summary>
        /// vcsel
        /// </summary>
        public string Vcsel { get; set; }
        /// <summary>
        /// MCU
        /// </summary>
        public string MCU { get; set; }
        /// <summary>
        /// 线束
        /// </summary>
        public string Harness { get; set; }
        /// <summary>
        /// 支架
        /// </summary>
        public string Stand { get; set; }
        /// <summary>
        /// 传动结构
        /// </summary>
        public string TransmissionStructure { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }
        /// <summary>
        /// 工艺方案
        /// </summary>
        public string ProcessProgram { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        public string Rests { get; set; }
        /// <summary>
        /// 3D爆炸图ID
        /// </summary>
        public long FileId { get; set; }
    }
}
