using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering
{
    //单价提交记录
    internal static class ResourceSubmitLog
    {
        //结构单价提价记录
        internal static List<long> IsPostStructural = new();
        //电子单价提价记录
        internal static List<long> IsPostElectronic = new();
    }
}
