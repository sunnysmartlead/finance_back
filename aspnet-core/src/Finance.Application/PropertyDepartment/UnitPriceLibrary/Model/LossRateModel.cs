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
        [ExcelColumnIndex("C")]
        public decimal Sop { get; set; }
        /// <summary>
        /// Sop1
        /// </summary>
        [ExcelColumnIndex("D")]
        public decimal Sop1 { get; set; }
        /// <summary>
        /// Sop2
        /// </summary>
        [ExcelColumnIndex("E")]
        public decimal Sop2 { get; set; }
        /// <summary>
        /// Sop3
        /// </summary>
        [ExcelColumnIndex("F")]
        public decimal Sop3 { get; set; }
        /// <summary>
        /// Sop4
        /// </summary>
        [ExcelColumnIndex("G")]
        public decimal Sop4 { get; set; }
        /// <summary>
        /// Sop5
        /// </summary>
        [ExcelColumnIndex("H")]
        public decimal Sop5 { get; set; }
        /// <summary>
        /// Sop6
        /// </summary>
        [ExcelColumnIndex("I")]
        public decimal Sop6 { get; set; }
        /// <summary>
        /// Sop7
        /// </summary>
        [ExcelColumnIndex("J")]
        public decimal Sop7 { get; set; }
        /// <summary>
        /// Sop8
        /// </summary>
        [ExcelColumnIndex("K")]
        public decimal Sop8 { get; set; }
        /// <summary>
        /// Sop9
        /// </summary>
        [ExcelColumnIndex("L")]
        public decimal Sop9 { get; set; }
        /// <summary>
        /// Sop10
        /// </summary>
        [ExcelColumnIndex("M")]
        public decimal Sop10 { get; set; }
    }
}
