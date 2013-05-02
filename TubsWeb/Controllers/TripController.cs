﻿// -----------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ServiceModel.Syndication;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using PagedList;
    using PagedList.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common; // For date utilities
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private IQueryable<Trip> SearchImpl(SearchCriteria criteria)
        {
            return TubsDataService.Search(MvcApplication.CurrentStatelessSession, criteria);
        }


        //
        // GET: /Trip/
        public ActionResult Index(int? page, int itemsPerPage = 15)
        {
            // TODO Convert this to Troy Goode's PagedList implementation
            // https://github.com/TroyGoode/PagedList
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentStatelessSession);
            var pageNumber = page ?? 1;
            var entities = repo.All().ToPagedList(pageNumber, itemsPerPage);
            //int tripCount = Math.Max(repo.All().Count() - 1, 0);
            ViewBag.TotalRows = entities.TotalItemCount;
            return View(entities);
        }

        public ActionResult MyTrips(int? page, int itemsPerPage = 15)
        {
            string filterCriteria = User.Identity.Name.WithoutDomain().ToUpper();
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentStatelessSession);
            var pageNumber = page ?? 1;
            var entities = 
                repo.FilterBy(t => t.EnteredBy.ToUpper()
                    .Contains(filterCriteria))
                    .OrderByDescending(t => t.EnteredDate)
                    .ToPagedList(pageNumber, itemsPerPage);
            ViewBag.TotalRows = entities.TotalItemCount;
            ViewBag.ActionName = "MyTrips";
            ViewBag.FilterCriteria = filterCriteria; // For debug use
            return View("Index", entities);
        }

        public ActionResult MyOpenTrips(int? page, int itemsPerPage = 15)
        {
            string filterCriteria = User.Identity.Name.WithoutDomain().ToUpper();
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentStatelessSession);
            var pageNumber = page ?? 1;
            var entities =
                repo.FilterBy(t => t.EnteredBy.ToUpper().Contains(filterCriteria) && !t.ClosedDate.HasValue)
                    .OrderByDescending(t => t.EnteredDate)
                    .ToPagedList(pageNumber, itemsPerPage);

            ViewBag.Title = "My Open Trips";
            ViewBag.TotalRows = entities.TotalItemCount;
            ViewBag.ActionName = "MyOpenTrips";
            ViewBag.FilterCriteria = filterCriteria; // For debug use
            return View("Index", entities);
        }

        // TODO Add start/end year criteria
        [HttpPost]
        public PartialViewResult Search(string staffCode, string vessel, string program)
        {
            // If there are no criteria, we don't want to do anything.
            if (String.IsNullOrEmpty(staffCode) && String.IsNullOrEmpty(vessel) && String.IsNullOrEmpty(program))
                throw new Exception("No criteria, no can do.");

            Expression<Func<TripHeader, bool>> IsMatch = trip =>
                (String.IsNullOrEmpty(staffCode) || trip.StaffCode == staffCode) &&
                (String.IsNullOrEmpty(vessel) || trip.Vessel.Name.ToUpper().Contains(vessel.ToUpper())) &&
                (String.IsNullOrEmpty(program) || trip.ProgramCode.ToString() == program);

            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentStatelessSession);
            var trips = repo.FilterBy(IsMatch);

            ViewBag.TotalRows = trips.Count();
            ViewBag.ActionName = "Search";
            return PartialView("_Trips", trips);
        }

        /// <summary>
        /// For now, this is for TUFMAN/VMS reconciliation. 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReconciliationSearch(SearchCriteria criteria)
        {
            if (null == criteria || !criteria.IsValid())
            {
                return GettableJsonNetData("No criteria, no can do.");
            }

            var slimList =
                from r in SearchImpl(criteria)
                select new SearchResult
                {
                   DetailUrl = Url.Action("Details", "Trip", new { tripId = r.Id }),
                   VesselName = r.Vessel.Name,
                   DeparturePort = r.DeparturePort.Name,
                   StartDate = r.DepartureDate.Value,
                   ReturnPort = r.ReturnPort.Name,
                   ReturnDate = r.ReturnDate.Value
                };
            return GettableJsonNetData(slimList);
        }

        /// <summary>
        /// Publishes the last 10 trips as a simple RSS feed with the
        /// Spc trip number, DCT who opened it, and direct link to
        /// the trip in TUBS.
        /// </summary>
        /// <returns></returns>
        public ActionResult Rss()
        {
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentStatelessSession);
            var lastTen = repo.All().OrderByDescending(t => t.EnteredDate).Take(10).ToList();
            string formatString = Url.Content("~/Trip/{0}");
            var feedItems =
                from trip in lastTen
                select new SyndicationItem(
                    trip.ToString(), 
                    String.Format("Entered by {0}", trip.EnteredBy), 
                    new Uri(String.Format(formatString, trip.Id), UriKind.Relative))
                {
                    PublishDate = trip.EnteredDate.Value,                    
                };

            SyndicationFeed feed =
                new SyndicationFeed(
                    "Latest TUBS Trips",
                    Url.Content("~/Trip/Rss"),                     
                    Request.Url, 
                    feedItems);
            feed.Language = "en-US";
            feed.Generator = "Tuna Observer System (TUBS)";
            // TODO Set image
            // The framework sets a single author into the managingEditor property.  More than one and they're all a10:author
            feed.Authors.Add(new SyndicationPerson("coreyc@spc.int", "Corey Cole", "http://www.spc.int/oceanfish/en/ofpsection/data-management"));

            return new RssResult(feed);
        }

        public ActionResult Positions(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            // Exclude any pushpins that won't display nicely
            var pushpins = tripId.Pushpins.Where(p => p.CanDisplay()).ToList();           
            // Sort by date (assumes all timestamps have the same base frame of reference for date)
            // which occasionally is not true.
            pushpins.Sort(
                delegate(Pushpin p1, Pushpin p2)
                {
                    return Comparer<DateTime?>.Default.Compare(p1.Timestamp, p2.Timestamp);
                });

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

        public ActionResult PositionAudit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            IList<PositionAuditViewModel> auditResults = new List<PositionAuditViewModel>(tripId.Pushpins.Count);
            var pushpins = tripId.Pushpins.ToList();
            pushpins.Sort(
                delegate(Pushpin p1, Pushpin p2)
                {
                    return Comparer<DateTime?>.Default.Compare(p1.Timestamp, p2.Timestamp);
                });

            PositionAuditViewModel previous = PositionAuditViewModel.FromPushpin(pushpins.FirstOrDefault());
            foreach (var pin in pushpins.Skip(1))
            {
                var current = PositionAuditViewModel.FromPushpin(pin);
                auditResults.Add(PositionAuditViewModel.Diff(previous, current));
                previous = current;
            }

            ViewBag.Title = String.Format("Position Audit for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(auditResults);
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
            ViewBag.BaseDownloadName = string.Format("{0}_{1}_", tripId.Observer.StaffCode, tripId.TripNumber);
            return View(tripId);
        }

        [EditorAuthorize]
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
        [EditorAuthorize]
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

        [EditorAuthorize]
        public ActionResult Create()
        {
            // Drive program code and country code if this is an in-country installation
            string defaultProgramCode = WebConfigurationManager.AppSettings["DefaultProgramCode"];
            string defaultCountryCode = WebConfigurationManager.AppSettings["DefaultCountryCode"];
            var thvm = new TripHeaderViewModel()
            {
                ProgramCode = defaultProgramCode,
                CountryCode = defaultCountryCode
            };
            return View(thvm);
        }

        //
        // This method is a little verbose, but then it's probably the most complex
        // CRUD operation in the application.
        //
        [HttpPost]
        [EditorAuthorize]
        public ActionResult Create(TripHeaderViewModel thvm)
        {
            // After much hassle with native DateTime objects, I've bowed to reality and
            // decided to transport the dates via string and do parsing here.
            
            // TODO:  Validate time only values via RegEx

            var departDate = thvm.DepartureDateOnly.Parse().Merge(thvm.DepartureTimeOnly);
            Logger.WarnFormat("departDate: {0}", departDate);
            if (!departDate.HasValue)
                ModelState["DepartureDateOnly"].Errors.Add("Missing or invalid departure date");

            var returnDate = thvm.ReturnDateOnly.Parse().Merge(thvm.ReturnTimeOnly);
            Logger.WarnFormat("returnDate: {0}", returnDate);
            if (!returnDate.HasValue)
                ModelState["ReturnDateOnly"].Errors.Add("Missing or invalid return date");

            if (returnDate.HasValue && departDate.HasValue && returnDate.Value.CompareTo(departDate.Value) < 0)
            {
                ModelState["ReturnDate"].Errors.Add("Return date can't be before departure date");
            }

            thvm.DepartureDate = departDate;
            thvm.ReturnDate = returnDate;

            // Check to see if the observer code/trip number combo already exists
            var repo = TubsDataService.GetRepository<Trip>(MvcApplication.CurrentSession);
            //var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
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
                // TODO Redirect to some error page that includes link to existing trip
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
            trip.SetAuditTrail(User.Identity.Name, DateTime.Now);

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
