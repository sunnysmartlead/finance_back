using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Ext;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Finance.DemandApplyAudit;

namespace Finance.Tests.Ext
{
    /// <summary>
    /// 扩展方法测试用例
    /// </summary>
    public class Extensions_Test : FinanceTestBase
    {
        [Fact]
        public async Task CostTypeSort_Test()
        {
            (new ManufacturingCost { CostType = CostType.COB }).CostTypeSort().CompareTo(1);
            (new ManufacturingCost { CostType = CostType.SMT }).CostTypeSort().CompareTo(2);
            (new ManufacturingCost { CostType = CostType.GroupTest }).CostTypeSort().CompareTo(3);
            (new ManufacturingCost { CostType = CostType.Other }).CostTypeSort().CompareTo(4);
            (new ManufacturingCost { CostType = CostType.Total }).CostTypeSort().CompareTo(5);
        }

        [Fact]
        public async Task GetValueOrDefault_Test()
        {
            decimal? testA = null;
            decimal? testB = 1;
            decimal? testC = 2;
            testA.GetValueOrDefault().CompareTo(0);
            testB.GetValueOrDefault().CompareTo(1);
            testC.GetValueOrDefault().CompareTo(2);
        }

        /// <summary>
        /// 测试字符串转list方法
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task StrToList_Test()
        {
            var str = "1,2,3,4,5,6,7,8,9,10,11,12";
            var result = str.StrToList();
            result.Count.CompareTo(12);
        }

        /// <summary>
        /// 测试list转字符串方法
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ListToStr_Test()
        {
            var strList = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
            var result = strList.ListToStr();
            result.CompareTo("1234567891011121314151617181920");
        }

        [Fact]
        public async Task Compress_Test()
        {
            var str = "Test string Test string Test string Test string Test string Test string Test string Test string Test string";
            var result = str.Compress();
            (result.Length < str.Length).CompareTo(true);
        }

        [Fact]
        public async Task Decompress_Test()
        {
            var str = "Test string Test string Test string Test string Test string Test string Test string Test string Test string";
            var result = str.Compress();

            var newstr = result.Decompress();
            (str == newstr).CompareTo(true);
        }

        [Fact]
        public async Task BomSort_Test()
        {
            FinanceConsts.ElectronicName.BomSort().CompareTo(1);
            FinanceConsts.StructuralName.BomSort().CompareTo(2);
            FinanceConsts.GlueMaterialName.BomSort().CompareTo(3);
            FinanceConsts.SMTOutSourceName.BomSort().CompareTo(4);
            FinanceConsts.PackingMaterialName.BomSort().CompareTo(5);
            "Test string".BomSort().CompareTo(100);
        }

        [Fact]
        public async Task GetTitle_Test()
        {
            "开始".GetTitle().CompareTo("开始");
            "核价需求录入".GetTitle().CompareTo("需求录入");
            "核价审批录入".GetTitle().CompareTo("核价需求审批 & 方案录入");
            "TR审核".GetTitle().CompareTo("主方案审核");
            "上传电子BOM".GetTitle().CompareTo("上传电子BOM");
            "电子BOM审核".GetTitle().CompareTo("电子BOM审核");
            "上传结构BOM".GetTitle().CompareTo("上传结构BOM");
            "结构BOM审核".GetTitle().CompareTo("结构BOM审核");
            "电子BOM匹配修改".GetTitle().CompareTo("电子单价录入");
            "电子BOM单价审核".GetTitle().CompareTo("电子单价审核");
            "定制结构件".GetTitle().CompareTo("结构单价录入");
            "结构BOM匹配修改".GetTitle().CompareTo("结构单价录入");
            "结构BOM单价审核".GetTitle().CompareTo("结构单价审核");
            "BOM成本审核".GetTitle().CompareTo("BOM成本审核");
            "查看每个方案初版BOM成本".GetTitle().CompareTo("查看初版BOM成本");
            "工序工时添加".GetTitle().CompareTo("工序工时添加");
            "COB制造成本录入".GetTitle().CompareTo("COB / 其他制造成本录入");
            "物流成本录入".GetTitle().CompareTo("物流成本录入");
            "NRE模具费录入".GetTitle().CompareTo("模具费用");
            "模具费审核".GetTitle().CompareTo("模具费审核");
            "NRE_可靠性实验费录入".GetTitle().CompareTo("可靠性实验费");
            "NRE_可靠性实验费审核".GetTitle().CompareTo("可靠性实验费审核");
            "NRE_EMC实验费录入".GetTitle().CompareTo("EMC实验费");
            "NRE_EMC实验费审核".GetTitle().CompareTo("EMC实验费审核");
            "NRE手板件".GetTitle().CompareTo("手板件 / 差旅费 / 其他费用");
            "贸易合规".GetTitle().CompareTo("贸易合规判定");
            "不合规是否退回".GetTitle().CompareTo("贸易不合规判定");
            "核价看板".GetTitle().CompareTo("核价看板");
            "项目部课长审核".GetTitle().CompareTo("项目部课长审核");
            "财务审核".GetTitle().CompareTo("财务审核");
            "项目部长查看核价表".GetTitle().CompareTo("项目部长查看核价表");
            "核心器件成本NRE费用拆分".GetTitle().CompareTo("核心器件成本、NRE费用拆分");
            "选择是否报价".GetTitle().CompareTo("报价分析");
            "审批报价策略与核价表".GetTitle().CompareTo("总经理审批");
            "报价单".GetTitle().CompareTo("报价单");
            "报价审批表".GetTitle().CompareTo("报价审批表");
            "报价反馈".GetTitle().CompareTo("报价反馈");
            "确认中标金额".GetTitle().CompareTo("财务中标确认");
            "总经理查看中标金额".GetTitle().CompareTo("总经理中标查看");
            "归档".GetTitle().CompareTo("核价表归档、报价审批表、报价单归档");
            "Test string".GetTitle().CompareTo("Test string");
        }

