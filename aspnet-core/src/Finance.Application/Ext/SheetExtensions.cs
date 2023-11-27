using Finance.PriceEval.Dto;
using NPOI.SS.UserModel;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    public static class SheetExtensions
    {
        /// <summary>
        /// 获取BOM成本
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<Material> GetMaterials(this ISheet sheet)
        {
            //在核价表Excel中，BOM成本开始的行
            var startRow = 9;

            //在核价表Excel中，BOM成本结束的行
            int endRow;

            // 遍历行
            for (int rowIndex = startRow - 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
            }
            return null;
        }
    }
}
