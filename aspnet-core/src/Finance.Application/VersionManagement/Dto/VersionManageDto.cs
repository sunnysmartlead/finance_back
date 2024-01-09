using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.PriceEval.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.VersionManagement.Dto
{
    /// <summary>
    /// 项目版本名和对应所有版本返回Dto
    /// </summary>
    public class ProjectNameAndVersionDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ProjectNumber { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public List<int> Versions { get; set; }

        /// <summary>
        /// 对应流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
    }

    /// <summary>
    /// 项目版本名和对应项目代码和新建版本号返回Dto
    /// </summary>
    public class NewProjectVersionDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ProjectNumber { get; set; }
        /// <summary>
        /// 新建的流程版本号
        /// </summary>
        public int NewVersion { get; set; }
    }
    /// <summary>
    /// 系统版本管理筛选条件
    /// </summary>
    public class VersionFilterInputDto
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public virtual string Number { get; set; }      
        /// <summary>
        /// 拟稿开始时间
        /// </summary>
        public DateTime? DraftStartTime { get; set; }
        /// <summary>
        /// 拟稿结束时间
        /// </summary>
        public DateTime? DraftEndTime { get; set; }
        /// <summary>
        /// 完成开始时间
        /// </summary>
        public DateTime? FinishedStartTime { get; set; }
        /// <summary>
        /// 完成结束时间
        /// </summary>
        public DateTime? FinishedEndTime { get; set; }
    }

    /// <summary>
    /// 系统版本管理返回基本信息Dto
    /// </summary>
    public class VersionBasicInfoDto
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public virtual string Number { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string ProjectManager { get; set; }
        /// <summary>
        /// 报价形式
        /// </summary>
        public string QuoteTypeName { get; set; }
        /// <summary>
        /// 拟稿时间
        /// </summary>
        public DateTime DraftTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishedTime { get; set; }
    }
    /// <summary>
    /// 审批流程系统版本管理返回Dto
    /// </summary>
    public class VersionManageDto
    {
        /// <summary>
        /// 系统信息
        /// </summary>
        public VersionBasicInfoDto VersionBasicInfo { get; set; }
        /// <summary>
        /// 核价表列表
        /// </summary>
        public List<PriceEvaluationTableDto> PriceEvaluationTableList { get; set; }
        /// <summary>
        /// 报价表
        /// </summary>
        public QuotationListDto QuotationTable { get; set; }
    }

    /// <summary>
    /// 版本管理返回Dto
    /// </summary>
    public class VersionManageListDto
    {
        /// <summary>
        /// 核价表列表
        /// </summary>
        public List<VersionManageDto> VersionManageList { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "执行成功";
    }

    /// <summary>
    /// 表示界面节点状态类型（1：“已完成”，2：“进行中”，3：“未开始”）
    /// </summary>
    public enum PROCESSTYPE : byte
    {
        /// <summary>
        ///节点已完成
        /// </summary>
        [Description("已完成")]
        ProcessFinished = 1,
        /// <summary>
        /// 节点进行中
        /// </summary>
        [Description("进行中")]
        ProcessRunning = 2,
        /// <summary>
        ///节点未开始
        /// </summary>
        [Description("未开始")]
        ProcessNoStart = 3,
    }

    /// <summary>
    /// 系统版本操作记录表返回Dto
    /// </summary>
    public class AuditFlowOperateReocrdDto
    {
        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 节点状态
        /// </summary>
        public PROCESSTYPE ProcessState { get; set; }
        /// <summary>
        /// 操作人（用户表用户名）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 操作角色（角色名）
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 要求完成时间
        /// </summary>
        public DateTime? RequiredTime { get; set; }

        /// <summary>
        /// 重置时间（如果为空，表示是正常流程。如果有值，表示经历过重置，显示的是重置时间）
        /// </summary>
        public DateTime? ResetTime { get; set; }
        /// <summary>
        /// 操作时间记录列表
        /// </summary>
        public List<AuditFlowOperateTime> auditFlowOperateTimes { get; set; }
    }

    /// <summary>
    /// 记录界面节点流转时间列表
    /// </summary>
    public class AuditFlowOperateTime
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }
    }

    /// <summary>
    /// 获取界面期望完成时间Dto
    /// </summary>
    public class GetInferaceRequiredTimeDto
    {
        /// <summary>
        /// 审批流程ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 流程界面标识符
        /// </summary>
        public string ProcessIdentifier { get; set; }
    }
}
