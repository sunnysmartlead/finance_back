using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    /// <summary>
    /// 操作类型编号
    /// </summary>
    public enum UPH
    {

        [Description("SMT-UPH值)")]
        smtuph,

        [Description("COB-UPH值")]
        cobuph,
        [Description("组测UPH值")]
        zcuph,

    }
}
