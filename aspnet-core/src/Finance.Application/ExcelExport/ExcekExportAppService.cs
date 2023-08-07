using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ExcelExport
{
    public class ExcekExportAppService : ApplicationService
    {
        public async Task<FileResult> DownloadFile(int num)
        {

            //创建Workbook
            XSSFWorkbook workbook = new XSSFWorkbook();
            //创建一个sheet
            workbook.CreateSheet("电子bom模板");


            ISheet sheet = workbook.GetSheetAt(0);//获取sheet

            //创建头部
            IRow row001 = sheet.CreateRow(0);
            row001.CreateCell(0).SetCellValue("物料大类");
            row001.CreateCell(1).SetCellValue("物料种类");
            row001.CreateCell(2).SetCellValue("是否涉及（必填）");
            row001.CreateCell(3).SetCellValue("物料编号（SAP）");
            row001.CreateCell(4).SetCellValue("材料名称");
            row001.CreateCell(5).SetCellValue("装配数量");
            row001.CreateCell(6).SetCellValue("封装（需要体现PAD的数量）");

            IRow row002 = sheet.CreateRow(1);
            row002.CreateCell(0).SetCellValue("板部件");
            row002.CreateCell(1).SetCellValue("");
            row002.CreateCell(2).SetCellValue("下拉选择“是”or“否”");
            row002.CreateCell(3).SetCellValue("手工录入");
            row002.CreateCell(4).SetCellValue("手工录入");
            row002.CreateCell(5).SetCellValue("手工录入");
            row002.CreateCell(6).SetCellValue("手工录入");

            if (null==num||num<1) {
                num=1;
            }

            for (int n=0;n<num;n++) {

                IRow row003 = sheet.CreateRow(22*n+2);
                row003.CreateCell(0).SetCellValue("PCB1/Sensor板(①如果是多块板，各PCB分别填写;②如果不包含该器件，选择不涉及;③如果同时涉及多个同类型器件，需要分行写出;）");
                sheet.AddMergedRegion(new CellRangeAddress(22*n+2, 22*n+23, 0, 0));

                row003.CreateCell(1).SetCellValue("Sensor芯片");

                IRow row004 = sheet.CreateRow(22*n+3); row004.CreateCell(1).SetCellValue("串行芯片");
                IRow row005 = sheet.CreateRow(22*n+4); row005.CreateCell(1).SetCellValue("芯片IC——电源芯片");
                IRow row006 = sheet.CreateRow(22*n+5); row006.CreateCell(1).SetCellValue("芯片IC——EEPROM/Flsah");
                IRow row007 = sheet.CreateRow(22*n+6); row007.CreateCell(1).SetCellValue("芯片IC——ISP");
                IRow row008 = sheet.CreateRow(22*n+7); row008.CreateCell(1).SetCellValue("芯片IC——门器件");
                IRow row009 = sheet.CreateRow(22*n+8); row009.CreateCell(1).SetCellValue("芯片IC——复位IC或延时IC");
                IRow row010 = sheet.CreateRow(22*n+9); row010.CreateCell(1).SetCellValue("芯片IC——其他传感器");
                IRow row011 = sheet.CreateRow(22*n+10); row011.CreateCell(1).SetCellValue("芯片IC——LED Driver IC");
                IRow row012 = sheet.CreateRow(22*n+11); row012.CreateCell(1).SetCellValue("其他IC——MCU/转换芯片/麦克风等");
                IRow row013 = sheet.CreateRow(22*n+12); row013.CreateCell(1).SetCellValue("LED/VCSEL");
                IRow row014 = sheet.CreateRow(22*n+13); row014.CreateCell(1).SetCellValue("二极管/三极管/MOS");
                IRow row015 = sheet.CreateRow(22*n+14); row015.CreateCell(1).SetCellValue("晶振");
                IRow row016 = sheet.CreateRow(22*n+15); row016.CreateCell(1).SetCellValue("电阻");
                IRow row017 = sheet.CreateRow(22*n+16); row017.CreateCell(1).SetCellValue("电容");
                IRow row018 = sheet.CreateRow(22*n+17); row018.CreateCell(1).SetCellValue("电感");
                IRow row019 = sheet.CreateRow(22*n+18); row019.CreateCell(1).SetCellValue("磁珠");
                IRow row020 = sheet.CreateRow(22*n+19); row020.CreateCell(1).SetCellValue("其他零件（金属弹片）");
                IRow row021 = sheet.CreateRow(22*n+20); row021.CreateCell(1).SetCellValue("BTB or ZIF连接器");
                IRow row022 = sheet.CreateRow(22*n+21); row022.CreateCell(1).SetCellValue("PIN针连接器/座");
                IRow row023 = sheet.CreateRow(22*n+22); row023.CreateCell(1).SetCellValue("LVDS连接器");
                IRow row024 = sheet.CreateRow(22*n+23); row024.CreateCell(1).SetCellValue("线路板（尺寸、叠构等）");

            }

            





            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper((XSSFSheet)sheet);
            //生成下拉框内容 
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateExplicitListConstraint(new String[] { "是", "否" });
            //只对（0，0）单元格有效
            CellRangeAddressList addressList = new CellRangeAddressList(2, 22*num+1, 2, 2); ;
            //绑定下拉框和作用区域  
            XSSFDataValidation validation = validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);
            sheet.AddValidationData(validation);



            //创建头部样式和列宽度
            XSSFCellStyle titleStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center; // 居中
            IFont titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            titleFont.FontHeightInPoints = 12;
            titleFont.Color = HSSFColor.Black.Index;//设置字体颜色
            titleStyle.SetFont(titleFont);
            sheet.GetRow(0).GetCell(0).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(1).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(2).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(3).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(4).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(5).CellStyle = titleStyle;
            sheet.GetRow(0).GetCell(6).CellStyle = titleStyle;

            XSSFCellStyle row02Style = (XSSFCellStyle)workbook.CreateCellStyle();
            row02Style.Alignment = HorizontalAlignment.Center; // 居中
            row02Style.SetFont(titleFont);
            //填充模式
            row02Style.FillPattern = FillPattern.SolidForeground;
            //创建颜色
            XSSFColor xssfcolor = new XSSFColor();
            //rbg值
            byte[] rgb = { 198, 223, 183 };
            //写入rgb
            xssfcolor.SetRgb(rgb);
            //设置颜色值
            row02Style.SetFillForegroundColor(xssfcolor);

            sheet.GetRow(1).GetCell(0).CellStyle = titleStyle;
            sheet.GetRow(1).GetCell(1).CellStyle = titleStyle;
            sheet.GetRow(1).GetCell(2).CellStyle = row02Style;
            sheet.GetRow(1).GetCell(3).CellStyle = row02Style;
            sheet.GetRow(1).GetCell(4).CellStyle = row02Style;
            sheet.GetRow(1).GetCell(5).CellStyle = row02Style;
            sheet.GetRow(1).GetCell(6).CellStyle = row02Style;
            sheet.SetColumnWidth(0, 4000);
            sheet.SetColumnWidth(1, 8500);
            sheet.SetColumnWidth(2, 6500);
            sheet.SetColumnWidth(3, 5000);
            sheet.SetColumnWidth(4, 5000);
            sheet.SetColumnWidth(5, 5000);
            sheet.SetColumnWidth(6, 8000);
            
            //创建物料种类样式
            ICellStyle wlStyle = workbook.CreateCellStyle();
            wlStyle.Alignment = HorizontalAlignment.Left;//水平靠左
            wlStyle.VerticalAlignment = VerticalAlignment.Center; ;//垂直居中
            wlStyle.SetFont(titleFont);
            for (int i= 2; i<22*num+2; i++)
            {
                sheet.GetRow(i).GetCell(1).CellStyle = wlStyle;
            }
            

            ICellStyle bigCellStyle = workbook.CreateCellStyle();
            bigCellStyle.WrapText = true;//自动换行
            bigCellStyle.Alignment = HorizontalAlignment.Center;
            bigCellStyle.VerticalAlignment = VerticalAlignment.Center; ;//垂直居中  
            bigCellStyle.SetFont(titleFont);
            for (int i = 0; i<num; i++)
            {
                sheet.GetRow(22*i+2).GetCell(0).CellStyle = bigCellStyle;
            }

           

            

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);

            Byte[] btye2 = ms.ToArray();
            FileContentResult fileContent = new FileContentResult(btye2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "aaa.xlsx" };

            return fileContent;
        }

        

    }
}

