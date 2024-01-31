using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DbExp
{
    internal static partial class StringExtensions
    {
        internal const string ExpDb = "备份数据库";
        internal const string ImportDb = "导入数据库";
        internal const string FilePath = "Data";

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static void Exp(this string connectionString, string dbName)
        {
            using var connection = new OracleConnection(connectionString);
            connection.Open();//打开数据库  
            Console.WriteLine($"连接到{dbName}");
            long dataCount = 0;
            var time = new Stopwatch();
            time.Start();


            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using var sw = new StreamWriter($@"{FilePath}\{dbName}{DateTime.Now:yyyy-MM-dd hh：mm}.sql", true, Encoding.GetEncoding("GB2312"));

            //不备份的表
            var noexp = new List<string> { "Al", "L", "__EFMigrationsHistory" };

            var list = connection.Query<string>($"SELECT TABLE_NAME FROM USER_TABLES")
                .Where(p => !noexp.Contains(p))
                .OrderBy(p => p.TableOrder()).ToList();

            foreach (var item in list)
            {
                Console.WriteLine($"正在备份{item}表");

                var ii = 0;
                ConsoleUtility.WriteProgress(ii);

                var hasId = connection.QuerySingle<bool>($"SELECT Count(*) FROM USER_TAB_COLUMNS where table_name='{item}' and column_name='Id'");
                var selectSQL = $"SELECT * FROM \"{item}\" ";
                if (hasId)
                {
                    selectSQL += "Order by \"Id\"";
                }
                var dr = new OracleCommand(selectSQL, connection).ExecuteReader();
                while (dr.Read())
                {
                    ConsoleUtility.WriteProgress(++ii, true);

                    var datas = Enumerable.Range(0, dr.FieldCount).Select(p => ($"\"{dr.GetName(p)}\"", dr.GetValue(p).ToSQL(dr.GetFieldType(p))));

                    sw.WriteLine($"Insert into \"{item}\" ({string.Join(",", datas.Select(p => p.Item1))}) values ({string.Join(",", datas.Select(p => p.Item2.SQL))});");

                    if (datas.Any(p => p.Item2.Appends != null))
                    {
                        foreach (var data in datas)
                        {
                            if (data.Item2.Appends != null)
                            {
                                foreach (var append in data.Item2.Appends)
                                {
                                    sw.WriteLine($"UPDATE \"{item}\" SET {data.Item1} ={data.Item1}||{append} WHERE \"Id\" = {dr.GetInt64(dr.GetOrdinal("Id"))};");
                                }
                            }
                        }
                    }
                    dataCount++;
                }
                Console.WriteLine();

                Console.WriteLine($"{item}表备份完毕");
                dr.Close();
            }
            sw.Close();
            connection.Close();
            time.Stop();
            Console.WriteLine($"全部备份完毕，共备份{list.Count}个表，{dataCount}条数据。耗时{time.Elapsed}");
            Console.ReadLine();
        }

        internal static void Import(this string connectionString, string dbName)
        {
            Console.WriteLine($"请选择要导入的文件");
            var files = Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories).Select((s, i) => (s, i)).ToList();
            foreach (var (s, i) in files)
            {
                Console.WriteLine($"{i + 1}、{s}");
            }
            var selectAction = "输入序号选择".GetString(p =>
            {
                try
                {
                    if ((!files.Select(o => o.i).Contains(Convert.ToInt32(p) - 1)))
                    {
                        throw new Exception("输入不正确，请重新输入");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("输入不正确，请重新输入");
                }
                return Convert.ToInt32(p) - 1;
            });
            using var sw = new StreamWriter($@"{FilePath}\{dbName}导入错误日志{DateTime.Now:yyyy-MM-dd hh：mm}.sql", true, Encoding.GetEncoding("GB2312"));

            using var connection = new OracleConnection(connectionString);
            connection.Open();//打开数据库  
            Console.WriteLine($"连接到{dbName}");
            var time = new Stopwatch();
            time.Start();
            var sqlCount = 0;
            var errorCount = 0;

            var ii = 0;
            ConsoleUtility.WriteProgress(ii);
            foreach (var sql in File.ReadLines(files[selectAction].s, Encoding.GetEncoding("GB2312")))
            {
                ConsoleUtility.WriteProgress(++ii, true);
                using (var cmd = new OracleCommand(sql[..^1], connection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery(); // 执行每条 SQL 语句
                        sqlCount++;
                    }
                    catch (Exception ex)
                    {
                        sw.WriteLine("--错误SQL：");
                        sw.WriteLine(sql);
                        sw.WriteLine($"--错误信息：{ex.Message}");
                        sw.WriteLine($"--堆栈跟踪：{ex.StackTrace}");
                        sw.WriteLine();
                        errorCount++;
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("数据导入完毕，开始刷新序列");

            var sqlModel = @"
            DECLARE
            vnumber NUMBER;
            nnumber NUMBER;
            BEGIN
            SELECT ((SELECT max(""Id"") FROM ""{0}"") -
            ""SQ_{0}"".nextval)
            INTO vnumber
            FROM dual;
            IF vnumber > 0 THEN
            EXECUTE IMMEDIATE 'ALTER SEQUENCE ""SQ_{0}"" INCREMENT BY ' ||
            vnumber;
            SELECT ""SQ_{0}"".nextval INTO nnumber FROM dual;
            EXECUTE IMMEDIATE 'ALTER SEQUENCE ""SQ_{0}"" INCREMENT BY 1 cache 20';
            END IF;
            END;";

            var list = connection.Query<string>("SELECT SEQUENCE_NAME FROM USER_SEQUENCES").Select(p => p.Replace("SQ_", string.Empty)).Select(p => new { p, sql = string.Format(sqlModel, p) });

            foreach (var item in list)
            {
                try
                {
                    connection.Execute(item.sql);
                    Console.WriteLine($"{item.p}序列刷新成功");
                }
                catch (Exception ex)
                {
                    sw.WriteLine($"--{item.p}序列刷新失败：");
                    sw.WriteLine($"--错误信息：{ex.Message}");
                    sw.WriteLine($"--堆栈跟踪：{ex.StackTrace}");
                    sw.WriteLine();

                    Console.WriteLine($"{item.p}序列刷新失败：{ex.Message}");
                }
            }

            sw.Close();
            connection.Close();
            time.Stop();
            Console.WriteLine($"全部导入完毕，共导入{sqlCount}条，有个{errorCount}错误。耗时{time.Elapsed}");
            Console.ReadLine();

        }


        internal static T GetString<T>(this string msg, Func<string, T> func)
        {
            Console.WriteLine(msg);
            var userInput = Console.ReadLine();
            try
            {
                return func(userInput);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                return GetString(msg, func);
            }
        }

        private const int OracleMaxStringLength = 1000;

        private static int GetSubLength(int index, int stringLength)
        {
            var startIndex = (index + 1) * OracleMaxStringLength;
            if (startIndex > stringLength)
            {
                return stringLength - index * OracleMaxStringLength;
            }
            else
            {
                return OracleMaxStringLength;
            }
        }

        internal static (string SQL, List<string>? Appends) ToSQL(this object data, Type type)
        {
            if (data.GetType() == DBNull.Value.GetType())
            {
                return ("null", null);
            }
            if (type == typeof(DateTime))
            {
                return ($"to_timestamp('{Convert.ToDateTime(data):yyyy-MM-dd HH:mm:ss.fff}','yyyy-mm-dd hh24:mi:ss.ff3')", null);
            }
            else if (type == typeof(string))
            {
                var sql = data.ToString().Replace("'", "''");
                if (sql.Length > OracleMaxStringLength)
                {
                    var list = new List<string>();
                    var sqlCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(sql.Length / OracleMaxStringLength)));
                    string returnSQL = string.Empty;
                    for (var i = 0; i < sqlCount; i++)
                    {
                        if (i == 0)
                        {
                            returnSQL = $"'{sql.Substring(i * OracleMaxStringLength, GetSubLength(i, sql.Length))}'";
                        }
                        else
                        {
                            list.Add($"'{sql.Substring(i * OracleMaxStringLength, GetSubLength(i, sql.Length))}'");
                        }
                    }
                    return (returnSQL, list);
                }
                else
                {
                    return ($"'{sql}'", null);
                }
            }
            else
            {
                return ($"{data}", null);
            }
        }
        internal static long TableOrder(this string data)
        {
            return data switch
            {
                "U" => 1,
                "R" => 2,
                "Ur" => 3,
                _ => 10,
            };
        }
    }
}
