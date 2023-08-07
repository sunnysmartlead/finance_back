using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    ///Nre核价  资源部录入 交互类
    /// </summary>
    public class ResourcesManagementDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// Nre核价  资源部录入 实体
        /// </summary>
        public List<ResourcesManagementModel> ResourcesManagementModels { get; set; }
    }
    /// <summary>
    ///Nre核价  资源部录入 交互类
    /// </summary>
    public class ResourcesManagementSingleDto
    {      
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// Nre核价  资源部录入 实体
        /// </summary>
        public ResourcesManagementModel ResourcesManagementModels { get; set; }
        /// <summary>
        /// true/提交  flase/保存
        /// </summary>
        public bool IsSubmit { get; set; } = true;
    }
}
