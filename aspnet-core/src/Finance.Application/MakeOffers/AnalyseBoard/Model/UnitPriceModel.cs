using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价分析看板返回实体类=>单价表 模型
    /// </summary>
    public class UnitPriceModel: GrossMarginListModel
    {
        /// <summary>
        /// 模组id 关联ModelCount
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
    }  
}
