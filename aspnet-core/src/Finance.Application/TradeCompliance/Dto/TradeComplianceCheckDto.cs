using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance.Dto
{
    /// <summary>
    /// 贸易合规表单接口输入参数Dto
    /// </summary>
    public class TradeComplianceInputDto
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
    }
    /// <summary>
    /// 贸易合规判定界面Dto
    /// </summary>
    public class TradeComplianceCheckDto
    {
        /// <summary>
        /// 贸易合规产品信息表
        /// </summary>
        public TradeComplianceCheck TradeComplianceCheck { get; set; }
        /// <summary>
        /// 产品组成物料信息表
        /// </summary>
        public List<ProductMaterialInfo> ProductMaterialInfos { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 财务贸易合规审核员规判定Dto
    /// </summary>
    public class FinanceCheckDto
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 判断意见（true退回到核价板，false直接归档）
        /// </summary>
        public bool IsAgree { get; set; }
        /// <summary>
        /// 审核意见说明
        /// </summary>
        public string OpinionDescription { get; set; }
    }


}
