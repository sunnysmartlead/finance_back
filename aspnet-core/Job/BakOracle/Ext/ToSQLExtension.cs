using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakOracle.Ext
{
    public static class ToSQLExtension
    {
        public static string ToSQL(this object value)
        {
            if (value == null)
                return string.Empty;

            return GetSQLString(value);
        }
        public static void CombineFile(this List<string> infileName, String outfileName)
        {
            int b;
            int n = infileName.Count;
            FileStream[] fileIn = new FileStream[n];
            using (FileStream fileOut = new FileStream(outfileName, FileMode.Create))
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        fileIn[i] = new FileStream(infileName[i], FileMode.Open);
                        while ((b = fileIn[i].ReadByte()) != -1)
                            fileOut.WriteByte((byte)b);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        fileIn[i].Close();
                    }

                }
            }
        }
        private static string GetSQLString(object value)
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
        private static bool IsDate(string strDate)
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
        private static bool IsNumber(string strDate)
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
