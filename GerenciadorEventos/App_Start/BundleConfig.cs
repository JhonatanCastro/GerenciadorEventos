using System.Web;
using System.Web.Optimization;

namespace GerenciadorEventos
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.0.0.min.js",
                        "~/Scripts/jquery-ui-1.11.4.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymask").Include(   
                        "~/Scripts/jquery.maskedinput.js",
                        "~/Scripts/jquery.maskMoney.js",
                        "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                        "~/Scripts/site.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/DataTables/css/jquery.dataTables.css",
                      "~/Content/DataTables/css/dataTables.bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));
        }
    }
}
