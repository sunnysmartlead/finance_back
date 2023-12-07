﻿using Abp.Domain.Entities.Auditing;
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
    /// 产品开发部结构BOM输入信息
    /// </summary>
    [Table("STRUCTUREBOMINFO ")]
    public class StructureBomInfo: FullAuditedEntity<long>
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
        /// 超级大种类
        /// </summary>
        [Column("SUPERTYPENAME")]
        public string SuperTypeName { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Column("IDNUMBER")]
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        [Column("TYPENAME")]
        public string TypeName { get; set; }
        /// <summary>
        /// 是否涉及（必填）
        /// </summary>
        [Column("ISINVOLVEITEM")]
        public string IsInvolveItem { get; set; }
        /// <summary>
        /// 物料编号（SAP）
        /// </summary>
        [Column("SAPITEMNUM")]
        public string SapItemNum { get; set; }
        /// <summary>
        /// 图号名称
        /// </summary>
        [Column("DRAWINGNUMNAME")]
        public string DrawingNumName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        [Column("ASSEMBLYQUANTITY")]
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// 外形尺寸mm
        /// </summary>
        [Column("OVERALLDIMENSIONSIZE")]
        public string OverallDimensionSize { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        [Column("MATERIALNAME")]
        public string MaterialName { get; set; }
        /// <summary>
        /// 重量g
        /// </summary>
        [Column("WEIGHTNUMBER")]
        public double WeightNumber { get; set; }
        /// <summary>
        /// 成型工艺
        /// </summary>
        [Column("MOLDINGPROCESS")]
        public string MoldingProcess { get; set; }
        /// <summary>
        /// 是否新开模
        /// </summary>
        [Column("ISNEWMOULDPRODUCT")]
        public string IsNewMouldProduct { get; set; }
        /// <summary>
        /// 二次加工方法
        /// </summary>
        [Column("SECONDARYPROCESSINGMETHOD")]
        public string SecondaryProcessingMethod { get; set; }
        /// <summary>
        /// 表面处理
        /// </summary>
        [Column("SURFACETREATMENTMETHOD")]
        public string SurfaceTreatmentMethod { get; set; }
        /// <summary>
        /// 关键尺寸精度及重要要求
        /// </summary>
        [Column("DIMENSIONALACCURACYREMARK")]
        public string DimensionalAccuracyRemark { get; set; }
        /// <summary>
        /// 电子BOM文件Id
        /// </summary>
        [Column("FILEID")]
        public long FileId { get; set; }

    }
}
