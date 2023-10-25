using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 对外报价单
    /// </summary>
    public class ExternalQuotation : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
     
        public long AuditFlowId { get; set; }      
        /// <summary>
        /// 方案表ID
        /// </summary>      
        public long SolutionId { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public virtual string CustomerName { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        public virtual string CustomerAdd { get; set; }
        /// <summary>
        /// 客户联系人
        /// </summary>
        public virtual string CustomerLink { get; set; }
        /// <summary>
        /// 客户联系人号码
        /// </summary>
        public virtual string CustomerNum { get; set; }
        /// <summary>
        /// 报价单位
        /// </summary>
        public virtual string QuotationName { get; set; }
        /// <summary>
        /// 报价地址
        /// </summary>
        public virtual string QuotationAdd { get; set; }
        /// <summary>
        /// 报价联系人
        /// </summary>
        public virtual string QuotationLink { get; set; }
        /// <summary>
        /// 报价联系人号码
        /// </summary>
        public virtual string QuotationNum { get; set; }
        /// <summary>
        /// 项目周期（年）
        /// </summary>       
        public virtual long ProjectCycle { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public virtual string ProjectName { get; set; }
        /// <summary>
        /// Sop时间（年份）
        /// </summary>       
        public virtual int SopTime { get; set; }
        /// <summary>
        /// 报价币种
        /// </summary>
        public virtual string Currency { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// 制作
        /// </summary>
        public virtual string Make { get; set; }
        /// <summary>
        /// 审核
        /// </summary>
        public virtual string ToExamine { get; set; }
        /// <summary>
        /// 记录编号
        /// </summary>
        public virtual string RecordNo { get; set; }
        /// <summary>
        /// 户名
        /// </summary>
        public virtual string AccountName { get; set; }
        /// <summary>
        /// 税号
        /// </summary>
        public virtual string DutyParagraph { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public virtual string BankOfDeposit { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public virtual string AccountNumber { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// 报价次数
        /// </summary>
        public virtual long NumberOfQuotations { get; set; }
        /// <summary>
        /// 是否提交
        /// </summary>
        public virtual bool IsSubmit { get; set; }
    }
}
