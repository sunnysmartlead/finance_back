using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Finance.Ext;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.NrePricing.Method.NrePricingMapper;

namespace Finance.NrePricing.Method
{
    /// <summary>
    /// Nre 方法 类
    /// </summary>
    public class NrePricingMethod : ISingletonDependency
    {
        /// <summary>
        /// 产品开发部结构BOM输入信息
        /// </summary>
        private readonly IRepository<StructureBomInfo, long> _resourceStructureBomInfo;
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 实体映射
        /// </summary>
        private static IObjectMapper ObjectMapper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="structureBomInfo"></param>
        /// <param name="modelCount"></param>
        /// <param name="objectMapper"></param>
        public NrePricingMethod(IRepository<StructureBomInfo, long> structureBomInfo, IRepository<ModelCount, long> modelCount, IObjectMapper objectMapper)
        {
            _resourceStructureBomInfo= structureBomInfo;
            _resourceModelCount= modelCount;
            ObjectMapper = objectMapper;
        }
        /// <summary>
        /// 根据 流程id和方案id 返回 模具清单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        internal async Task<List<MouldInventoryModel>> MouldInventoryModels(long id, long partId)
        {
            //筛选 结构BOM 还要进行 是否新开模的筛选
            List<StructureBomInfo> structureBomInfos = await _resourceStructureBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(id)&&p.ProductId.Equals(partId)&&p.IsNewMouldProduct.Equals(YesNO.Yes.GetDescription()));
            List<MouldInventoryModel> inventoryModels = ObjectMapper.Map<List<MouldInventoryModel>>(structureBomInfos);
            if (inventoryModels.Count is not 0) inventoryModels.Add(new MouldInventoryModel() { ModelName ="吸塑/包材" });
            return inventoryModels;
        }
        /// <summary>
        /// 计算 模组清单的数量
        /// </summary>
        /// <returns></returns>
        internal async Task<double> CalculateCount(long PartId, MouldInventoryModel mouldInventoryPartIdModel)
        {
            //获取物料的装配数量(BOM表)
            if(mouldInventoryPartIdModel.MoldCavityCount!=0&&mouldInventoryPartIdModel.ModelNumber!=0)
            {
                StructureBomInfo structureBomInfo = await _resourceStructureBomInfo.FirstOrDefaultAsync(p => p.Id.Equals(mouldInventoryPartIdModel.StructuralId));
                if (structureBomInfo is not null)
                {
                    double AssemblyQuantity = structureBomInfo.AssemblyQuantity;
                    //获取声明周期的产品总量 查询 某个零件的 模组总量
                    ModelCount modelCount = await _resourceModelCount.FirstOrDefaultAsync(p => p.Id.Equals(PartId));
                    double ModelTotal=0.0;
                    if(modelCount is not null) ModelTotal=modelCount.ModelTotal;
                    double price = AssemblyQuantity*ModelTotal/mouldInventoryPartIdModel.MoldCavityCount/mouldInventoryPartIdModel.ModelNumber;
                    return Math.Ceiling(price);
                }
            }
          
            return 0;
        }
    }
}
