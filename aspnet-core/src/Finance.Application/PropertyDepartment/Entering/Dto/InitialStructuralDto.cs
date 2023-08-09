using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary>
    /// 资源部录入时 结构料 加载初始值 交互类
    /// </summary>
    public class InitialStructuralDto
    {
        /// <summary>
        ///  方案模型
        /// </summary>
        public List<SolutionModel> PartDtoList { get; set; }
        /// <summary>
        /// 结构BOM单价表单
        /// </summary>
        public List<ConstructionDto> ConstructionBomList { get; set; }
    }
}
