using System;
using System.Data;
using System.Data.SqlClient;

namespace Common.DotNetData
{
    /// <summary>
    /// SqlServerHelper 
    /// SQL Server �򵥷�����
    /// By H.Z.XIN 2017-12-20
    /// Modified:
    ///     2018-03-06 ��Ӵ洢����ִ�й���
    ///     2018-08-01 change BulkToDB
    ///     2018-08-02 ��д
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="sSql">��ѯ���</param>
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

        // ִ��SQL��䣬����Ӱ��ļ�¼��
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

        // ִ��SQL��䣬����ʱ
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

        //args������IDbDataParameter, Ҳ������һ������
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
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="strSQL">��ѯ���</param>
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
                    SqlTransaction trans = conn.BeginTransaction();//�������
                    com.Connection = conn;//ָ������
                    com.Transaction = trans;//ָ������

                    try
                    {
                        com.CommandText = sql;
                        com.Parameters.Clear();
                        com.Parameters.AddRange(cmdParms);
                        var nRow = com.ExecuteNonQuery();
                        trans.Commit();//���ȫ��ִ�����.�ύ
                        return nRow;
                    }
                    catch (Exception ex)
                    {
                        sErr = ex.Message;
                        trans.Rollback();//������쳣.�ع�.
                        return 0;
                    }
                }
            }
        }

        //SqlDataReader ��Ҫ�ֶ��ر�
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
                    // ���δ����ֵ���������,���������DBNull.Value.
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
                //����SqlBulkCopy ,using�ͷŷ��й���Դ
                using (var sqlBC = new SqlBulkCopy(constring))
                {
                    //һ�������Ĳ����������
                    sqlBC.BatchSize = 3000;
                    //��ʱ������ع�
                    sqlBC.BulkCopyTimeout = 180;
                    //����Ҫ����д��ı�
                    sqlBC.DestinationTableName = tarTble;
                    sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, args) =>
                    {
                        nItem = (long)args.RowsCopied;
                    });
                    sqlBC.NotifyAfter = dt.Rows.Count;

                    //�Զ����OleDbDataReader�����ݿ���ֶν��ж�Ӧ
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }

                    //����д��
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
                //����SqlBulkCopy ,using�ͷŷ��й���Դ
                using (var sqlBC = new SqlBulkCopy(conn))
                {
                    //һ�������Ĳ����������
                    sqlBC.BatchSize = 3000;
                    //��ʱ������ع�
                    sqlBC.BulkCopyTimeout = 180;
                    //����Ҫ����д��ı�
                    sqlBC.DestinationTableName = tarTble;
                    sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, args) =>
                    {
                        nItem = (long)args.RowsCopied;
                    });
                    sqlBC.NotifyAfter = dt.Rows.Count;

                    //�Զ����OleDbDataReader�����ݿ���ֶν��ж�Ӧ
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }

                    //����д��
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