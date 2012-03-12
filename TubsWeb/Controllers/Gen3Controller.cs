// -----------------------------------------------------------------------
// <copyright file="Gen3Controller.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// 
    /// </summary>
    public class Gen3Controller : SuperController
    {
        private ActionResult Load(Trip tripId, string titleFormat, bool createNew = false)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format(titleFormat, tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            AddMinMaxDates(tripId);
            return
                createNew ?
                    View(actionName, tripId.TripMonitor ?? new TripMonitor()) :
                    View(actionName, tripId.TripMonitor);
        }

        public ActionResult Index(Trip tripId)
        {
            return Load(tripId, "GEN-3 for trip {0}");
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId)
        {
            return Load(tripId, "Edit GEN-3 for trip {0}", true);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public PartialViewResult BlankEditorRow(Trip tripId)
        {
            AddMinMaxDates(tripId);
            return PartialView("_DetailEditorRow", new TripMonitorDetail());
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, TripMonitor header)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            if (null == header)
            {
                return View("Edit", new TripMonitor());
            }

            // There's no real validation in the header, so we'll start by checking the
            // date for each detail
            // At the same time, perform some minor fixups on the entities, like resetting
            // parent/child relationship and adding audit trail data.
            var enteredDate = DateTime.Now;
            if (null != header.Details)
            {
                foreach (var detail in header.Details)
                {
                    // (Re)set the association between parent and child
                    detail.Header = header;
                    // Set EnteredBy/EnteredDate where necessary
                    if (default(int) == detail.Id)
                    {
                        detail.EnteredBy = User.Identity.Name;
                        detail.EnteredDate = enteredDate;
                    }
                    if (detail.DetailDate.HasValue)
                    {
                        if (!tripId.IsDuringTrip(detail.DetailDate.Value))
                        {
                            ModelState.AddModelError("DetailDate", "Date doesn't fall between departure and return dates");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                header.Trip = tripId;
                bool isNewHeader = default(int) == header.Id;
                if (isNewHeader)
                {
                    header.EnteredBy = User.Identity.Name;
                    header.EnteredDate = enteredDate;
                }

                var repo = new TubsRepository<TripMonitor>(MvcApplication.CurrentSession);
                bool success =
                    isNewHeader ?
                        repo.Add(header) :
                        repo.Update(header);
                if (success)
                {
                    return RedirectToAction("Index", new { tripId = tripId.Id });
                }
            }

            tripId.TripMonitor = header;
            return View("Edit", tripId);            
        }

    }
}
