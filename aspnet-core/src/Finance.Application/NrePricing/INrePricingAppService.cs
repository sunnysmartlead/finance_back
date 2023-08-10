using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing
{
    /// <summary>
    /// Nre核价接口
    /// </summary>
    public interface INrePricingAppService
    {
        /// <summary>
        /// 获取 零件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<List<SolutionModel>> TotalSolution(long Id);
        /// <summary>
        /// 项目管理部录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        Task PostProjectManagement(ProjectManagementDto price);
        /// <summary>
        /// 资源部录入初始值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<InitialResourcesManagementDto> GetInitialResourcesManagement(long Id);
        /// <summary>
        /// 计算 模具清单的 数量以及费用
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        Task<List<ResourcesManagementModel>> PostCalculateMouldInventory(ResourcesManagementDto resources);
        /// <summary>
        /// 资源部录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        Task  PostResourcesManagement(ResourcesManagementDto price);
        /// <summary>
        /// 产品部-电子工程师
        /// </summary>
        /// <returns></returns>
        Task  PostProductDepartment(ProductDepartmentDto price);
        /// <summary>
        /// Nre 品保部 录入
        /// </summary>
        /// <param name="qADepartmentDto"></param>
        /// <returns></returns>
        Task  PostQADepartment(QADepartmentDto qADepartmentDto);
        /// <summary>
        /// Ner  营销部录入初始值
        /// </summary>
        /// <param name="Id">流程表Id</param>
        /// <returns></returns>
        Task<List<ReturnSalesDepartmentDto>> GetInitialSalesDepartment(long Id);
        /// <summary>
        /// Ner  营销部录入
        /// </summary>
        /// <param name="departmentDtos"></param>
        /// <returns></returns>
        Task PostSalesDepartment(List<InitialSalesDepartmentDto> departmentDtos);
        /// <summary>
        /// 获取 Nre 核价表
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PartId"></param>
        /// <returns></returns>
        Task<PricingFormDto> GetPricingForm(long Id, long PartId);
    }
}
