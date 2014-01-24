using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js").Include(
                        "~/Scripts/jquery-migrate-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/Default").Include(
                      "~/Content/themes/Default/bootstrap.css",
                      "~/Content/themes/Default/datepicker.css",
                      "~/Content/themes/Default/Site.css",
                      "~/Content/css/select2.css"));

            bundles.Add(new StyleBundle("~/Content/Red").Include(
                      "~/Content/themes/Red/bootstrap.min.css",
                      "~/Content/themes/Red/datepicker.css",
                      "~/Content/themes/Red/Site.css",
                      "~/Content/css/select2.css"));
        }
    }
}
