using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProjectManagement
{
    /// <summary>
    /// 项目管理部人员输入
    /// </summary>
     [Table("USERINPUTINFO ")]
    public class UserInputInfo: FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id
        /// </summary>
        [Column("PRODUCTID")]
        public long? ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1，零件2...）
        /// </summary>
        [Column("PRODUCT")]
        public string Product { get; set; }
        /// <summary>
        /// 产品部-电子工程师（USER表ID）
        /// </summary>
        [Column("ELECTRONIC")]
        public int ElectronicEngineerId { get; set; }
        /// <summary>
        /// 产品部-结构工程师（USER表ID）
        /// </summary>
        [Column("STRUCTURE")]
        public int StructureEngineerId { get; set; }
        /// <summary>
        /// 资源管理部-电子资源开发
        /// </summary>
        [Column("RESOURCEELECID")]
        public int ResourceElecId { get; set; }
        /// <summary>
        /// 资源管理部-结构资源开发
        /// </summary>
        [Column("RESOURCESTRUCTID")]
        public int ResourceStructId { get; set; }

        /// <summary>
        /// 工程技术部-损耗率录入员
        /// </summary>
        [Column("ENGINEERLOSSRATEID")]
        public int EngineerLossRateId { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员
        /// </summary>
        [Column("ENGINEERWORKHOURID")]
        public int EngineerWorkHourId { get; set; }
        /// <summary>
        /// 品质保证部-实验费用录入员
        /// </summary>
        [Column("QUAKITYBENCHID")]
        public int QualityBenchId { get; set; }
        /// <summary>
        /// 品质保证部-检具费用录入员
        /// </summary>
        [Column("QUAKITYTOOLID")]
        public int QualityToolId { get; set; }
        /// <summary>
        /// 生产管理部
        /// </summary>
        [Column("PRODUCTIONID")]
        public int ProductManageId { get; set; }
        /// <summary>
        /// 项目部核价审核员
        /// </summary>
        [Column("PROJECTAUDITOR")]
        public int ProjectAuditorId { get; set; }
        /// <summary>
        /// 是否首个
        /// </summary>
        [Column("ISFIRST")]
        public bool IsFirst { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Column("FILEID")]
        public long FileId { get; set; }

        /// <summary>
        /// TR预计提交时间
        /// </summary>
        [Column("TRSUBMITTIME")]
        public DateTime TRSubmitTime { get; set; }
        /// <summary>
        /// 产品部-电子工程师期望完成时间
        /// </summary>
        [Column("ELECENGINEERTIME")]
        public DateTime ElecEngineerTime { get; set; }
        /// <summary>
        /// 产品部-结构工程师期望完成时间
        /// </summary>
        [Column("STRUCTENGINEERTIME")]
        public DateTime StructEngineerTime { get; set; }
        /// <summary>
        /// 品质保证部-实验室费用录入员期望完成时间
        /// </summary>
        [Column("QUALITYBENCHTIME")]
        public DateTime QualityBenchTime { get; set; }
        /// <summary>
        /// 品质保证部-检具费用录入员期望完成时间
        /// </summary>
        [Column("QUALITYTOOLTIME")]
        public DateTime QualityToolTime { get; set; }
        /// <summary>
        /// 资源管理部-电子资源开发期望完成时间
        /// </summary>
        [Column("RESOURCEELECTIME")]
        public DateTime ResourceElecTime { get; set; }
        /// <summary>
        /// 资源管理部-结构资源开发期望完成时间
        /// </summary>
        [Column("RESOURCESTRUCTTIME")]
        public DateTime ResourceStructTime { get; set; }
        /// <summary>
        /// 工程技术部-损耗率录入员期望完成时间
        /// </summary>
        [Column("ENGINEERLOSSRATETIME")]
        public DateTime EngineerLossRateTime { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员期望完成时间
        /// </summary>
        [Column("ENGINEERWORKHOURTIME")]
        public DateTime EngineerWorkHourTime { get; set; }
        /// <summary>
        /// 生成管理部-物流成本录入员期望完成时间
        /// </summary>
        [Column("PRODUCTMANAGETIME")]
        public DateTime ProductManageTime { get; set; }
        /// <summary>
        /// 制造成本录入员期望完成时间
        /// </summary>
        [Column("PRODUCTCOSTINPUTTIME")]
        public DateTime ProductCostInputTime { get; set; }
    }
}
