using System.Web;
using System.Web.Optimization;

namespace EmployeeInformationSystem.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/vendor/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                        "~/Content/vendor/popper.js/umd/popper.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Content/vendor/bootstrap/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerycookie").Include(
                        "~/Content/vendor/jquery.cookie/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                        "~/Content/vendor/chart.js/Chart.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/vendor/jquery-validation/jquery.validate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/charts-home").Include(
                        "~/Scripts/js/charts-home.js"));

            bundles.Add(new ScriptBundle("~/bundles/front").Include(
                        "~/Scripts/js/front.js"));

            bundles.Add(new ScriptBundle("~/bundles/gijgo").Include(
                        "~/Content/vendor/gijgo/js/gijgo.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/vendor/bootstrap/css/bootstrap.min.css",
                      "~/Content/vendor/font-awesome/css/font-awesome.min.css",
                      "~/Content/css/fontastic.css",
                      "~/Content/css/style.default.css",
                      "~/Content/css/custom.css"));
            bundles.Add(new StyleBundle("~/Content/gijgo").Include("~/Content/vendor/gijgo/css/gijgo.min.css"));
        }
    }
}
