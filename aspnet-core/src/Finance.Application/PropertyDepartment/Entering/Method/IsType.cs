using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Method
{
    /// <summary>
    /// 用户录入的是原币还是年降 0:根据原币计算年降  1:根据年降计算原币
    /// </summary>
    public enum IsType
    {
        /// <summary>
        /// 根据原币计算年降
        /// </summary>
        [Description("原币")]
        OriginalCurrency,
        /// <summary>
        /// 根据年降计算原币
        /// </summary>
        [Description("年降")]
        AnnualDecline,
    }
}
