using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 核心部件 模型
    /// </summary>
    public class PartsModel
    {
        /// <summary>
        /// 部件名称
        /// </summary>
        public string PartsName { get; set; }
        /// <summary>
        /// 核心部件
        /// </summary>
        public string Parts { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
