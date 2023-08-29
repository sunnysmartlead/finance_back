using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 贸易合规 国家参数基础库
    /// </summary>
    public class CountryLibrary : FullAuditedEntity<long>
    {
        ///// <summary>
        ///// 界面序号
        ///// </summary>
        //[Column("NUM")]
        //public long Num { get; set; }
        /// <summary>
        /// 国家类型
        /// </summary>
        [Column("NATIONALTYPE")]
        public string NationalType { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [Column("COUNTRY")]
        public string Country { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        [Column("RATE")]
        public string Rate { get; set; }
    }
}
