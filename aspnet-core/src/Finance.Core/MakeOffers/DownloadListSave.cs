using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 归档文件列表实体类
    /// </summary>
    public class DownloadListSave : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 核报价项目名称
        /// </summary>   
        public string QuoteProjectName { get; set; }
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 零件名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 归档文件名称
        /// </summary>
        public string FileName   { get; set; }
        /// <summary>
        /// 归档文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 数据库FileManagement表中的Id
        /// </summary>
        public long FileId { get; set; }
    }
}
