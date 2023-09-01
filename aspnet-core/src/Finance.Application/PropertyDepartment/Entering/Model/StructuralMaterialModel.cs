using Finance.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 结构料单价录入 模型
    /// </summary>
    public class StructuralMaterialModel
    {
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 结构料 Id
        /// </summary>
        public long StructureId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>    
        [FriendlyRequired("币种")]
        public string Currency { get; set; }             
        /// <summary>
        /// 物料可返利金额
        /// </summary>
        public List<KvMode> RebateMoney { get; set; }
        /// <summary>
        /// 项目物料的使用量
        /// </summary>   
        public List<YearOrValueKvMode> MaterialsUseCount { get; set; }
        /// <summary>
        /// 系统单价（原币）
        /// </summary>
        public List<YearOrValueKvMode> SystemiginalCurrency { get; set; }
        /// <summary>
        /// 项目物料的年降率
        /// </summary>
        public List<YearOrValueKvMode> InTheRate { get; set; }      
        /// <summary>
        /// 本位币
        /// </summary>
        public List<YearOrValueKvMode> StandardMoney { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// MOQ
        /// </summary>
        public decimal MOQ { get; set; }   
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 确认人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 是否录入 true/1 录入  false/0 未录入
        /// </summary>
        public bool IsEntering { get; set; }
        /// <summary>
        /// 物料管制状态
        /// </summary> 
        [FriendlyRequired("物料管制状态")]
        public virtual string MaterialControlStatus { get; set; }
    }
}
