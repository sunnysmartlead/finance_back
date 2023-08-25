using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    /// <summary>
    /// 创建项目自建表记录输入
    /// </summary>
    public class CreateProjectSelfInput
    {
        /// <summary>
        /// 客户
        /// </summary>
        [ExcelColumn(Name = "客户",Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string Custom { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [ExcelColumn(Name = "客户名称", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string CustomName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        [ExcelColumn(Name = "项目代码", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        [ExcelColumn(Name = "项目描述", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string Description { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        [ExcelColumn(Name = "子项目代码", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string SubCode { get; set; }

        /// <summary>
        /// 子项目描述
        /// </summary>
        [ExcelColumn(Name = "子项目描述", Width = FinanceConsts.ExcelColumnWidth)]
        public virtual string SubDescription { get; set; }
    }
}
