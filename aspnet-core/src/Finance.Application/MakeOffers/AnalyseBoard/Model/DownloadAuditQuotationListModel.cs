using Finance.MakeOffers.AnalyseBoard.DTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价审核表 下载--流 模型
    /// </summary>
    public class DownloadAuditQuotationListModel
    {
        /// <summary>
        /// //日期
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 记录编号
        /// </summary>
        public string RecordNumber { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Versions { get; set; }
        /// <summary>
        /// 直接客户名称
        /// </summary>
        public string DirectCustomerName { get; set; }
        /// <summary>
        /// 终端客户名称
        /// </summary>
        public string TerminalCustomerName { get; set; }
        /// <summary>
        /// 报价形式
        /// </summary>
        public string OfferForm { get; set; }
        /// <summary>
        /// SOP时间
        /// </summary>
        public int SopTime { get; set; }
        /// <summary>
        /// 项目生命周期
        /// </summary>
        public int ProjectCycle { get; set; }
        /// <summary>
        /// 销售类型
        /// </summary>
        public string ForSale { get; set; }
        /// <summary>
        /// 贸易方式
        /// </summary>
        public string modeOfTrade { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// SOP
        /// </summary>
        public List<SopModel> Sop { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 核心部件
        /// </summary>
        public List<PartsModel> Parts { get; set; }
        /// <summary>
        /// NRE
        /// </summary>
        public List<NREModel> NRE { get; set; }
        /// <summary>
        /// 成本单价信息
        /// </summary>
        public List<PricingModel> Cost { get; set; }
        /// <summary>
        /// 报价策略
        /// </summary>
        public List<BiddingStrategy> Strategy { get; set; }
        /// <summary>
        /// 费用表
        /// </summary>
        public List<ExpensesStatementModel> Offer { get; set; }
    }
}
