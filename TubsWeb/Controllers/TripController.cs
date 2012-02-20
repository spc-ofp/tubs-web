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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;
       
    public class TripController : SuperController
    {

        // Default page size
        const int PAGE_SIZE = 20;

        //
        // GET: /Trip/
        public ActionResult Index(int? page)
        {
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trips = repo.GetPagedList((page ?? 0) * PAGE_SIZE, PAGE_SIZE);
            ViewBag.HasPrevious = trips.HasPrevious;
            ViewBag.HasNext = trips.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            return View(trips.Entities);
        }

        //
        // GET: /Trip/Details/1
        public ActionResult Details(int id)
        {
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            Trip trip = repo.FindBy(id);
            if (null == trip)
            {
                return View("NotFound");
            }
            ViewBag.Title = String.Format("Details for trip {0} / {1}", trip.Observer.StaffCode, trip.TripNumber);
            return View("Details", trip);
        }

        // NOTE:  SPC\AL... doesn't seem to want to work on my workstation that's joined to the NOUMEA domain...
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // NOTE:  SPC\AL... doesn't seem to want to work on my workstation that's joined to the NOUMEA domain...
        [HttpPost]
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
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
