using Finance.PropertyDepartment.Entering.Model;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Model
{
    /// <summary>
    /// 公共物料库 模型
    /// </summary>
    public class SharedMaterialWarehouseMode
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [ExcelColumnName("项目名称")]
        public string EntryName { get; set; }
        /// <summary>
        /// 项目子代码
        /// </summary>
        [ExcelColumnName("项目子代码")]
        public string ProjectSubcode { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [ExcelColumnName("产品名称")]
        public string ProductName { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [ExcelColumnName("物料编码")]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [ExcelColumnName("物料名称")]
        public string MaterialName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        [ExcelColumnName("装配数量")]
        public decimal AssemblyQuantity { get; set; }
        /// <summary>
        /// 模组走量
        /// </summary>
        public List<YearOrValueModeCanNull> ModuleThroughputs { get; set; }
    }
    /// <summary>
    /// 键值对 一个年份对应一个值可为空 模型
    /// </summary>
    public class YearOrValueModeCanNull
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal? Value { get; set; }
    }
}
