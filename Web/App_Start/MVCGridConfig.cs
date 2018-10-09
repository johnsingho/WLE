[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WarehouseLaborEfficiencyWeb.MVCGridConfig), "RegisterGrids")]

namespace WarehouseLaborEfficiencyWeb
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Linq;
    using System.Collections.Generic;

    using MVCGrid.Models;
    using MVCGrid.Web;
    using Database;
    using Common.DotNetCode;

    public static class MVCGridConfig 
    {
        public static void RegisterGrids()
        {
            RegisterWeekData();
            RegisterMonthData();
            RegisterHCData();
        }

        private static void RegisterWeekData()
        {
            MVCGridDefinitionTable.Add("WeekDataGrid", new MVCGridBuilder<tbl_weekdata>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithPaging(true, 20)
                .WithAdditionalQueryOptionNames("Search")
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add("Delete").WithHtmlEncoding(false)
                    .WithSorting(false)
                    .WithHeaderText(" ")
                    .WithValueExpression((p, c) => p.id.ToString())
                    .WithValueTemplate("<button class='btn btn-danger btn-sm' type='button' role='button' onclick='DoDelete({Value})'>Delete</button>");

                    cols.Add("id").WithValueExpression(p => p.id.ToString());
                    cols.Add("Date").WithValueExpression(p => DateTimeHelper.GetLocalDateStr(p.Date));
                    cols.Add("Warehouse").WithValueExpression(p => p.Warehouse);
                    cols.Add("HC_FCST").WithValueExpression(p => p.HC_FCST.ToString());
                    cols.Add("HC_Actual").WithValueExpression(p => p.HC_Actual.ToString());
                    cols.Add("HC_Support").WithValueExpression(p => p.HC_Support.ToString());
                    cols.Add("HC_Utilization").WithValueExpression(p => p.HC_Utilization.ToString());
                    cols.Add("Case_ID_in").WithValueExpression(p => p.Case_ID_in.ToString());
                    cols.Add("Case_ID_out").WithValueExpression(p => p.Case_ID_out.ToString());
                    cols.Add("Pallet_In").WithValueExpression(p => p.Pallet_In.ToString());
                    cols.Add("Pallet_Out").WithValueExpression(p => p.Pallet_Out.ToString());
                    cols.Add("Jobs_Rec").WithValueExpression(p => p.Jobs_Rec.ToString());
                    cols.Add("Jobs_Issue").WithValueExpression(p => p.Jobs_Issue.ToString());
                    cols.Add("Reel_ID_Rec").WithValueExpression(p => p.Reel_ID_Rec.ToString());

                    //cols.Add().WithColumnName("UrlExample")
                    //    .WithHeaderText("Edit")
                    //    .WithValueExpression((i, c) => c.UrlHelper.Action("detail", "demo", new { id = i.Id }));
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    var result = new QueryResult<tbl_weekdata>();

                    string globalSearch = options.GetAdditionalQueryOptionString("Search");
                    using (var db = new WarehouseLaborEffEntities())
                    {
                        var query = db.tbl_weekdata.AsQueryable();
                        //warehourse filter only
                        if (!string.IsNullOrEmpty(globalSearch))
                        {
                            query = query.Where(p => p.Warehouse.Contains(globalSearch));
                        }

                        result.TotalRecords = query.Count();
                        query = query.OrderBy(p => p.id);
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }
                        result.Items = query.ToList();
                    }
                    return result;
                })
            );
        }

        private static void RegisterMonthData()
        {
            MVCGridDefinitionTable.Add("MonthDataGrid", new MVCGridBuilder<tbl_monthdata>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithPaging(true, 20)
                .WithAdditionalQueryOptionNames("Search")
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add("Delete").WithHtmlEncoding(false)
                    .WithSorting(false)
                    .WithHeaderText(" ")
                    .WithValueExpression((p, c) => p.id.ToString())
                    .WithValueTemplate("<button class='btn btn-danger btn-sm' type='button' role='button' onclick='DoDelete({Value})'>Delete</button>");

                    cols.Add("id").WithValueExpression(p => p.id.ToString());
                    cols.Add("Date").WithValueExpression(p => DateTimeHelper.GetLocalDateStr(p.Date));
                    cols.Add("Warehouse").WithValueExpression(p => p.Warehouse);
                    cols.Add("HC_FCST").WithValueExpression(p => p.HC_FCST.ToString());
                    cols.Add("HC_Actual").WithValueExpression(p => p.HC_Actual.ToString());
                    cols.Add("HC_Support").WithValueExpression(p => p.HC_Support.ToString());
                    cols.Add("HC_Utilization").WithValueExpression(p => p.HC_Utilization.ToString());
                    cols.Add("Case_ID_in").WithValueExpression(p => p.Case_ID_in.ToString());
                    cols.Add("Case_ID_out").WithValueExpression(p => p.Case_ID_out.ToString());
                    cols.Add("Pallet_In").WithValueExpression(p => p.Pallet_In.ToString());
                    cols.Add("Pallet_Out").WithValueExpression(p => p.Pallet_Out.ToString());
                    cols.Add("Jobs_Rec").WithValueExpression(p => p.Jobs_Rec.ToString());
                    cols.Add("Jobs_Issue").WithValueExpression(p => p.Jobs_Issue.ToString());
                    cols.Add("Reel_ID_Rec").WithValueExpression(p => p.Reel_ID_Rec.ToString());

                    //cols.Add().WithColumnName("UrlExample")
                    //    .WithHeaderText("Edit")
                    //    .WithValueExpression((i, c) => c.UrlHelper.Action("detail", "demo", new { id = i.Id }));
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    var result = new QueryResult<tbl_monthdata>();

                    string globalSearch = options.GetAdditionalQueryOptionString("Search");
                    using (var db = new WarehouseLaborEffEntities())
                    {
                        var query = db.tbl_monthdata.AsQueryable();
                        //warehourse filter only
                        if (!string.IsNullOrEmpty(globalSearch))
                        {
                            query = query.Where(p => p.Warehouse.Contains(globalSearch));
                        }

                        result.TotalRecords = query.Count();
                        query = query.OrderBy(p => p.id);
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }
                        result.Items = query.ToList();
                    }
                    return result;
                })
            );
        }

        private static void RegisterHCData()
        {
            MVCGridDefinitionTable.Add("HCDataGrid", new MVCGridBuilder<tbl_hcdata>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithPaging(true, 20)
                .WithAdditionalQueryOptionNames("Search")
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add("Delete").WithHtmlEncoding(false)
                    .WithSorting(false)
                    .WithHeaderText(" ")
                    .WithValueExpression((p, c) => p.id.ToString())
                    .WithValueTemplate("<button class='btn btn-danger btn-sm' type='button' role='button' onclick='DoDelete({Value})'>Delete</button>");

                    cols.Add("id").WithValueExpression(p => p.id.ToString());
                    cols.Add("Date").WithValueExpression(p => DateTimeHelper.GetLocalDateStr(p.Date));
                    cols.Add("Warehouse").WithValueExpression(p => p.Warehouse);
                    cols.Add("Overall").WithValueExpression(p => p.Overall.ToString());
                    cols.Add("System_Clerk").WithValueExpression(p => p.System_Clerk.ToString());
                    cols.Add("Inventory_Control").WithValueExpression(p => p.Inventory_Control.ToString());

                    cols.Add("RTV_Scrap").WithValueExpression(p => p.RTV_Scrap.ToString());
                    cols.Add("Receiving").WithValueExpression(p => p.Receiving.ToString());
                    cols.Add("Shipping").WithValueExpression(p => p.Shipping.ToString());
                    cols.Add("Forklift_Driver").WithValueExpression(p => p.Forklift_Driver.ToString());
                    cols.Add("Total").WithValueExpression(p => p.Total.ToString());

                    //cols.Add().WithColumnName("UrlExample")
                    //    .WithHeaderText("Edit")
                    //    .WithValueExpression((i, c) => c.UrlHelper.Action("detail", "demo", new { id = i.Id }));
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    var result = new QueryResult<tbl_hcdata>();

                    string globalSearch = options.GetAdditionalQueryOptionString("Search");
                    using (var db = new WarehouseLaborEffEntities())
                    {
                        var query = db.tbl_hcdata.AsQueryable();
                        //warehourse filter only
                        if (!string.IsNullOrEmpty(globalSearch))
                        {
                            query = query.Where(p => p.Warehouse.Contains(globalSearch));
                        }

                        result.TotalRecords = query.Count();
                        query = query.OrderBy(p => p.id);
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }
                        result.Items = query.ToList();
                    }
                    return result;
                })
            );
        }

    }
}