using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 电子料汇总信息
    /// </summary>
    public class ElectronicSum
    {
        public virtual string Name { get; set; }
        public virtual decimal Value { get; set; }
    }

    /// <summary>
    /// 结构料汇总信息
    /// </summary>
    public class StructuralSum
    {
        public virtual string Name { get; set; }
        public virtual decimal Value { get; set; }
    }

    /// <summary>
    /// 胶水等辅材汇总信息
    /// </summary>
    public class GlueMaterialSum
    {
        public virtual string Name { get; set; }
        public virtual decimal Value { get; set; }
    }

    /// <summary>
    /// SMT外协汇总信息
    /// </summary>
    public class SMTOutSourceSum
    {
        public virtual string Name { get; set; }
        public virtual decimal Value { get; set; }
    }

    /// <summary>
    /// 包材汇总信息
    /// </summary>
    public class PackingMaterialSum
    {
        public virtual string Name { get; set; }
        public virtual decimal Value { get; set; }
    }
}
