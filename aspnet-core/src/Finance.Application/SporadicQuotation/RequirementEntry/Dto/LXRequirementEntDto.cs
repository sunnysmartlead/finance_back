using Finance.LXRequirementEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.SporadicQuotation.RequirementEntry.Dto
{
    /// <summary>
    /// 零星报价需求录入交互类
    /// </summary>
    public class LXRequirementEntDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual long Id { get; set; }

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

        /// <summary>
        /// 零星报价列表实体类
        /// </summary>
        public virtual List<LXDataListDto> LXDataListDtos { get; set; }

        /// <summary>
        /// 是否提交
        /// </summary>
        public virtual bool IsSubmit {  get; set; }
    }
    /// <summary>
    /// 零星报价列表交互类 保存\提交\查询
    /// </summary>
    public class LXDataListDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// 零星报价需求录入主表Id
        /// </summary>
        public virtual long RequirementEntryId { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public virtual ListName ListName { get; set; }

        /// <summary>
        /// 列名称注释
        /// </summary>
        public virtual string ListNameDisplayName { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public virtual List<string> Data { get; set; }
    } 
}
