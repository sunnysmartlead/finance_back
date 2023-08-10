using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary>
    /// 资源部录入时 电子料 加载初始值 交互类
    /// </summary>
    public class InitialElectronicDto
    {
        /// <summary>
        /// 方案模型
        /// </summary>
        public List<SolutionModel> PartDtoList { get; set; }
        /// <summary>
        /// 电子BOM单价表单
        /// </summary>
        public List<ElectronicDto> ElectronicBomList { get; set; }
    }
}
