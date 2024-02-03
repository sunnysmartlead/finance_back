using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlImport.Models
{
    public class TaskList
    {
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string DegreeEmergency { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<FileModel>  FileModel { get; set; }
    }
}
