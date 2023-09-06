using Finance.Ext;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary> 
    /// 电子BOM表单 交互类
    /// </summary>
    public class ElectronicDto
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.AuditFlowIdVerification, true)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification, true)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 电子料BOM Id
        /// </summary>
        public long ElectronicId { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string SapItemNum { get; set; }
        /// <summary>
        /// 材料名称
        /// </summary>
        public string SapItemName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// 项目物料的使用量
        /// </summary>
        public List<YearOrValueKvMode> MaterialsUseCount { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        [FriendlyRequired("币种",skip: true)]
        public string Currency { get; set; }
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
        /// 物料返利金额
        /// </summary>
        public List<KvMode> RebateMoney { get; set; }
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
        /// 确认人姓名
        /// </summary>
        public string PeopleName { get; set; }
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
        [FriendlyRequired("物料管制状态不能为空",skip: true)]
        public virtual string MaterialControlStatus { get; set; }
    }
    public class IsALLElectronicDto
    {
        /// <summary>
        /// 是否全部提交完成
        /// </summary>
        public bool isAll { get; set; }

        public List<ElectronicDto> ElectronicDtos { get; set; }
    }
}
