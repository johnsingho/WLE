﻿using WarehouseLaborEfficiencyWeb.Database;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.DotNetCode;
using System.Data;
using Common.DotNetData;
using WarehouseLaborEfficiencyBLL;
using Common.DotNetJson;
using MyDBQuery.common;
using MySql.Data.MySqlClient;

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
            using (var context = new WarehouseLaborEffEntities())
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
            using (var context = new WarehouseLaborEffEntities())
            {
                var qry = (from c in context.v_tbl_weekdata
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
            using (var context = new WarehouseLaborEffEntities())
            {
                var qry = (from c in context.v_tbl_monthdata
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
            var sql = string.Format(@"select `Date`
                                              ,`HC_FCST`
                                              ,`HC_Actual`
                                              ,`HC_Support`
                                              ,`HC_Utilization` as `HC_Utilization(%)`
                                              ,`Case_ID_in`
                                              ,`Case_ID_out`
                                              ,`Pallet_In`
                                              ,`Pallet_Out`
                                              ,`Jobs_Rec`
                                              ,`Jobs_Issue`
                                              ,`Reel_ID_Rec` 
                                      from V_Tbl_WeekData
                                      where Warehouse= @Warehouse and ( @StartDate<=`Date` and `Date`<= @EndDate)
                                      order by `Date`
                                    "
                                    );
            var parameter = new MySqlParameter[]
            {
                    new MySqlParameter("@Warehouse", bu),
                    new MySqlParameter("@StartDate", startDate),
                    new MySqlParameter("@EndDate", endDate)
            };
            var ds = MySqlClientHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
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

        
        internal static TDatatables GetMonthData(string selKind)
        {
            var res = new TDatatables();
            if (string.IsNullOrEmpty(selKind))
            {
                return res;
            }

            var sql = string.Format(@"select cast(`Date` as char(10)) as `Date`
                                              ,`Warehouse`
                                              ,{0}
                                      from V_Tbl_MonthData
                                      order by `Date`,Warehouse
                                    "
                                    , selKind
                                    );
            MySqlParameter[] parameter = null;
            var ds = MySqlClientHelper.ExecuteQuery(CustomConfig.ConnStrMain, sql, parameter);
            if (DataTableHelper.IsEmptyDataSet(ds))
            {
                return res;
            }
            var dt = DataTableHelper.GetDataTable0(ds);

            var qry = dt.AsEnumerable().Select(x=>new
            {
                Date = x["Date"].ToString().Substring(0,7), /*yyyy-MM*/
                Warehouse = x["Warehouse"].ToString(),
                Item = x[selKind].ToString()
            });
            //var lstRows = GetKinds();

            var kinds = qry.Select(x => x.Warehouse)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            var cols = qry.Select(x => x.Date)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(y=>new TColEntry(y, y)).ToList();

            var qry2 = from x in qry
                       group x by x.Warehouse into g
                       select new
                       {
                           name = g.Key,
                           items = g.Select(x =>x.Item).ToList()
                       };
                        
            res.data = qry2.ToList();
            res.columns = cols;
            res.kinds = kinds;
            return res;
        }

        #region HCData
        private static List<string> GetHCRows()
        {
            var lst = new List<string>();
            lst.Add("System_Clerk");
            lst.Add("Receiving");
            lst.Add("Shipping");
            lst.Add("RTV_Scrap");
            lst.Add("Inventory_Control");
            lst.Add("Overall");
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

            using (var context = new WarehouseLaborEffEntities())
            {
                var qry = (from c in context.v_tbl_hcdata
                           where 0 == string.Compare(c.Warehouse, bu, true)
                           orderby c.Date
                           select c
                           ).ToList().Select(c => new
                           {
                               Date = DateTimeHelper.GetLocalDateStr(c.Date).Substring(0, 7),/*yyyy-MM*/
                               c.System_Clerk,
                               c.Receiving,
                               c.Shipping,
                               c.RTV_Scrap,
                               c.Inventory_Control,
                               c.Overall,
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