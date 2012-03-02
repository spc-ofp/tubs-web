﻿// -----------------------------------------------------------------------
// <copyright file="GearController.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class GearController : SuperController
    {
        //
        // GET: /Gear/
        public ActionResult Index(int tripId)
        {
            ViewBag.TripId = tripId;
            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == tripId).FirstOrDefault();
            ViewBag.Title =
                null == gear ?
                    "No gear recorded for this trip" :
                    String.Format("Gear for {0}", gear.Trip.ToString());

            return View(gear);
        }

        private static Type GetSpecificGearType(Trip trip)
        {
            Type gearType = typeof(Gear);
            var tripType = trip.GetType();
            if (tripType == typeof(PurseSeineTrip))
            {
               gearType = typeof(PurseSeineGear);
            }
            // TODO As new trip types come on line, add their gear types here...
            return gearType;
        }

        private static Gear NewGearInstance(Trip trip)
        {
            Gear retval = null;
            if (null != trip)
            {
                var tripType = trip.GetType();
                if (typeof(PurseSeineTrip) == tripType)
                {
                    retval = new PurseSeineGear();
                }
                // TODO As new trip types come on line, add their gear types here...
            }
            return retval;
        }

        // FIXME Need to come up with a better way of doing this -- maybe a "Mutator" attribute
        // and an authorize filter?
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(int tripId)
        {
            ViewBag.TripId = tripId;

            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == tripId).FirstOrDefault();
            if (null == gear)
            {
                var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(tripId);
                if (null == trip)
                {
                    Flash("Can't add gear for a trip that doesn't exist");
                    // TODO Need a better view for this...
                    return View();
                }
                // This is a consequence of sharing an edit page for Create and Edit
                gear = NewGearInstance(trip);
            }            
            ViewBag.GearType = gear.GetType();
            return View(gear);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(int tripId, [AbstractBind(ConcreteTypeParameter = "gearType")] Gear gear)
        {
            var tripRepo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trip = tripRepo.FindBy(tripId);
            if (null == trip)
            {
                Flash("Can't add gear for a trip that doesn't exist");
                // TODO Need a better view for this...
                return View(gear);
            }

            if (ModelState.IsValid)
            {
                var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
                gear.Trip = trip;
                if (default(int) == gear.Id)
                {
                    repo.Add(gear);
                }
                else
                {
                    repo.Update(gear, true);
                }
                return RedirectToAction("Index", new { tripId = tripId });
            }
            ViewBag.GearType = GetSpecificGearType(trip);
            return View(gear);
        }

    }
}
