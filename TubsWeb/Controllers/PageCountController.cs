// -----------------------------------------------------------------------
// <copyright file="PageCountController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;
    
    public class PageCountController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            var vm = tripId is PurseSeineTrip ?
                Mapper.Map<PurseSeineTrip, PurseSeinePageCountViewModel>(tripId as PurseSeineTrip) :
                Mapper.Map<Trip, PageCountViewModel>(tripId);

            if (IsApiRequest())
                return GettableJsonNetData(vm);
 
            return View(CurrentAction(), vm);
        }
        
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [HttpPost]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, PageCountViewModel pcvm)
        {
            throw new NotImplementedException("");
        }



        /*
        [HttpPost]
        [EditorAuthorize]
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
        */

    }
}
