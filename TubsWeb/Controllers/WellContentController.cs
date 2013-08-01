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
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;
    
    /// <summary>
    /// 
    /// </summary>
    public class WellContentController : SuperController
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var vm = Mapper.Map<PurseSeineTrip, WellContentViewModel>(trip);

            if (IsApiRequest)
                return GettableJsonNetData(vm);

            return View(CurrentAction, vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="wcvm"></param>
        /// <returns></returns>
        [HttpPost]       
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, WellContentViewModel wcvm)
        {            
            var trip = tripId as PurseSeineTrip;
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(wcvm);
            }

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<PurseSeineWellContent> drepo = TubsDataService.GetRepository<PurseSeineWellContent>(MvcApplication.CurrentSession);
                wcvm.DeletedIds.ToList().ForEach(x => drepo.DeleteById(x));
                foreach (var item in wcvm.WellContentItems)
                {
                    if (item == null || item._destroy)
                        continue;
                    var entity = Mapper.Map<WellContentViewModel.WellContentItem, PurseSeineWellContent>(item);
                    entity.Trip = trip;
                    entity.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    drepo.Save(entity);
                }
                xa.Commit();
            }

            if (IsApiRequest)
            {
                // For some reason (could be a bug, could be something I'm forgetting to do)
                // the ISession that was used for the updates doesn't reflect said updates
                // after the commit.
                // This didn't appear in CrewController because the reload used the
                // stateless session (no dependent objects on crew).
                // To be clear, if I use MvcApplication.CurrentSession here, the parent object
                // (PurseSeineSeaDay) is loaded, but the child objects (Activities) are not.
                // This isn't a great workaround, but it's a workaround nonetheless.
                using (var repo = TubsDataService.GetRepository<Trip>(false))
                {
                    var savedTrip = repo.FindById(trip.Id) as PurseSeineTrip;
                    wcvm = Mapper.Map<PurseSeineTrip, WellContentViewModel>(savedTrip);
                }

                return GettableJsonNetData(wcvm);
            }

            return RedirectToAction("Index", new { tripId = tripId.Id });
        }

    }
}
