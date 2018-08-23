using System;
using System.Data;
using System.Text;

namespace Common.DotNetData
{
    /// <summary>
    /// DataTableHelper
    /// By H.Z.XIN
    /// Modified:
    ///     2018-08-15 整理
    /// 
    /// </summary>
    public static class DataTableHelper
    {
        public static bool IsEmptyDataTable(DataTable dt)
        {
            return (null == dt || 0 == dt.Rows.Count);
        }
        public static bool IsEmptyDataSet(DataSet ds)
        {
            if (null == ds || 0 == ds.Tables.Count)
            {
                return true;
            }
            return IsEmptyDataTable(ds.Tables[0]);
        }
		public static DataTable GetDataTable0(DataSet ds)
        {
            var dt = ds.Tables[0];
            return dt;
        }
        public static DataRow GetDataSet_Row0(DataSet ds)
        {
            var dt = ds.Tables[0];
            return dt.Rows[0];
        }

        public static string TryGet(DataRow dr, string colName)
        {
            var sRet = string.Empty;
            try
            {
                sRet = dr[colName].ToString().Trim();
            }
            catch (System.Exception ex)
            {
                //LogManager.GetInstance().ErrorLog("DataTableHelper::TryGet", ex);
            }

            return sRet;
        }

        public static void CopyDataRow(DataTable tarDt, DataRow srcDataRow)
        {
            var dr = tarDt.Rows.Add();
            foreach (DataColumn col in tarDt.Columns)
            {
                dr[col.ColumnName] = srcDataRow[col.ColumnName];
            }
        }


        /// <summary>
        ///将DataTable转换为标准的CSV
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>返回标准的CSV</returns>
        public static string DataTableToCsv(DataTable dt)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn colum = dt.Columns[i];
                    if (i != 0) { sb.Append(","); }
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        
        //2018-08-01 DataTable行列转换
        //注意：
        //转换完之后所有值类型都是相同，默认转换为字符串，都没有列名了。
        //5*3 ==> (3+1)*5
        public static DataTable DataTableRotate(DataTable dt, Type dtVal = null)
        {
            DataTable dtNew = new DataTable();
            int nOldRows = dt.Rows.Count;
            int nOldCols = dt.Columns.Count;
            for (int i = 0; i <= nOldRows; i++)
            {
                if (0 == i || null == dtVal)
                {
                    dtNew.Columns.Add();
                }
                else
                {
                    dtNew.Columns.Add(string.Empty, dtVal);
                }
            }
            for (int i = 0; i < nOldCols; i++)
            {
                dtNew.Rows.Add();
                dtNew.Rows[i][0] = dt.Columns[i].ColumnName;
            }
            for (int i = 0; i < nOldCols; i++)
            {
                for (int j = 0; j < nOldRows; j++)
                {
                    dtNew.Rows[i][j + 1] = dt.Rows[j][i];
                }
            }
            return dtNew;
        }

        //原dt的最左列不转换为第一行
        public static DataTable DataTableRotateNoLeft(DataTable dt, Type dtVal = null)
        {
            DataTable dtNew = DataTableRotate(dt, dtVal);
            dtNew.Rows.RemoveAt(0);
            return dtNew;
        }
        
    }
}
