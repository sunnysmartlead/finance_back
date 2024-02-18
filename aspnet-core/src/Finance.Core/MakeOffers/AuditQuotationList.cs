using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价审核表
    /// </summary>
    public class AuditQuotationList : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 审核表内容 存json  实体类为 AuditQuotationListDto
        /// </summary>
        [Column(TypeName = "CLOB")]
        public string AuditQuotationListJson { get; set; }
     
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
        
        /// <summary>
        /// 序号
        /// </summary>
        public int ntype { get; set; }
        /// <summary>
        /// 1 类别  2报价审批表，3上轮单价
        /// </summary>
        public int nsource { get; set; }

    }

    /// <summary>
    /// 报价审核表保存数据(后加)
    /// </summary>
    public class AuditQuotationListSave : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 审核表内容 存json  实体类为 AuditQuotationListDto
        /// </summary>
        [Column(TypeName = "CLOB")]
        public string AuditQuotationListJson { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int ntype { get; set; }
        /// <summary>
        /// 0:报价审批表保存
        /// </summary>
        public int nsource { get; set; }
        /// <summary>
        /// 开发计划
        /// </summary>
        public string DevelopmentPlan { get; set; }
        /// <summary>
        /// 报价币种
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// SOP时间
        /// </summary>
        public int sopTime { get; set; }

        /// <summary>
        /// 项目生命周期
        /// </summary>
        public int projectCycle { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string paymentMethod { get; set; }
    }
}

