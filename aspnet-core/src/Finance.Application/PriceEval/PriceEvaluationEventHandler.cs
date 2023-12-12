using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.DemandApplyAudit;
using Finance.WorkFlows;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价需求录入后的事件处理
    /// </summary>
    public class PriceEvaluationEventHandler : IEventHandler<EntityCreatedEventData<PriceEvaluation>>, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<UpdateItem, long> _updateItemRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unitOfWorkManager"></param>
        /// <param name="gradientRepository"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="solutionRepository"></param>
        /// <param name="updateItemRepository"></param>
        public PriceEvaluationEventHandler(IUnitOfWorkManager unitOfWorkManager, IRepository<Gradient, long> gradientRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<Solution, long> solutionRepository, IRepository<UpdateItem, long> updateItemRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _gradientRepository = gradientRepository;
            _modelCountRepository = modelCountRepository;
            _solutionRepository = solutionRepository;
            _updateItemRepository = updateItemRepository;
        }

        /// <summary>
        /// 核价需求录入后触发
        /// </summary>
        /// <param name="eventData"></param>
        public async void HandleEvent(EntityCreatedEventData<PriceEvaluation> eventData)
        {
            //如果引用流程的ID无值或为默认值，不执行
            if ((!eventData.Entity.QuickQuoteAuditFlowId.HasValue) || eventData.Entity.QuickQuoteAuditFlowId == default)
            {
                return;
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    //获取ProductId对应关系
                    var oldProduct = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == eventData.Entity.QuickQuoteAuditFlowId).Select(p => new { p.Id, p.Product }).ToListAsync();
                    var newProduct = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == eventData.Entity.AuditFlowId).Select(p => new { p.Id, p.Product }).ToListAsync();
                    var productIdMap = from o in oldProduct
                                       join n in newProduct on o.Product equals n.Product
                                       select new
                                       {
                                           OldProductId = o.Id,
                                           NewProductId = n.Id
                                       };


                    //获取GradientId对应关系
                    var oldGradient = await _gradientRepository.GetAll().Where(p => p.AuditFlowId == eventData.Entity.QuickQuoteAuditFlowId).Select(p => new { p.Id, p.GradientValue }).ToListAsync();
                    var newGradient = await _gradientRepository.GetAll().Where(p => p.AuditFlowId == eventData.Entity.AuditFlowId).Select(p => new { p.Id, p.GradientValue }).ToListAsync();
                    var gradientIdMap = from o in oldGradient
                                        join n in newGradient on o.GradientValue equals n.GradientValue
                                        select new
                                        {
                                            OldGradient = o.Id,
                                            NewGradient = n.Id
                                        };

                    //获取SolutionId对应关系
                    var oldSolution = await _solutionRepository.GetAllListAsync(p => p.AuditFlowId == eventData.Entity.QuickQuoteAuditFlowId);
                    var newSolution = await _solutionRepository.GetAllListAsync(p => p.AuditFlowId == eventData.Entity.AuditFlowId);
                    var solutionIdMap = from o in oldSolution
                                        join n in newSolution on o.Product equals n.Product
                                        select new
                                        {
                                            OldSolution = o.Id,
                                            NewSolution = n.Id
                                        };

                    //读取修改项内容
                    var oldUpdateItems = await _updateItemRepository.GetAllListAsync(p => p.AuditFlowId == eventData.Entity.QuickQuoteAuditFlowId);
                    foreach (var oldUpdateItem in oldUpdateItems)
                    {
                        var solution = await _solutionRepository.GetAsync(oldUpdateItem.SolutionId);
                        var productId = solution.Productld;

                        var updateItem = new UpdateItem
                        {
                            AuditFlowId = eventData.Entity.AuditFlowId,
                            ProductId = productIdMap.FirstOrDefault(p => p.OldProductId == productId).NewProductId,
                            GradientId = gradientIdMap.FirstOrDefault(p => p.OldGradient == oldUpdateItem.GradientId).NewGradient,
                            SolutionId = solutionIdMap.FirstOrDefault(p => p.OldSolution == oldUpdateItem.SolutionId).NewSolution,
                            UpdateItemType = oldUpdateItem.UpdateItemType,
                            Year = oldUpdateItem.Year,
                            UpDown = oldUpdateItem.UpDown,
                            MaterialJson = oldUpdateItem.MaterialJson,
                            File = oldUpdateItem.File
                        };
                        await _updateItemRepository.InsertAsync(updateItem);
                    }
                }
                uow.Complete();
            }
        }
    }
}
