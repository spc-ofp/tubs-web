// -----------------------------------------------------------------------
// <copyright file="TripController.cs" company="Secretariat of the Pacific Community">
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
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;

    public class TripController : SuperController
    {
        
        /// <summary>
        /// Check that the required dependent objects are present.
        /// </summary>
        /// <param name="trip"></param>
        private void ValidateTripDependencies(Trip trip, TripHeaderViewModel thvm)
        {
            // These shouldn't happen, so in addition to notifying user, drop a warning message into the application log
            if (null == trip.Observer)
            {
                var message = String.Format("Can't find observer with name [{0}] and staff code [{1}]", thvm.ObserverFullName, thvm.ObserverCode);
                Logger.Warn(message);
                ModelState["ObserverName"].Errors.Add(message);
            }

            if (null == trip.Vessel)
            {
                var message = String.Format("Can't find vessel with name [{0}]", thvm.VesselName);
                Logger.Warn(message);
                ModelState["VesselName"].Errors.Add(message);
            }

            if (null == trip.DeparturePort)
            {
                var message = String.Format("Can't find port with name [{0}] and port code [{1}]", thvm.DeparturePortName, thvm.DeparturePortCode);
                Logger.Warn(message);
                ModelState["DeparturePortName"].Errors.Add(message);
            }

            if (null == trip.ReturnPort)
            {
                var message = String.Format("Can't find port with name [{0}] and port code [{1}]", thvm.ReturnPortName, thvm.ReturnPortCode);
                Logger.Warn(message);
                ModelState["ReturnPortName"].Errors.Add(message);
            }
        }


        //
        // GET: /Trip/
        public ActionResult Index(int? page, int itemsPerPage = 15)
        {
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trips = repo.GetPagedList((page ?? 0) * itemsPerPage, itemsPerPage);
            ViewBag.HasPrevious = trips.HasPrevious;
            ViewBag.HasNext = trips.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            ViewBag.TotalRows = Math.Max(repo.All().Count() - 1, 0);
            return View(trips.Entities);
        }

        public ActionResult MyTrips(int? page, int itemsPerPage = 15)
        {
            string filterCriteria = User.Identity.Name.WithoutDomain().ToUpper();
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trips = repo.GetPagedList(t => t.EnteredBy.ToUpper().Contains(filterCriteria), (page ?? 0) * itemsPerPage, itemsPerPage);
            ViewBag.HasPrevious = trips.HasPrevious;
            ViewBag.HasNext = trips.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            ViewBag.TotalRows = Math.Max(repo.All().Count() - 1, 0);
            ViewBag.FilterCriteria = filterCriteria; // For debug use
            return View(trips.Entities);
        }

        /// <summary>
        /// Publishes the last 10 trips as a simple RSS feed with the
        /// Spc trip number, DCT who opened it, and direct link to
        /// the trip in TUBS.
        /// </summary>
        /// <returns></returns>
        public ActionResult Rss()
        {
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var lastTen = repo.All().OrderByDescending(t => t.EnteredDate).Take(10).ToList();
            string formatString = Url.Content("~/Trip/{0}");
            var feedItems =
                from trip in lastTen
                select new SyndicationItem(
                    trip.ToString(), 
                    String.Format("Entered by {0}", trip.EnteredBy), 
                    new Uri(String.Format(formatString, trip.Id), UriKind.Relative))
                {
                    PublishDate = trip.EnteredDate.Value
                };

            SyndicationFeed feed =
                new SyndicationFeed(
                    "Latest TUBS Trips", 
                    "http://nouofpweb01.corp.spc.int/tubs/Trip/Rss", 
                    Request.Url, 
                    feedItems);

            return new RssResult(feed);
        }

        public ActionResult Positions(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var tripDoc = KmlBuilder.Build(tripId.Pushpins);
            tripDoc.name = "All Trip Positions";
            tripDoc.description =
                String.Format(
                    "Positions for trip {0} generated on {1} via URL: [{2}]",
                    tripId.ToString(),
                    DateTime.Now.ToShortDateString(),
                    this.HttpContext.Request.RawUrl);
            return new KmlResult(tripDoc);
        }

        // GET: /Trip/Details/1
        /// <summary>
        /// Retrieves Trip details for display.
        /// NOTE:  Trip is retrieved by TripModelBinder -- the caller actually passes in the
        /// integer TripId.  If no such trip exists, the ModelBinder will return null.
        /// </summary>
        /// <param name="id">Trip</param>
        /// <returns></returns>
        public ActionResult Details(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            AddTripNavbar(tripId);
            ViewBag.Title = tripId.ToString();
            return View(tripId);
        }

        // NOTE:  SPC\AL... doesn't seem to want to work on my workstation that's joined to the NOUMEA domain...
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Create()
        {           
            return View(new TripHeaderViewModel());
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Close(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            if (tripId.IsReadOnly)
            {
                return RedirectToAction("Details", "Trip", new { tripId = tripId.Id });
            }

            ViewBag.Title = tripId.ToString();
            ViewBag.TripNumber = tripId.SpcTripNumber;
            TripClosureViewModel tcvm = new TripClosureViewModel()
            {
                TripId = tripId.Id
            };

            return View(tcvm);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Close(TripClosureViewModel tcvm)
        {
            if (!tcvm.TripId.HasValue)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trip = repo.FindBy(tcvm.TripId.Value);
            if (null == trip)
            {
                return new NoSuchTripResult();
            }

            // Idempotent -- if the trip is already closed, just push to the details page
            if (trip.IsReadOnly)
            {
                return RedirectToAction("Details", "Trip", new { tripId = trip.Id });
            }

            // Just in case we have to push back to the edit form
            ViewBag.Title = trip.ToString();
            ViewBag.TripNumber = trip.SpcTripNumber;

            trip.ClosedDate = DateTime.Now;
            trip.Comments = tcvm.Comments;
            try
            {
                repo.Update(trip);
                return RedirectToAction("Details", "Trip", new { tripId = trip.Id });
            }
            catch (Exception ex)
            {
                Flash("Unable to close trip -- contact technical support");
                Logger.Error("Error while closing trip", ex);
            }
            return View(tcvm);
        }

        //
        // This method is a little verbose, but then it's probably the most complex
        // CRUD operation in the application.
        //
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Create(TripHeaderViewModel thvm)
        {
            // Validate ViewModel (potentially, just copy and then validate domain model...)
            Logger.Debug("Entering function...");
            // Model level validations
            if (ModelState.IsValidField("DepartureDate") && ModelState.IsValidField("ReturnDate"))
            {
                Logger.Debug("Has departure/return date fields");
                if (!thvm.ReturnDate.HasValue)
                {
                    ModelState["ReturnDate"].Errors.Add("Return Date is required");
                    Logger.Debug("Failed on ReturnDate validation");
                }

                if (!thvm.DepartureDate.HasValue)
                {
                    ModelState["DepartureDate"].Errors.Add("Departure Date is required");
                    Logger.Debug("Failed on DepartureDate validation");
                }
                
                if (thvm.ReturnDate.Value.CompareTo(thvm.DepartureDate.Value) < 0)
                {
                    // This will prevent ModelState.IsValid from returning true
                    ModelState["ReturnDate"].Errors.Add("Return Date can't be before departure date");
                    Logger.Debug("Failed on return date after departure date");
                }
            }

            // Check to see if the observer code/trip number combo already exists
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            int existingId = (
                from t in repo.FilterBy(t => t.Observer.StaffCode == thvm.ObserverCode && t.TripNumber == thvm.TripNumber)
                select t.Id).FirstOrDefault<int>();

            Logger.Debug("Checking for observer/tripNumber violation...");
            if (existingId != default(int))
            {
                string message =
                    String.Format(
                        "Trip with obstripId {0} already has this observer/trip number combination",
                        existingId);
                ModelState["ObserverCode"].Errors.Add(message);
                ModelState["TripNumber"].Errors.Add(message);
                Logger.DebugFormat("Found observer/tripNumber violation.  Existing obstripId={0}");
                // Redirect to some error page that includes link to existing trip
                Flash(message);
                return View(thvm);
            }

            Trip trip = thvm.ToTrip();
            if (null == trip)
            {
                var message = String.Format("[{0}] is an unsupported gear code", thvm.GearCode);
                ModelState["GearCode"].Errors.Add(message);
            }

            // Add Vessel, Observer and Ports
            trip.FillDependentObjects(thvm, MvcApplication.CurrentSession);

            // This method fills in ModelState errors
            ValidateTripDependencies(trip, thvm);
            
            Logger.DebugFormat("ModelState.IsValid? {0}", ModelState.IsValid);
            if (!ModelState.IsValid)
            {
                Flash("Fix the errors below and try again");
                return View(thvm);
            }

            // Set audit trail data
            trip.EnteredDate = DateTime.Now;
            trip.EnteredBy = User.Identity.Name ?? "Unknown User";

            try
            {
                repo.Add(trip);
                // Push to detail page for this trip
                return RedirectToAction("Details", "Trip", new { tripId = trip.Id });
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save trip header", ex);
                Flash("Failed to register trip.  Please contact technical support.");
            }
 
            return View(thvm);
        }

    }
}
