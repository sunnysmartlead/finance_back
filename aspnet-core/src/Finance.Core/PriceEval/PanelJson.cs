﻿using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价看板数据
    /// </summary>
    [Table("Pe_PanelJson")]
    public class PanelJson : Entity<long>
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public virtual long SolutionId { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        [Required]
        public virtual int InputCount { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 数据缓存
        /// </summary>
        [Column(TypeName = "CLOB")]
        public virtual string DataJson { get; set; }
    }
}
