﻿using Abp.Domain.Entities.Auditing;
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
    /// 梯度
    /// </summary>
    [Table("Pe_Gradient")]
    public class Gradient : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 梯度序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 梯度(K/Y)
        /// </summary>
        public virtual decimal GradientValue { get; set; }

        /// <summary>
        /// 显示用梯度
        /// </summary>
        public virtual decimal DisplayGradientValue { get; set; }


        /// <summary>
        /// 系统取梯度（这里单词存在拼写错误）（应该废弃）
        /// </summary>
        public virtual decimal SystermGradientValue { get; set; }
    }

    /// <summary>
    /// 梯度模组
    /// </summary>
    [Table("Pe_GradientModel")]
    public class GradientModel : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 产品大类
        /// </summary>
        public virtual string Type { get; set; }
    }

    /// <summary>
    /// 梯度模组年份
    /// </summary>
    [Table("Pe_GradientModelYear")]
    public class GradientModelYear : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 梯度模组Id
        /// </summary>
        public virtual long GradientModelId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 梯度走量
        /// </summary>
        public virtual decimal Count { get; set; }


    }
}
