﻿using System.Web;
using System.Web.Optimization;

namespace WarehouseLaborEfficiencyWeb
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // 使用 Modernizr 的开发版本进行开发和了解信息。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css")
                .Include("~/Content/datatables.min.css")                
                );
            bundles.Add(new ScriptBundle("~/bundles/bs")
                .Include("~/Scripts/bootstrap.min.js")
                .Include("~/Scripts/bootstrap-dialog.min.js")
                .Include("~/Scripts/datatables.min.js")
                .Include("~/Scripts/select2.min.js")
                .Include("~/Scripts/echarts.min.js")
                );
            bundles.Add(new StyleBundle("~/Content/bs")
                        .Include("~/Content/bootstrap.min.css")
                        .Include("~/Content/bootstrap-theme.min.css")
                        .Include("~/Content/bootstrap-dialog.min.css")
                        .Include("~/Content/select2.min.css")
                        .Include("~/Content/select2-bootstrap.min.css")
                        );

            bundles.Add(new ScriptBundle("~/bundles/main")
                        .Include("~/Scripts/main.js")
                        .Include("~/Scripts/wle.js")
                        );

            bundles.Add(new ScriptBundle("~/bundles/jsgrid")
                //.Include("~/Scripts/jsgrid.js")
                .Include("~/Scripts/jsgrid.min.js")
            );
            bundles.Add(new StyleBundle("~/Content/jsgrid")
                //.Include("~/Content/jsgrid.css")
                //.Include("~/Content/jsgrid-theme.css")
                .Include("~/Content/jsgrid.min.css") //for release
                .Include("~/Content/jsgrid-theme.min.css") //for release
            );

        }
    }
}