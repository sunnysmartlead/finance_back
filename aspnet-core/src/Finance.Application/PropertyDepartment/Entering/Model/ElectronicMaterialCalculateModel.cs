using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Entering.Model
{
    /// <summary>
    /// 计算电子料单价录入  模型
    /// </summary>
    public class ElectronicMaterialCalculateModel
    {
        /// <summary>
        /// 零件的ID
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 电子料Bom表单的id
        /// </summary>
        public long BomId { get; set; }
        /// <summary>
        /// 项目物料的使用量
        /// </summary>
        public List<YearOrValueMode> ElectronicMaterialsUseCount { get; set; }
        /// <summary>
        /// 原币
        /// </summary>
        public List<YearOrValueMode> ElectronicOriginalCurrency { get; set; }
        /// <summary>
        /// 本位币
        /// </summary>
        public List<YearOrValueMode> ElectronicStandardMoney { get; set; }

    } 
 
}
