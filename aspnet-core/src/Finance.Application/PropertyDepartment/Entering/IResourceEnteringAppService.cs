 
using Finance.Entering.Model;
using Finance.PropertyDepartment.Entering.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Entering
{
    /// <summary>
    /// 资源部录入接口
    /// </summary>
    public interface IResourceEnteringAppService
    {
        
        /// <summary>
        /// 资源部输入时,加载结构料初始值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<int> InitialValueOfLoadingStructuralMaterials(long Id);
        /// <summary>
        /// 电子料单价录入录入提交
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        Task  PostElectronicMaterialEntering(SubmitElectronicDto electronicDto);
        /// <summary>
        /// 计算电子料单价录入
        /// </summary>
        /// <param name="electronicBomList"></param>
        /// <returns></returns>
        Task<List<ElectronicDto>> PostElectronicMaterialCalculate(List<ElectronicDto> electronicBomList);
        /// <summary>
        /// 结构件单价录入
        /// </summary>
        /// <param name="structuralMemberEnteringModel"></param>
        /// <returns></returns>
        Task  PostStructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEnteringModel);
        /// <summary>
        /// 查看项目走量
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<List<ModuleNumberDto>> GetProjectGoQuantity(long Id);       
    }
}
