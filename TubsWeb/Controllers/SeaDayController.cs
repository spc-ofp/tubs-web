// -----------------------------------------------------------------------
// <copyright file="SeaDayController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
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
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;

    public class SeaDayController : SuperController
    {
        //
        // GET: /SeaDay/
        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);            
            // Push the projection into a List so that it's not the NHibernate collection implementation
            var days = repo.FilterBy(d => d.Trip.Id == tripId.Id).ToList<SeaDay>();
            AddMinMaxDates(tripId);
            ViewBag.Title = String.Format("Sea days for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(days);
        }

        private SeaDay FromTripAndDayNumber(int tripId, int dayNumber)
        {
            ViewBag.TripId = tripId;
            var repo = TubsDataService.GetRepository<SeaDay>(MvcApplication.CurrentSession);
            var days = repo.FilterBy(d => d.Trip.Id == tripId);
            int maxDays = days.Count();
            if (dayNumber > maxDays)
            {
                dayNumber = maxDays;
            }
            ViewBag.MaxDays = maxDays;
            ViewBag.CurrentDay = dayNumber;
            ViewBag.Title = String.Format("Sea day {0} of {1}", dayNumber, maxDays);
            return days.Skip(dayNumber - 1).Take(1).FirstOrDefault();
        }

        public ActionResult Index(int tripId, int dayNumber)
        {
            var day = FromTripAndDayNumber(tripId, dayNumber);
            if (IsApiRequest())
            {
                var sdvm = (day as PurseSeineSeaDay).AsViewModel();
                return Json(sdvm, JsonRequestBehavior.AllowGet);
            }
            return View(day);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult EditDay(Trip tripId, int dayNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return new NoSuchTripResult();
            }
            var day = FromTripAndDayNumber(tripId.Id, dayNumber) as PurseSeineSeaDay;
            // FromTripAndDayNumber set the title, we want to enhance it
            ViewBag.Title = "Edit " + ViewBag.Title;
            // So there's a small problem here.  If this is a new day, the ViewModel is too empty
            var sdvm = day.AsViewModel();
            if (IsApiRequest())
                return Json(sdvm, JsonRequestBehavior.AllowGet);
            return View(sdvm);
        }

        // This should only ever be hit by the Knockout function, but we'll still probably want
        // to cater for straight up HTML Forms
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult EditDay(Trip tripId, int dayNumber, SeaDayViewModel sdvm)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                if (IsApiRequest())
                    return Json("No such trip", JsonRequestBehavior.AllowGet);
                return new NoSuchTripResult();
            }

            // The delta between Ship's date and UTC date shouldn't be more than +/- 1 day 
            DateTime? shipStartOfDay = sdvm.ShipsDate.Merge(sdvm.ShipsTime);
            DateTime? utcStartOfDay = sdvm.UtcDate.Merge(sdvm.UtcTime);
            if (shipStartOfDay.HasValue && utcStartOfDay.HasValue)
            {
                var deltaT = shipStartOfDay.Value.Subtract(utcStartOfDay.Value);
                if (Math.Abs(deltaT.TotalDays) > 1.0d)
                {
                    var msg = string.Format("Difference of {0} days between ship's and local time too large", Math.Abs(deltaT.TotalDays));
                    ModelState["UtcDate"].Errors.Add(msg);
                    // TODO Work out whether we should set the error state on all 4 fields, or just one at random
                }
            }

            // TODO Validate non-null events to check that codes are in the correct
            // ranges given the version

            // Remove any entrys from DeletedEvents that have invalid values (e.g. less than 1)
            var vde = sdvm.DeletedEvents.Where(c => c > 0);
            sdvm.DeletedEvents = vde.ToList();

            // TODO:  Check to see if any of the still-valid Ids in DeletedEvents has associated
            // set.  If so, don't allow the delete (we'll have to work out how to delete bad ones later)
            // Also, if there is a set, don't allow the activity to change.
            // Maybe this means we'll have to change the ViewModel to prevent deletes/changes for these...

            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(sdvm);
            }           

            // Try this for now, refactor later (if at all)
            // drepo for (d)ay, erepo for (e)vent (aka Activity).
            IRepository<SeaDay> drepo = TubsDataService.GetRepository<SeaDay>(MvcApplication.CurrentSession);
            IRepository<Activity> erepo = TubsDataService.GetRepository<Activity>(MvcApplication.CurrentSession);

            // VERY IMPORTANT TODO:  If an activity is _new_ and has an activity code of fishing, create a new empty
            // set.  This will enable easier entry on the PS-3 side.  Set# and Page# should match, so remind users
            // to check?
 
            // This is the happy path -- if we get here, everything should have worked...
            if (IsApiRequest())
            {
                // Return JSON for debug only
                //return Json(sdvm, JsonRequestBehavior.AllowGet);
                Response.StatusCode = (int)HttpStatusCode.NoContent;
                return null;
            }
            return RedirectToAction("Index", "SeaDay", new { tripId = tripId.Id, dayNumber = dayNumber });
        }

        public ActionResult AutoFill(Trip tripId)
        {
            if (null == tripId)
            {
                // TODO Figure out if this is really how we want to handle this...
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);
            if (repo.FilterBy(d => d.Trip.Id == tripId.Id).Count() > 0)
            {
                // Already has at least one day -- don't mess with it
                return JavaScript("alert('One or more days already present');");
            }

            // Figure out how many days are between departure and end date
            TimeSpan span = tripId.ReturnDate.Value.Subtract(tripId.DepartureDate.Value);
            int daysAdded = 0;
            for (int i = 0; i <= span.Days; i++)
            {
                DateTime startDate = tripId.DepartureDate.Value.AddDays(i);
                SeaDay seaDay = tripId.CreateSeaDay(startDate);
                var enteredDate = DateTime.Now;
                if (null != seaDay)
                {
                    seaDay.Trip = tripId;
                    seaDay.EnteredBy = User.Identity.Name;
                    seaDay.EnteredDate = enteredDate;
                    repo.Add(seaDay);
                    daysAdded++;
                }
            }

            return JavaScript(String.Format("alert('Added {0} day(s)');", daysAdded));
        }

    }
}
