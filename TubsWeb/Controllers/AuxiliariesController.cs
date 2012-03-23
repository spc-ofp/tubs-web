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
    
    public class AuxiliariesController : SuperController
    {

        private ActionResult Modify(Trip id, PurseSeineVesselAttributes aux)
        {
            var trip = id as PurseSeineTrip;
            if (null == trip)
            {
                // TODO May want to rethink this
                return new NoSuchTripResult();
            }

            if (ModelState.IsValid)
            {
                var repo = new TubsRepository<PurseSeineVesselAttributes>(MvcApplication.CurrentSession);
                aux.Trip = trip;
                if (default(int) == aux.Id)
                {                   
                    repo.Add(aux);
                }
                else
                {
                    repo.Update(aux, true);
                }
                return RedirectToAction("Index", new { tripId = trip.Id });
            }
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            return View(actionName, aux);
        }
        
        //
        // GET: /Auxiliaries/
        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return new NoSuchTripResult();
            }

            var aux = trip.VesselAttributes;
            if (null == aux)
            {
                aux = new PurseSeineVesselAttributes();
                aux.Trip = tripId;
            }

            ViewBag.Title = String.Format("{0} auxiliaries", trip.ToString());
            return View(aux);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Create(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return new NoSuchTripResult();
            }

            if (null != trip.VesselAttributes)
            {
                // Should be edit
                return RedirectToAction("Edit", new { tripId = tripId.Id });
            }
            var aux = new PurseSeineVesselAttributes() { Trip = tripId };
            ViewBag.Title = String.Format("Create auxiliaries for {0}", trip.ToString());
            return View(aux);
        }

        

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Create(Trip tripId, PurseSeineVesselAttributes aux)
        {
            aux.EnteredDate = DateTime.Now;
            aux.EnteredBy = User.Identity.Name;
            return Modify(tripId, aux);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return new NoSuchTripResult();
            }

            if (null == trip.VesselAttributes)
            {
                // Should be create
                return RedirectToAction("Create", new { tripId = tripId.Id }); 
            }

            ViewBag.Title = String.Format("Edit auxiliaries for {0}", trip.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(trip.VesselAttributes);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(int tripId, PurseSeineVesselAttributes aux)
        {
            var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(tripId) as PurseSeineTrip;
            Logger.DebugFormat("TripId: {0}, Auxiliary Id: {1}", tripId, aux.Id);
            aux.UpdatedBy = User.Identity.Name;
            aux.UpdatedDate = DateTime.Now;
            return Modify(trip, aux);
        }

    }
}
