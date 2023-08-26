using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 对外报价单
/// </summary>
public class ExternalQuotationDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    //[Required]
    public virtual string CustomerName { get; set; }
    /// <summary>
    /// 客户地址
    /// </summary>
    //[Required]
    public virtual string CustomerAdd { get; set; }
    /// <summary>
    /// 客户联系人
    /// </summary>
    //[Required]
    public virtual string CustomerLink { get; set; }
    /// <summary>
    /// 客户联系人号码
    /// </summary>
    //[Required]
    public virtual string CustomerNum { get; set; }
    
    /// <summary>
    /// 报价单位
    /// </summary>
    //[Required]
    public virtual string QuotationName { get; set; }
    /// <summary>
    /// 报价地址
    /// </summary>
    //[Required]
    public virtual string QuotationAdd { get; set; }
    /// <summary>
    /// 报价联系人
    /// </summary>
    //[Required]
    public virtual string QuotationLink { get; set; }
    /// <summary>
    /// 报价联系人号码
    /// </summary>
    //[Required]
    public virtual string QuotationNum { get; set; }
    
    /// <summary>
    /// 项目周期（年）
    /// </summary>
    public virtual int ProjectCycle { get; set; }
    /// <summary>
    /// 项目名称
    /// </summary>
    //[Required]
    public virtual string ProjectName { get; set; }
    /// <summary>
    /// Sop时间（年份）
    /// </summary>
    public virtual int SopTime { get; set; }
    /// <summary>
    /// 报价币种（汇率录入表（ExchangeRate）主键）（二开应该注释掉）
    /// </summary>
    //[Required]
    public virtual long Currency { get; set; }


    public List<SopOrValueMode> sopls;
    public List<ExternalQuotationMxDto> mxs;
}

public class ExternalQuotationMxDto
{
    /// <summary>
    /// 产品名称
    /// </summary>
    public virtual string productName { get; set; }
    /// <summary>
    /// 年份
    /// </summary>
    public virtual string year { get; set; }
    /// <summary>
    /// 数量
    /// </summary>
    public virtual decimal amout { get; set; }
    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string remark { get; set; }
    
}