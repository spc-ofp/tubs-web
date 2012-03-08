// -----------------------------------------------------------------------
// <copyright file="SuperController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
{
    using System.Web.Mvc;
    using TubsWeb.Core;
    using Spc.Ofp.Tubs.DAL.Entities;

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
    using System.Reflection;

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