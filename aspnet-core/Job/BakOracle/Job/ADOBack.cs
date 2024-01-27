
using BakOracle.Ext;
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
    public class ADOBack
    {
        int taskCount = 1;
        string suffix = ".sql";
        const int MaxLength = 4000;
        public ADOBack()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!int.TryParse(Program.Config["taskCount"], out taskCount) || taskCount == 0)
            {
                taskCount = 1;
            }
        }
        public void Main()
        {   
            // 指定数据库连接信息
            var ip = "10.1.1.131";
            var userId = "WISSEN_TEST_V2";
            var password = "admin";

            // 构建文件路径
            string filePath = $@"File\{DateTime.Now.ToString("yyyyMMddHHmm")}";

            // 不必要的表过滤列表
            List<string> removeTables = new List<string>() { "Al", "__EFMigrationsHistory" };

            ParallelOptions parallelOptions = new ParallelOptions();

            // 并行线程数量
            parallelOptions.MaxDegreeOfParallelism = taskCount;

            // 数据库连接字符串
            string connectionString = $"Data Source={ip}/ORCL;Persist Security Info=True;user id={userId};password={password};";

            // 使用 OracleConnection 进行数据库连接
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // 获取所有表名
                    string getTableAllSql = $"SELECT table_name FROM all_tables WHERE owner = '{userId}'";
                    List<string> allTableNames = new List<string>();
                    using (OracleCommand command = new OracleCommand(getTableAllSql, connection))
                    {
                        OracleDataReader dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            allTableNames.Add(dataReader.GetValue(0).ToString());
                        }
                    }

                    // 移除不必要的表
                    allTableNames.RemoveAll(table => removeTables.Contains(table));

                    // 按指定顺序排序表名
                    List<string> orderedTableNames = new List<string>() { "U", "R", "Ur" };
                    allTableNames = allTableNames.OrderByFunc(table => table, orderedTableNames);

                    // 创建任务列表
                    List<Task> taskList = new List<Task>();

                    // 创建文件夹（如果不存在）
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    //allTableNames = new List<string>() { "AuditFlowIdPricingForm" };
                    // 遍历表名并创建任务
                    allTableNames.ForEach(tableName =>
                    {
                        taskList.Add(new Task(() =>
                        {
                            string query = $"SELECT * FROM \"{tableName}\"";
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                OracleDataReader dataReader = command.ExecuteReader();
                                string outputPath = filePath + $@"\{DateTime.Now.ToString("yyyyMMdd")}_{tableName}{suffix}";

                                using (StreamWriter streamWriter = new StreamWriter(outputPath,true, Encoding.GetEncoding("GB2312")))
                                {
                                    if (!File.Exists(outputPath))
                                    {
                                        File.CreateText(outputPath);
                                    }

                                    streamWriter.WriteLine($"--文件已创建{tableName} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                                    streamWriter.WriteLine("--======================开始========================");
                                    streamWriter.WriteLine("set define off;");
                                    while (dataReader.Read())
                                    {                                       
                                        var data = Enumerable.Range(0, dataReader.FieldCount)
                                            .Select(index => ($"\"{dataReader.GetName(index)}\"", dataReader.GetValue(index).ToSQLType()));
                                        var dataBool = data.Any(p => p.Item2.Length >= MaxLength);
                                        if (dataBool)
                                        {
                                            streamWriter.WriteLine("DECLARE");
                                            data.Select((pair, index) =>
                                            {
                                                if (pair.Item2.Length >= MaxLength)
                                                {
                                                    streamWriter.WriteLine($"v_clob{index} clob;");
                                                }
                                                return pair;
                                            }).ToList();
                                            streamWriter.WriteLine("BEGIN");
                                            data = data.Select((pair, index) =>
                                            {
                                                if (pair.Item2.Length >= MaxLength)
                                                {
                                                    streamWriter.WriteLine($"v_clob{index} :={pair.Item2};");
                                                    pair.Item2 = $"v_clob{index}";
                                                }
                                                return pair;
                                            }).ToList();
                                            streamWriter.WriteLine($"INSERT INTO \"{tableName}\"({string.Join(",", data.Select(pair => pair.Item1))}) VALUES({string.Join(",", data.Select(pair => pair.Item2))});");
                                            streamWriter.WriteLine("end;");
                                            streamWriter.WriteLine("/");
                                        }
                                        // 如果数据不需要拆分，则直接写入INSERT INTO语句
                                        else
                                        {
                                            streamWriter.WriteLine($"INSERT INTO \"{tableName}\"({string.Join(",", data.Select(pair => pair.Item1))}) VALUES({string.Join(",", data.Select(pair => pair.Item2))});");
                                        }

                                    }
                                    streamWriter.WriteLine("--======================结束========================");                                   
                                }
                            }
                        }));
                    });

                    // 并行执行任务
                    Parallel.ForEach(taskList, parallelOptions, task =>
                    {
                        task.Start();
                        task.Wait();                       
                    });

                    // 合并所有生成的文件
                    string allSqlPath = filePath + $@"\{DateTime.Now.ToString("yyyyMMdd")}__________________ALLSql{suffix}";
                    List<string> inputFiles = allTableNames.Select(prefix => $"{filePath}\\{DateTime.Now.ToString("yyyyMMdd")}_{prefix}{suffix}").ToList();
                    inputFiles.CombineFile(allSqlPath);
                    Console.WriteLine("--======================结束========================");
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

    }
}
