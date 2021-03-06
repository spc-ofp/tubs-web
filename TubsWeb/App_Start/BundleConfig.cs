﻿// -----------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
{
    /*
     * This file is part of TUBS.
     *
     * TUBS is free software: you can redistribute it and/or modify
     * it under the terms of the GNU Affero General Public License as published by
     * the Free Software Foundation, either version 3 of the License, or
     * (at your option) any later version.
     *  
     * TUBS is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU Affero General Public License for more details.
     *  
     * You should have received a copy of the GNU Affero General Public License
     * along with TUBS.  If not, see <http://www.gnu.org/licenses/>.
     */
    using System.Web.Optimization;
    
    /// <summary>
    /// BundleConfig configures bundling of CSS and JavaScript resources.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Register CSS and JavaScript bundles.
        /// </summary>
        /// <param name="bundles">BundleCollection provided by application.</param>
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

            // TODO:  We've gotten this far down the road without Modernizr
            // Should it be pulled from the project?
            // Modernizr goes separate since it loads first
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-{version}.js"));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include(
                    "~/Scripts/jquery-{version}.js"
                ));

            // knockout and plugins
            // TODO Remove Knockout.Viewmodel plugin for now.
            // Knockout.Mapping may not have a maintainer, but it's very performant
            bundles.Add(new ScriptBundle("~/bundles/knockout")
                .Include(
                    "~/Scripts/toastr.js", // For some reason, toastr in the jsextlibs bundle isn't appearing here...
                    "~/Scripts/knockout-{version}.js",
                    "~/Scripts/knockout.mapping-latest.js",
                    "~/Scripts/knockout.activity.js",
                    "~/Scripts/knockout.command.js",
                    "~/Scripts/knockout.dirtyFlag.js",
                    "~/Scripts/knockout.validation.js",
                    "~/Scripts/knockout.viewmodel.{version}.js",
                    "~/Scripts/tubs-common-extensions.js",
                    "~/Scripts/tubs-custom-bindings.js"
                ));

            // 3rd Party JavaScript files
            bundles.Add(new ScriptBundle("~/bundles/jsextlibs")
                .Include(
                    "~/Scripts/underscore.js", // Better array ops
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/bootstrap-datepicker.js", // jQuery UI doesn't work well with Bootstrap

                    // jQuery plugins
                    "~/Scripts/jquery.hotkeys.js",
                    "~/Scripts/activity-indicator-{version}.js",
                    "~/Scripts/jquery.rateit.js",
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
                    "~/Scripts/trunk8.js",

                    // Other 3rd party libraries
                    "~/Scripts/select2.js",
                    "~/Scripts/pageguide.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/amplify.js",
                    "~/Scripts/toastr.js"
                 ));

            // sco.js includes more components than what is present
            // but this is the minimum of what is used in TUBS
            bundles.Add(new ScriptBundle("~/bundles/scojs")
                .Include(
                    "~/Scripts/sco.confirm.js",
                    "~/Scripts/sco.modal.js",
                    "~/Scripts/sco.panes.js",
                    "~/Scripts/sco.tab.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/leaflet")
                .Include(
                    "~/Scripts/leaflet-src.js"
                ));

            // Typeahead is, at least at present, a fairly uncommon facility
            // Initial use isn't too promising.  Requires specific JSON
            // return format and CSS wizardry
            bundles.Add(new ScriptBundle("~/bundles/typeahead")
                .Include(
                    "~/Scripts/typeahead.js",
                    "~/Scripts/hogan-{version}.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/bootstrap-datepicker.css",
                "~/Content/pageguide.css",
                "~/Content/select2.css",
                "~/Content/rateit.css",
                "~/Content/toastr.css",
                "~/Content/scojs.css",
                "~/Content/tubs.css"
                ));
        }
    }
}