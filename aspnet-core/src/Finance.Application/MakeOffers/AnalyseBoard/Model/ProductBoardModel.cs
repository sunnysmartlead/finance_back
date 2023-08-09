using Finance.MakeOffers.AnalyseBoard.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 产品看板实体类 模型
    /// </summary>

    public class ProductBoardModel
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 单车产品数量
        /// </summary>
        public long ProductNumber { get; set; }
        private decimal interiorTargetUnitPrice;
        /// <summary>
        /// 目标单价(内部)
        /// </summary>
        public decimal InteriorTargetUnitPrice
        {
            get { return interiorTargetUnitPrice.GetDecimal(2); }   // get 方法
            set { interiorTargetUnitPrice = value; }  // set 方法
        }
        private decimal interiorTargetGrossMargin;
        /// <summary>
        /// 目标毛利率(内部)
        /// </summary>
        public decimal InteriorTargetGrossMargin
        {
            get { return interiorTargetGrossMargin.GetDecimal(2); }   // get 方法
            set { interiorTargetGrossMargin = value; }  // set 方法
        }
        private decimal clientTargetUnitPrice;
        /// <summary>
        /// 目标单价(客户)
        /// </summary>
        public decimal ClientTargetUnitPrice
        {
            get { return clientTargetUnitPrice.GetDecimal(2); }   // get 方法
            set { clientTargetUnitPrice = value; }  // set 方法
        }
        private decimal clientTargetGrossMargin;
        /// <summary>
        /// 目标毛利率(客户)
        /// </summary>
        public decimal ClientTargetGrossMargin
        {
            get { return clientTargetGrossMargin.GetDecimal(2); }   // get 方法
            set { clientTargetGrossMargin = value; }  // set 方法
        }
        private decimal offerUnitPrice;
        /// <summary>
        /// 本次报价-单价
        /// </summary>
        public decimal OfferUnitPrice
        {
            get { return offerUnitPrice.GetDecimal(2); }   // get 方法
            set { offerUnitPrice = value; }  // set 方法
        }
        private decimal offeGrossMargin;
        /// <summary>
        /// 本次报价-毛利率
        /// </summary>
        public decimal OffeGrossMargin
        {
            get { return offeGrossMargin.GetDecimal(2); }   // get 方法
            set { offeGrossMargin = value; }  // set 方法
        }
        /// <summary>
        /// 之前版本的报价
        /// </summary>
        public List<OneGoNOffer> OldOffer { get; set; }
       
    }
    /// <summary>
    /// 第一轮报价到第N轮的报价
    /// </summary>
    public class OneGoNOffer
    {
        private decimal unitPrice;
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice
        {
            get { return unitPrice.GetDecimal(2); }   // get 方法
            set { unitPrice = value; }  // set 方法
        }
        private decimal grossMargin;
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin
        {
            get { return grossMargin.GetDecimal(2); }   // get 方法
            set { grossMargin = value; }  // set 方法
        }
    }
}
