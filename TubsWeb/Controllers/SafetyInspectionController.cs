// -----------------------------------------------------------------------
// <copyright file="SafetyInspectionController.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;

    public class SafetyInspectionController : SuperController
    {
        //
        // GET: /SafetyInspection/
        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = tripId.ToString();
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(tripId.Inspection);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = tripId.ToString();
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            var inspection =
                tripId.Inspection ??
                    new SafetyInspection()
                    {
                        Trip = tripId,
                        Epirb1 = new EpirbResult()
                        {
                            BeaconType = "406"
                        },
                        Epirb2 = new EpirbResult(),
                        Raft1 = new RaftResult(),
                        Raft2 = new RaftResult(),
                        Raft3 = new RaftResult(),
                        Raft4 = new RaftResult()
                    };
            return View(inspection);
        }

        [HttpPost]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, SafetyInspection inspection)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            if (ModelState.IsValid)
            {
                // Remove empty beacons
                if (inspection.Epirb1 != null && !inspection.Epirb1.Include)
                    inspection.Epirb1 = null;
                if (inspection.Epirb2 != null && !inspection.Epirb2.Include)
                    inspection.Epirb2 = null;

                // Remove empty rafts
                if (inspection.Raft1 != null && !inspection.Raft1.Include)
                    inspection.Raft1 = null;
                if (inspection.Raft2 != null && !inspection.Raft2.Include)
                    inspection.Raft2 = null;
                if (inspection.Raft3 != null && !inspection.Raft3.Include)
                    inspection.Raft3 = null;
                if (inspection.Raft4 != null && !inspection.Raft4.Include)
                    inspection.Raft4 = null;

                var repo = new TubsRepository<SafetyInspection>(MvcApplication.CurrentSession);
                inspection.Trip = tripId;
                if (default(int) == inspection.Id)
                {
                    inspection.EnteredBy = User.Identity.Name;
                    inspection.EnteredDate = DateTime.Now;
                    repo.Add(inspection);
                }
                else
                {
                    inspection.UpdatedBy = User.Identity.Name;
                    inspection.UpdatedDate = DateTime.Now;
                    repo.Update(inspection);
                }
                return RedirectToAction("Index", new { tripId = tripId.Id });
            }

            return View(inspection);
        }

    }
}
