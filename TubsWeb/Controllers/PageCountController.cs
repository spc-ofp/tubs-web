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
    using System.Linq;
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

            var vm = Mapper.Map<Trip, PageCountViewModel>(tripId);

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
        [HandleTransactionManually]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public ActionResult Edit(Trip tripId, PageCountViewModel pcvm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (null == pcvm)
            {
                return ViewActionImpl(tripId);
            }

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                var repo = TubsDataService.GetRepository<PageCount>(MvcApplication.CurrentSession);
                // Deletes first
                pcvm.PageCounts.Where(pc => pc != null && pc._destroy).ToList().ForEach(x => repo.DeleteById(x.Id));
                // Save or update
                var saves = pcvm.PageCounts.Where(pc => pc != null && !pc._destroy);
                foreach (var item in saves)
                {
                    Logger.ErrorFormat("Saving PageCount with key {0} and value {1}", item.Key, item.Value);
                    var entity = Mapper.Map<PageCountViewModel.PageCount, PageCount>(item);
                    if (null == entity) { continue; }
                    Logger.ErrorFormat("Entity values for Form {0} and Count {1}", entity.FormName, entity.FormCount);
                    entity.Trip = tripId;
                    entity.EnteredBy = User.Identity.Name;
                    entity.EnteredDate = DateTime.Now;
                    repo.Save(entity);
                    
                }

                xa.Commit();
            }

            if (IsApiRequest())
            {
                using (var trepo = TubsDataService.GetRepository<Trip>(false))
                {
                    var trip = trepo.FindById(tripId.Id);
                    pcvm = Mapper.Map<Trip, PageCountViewModel>(tripId);
                    return GettableJsonNetData(pcvm);
                }
               
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.
            return RedirectToAction("Edit", "PageCount", new { tripId = tripId.Id });
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