        [Fact]
        public async Task GetTypeName_Test()
        {
            "开始".GetTypeName().CompareTo("开始");
            "核价需求录入".GetTypeName().CompareTo("核价需求");
            "核价审批录入".GetTypeName().CompareTo("核价需求");
            "TR审核".GetTypeName().CompareTo("核价需求");
            "上传电子BOM".GetTypeName().CompareTo("产品核价");
            "电子BOM审核".GetTypeName().CompareTo("产品核价");
            "上传结构BOM".GetTypeName().CompareTo("产品核价");
            "结构BOM审核".GetTypeName().CompareTo("产品核价");
            "电子BOM匹配修改".GetTypeName().CompareTo("产品核价");
            "电子BOM单价审核".GetTypeName().CompareTo("产品核价");
            "定制结构件".GetTypeName().CompareTo("产品核价");
            "结构BOM匹配修改".GetTypeName().CompareTo("产品核价");
            "结构BOM单价审核".GetTypeName().CompareTo("产品核价");
            "BOM成本审核".GetTypeName().CompareTo("产品核价");
            "查看每个方案初版BOM成本".GetTypeName().CompareTo("产品核价");
            "工序工时添加".GetTypeName().CompareTo("产品核价");
            "COB制造成本录入".GetTypeName().CompareTo("产品核价");
            "物流成本录入".GetTypeName().CompareTo("产品核价");
            "NRE模具费录入".GetTypeName().CompareTo("NRE核价");
            "模具费审核".GetTypeName().CompareTo("NRE核价");
            "NRE_可靠性实验费录入".GetTypeName().CompareTo("NRE核价");
            "NRE_可靠性实验费审核".GetTypeName().CompareTo("NRE核价");
            "NRE_EMC实验费录入".GetTypeName().CompareTo("NRE核价");
            "NRE_EMC实验费审核".GetTypeName().CompareTo("NRE核价");
            "NRE手板件".GetTypeName().CompareTo("NRE核价");
            "贸易合规".GetTypeName().CompareTo("核价审核");
            "不合规是否退回".GetTypeName().CompareTo("核价审核");
            "核价看板".GetTypeName().CompareTo("核价审核");
            "项目部课长审核".GetTypeName().CompareTo("核价审核");
            "财务审核".GetTypeName().CompareTo("核价审核");
            "项目部长查看核价表".GetTypeName().CompareTo("核价审核");
            "核心器件成本NRE费用拆分".GetTypeName().CompareTo("核价审核");
            "选择是否报价".GetTypeName().CompareTo("报价审核");
            "审批报价策略与核价表".GetTypeName().CompareTo("报价审核");
            "报价单".GetTypeName().CompareTo("报价审核");
            "报价审批表".GetTypeName().CompareTo("报价审核");
            "报价反馈".GetTypeName().CompareTo("报价审核");
            "确认中标金额".GetTypeName().CompareTo("报价审核");
            "总经理查看中标金额".GetTypeName().CompareTo("报价审核");
            "归档".GetTypeName().CompareTo("报价归档");
            "Test string".GetTypeName().CompareTo("Test string");
        }

