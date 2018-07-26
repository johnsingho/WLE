using System.Data;
using System.Data.SqlClient;

namespace Common.Data
{
    public class SqlServerHelper
    {
        public static object ExecuteScalar(string sConn, string sSql)
        {
            using ( var conn = new SqlConnection(sConn))
            {
                return ExecuteScalar(conn, sSql);
            }
        }
        public static object ExecuteScalar(SqlConnection conn, string sSql)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmm = new SqlCommand(sSql, conn);
            cmm.CommandType = CommandType.Text;
            return cmm.ExecuteScalar();
        }

        public static DataTable ExecuteQuery(string sConn, string sSql)
        {
            using (SqlConnection conn = new SqlConnection(sConn))
            {
                conn.Open();
                return ExecuteQuery(conn, sSql);
            }
        }

        public static DataTable ExecuteQuery(SqlConnection conn, string sSql)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmm = new SqlCommand(sSql, conn);
            cmm.CommandType = CommandType.Text;
            var dt = new DataTable();
            using (var dr = cmm.ExecuteReader())
            {
                dt.Load(dr);
            }
            return dt;
        }

        public static void ExecuteCommand(SqlConnection conn, string sSql)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmm = new SqlCommand(sSql, conn);
            cmm.CommandType = CommandType.Text;
            cmm.ExecuteNonQuery();
        }
    }
}