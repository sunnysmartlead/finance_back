using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlImport.Models
{
    public class FileModel
    {
        /// <summary>
        /// 图片全路径
        /// </summary>
        public string ImgAllName { get; set; }
        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImgName { get; set; }
        /// <summary>
        /// json全路径
        /// </summary>
        public string JsonAllName { get; set; }
        /// <summary>
        /// json名称
        /// </summary>
        public string JsonName { get; set; }
        /// <summary>
        /// Sql全路径
        /// </summary>
        public string SqlAllName { get; set; }
        /// <summary>
        /// Sql名称
        /// </summary>
        public string SqlName { get; set; }
        /// <summary>
        /// 不带后缀的名称
        /// </summary>
        public string Name { get; set; }
    }
}
