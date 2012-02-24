// -----------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    public class GearController : SuperController
    {
        //
        // GET: /Gear/
        public ActionResult Index(int id)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == id).FirstOrDefault();
            ViewBag.Title =
                null == gear ?
                    "No gear recorded for this trip" :
                    String.Format("Gear for trip {0} / {1}", gear.Trip.Observer.StaffCode, gear.Trip.TripNumber);

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
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.TripId = id;

            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == id).FirstOrDefault();
            if (null == gear)
            {
                var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(id);
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
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Edit(int id, [AbstractBind(ConcreteTypeParameter = "gearType")] Gear gear)
        {
            var tripRepo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trip = tripRepo.FindBy(id);
            if (null == trip)
            {
                Flash("Can't add gear for a trip that doesn't exist");
                // TODO Need a better view for this...
                return View(gear);
            }

            if (ModelState.IsValid)
            {
                //Don't save via trip -- instantiate a Gear repository, set the Trip property, and then save
                //Gear.
                var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
                if (null == gear.Trip)
                {
                    gear.Trip = trip;
                    repo.Add(gear);
                }
                else
                {
                    repo.Update(gear);
                }
                return RedirectToAction("Index", new { id = id });
            }
            ViewBag.GearType = GetSpecificGearType(trip);
            return View(gear);
        }

    }
}
