using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// Nre 项目管理部 手板件 修改项实体类
    /// </summary>
    public class HandPieceCostModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }     
        /// <summary>
        /// 料号
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
