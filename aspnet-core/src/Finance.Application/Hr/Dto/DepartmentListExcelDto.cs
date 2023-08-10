using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    /// <summary>
    /// 导出Excel所用的部门Dto
    /// </summary>
    public class DepartmentListExcelDto
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [ExcelColumn(Name = "部门名称")]
        public virtual string Name { get; set; }


        /// <summary>
        /// 用点（.）分隔的路径，部门名称，是根部门，此字段为空。此字段是以Fid计算得到，目的是缩短查询的时间和复杂度。
        /// </summary>
        [ExcelColumn(Name = "部门名称", Width = 60)]
        public virtual string PathName { get; set; }
    }
}
