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
    /// 产品信息表
    /// </summary>
    [Table("Pe_ProductInformation")]
    public class ProductInformation : FullAuditedEntity<long>
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
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// 客户目标价
        /// </summary>
        [Required]
        public virtual decimal CustomerTargetPrice { get; set; }

        /// <summary>
        /// Sensor
        /// </summary>
        public virtual string Sensor { get; set; }

        /// <summary>
        /// Sensor类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string SensorTypeSelect { get; set; }

        /// <summary>
        /// Sensor单价
        /// </summary>
        public virtual decimal? SensorPrice { get; set; }

        /// <summary>
        /// Lens
        /// </summary>
        public virtual string Lens { get; set; }

        /// <summary>
        /// Lens类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string LensTypeSelect { get; set; }

        /// <summary>
        /// Lens单价
        /// </summary>
        public virtual decimal? LensPrice { get; set; }

        /// <summary>
        /// Isp
        /// </summary>
        public virtual string Isp { get; set; }

        /// <summary>
        /// Isp类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string IspTypeSelect { get; set; }

        /// <summary>
        /// Isp单价
        /// </summary>
        public virtual decimal? IspPrice { get; set; }

        /// <summary>
        /// 串行芯片
        /// </summary>
        public virtual string SerialChip { get; set; }

        /// <summary>
        /// 串行芯片 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string SerialChipTypeSelect { get; set; }

        /// <summary>
        /// 串行芯片 单价
        /// </summary>
        public virtual decimal? SerialChipPrice { get; set; }

        /// <summary>
        /// 线缆
        /// </summary>
        public virtual string Cable { get; set; }

        /// <summary>
        /// 线缆 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string CableTypeSelect { get; set; }

        /// <summary>
        /// 线缆 单价
        /// </summary>
        public virtual decimal? CablePrice { get; set; }

        /// <summary>
        /// LED
        /// </summary>
        public virtual string Led { get; set; }

        /// <summary>
        /// LED 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string LedTypeSelect { get; set; }

        /// <summary>
        /// LED 单价
        /// </summary>
        public virtual decimal? LedPrice { get; set; }

        /// <summary>
        /// 制程
        /// </summary>
        public virtual string ManufactureProcess { get; set; }

        /// <summary>
        /// 安装位置
        /// </summary>
        public virtual string InstallationPosition { get; set; }


    }
}
