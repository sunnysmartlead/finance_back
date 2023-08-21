using Finance.Dto;
using Finance.Ext;
using Finance.NrePricing.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 品保部=>试验项目 录入 模型
    /// </summary>
    public class ExperimentItemsDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public List<ExperimentItemsModel> ExperimentItems { get; set; }
    }
    /// <summary>
    /// Nre 品保部=>试验项目 录入 模型(单个零件)
    /// </summary>
    public class ExperimentItemsSingleDto: ToExamineDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        ///品保录入  试验项目表 模型
        /// </summary>
        public List<EnvironmentalExperimentFeeModel> EnvironmentalExperimentFeeModels { get; set; }
        /// <summary>
        /// 是否提交 true 提价  false 保存
        /// </summary>
        public bool IsSubmit { get; set; }
    }   
    /// <summary>
    /// 带零件id 的 品保部=>试验项目 录入 模型
    /// </summary>
    public class ExperimentItemsModel
    {
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        ///品保录入  试验项目表 模型
        /// </summary>
        public List<EnvironmentalExperimentFeeModel> EnvironmentalExperimentFeeModels { get; set; }
        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }
  
}
