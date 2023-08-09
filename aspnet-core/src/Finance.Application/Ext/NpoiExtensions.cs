using Finance.Authorization.Users;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.Util.ArrayExtensions;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// NPOI的扩展方法
    /// </summary>
    public static class NpoiExtensions
    {

        /// <summary>
        /// 创建Sheet并指定其列
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <param name="titleColumn"></param>
        /// <returns></returns>
        public static ISheet SetSheet(this IWorkbook workbook, string sheetName = "Sheet1", params (string titleName, int titleWidth)[] titleColumn)
        {
            var sheet = workbook.CreateSheet(sheetName);
            var title = sheet.CreateRow(0);
            for (int i = 0; i < titleColumn.Length; i++)
            {
                title.CreateCell(i).SetCellValue(titleColumn[i].titleName);
                sheet.SetColumnWidth(i, titleColumn[i].titleWidth << UserConsts.WidthUnit);
            }
            return sheet;
        }

        /// <summary>
        /// 给Sheet指定数据
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SetData(this ISheet sheet, params string[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < data[i].Length; j++)
                {
                    row.CreateCell(j).SetCellValue(data[i][j]);
                }
            }
        }

        /// <summary>
        /// 设置数据约束（Sheet法）
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="data"></param>
        public static void SetConstraint(this XSSFWorkbook workbook, ISheet sheet, int columnIndex, params string[] data)
        {
            var sheetName = $"{sheet.SheetName}{columnIndex}ColumnConstraint"; ;
            var sheetConstraint = workbook.SetSheet(sheetName, ($"{sheetName}Value", 40));
            sheetConstraint.SetData(data.Select(p => new string[1] { p }).ToArray());
            workbook.SetSheetHidden(workbook.GetSheetIndex(sheetConstraint), true);
            var regions = new CellRangeAddressList(1, 65535, columnIndex, columnIndex);
            var helper = new XSSFDataValidationHelper((XSSFSheet)sheet);
            //创建约束
            var dropDownConstraint = helper.CreateFormulaListConstraint(sheetName + "!$A$2:$A$" + data.Length + 1);
            var dropDownValidation = helper.CreateValidation(dropDownConstraint, regions);
            dropDownValidation.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            dropDownValidation.ShowPromptBox = true;
            sheet.AddValidationData(dropDownValidation);
        }


        /// <summary>
        /// Excel sheet合并，多个单sheet的Excel合并为一个
        /// </summary>
        /// <param name="excels">Excel文件流，以及合并后sheet的名称</param>
        /// <returns>合并后的Excel文件流</returns>
        public static MemoryStream ExcelMerge(params (Stream stream, string sheetName)[] excels)
        {
            //设置文件框架
            var workbook = new XSSFWorkbook();

            //将sheet填入框架中
            foreach (var (stream, sheetName) in excels)
            {
                stream.Position = 0;
                var wb = new XSSFWorkbook(stream);
                var st = wb.GetSheetAt(0);
                st.CopyTo(workbook, sheetName, true, true);
            }

            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            return memoryStream;
        }
    }
}
