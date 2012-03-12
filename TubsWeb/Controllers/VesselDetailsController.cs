// -----------------------------------------------------------------------
// <copyright file="AuxiliariesController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using Spc.Ofp.Tubs.DAL;
    
    public class VesselDetailsController : SuperController
    {
        private ActionResult Modify(Trip trip, VesselNotes notes, string viewName)
        {
            if (null == trip)
            {
                Flash("Can't add/edit vessel details for a non-existant trip");
                return new NoSuchTripResult();
            }

            Logger.DebugFormat("ModelState.IsValid? {0}", ModelState.IsValid);

            if (ModelState.IsValid)
            {
                trip.VesselNotes = notes;
                // FIXME I don't think we want to update through Trip
                var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
                repo.Update(trip);
                Logger.Debug("Updated trip");
                return RedirectToAction("Index", "VesselDetails", new { tripId = trip.Id });
            }
            return View(viewName, notes);
        }
        
        //
        // GET: /VesselDetails/
        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("{0} vessel details", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(tripId.VesselNotes);
        }

        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Create(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            if (null != tripId.VesselNotes)
            {
                // Should be edit
                return RedirectToAction("Edit", new { tripId = tripId });
            }

            ViewBag.Title = String.Format("Create vessel details for {0}", tripId.ToString());
            return View(new VesselNotes());
        }

        [HttpPost]
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Create(Trip tripId, VesselNotes notes)
        {
            return Modify(tripId, notes, "Create");
        }


        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            if (null == tripId.VesselNotes)
            {
                // Should be create
                return RedirectToAction("Create", new { tripId = tripId });
            }

            ViewBag.Title = String.Format("Edit vessel details for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(tripId.VesselNotes);
        }

        [HttpPost]
        [Authorize(Roles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin")]
        public ActionResult Edit(Trip tripId, VesselNotes notes)
        {
            return Modify(tripId, notes, "Edit");
        }

    }
}
