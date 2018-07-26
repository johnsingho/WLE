using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Common.DotNetUI
{
    public class EPPExcelHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dateFormat">日期格式,例如:yyyy-MM-dd,yyyy/MM/dd ...</param>
        /// <returns></returns>
        public static byte[] BuilderExcel(DataTable table, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            OfficeOpenXml.ExcelPackage pkg = new ExcelPackage();
            var sheet1 = pkg.Workbook.Worksheets.Add("sheet1");
            if (table == null)
            {
                return null;
            }
            sheet1.Cells[1, 1].LoadFromDataTable(table, true);
            var len = table.Columns.Count;
            for (int i = 0; i < len; i++)
            {
                if (table.Columns[i].DataType == typeof(DateTime))
                {
                    sheet1.Column(i + 1).Style.Numberformat.Format = dateFormat;
                }
            }
            //int colCount = table.Columns.Count;
            //int rowCount = table.Rows.Count;
            //for (int i = 0; i < colCount; i++) {
            //    sheet1.Cells[1, i + 1].Value = table.Columns[i].ColumnName;
            //}
            //if (table.Rows.Count > 0) {
            //    for (int i = 0; i < rowCount; i++) {
            //        for (int j = 0; j < colCount; j++) {
            //            sheet1.Cells[2 + i, j + 1].Value = string.Format("{0}", table.Rows[i][j]);
            //        }
            //    }
            //}
            return pkg.GetAsByteArray();
        }

        public static byte[] BuilderExcel(DbDataReader dtReader, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            OfficeOpenXml.ExcelPackage pkg = new ExcelPackage();
            var sheet1 = pkg.Workbook.Worksheets.Add("sheet1");
            if (dtReader == null)
            {
                return null;
            }
            sheet1.Cells[1, 1].LoadFromDataReader(dtReader, true);
            var len = dtReader.FieldCount;
            for (int i = 0; i < len; i++)
            {
                if (dtReader.GetFieldType(i) == typeof(DateTime))
                {
                    sheet1.Column(i + 1).Style.Numberformat.Format = dateFormat;
                }
            }
            return pkg.GetAsByteArray();
        }


        public static DataTable ReadExcel(System.IO.Stream stream)
        {
            using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets[1];
                int colStart = sheet.Dimension.Start.Column;    //工作区开始列
                int colEnd = sheet.Dimension.End.Column;        //工作区结束列
                int rowStart = sheet.Dimension.Start.Row;       //工作区开始行号
                int rowEnd = sheet.Dimension.End.Row;           //工作区结束行号
                DataTable dt = new DataTable();
                for (int i = colStart; i <= colEnd; i++)
                {
                    var val = string.Format("{0}", sheet.Cells[rowStart, i].Value).Trim();
                    if (val == "") { val = "C" + i; }
                    dt.Columns.Add(val, typeof(string));
                }
                int s = colStart;
                for (int i = rowStart + 1; i <= rowEnd; i++)
                {
                    DataRow rw = dt.NewRow();
                    for (int j = colStart; j <= colEnd; j++)
                    {
                        var val = sheet.Cells[i, j].Value;
                        var fmtID = sheet.Cells[i, j].Style.Numberformat.NumFmtID;//
                        var fmt = sheet.Cells[i, j].Style.Numberformat.Format;
                        var dateFmt = new string[] { "mm-dd-yy", "mm/dd/yyyy", "dd-mmm-yy", "mm/dd/yyyy hh:mm", "dd-mmm-yy", "m/d/yyyy H:mm", "yyyy\\-mm\\-dd" };
                        if ((fmtID == 14 || fmtID == 15 || fmtID == 22) || dateFmt.Contains(fmt) || (fmtID == 173 && fmt.IndexOf("m", StringComparison.InvariantCultureIgnoreCase) > -1 && fmt.IndexOf("y", StringComparison.InvariantCultureIgnoreCase) > -1 && fmt.IndexOf("d", StringComparison.InvariantCultureIgnoreCase) > -1))
                        {
                            val = string.Format("{0:yyyy-MM-dd hh:mm:ss}", sheet.Cells[i, j].GetValue<DateTime>());
                        }
                        rw[j - s] = string.Format("{0}", val).Trim();
                    }
                    dt.Rows.Add(rw);
                }
                return dt;
            }
        }

        public object GetMegerValue(ExcelWorksheet wSheet, int row, int column)
        {
            string range = wSheet.MergedCells[row, column];
            if (range == null)
            {
                return wSheet.Cells[row, column].Value;
            }
            else
            {
                return wSheet.Cells[(new ExcelAddress(range)).Start.Row, (new ExcelAddress(range)).Start.Column].Value;
            }
        }

        public static DataTable ReadExcel(System.IO.FileInfo file)
        {
            using (System.IO.Stream stream = file.OpenRead())
            {
                return ReadExcel(stream);
            }
        }
    }
}
