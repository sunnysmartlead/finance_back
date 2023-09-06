using Finance.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 结构BOM表单 模型
    /// </summary>
    public class ConstructionModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 结构料BOM Id
        /// </summary>
        public long StructureId { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 物料编号（SAP）
        /// </summary>
        public string SapItemNum { get; set; }
        /// <summary>
        /// 图号名称
        /// </summary>
        public string DrawingNumName { get; set; }
        /// <summary>
        /// 外形尺寸mm
        /// </summary>
        public string OverallDimensionSize { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 重量g
        /// </summary>
        public string WeightNumber { get; set; }
        /// <summary>
        /// 成型工艺
        /// </summary> 
        public string MoldingProcess { get; set; }
        /// <summary>
        /// 二次加工方法
        /// </summary> 
        public string SecondaryProcessingMethod { get; set; }
        /// <summary>
        /// 表面处理
        /// </summary> 
        public string SurfaceTreatmentMethod { get; set; }
        /// <summary>
        /// 关键尺寸精度及重要要求
        /// </summary> 
        public string DimensionalAccuracyRemark { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        [FriendlyRequired("币种", skip: true)]
        public string Currency { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        public double AssemblyQuantity { get; set; }
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
        /// 物料可返利金额
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
        public virtual string MaterialControlStatus { get; set; }
    }
}
