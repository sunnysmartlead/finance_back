using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    /// <summary>
    /// 设备状态类型编号
    /// </summary>
    public enum Status
    {

        [Description("专用)")]
        zy,

        [Description("共用")]
        gy,
        [Description("现有")]
        xy,
        [Description("新购")]
        xg,
        [Description("改造 ")]
        gz,

    }
}
