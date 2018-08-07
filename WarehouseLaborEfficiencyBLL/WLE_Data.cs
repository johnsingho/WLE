using Common.DotNetData;
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
        public static long ImportWeekData(FileInfo xlsxFile, out string sErr)
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
                return nWritten;
            }
        }


        public static byte[] GetWeekData_Down(string bu, string startWeek, string endWeek)
        {
            byte[] bys = null;
            var sql = string.Format(@"select 
                                          cast([Date] as char(10)) as [Date]
                                          ,[Warehouse]
                                          ,[HC_FCST]
                                          ,[HC_Actual]
                                          ,[HC_Support]
                                          , ([HC_Utilization] /100.0) as HC_Utilization
                                          ,[Case_ID_in]
                                          ,[Case_ID_out]
                                          ,[Pallet_In]
                                          ,[Pallet_Out]
                                          ,[Jobs_Rec]
                                          ,[Jobs_Issue]
                                          ,[Reel_ID_Rec]
                                      from V_Tbl_WeekData
                                      where Warehouse= @Warehouse and ( @StartDate<=[Date] and [Date]<= @EndDate)
                                      order by [Date]
                                    "
                                    );
            var parameter = new SqlParameter[]
            {
                    new SqlParameter("@Warehouse", bu),
                    new SqlParameter("@StartDate", startWeek),
                    new SqlParameter("@EndDate", endWeek)
            };
            var ds = SqlServerHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return bys;
            }
            var dt = DataTableHelper.GetDataTable0(ds);
            bys = NPOIExcelHelper.BuilderExcel(dt);
            return bys;
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
        public static long ImportMonthData(FileInfo xlsxFile, out string sErr)
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
                return nWritten;
            }
        }


        public static byte[] GetMonthData_Down(string bu, string startWeek, string endWeek)
        {
            byte[] bys = null;
            var sql = string.Format(@"select 
                                     cast([Date] as char(10)) as [Date]
                                          ,[Warehouse]
                                          ,[HC_FCST]
                                          ,[HC_Actual]
                                          ,[HC_Support]
                                          , ([HC_Utilization] /100.0) as HC_Utilization
                                          ,[Case_ID_in]
                                          ,[Case_ID_out]
                                          ,[Pallet_In]
                                          ,[Pallet_Out]
                                          ,[Jobs_Rec]
                                          ,[Jobs_Issue]
                                          ,[Reel_ID_Rec]
                                      from V_Tbl_MonthData
                                      where Warehouse= @Warehouse and ( @StartDate<=[Date] and [Date]<= @EndDate)
                                      order by [Date]
                                    "
                                    );
            var parameter = new SqlParameter[]
            {
                    new SqlParameter("@Warehouse", bu),
                    new SqlParameter("@StartDate", startWeek),
                    new SqlParameter("@EndDate", endWeek)
            };
            var ds = SqlServerHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return bys;
            }
            var dt = DataTableHelper.GetDataTable0(ds);
            bys = NPOIExcelHelper.BuilderExcel(dt);
            return bys;
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
        
        public static long ImportHCData(FileInfo xlsxFile, out string sErr)
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
                return nWritten;
            }
        }

        public static byte[] GetHCData_Down(string bus)
        {
            byte[] bys = null;
            SqlParameter[] parameter = null;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(bus) || 0 == string.Compare("all", bus, true))
            {
                sql = string.Format(@"SELECT 
                                          cast([Date] as char(10)) as [Date]
                                          ,[Warehouse]
                                          ,[Overall]
                                          ,[System_Clerk]
                                          ,[Inventory_Control]
                                          ,[RTV_Scrap]
                                          ,[Receiving]
                                          ,[Shipping]
                                          ,[Forklift_Driver]
                                          ,[Total]
                                    FROM [dbo].[V_Tbl_HCData]
                                    order by Date
                                    "
                                    );
            }
            else
            {
                var whList = bus.Split(new char[] { ',' });
                var sb = new StringBuilder();
                foreach(var bu in whList)
                {
                    sb.AppendFormat("'{0}',", bu);
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sql = string.Format(@"SELECT 
                                        cast([Date] as char(10)) as [Date]
                                          ,[Warehouse]
                                          ,[Overall]
                                          ,[System_Clerk]
                                          ,[Inventory_Control]
                                          ,[RTV_Scrap]
                                          ,[Receiving]
                                          ,[Shipping]
                                          ,[Forklift_Driver]
                                          ,[Total]
                                    FROM [dbo].[V_Tbl_HCData]
                                    where Warehouse in ({0})
                                    order by Date
                                    ",
                                    sb.ToString()
                                    );
            }
            
            var ds = SqlServerHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return bys;
            }
            var dt = DataTableHelper.GetDataTable0(ds);
            bys = NPOIExcelHelper.BuilderExcel(dt);
            return bys;
        }
        #endregion

    }
}
