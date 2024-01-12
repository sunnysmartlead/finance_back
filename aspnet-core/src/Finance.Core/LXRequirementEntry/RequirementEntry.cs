using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;

namespace Finance.LXRequirementEntry
{
    /// <summary>
    /// 零星报价需求录入
    /// </summary>
    public class RequirementEnt : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>       
        public virtual string Title { get; set; }

        /// <summary>
        /// 拟稿人
        /// </summary>      
        public virtual string Drafter { get; set; }

        /// <summary>
        /// 拟稿人工号
        /// </summary>       
        public virtual long DrafterNumber { get; set; }

        /// <summary>
        /// 拟稿部门
        /// </summary>     
        public virtual string DraftingDepartment { get; set; }

        /// <summary>
        /// 拟稿公司
        /// </summary>       
        public virtual string DraftingCompany { get; set; }

        /// <summary>
        /// 拟稿部门Id
        /// </summary>       
        public virtual long DraftingDepartmentId { get; set; }

        /// <summary>
        /// 拟稿公司Id
        /// </summary>       
        public virtual long DraftingCompanyId { get; set; }

        /// <summary>
        /// 拟稿日期        
        /// </summary>       
        public virtual DateTime DraftDate { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>       
        public virtual string Number { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>        
        public virtual string ProjectName { get; set; }

        /// <summary>
        /// 直接客户名称
        /// </summary>
        public virtual string DirectCustomerName { get; set; }

        /// <summary>
        /// 客户性质
        /// </summary>
        public virtual string CustomerNature { get; set; }

        /// <summary>
        /// 终端客户名称
        /// </summary>
        public virtual string EndCustomerName { get; set; }

        /// <summary>
        /// 终端客户性质
        /// </summary>
        public virtual string EndCustomerNature { get; set; }

        /// <summary>
        /// 零部件类型(字典明细表主键)
        /// </summary>
        public virtual string ComponentType { get; set; }

        /// <summary>
        /// 附件Id
        /// </summary>
        public virtual long EnclosureId { get; set; }
    }
    /// <summary>
    /// 数据列表
    /// </summary>
    public class DataList : FullAuditedEntity<long>
    {
        /// <summary>
        /// 零星报价需求录入主表Id
        /// </summary>
        public virtual long RequirementEntryId { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public virtual ListName ListName { get; set; }

        /// <summary>
        /// 数据 用逗号分割
        /// </summary>
        public virtual string Data { get; set; } 
    }

    public enum ListName
    {
        [Description("零件名称")]
        零件名称,
        [Description("数量")]
        数量,
        [Description("单价")]
        单价,
        [Description("单位成本")]
        单位成本,
        [Description("销售收入")]
        销售收入,
        [Description("销售成本")]
        销售成本,
        [Description("销售毛利")]
        销售毛利,
        [Description("毛利率")]
        毛利率,
        [Description("备注")]
        备注
    }
}
