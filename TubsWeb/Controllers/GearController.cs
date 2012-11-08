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
    using System.Linq;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class GearController : SuperController
    {
        //
        // GET: /Gear/
        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == tripId.Id).FirstOrDefault();

            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            ViewBag.Title =
                null == gear ?
                    "No gear recorded for this trip" :
                    String.Format("Gear for {0}", tripId.ToString());

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

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == tripId.Id).FirstOrDefault();
            if (null == gear)
            {
                // This is a consequence of sharing an edit page for Create and Edit
                gear = NewGearInstance(tripId);
                gear.Trip = tripId;
            }            
            ViewBag.GearType = gear.GetType();
            return View(gear);
        }

        [HttpPost]
        [EditorAuthorize]
        public ActionResult Edit(int tripId, [AbstractBind(ConcreteTypeParameter = "gearType")] Gear gear)
        {
            var tripRepo = TubsDataService.GetRepository<Trip>(MvcApplication.CurrentSession);
            var trip = tripRepo.FindById(tripId);
            if (null == trip)
            {
                Flash("Can't add gear for a trip that doesn't exist");
                // TODO Need a better view for this...
                return View(gear);
            }

            if (ModelState.IsValid)
            {
                var repo = TubsDataService.GetRepository<Gear>(MvcApplication.CurrentSession);
                gear.Trip = trip;
                repo.Save(gear);
                return RedirectToAction("Index", new { tripId = tripId });
            }
            ViewBag.GearType = GetSpecificGearType(trip);
            return View(gear);
        }

    }
}
