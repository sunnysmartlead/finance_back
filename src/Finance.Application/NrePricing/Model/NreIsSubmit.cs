﻿using System;
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
        /// 资源部
        /// </summary>
        [Description("资源部")]
        ResourcesManagement,
        /// <summary>
        /// 产品部
        /// </summary>
        [Description("产品部")]
        ProductDepartment,
        /// <summary>
        /// 品保部页面1 试验项目
        /// </summary>
        [Description("品保部页面1 试验项目")]
        QRA1,
        /// <summary>
        /// 品保部页面2 项目制程QC量检具
        /// </summary>
        [Description("品保部页面2 项目制程QC量检具")]
        QRA2,
       
    }
}
