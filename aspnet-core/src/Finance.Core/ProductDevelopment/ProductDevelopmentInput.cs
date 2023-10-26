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
    /// 产品开发部用户信息输入信息
    /// </summary>
    [Table("PRODUCTDEVELOPMENTINPUT")]
    public class ProductDevelopmentInput: FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        [Required]
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
        /// 3D爆炸图文件id，关联至filemanagerid
        /// </summary>
        [Required]
        [Column("PICTURE3DFILEID")]
        public long Picture3DFileId { get; set; }

        /// <summary>
        /// 像素（M）
        /// </summary>
        [Column("PIXEL")]
        public double Pixel { get; set; }
        /// <summary>
        /// FOV
        /// </summary>
        [Column("FOV")]
        public double FOV { get; set; }

        /// <summary>
        /// 外包装长
        /// </summary>
        [Column("OUTERPACKAGINGLENGTH")]
        public double OuterPackagingLength { get; set; }
        /// <summary>
        /// 外包装宽
        /// </summary>
        [Column("OUTERPACKAGINGWIDTH")]
        public double OuterPackagingWidth { get; set; }
        /// <summary>
        /// 外包装高
        /// </summary>
        [Column("OUTERPACKAGINGHEIGHT")]
        public double OuterPackagingHeight { get; set; }
        /// <summary>
        /// 单个产品重量
        /// </summary>
        [Column("SINGLEPRODUCTWEIGHT")]
        public double SingleProductWeight { get; set; }
        /// <summary>
        /// 单箱数量
        /// </summary>
        [Column("SINGLEBOXQUANTITY")]
        public int SingleBoxQuantity { get; set; }
        /// <summary>
        /// 内包装长
        /// </summary>
        [Column("INNERPACKAGINGLENGTH")]
        public double InnerPackagingLength { get; set; }
        /// <summary>
        /// 内包装宽
        /// </summary>
        [Column("INNERPACKAGINGWIDTH")]
        public double InnerPackagingWidth { get; set; }
        /// <summary>
        /// 内包装高
        /// </summary>
        [Column("INNERPACKAGINGHEIGHT")]
        public double InnerPackagingHeight { get; set; }
        /// <summary>
        /// 是否打托
        /// </summary>
        [Column("ISHIT")]
        public string IsHit { get; set; }
        /// <summary>
        /// 每托盘箱数
        /// </summary>
        [Column("BOXESPERPALLET")]
        public int BoxesPerPallet { get; set; }
        /// <summary>
        /// 单箱包装数量
        /// </summary>
        [Column("QUANTITYPERBOX")]
        public int QuantityPerBox { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARKS")]
        public string Remarks { get; set; }

        /// <summary>
        /// 电子BOM文件id，关联至filemanagerid
        /// </summary>
        [Required]
        [Column("ELCFILEID")]
        public long ElcFileId { get; set; }
        /// <summary>
        /// 结构BOM文件id，关联至filemanagerid
        /// </summary>
        [Required]
        [Column("STUFILEID")]
        public long StuFileId { get; set; }
    }
}
