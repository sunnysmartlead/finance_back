using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre 录入页面唯一值
    /// </summary>
    public enum NreIsSubmitDto
    {
        /// <summary>
        /// 项目管理部
        /// </summary>
        [Description("项目管理部")]
        ProjectManagement,
        /// <summary>
        /// 资源部模具费
        /// </summary>
        [Description("资源部")]
        ResourcesManagement,
        /// <summary>
        /// 产品部
        /// </summary>
        [Description("产品部")]
        ProductDepartment,
        /// <summary>
        /// 品保部页面 环境实验费
        /// </summary>
        [Description("环境实验费")]
        EnvironmentalExperimentFee,
        /// <summary>
        /// 品保部页面2 项目制程QC量检具
        /// </summary>
        [Description("品保部页面2 项目制程QC量检具")]
        QRA2,
        /// <summary>
        /// 物流信息录入 
        /// </summary>
        [Description("物流信息录入")]
        Logisticscost,
        /// <summary>
        /// COB制造成本录入
        /// </summary>
        [Description("COB制造成本录入")]
        COB,
        /// <summary>
        /// 工时工序
        /// </summary>
        [Description("工时工序")]
        ProcessHoursEnter,

    }
}
