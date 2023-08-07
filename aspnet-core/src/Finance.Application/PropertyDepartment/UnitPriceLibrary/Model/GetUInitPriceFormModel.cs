using Finance.FinanceMaintain;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Model
{
    /// <summary>
    /// 查询单价库 模型
    /// </summary>
    public class GetUInitPriceFormModel: UInitPriceForm
    {
        /// <summary>
        /// 冻结状态显示
        /// </summary>     
        public virtual string DisplayFrozenState { get; set; }
    }
}
