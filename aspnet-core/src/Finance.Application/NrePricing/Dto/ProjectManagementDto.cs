using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre核价 项目管理部 交互类
    /// </summary>
    public class ProjectManagementDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        ///  Nre核价 项目管理部 实体
        /// </summary>
        public List<ProjectManagementModel> projectManagements { get; set; }
    }
    /// <summary>
    /// Nre核价 项目管理部 交互类(单个零件)
    /// </summary>
    public class ProjectManagementDtoSingle
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        ///  Nre核价 项目管理部 实体
        /// </summary>
        public ProjectManagementModel projectManagement { get; set; }
    }
}
