// -----------------------------------------------------------------------
// <copyright file="Gen1Controller.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// MVC controller for working with GEN-1 sightings and transfers.
    /// </summary>
    public class Gen1Controller : SuperController
    {

        internal ActionResult SightingViewActionImpl(Trip tripId)
        {
            var svm = Mapper.Map<Trip, SightingViewModel>(tripId);
            if (IsApiRequest)
                return GettableJsonNetData(svm);

            return View(CurrentAction,svm);
        }

        internal ActionResult TransferViewActionImpl(Trip tripId)
        {
            var tvm = Mapper.Map<Trip, TransferViewModel>(tripId);
            if (IsApiRequest)
                return GettableJsonNetData(tvm);

            return View(CurrentAction, tvm);
        }
        
        /// <summary>
        /// MVC Action to display list of all sightings.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter]
        public ActionResult Sightings(Trip tripId)
        {
            return SightingViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action to display editor for all sightings.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult EditSightings(Trip tripId)
        {
            return SightingViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action to update sighting data for a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="svm">ViewModel containing all sightings for current trip.</param>
        /// <returns></returns>
        [HttpPost]       
        [EditorAuthorize]
        [ValidTripFilter]
        [HandleTransactionManually]
        public ActionResult EditSightings(Trip tripId, SightingViewModel svm)
        {
            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(svm);
            }

            var sightings = new List<Sighting>();
            // I can almost get there with LINQ, but I fear I'd have problems with it
            // in the Brian Kernighan "twice-as-smart-to-debug" sense.
            foreach (var sighting in svm.Sightings)
            {
                if (null == sighting || sighting._destroy)
                    continue;
                var entity = Mapper.Map<SightingViewModel.Sighting, Sighting>(sighting);
                if (null != entity)
                {
                    entity.Trip = tripId;
                    entity.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    sightings.Add(entity);
                }
            }
            

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Sighting> repo = TubsDataService.GetRepository<Sighting>(MvcApplication.CurrentSession);

                // Deletes first
                svm.DeletedIds.ToList().ForEach(id => repo.DeleteById(id));

                sightings.ForEach(s => repo.Save(s));
                xa.Commit();
            }


            if (IsApiRequest)
            {
                using (var trepo = TubsDataService.GetRepository<Trip>(false))
                {
                    var t = trepo.FindById(tripId.Id);
                    svm = Mapper.Map<Trip, SightingViewModel>(t);
                }
                return GettableJsonNetData(svm);

            }

            return RedirectToAction("EditSightings", "Gen1", new { tripId = tripId.Id });
        }

        /// <summary>
        /// MVC Action to display list of all transfers.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter]
        public ActionResult Transfers(Trip tripId)
        {
            return TransferViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action to display editor for all transfers.
        /// </summary>
        /// <remarks>
        /// HttpGet attribute without HttpPost locks out client scripts from
        /// POSTing to this endpoint.
        /// </remarks>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult EditTransfers(Trip tripId)
        {
            return TransferViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action to update transfer data for a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="tvm">ViewModel containing all transfers.</param>
        /// <returns></returns>
        [HttpPost]        
        [EditorAuthorize]
        [ValidTripFilter]
        [HandleTransactionManually]
        public ActionResult EditTransfers(Trip tripId, TransferViewModel tvm)
        {
            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(tvm);
            }

            var transfers = new List<Transfer>();
            // I can almost get there with LINQ, but I fear I'd have problems with it
            // in the Brian Kernighan "twice-as-smart-to-debug" sense.
            foreach (var transfer in tvm.Transfers)
            {
                if (null == transfer || transfer._destroy)
                    continue;
                var entity = Mapper.Map<TransferViewModel.Transfer, Transfer>(transfer);
                if (null != entity)
                {
                    entity.Trip = tripId;
                    entity.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    transfers.Add(entity);
                }
            }

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Transfer> repo = TubsDataService.GetRepository<Transfer>(MvcApplication.CurrentSession);

                // Deletes first
                tvm.DeletedIds.ToList().ForEach(id => repo.DeleteById(id));
                transfers.ForEach(t => repo.Save(t));
                xa.Commit();
            }


            if (IsApiRequest)
            {
                using (var trepo = TubsDataService.GetRepository<Trip>(false))
                {
                    var t = trepo.FindById(tripId.Id);
                    tvm = Mapper.Map<Trip, TransferViewModel>(t);
                }
                return GettableJsonNetData(tvm);

            }

            return RedirectToAction("EditTransfers", "Gen1", new { tripId = tripId.Id });
        }
        
        
        //
        // GET: /Gen1/
        [ValidTripFilter]
        public ActionResult Index(Trip tripId)
        {
            return Load(tripId, "GEN-1 events for trip {0}");
        }

        private ActionResult Load(Trip tripId, string titleFormat)
        {
            ViewBag.Title = String.Format(titleFormat, tripId.ToString());
            return View(CurrentAction, tripId);
        }
    }
}
