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
            return GetSQLString(value);
        }
        public static string ToSQLType(this object value)
        {           
            if (value == null)
            {
                return "null";
            }
            Type type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int64:
                case TypeCode.Int16:

                    return $"{value}";
                case TypeCode.DateTime:
                    return $"TO_DATE('{value.ToString()}', 'YYYY-MM-DD HH24:MI:SS')";
                case TypeCode.String:                    
                    return $"\'{value.ToString()}\'";
                case TypeCode.DBNull: return "null";
                default:
                    return $"\'{value.ToString()}\'";
            }
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
            Type type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int64:
                case TypeCode.Int16:

                    return $"{value}";
                case TypeCode.DateTime:
                    return $"TO_DATE('{value.ToString()}', 'YYYY-MM-DD HH24:MI:SS')";
                case TypeCode.String:
                    return $"\'{value.ToString()}\'";
                case TypeCode.DBNull: return "null";
                default:
                    return $"\'{value.ToString()}\'";
            }
        }
        private static string GetSQLString(object value, Type type)
        {
            if (value == null)
            {
                return "null";
            }                   
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int64:
                case TypeCode.Int16:
                
                    return $"{value}";
                case TypeCode.DateTime:
                    return $"TO_DATE('{value.ToString()}', 'YYYY-MM-DD HH24:MI:SS')";
                case TypeCode.String:
                    return $"\'{value.ToString()}\'";
                case TypeCode.DBNull: return "null";
                default:
                    return $"\'{value.ToString()}\'";
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
