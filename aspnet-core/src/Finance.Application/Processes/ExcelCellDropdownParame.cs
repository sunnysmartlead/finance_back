using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class ExcelCellDropdownParame
    {
        /// <summary>
        /// 开始列
        /// </summary>
        public int Firstcol { get; set; }
        /// <summary>
        /// 结束列
        /// </summary>
        public int Lastcol { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string[] Vals { get; set; }
        public ExcelCellDropdownParame(int firstcol, int lastcol, string[] vals) 
        {
            this.Firstcol = firstcol;
            this.Lastcol = lastcol;
            this.Vals = vals;
        }

        /// <summary>
        /// 设置下拉框
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="parame"></param>
        public void SetCellDropdownList(ISheet sheet)
        {
            //設置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, this.Firstcol, this.Lastcol);
            IDataValidation validation = null;
            if (sheet.GetType().Name.Contains("XSSF")) // .xlsx
            {
                XSSFDataValidationHelper helper = new XSSFDataValidationHelper((XSSFSheet)sheet);//获得一个数据验证Helper
                validation = helper.CreateValidation(
                helper.CreateExplicitListConstraint(this.Vals), cellRegions);//创建约束
            }
            else // HSSF .xls
            {
                //設置 下拉框內容
                DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(this.Vals);
                validation = new HSSFDataValidation(cellRegions, constraint);
            }
            validation.CreateErrorBox("输入不合法", "请输入和选择下拉列表中的值。");
            validation.ShowErrorBox = true;
            sheet.AddValidationData(validation);
        }

    }
}
