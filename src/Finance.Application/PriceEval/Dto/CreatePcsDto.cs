﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建Pcs的Dto
    /// </summary>
    public class CreatePcsDto
    {
        /// <summary>
        /// 车厂
        /// </summary>
        [Required]
        public virtual string CarFactory { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        [Required]
        public virtual string CarModel { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual List<CreatePcsYearDto> PcsYearList { get; set; }
    }

    /// <summary>
    /// 创建Pcs年份的Dto
    /// </summary>
    public class CreatePcsYearDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Range(FinanceConsts.MinYear, FinanceConsts.MaxYear)]
        public virtual int Year { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public virtual int Quantity { get; set; }
    }
}
