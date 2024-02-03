using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlImport.ViewModels.Base;

namespace SqlImport.Utils
{
    public class FileHelp: Controller
    {
        /// <summary>
        /// 判断目标是文件夹还是目录(目录包括磁盘)
        /// </summary>
        /// <param name="filepath">路径</param>
        /// <returns>返回true为一个文件夹，返回false为一个文件</returns>
        public static bool IsDir(string filepath)
        {
            try
            {
                FileInfo fi = new FileInfo(filepath);
                return (fi.Attributes & FileAttributes.Directory) != 0;
            }
            catch (Exception)
            {
                return true;
            }
        }

    }
}
