using System.Data;

namespace Common.Data
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
        public static DataRow GetDataSet_Row0(DataSet ds)
        {
            var dt = ds.Tables[0];
            return dt.Rows[0];
        }

    }
}
