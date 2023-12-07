using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 项目看板实体类 模型
    /// </summary>
    public class ProjectBoardModel
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 目标价(内部)
        /// </summary>
        public GrossMarginModel InteriorTarget { get; set; }
        /// <summary>
        /// 目标价(客户)
        /// </summary>
        public GrossMarginModel ClientTarget { get; set; }
        /// <summary>
        /// 本次报价
        /// </summary>
        public GrossMarginModel Offer { get; set; }       
        /// <summary>
        /// 之前几轮的 本次报价的值
        /// </summary>
        public List<GrossMarginModel> OldOffer { get; set; }
    }

    public class BoardModel
    {
        
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }
        public List<ProjectBoardSecondModel> ProjectBoardModels { get; set; }
    }

    public class ProjectBoardSecondModel
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 数据id
        /// </summary>
        public long Id{ get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 目标价(内部)
        /// </summary>
        public decimal InteriorTarget { get; set; }
        /// <summary>
        /// 目标价(客户)
        /// </summary>
        public decimal ClientTarget { get; set; }
        /// <summary>
        /// 本次报价
        /// </summary>
        public decimal? Offer { get; set; }       
        /// <summary>
        /// 之前几轮的 本次报价的值
        /// </summary>
        public decimal? OldOffer { get; set; }
        
        
        
        
    }
}
