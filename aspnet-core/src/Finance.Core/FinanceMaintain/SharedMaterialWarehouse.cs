using Abp.Domain.Entities.Auditing;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceMaintain
{
    /// <summary>
    /// 共用物料库 实体类
    /// </summary>
    public class SharedMaterialWarehouse : FullAuditedEntity<long>
    {
        /// <summary>
        /// 项目名称
        /// </summary>       
        public string EntryName { get; set; }
        /// <summary>
        /// 项目子代码
        /// </summary>       
        public string ProjectSubcode { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>       
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>      
        public string MaterialName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>       
        public decimal AssemblyQuantity { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 模组走量(存JSON) 实体类为YearOrValueModeCanNull
        /// </summary>
        public string ModuleThroughputs { get; set; }
    }
}
