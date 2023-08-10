using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    ///  Nre核价 项目管理部  模型
    /// </summary>
    public class ProjectManagementModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }


        /// <summary>
        /// 手板件费用
        /// </summary>
        public List<HandPieceCostModel> HandPieceCost { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public List<RestsCostModel> RestsCost { get; set; }
        /// <summary>
        /// 差旅费
        /// </summary>
        public List<TravelExpenseModel> TravelExpense { get; set; }
        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }
}
