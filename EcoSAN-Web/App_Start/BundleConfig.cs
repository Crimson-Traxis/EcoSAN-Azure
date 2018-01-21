using System.Web;
using System.Web.Optimization;

namespace EcoSAN_Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Content/js/bootstrap.js",
                      "~/Content/js/easyResponsiveTabs.js",
                      "~/Content/js/fastclick.js",
                      "~/Content/js/jquery-1.10.2.min.js",
                      "~/Content/js/jquery.easing.1.3.js",
                      "~/Content/js/jquery.magnific-popup.min.js",
                      "~/Content/js/owl.carousel.min.js",
                      "~/Content/js/velocity.min.js",
                      "~/Content/js/bootstrap-notify.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                      "~/Content/js/main.js"));

            bundles.Add(new StyleBundle("~/bundles/cesium").IncludeDirectory("~/Scripts/Cesium", "*.css",true));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/style.css",
                      "~/Content/css/easy-responsive-tabs.css",
                      "~/Content/css/magnific-popup.css",
                      "~/Content/css/owl.carousel.min.css",
                      "~/Content/css/owl.theme.default.min.css",
                      "~/Content/css/themify-icons.css"));
        }
    }
}
