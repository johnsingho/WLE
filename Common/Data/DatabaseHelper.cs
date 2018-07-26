using Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class DatabaseHelper
    {
        //TODO 如果有部分不符合约束条件的话，会导致整个插入失败。
        public static bool ImportDatatable(DbConnection conn, DataTable dt, string sTabName, out string serr)
        {
            serr = string.Empty;
            bool bRet = false;
            var sqlConn = conn as SqlConnection;
            var tran = sqlConn.BeginTransaction();
            try
            {
                using (var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, tran))
                {
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.DestinationTableName = sTabName;
                    //if (columnMappings != null)
                    //{
                    //    foreach (var col in columnMappings)
                    //    {
                    //        bulkCopy.ColumnMappings.Add(col.Value, col.Key);
                    //    }
                    //}
                    bulkCopy.WriteToServer(dt);
                }
                tran.Commit();
                bRet = true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                LogHelper.WriteError(conn.GetType(), ex);
                serr = "部分课程编号有重复，无法导入";
            }
            return bRet;
        }
    }
}
