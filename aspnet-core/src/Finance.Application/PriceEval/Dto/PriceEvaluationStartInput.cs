using Finance.Dto;
using Finance.ProjectManagement.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 返回营销部录入信息
    /// </summary>
    public class PriceEvaluationStartInputResult : PriceEvaluationStartInput
    {
       

        /// <summary>
        /// 上传的文件信息（只有Id和FileName有值）
        /// </summary>
        public virtual List<FileUploadOutputDto> Files { get; set; }
    }
    /// <summary>
    /// PriceEvaluationStart 方法的输入参数
    /// </summary>
    public class PriceEvaluationStartInput : ToExamineDto
    {
        /// <summary>
        /// 如果为false，则为保存；为true，提交。 
        /// </summary>
        public virtual bool IsSubmit { get; set; }

        ///// <summary>
        ///// 审批流程主表Id（如果是新建流程，填空。非新建，添流程Id）
        ///// </summary>
        //public virtual long? AuditFlowId { get; set; }

        ///// <summary>
        ///// 核价原因
        ///// </summary>
        //public virtual string EvalReason { get; set; }

        ///// <summary>
        ///// 审批评论
        ///// </summary>
        //public virtual string Comment { get; set; }

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
        /// 核报价流程版本
        /// </summary>
        public virtual int QuoteVersion { get; set; }

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
        /// 客户类别（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【CustomerNature】）
        /// </summary>
        [Required]
        public virtual string CustomerNature { get; set; }




        /// <summary>
        /// 终端名称
        /// </summary>
        public virtual string TerminalName { get; set; }

        /// <summary>
        /// 终端类别（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TerminalNature】）
        /// </summary>
        public virtual string TerminalNature { get; set; }

        ///// <summary>
        ///// 报价形式（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【QuotationType】）
        ///// </summary>
        //[Required]
        //public virtual string QuotationType { get; set; }

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
        /// 样品
        /// </summary>
        public virtual List<CreateSampleDto> Sample { set; get; }

        /// <summary>
        /// 走量系数K
        /// </summary>
        public virtual decimal KValue { get; set; }

        /// <summary>
        /// 终端走量（PCS）
        /// </summary>
        [Required]
        public virtual List<CreatePcsDto> Pcs { set; get; }

        /// <summary>
        /// 车型模组数量
        /// </summary>
        [Required]
        public virtual List<CreateCarModelCountDto> CarModelCount { set; get; }

        /// <summary>
        /// 模组数量合计
        /// </summary>
        [Required]
        public virtual List<CreateModelCountDto> ModelCount { set; get; }



        /// <summary>
        /// 是否包含梯度核价
        /// </summary>
        public virtual bool? IsHasGradient { get; set; }

        /// <summary>
        /// 梯度
        /// </summary>
        public virtual List<GradientInput> Gradient { set; get; }


        /// <summary>
        /// 梯度模组
        /// </summary>
        public virtual List<GradientModelInput> GradientModel { set; get; }


        /// <summary>
        /// 要求
        /// </summary>
        [Required]
        public virtual List<CreateRequirementDto> Requirement { set; get; }

        /// <summary>
        /// 是否有NRE费用分摊至模组
        /// </summary>
        public virtual bool? IsHasNre { get; set; }

        /// <summary>
        /// 分摊数量
        /// </summary>
        public virtual List<ShareCountInput> ShareCount { set; get; }

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
        /// 落地工厂（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【LandingFactory】）
        /// </summary>
        //[Required]
        public virtual string LandingFactory { set; get; }

        /// <summary>
        /// 产品信息（下表【客户指定/供应详情】，根据此表内容生成，只作展示用，不填写）
        /// </summary>
        public virtual List<CreateColumnFormatProductInformationDto> ProductInformation { set; get; }


        /// <summary>
        /// 客户目标价
        /// </summary>
        public virtual List<CreateCustomerTargetPriceDto> CustomerTargetPrice { set; get; }

        ///// <summary>
        ///// 客户指定/供应详情（【产品信息】和【客户指定/供应详情】是同一张表的不同展示形式）
        ///// </summary>
        //public virtual List<CreateProductInformationDto> AppointDetail { set; get; }

        /// <summary>
        /// 贸易方式（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TradeMethod】）
        /// </summary>
        [Required]
        public virtual string TradeMode { get; set; }

        /// <summary>
        /// 销售类型（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【SalesType】）
        /// </summary>
        [Required]
        public virtual string SalesType { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [Required]
        public virtual string PaymentMethod { get; set; }


        ///// 报价币种（汇率录入表主键，调用【UnitPriceLibrary/GetExchangeRate】获取货币）
        ///// </summary>
        //[Required]
        //public virtual long Currency { get; set; }

        ///// <summary>
        ///// 客户目标价
        ///// </summary>
        //[Required]
        //public virtual decimal CustomerTargetPrice { get; set; }

        ///// <summary>
        ///// 汇率（报价币种选择的货币，得到的汇率数据，填入此字段中）
        ///// </summary>
        //[Required]
        //public virtual decimal ExchangeRate { get; set; }

        /// <summary>
        /// 客户特殊性要求
        /// </summary>
        public virtual string CustomerSpecialRequest { get; set; }

        /// <summary>
        /// 运输方式（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【ShippingType】）
        /// </summary>
        [Required]
        public virtual string ShippingType { get; set; }

        /// <summary>
        /// 包装方式（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【PackagingType】）
        /// </summary>
        [Required]
        public virtual string PackagingType { get; set; }

        /// <summary>
        /// 交货地点
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public virtual string PlaceOfDelivery { get; set; }


        /// <summary>
        /// 国家（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【Country】）
        /// </summary>
        [Required]
        public virtual string Country { get; set; }

        /// <summary>
        /// 国家类型
        /// </summary>
        //[Required]
        public virtual string CountryType { get; set;}

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
        /// SOR附件上传
        /// </summary>
        [Required]
        public virtual List<long> SorFile { get; set; }

        /// <summary>
        /// 核价原因
        /// </summary>
        public virtual string Reason { get; set; }
    }
}
