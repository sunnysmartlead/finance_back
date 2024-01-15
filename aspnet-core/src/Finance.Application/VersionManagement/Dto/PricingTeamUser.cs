using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.VersionManagement.Dto
{
    /// <summary>
    /// 核价团队人员
    /// </summary>
    internal class PricingTeamUser
    {
        /// <summary>
        /// 工程技术部-工序工时录入员
        /// </summary>
        public string Engineer { get; set; }
        /// <summary>
        ///品质保证部-实验费用录人员
        /// </summary>
        public string QualityBench { get; set; }
        /// <summary>
        ///产品开发部-EMC实验费用录入员
        /// </summary>
        public string EMC { get; set; }
        /// <summary>
        /// 财务部-制造费用录入员
        /// </summary>
        public string ProductCostInput { get; set; }
        /// <summary>
        /// 生产管理部-物流费用录入员
        /// </summary>
        public string ProductManageTime { get; set; }
        /// <summary>
        /// 项目核价审核员
        /// </summary>
        public string Audit { get; set; }
    }
}
