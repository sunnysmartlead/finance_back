using Abp.Domain.Entities.Auditing;
using Finance.Dto;
using Finance.Ext;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.DemandApplyAudit.Method;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.DemandApplyAudit.Dto
{
    /// <summary>
    /// 营销部审核 录入类
    /// </summary>
    public class AuditEntering: ToExamineDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程ID")]
        public virtual long AuditFlowId { get; set; }
        /// <summary>
        /// 核价团队  其中包含(核价人员以及对应完成时间)
        /// </summary>
        public PricingTeamDto PricingTeam { get; set; }
        /// <summary>
        /// 设计方案
        /// </summary>
        [FriendlyRequired("设计方案")]
        public List<DesignSolutionDto> DesignSolutionList { get; set; }
        /// <summary>
        /// 营销部审核 方案表
        /// </summary>  
        [FriendlyRequired("方案表")]
        public List<SolutionTableDto> SolutionTableList { get; set; }        
        
    }
    /// <summary>
    /// 营销部审核中项目设计方案
    /// </summary>
    public class DesignSolutionDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long  Id { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        [FriendlyRequired("设计方案-方案名称")]
        public string SolutionName { get; set; }
        /// <summary>
        /// sensor
        /// </summary>        
        public string Sensor { get; set; }
        /// <summary>
        /// serial
        /// </summary>      
        public string Serial { get; set; }
        /// <summary>
        /// lens
        /// </summary>        
        public string Lens { get; set; }
        /// <summary>
        /// lsp
        /// </summary>        
        public string ISP { get; set; }
        /// <summary>
        /// vcsel
        /// </summary>        
        public string Vcsel { get; set; }
        /// <summary>
        /// MCU
        /// </summary>      
        public string MCU { get; set; }
        /// <summary>
        /// 连接器
        /// </summary>
        public string Connector { get; set; }
        /// <summary>
        /// 线束
        /// </summary>        
        public string Harness { get; set; }
        /// <summary>
        /// 支架
        /// </summary>        
        public string Stand { get; set; }
        /// <summary>
        /// 传动结构
        /// </summary>       
        public string TransmissionStructure { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>     
        public string ProductType { get; set; }
        /// <summary>
        /// 工艺方案
        /// </summary>       
        public string ProcessProgram { get; set; }
        /// <summary>
        /// 其他
        /// </summary>        
        public string Rests { get; set; }
        /// <summary>
        /// 3D爆炸图ID
        /// </summary>
        [FriendlyRequired("设计方案-3D爆炸图")]
        public long FileId { get; set; }
        /// <summary>
        /// 上传的文件信息（只有Id和FileName有值）
        /// </summary>
        public virtual FileUploadOutputDto File { get; set; }
    }
    /// <summary>
    /// 营销部审核中项目设计方案 导入模型
    /// </summary>
    public class DesignSolutionModel
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        [ExcelColumnName("方案名称")]
        public string SolutionName { get; set; }
        /// <summary>
        /// sensor
        /// </summary>
        [ExcelColumnName("SENSOR")]
        public string Sensor { get; set; }
        /// <summary>
        /// serial
        /// </summary>
        [ExcelColumnName("Serial")]
        public string Serial { get; set; }
        /// <summary>
        /// lens
        /// </summary>
        [ExcelColumnName("lens")]
        public string Lens { get; set; }
        /// <summary>
        /// ISP
        /// </summary>
        [ExcelColumnName("ISP")]
        public string ISP { get; set; }
        /// <summary>
        /// vcsel
        /// </summary>
        [ExcelColumnName("vcsel/LED")]
        public string Vcsel { get; set; }
        /// <summary>
        /// MCU
        /// </summary>
        [ExcelColumnName("MCU")]
        public string MCU { get; set; }
        /// <summary>
        /// 连接器
        /// </summary>
        [ExcelColumnName("连接器")]
        public string Connector { get; set; }
        /// <summary>
        /// 线束
        /// </summary>
        [ExcelColumnName("线束")]
        public string Harness { get; set; }
        /// <summary>
        /// 支架
        /// </summary>
        [ExcelColumnName("支架")]
        public string Stand { get; set; }
        /// <summary>
        /// 传动结构
        /// </summary>
        [ExcelColumnName("传动结构")]
        public string TransmissionStructure { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        [ExcelColumnName("产品类型")]
        public string ProductType { get; set; }
        /// <summary>
        /// 工艺方案
        /// </summary>
        [ExcelColumnName("工艺方案")]
        public string ProcessProgram { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        [ExcelColumnName("其他")]
        public string Rests { get; set; }       
    }
    /// <summary>
    /// 营销部审核 方案表
    /// </summary>
    public class SolutionTableDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 模组id
        /// </summary>
        [FriendlyRequired("前端方案表没有的模组ID")]
        public long Productld { get; set; }
        /// <summary>
        /// 模组名称
        /// </summary>
        [FriendlyRequired("方案表-模组名称")]
        public string ModuleName { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        [FriendlyRequired("方案表-方案名称")]
        public string SolutionName { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [FriendlyRequired("方案表-产品名称")]
        public string Product { get; set; }
        /// <summary>
        /// 是否COB方案
        /// </summary>
        public bool IsCOB { get; set; }
        /// <summary>
        /// 电子工程师(用户ID)
        /// </summary>
        [FriendlyRequired("方案表-电子工程师")]
        public long? ElecEngineerId { get; set; }
        /// <summary>
        /// 结构工程师(用户ID)
        /// </summary>
        [FriendlyRequired("方案表-结构工程师")]
        public long? StructEngineerId { get; set; }
        /// <summary>
        /// 是否为首款产品
        /// </summary>        
        public bool IsFirst { get; set; }
    }
    /// <summary>
    /// 核价团队  其中包含(核价人员以及对应完成时间)
    /// </summary>
    public class PricingTeamDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员
        /// </summary>
        [FriendlyRequired("工序工时录入员")]
        public long? EngineerId { get; set; }
        /// <summary>
        ///品质保证部-可靠性实验费用录人员
        /// </summary>
        [FriendlyRequired("品质保证部-可靠性实验费用录人员")]
        public long? QualityBenchId { get; set; }
        /// <summary>
        ///产品开发部-EMC实验费用录入员
        /// </summary>
        [FriendlyRequired("产品部-EMC实验费用录入员")]
        public long? EMCId { get; set; }
        /// <summary>
        /// 财务部-制造费用录入员
        /// </summary>
        [FriendlyRequired("制造费用录入员")]
        public long? ProductCostInputId { get; set; }
        /// <summary>
        /// 生产管理部-物流费用录入员
        /// </summary>
        [FriendlyRequired("物流费用录入员")]
        public long? ProductManageTimeId { get; set; }
        /// <summary>
        /// 项目核价审核员
        /// </summary>
        [FriendlyRequired("项目核价审核员")]
        public long? AuditId { get; set; }
        /// <summary>
        /// TR预计提交时间
        /// </summary>
        [FriendlyRequired("TR预计提交时间")]
        public DateTime? TrSubmitTime { get; set; }
        /// <summary>
        /// 产品部-电子工程师期望完成时间
        /// </summary>
        [FriendlyRequired("电子工程师期望完成时间")]
        public DateTime? ElecEngineerTime { get; set; }
        /// <summary>
        ///产品部-结构工程师期望完成时间
        /// </summary>
        [FriendlyRequired("结构工程师期望完成时间")]
        public DateTime? StructEngineerTime { get; set; }
        /// <summary>
        ///产品部-EMC实验费录入员期望完成时间
        /// </summary>
        [FriendlyRequired("产品部-EMC实验费录入员期望完成时间")]
        public DateTime? EMCTime { get; set; }
        /// <summary>
        ///  品质保证部-可靠性实验费用录入员期望完成时间
        /// </summary>
        [FriendlyRequired("品质保证部-可靠性实验费用录入员期望完成时间")]
        public DateTime? QualityBenchTime { get; set; }
        /// <summary>
        /// 资源管理部-电子资源开发期望完成时间
        /// </summary>
        [FriendlyRequired("电子资源开发期望完成时间")]
        public DateTime? ResourceElecTime { get; set; }
        /// <summary>
        /// 资源管理部-结构资源开发期望完成时间
        /// </summary>
        [FriendlyRequired("结构资源开发期望完成时间")]
        public DateTime? ResourceStructTime { get; set; }
        /// <summary>
        /// 资源部-模具录入员期望完成时间
        /// </summary>
        [FriendlyRequired("模具录入员期望完成时间")]
        public DateTime? MouldWorkHourTime { get; set; }
        /// <summary>
        /// 工程技术部-工序工时录入员期望完成时间
        /// </summary>
        [FriendlyRequired("工序工时录入员期望完成时间")]
        public DateTime? EngineerWorkHourTime { get; set; }
        /// <summary>
        ///  生成管理部-物流成本录入员期望完成时间
        /// </summary>
        [FriendlyRequired("物流成本录入员期望完成时间")]
        public DateTime? ProductManageTime { get; set; }
        /// <summary>
        /// 制造成本录入员期望完成时间
        /// </summary>
        [FriendlyRequired("制造成本录入员期望完成时间")]
        public DateTime? ProductCostInputTime { get; set; }
        /// <summary>
        /// 营销部要求核价完成时间期望完成时间
        /// </summary>        
        public DateTime? Deadline { get; set; }
    }
}
