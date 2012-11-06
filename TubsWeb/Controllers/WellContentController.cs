// -----------------------------------------------------------------------
// <copyright file="WellContentController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class WellContentController : SuperController
    {
        // TODO What about Longline trips?
        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("Well content for trip {0}", tripId.ToString());
            return View(trip);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("Well content for trip {0}", tripId.ToString());
            return View(trip);
        }

        [HttpPost]
        [EditorAuthorize]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public ActionResult AddItem(Trip tripId, [Bind(Prefix = "item")] PurseSeineWellContent content)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == tripId)
            {
                return GettableJsonNetData(new { error = true, message = "No trip found with given tripId" });
            }

            if (!ModelState.IsValid)
            {
                return GettableJsonNetData(new { error = true, message = "TODO:  Fix this." });
            }

            content.Trip = trip;
            var repo = new TubsRepository<PurseSeineWellContent>(MvcApplication.CurrentSession);
            // Save the incoming content
            if (default(int) == content.Id)
            {
                content.EnteredBy = User.Identity.Name;
                content.EnteredDate = DateTime.Now;
                repo.Add(content);
            }
            else
            {
                content.UpdatedBy = User.Identity.Name;
                content.UpdatedDate = DateTime.Now;
                repo.Update(content);
            }

            // Refresh table containing the well content
            var contents = repo.FilterBy(c => c.Trip.Id == trip.Id);
            return PartialView("_WellContent", contents);
        }

        [HttpPost]
        [EditorAuthorize]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public ActionResult EditItem(Trip tripId, PurseSeineWellContent content)
        {
            throw new NotImplementedException("TODO Implement this as part of editable grid");
            //return PartialView("_EditWellContent", content);
        }

        [HttpPost]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, IList<PurseSeineWellContent> wells)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            // Save the contents one by one
            foreach (var content in wells)
            {
                content.Trip = trip;
            }

            return RedirectToAction("Index", new { tripId = tripId.Id });
        }

    }
}
