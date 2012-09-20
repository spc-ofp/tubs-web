// -----------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
{
    using System.Web.Optimization;
    
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Force optimization to be on or off, regardless of web.config setting
            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = false;

            // .debug.js, -vsdoc.js and .intellisense.js files 
            // are in BundleTable.Bundles.IgnoreList by default.
            // Clear out the list and add back the ones we want to ignore.
            // Don't add back .debug.js.
            bundles.IgnoreList.Clear();
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*intellisense.js");

            // Modernizr goes separate since it loads first
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-{version}.js"));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include(
                    "~/Scripts/jquery-{version}.js"                    
                ));

            // 3rd Party JavaScript files
            bundles.Add(new ScriptBundle("~/bundles/jsextlibs")
                .Include(
                    "~/Scripts/json2.js", // IE7 needs this
                    "~/Scripts/bootstrap.js",

                    // jQuery plugins
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/activity-indicator-{version}.js",
                    "~/Scripts/jquery-ui-timepicker-addon.js",
                    "~/Scripts/jquery.rateit.js",
                    "~/Scripts/jquery.sparkline.min.js",
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js",

                    // Knockout and its plugins
                    "~/Scripts/knockout-{version}.js",
                    "~/Scripts/knockout.mapping-latest.js",
                    "~/Scripts/knockout.activity.js",
                    "~/Scripts/knockout.asyncCommand.js",
                    "~/Scripts/knockout.dirtyFlag.js",
                    "~/Scripts/knockout.validation.js",

                    // Other 3rd party libraries
                    "~/Scripts/pageguide.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/amplify.js",
                    "~/Scripts/toastr.js"
                 ));

            // TODO Add another bundle for jQuery UI
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-sticky-footer.css", // Custom for sticky footer
                "~/Content/datetimepicker.css",
                "~/Content/pageguide.css",
                "~/Content/rateit.css",
                "~/Content/toastr.css",
                "~/Content/toastr-responsive.css"));
        }
    }
}