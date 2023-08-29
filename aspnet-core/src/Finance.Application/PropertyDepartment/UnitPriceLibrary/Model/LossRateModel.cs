using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Model
{
    /// <summary>
    /// 损耗率模型
    /// </summary>
    public class LossRateModel
    {
        /// <summary>
        /// 产品大类
        /// </summary>
        [ExcelColumnName("产品大类")]
        public string SuperType { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        [ExcelColumnName("物料大类")]
        public string MaterialCategory { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        [ExcelColumnName("物料种类")]
        public string CategoryName { get; set; }
    }
    /// <summary>
    /// 损耗率模型
    /// </summary>
    public class LossRateSopModel
    {
        /// <summary>
        /// Sop
        /// </summary>
        [ExcelColumnName("SOP")]
        public decimal Sop { get; set; }
        /// <summary>
        /// Sop1
        /// </summary>
        [ExcelColumnName("SOP+1")]
        public decimal Sop1 { get; set; }
        /// <summary>
        /// Sop2
        /// </summary>
        [ExcelColumnName("SOP+2")]
        public decimal Sop2 { get; set; }
        /// <summary>
        /// Sop3
        /// </summary>
        [ExcelColumnName("SOP+3")]
        public decimal Sop3 { get; set; }
        /// <summary>
        /// Sop4
        /// </summary>
        [ExcelColumnName("SOP+4")]
        public decimal Sop4 { get; set; }
        /// <summary>
        /// Sop5
        /// </summary>
        [ExcelColumnName("SOP+5")]
        public decimal Sop5 { get; set; }
        /// <summary>
        /// Sop6
        /// </summary>
        [ExcelColumnName("SOP+6")]
        public decimal Sop6 { get; set; }
        /// <summary>
        /// Sop7
        /// </summary>
        [ExcelColumnName("SOP+7")]
        public decimal Sop7 { get; set; }
        /// <summary>
        /// Sop8
        /// </summary>
        [ExcelColumnName("SOP+8")]
        public decimal Sop8 { get; set; }
        /// <summary>
        /// Sop9
        /// </summary>
        [ExcelColumnName("SOP+9")]
        public decimal Sop9 { get; set; }
        /// <summary>
        /// Sop10
        /// </summary>
        [ExcelColumnName("SOP+10")]
        public decimal Sop10 { get; set; }
    }
}
