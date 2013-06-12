﻿// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using TubsWeb.Models;

    /// <summary>
    /// 
    /// </summary>
    public class HomeController : SuperController
    {
        
        /// <summary>
        /// TripsEnteredSince returns the number of trips entered/registered by the
        /// currently logged in user
        /// </summary>
        /// <param name="daysBack"></param>
        /// <returns></returns>
        private int TripsEnteredSince(int daysBack)
        {
            string query = @"select count(1) from obsv.trip where DATEADD(dd, ?, GETDATE()) < entered_dtime";
            var results = TubsDataService.Execute(query, daysBack * -1);
            return (int)results[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private int TripsInLastWeekFor(string user)
        {
            // Chomp the domain off user
            int slashIndex = user.IndexOf(@"\");
            if (slashIndex > -1)
            {
                user = user.Substring(slashIndex + 1);
            }

            // Prefix with a wildcard
            user = "%" + user.ToUpper();

            string query = @"select count(1) from obsv.trip where DATEADD(dd, -7, GETDATE()) < entered_dtime AND UPPER(entered_by) like ?";
            var results = TubsDataService.Execute(query, user);
            return (int)results[0];            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Home Page";
            ViewBag.TripsEnteredSince = TripsEnteredSince(30);
            ViewBag.TripsInLastWeek = TripsInLastWeekFor(User.Identity.Name);
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        // TODO: Consider pushing this into a comma separated list in web.config
        // and then just doing a split here...
        public ActionResult EntryHelp()
        {
            // Everybody wants some!
            HashSet<string> sections = new HashSet<string>
            {
                "PS-2",
                "GEN-3"
            };
            // Check the installed location:
            string installedFor = WebConfigurationManager.AppSettings["DefaultProgramCode"] ?? String.Empty;
            if ("PGOB".Equals(installedFor, StringComparison.InvariantCultureIgnoreCase))
            {
                sections.Add("PS-1 (PG)");
                sections.Add("PS-3");
            }
            else if ("SBOB".Equals(installedFor, StringComparison.InvariantCultureIgnoreCase))
            {
                sections.Add("PS-1 (SB)");
            }
            else
            {
                sections.Add("PS-1");
                sections.Add("PS-3");
                sections.Add("GEN-1");
                sections.Add("GEN-2");
                sections.Add("GEN-5");
                sections.Add("GEN-6");
            }
            return View(sections);
        }

        // FIXME Add some kind of security here
        public ActionResult Debug()
        {
            ViewBag.Xyzzy = "Xyzzy";
            return View();
        }

        /// <summary>
        /// Action for displaying the confidentiality disclaimer.
        /// </summary>
        /// <returns></returns>
        public ActionResult Confidentiality()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Reports()
        {
            // Not exactly this, but similar
            // http://ofp-reports/Reports/Pages/ReportViewer.aspx?%2f{0}&amp;rs:Command=Render&amp;tripId={1}
            // This Url 
            // http://localhost/ReportServer/Pages/ReportViewer.aspx?%2fProgramSummary&rs:Command=Render
            string reportServer = WebConfigurationManager.AppSettings["ReportingServicesFormatUrl"] ?? String.Empty;
          
            int offset = reportServer.IndexOf('?');
            if (offset > 0)
            {
                reportServer = reportServer.Substring(0, offset);
            }
            
            ViewBag.ReportingUrl = reportServer;
            return View();
        }
    }
}
