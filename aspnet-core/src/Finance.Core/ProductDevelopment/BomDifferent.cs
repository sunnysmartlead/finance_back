using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// 表示两次BOM表差异中增删改的类型
    /// </summary>
    public enum MODIFYTYPE : byte
    {
        /// <summary>
        ///BOM表中增加了一条记录
        /// </summary>
        [Description("增加一条记录")]
        ADDNEWDATA = 1,
        /// <summary>
        /// BOM表中删除了一条记录
        /// </summary>
        [Description("删除一条记录")]
        DELNEWDATA = 2,
        /// <summary>
        /// BOM表中修改了一条记录
        /// </summary>
        [Description("修改一条记录")]
        MODIFYNEWDATA = 3,
    }
    /// <summary>
    /// 电子BOM两次上传差异化表
    /// </summary>
    public class ElecBomDifferent : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id (零件ID)
        /// </summary>
        [Column("PRODUCTID")]
        public long ProductId { get; set; }
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
        /// 电子bom单价表单id
        /// </summary>
        [Column("ELECTRONICID")]
        public long ElectronicId { get; set; }
        /// <summary>
        /// 电子BOM表差异类型
        /// </summary>
        [Column("MODIFYTYPEVALUE")]
        public MODIFYTYPE ModifyTypeValue { get; set; }
        
    }

    /// <summary>
    /// 结构BOM两次上传差异化表
    /// </summary>
    public class StructBomDifferent : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id (零件ID)
        /// </summary>
        [Column("PRODUCTID")]
        public long ProductId { get; set; }

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
        /// 结构bom单价表单id
        /// </summary>
        [Column("STRUCTUREID")]
        public long StructureId { get; set; }
        /// <summary>
        /// 结构BOM表差异类型
        /// </summary>
        [Column("MODIFYTYPEVALUE")]
        public MODIFYTYPE ModifyTypeValue { get; set; }
    }

    /// <summary>
    /// 产品开发部电子BOM输入信息备份
    /// </summary>
    public class ElectronicBomInfoBak : FullAuditedEntity<long>
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
        [Required]
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
        /// 材料名称
        /// </summary>
        [Column("SAPITEMNAME")]
        public string SapItemName { get; set; }

        /// <summary>
        /// 装配数量
        /// </summary>
        [Column("ASSEMBLYQUANTITY")]
        public double AssemblyQuantity { get; set; }

        /// <summary>
        /// 封装(需要体现PAD的数量)
        /// </summary>
        [Column("ENCAPSULATIONSIZE")]
        public string EncapsulationSize { get; set; }
    }

    /// <summary>
    /// 产品开发部结构BOM输入信息备份
    /// </summary>
    public class StructureBomInfoBak : FullAuditedEntity<long>
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
        [Required]
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
    }
}
