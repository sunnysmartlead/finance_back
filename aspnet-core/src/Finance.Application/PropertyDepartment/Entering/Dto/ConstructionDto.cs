using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Dto
{
    // /// <summary>
    /// 结构BOM表单 交互类
    /// </summary>
    public class ConstructionDto 
    {      
        /// <summary>
        /// 超级大种类  结构料 胶水等辅材 SMT外协 包材
        /// </summary> 
        public string SuperTypeName { get; set; }      
        /// <summary>
        /// 结构料
        /// </summary>
        public List<ConstructionModel> StructureMaterial { get; set; }       
    }
    // /// <summary>
    /// 结构BOM表单复制项 交互类
    /// </summary>
    public class ConstructionDtoCopy
    {
        /// <summary>
        /// 超级大种类  结构料 胶水等辅材 SMT外协 包材
        /// </summary> 
        public string SuperTypeName { get; set; }
        /// <summary>
        /// 结构料
        /// </summary>
        public List<ConstructionModelCopy> StructureMaterial { get; set; }
    }
    public class IsALLConstructionDto
    {
        /// <summary>
        /// 是否全部提交完成
        /// </summary>
        public bool isAll { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalNumber { get; set; }
        /// <summary>
        /// 提交的条数
        /// </summary>
        public int NumberOfSubmissions { get; set; }
        /// <summary>
        /// table表格中的内容
        /// </summary>

        public List<ConstructionDto> ConstructionDtos { get; set; }
    }
}
