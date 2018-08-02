using System;
using System.Data;
using System.Data.SqlClient;

namespace Common.DotNetData
{
    /// <summary>
    /// SqlServerHelper 
    /// SQL Server 简单访问类
    /// By H.Z.XIN 2017-12-20
    /// Modified:
    ///     2018-03-06 添加存储过程执行功能
    ///     2018-08-01 change BulkToDB
    ///     2018-08-02 重写
    /// 
    /// </summary>
    public class SqlServerHelper
    {
        private static void KeepOpen(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public static DataSet ExecuteQuery(SqlConnection conn, string sSql)
        {
            KeepOpen(conn);
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter command = new SqlDataAdapter(sSql, conn);
                command.Fill(ds, "ds");
            }
            catch (Exception ex)
            {
                //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                throw;
            }
            return ds;
        }
        public static DataSet ExecuteQuery(SqlConnection conn, string sSql, params SqlParameter[] cmdParms)
        {
            KeepOpen(conn);
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, sSql, cmdParms);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                    throw;
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sSql">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteQuery(string sConn, string sSql, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, sSql, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                        throw;
                    }
                    return ds;
                }
            }
        }

        public static DataSet ExecuteQueryTimeout(SqlConnection conn, string sSql, int nSec)
        {
            KeepOpen(conn);
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter command = new SqlDataAdapter(sSql, conn);
                command.SelectCommand.CommandTimeout = nSec;
                command.Fill(ds, "ds");
            }
            catch (Exception ex)
            {
                //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                throw;
            }
            return ds;
        }

        // 执行SQL语句，返回影响的记录数
        public static int ExecuteNonQuery(string sConn, string sSql)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand(sSql, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string sConn, string sSql, params SqlParameter[] cmdParms)
        {
            using (var connection = new SqlConnection(sConn))
            {
                using (var cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sSql, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (Exception ex)
                    {
                        //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                        throw ex;
                    }
                }
            }
        }

        // 执行SQL语句，带超时
        public static int ExecuteNonQueryTimeOut(string sConn, string sSql, int nSec)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand(sSql, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = nSec;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw;
                    }
                }
            }
        }

        public static object ExecuteScalar(string sConn, string sSql)
        {
            using (var conn = new SqlConnection(sConn))
            {
                return ExecuteScalar(conn, sSql);
            }
        }
        public static object ExecuteScalar(SqlConnection conn, string sSql)
        {
            KeepOpen(conn);
            using (SqlCommand cmm = new SqlCommand(sSql, conn))
            {
                cmm.CommandType = CommandType.Text;
                return cmm.ExecuteScalar();
            }                
        }

        //args可以是IDbDataParameter, 也可以是一般类型
        public static object ExecuteScalar(SqlConnection conn, string sSql, params SqlParameter[] cmdParms)
        {
            KeepOpen(conn);
            using (var cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, conn, null, sSql, cmdParms);
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                    throw;
                }
            }
        }


        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sConn, string sSql, params SqlParameter[] cmdParms)
        {
            using (var connection = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sSql, cmdParms);
                        SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();
                        return myReader;
                    }
                    catch (Exception ex)
                    {
                        //LogManager.GetInstance().ErrorLog("GetSingle", ex);
                        throw;
                    }
                }
            }
        }

        public static int ExecuteSqlTran(string sConn, string sql, SqlParameter[] cmdParms, out string sErr)
        {
            sErr = string.Empty;
            using (SqlConnection conn = new SqlConnection(sConn))
            {
                conn.Open();
                using (SqlCommand com = new SqlCommand())
                {
                    SqlTransaction trans = conn.BeginTransaction();//事务对象
                    com.Connection = conn;//指定连接
                    com.Transaction = trans;//指定事务

                    try
                    {
                        com.CommandText = sql;
                        com.Parameters.Clear();
                        com.Parameters.AddRange(cmdParms);
                        var nRow = com.ExecuteNonQuery();
                        trans.Commit();//如果全部执行完毕.提交
                        return nRow;
                    }
                    catch (Exception ex)
                    {
                        sErr = ex.Message;
                        trans.Rollback();//如果有异常.回滚.
                        return 0;
                    }
                }
            }
        }

        //SqlDataReader 需要手动关闭
        public static SqlDataReader ExecuteSP(string sConn, string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                connection.Open();
                SqlCommand command = PrepareCommandSP(connection, storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return returnReader;
            }
        }

        public DataSet ExecuteSP(string sConn, string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = PrepareCommandSP(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                return dataSet;
            }
        }
        public DataSet ExecuteSP(string sConn, string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(sConn))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = PrepareCommandSP(connection, storedProcName, parameters);
                sqlDA.SelectCommand.CommandTimeout = Times;
                sqlDA.Fill(dataSet, tableName);
                return dataSet;
            }
        }
                

        #region PrepareCommand

        private static void PrepareCommand(SqlCommand cmd,
                                            SqlConnection conn,
                                            SqlTransaction trans,
                                            string cmdText, SqlParameter[] cmdParms
                                            )
        {
            if (conn.State != ConnectionState.Open){
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null){
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput
                        || parameter.Direction == ParameterDirection.Input
                        ) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        private static SqlCommand PrepareCommandSP(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput
                        || parameter.Direction == ParameterDirection.Input
                        ) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }
        #endregion


        public static long BulkToDB(string constring, DataTable dt, string tarTble, out string sErr)
        {
            sErr = string.Empty;
            try
            {
                long nItem = 0;
                //声明SqlBulkCopy ,using释放非托管资源
                using (var sqlBC = new SqlBulkCopy(constring))
                {
                    //一次批量的插入的数据量
                    sqlBC.BatchSize = 3000;
                    //超时则事务回滚
                    sqlBC.BulkCopyTimeout = 180;
                    //设置要批量写入的表
                    sqlBC.DestinationTableName = tarTble;
                    sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, args) =>
                    {
                        nItem = (long)args.RowsCopied;
                    });
                    sqlBC.NotifyAfter = dt.Rows.Count;

                    //自定义的OleDbDataReader和数据库的字段进行对应
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }

                    //批量写入
                    sqlBC.WriteToServer(dt);
                    return nItem;
                }
            }
            catch (System.Exception ex)
            {
                sErr = ex.Message;
                return 0;
            }
        }

        public static long BulkToDB(SqlConnection conn, DataTable dt, string tarTble, out string sErr)
        {
            sErr = string.Empty;
            try
            {
                long nItem = 0;
                //声明SqlBulkCopy ,using释放非托管资源
                using (var sqlBC = new SqlBulkCopy(conn))
                {
                    //一次批量的插入的数据量
                    sqlBC.BatchSize = 3000;
                    //超时则事务回滚
                    sqlBC.BulkCopyTimeout = 180;
                    //设置要批量写入的表
                    sqlBC.DestinationTableName = tarTble;
                    sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, args) =>
                    {
                        nItem = (long)args.RowsCopied;
                    });
                    sqlBC.NotifyAfter = dt.Rows.Count;

                    //自定义的OleDbDataReader和数据库的字段进行对应
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }

                    //批量写入
                    sqlBC.WriteToServer(dt);
                    return nItem;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteError(typeof(SqlServerHelper), ex);
                sErr = ex.Message;
                return 0;
            }
        }
        

    }
}