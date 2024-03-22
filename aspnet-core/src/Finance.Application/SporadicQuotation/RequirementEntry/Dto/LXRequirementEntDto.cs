using Finance.Dto;
using Finance.Ext;
using Finance.LXRequirementEntry;
using Finance.ProjectManagement.Dto;
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
    public class LXRequirementEntDto : ToExamineDtoLX
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
        [FriendlyRequired("标题")]
        public virtual string Title { get; set; }

        /// <summary>
        /// 拟稿人
        /// </summary>      
        [FriendlyRequired("拟稿人")]
        public virtual string Drafter { get; set; }

        /// <summary>
        /// 拟稿人工号
        /// </summary>       
        [FriendlyRequired("拟稿人工号")]
        public virtual long DrafterNumber { get; set; }

        /// <summary>
        /// 拟稿部门
        /// </summary>    
        [FriendlyRequired("拟稿部门")]
        public virtual string DraftingDepartment { get; set; }

        /// <summary>
        /// 拟稿公司
        /// </summary>       
        [FriendlyRequired("拟稿公司")]
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
        [FriendlyRequired("单据编号")]
        public virtual string Number { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>   
        [FriendlyRequired("项目名称")]
        public virtual string ProjectName { get; set; }

        /// <summary>
        /// 直接客户名称
        /// </summary>
        [FriendlyRequired("直接客户名称")]
        public virtual string DirectCustomerName { get; set; }

        /// <summary>
        /// 客户性质
        /// </summary>
        [FriendlyRequired("客户性质")]
        public virtual string CustomerNature { get; set; }

        /// <summary>
        /// 终端客户名称
        /// </summary>
        [FriendlyRequired("终端客户名称")]
        public virtual string EndCustomerName { get; set; }

        /// <summary>
        /// 终端客户性质
        /// </summary>
        [FriendlyRequired("终端客户性质")]
        public virtual string EndCustomerNature { get; set; }

        /// <summary>
        /// 零部件类型(字典明细表主键)
        /// </summary>
        [FriendlyRequired("零部件类型")]
        public virtual string ComponentType { get; set; }

        /// <summary>
        /// 零部件类型字典包DisplayName
        /// </summary>         
        public virtual string ComponentTypeDisplayName { get; set; }

        /// <summary>
        /// 附件Id
        /// </summary>
        [FriendlyRequired("附件")]
        public virtual long EnclosureId { get; set; }

        /// <summary>
        /// 零星报价列表实体类
        /// </summary>
        [FriendlyRequired("零星报价列表实体类")]
        public virtual List<LXDataListDto> LXDataListDtos { get; set; }

        /// <summary>
        /// 是否提交
        /// </summary>
        public virtual bool IsSubmit {  get; set; }

        /// <summary>
        /// 上传的文件信息（只有Id和FileName有值）
        /// </summary>
        public virtual FileUploadOutputDto File { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>         
        public override string Opinion { get; set; }
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
        [FriendlyRequired("零星报价列表实体-列名称")]
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
    /// <summary>
    /// 总经理审批Dto
    /// </summary>
    public class ManagerApprovalDto
    {
        /// <summary>
        /// 直接客户名称
        /// </summary>
        [FriendlyRequired("直接客户名称")]
        public virtual string DirectCustomerName { get; set; }

        /// <summary>
        /// 客户性质
        /// </summary>
        [FriendlyRequired("客户性质")]
        public virtual string CustomerNature { get; set; }

        /// <summary>
        /// 终端客户名称
        /// </summary>
        [FriendlyRequired("终端客户名称")]
        public virtual string EndCustomerName { get; set; }

        /// <summary>
        /// 终端客户性质
        /// </summary>
        [FriendlyRequired("终端客户性质")]
        public virtual string EndCustomerNature { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>   
        [FriendlyRequired("项目名称")]
        public virtual string ProjectName { get; set; }
        /// <summary>
        /// 零部件类型(字典明细表主键)
        /// </summary>
        [FriendlyRequired("零部件类型")]
        public virtual string ComponentType { get; set; }
        /// <summary>
        /// 零部件类型字典包DisplayName
        /// </summary>         
        public virtual string ComponentTypeDisplayName { get; set; }
        /// <summary>
        /// 零星报价列表实体类
        /// </summary>
        [FriendlyRequired("零星报价列表实体类")]
        public virtual List<LXDataListDto> LXDataListDtos { get; set; }
       
    }
}
