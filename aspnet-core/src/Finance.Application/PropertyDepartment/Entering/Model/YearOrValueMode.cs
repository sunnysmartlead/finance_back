using Finance.PriceEval;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 键值对 一个年份对应一个值 模型
    /// </summary>
    public class YearOrValueMode
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>     
        public virtual YearType? UpDown { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
    }
    /// <summary>
    /// 加梯度 年份和值的模型
    /// </summary>
    public class YearOrValueKvMode
    {
        /// <summary>
        /// 梯度
        /// </summary>     
        public  decimal Kv { get; set; }
        /// <summary>
        /// 年份键值对模型
        /// </summary>
        public List<YearOrValueMode> YearOrValueModes { get; set; }
    }
    /// <summary>
    /// 物料返利金额模型
    /// </summary>
    public class KvMode
    {
        /// <summary>
        /// 梯度
        /// </summary>     
        public decimal Kv { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
    }  
    /// <summary>
    /// 电子返利金额内部开放接口模型
    /// </summary>
    public class RebateAmountKvModeElectronic
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 电子bom表单id
        /// </summary>
        public long ElectronicId { get; set; }
        /// <summary>
        /// 电子单价Id
        /// </summary>
        public long ElectronicUnitPriceId { get; set; }
        /// <summary>
        /// 物料返利金额模型
        /// </summary>
        public List<KvMode> KvModes { get; set; }
    }
    /// <summary>
    /// 结构返利金额内部开放接口模型
    /// </summary>
    public class RebateAmountKvModeElectronicStructure
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 结构料BOM Id
        /// </summary>
        public long StructureId { get; set; }
        /// <summary>
        /// 结构单价Id
        /// </summary>
        public long StructuralUnitPriceId { get; set; }
        /// <summary>
        /// 物料返利金额模型
        /// </summary>
        public List<KvMode> KvModes { get; set; }
    }
}
