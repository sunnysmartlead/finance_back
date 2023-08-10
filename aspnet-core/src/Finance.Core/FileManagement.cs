using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance
{
    /// <summary>
    /// 文件管理表
    /// </summary>
     [Table("FILEMANAGEMENT ")]
    public class FileManagement: FullAuditedEntity<long>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        [Column("PATH")]
        public string Path { get; set; }

        /// <summary>
        /// 文件标记
        /// </summary>
        [Column("FILETAG")]
        public string FileTag { get; set; }
    }
}
