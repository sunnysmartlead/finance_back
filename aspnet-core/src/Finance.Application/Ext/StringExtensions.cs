using NPOI.SS.Formula.Functions;
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

        /// <summary>
        /// 根据界面名称获取标题
        /// </summary>
        /// <returns></returns>
        public static string GetTitle(this string projectName)
        {
            switch (projectName)
            {
                case "开始": return "开始";
                case "核价需求录入": return "需求录入";
                case "核价审批录入": return "核价需求审批 & 方案录入";
                case "TR审核": return "主方案审核";
                case "上传电子BOM": return "上传电子BOM";
                case "电子BOM审核": return "电子BOM审核";
                case "上传结构BOM": return "上传结构BOM";
                case "结构BOM审核": return "结构BOM审核";
                case "电子BOM匹配修改": return "电子单价录入";
                case "电子BOM单价审核": return "电子单价审核";
                case "定制结构件": return "结构单价录入";
                case "结构BOM匹配修改": return "结构单价录入";
                case "结构BOM单价审核": return "结构单价审核";
                case "BOM成本审核": return "BOM成本审核";
                case "查看每个方案初版BOM成本": return "查看初版BOM成本";
                case "工序工时添加": return "工序工时添加";
                case "COB制造成本录入": return "COB / 其他制造成本录入";
                case "物流成本录入": return "物流成本录入";
                case "NRE模具费录入": return "模具费用";
                case "模具费审核": return "模具费审核";
                case "NRE_可靠性实验费录入": return "可靠性实验费";
                case "NRE_可靠性实验费审核": return "可靠性实验费审核";
                case "NRE_EMC实验费录入": return "EMC实验费";
                case "NRE_EMC实验费审核": return "EMC实验费审核";
                case "NRE手板件": return "手板件 / 差旅费 / 其他费用";
                case "贸易合规": return "贸易合规判定";
                case "不合规是否退回": return "贸易不合规判定";
                case "核价看板": return "核价看板";
                case "项目部课长审核": return "项目部课长审核";
                case "财务审核": return "财务审核";
                case "项目部长查看核价表": return "项目部长查看核价表";
                case "核心器件成本NRE费用拆分": return "核心器件成本、NRE费用拆分";
                case "选择是否报价": return "报价分析";
                case "审批报价策略与核价表": return "总经理审批";
                case "报价单": return "报价单";
                case "报价审批表": return "报价审批表";
                case "报价反馈": return "报价反馈";
                case "确认中标金额": return "财务中标确认";
                case "总经理查看中标金额": return "总经理中标查看";
                case "归档": return "核价表归档、报价审批表、报价单归档";

                default: return string.Empty;
            }
        }

        /// <summary>
        /// 根据界面名称获取标题
        /// </summary>
        /// <returns></returns>
        public static string GetTypeName(this string projectName)
        {
            switch (projectName)
            {
                case "开始": return "核价需求";
                case "核价需求录入": return "核价需求";
                case "核价审批录入": return "核价需求";
                case "TR审核": return "核价需求";

                case "上传电子BOM": return "产品核价";
                case "电子BOM审核": return "产品核价";
                case "上传结构BOM": return "产品核价";
                case "结构BOM审核": return "产品核价";
                case "电子BOM匹配修改": return "产品核价";
                case "电子BOM单价审核": return "产品核价";
                case "定制结构件": return "产品核价";
                case "结构BOM匹配修改": return "产品核价";
                case "结构BOM单价审核": return "产品核价";
                case "BOM成本审核": return "产品核价";
                case "查看每个方案初版BOM成本": return "产品核价";
                case "工序工时添加": return "产品核价";
                case "COB制造成本录入": return "产品核价";
                case "物流成本录入": return "产品核价";

                case "NRE模具费录入": return "NRE核价";
                case "模具费审核": return "NRE核价";
                case "NRE_可靠性实验费录入": return "NRE核价";
                case "NRE_可靠性实验费审核": return "NRE核价";
                case "NRE_EMC实验费录入": return "NRE核价";
                case "NRE_EMC实验费审核": return "NRE核价";
                case "NRE手板件": return "NRE核价";

                case "贸易合规": return "核价审核";
                case "不合规是否退回": return "核价审核";
                case "核价看板": return "核价审核";
                case "项目部课长审核": return "核价审核";
                case "财务审核": return "核价审核";
                case "项目部长查看核价表": return "核价审核";
                case "核心器件成本NRE费用拆分": return "核价审核";

                case "选择是否报价": return "报价审核";
                case "审批报价策略与核价表": return "报价审核";
                case "报价单": return "报价审核";
                case "报价审批表": return "报价审核";
                case "报价反馈": return "报价审核";
                case "确认中标金额": return "报价审核";
                case "总经理查看中标金额": return "报价审核";

                case "归档": return "报价归档";

                default: return string.Empty;

            }
        }

        /// <summary>
        /// 根据界面名称获取排序
        /// </summary>
        /// <returns></returns>
        public static int GetTypeNameSort(this string projectName)
        {
            switch (projectName)
            {
                case "开始": return 1;
                case "核价需求录入": return 2;
                case "核价审批录入": return 3;
                case "TR审核": return 4;

                case "上传电子BOM": return 5;
                case "电子BOM审核": return 6;
                case "上传结构BOM": return 7;
                case "结构BOM审核": return 8;
                case "电子BOM匹配修改": return 9;
                case "电子BOM单价审核": return 10;
                case "定制结构件": return 11;
                case "结构BOM匹配修改": return 12;
                case "结构BOM单价审核": return 13;
                case "BOM成本审核": return 14;
                case "查看每个方案初版BOM成本": return 15;
                case "工序工时添加": return 16;
                case "COB制造成本录入": return 17;
                case "物流成本录入": return 18;

                case "NRE模具费录入": return 19;
                case "模具费审核": return 20;
                case "NRE_可靠性实验费录入": return 21;
                case "NRE_可靠性实验费审核": return 22;
                case "NRE_EMC实验费录入": return 23;
                case "NRE_EMC实验费审核": return 24;
                case "NRE手板件": return 25;

                case "贸易合规": return 26;
                case "不合规是否退回": return 27;
                case "核价看板": return 28;
                case "项目部课长审核": return 29;
                case "财务审核": return 30;
                case "项目部长查看核价表": return 31;
                case "核心器件成本NRE费用拆分": return 32;

                case "选择是否报价": return 33;
                case "审批报价策略与核价表": return 34;
                case "报价单": return 35;
                case "报价审批表": return 36;
                case "报价反馈": return 37;
                case "确认中标金额": return 38;
                case "总经理查看中标金额": return 39;

                case "归档": return 40;

                default: return 999;

            }
        }
    }
}