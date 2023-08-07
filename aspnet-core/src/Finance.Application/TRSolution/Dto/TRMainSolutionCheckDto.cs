using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TRSolution.Dto
{
    /// <summary>
    /// TR主方案审核界面信息返回Dto
    /// </summary>
    public class TRMainSolutionCheckDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// TR主方案文件ID
        /// </summary>
        public long SolutionFileIdentifier { get; set; }
        /// <summary>
        /// TR主方案文件名称
        /// </summary>
        public string SolutionFileName { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
    }


    /// <summary>
    /// 表示TR主方案界面类型（1：“市场部TR主方案审核”，2：“产品开发部TR主方案审核”）
    /// </summary>
    public enum TRCHECKTYPE : byte
    {
        /// <summary>
        ///市场部TR主方案审核
        /// </summary>
        [Description("市场部TR主方案审核")]
        MKTTRCheck = 1,
        /// <summary>
        /// 产品开发部TR主方案审核
        /// </summary>
        [Description("产品开发部TR主方案审核")]
        R_DTRCheck = 2,
    }

    /// <summary>
    /// TR主方案审核Dto
    /// </summary>
    public class SetTRMainSolutionStateDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// TR主方案界面类型
        /// </summary>
        public TRCHECKTYPE TRCheckType { get; set; }
        /// <summary>
        /// TR主方案审核意见
        /// </summary>
        public bool IsAgree { get; set; }
        /// <summary>
        /// TR主方案审核意见说明
        /// </summary>
        public string OpinionDescription { get; set; }
    }
}
