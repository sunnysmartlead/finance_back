
using BakOracle.Ext;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BakOracle.Job
{
    public class ADOBackTwo
    {
        private readonly ILogger<Backups> _logger;
        int taskCount = 1;
        string suffix = ".sql";
        const int MaxLength = 3000;
        private static Encoding _encoding = System.Text.Encoding.GetEncoding("GB2312");
        public ADOBackTwo(ILogger<Backups> logger)
        {
            _logger = logger;
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
#if DEBUG

#else
 filePath = AppDomain.CurrentDomain.BaseDirectory + filePath;
#endif

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
                    //allTableNames = new List<string>() { "Pe_PanelJson" };
                    // 遍历表名并创建任务
                    allTableNames.ForEach(tableName =>
                    {
                        taskList.Add(new Task(() =>
                        {
                            var hasId = connection.QuerySingle<bool>($"SELECT Count(*) FROM USER_TAB_COLUMNS where table_name='{tableName}' and column_name='Id'");
                            string query = $"SELECT * FROM \"{tableName}\"";
                            if (hasId)
                            {
                                query += "Order by \"Id\"";
                            }
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                OracleDataReader dataReader = command.ExecuteReader();
                                string outputPath = filePath + $@"\{DateTime.Now.ToString("yyyyMMdd")}_{tableName}{suffix}";

                                using (StreamWriter streamWriter = new StreamWriter(outputPath,true, _encoding))
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
                                        var dataBool = data.Any(p => _encoding.GetBytes(p.Item2.ToCharArray()).Length >= MaxLength);                                       
                                        if (dataBool)
                                        {
                                            var Id = data.FirstOrDefault(p => p.Item1.Equals("\"Id\"")).Item2;
                                            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
                                            streamWriter.WriteLine("DECLARE");
                                            data.Select((pair, index) =>
                                            {
                                                if (_encoding.GetBytes(pair.Item2.ToCharArray()).Length >= MaxLength)
                                                {
                                                    streamWriter.WriteLine($"v_clob{index} clob;");
                                                    keyValuePairs.Add($"{pair.Item1}", SplitByByteLength(pair.Item2, MaxLength));
                                                }
                                                return pair;
                                            }).ToList();
                                            streamWriter.WriteLine("BEGIN");
                                            data = data.Select((pair, index) =>
                                            {
                                                if (_encoding.GetBytes(pair.Item2.ToCharArray()).Length >= MaxLength)
                                                {
                                                    streamWriter.WriteLine($"v_clob{index} :='随便什么';");
                                                    pair.Item2 = $"v_clob{index}";
                                                }
                                                return pair;
                                            }).ToList();

                                            streamWriter.WriteLine($"INSERT INTO \"{tableName}\"({string.Join(",", data.Select(pair => pair.Item1))}) VALUES({string.Join(",", data.Select(pair => pair.Item2))});");
                                            //清空清空DataJson字段
                                            foreach (var item in keyValuePairs)
                                            {
                                                streamWriter.WriteLine($"UPDATE \"{tableName}\"  SET {item.Key} = EMPTY_CLOB() WHERE \"Id\" = {Id};");                                            
                                                foreach (var valuies in item.Value)
                                                {
                                                    String stringBuilder = valuies;
                                                    if (stringBuilder.Length > 0 && stringBuilder[0] == '\'')
                                                    {
                                                        stringBuilder=stringBuilder.TrimStart('\'');
                                                    }

                                                    if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\'')
                                                    {
                                                        stringBuilder=stringBuilder.TrimEnd('\'');
                                                    }
                                                    streamWriter.WriteLine($"UPDATE \"{tableName}\"  SET {item.Key} ={item.Key}||\'{stringBuilder}'  WHERE \"Id\" = {Id};");
                                                }
                                            }                                           
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
                                    _logger.LogInformation($"日志打印日期:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ,{streamWriter}表已备份完成, 地址为:{outputPath}");
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
                    _logger.LogError($"日志打印日期:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},数据库自动备份失败=>>>{DateTime.Now},错误消息=>>>" + e.Message);
                }
            }
        }
        //第一种方法
        private string SubstringByte(string text, int startIndex, int length)
        {
            byte[] bytes = _encoding.GetBytes(text);
            return _encoding.GetString(bytes, startIndex, length);
        }
        /// <summary>
        /// 按照指定字节长度截取字符串
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="maxByteLength">最大字节长度</param>
        /// <returns>截取后的字符串列表</returns>
        static List<string> SplitByByteLength(string input, int maxByteLength)
        {
            // 用于存储截取后的字符串列表
            List<string> result = new List<string>();
            // 将输入字符串转换为字节数组
            byte[] bytes = Encoding.Default.GetBytes(input);
            int index = 0;
            while (index < bytes.Length)
            {
                // 用于构建截取后的子串
                StringBuilder builder = new StringBuilder();
                int byteCount = 0;
                int length = 0;
                while (index + length < bytes.Length && byteCount < maxByteLength)
                {
                    // 获取当前字节
                    byte currentByte = bytes[index + length];
                    if (IsChineseCharacter(currentByte))
                    {
                        // 如果是中文字符，将3个字节添加到子串中
                        byteCount += 3;
                        builder.Append(Encoding.Default.GetString(bytes, index + length, 3));
                        length += 3;
                    }
                    else
                    {
                        // 如果不是中文字符，将1个字节添加到子串中
                        byteCount += 1;
                        builder.Append((char)currentByte);
                        length += 1;
                    }
                }
                // 将截取后的子串添加到结果列表中
                result.Add(builder.ToString());
                index += length;
            }
            // 返回截取后的字符串列表
            return result;
        }

        /// <summary>
        /// 判断一个字节是否为中文字符
        /// </summary>
        /// <param name="b">输入字节</param>
        /// <returns>是否为中文字符</returns>
        static bool IsChineseCharacter(byte b)
        {
            // 判断一个字节是否为中文字符
            return b >= 0x80;
        }
    }
}
