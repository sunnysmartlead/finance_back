using Finance.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    /// <summary>
    /// /审批方法通用交互类
    /// </summary>
    public class ToExamineDto
    {
        /// <summary>
        /// 提交状态 0/拒绝  1/提交 2/保存
        /// </summary>
        public IsSubmitType SubmissionStatus { get; set; }
        /// <summary>
        /// 备注(必填)
        /// </summary>
        [FriendlyRequired("备注")]
        public string Remarks { get; set; }
    }
    /// <summary>
    /// 提交状态
    /// </summary>
    public enum IsSubmitType
    {
        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Refuse,
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit,
        /// <summary>
        /// 保存
        /// </summary>
        [Description("保存")]
        Preservation,
       
    }
}
