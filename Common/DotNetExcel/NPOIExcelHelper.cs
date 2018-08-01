using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System;

namespace Common.DotNetExcel
{
    /// <summary>
    /// NPOIExcelHelper
    /// 使用NPOI来进行excel导入，导出
    /// By H.Z.XIN
    /// Modified:
    ///     2018-03-06 添加存储过程执行功能
    ///     2018-07-01 support formula,date
    /// </summary>
    public class NPOIExcelHelper
    {
        public static byte[] BuilderExcel(DataTable table, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            IWorkbook workbook = new XSSFWorkbook(); //office2007            
            ISheet sheet = workbook.CreateSheet("Sheet1");
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
            foreach (DataColumn column in table.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            }   

            int rowIndex = 1;
            foreach (DataRow row in table.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in table.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }
                rowIndex++;
            }

            byte[] bys = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                memoryStream.Flush();
                bys = memoryStream.ToArray();
            }
            return bys;
        }

        // untest
        public static byte[] BuilderExcel(DbDataReader dtReader, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            IWorkbook workbook = new XSSFWorkbook(); //office2007            
            ISheet sheet = workbook.CreateSheet("Sheet1");
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
            for(var i=0; i<dtReader.FieldCount; i++)
            {
                headerRow.CreateCell(i).SetCellValue(dtReader.GetName(i));
            }

            int rowIndex = 1;
            while (dtReader.Read())
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                for (var i= 0; i < dtReader.FieldCount; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(dtReader[i].ToString());
                }
                rowIndex++;
            }

            byte[] bys = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                memoryStream.Flush();
                bys = memoryStream.ToArray();
            }
            return bys;
        }

        private static object RetrivCellVal(IWorkbook wb, bool bNewExcel, ICell cell)
        {
            object obj = "";
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    {
                        obj = DateUtil.IsCellDateFormatted(cell)
                                    ? cell.DateCellValue.ToString()
                                    : cell.NumericCellValue.ToString();
                    }
                    break;
                case CellType.Formula:
                    {
                        IFormulaEvaluator e = null;
                        if (bNewExcel)
                        {
                            e = new XSSFFormulaEvaluator(wb);
                        }
                        else
                        {
                            e = new HSSFFormulaEvaluator(wb);
                        }
                        var cellVal = e.Evaluate(cell);
                        switch(cellVal.CellType)
                        {
                            case CellType.Numeric:
                                obj = cellVal.NumberValue;
                                break;
                            default:
                                obj = cellVal.StringValue;
                                break;
                        }
                    }
                    break;
                default:
                    {
                        // String
                        obj = cell.StringCellValue;
                    }
                    break;
            }
            return obj;
        }
        public static DataTable ReadExcel(System.IO.Stream stream, bool bNewExcel = true)
        {
            IWorkbook wb;
            if (bNewExcel)
            {
                wb = new XSSFWorkbook(stream); // excel2007
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            }
            else
            {
                wb = new HSSFWorkbook(stream); // excel97
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            }

            // 只取第一个sheet
            ISheet sheet = wb.GetSheetAt(0);
            DataTable dt = new DataTable();

            // 由第一行取标题
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum; // 取列数
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var sColName = headerRow.GetCell(i).StringCellValue.Trim();
                dt.Columns.Add(new DataColumn(sColName));
            }
            
            //TODO 这里改成读列名应该会好点
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell != null)
                    {
                        dataRow[j] = RetrivCellVal(wb, bNewExcel, cell);
                    }
                }

                dt.Rows.Add(dataRow);
            }
            
            return dt;
        }
        public static DataTable ReadExcel(System.IO.FileInfo file)
        {
            using (System.IO.Stream stream = file.OpenRead())
            {
                var bNewExcel = (0==string.Compare(file.Extension,".xlsx", false));
                return ReadExcel(stream, bNewExcel);
            }
        }
    }
}
