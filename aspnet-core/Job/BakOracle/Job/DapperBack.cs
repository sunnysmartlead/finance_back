
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;

namespace BakOracle.Job
{
    public class DapperBack
    {
        public void Main()
        {
            var ip = "139.196.216.165";
            var userId = "FINANCE_V2_COPY";
            var password = "admin";

            using (IDbConnection connection = new OracleConnection($"Data Source={ip}/ORCL;Persist Security Info=True;user id={userId};password={password};"))
            {
                try
                {
                    string result = string.Empty;
                    connection.Open();
                    var tableNames = connection.Query<string>($"SELECT table_name FROM all_tables WHERE owner = '{userId}'");
                    List<string> list = new List<string>();
                    list = tableNames.ToList();
                    StringBuilder sqlBuilder = new StringBuilder();
                    //string sql = "INSERT INTO \"ElectronicBomInfoBak\"(\"Id\", \"AUDITFLOWID\", \"PRODUCTID\", \"PRODUCT\", \"SOLUTIONID\", \"SOLUTIONNUM\", \"IDNUMBER\", \"CATEGORYNAME\", \"TYPENAME\", \"ISINVOLVEITEM\", \"SAPITEMNUM\", \"SAPITEMNAME\", \"ASSEMBLYQUANTITY\", \"ENCAPSULATIONSIZE\", \"FILEID\", \"CreationTime\", \"CreatorUserId\", \"LastModificationTime\", \"LastModifierUserId\", \"IsDeleted\", \"DeleterUserId\", \"DeletionTime\") VALUES(\"1\", \"67\", \"526\", \"null\", \"63\", \"null\", \"0\", \"Sensor板\", \"芯片IC—电源芯片\", \"是\", \"3190X1845\", \"IR LEDLED_PA35RT1-811D_LEXTAR\", \"2\", \"2AFSDF\", \"0\", \"2023/6/28 17:09:13\", \"null\", \"null\", \"null\", \"0\", \"null\", \"null\") ";
                    //connection.ExecuteScalar(sql);                     

               // return;
                foreach (var rowTable in list)
                {

                    //string query = $"SELECT *FROM  \"{rowTable}\" ";
                    string query = $"SELECT *FROM  \"ElectronicBomInfoBak\"";
                    var data = connection.Query(query).ToList();

                    string insertSql = string.Empty;

                    foreach (var item in data)
                    {
                        var ppp = (IDictionary<string, object>)item;

                        var keys = ppp.Select(p => p.Key.ToString()).ToList();
                        var values = ppp.Select(p => p.Value).ToList();                        
                        for (int i = 0; i < values.Count; i++)
                        {
                            if (values[i] != null&&IsDate(values[i].ToString()))
                            {
                                values[i] = $"TO_DATE('{values[i]}','YYYY-MM-DD HH24:MI:SS')";
                            }else if(values[i] !=null)
                            {
                                values[i] = $"{values[i]}";
                            }else if (values[i] == null)
                            {
                                 values[i] = null;
                            }
                        }
                        string keysNames = "\""+ string.Join("\", \"", keys)+ "\"";
                        string valuesNames = "\"" + string.Join("\", \"", values)+ "\"";

                        insertSql = $"INSERT INTO \"ElectronicBomInfoBak\"({keysNames}) VALUES({valuesNames}) ";
                        //insertSql += $"({columnNames}) VALUES ({columnValues})";
                        //// 生成每行数据的VALUES部分
                        //string values = "";
                        //foreach (var property in item.GetType().GetRuntimeProperties())
                        //{
                        //    // 获取属性值
                        //    var value = property.GetValue(item);

                        //    // 处理NULL值
                        //    if (value == null)
                        //    {
                        //        values += "NULL, ";
                        //    }
                        //    else
                        //    {
                        //        // 处理字符串值中的单引号
                        //        var formattedValue = value.ToString().Replace("'", "''");
                        //        values += $"'{formattedValue}', ";
                        //    }
                        //}
                        // 去除最后一个逗号和空格
                        //values = values.TrimEnd(',', ' ');

                        //// 拼接INSERT语句
                        //insertSql += $"({values}), ";
                    }
                    // 去除最后一个逗号和空格
                    insertSql = insertSql.TrimEnd(',', ' ');
                    sqlBuilder.Append(insertSql);
                    // 保存为SQL文件
                    System.IO.File.WriteAllText("", insertSql);
                }

                // 保存为SQL文件
                WriteStringBuilderToFile(sqlBuilder, "");
                }
                catch (Exception e)
                {

                    throw;
                }
            }          

        }
        public bool IsDate(string strDate)
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
        public static void WriteStringBuilderToFile(StringBuilder stringBuilder, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(stringBuilder.ToString());
            }
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
    }
}
