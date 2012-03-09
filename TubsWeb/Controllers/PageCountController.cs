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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class PageCountController : SuperController
    {
        //
        // GET: /PageCount/

        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("Page counts for trip {0}", tripId.ToString());           
            return View(tripId);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult AddPageCount(Trip tripId, PageCount pageCount)
        {
            var repo = new TubsRepository<PageCount>(MvcApplication.CurrentSession);            

            if (ModelState.IsValid)
            {
                pageCount.Trip = tripId;
                pageCount.EnteredBy = User.Identity.Name;
                pageCount.EnteredDate = DateTime.Now;
                repo.Add(pageCount);
            }

            var pageCounts = repo.FilterBy(t => t.Trip.Id == tripId.Id);
            return PartialView("_PageCounts", pageCounts);
        }

    }
}
