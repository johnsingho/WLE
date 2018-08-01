using Common.Data;
using Common.DotNetExcel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace WarehouseLaborEfficiencyBLL
{
    public class WLE_Data
    {
        private static readonly string TBL_WEEKDATA = "tbl_WeekData";
        private static readonly string TBL_MONTHDATA = "tbl_MonthData";
        private static readonly string TBL_HCDATA = "tbl_HCData";

        #region Common
        private static void WriteDBLog(SqlConnection conn, string msg)
        {
            var sql = string.Format("insert into task_log values('{0}', getdate())", msg);
            SqlCommand cmm = new SqlCommand(sql, conn);
            cmm.CommandType = CommandType.Text;
            cmm.ExecuteNonQuery();
        }
        #endregion

        #region Week Data
        private static DataTable ReadWeekData(FileInfo xlsxFile)
        {
            var dtNew = NPOIExcelHelper.ReadExcel(xlsxFile);
            if (dtNew == null)
            {
                return null;
            }

            //convert 
            dtNew.Columns.Add("UpdateTime", typeof(DateTime));
            dtNew.Columns["HC FCST"].ColumnName = "HC_FCST";
            dtNew.Columns["HC Actual"].ColumnName = "HC_Actual";
            dtNew.Columns["HC Support"].ColumnName = "HC_Support";
            dtNew.Columns["HC Utilization"].ColumnName = "HC_Utilization";
            dtNew.Columns["Case ID in"].ColumnName = "Case_ID_in";
            dtNew.Columns["Case ID out"].ColumnName = "Case_ID_out";
            dtNew.Columns["Pallet In"].ColumnName = "Pallet_In";
            dtNew.Columns["Pallet Out"].ColumnName = "Pallet_Out";
            dtNew.Columns["Jobs Rec"].ColumnName = "Jobs_Rec";
            dtNew.Columns["Jobs Issue"].ColumnName = "Jobs_Issue";
            dtNew.Columns["Reel ID Rec"].ColumnName = "Reel_ID_Rec";
            
            return dtNew;
        }
        public static bool ImportWeekData(FileInfo xlsxFile, out string sErr)
        {
            sErr = string.Empty;
            DataTable dtRead = ReadWeekData(xlsxFile);
            var timNow = DateTime.Now;
            foreach(DataRow r in dtRead.Rows)
            {
                var sDate = r["Date"];
                r["Date"] = Convert.ToDateTime(sDate); //转换日期
                r["UpdateTime"] = timNow;
            }

            using (var conn = new SqlConnection(CustomConfig.ConnStrMain))
            {
                conn.Open();
                WriteDBLog(conn, "start Upload WeekData");
                var nWritten = SqlServerHelper.BulkToDB(conn, dtRead, TBL_WEEKDATA, out sErr);
                WriteDBLog(conn, string.Format("end Upload WeekData:{0}", nWritten));
                return nWritten > 0;
            }
        }
        #endregion

        #region Month Data
        private static DataTable ReadMonthData(FileInfo xlsxFile)
        {
            var dtNew = NPOIExcelHelper.ReadExcel(xlsxFile);
            if (dtNew == null)
            {
                return null;
            }

            //convert 
            dtNew.Columns.Add("UpdateTime", typeof(DateTime));
            dtNew.Columns["HC FCST"].ColumnName = "HC_FCST";
            dtNew.Columns["HC Actual"].ColumnName = "HC_Actual";
            dtNew.Columns["HC Support"].ColumnName = "HC_Support";
            dtNew.Columns["HC Utilization"].ColumnName = "HC_Utilization";
            dtNew.Columns["Case ID in"].ColumnName = "Case_ID_in";
            dtNew.Columns["Case ID out"].ColumnName = "Case_ID_out";
            dtNew.Columns["Pallet In"].ColumnName = "Pallet_In";
            dtNew.Columns["Pallet Out"].ColumnName = "Pallet_Out";
            dtNew.Columns["Jobs Rec"].ColumnName = "Jobs_Rec";
            dtNew.Columns["Jobs Issue"].ColumnName = "Jobs_Issue";
            dtNew.Columns["Reel ID Rec"].ColumnName = "Reel_ID_Rec";

            return dtNew;
        }
        public static bool ImportMonthData(FileInfo xlsxFile, out string sErr)
        {
            sErr = string.Empty;
            DataTable dtRead = ReadMonthData(xlsxFile);
            var timNow = DateTime.Now;
            foreach (DataRow r in dtRead.Rows)
            {
                var sDate = r["Date"];
                r["Date"] = Convert.ToDateTime(sDate); //转换日期
                r["UpdateTime"] = timNow;
            }

            using (var conn = new SqlConnection(CustomConfig.ConnStrMain))
            {
                conn.Open();
                WriteDBLog(conn, "start Upload MonthData");
                var nWritten = SqlServerHelper.BulkToDB(conn, dtRead, TBL_MONTHDATA, out sErr);
                WriteDBLog(conn, string.Format("end Upload MonthData:{0}", nWritten));
                return nWritten > 0;
            }
        }
        #endregion

        #region HCData
        private static DataTable ReadHCData(FileInfo xlsxFile)
        {
            var dtNew = NPOIExcelHelper.ReadExcel(xlsxFile);
            if (dtNew == null)
            {
                return null;
            }

            //convert 
            dtNew.Columns.Add("UpdateTime", typeof(DateTime));
            dtNew.Columns["Warehouse"].ColumnName = "Warehouse";
            dtNew.Columns["System Clerk"].ColumnName = "System_Clerk";
            dtNew.Columns["Inventory Control"].ColumnName = "Inventory_Control";
            dtNew.Columns["RTV & Scrap"].ColumnName = "RTV_Scrap";
            dtNew.Columns["Receiving"].ColumnName = "Receiving";
            dtNew.Columns["Shipping"].ColumnName = "Shipping";
            dtNew.Columns["Forklift Driver"].ColumnName = "Forklift_Driver";
            dtNew.Columns["Total"].ColumnName = "Total";

            return dtNew;
        }
        public static bool ImportHCData(FileInfo xlsxFile, out string sErr)
        {
            sErr = string.Empty;
            DataTable dtRead = ReadHCData(xlsxFile);
            var timNow = DateTime.Now;
            foreach (DataRow r in dtRead.Rows)
            {
                var sDate = r["Date"];
                r["Date"] = Convert.ToDateTime(sDate); //转换日期
                r["UpdateTime"] = timNow;
            }

            using (var conn = new SqlConnection(CustomConfig.ConnStrMain))
            {
                conn.Open();
                WriteDBLog(conn, "start Upload HCData");
                var nWritten = SqlServerHelper.BulkToDB(conn, dtRead, TBL_HCDATA, out sErr);
                WriteDBLog(conn, string.Format("end Upload HCData:{0}", nWritten));
                return nWritten > 0;
            }
        }
        #endregion

    }
}
