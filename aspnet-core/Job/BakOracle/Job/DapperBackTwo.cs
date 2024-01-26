
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BakOracle.Job
{
    public class DapperBacktTow
    {
        List<List<string>> listAll = new List<List<string>>();
        public void Main()
        {
            var ip = "10.1.1.131";
            var userId = "WISSEN_TEST_V2";
            var password = "admin";
            //过滤不必要的表
            List<string> Remove = new List<string>() { "Al", "__EFMigrationsHistory" };
            //开启线程数量
            const int taskCount = 30;
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = taskCount;
            using (OracleConnection connection = new OracleConnection($"Data Source={ip}/ORCL;Persist Security Info=True;user id={userId};password={password};"))
            {
                try
                {
                    connection.Open();
                    var tableNames = connection.Query<string>($"SELECT table_name FROM all_tables WHERE owner = '{userId}'").ToList();
                    List<string> list = tableNames.ToList();
                    list.RemoveAll(p => Remove.Contains(p));
                    List<Task> listTask = new();
                    list.ForEach(rowTable =>
                    {
                        listTask.Add(new Task(() =>{
                                List<string> strings = new List<string>();
                                string query = $"SELECT * FROM \"{rowTable}\"";
                                var data = connection.Query(query);
                                foreach (var item in data)
                                {
                                    var ppp = (IDictionary<string, object>)item;
                                    var keys = ppp.Keys.ToList();
                                    var values = ppp.Values.Select(val => FormatValue(val)).ToList();

                                    string keysNames = "\"" + string.Join("\",\"", keys) + "\"";
                                    string valuesNames = string.Join(",", values);

                                    strings.Add($"INSERT INTO \"{rowTable}\"({keysNames}) VALUES({valuesNames}) ");
                                    //Console.WriteLine($"INSERT INTO \"{rowTable}\"({keysNames}) VALUES({valuesNames}) ");
                                }
                                string path = $@"File\{DateTime.Now.ToString("yyyyMMdd")}_{rowTable}.txt";
                                SaveToFile(path, rowTable, strings);
                                listAll.Add(strings);
                         }));
                    });

                  
                    Parallel.ForEach(listTask, parallelOptions, rowTable =>
                    {
                        rowTable.Start();
                        rowTable.Wait();
                    });

                    string pathAll = $@"File\{DateTime.Now.ToString("yyyyMMdd")}_ALLSql.txt";
                    SaveAllToFile(pathAll, list);
                    connection.Close();
                }
                catch (Exception e)
                {
                    throw;
                }
            }

        }
        // Helper method to format values
        private string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (IsDate(value.ToString()))
            {
                return $"TO_DATE('{value}','YYYY-MM-DD HH24:MI:SS')";
            }
            else if (IsNumber(value.ToString()))
            {
                return value.ToString();
            }
            else
            {
                return $"'{value}'";
            }
        }

        // Helper method to save to individual file
        private void SaveToFile(string path, string rowTable, List<string> strings)
        {
            if(strings.Count==0)
            {
                File.CreateText(path+"空");
                return;
            }
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine($"--文件已创建{rowTable} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    sw.WriteLine("--======================开始========================");
                    foreach (var item in strings)
                    {
                        sw.WriteLine(item);
                    }
                    sw.WriteLine("--======================结束========================");
                }
            }
        }

        // Helper method to save to all file
        private void SaveAllToFile(string pathAll, List<string> list)
        {
            if (list.Count == 0)
            {
                File.CreateText(pathAll + "空");
                return;
            }
            if (!File.Exists(pathAll))
            {
                using (StreamWriter sw = File.CreateText(pathAll))
                {
                    sw.WriteLine($"--文件已创建_ALLSql - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    sw.WriteLine("----------------------------------------------------");
                    foreach (var rowTable in list)
                    {
                        sw.WriteLine($"--====================={rowTable}=========================");
                    }
                    sw.WriteLine("--======================开始========================");
                    foreach (List<string> item in listAll)
                    {
                        foreach (var item2 in item)
                        {
                            sw.WriteLine(item2);
                        }
                    }
                    sw.WriteLine("--======================结束========================");
                }
            }
        }
        private bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);  //不是字符串时会出现异常
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsNumber(string strDate)
        {
            try
            {
                decimal.Parse(strDate);  //不是字符串时会出现异常
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
