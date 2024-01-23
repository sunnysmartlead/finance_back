using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;

namespace OracleSeq
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = "139.196.216.165";
            var userId = "FINANCE_V2_COPY";
            var password = "admin";

            using (IDbConnection connection = new OracleConnection($"Data Source={ip}/ORCL;Persist Security Info=True;user id={userId};password={password};"))
            {
                string result = string.Empty;
                connection.Open();
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT SEQUENCE_NAME FROM ALL_SEQUENCES WHERE SEQUENCE_OWNER = '{userId}'"; // 替换'YOUR_SCHEMA_NAME'为你的模式名称  
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        StringBuilder sb = new StringBuilder();
                        while (reader.Read())
                        {
                            string sequenceName = reader.GetString(0);
                            sb.Append(sequenceName).Append(",");
                        }
                        result = sb.ToString().TrimEnd(','); // 移除最后的逗号                       
                    }
                }
                var list2 = result.Split(",").Select(p => p.Replace("\r\n", string.Empty)).ToList();
                Console.WriteLine("序列个数: " + list2.Count);
                WriteLineBlue($"连接到{ip}/{userId}");
                WriteLineBlue($"输入任意字符开始刷新");
                Console.ReadKey();
                WriteLineBlue($"开始刷新序列...");

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

                //var list = connection.Query<string>($"SELECT SEQUENCE_NAME FROM DBA_SEQUENCES WHERE SEQUENCE_OWNER='{userId}'").Select(p => p.Replace("SQ_", string.Empty)).Select(p => new { p, sql = string.Format(sqlModel, p) });


                var list = list2.Select(p => p.Replace("SQ_", string.Empty)).Select(p => new { p, sql = string.Format(sqlModel, p) });

                foreach (var item in list)
                {
                    try
                    {
                        connection.Execute(item.sql);                         
                        WriteLineOk($"{item.p}成功");
                    }
                    catch (Exception e)
                    {
                        WriteLineError($"{item.p}失败：{e.Message}");
                    }
                }               
            }
            WriteLineOk($"结束任务!");
            Console.ReadKey();
        }

        internal static void WriteLineBlue(string str)
        {
            Console.WriteLine($"[underline blue]{str}[/]");
        }

        internal static void WriteLineOk(string str)
        {
            Console.WriteLine($"[underline lime]{str}[/]");
        }

        internal static void WriteLineError(string str)
        {
            Console.WriteLine($"[underline red]{str}[/]");
        }

        //删除数据库表记录

        //Console.WriteLine($"开始刷新序列...");

        //var sqlModel = @"DELETE FROM ""{0}""";

        //var list = connection.Query<string>($"SELECT TABLE_NAME FROM ALL_TABLES WHERE TABLESPACE_NAME ='{userId}'").Select(p => new { p, sql = string.Format(sqlModel, p) });

        //foreach (var item in list)
        //{
        //    try
        //    {
        //        connection.Execute(item.sql);
        //        Console.WriteLine($"{item.p}成功");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"{item.p}失败：{e.Message}");
        //    }
        //}
    }
}
