using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Data
{
    //for DbParameter
    public class TParamVal
    {
        //参数名
        public string name { get; set; }
        //参数值
        public object val { get; set; }
        //参数方向
        public ParameterDirection dir { get; set; }
    }

    /// <summary>
    /// SqlServerHelper 
    /// SQL Server 简单访问类
    /// By H.Z.XIN 2017-12-20
    /// Modified:
    ///     2018-03-06 添加存储过程执行功能
    ///     2018-07-31 change BulkToDB
    /// 
    /// </summary>
    public class SqlServerHelper
    {
        public static TParamVal CreateParameter(string sname, object oVal, ParameterDirection pdir = ParameterDirection.Input)
        {
            var p = new TParamVal
            {
                name = sname,
                val = oVal,
                dir = pdir
            };
            return p;
        }

        private static void KeepOpen(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
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
            SqlCommand cmm = new SqlCommand(sSql, conn);
            cmm.CommandType = CommandType.Text;
            return cmm.ExecuteScalar();
        }
        public static object ExecuteScalar(SqlConnection conn, string sSql, params object[] args)
        {
            KeepOpen(conn);
            var cmm = CreateQueryCommand(conn, sSql, args);
            return cmm.ExecuteScalar();
        }

        public static DataTable ExecuteQuery(string sConn, string sSql)
        {
            using (SqlConnection conn = new SqlConnection(sConn))
            {
                conn.Open();
                return ExecuteQuery(conn, sSql, null);
            }
        }

        //public static DataTable ExecuteQuery(SqlConnection conn, string sSql)
        //{
        //    KeepOpen(conn);
        //    SqlCommand cmm = new SqlCommand(sSql, conn);
        //    cmm.CommandType = CommandType.Text;
        //    var dt = new DataTable();
        //    using (var dr = cmm.ExecuteReader())
        //    {
        //        dt.Load(dr);
        //    }
        //    return dt;
        //}
        public static DataTable ExecuteQuery(SqlConnection conn, string sSql, params object[] args)
        {
            KeepOpen(conn);
            var cmm = CreateQueryCommand(conn, sSql, args);
            var dt = new DataTable();
            using (var dr = cmm.ExecuteReader())
            {
                dt.Load(dr);
            }
            return dt;
        }

        //public static void ExecuteCommand(SqlConnection conn, string sSql)
        //{
        //    KeepOpen(conn);
        //    SqlCommand cmm = new SqlCommand(sSql, conn);
        //    cmm.CommandType = CommandType.Text;
        //    cmm.ExecuteNonQuery();
        //}

        public static void ExecuteCommand(SqlConnection conn, string sSql, params object[] args)
        {
            KeepOpen(conn);
            var cmm = CreateQueryCommand(conn, sSql, args);
            cmm.ExecuteNonQuery();
        }

        public static void ExecuteSP(SqlConnection conn, string sSql, IList<TParamVal> args)
        {
            KeepOpen(conn);
            var cmm = CreateSPCommand(conn, sSql, args);
            cmm.ExecuteNonQuery();
        }
        public static DataTable ExecuteSPQuery(SqlConnection conn, string sSql, IList<TParamVal> args)
        {
            KeepOpen(conn);
            var cmm = CreateSPCommand(conn, sSql, args);
            var dt = new DataTable();
            using (var dr = cmm.ExecuteReader())
            {
                dt.Load(dr);
            }
            return dt;
        }

        #region Paramters
        private static object MapParameterValue(object value)
        {
            if (value is bool)
                return ((bool)value) ? 1 : 0;
            return value;
        }
        /// <summary>
        ///     Add a parameter to a DB command
        /// </summary>
        private static void AddQueryParam(IDbCommand cmd, object value)
        {
            // Support passed in parameters
            var idbParam = value as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", ParamPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            // Create the parameter
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParamPrefix, cmd.Parameters.Count);

            // Assign the parmeter value
            if (value == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = MapParameterValue(value);

                var t = value.GetType();
                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = (int)value;
                }
                else if (t == typeof(Guid))
                {
                    p.Value = value.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);

                    p.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else
                {
                    p.Value = value;
                }
            }

            // Add to the collection
            cmd.Parameters.Add(p);
        }

        private static readonly string ParamPrefix = "@";
        internal static class ParametersHelper
        {
            public static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
            // Helper to handle named parameters from object properties
            public static string ProcessParams(string sql, object[] args_src, List<object> args_dest)
            {
                return rxParams.Replace(sql, m =>
                {
                    string param = m.Value.Substring(1);
                    object arg_val;
                    int paramIndex;
                    if (int.TryParse(param, out paramIndex))
                    {
                        // 个数不够
                        if (paramIndex < 0 || paramIndex >= args_src.Length)
                            throw new System.ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)",
                                    paramIndex, args_src.Length, sql));
                        arg_val = args_src[paramIndex];
                    }
                    else
                    {
                        // Look for a property on one of the arguments with this name
                        bool found = false;
                        arg_val = null;
                        foreach (var o in args_src)
                        {
                            var pi = o.GetType().GetProperty(param);
                            if (pi != null)
                            {
                                arg_val = pi.GetValue(o, null);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            throw new System.ArgumentException(
                                string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')",
                                                param, sql));
                    }

                    // Expand collections to parameter lists
                    if ((arg_val as System.Collections.IEnumerable) != null &&
                    (arg_val as string) == null &&
                    (arg_val as byte[]) == null)
                    {
                        var sb = new StringBuilder();
                        foreach (var i in arg_val as System.Collections.IEnumerable)
                        {
                            sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                            args_dest.Add(i);
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        args_dest.Add(arg_val);
                        return "@" + (args_dest.Count - 1).ToString();
                    }
                }
                    );
            }
        }
        #endregion
        private static IDbCommand CreateQueryCommand(IDbConnection connection, string sql, params object[] args)
        {
            // Perform named argument replacements
            if (null != args)
            {
                var new_args = new List<object>();
                sql = ParametersHelper.ProcessParams(sql, args, new_args);
                args = new_args.ToArray();
            }

            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            cmd.Transaction = null;
            if (null != args)
            {
                foreach (var item in args)
                {
                    AddQueryParam(cmd, item);
                }
            }
            return cmd;
        }
        private static IDbCommand CreateSPCommand(IDbConnection connection, string sql, IList<TParamVal> args)
        {
            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sql;
            cmd.Transaction = null;
            if (null != args)
            {
                foreach (var item in args)
                {
                    AddSPParam(cmd, item);
                }
            }

            return cmd;
        }


        private static void AddSPParam(IDbCommand cmd, TParamVal item)
        {
            // Create the parameter
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParamPrefix, item.name);
            p.Value = item.val;
            p.Direction = item.dir;

            // Add to the collection
            cmd.Parameters.Add(p);
        }


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
                sErr = ex.Message;
                return 0;
            }
        }
        

    }
}