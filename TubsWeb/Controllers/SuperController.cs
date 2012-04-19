// -----------------------------------------------------------------------
// <copyright file="SuperController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;

    /// <summary>
    /// SuperController adds logging and user error reporting capabilities to the MVC3 Controller.
    /// </summary>
    [UseTransactionsByDefault]
    [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
    public class SuperController : Controller
    {
        // Alert levels
        public const string ALERT_SUCCESS = "alert-success";
        public const string ALERT_DANGER = "alert-danger";
        public const string ALERT_ERROR = "alert-error";
        public const string ALERT_INFO = "alert-info";

        // Minimum/Maximum Date keys
        public const string MINUMUM_DATE = "MinDate";
        public const string MAXIMUM_DATE = "MaxDate";

        // Report names
        public const string PSTripSummary = "PS-TripSummary";
        public const string CoverPage = "CoverPage";

        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(SuperController));

        /// <summary>
        /// Flash stores an alert message and level into ViewData.
        /// Note that ViewData doesn't persist across a redirect, so anything stored thus
        /// will be lost if the user is pushed to another page via RedirectToAction(...)
        /// </summary>
        /// <param name="message">Alert message</param>
        /// <param name="level">Alert level</param>
        public void Flash(string message, string level = ALERT_ERROR)
        {
            ViewData["flash"] = message;
            ViewData["flash-level"] = level;
        }

        public string GetReportUrl(string reportName, int tripId)
        {
            string formatString = WebConfigurationManager.AppSettings["ReportingServicesFormatUrl"].ToString();
            return String.Format(formatString, reportName, tripId);
        }

        /// <summary>
        /// Push JavaScript-compatible string values of departure and return dates into
        /// ViewBag for use in wiring up the date/time picker.
        /// </summary>
        /// <param name="trip"></param>
        public void AddMinMaxDates(Trip trip)
        {
            if (null != trip)
            {
                if (trip.DepartureDate.HasValue || trip.DepartureDateOnly.HasValue)
                {
                    ViewData[MINUMUM_DATE] =
                        trip.DepartureDate.HasValue ?
                            trip.DepartureDate.Value.ToString("r") :
                            trip.DepartureDateOnly.Value.ToString("r");
                }

                if (trip.ReturnDate.HasValue || trip.ReturnDateOnly.HasValue)
                {
                    ViewData[MAXIMUM_DATE] =
                        trip.ReturnDate.HasValue ?
                            trip.ReturnDate.Value.ToString("r") :
                            trip.ReturnDateOnly.Value.ToString("r");
                }
            }
        }

        protected void AddTripNavbar(Trip tripId)
        {
            // TODO Figure out how to get the Controller name from the ControllerContext
            // Until then, no active pill
            // Okay, now we've got the controller name, but we need to figure out how to
            // pass another value to the view showing which matches the current controller
            // Probably best way to do this is to push the title, action name, and controller name into
            // a List<Tuple<string, string, string>>
            // Yet another thing to consider -- how to deal with PS or LL only views?
            var controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            var routeName = this.ControllerContext.RouteData.GetRequiredString("action");

            var routeValues = new { tripId = tripId.Id };

            IList<Tuple<string, string>> pills = new List<Tuple<string, string>>(12);

            if (typeof(PurseSeineTrip) == tripId.GetType())
            {
                pills.Add(Tuple.Create("Auxiliaries", Url.Action("Index", "Auxiliaries", routeValues)));
                pills.Add(Tuple.Create("Well Content", Url.Action("Index", "WellContent", routeValues)));
            }

            pills.Add(Tuple.Create("Vessel", Url.Action("Index", "VesselDetails", routeValues)));
            pills.Add(Tuple.Create("Crew", Url.Action("Index", "Crew", routeValues)));
            pills.Add(Tuple.Create("Electronics", Url.Action("List", "Electronics", routeValues)));
            pills.Add(Tuple.Create("Safety Gear", Url.Action("Index", "SafetyInspection", routeValues)));
            pills.Add(Tuple.Create("Fishing Gear", Url.Action("Index", "Gear", routeValues)));            
            pills.Add(Tuple.Create("GEN-1", Url.Action("Index", "Gen1", routeValues)));
            pills.Add(Tuple.Create("GEN-2", Url.Action("List", "Gen2", routeValues)));
            pills.Add(Tuple.Create("GEN-3", Url.Action("Index", "Gen3", routeValues))); 
            pills.Add(Tuple.Create("GEN-6", Url.Action("List", "Gen6", routeValues)));
            pills.Add(Tuple.Create("Days", Url.Action("List", "SeaDay", routeValues)));
            pills.Add(Tuple.Create("Sets", Url.Action("List", "FishingSet", routeValues)));
            pills.Add(Tuple.Create("Page Counts", Url.Action("Index", "PageCount", routeValues)));
            
            // It's possible that the scan didn't come with a registration cover page.  If not, TUBS has to create it
            // This URL is outside the system
            // TODO Figure out the best way to have this URL open in a new window.
            string coverPageUrl = GetReportUrl(CoverPage, tripId.Id);
            pills.Add(Tuple.Create("Cover Page", coverPageUrl));
            string summaryReportUrl = GetReportUrl(PSTripSummary, tripId.Id);
            pills.Add(Tuple.Create("Trip Summary", summaryReportUrl));

            pills.Add(Tuple.Create("Position Audit", Url.Action("PositionAudit", "Trip", routeValues)));
            if (!tripId.IsReadOnly)
            {
                pills.Add(Tuple.Create("Close Trip", Url.Action("Close", "Trip", routeValues)));
            }

            ViewBag.NavPills = pills;
        }

        // FIXME Work on this as a refactoring effort
        protected ActionResult LoadDoesntWork(Trip tripId, string titleFormat, string fieldName = "this", bool createNew = false)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format(titleFormat, tripId.ToString());
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            AddMinMaxDates(tripId);
            if ("this".Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
            {
                return View(actionName, tripId);
            }

            var fi = tripId.GetType().GetField(fieldName);
            if (null == fi)
            {
                // No such field
                return View("Error");
            }

            return
                createNew ?
                View(actionName, fi.GetValue(tripId) ?? Activator.CreateInstance(fi.GetType())) :
                View(actionName, fi.GetValue(tripId));

        }
    }
}