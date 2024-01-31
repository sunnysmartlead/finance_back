using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DbExp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var configurationProvider = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var connectionString = "请选择要连接的数据库： 1、测试数据库；2、正式数据库  （为防错误，请输入汉字）".GetString(p => (p switch
            {
                "测试数据库" => configurationProvider.GetSection("TestDatabase").Value,
                "正式数据库" => configurationProvider.GetSection("Database").Value,
                _ => throw new Exception("输入不正确，请重新输入"),
            }, p switch
            {
                "测试数据库" => "测试数据库",
                "正式数据库" => "正式数据库",
                _ => throw new Exception("输入不正确，请重新输入"),
            }));

            if (string.IsNullOrWhiteSpace(connectionString.Item1))
            {
                throw new Exception("连接字符串不正确，请检查");
            }

            var selectAction = "请选择要执行的操作： 1、备份数据库；2、导入数据库 （为防错误，请输入汉字）".GetString(p => p switch
            {
                "备份数据库" => StringExtensions.ExpDb,
                "导入数据库" => StringExtensions.ImportDb,
                _ => throw new Exception("输入不正确，请重新输入"),
            });
            if (selectAction == StringExtensions.ExpDb)
            {
                connectionString.Item1.Exp(connectionString.Item2);
            }
            else if (selectAction == StringExtensions.ImportDb)
            {
                connectionString.Item1.Import(connectionString.Item2);
            }
        }
    }
}
