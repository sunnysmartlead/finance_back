using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProjectManagement.Dto
{
    /// <summary>
    /// 文件上传成功后的实体
    /// </summary>
    public class FileUploadOutputDto
    {
        /// <summary>
        /// 文件名称：文件上传的原始名称
        /// </summary>
        public string FileName { get; set; }


        /// <summary>
        /// 配置的文件保存路径+guid生成的文件名+文件的原始后缀
        /// </summary>
        public string FileUrl { get; set; }


        /// <summary>
        /// 文件的大小
        /// </summary>
        public long FileLength { get; set; }




        /// <summary>
        /// 文件的类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 数据库FileManagement表中的Id
        /// </summary>
        public long FileId { get; set; }
    }
}
