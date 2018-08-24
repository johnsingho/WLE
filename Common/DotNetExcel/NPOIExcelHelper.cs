using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System;
using System.Collections.Generic;

namespace Common.DotNetExcel
{
    /// <summary>
    /// NPOIExcelHelper
    /// 使用NPOI来进行excel导入，导出
    /// By H.Z.XIN
    /// Modified:
    ///     2018-03-06 添加存储过程执行功能
    ///     2018-07-01 support formula,date
    ///     2018-08-23 对数值类型进行默认值处理
    /// </summary>
    public class NPOIExcelHelper
    {
        //默认全部当文本来输出
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

        //进行数值类型输出
        public static byte[] BuilderExcelWithDataType(DataTable table, string dateFormat = "yyyy-MM-dd HH:mm:ss")
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
                var bHandle = false;
                foreach (DataColumn column in table.Columns)
                {
                    bHandle = false;
                    var cell = dataRow.CreateCell(column.Ordinal);
                    if (column.DataType==typeof(int) 
                        || column.DataType == typeof(double)
                        || column.DataType == typeof(float)
                        || column.DataType == typeof(decimal)/*这也许会有问题*/
                        )
                    {
                        double dbl = 0.0;
                        try
                        {
                            dbl = Convert.ToDouble(row[column]);                            
                            cell.SetCellType(CellType.Numeric);
                            cell.SetCellValue(dbl);
                            bHandle = true;
                        }
                        catch {
                        }                        
                    }

                    if (!bHandle)
                    {
                        cell.SetCellValue(row[column].ToString());
                    }
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
                        if(DateUtil.IsCellDateFormatted(cell))
                        {
                            obj = cell.DateCellValue;
                        }
                        else
                        {
                            obj = cell.NumericCellValue;
                        }
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

        //TODO 暂时只处理数值类型,字符串
        private static object TryFillCellDefault(ICell cell, object cellVal, CellType cellType)
        {
            object obj = "";
            switch (cellType)
            {
                case CellType.Numeric:
                    {
                        var bok = false;
                        do
                        {
                            if (null == cellVal) { break; }
                            var sVal = cellVal.ToString().Trim();
                            if (!string.IsNullOrEmpty(sVal)) { bok = true; break; }
                        } while (false);
                        obj = bok ? cellVal : 0;
                    }
                    break;                
                default:
                    {
                        // String
                        obj = null!=cellVal ? cellVal : string.Empty;
                    }
                    break;
            }
            return obj;
        }

        private static DataTable DecodeExcel(bool bNewExcel, IWorkbook wb, int iSheet, bool bDefaultFromFirstRow=true)
        {
            ISheet sheet = wb.GetSheetAt(iSheet);
            DataTable dt = new DataTable();

            // 由第一行取标题
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum; // 取列数
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var sColName = headerRow.GetCell(i).StringCellValue.Trim();
                dt.Columns.Add(new DataColumn(sColName));
            }

            var firstRowCellTypes = new Dictionary<int, CellType>(); //缓存第一一行的列格式
            var bFirstRow = true;
            //TODO 这里改成读列名应该会好点
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell == null)
                    {
                        continue;
                    }

                    var cellVal = RetrivCellVal(wb, bNewExcel, cell);                    
                    if (bFirstRow)
                    {
                        firstRowCellTypes.Add(j, cell.CellType);
                    }
                    else if(bDefaultFromFirstRow)
                    {
                        cellVal = TryFillCellDefault(cell, cellVal, firstRowCellTypes[j]);
                    }

                    dataRow[j] = cellVal;
                }

                bFirstRow = false;
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// Excel sheet ---> DataTable
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bNewExcel"></param>
        /// <param name="bDefaultFromFirstRow">是否要根据第一行的类型来推导后续行的数据类型</param>
        /// <returns></returns>
        public static DataTable ReadExcel(System.IO.Stream stream, bool bNewExcel = true, bool bDefaultFromFirstRow = true)
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

            return DecodeExcel(bNewExcel, wb, 0, bDefaultFromFirstRow);//默认取第一页
        }
        public static DataTable ReadExcel(System.IO.FileInfo file, bool bDefaultFromFirstRow)
        {
            using (System.IO.Stream stream = file.OpenRead())
            {
                var bNewExcel = (0==string.Compare(file.Extension,".xlsx", false));
                return ReadExcel(stream, bNewExcel, bDefaultFromFirstRow);
            }
        }
    }
}
