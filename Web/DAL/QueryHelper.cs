using WarehouseLaborEfficiencyWeb.Database;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.DotNetCode;
using System.Data;
using Common.DotNetData;
using WarehouseLaborEfficiencyBLL;
using System.Data.SqlClient;
using Common.DotNetJson;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class QueryHelper
    {
        public class TSelectOpt
        {
            public string id { get; set; }
            public string text { get; set; }
        }
        
        public class TDatatables
        {
            public List<TColEntry> columns { get; set; }
            public object data { get; set; }            
            public List<string> kinds { get; set; }
        }
        public class TColEntry
        {
            public TColEntry(string t, string d)
            {
                title = t;
                data = d;
            }
            public string title { get; set; }
            public string data { get; set; }
        }
        public class TMapDatatables
        {
            public string name { get; set; }
            public TDatatables entry { get; set; }
        }

        //========================================
        
        internal static List<TSelectOpt> GetWarehouseList()
        {
            using (var context = new WarehouseLaborEfficiencyEntities())
            {
                var qry = from c in context.tbl_bu
                          orderby c.bu
                          select new TSelectOpt
                          {
                              id= c.bu,
                              text=c.bu
                          };
                return qry.ToList();
            }
        }
        
        internal static object GetWeekdateList()
        {
            using (var context = new WarehouseLaborEfficiencyEntities())
            {
                var qry = (from c in context.V_Tbl_WeekData
                           group c by c.Date into g
                           orderby g.Key
                           select g.FirstOrDefault()
                          ).ToList().Select(x => new TSelectOpt
                          {
                              id = DateTimeHelper.GetLocalDateStrNull(x.Date),
                              text = DateTimeHelper.GetLocalDateStrNull(x.Date)
                          });
                return qry.ToList();
            }
        }

        internal static List<TSelectOpt> GetMonthdateList()
        {
            using (var context = new WarehouseLaborEfficiencyEntities())
            {
                var qry = (from c in context.V_Tbl_MonthData
                           group c by c.Date into g
                           orderby g.Key
                           select g.FirstOrDefault()
                          ).ToList().Select(x => new TSelectOpt
                          {
                              id = DateTimeHelper.GetLocalDateStrNull(x.Date),
                              text = DateTimeHelper.GetLocalDateStrNull(x.Date)
                          });
                return qry.ToList();
            }
        }
        
        private static List<string> GetKinds()
        {
            var lst = new List<string>();
            lst.Add("HC_FCST");
            lst.Add("HC_Actual");
            lst.Add("HC_Support");
            lst.Add("HC_Utilization(%)");
            lst.Add("Case_ID_in");
            lst.Add("Case_ID_out");
            lst.Add("Pallet_In");
            lst.Add("Pallet_Out");
            lst.Add("Jobs_Rec");
            lst.Add("Jobs_Issue");
            lst.Add("Reel_ID_Rec");
            return lst;
        }

        public static TDatatables GetWeekData(string bu, string startDate, string endDate)
        {
            var res = new TDatatables();
            var sql = string.Format(@"select [Date]
                                              ,[HC_FCST]
                                              ,[HC_Actual]
                                              ,[HC_Support]
                                              ,[HC_Utilization]
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
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate)
            };
            var ds = SqlServerHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return res;
            }
            var dt = DataTableHelper.GetDataTable0(ds);

            var qry = dt.AsEnumerable();
            //var lstRows = GetKinds();

            var cols = qry.Select(x => DateTimeHelper.GetLocalDateStrNull(x["Date"]))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

            var dtNew = DataTableHelper.DataTableRotateNoLeft(dt);
            dtNew.Columns[0].ColumnName = "Week";
            int nColNew = dtNew.Columns.Count;
            for (var i = 0; i < nColNew-1; i++)
            {
                dtNew.Columns[i + 1].ColumnName = cols[i];
            }

            var arrCols = new List<TColEntry>();
            arrCols.Add(new TColEntry("Week", "Week"));
            arrCols.AddRange((from x in cols
                              select new TColEntry(x, x)
                              ).ToList());
            res.columns = arrCols;

            var sData = JsonHelper.DataTableToJsonArr(dtNew);
            res.data = sData;
            res.kinds = GetKinds();
            return res;
        }

        
        internal static TDatatables GetMonthData(string bu, string startDate, string endDate)
        {
            var res = new TDatatables();
            var sql = string.Format(@"select [Date]
                                              ,[HC_FCST]
                                              ,[HC_Actual]
                                              ,[HC_Support]
                                              ,[HC_Utilization]
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
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate)
            };
            var ds = SqlServerHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return res;
            }
            var dt = DataTableHelper.GetDataTable0(ds);

            var qry = dt.AsEnumerable();
            //var lstRows = GetKinds();

            var cols = qry.Select(x => DateTimeHelper.GetLocalDateStrNull(x["Date"]))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

            var dtNew = DataTableHelper.DataTableRotateNoLeft(dt);
            dtNew.Columns[0].ColumnName = "Week";
            int nColNew = dtNew.Columns.Count;
            for (var i = 0; i < nColNew - 1; i++)
            {
                dtNew.Columns[i + 1].ColumnName = cols[i];
            }

            var arrCols = new List<TColEntry>();
            arrCols.Add(new TColEntry("Week", "Week"));
            arrCols.AddRange((from x in cols
                              select new TColEntry(x, x)
                              ).ToList());
            res.columns = arrCols;

            var sData = JsonHelper.DataTableToJsonArr(dtNew);
            res.data = sData;
            res.kinds = GetKinds();
            return res;
        }

        #region HCData
        private static List<string> GetHCRows()
        {
            var lst = new List<string>();
            lst.Add("Overall");
            lst.Add("System_Clerk");
            lst.Add("Inventory_Control");
            lst.Add("RTV_Scrap");
            lst.Add("Receiving");
            lst.Add("Shipping");
            lst.Add("Forklift_Driver");
            lst.Add("Total");            
            return lst;
        }

        public static List<TMapDatatables> GetHCData(string bus)
        {
            var buList = bus.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return GetHCDataAll(buList);
        }

        private static List<TMapDatatables> GetHCDataAll(string[] buList)
        {
            var items = new List<TMapDatatables>();
            if(null==buList || 0 == buList.Length) { return items; }
            foreach(var bu in buList)
            {
                items.Add(new TMapDatatables
                    {
                        name = bu,
                        entry = GetHCDataBu(bu)
                    }
                );
            }
            return items;
        }

        private static TDatatables GetHCDataBu(string bu)
        {
            var res = new TDatatables();
            res.kinds = GetHCRows();

            using (var context = new WarehouseLaborEfficiencyEntities())
            {
                var qry = (from c in context.V_Tbl_HCData
                           where 0==string.Compare(c.Warehouse,bu,true)
                           orderby c.Date
                           select c
                           ).ToList().Select(c=>new
                           {
                               Date=DateTimeHelper.GetLocalDateStr(c.Date.Value),
                               Overall=c.Overall,
                               c.System_Clerk,
                               c.Inventory_Control,
                               c.RTV_Scrap,
                               c.Receiving,
                               c.Shipping,
                               c.Forklift_Driver,
                               c.Total
                           });
                res.data = qry.ToList();
            }
         
            return res;
        }
        #endregion

    }
}