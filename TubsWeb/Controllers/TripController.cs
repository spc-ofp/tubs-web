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
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;
       
    public class TripController : SuperController
    {

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

        public ActionResult Rss()
        {
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var lastTen = repo.All().OrderByDescending(t => t.EnteredDate).Take(10).ToList();
            var feedItems =
                from trip in lastTen
                select new SyndicationItem(
                    trip.ToString(), 
                    String.Format("Entered by {0}", trip.EnteredBy), 
                    new Uri(String.Format("/Trip/{0}", trip.Id), UriKind.Relative))
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

            var positions = tripId.Pushpins;

            return new KmlResult();
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
            return View();
        }

        // NOTE:  SPC\AL... doesn't seem to want to work on my workstation that's joined to the NOUMEA domain...
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Create(TripHeaderViewModel thvm)
        {
            // Model level validations
            if (ModelState.IsValidField("DepartureDate") && ModelState.IsValidField("ReturnDate"))
            {
                if (thvm.ReturnDate.CompareTo(thvm.DepartureDate) < 0)
                {
                    // This will prevent ModelState.IsValid from returning true
                    ModelState["ReturnDate"].Errors.Add("Return date can't be before departure date");
                }
            }

            // Check to see if the observer code/trip number combo already exists
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var constraintQuery =
                from t in repo.FilterBy(t => t.Observer.StaffCode == thvm.ObserverCode && t.TripNumber == thvm.TripNumber)
                select t.Id;

            if (constraintQuery.Count() > 0)
            {
                // Redirect to some error page that includes link to existing trip
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Trip trip = null;
                    switch (thvm.GearCode) {
                        case "S":
                            trip = new PurseSeineTrip();
                            break;
                        case "L":
                            trip = new LongLineTrip();
                            break;
                        case "P":
                            // No support for Pole and Line yet
                            break;
                        default:
                            // Don't know what this is...
                            break;                              
                    }
                    if (null == trip)
                    {
                        Flash("Unsupported Gear Code");
                        return View(thvm);
                    }
                    trip.EnteredDate = DateTime.Now;
                    trip.EnteredBy = User.Identity.Name ?? "Xyzzy";
                    trip.DepartureDate = thvm.DepartureDate;
                    trip.DepartureDateOnly = thvm.DepartureDate.Subtract(thvm.DepartureDate.TimeOfDay);
                    trip.ReturnDate = thvm.ReturnDate;
                    trip.ReturnDateOnly = thvm.ReturnDate.Subtract(thvm.ReturnDate.TimeOfDay);
                    trip.TripNumber = thvm.TripNumber;
                    if (Enum.IsDefined(typeof(ObserverProgram), thvm.ProgramCode))
                    {
                        trip.ProgramCode = (ObserverProgram)Enum.Parse(typeof(ObserverProgram), thvm.ProgramCode);
                    }
                        
                    if (Enum.IsDefined(typeof(WorkbookVersion), thvm.Version))
                    {
                        trip.Version = (WorkbookVersion)Enum.Parse(typeof(WorkbookVersion), thvm.Version);
                    }
                    trip.FillDependentObjects(thvm, MvcApplication.CurrentSession);
                    if (null == trip.Observer || null == trip.DeparturePort || null == trip.ReturnPort || null == trip.Vessel)
                    {
                        Flash("One or more entities (Observer, Port, and/or Vessel) not found in TUBS database");
                        return View(thvm);
                    }
                    repo.Add(trip);
                    // Return to list o' trips
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    Flash("Failed to register trip.  Please contact technical support.");
                    return View(thvm);
                }               
            }
            
            return View(thvm);
        }

    }
}
