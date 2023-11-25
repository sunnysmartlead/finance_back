using System;
using System.Collections.Generic;
using Finance.Dto;
using Finance.Ext;
using Finance.MakeOffers.AnalyseBoard.Model;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 对外报价单
/// </summary>
public class ExternalQuotationDto : ToExamineDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 审批流程表ID
    /// </summary>
    [FriendlyRequired("审批流程表ID", SpecialVerification.AuditFlowIdVerification)]
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 方案表ID
    /// </summary>      
    [FriendlyRequired("方案表ID")]
    public long SolutionId { get; set; }
    /// <summary>
    /// 客户名称
    /// </summary>
    //[Required]
    [FriendlyRequired("客户名称")]
    public virtual string CustomerName { get; set; }
    /// <summary>
    /// 客户地址
    /// </summary>
    //[Required]
    [FriendlyRequired("客户地址")]
    public virtual string CustomerAdd { get; set; }
    /// <summary>
    /// 客户联系人
    /// </summary>
    //[Required]
    [FriendlyRequired("客户联系人")]
    public virtual string CustomerLink { get; set; }
    /// <summary>
    /// 客户联系人号码
    /// </summary>
    //[Required]
    [FriendlyRequired("客户联系人号码")]
    public virtual string CustomerNum { get; set; }
    /// <summary>
    /// 报价单位
    /// </summary>
    //[Required]
    [FriendlyRequired("报价单位")]
    public virtual string QuotationName { get; set; }
    /// <summary>
    /// 报价地址
    /// </summary>
    //[Required]
    [FriendlyRequired("报价地址")]
    public virtual string QuotationAdd { get; set; }
    /// <summary>
    /// 报价联系人
    /// </summary>
    //[Required]
    [FriendlyRequired("报价联系人")]
    public virtual string QuotationLink { get; set; }
    /// <summary>
    /// 报价联系人号码
    /// </summary>
    //[Required]
    [FriendlyRequired("报价联系人号码")]
    public virtual string QuotationNum { get; set; }

    /// <summary>
    /// 项目周期（年）
    /// </summary>
    [FriendlyRequired("项目周期（年）")]
    public virtual int ProjectCycle { get; set; }
    /// <summary>
    /// 项目名称
    /// </summary>
    //[Required]
    [FriendlyRequired("项目名称")]
    public virtual string ProjectName { get; set; }
    /// <summary>
    /// Sop时间（年份）
    /// </summary>
    [FriendlyRequired("Sop时间（年份）")]
    public virtual int SopTime { get; set; }
    /// <summary>
    /// 报价币种
    /// </summary>
    //[Required]
    [FriendlyRequired("报价币种")]
    public virtual string Currency { get; set; }
    /// <summary>
    /// 制作
    /// </summary>
    [FriendlyRequired("制作")]
    public virtual string Make { get; set; }
    /// <summary>
    /// 审核
    /// </summary>
    [FriendlyRequired("审核")]
    public virtual string ToExamine { get; set; }
    /// <summary>
    /// 记录编号
    /// </summary>
    [FriendlyRequired("记录编号")]
    public virtual string RecordNo { get; set; }
    /// <summary>
    /// 户名
    /// </summary>
    [FriendlyRequired("户名")]
    public virtual string AccountName { get; set; }
    /// <summary>
    /// 税号
    /// </summary>
    [FriendlyRequired("税号")]
    public virtual string DutyParagraph { get; set; }
    /// <summary>
    /// 开户行
    /// </summary>
    [FriendlyRequired("开户行")]
    public virtual string BankOfDeposit { get; set; }
    /// <summary>
    /// 账号
    /// </summary>
    [FriendlyRequired("账号")]
    public virtual string AccountNumber { get; set; }
    /// <summary>
    /// 地址
    /// </summary>
    [FriendlyRequired("地址")]
    public virtual string Address { get; set; }
    /// <summary>
    /// 报价次数
    /// </summary>
    public virtual long NumberOfQuotations { get; set; }
    /// <summary>
    /// 是否提交
    /// </summary>
    public virtual bool IsSubmit { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string Remark { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime CreationTime { get; set; }
    /// <summary>
    /// 产品报价清单
    /// </summary>
    [FriendlyRequired("产品报价清单")]
    public List<ProductQuotationListDto> ProductQuotationListDtos { get; set; }
    /// <summary>
    /// NRE报价清单
    /// </summary>
    [FriendlyRequired("NRE报价清单")]
    public List<NreQuotationListDto> NreQuotationListDtos { get; set; }
}
/// <summary>
/// 产品报价清单交互类
/// </summary>

public class ProductQuotationListDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 序号
    /// </summary>
    public int SerialNumber { get; set; }
    /// <summary>
    /// 产品名称
    /// </summary>
    [FriendlyRequired("产品报价清单-产品名称")]
    public virtual string ProductName { get; set; }
    /// <summary>
    /// 年份
    /// </summary>
    [FriendlyRequired("产品报价清单-年份")]
    public virtual string Year { get; set; }
    /// <summary>
    /// 走量
    /// </summary>       
    [FriendlyRequired("产品报价清单-走量")]
    public virtual decimal TravelVolume { get; set; }
    /// <summary>
    /// 单价
    /// </summary>
    [FriendlyRequired("产品报价清单-单价")]
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string Remark { get; set; }

}
/// <summary>
/// NRE报价清单交互类
/// </summary>

public class NreQuotationListDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 序号
    /// </summary>
    public int SerialNumber { get; set; }
    /// <summary>
    /// 产品名称
    /// </summary>       
    [FriendlyRequired("NRE报价清单-产品名称")]
    public virtual string ProductName { get; set; }
    /// <summary>
    /// 走量
    /// </summary>     
    [FriendlyRequired("NRE报价清单-走量")]
    public virtual decimal TravelVolume { get; set; }
    /// <summary>
    /// 手板件费
    /// </summary>
    [FriendlyRequired("NRE报价清单-手板件费")]
    public virtual decimal HandmadePartsFee { get; set; }
    /// <summary>
    /// 模具费
    /// </summary>
    [FriendlyRequired("NRE-报价清单模具费")]
    public virtual decimal MyPropMoldCosterty { get; set; }
    /// <summary>
    /// 工装治具费
    /// </summary>
    [FriendlyRequired("NRE报价清单-工装治具费")]
    public virtual decimal CostOfToolingAndFixtures { get; set; }
    /// <summary>
    /// 实验费
    /// </summary>
    [FriendlyRequired("NRE报价清单-实验费")]
    public virtual decimal ExperimentalFees { get; set; }
    /// <summary>
    /// 研发费	
    /// </summary>
    [FriendlyRequired("NRE报价清单-研发费")]
    public virtual decimal RDExpenses { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string Remark { get; set; }

}