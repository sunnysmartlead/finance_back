using MiniExcelLibs.Attributes;
using NPOI.SS.Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.MoqFormulas
{
    /// <summary>
    /// 接收Excel数据的Dto
    /// </summary>
    public class MoqFormulaDto
    {
        /// <summary>
        /// 公式1
        /// </summary>
        public const string Formula1 = "等于大于MOQ，但是要等于最小包装的倍数";

        /// <summary>
        /// 公式2
        /// </summary>
        public const string Formula2 = "等于大于MOQ即可";

        /// <summary>
        /// Pcb
        /// </summary>
        public const string Pcb = "PCB";

        /// <summary>
        /// 物料大类
        /// </summary>
        [ExcelColumn(Name = "物料大类", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string CategoryName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
        [ExcelColumn(Name = "物料种类", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string TypeName { get; set; }

        /// <summary>
        /// 公式
        /// </summary>
        [ExcelColumn(Name = "采购量与MOQ的关系", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string Formula { get; set; }
    }
}
