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
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;

    /// <summary>
    /// SuperController adds logging and user error reporting capabilities to the Controller.
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

        // This is here until I can find a better way to manage this.
        protected bool IsApiRequest()
        {
            return null != Request.AcceptTypes && Request.AcceptTypes.Contains("application/json");
        }

        protected ActionResult InvalidTripResponse()
        {
            if (IsApiRequest())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return GettableJsonNetData("Missing or invalid trip");
            }
            return new NoSuchTripResult();
        }

        /// <summary>
        /// After much hair-pulling, this seems like the best way to manage replacing the
        /// crappy JSON serializer in MVC with the Newtonsoft version.
        /// NOTE:  MVC4 uses the Newtonsoft library, but only on the WebApi side of the
        /// house.  If you're still here in MVC, you're SOL
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult GettableJsonNetData(object data)
        {
            return new JsonNetResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = data
            };
        }

        /// <summary>
        /// Convert MVC model errors into a single string
        /// </summary>
        /// <param name="errors">MVC model error collection</param>
        /// <returns>Comma separated string containing all errors.</returns>
        protected string ModelErrorString(ModelErrorCollection errors)
        {
            var messages = errors.Select(e => e.ErrorMessage);
            return String.Join(", ", messages);
        }

        /// <summary>
        /// Write MVC model errors to error log.
        /// </summary>
        protected void LogModelErrors()
        {
            StringBuilder buffer = new StringBuilder();
            var errors =
                from prop in ModelState
                where prop.Value.Errors.Count > 0
                select prop.Key + ": " + ModelErrorString(prop.Value.Errors);
            errors.ToList().ForEach(e => buffer.Append(e));
            Logger.WarnFormat("Model validation failed with the following errors:\n{0}", buffer);
        }

        /// <summary>
        /// Convert MVC model errors into JSON.
        /// </summary>
        /// <returns>JSON representation of MVC model errors.</returns>
        protected JsonResult ModelErrorsResponse()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errors =
                from prop in ModelState
                where prop.Value.Errors.Count > 0
                select ModelErrorString(prop.Value.Errors);

            return GettableJsonNetData(errors.ToList());
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
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

        /// <summary>
        /// Read action out of the route data.
        /// </summary>
        /// <returns>Currently executing action name</returns>
        protected string CurrentAction()
        {
            return this.ControllerContext.RouteData.GetRequiredString("action");
        }

        /// <summary>
        /// Read controller out of the route data.
        /// </summary>
        /// <returns>Controller hosting the currently executing action.</returns>
        protected string CurrentController()
        {
            return this.ControllerContext.RouteData.GetRequiredString("controller");
        }

        /// <summary>
        /// Check to see if this is the "Add" action
        /// </summary>
        /// <returns>true if this is the "Add" action, false otherwise</returns>
        protected bool IsAdd()
        {
            return "Add".Equals(CurrentAction(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check to see if this is the "Edit" action
        /// </summary>
        /// <returns>true if this is the "Edit" action, false otherwise</returns>
        protected bool IsEdit()
        {
            return "Edit".Equals(CurrentAction(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check to see if this is the "Index" action
        /// </summary>
        /// <returns>true if this is the "Index" action, false otherwise</returns>
        protected bool IsIndex()
        {
            return "Index".Equals(CurrentAction(), StringComparison.InvariantCultureIgnoreCase);
        }

        // This is still a work in progress
        private void AddTripNavbar(PurseSeineTrip tripId)
        {
            var routeValues = new { tripId = tripId.Id };
            IList<NavPill> pills = new List<NavPill>(12);

            pills.Add(new NavPill { Title = "PS-1", Href = Url.Action("Index", "Ps1", routeValues) });

            // Don't display pills for data we know we don't have
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.VesselAttributes != null))
            {
                pills.Add(new NavPill { Title = "Vessel Characteristics (PS-1)", Href = Url.Action("Index", "Auxiliaries", routeValues) });
            }
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.WellContent.Count > 0))
            {
                pills.Add(new NavPill { Title = "Well Contents (PS-1)", Href = Url.Action("Index", "WellContent", routeValues) });
            }
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.VesselNotes != null))
            {
                pills.Add(new NavPill { Title = "Vessel Comments (PS-1)", Href = Url.Action("Index", "WellContent", routeValues) });
            }
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.Crew.Count > 0))
            {
                pills.Add(new NavPill { Title = "Crew (PS-1)", Href = Url.Action("Index", "Crew", routeValues) });
            }
        }

        /// <summary>
        /// AddTripNavBar adds a list of titles and links for use in navigating within
        /// a trip.  Data is set into ViewBag.NavPills
        /// </summary>
        /// <param name="tripId">Trip used to construct navigation links.</param>
        protected void AddTripNavbar(Trip tripId)
        {
            var routeValues = new { tripId = tripId.Id };

            IList<Tuple<string, string>> pills = new List<Tuple<string, string>>(12);

            if (typeof(PurseSeineTrip) == tripId.GetType())
            {
                var pstrip = tripId as PurseSeineTrip;
                pills.Add(Tuple.Create("PS-1", Url.Action("Index", "Ps1", routeValues)));
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && pstrip.Electronics.Any()))
                {
                    pills.Add(Tuple.Create("Electronics (PS-1)", Url.Action("Index", "Electronics", routeValues)));
                }
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && pstrip.WellContent.Any()))
                {
                    pills.Add(Tuple.Create("Well Content (PS-1)", Url.Action("Index", "WellContent", routeValues)));
                }
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && pstrip.Crew.Any()))
                {
                    pills.Add(Tuple.Create("Crew (PS-1)", Url.Action("Index", "Crew", routeValues)));
                }
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && pstrip.SeaDays.Any()))
                {
                    pills.Add(Tuple.Create("Days (PS-2)", Url.Action("List", "SeaDay", routeValues)));
                }
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && pstrip.FishingSets.Any()))
                {
                    pills.Add(Tuple.Create("Sets (PS-3)", Url.Action("List", "FishingSet", routeValues)));
                }

                // TODO:  Not sure if we want to force entry of some set information before PS-4 entry
                pills.Add(Tuple.Create("Samples (PS-4)", Url.RouteUrl(RouteConfig.Ps4List, routeValues)));
                
            }

            if (typeof(LongLineTrip) == tripId.GetType())
            {
                var lltrip = tripId as LongLineTrip;
                pills.Add(Tuple.Create("LL-1", Url.Action("Index", "TripInfo", routeValues)));
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && lltrip.Electronics.Any()))
                {
                    pills.Add(Tuple.Create("Electronics (LL-1)", Url.Action("Index", "Electronics", routeValues)));
                }

                if (!tripId.IsReadOnly || (tripId.IsReadOnly || lltrip.FishingSets.Any()))
                {
                    pills.Add(Tuple.Create("Sets (LL-2/3)", Url.Action("List", "SetHaul", routeValues)));
                    pills.Add(Tuple.Create("Catch Monitoring (LL-4)", Url.RouteUrl(RouteConfig.LongLineSampleList, routeValues)));
                }

            }
         
            // Hide pills if trip is closed and no such entity exists
            bool hasGen1 = tripId.Sightings.Any() || tripId.Transfers.Any();
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && hasGen1))
            {
                pills.Add(Tuple.Create("GEN-1", Url.RouteUrl(RouteConfig.Gen1, routeValues)));
            }
            
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.Interactions.Any()))
            {
                pills.Add(Tuple.Create("GEN-2", Url.RouteUrl(RouteConfig.Gen2, routeValues)));
            }
            
            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.TripMonitor != null))
            {
                pills.Add(Tuple.Create("GEN-3", Url.RouteUrl(RouteConfig.Gen3, routeValues)));
            }
            // GEN-5 is Purse Seine only
            if (typeof(PurseSeineTrip) == tripId.GetType())
            {
                if (!tripId.IsReadOnly || (tripId.IsReadOnly && ((PurseSeineTrip)tripId).FishAggregatingDevices.Any()))
                {
                    pills.Add(Tuple.Create("GEN-5", Url.RouteUrl(RouteConfig.Gen5, routeValues)));
                }
            }

            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.PollutionEvents.Any()))
            {
                pills.Add(Tuple.Create("GEN-6", Url.Action("List", "Gen6", routeValues)));
            }

            if (!tripId.IsReadOnly || (tripId.IsReadOnly && tripId.PageCounts.Any()))
            {
                pills.Add(Tuple.Create("Page Counts", Url.RouteUrl(RouteConfig.PageCount, routeValues)));
            }

            pills.Add(Tuple.Create("Position Audit", Url.Action("PositionAudit", "Trip", routeValues)));

            // View trip positions and track in the browser
            if (null != tripId.Pushpins && tripId.Pushpins.Any())
            {
                pills.Add(
                    Tuple.Create(
                        "Map",
                        Url.RouteUrl(TubsWeb.RouteConfig.TripMap, routeValues)
                     )
                );
            }

            // TODO Add security trimming
            if (!tripId.IsReadOnly)
            {
                pills.Add(Tuple.Create("Close Trip", "#closeModal"));
            }

            ViewBag.NavPills = pills;
        }

    }
}