        [Fact]
        public async Task GetTypeNameSort_Test()
        {
            "开始".GetTypeNameSort().CompareTo(1);
            "核价需求录入".GetTypeNameSort().CompareTo(2);
            "核价审批录入".GetTypeNameSort().CompareTo(3);
            "TR审核".GetTypeNameSort().CompareTo(4);
            "上传电子BOM".GetTypeNameSort().CompareTo(5);
            "电子BOM审核".GetTypeNameSort().CompareTo(6);
            "上传结构BOM".GetTypeNameSort().CompareTo(7);
            "结构BOM审核".GetTypeNameSort().CompareTo(8);
            "电子BOM匹配修改".GetTypeNameSort().CompareTo(9);
            "电子BOM单价审核".GetTypeNameSort().CompareTo(10);
            "定制结构件".GetTypeNameSort().CompareTo(11);
            "结构BOM匹配修改".GetTypeNameSort().CompareTo(12);
            "结构BOM单价审核".GetTypeNameSort().CompareTo(13);
            "BOM成本审核".GetTypeNameSort().CompareTo(14);
            "查看每个方案初版BOM成本".GetTypeNameSort().CompareTo(15);
            "工序工时添加".GetTypeNameSort().CompareTo(16);
            "COB制造成本录入".GetTypeNameSort().CompareTo(17);
            "物流成本录入".GetTypeNameSort().CompareTo(18);
            "NRE模具费录入".GetTypeNameSort().CompareTo(19);
            "模具费审核".GetTypeNameSort().CompareTo(20);
            "NRE_可靠性实验费录入".GetTypeNameSort().CompareTo(21);
            "NRE_可靠性实验费审核".GetTypeNameSort().CompareTo(22);
            "NRE_EMC实验费录入".GetTypeNameSort().CompareTo(23);
            "NRE_EMC实验费审核".GetTypeNameSort().CompareTo(24);
            "NRE手板件".GetTypeNameSort().CompareTo(25);
            "贸易合规".GetTypeNameSort().CompareTo(26);
            "不合规是否退回".GetTypeNameSort().CompareTo(27);
            "核价看板".GetTypeNameSort().CompareTo(28);
            "项目部课长审核".GetTypeNameSort().CompareTo(29);
            "财务审核".GetTypeNameSort().CompareTo(30);
            "项目部长查看核价表".GetTypeNameSort().CompareTo(31);
            "核心器件成本NRE费用拆分".GetTypeNameSort().CompareTo(32);
            "选择是否报价".GetTypeNameSort().CompareTo(33);
            "审批报价策略与核价表".GetTypeNameSort().CompareTo(34);
            "报价单".GetTypeNameSort().CompareTo(35);
            "报价审批表".GetTypeNameSort().CompareTo(36);
            "报价反馈".GetTypeNameSort().CompareTo(37);
            "确认中标金额".GetTypeNameSort().CompareTo(38);
            "总经理查看中标金额".GetTypeNameSort().CompareTo(39);
            "归档".GetTypeNameSort().CompareTo(40);
            "Test string".GetTypeNameSort().CompareTo(99);
        }


        /// <summary>
        /// 测试字符串是否为数据的判断函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task IsNumber_Test() 
        {
            "123".IsNumber().CompareTo(true);
            "47567".IsNumber().CompareTo(true);
            "5".IsNumber().CompareTo(true);
            "7".IsNumber().CompareTo(true);
            "9035".IsNumber().CompareTo(true);
            "0034234".IsNumber().CompareTo(true);
            "3333333".IsNumber().CompareTo(true);
            "0000000".IsNumber().CompareTo(true);
            "0".IsNumber().CompareTo(true);
            "0.000000000000000001".IsNumber().CompareTo(true);
            "0.0000000发1".IsNumber().CompareTo(false);
            "O.1".IsNumber().CompareTo(false);
            "1q".IsNumber().CompareTo(false);
            "2e".IsNumber().CompareTo(false);
            "sfdf".IsNumber().CompareTo(false);
            "放大".IsNumber().CompareTo(false);
            "公馆".IsNumber().CompareTo(false);
        }
    }
}
