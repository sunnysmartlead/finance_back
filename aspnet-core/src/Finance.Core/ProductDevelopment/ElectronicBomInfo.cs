    using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// 产品开发部电子BOM输入信息
    /// </summary>
    [Table("ELECTRONICBOMINFO ")]
    public class ElectronicBomInfo: FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>

        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }

        /// <summary>
        /// ModelCount表id
        /// </summary>
        [Column("PRODUCTID")]
        public long ProductId { get; set; }

        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        [Column("PRODUCT")]
        public string Product { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        [Column("SOLUTIONID")]
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        [Column("SOLUTIONNUM")]
        public string SolutionNum { get; set; }

        /// <summary>
        /// 序号
        /// </summary>

        [Column("IDNUMBER")]
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        
        [Column("TYPENAME")]
        public string TypeName { get; set; }
        /// <summary>
        /// 是否涉及（必填）
        /// </summary>
        
        [Column("ISINVOLVEITEM")]
        public string IsInvolveItem { get; set; }
        /// <summary>
        /// 物料编号（SAP）
        /// </summary>
        
        [Column("SAPITEMNUM")]
        public string SapItemNum { get; set; }
        /// <summary>
        /// 材料名称
        /// </summary>
        
        [Column("SAPITEMNAME")]
        public string SapItemName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        
        [Column("ASSEMBLYQUANTITY")]
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// 封装(需要体现PAD的数量)
        /// </summary>
        
        [Column("ENCAPSULATIONSIZE")]
        public string EncapsulationSize { get; set; }



    }
}
