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
    public class BoardInfo: FullAuditedEntity<long>
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
        /// 板部件序号
        /// </summary>

        [Column("BOARDID")]
        public long BoardId { get; set; }
        /// <summary>
        /// 板部件名称
        /// </summary>

        [Column("BOARDNAME")]
        public string BoardName { get; set; }
        /// <summary>
        /// 板部件长
        /// </summary>

        [Column("BOARDLENTH")]
        public decimal BoardLenth { get; set; }
        /// <summary>
        /// 板部件宽
        /// </summary>

        [Column("BOARDWIDTH")]
        public decimal BoardWidth { get; set; }
        /// <summary>
        /// 板部件面积
        /// </summary>

        [Column("BOARDSQUARE")]
        public decimal BoardSquare { get; set; }
        /// <summary>
        /// 拼版数量
        /// </summary>

        [Column("STONEQUANTITY")]
        public long StoneQuantity { get; set; }
    }
}
