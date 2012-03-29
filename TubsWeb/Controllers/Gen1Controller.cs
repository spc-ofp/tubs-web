// -----------------------------------------------------------------------
// <copyright file="Gen1Controller.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>

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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;

    public class Gen1Controller : SuperController
    {

        //
        // GET: /Gen1/
        public ActionResult Index(Trip tripId)
        {
            return Load(tripId, "GEN-1 events for trip {0}");
        }

        private ActionResult Load(Trip tripId, string titleFormat)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format(titleFormat, tripId.ToString());
            AddMinMaxDates(tripId);
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            return View(actionName, tripId);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult EditSightings(Trip tripId)
        {
            return Load(tripId, "Edit GEN-1 sightings for trip {0}");
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult EditTransfers(Trip tripId)
        {
            return Load(tripId, "Edit GEN-1 transfers for trip {0}");
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult EditSighting(Trip tripId, Sighting sighting)
        {            
            if (ModelState.IsValid)
            {
                var repo = new TubsRepository<Sighting>(MvcApplication.CurrentSession);
                sighting.Trip = tripId;
                repo.Update(sighting, true);
                Logger.Debug("Sighting updated!");
            }           
            return PartialView("_EditSighting", sighting);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult AddSighting(Trip tripId, [Bind(Prefix = "sighting")] Sighting sighting)
        {
            var repo = new TubsRepository<Sighting>(MvcApplication.CurrentSession);
            if (ModelState.IsValid)
            {
                sighting.Trip = tripId;
                sighting.EnteredBy = User.Identity.Name;
                sighting.EnteredDate = DateTime.Now;
                repo.Add(sighting);
            }
            var sightings = repo.FilterBy(t => t.Trip.Id == tripId.Id);
            return PartialView("_EditSightings", sightings);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult EditTransfer(Trip tripId, Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                var repo = new TubsRepository<Transfer>(MvcApplication.CurrentSession);
                transfer.Trip = tripId;
                repo.Update(transfer, true);
            }
            return PartialView("_EditTransfer", transfer);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult AddTransfer(Trip tripId, [Bind(Prefix = "transfer")] Transfer transfer)
        {
            var repo = new TubsRepository<Transfer>(MvcApplication.CurrentSession);
            if (ModelState.IsValid)
            {
                transfer.Trip = tripId;
                transfer.EnteredBy = User.Identity.Name;
                transfer.EnteredDate = DateTime.Now;
                repo.Add(transfer);
            }
            var transfers = repo.FilterBy(t => t.Trip.Id == tripId.Id);
            return PartialView("_EditTransfers", transfers);
        }

    }
}
