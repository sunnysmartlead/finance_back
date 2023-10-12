using NUglify.JavaScript.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 按照逗号分割，字符串转list
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> StrToList(this string str)
        {
            return str.Split(',').ToList();
        }
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(this string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] result = Compress(inputBytes);
            return Convert.ToBase64String(result);
        }
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(this string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = Decompress(inputBytes);
            return Encoding.Default.GetString(depressBytes);
        }

        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="inputBytes"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="inputBytes"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }

        /// <summary>
        /// 结构料Bom排序
        /// </summary>
        /// <param name="superTypeName"></param>
        /// <returns></returns>
        public static int BomSort(this string superTypeName)
        {
            switch (superTypeName)
            {
                case FinanceConsts.ElectronicName: return 1;
                case FinanceConsts.StructuralName: return 2;
                case FinanceConsts.GlueMaterialName: return 3;
                case FinanceConsts.SMTOutSourceName: return 4;
                case FinanceConsts.PackingMaterialName: return 5;
                default: return 100;
            }
        }
    }
}
