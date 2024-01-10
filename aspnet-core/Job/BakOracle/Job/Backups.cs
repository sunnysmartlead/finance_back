using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakOracle.Job
{

    public class Backups
    {
        private readonly ILogger<Backups> _logger;            
        public Backups(ILogger<Backups> logger)
        {          
            _logger = logger;
        }
        /// <summary>
        /// 调用脚本开始备份数据库
        /// </summary>
        public void GetIsMain()
        {           
            // 创建一个新的进程对象
            Process process = new Process();
            string path= AppDomain.CurrentDomain.BaseDirectory + @"Bat\Bak.bat";
            try
            {
                // 设置要执行的命令和参数 
                process.StartInfo.FileName = path;
                process.StartInfo.UseShellExecute = false;  //是否使用操作系统shell启动            
                process.StartInfo.UseShellExecute = false;//运行时隐藏dos窗口
                process.StartInfo.CreateNoWindow = true;//运行时隐藏dos窗口
                process.StartInfo.Verb = "runas";//设置该启动动作，会以管理员权限运行进程
                // 启动进程
                process.Start();
                // 等待进程执行完成
                process.WaitForExit();
                // 获取进程的退出代码
                int exitCode = process.ExitCode;
                Console.WriteLine("Exit code: " + exitCode);
                _logger.LogInformation($"路径是:{process.StartInfo.FileName}数据库自动备份完成=>>>{DateTime.Now},进程退出时候的代码=>>>"+ exitCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                _logger.LogError($"数据库自动备份失败=>>>{DateTime.Now},错误消息=>>>" + ex.Message);
            }
            finally
            {
                // 关闭进程
                process.Close();
                process.Dispose();
            }
        }
         
    }
}
