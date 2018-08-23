using System;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Data;
using System.IO;
using Common.DotNetData;

namespace MyDBQuery.common
{
    /// <summary>
    /// MySqlClientHelper 
    /// MySql 简单访问类
    /// By H.Z.XIN
    /// Modified:
    ///     2018-03-06 添加存储过程执行功能
    ///     2018-08-01 change BulkToDB
    ///     2018-08-02 重写
    ///     2018-08-23 整理
    /// 
    /// </summary>
    public class MySqlClientHelper
    {
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();

            //Create a connection
            using (var connection = new MySqlConnection(connectionString))
            {
                //Prepare the command
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static int ExecuteNonQuery(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        
        //=============================================
        public static DataSet ExecuteQuery(MySqlConnection conn, string sSql)
        {
            DataSet ds = new DataSet();
            try
            {
                var command = new MySqlDataAdapter(sSql, conn);
                command.Fill(ds, "ds");
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }
        public static DataSet ExecuteQuery(MySqlConnection conn, string sSql, params MySqlParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, conn, null, CommandType.Text, sSql, cmdParms);
            using (var da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
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
        public static DataSet ExecuteQuery(string sConn, string sSql, params MySqlParameter[] cmdParms)
        {
            using (var conn = new MySqlConnection(sConn))
            {
                var cmd = new MySqlCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, sSql, cmdParms);
                using (var da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    return ds;
                }
            }
        }

        public static MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();
            var conn = new MySqlConnection(connectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch (Exception ex)
            {
                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        public static MySqlDataReader ExecuteReader(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            //Create the command and connection
            var cmd = new MySqlCommand();
            //Prepare the command to execute
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

            //Execute the query, stating that the connection should close when the resulting datareader has been read
            var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();
            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        public static object ExecuteScalar(MySqlConnection connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        #region 内部
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (MySqlParameter parm in commandParameters)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

        public static int BulkToDB(string constring, DataTable dt, string tarTble, out string sErr)
        {
            sErr = string.Empty;
            if (DataTableHelper.IsEmptyDataTable(dt)) { return 0; }
            int nIns = 0;
            string tmpPath = Path.GetTempFileName();
            string csv = DataTableHelper.DataTableToCsv(dt);
            try
            {
                File.WriteAllText(tmpPath, csv);
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                return 0;
            }

            using(var conn = new MySqlConnection(constring))
            {
                MySqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = tarTble,
                    };
                    bulk.Columns.AddRange(dt.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToArray());
                    nIns = bulk.Load();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    if (tran != null)
                    {
                        tran.Rollback();
                    }
                    sErr = ex.Message;
                }
            }

            try
            {
                File.Delete(tmpPath);
            }
            catch { }            
            return nIns;
        }


    }
}
