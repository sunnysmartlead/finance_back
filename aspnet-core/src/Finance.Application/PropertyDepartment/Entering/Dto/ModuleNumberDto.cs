using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary>
    /// 查询项目走量 交互类
    /// </summary>
    public class ModuleNumberDto
    {
        /// <summary>
        /// 模组数量 id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }

        /// <summary>
        /// 产品（字典明细表主键）
        /// </summary>      
        public virtual string ProductName { get; set; }

        /// <summary>
        /// 产品小类（字典明细表主键）
        /// </summary>     
        public virtual string ProductTypeName { get; set; }
        /// <summary>
        /// 市场份额（%）
        /// </summary> 
        public virtual decimal MarketShare { get; set; }
        /// <summary>
        /// 模组搭载率
        /// </summary>   
        public virtual decimal ModuleCarryingRate { get; set; }
        /// <summary>
        /// 单车产品数量
        /// </summary>      
        public virtual int SingleCarProductsQuantity { get; set; }
        /// <summary>
        /// 模组数量年份
        /// </summary>
        public virtual List<YearOrValueMode> ModelCountYear { get; set; }
        /// <summary>
        /// 模组总量
        /// </summary>
        public virtual int ModelTotal { get; set; }
    }
}
