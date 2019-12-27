using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BugBounty
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var ejWaitingPopUp = new List<string>
            {
                "~/scripts/essentialjs/common/ej.core.min.js",
                "~/scripts/essentialjs/ej.waitingpopup.min.js"
            };

            var essentialStyles = new List<string>
            {
                "~/Content/Styles/Bootstrap/bootstrap.min.css",
                "~/Content/Styles/Fonts/font-RobotoRegular.css",
                "~/Content/Styles/Fonts/font-Server.less",
                "~/Content/Styles/Less/NewThemes/Mixins.less",
                "~/Content/Styles/LESS/Accounts/footer.less"
            };

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/General/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/General/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/General/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/General/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/General/bootstrap.css",
                      "~/Content/General/site.css"));

            bundles.Add(new StyleBundle("~/Content/bugs-home").Include(
                     "~/Content/Bugs/BugsHome.less"));

            bundles.Add(new StyleBundle("~/Content/accounts-layout").Include(
                "~/Content/Styles/Bootstrap/bootstrap.min.css",
                "~/Content/Styles/Fonts/font-RobotoRegular.css",
                "~/Content/Styles/Fonts/font-Server.less",
                "~/Content/Styles/Less/NewThemes/Mixins.less",
                "~/Content/Styles/LESS/Accounts/footer.less",
                "~/Content/Styles/EssentialJS/ej.theme.min.css",
                "~/Content/Styles/EssentialJS/ej.widgets.core.min.css",
                "~/Content/Styles/LESS/Accounts/Layout.less"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/accounts").Include(new List<string>
            {
                "~/Scripts/jQuery/jquery-1.10.2.min.js",
                "~/Scripts/jQuery/jquery.validate.min.js",
                "~/Scripts/jQuery/jquery.easing-1.3.min.js",
                "~/Scripts/Bootstrap/bootstrap.min.js",
                "~/Scripts/Core/Placeholder.js"
            }.Union(ejWaitingPopUp).ToArray()));

            bundles.Add(new StyleBundle("~/Content/accounts").Include(
                "~/Content/Styles/LESS/Accounts/Login.less"));

            bundles.Add(new StyleBundle("~/Content/syncfusion-login").Include(essentialStyles.Union(
               new List<string>
               {
                   "~/Content/Styles/EssentialJS/ej.theme.min.css",
                   "~/Content/Styles/EssentialJS/ej.widgets.core.min.css",
                   "~/Content/Styles/LESS/Accounts/SyncfusionLogin.less"
               }).ToArray()));

            bundles.Add(new ScriptBundle("~/bundles/scripts/accounts/login").Include(
                "~/Scripts/Accounts/Login.js",
                "~/Scripts/Accounts/SyncfusionLogin.js",
                "~/Scripts/Password/ShowHidePassword.js"));
        }
    }
}
