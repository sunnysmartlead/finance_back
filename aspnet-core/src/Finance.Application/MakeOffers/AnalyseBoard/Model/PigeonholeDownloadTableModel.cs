using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 归档文件下载列表 模型
    /// </summary>
    public class PigeonholeDownloadTableModel
    {
        /// <summary>
        /// 归档文件列表id
        /// </summary>
        public long DownloadListSaveId { get; set; }
        /// <summary>
        /// 归档文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 零件名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 核报价项目名称
        /// </summary>   
        public string QuoteProjectName { get; set; }
        public int MyProperty { get; set; }

    }
    /// <summary>
    /// 归档文件下载列表 模型(带 tab)
    /// </summary>
    public class TabPigeonholeDownloadTableModel
    {
        /// <summary>
        /// 核报价项目名称
        /// </summary>   
        public string QuoteProjectName { get; set; }
        /// <summary>
        /// 归档文件下载列表 模型
        /// </summary>
        public List<PigeonholeDownloadTableModel> PigeonholeDownloadTableModels { get; set; }
    }
}
