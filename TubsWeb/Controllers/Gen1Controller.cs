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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    public class Gen1Controller : SuperController
    {

        internal ActionResult SightingViewActionImpl(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            var svm = Mapper.Map<Trip, SightingViewModel>(tripId);
            if (IsApiRequest())
                return GettableJsonNetData(svm);

            return View(CurrentAction(),svm);
        }

        internal ActionResult TransferViewActionImpl(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            var tvm = Mapper.Map<Trip, TransferViewModel>(tripId);
            if (IsApiRequest())
                return GettableJsonNetData(tvm);

            return View(CurrentAction(), tvm);
        }
        
        public ActionResult Sightings(Trip tripId)
        {
            return SightingViewActionImpl(tripId);
        }

        [EditorAuthorize]
        public ActionResult EditSightings(Trip tripId)
        {
            return SightingViewActionImpl(tripId);
        }

        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult EditSightings(Trip tripId, SightingViewModel svm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
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
                // This I can do in LINQ
                svm.Sightings.Where(s => s != null && s._destroy).ToList().ForEach(s => repo.DeleteById(s.Id));
                // TODO:  Backfill audit.  I really need to fix this problem...
                sightings.ForEach(s => repo.Save(s));
                xa.Commit();
            }


            if (IsApiRequest())
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

        public ActionResult Transfers(Trip tripId)
        {
            return TransferViewActionImpl(tripId);
        }

        // HttpGet without HttpPost should lock out the client from POSTing to
        // this endpoint
        [HttpGet]
        [EditorAuthorize]
        public ActionResult EditTransfers(Trip tripId)
        {
            return TransferViewActionImpl(tripId);
        }

        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult EditTransfers(Trip tripId, TransferViewModel tvm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
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
                // This I can do in LINQ
                tvm.Transfers.Where(s => s != null && s._destroy).ToList().ForEach(s => repo.DeleteById(s.Id));
                // TODO:  Backfill audit.  I really need to fix this problem...
                transfers.ForEach(s => repo.Save(s));
                xa.Commit();
            }


            if (IsApiRequest())
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
            return View(CurrentAction(), tripId);
        }

        /*
        [EditorAuthorize]
        public ActionResult EditSightings(Trip tripId)
        {
            return Load(tripId, "Edit GEN-1 sightings for trip {0}");
        }
        */

        /*
        [EditorAuthorize]
        public ActionResult EditTransfers(Trip tripId)
        {
            return Load(tripId, "Edit GEN-1 transfers for trip {0}");
        }
        */

        /*
        [HttpPost]
        [EditorAuthorize]
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
        [EditorAuthorize]
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
        [EditorAuthorize]
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
        [EditorAuthorize]
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
        */

    }
}
