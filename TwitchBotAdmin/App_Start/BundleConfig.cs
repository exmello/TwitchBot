﻿using System.Web;
using System.Web.Optimization;

namespace TwitchBotAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //admin template bundles
            bundles.Add(new ScriptBundle("~/bundles/template-core").Include(
                        "~/js/jquery.min.js",
                        "~/js/bootstrap.min.js",
                        "~/js/jquery.nicescroll.min.js"));

            bundles.Add(new StyleBundle("~/Content/template-css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/fonts/css/font-awesome.min.css",
                      "~/Content/animate.min.css",
                      "~/Content/custom.css"));

            bundles.Add(new StyleBundle("~/Content/datatable-css").Include(
                      "~/js/datatables/jquery.dataTables.min.css",
                      "~/js/datatables/fixedHeader.bootstrap.min.css",
                      "~/js/datatables/responsive.bootstrap.min.css",
                      "~/js/datatables/scroller.bootstrap.min.css"));
        }
    }
}
