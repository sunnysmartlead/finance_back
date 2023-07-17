using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class CreateCustomerTargetPriceDto
    {
        /// <summary>
        /// 梯度
        /// </summary>
        [Required]
        public virtual decimal Kv { get; set; }


        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 目标价
        /// </summary>
        [Required]
        public virtual string TargetPrice { get; set; }


        /// <summary>
        /// 报价币种（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        [Required]
        public virtual decimal ExchangeRate { get; set; }
    }
}
