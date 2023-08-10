using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 类里面有  每一个产品  单车产数量  和单价
    /// </summary>
    public class UnitPriceCountModel
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品小类（字典明细表主键）
        /// </summary>         
        public string ProductType { get; set; }
        /// <summary>
        /// 单价(也就是单位成本)(全生命周期)
        /// </summary>
        public decimal Unit { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 单车产品数量
        /// </summary>
        public int ProductNumber { get; set; }

    }
}
