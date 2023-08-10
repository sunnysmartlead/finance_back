using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 品保部  项目制程QC量检具 录入交互类
    /// </summary>
    public class QcGaugeDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public List<QcGaugeDtoModel> QcGaugeDtoModels { get; set; }
    }
    /// <summary>
    /// Nre 品保部  项目制程QC量检具 录入交互类(单个零件)
    /// </summary>
    public class QcGaugeDtoSingle
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public QcGaugeDtoModel QcGaugeDtoModels { get; set; }
    }
    /// <summary>
    /// 带零件id 的 项目制程QC量检具 录入 模型
    /// </summary>
    public class QcGaugeDtoModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 品保录入  项目制程QC量检具表 模型
        /// </summary>
        public List<QADepartmentQCModel> QAQCDepartments { get; set; }
        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }
}
