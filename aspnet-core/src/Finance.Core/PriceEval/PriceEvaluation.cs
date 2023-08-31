using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价表 此表是核价流程的主表，其他的核价信息，作为附表，引用此表的Id为外键。
    /// </summary>
    [Table("Pe_PriceEvaluation")]
    public class PriceEvaluation : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// 拟稿人
        /// </summary>
        [Required]
        public virtual string Drafter { get; set; }

        /// <summary>
        /// 拟稿人工号
        /// </summary>
        [Required]
        public virtual long DrafterNumber { get; set; }

        /// <summary>
        /// 拟稿部门
        /// </summary>
        [Required]
        public virtual string DraftingDepartment { get; set; }

        /// <summary>
        /// 拟稿公司
        /// </summary>
        [Required]
        public virtual string DraftingCompany { get; set; }

        /// <summary>
        /// 拟稿部门Id
        /// </summary>
        [Required]
        public virtual long DraftingDepartmentId { get; set; }

        /// <summary>
        /// 拟稿公司Id
        /// </summary>
        [Required]
        public virtual long DraftingCompanyId { get; set; }

        /// <summary>
        /// 拟稿日期
        /// </summary>
        [Required]
        public virtual DateTime DraftDate { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        [Required]
        public virtual string Number { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        //[Required]
        public virtual string ProjectName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        //[Required]
        public virtual string ProjectCode { get; set; }

        /// <summary>
        /// 客户代码
        /// </summary>
        public virtual string CustomerCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        //[Required]
        public virtual string CustomerName { get; set; }

        /// <summary>
        /// 客户类别（字典明细表主键）  客户类别（二开）
        /// </summary>
        [Required]
        public virtual string CustomerNature { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        public virtual string TerminalName { get; set; }

        /// <summary>
        /// 终端类别（字典明细表主键）
        /// </summary>
        public virtual string TerminalNature { get; set; }

        /// <summary>
        /// 报价形式（字典明细表主键）（二开应该注释掉，被防报错保留）
        /// </summary>
        //[Required]
        public virtual string QuotationType { get; set; }

        ///// <summary>
        ///// 样品报价类型（字典明细表主键）（二开新增属性）
        ///// </summary>
        //[Required]
        //public virtual string SampleQuotationType { get; set; }

        /// <summary>
        /// Sop时间（年份）
        /// </summary>
        [Required]
        public virtual int SopTime { get; set; }

        /// <summary>
        /// 项目周期（年）
        /// </summary>
        [Required]
        public virtual int ProjectCycle { get; set; }

        /// <summary>
        /// 价格有效期（字典明细表主键）（二开新增属性）
        /// </summary>
        [Required]
        public virtual string UpdateFrequency { get; set; }

        /// <summary>
        /// 核价类型（字典明细表主键）（二开新增属性）
        /// </summary>
        //[Required]
        public virtual string PriceEvalType { get; set; }

        /// <summary>
        /// 是否包含样品核价（二开新增属性）
        /// </summary>
        public virtual bool? IsHasSample { get; set; }

        /// <summary>
        /// 走量系数K（项目走量的一部分）
        /// </summary>
        public virtual decimal KValue { get; set; }


        /// <summary>
        /// 是否包含梯度核价
        /// </summary>
        public virtual bool? IsHasGradient { get; set; }

        /// <summary>
        /// 是否有NRE费用分摊至模组
        /// </summary>
        public virtual bool? IsHasNre { get; set; }


        /// <summary>
        /// 模具费分摊
        /// </summary>
        //[Required]
        public virtual bool AllocationOfMouldCost { set; get; }

        /// <summary>
        /// 治具费分摊
        /// </summary>
        //[Required]
        public virtual bool AllocationOfFixtureCost { set; get; }

        /// <summary>
        /// 工装费分摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool FrockCost { set; get; }

        /// <summary>
        /// 专用设备费分摊
        /// </summary>
        //[Required]
        public virtual bool AllocationOfEquipmentCost { set; get; }

        ///// <summary>
        ///// 信赖性费用分摊
        ///// </summary>
        //[Required]
        //public virtual bool ReliabilityCost { set; get; }

        /// <summary>
        /// 检具费用分摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool FixtureCost { set; get; }

        /// <summary>
        /// 实验费分摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool ExperimentCost { set; get; }

        /// <summary>
        /// 测试软件费分摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool TestCost { set; get; }

        /// <summary>
        /// 差旅费摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool TravelCost { set; get; }

        /// <summary>
        /// 其他费用分摊（二开新增属性）
        /// </summary>
        //[Required]
        public virtual bool OtherCost { set; get; }

        ///// <summary>
        ///// 开发费分摊
        ///// </summary>
        //[Required]
        //public virtual bool DevelopmentCost { set; get; }

        /// <summary>
        /// 落地工厂（字典明细表主键）
        /// </summary>
        //[Required]
        public virtual string LandingFactory { set; get; }

        /// <summary>
        /// 贸易方式
        /// </summary>
        [Required]
        public virtual string TradeMode { get; set; }

        /// <summary>
        /// 销售类型（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string SalesType { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [Required]
        public virtual string PaymentMethod { get; set; }

        

        /// <summary>
        /// 客户特殊性要求
        /// </summary>
        public virtual string CustomerSpecialRequest { get; set; }

        /// <summary>
        /// 运输方式（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string ShippingType { get; set; }

        /// <summary>
        /// 包装方式（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string PackagingType { get; set; }

        /// <summary>
        /// 交货地点
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public virtual string PlaceOfDelivery { get; set; }

        /// <summary>
        /// 贸易合规国家库ID
        /// </summary>
        public virtual long CountryLibraryId { get; set; }

        /// <summary>
        /// 国家（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Country { get; set; }

        /// <summary>
        /// 国家类型
        /// </summary>
        [Required]
        public virtual string CountryType { get; set; }

        /// <summary>
        /// 要求核价完成时间
        /// </summary>
        [Required]
        public virtual DateTime Deadline { get; set; }

        /// <summary>
        /// 项目经理（User表的Id）
        /// </summary>
        [Required]
        public virtual long ProjectManager { get; set; }

        /// <summary>
        /// 文件Id串
        /// </summary>
        [Required]
        public virtual string SorFile { get; set; }

        /// <summary>
        /// TR方案所在文件表Id
        /// </summary>
        public virtual long? TrProgramme { get; set; }


        /// <summary>
        /// 核价原因
        /// </summary>
        public virtual string Reason { get; set; }


        /// <summary>
        /// 报价币种（汇率录入表（ExchangeRate）主键）（二开应该注释掉）
        /// </summary>
        //[Required]
        public virtual long Currency { get; set; }

        /// <summary>
        /// 汇率（二开应该注释掉）
        /// </summary>
        //[Required]
        public virtual decimal ExchangeRate { get; set; }

        /// <summary>
        /// BOM是否客供Json
        /// </summary>
        public virtual string BomIsCustomerSupplyJson { get; set;}
    }
}
