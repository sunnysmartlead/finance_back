using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--日志表
    /// </summary>
    [Table("FLogs")]
	public class FoundationLogs : FullAuditedEntity<long>
    {

		/// <summary>
		/// 备注
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 版本
		/// </summary>
		[Column("version")]
		[StringLength(255, ErrorMessage = "版本长度不能超出255字符")]
		public string Version { get; set; }

        /// <summary>
        /// 日志类型 类型1-实验室库环境
        /// </summary>
        [Column("Type")]
        [StringLength(255, ErrorMessage = "版本长度不能超出255字符")]
        public LogType Type { get; set; }
        

    }
    /// <summary>
    /// 日志类型 1 工装  2 EMC  3环境 4 工序 5 设备 6 治具 7 硬件及软件  8 工时库  9 标准工艺库 10 毛利率 11 汇率 12 损耗率 13 质量成本比例 14 贸易合规国家库
    /// </summary>    
    public enum LogType
    {
        /// <summary>
        /// 工装
        /// </summary>
        [Description("工装")]
        WorkClothes=1,
        /// <summary>
        /// EMC
        /// </summary>
        [Description("EMC")]
        EMC=2,
        /// <summary>
        /// 环境
        /// </summary>
        [Description("环境")]
        Environment=3,
        /// <summary>
        /// 工序
        /// </summary>
        [Description("工序")]
        WorkingProcedure = 4,
        /// <summary>
        /// 设备
        /// </summary>
        [Description("设备")]
        Equipment = 5,
        /// <summary>
        /// 治具
        /// </summary>
        [Description("治具")]
        Fixture = 6,
        /// <summary>
        /// 硬件及软件
        /// </summary>
        [Description("硬件及软件")]
        HardwareAndSoftware = 7,
        /// <summary>
        /// 工时库
        /// </summary>
        [Description("工时库")]
        TimeLibrary = 8,
        /// <summary>
        /// 标准工艺库
        /// </summary>
        [Description("标准工艺库")]
        StandardProcessLibrary = 9,
        /// <summary>
        /// 毛利率
        /// </summary>
        [Description("毛利率")]
        GrossProfitMargin=10,
        /// <summary>
        /// 汇率
        /// </summary>
        [Description("汇率")] 
        ExchangeRate =11,
        /// <summary>
        /// 损耗率
        /// </summary>
        [Description("损耗率")]
        LossRate =12,
        /// <summary>
        /// 质量成本比例
        /// </summary>
        [Description("质量成本比例")]
        QualityCostRatio = 13,
        /// <summary>
        /// 贸易合规国家库
        /// </summary>
        [Description("贸易合规国家库")]
        CountryLib = 14,
    }
}
