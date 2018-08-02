using System;
using System.Data;

namespace Common.DotNetData
{
    /// <summary>
    /// DataTableHelper
    /// By H.Z.XIN 2017-12-20
    /// Modified:
    ///     2018-07-31 
    /// </summary>
    public class DataTableHelper
    {
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
