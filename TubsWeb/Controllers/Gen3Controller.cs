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
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// MVC controller for GEN-3 (Trip Monitoring) form.
    /// </summary>
    public class Gen3Controller : SuperController
    {
        internal Gen3ViewModel LoadViewModel(Trip tripId)
        {
            // Build ViewModel from the appropriate Trip sub entity/entities
            return tripId.Version == Spc.Ofp.Tubs.DAL.Common.WorkbookVersion.v2009 ?
                Mapper.Map<Trip, Gen3ViewModel>(tripId) :
                Mapper.Map<TripMonitor, Gen3ViewModel>(tripId.TripMonitor);
        }
        
        
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            string formatString =
                IsEdit ?
                    "{0}: GEN-3 Edit" :
                    "{0}: GEN-3";

            ViewBag.Title = String.Format(formatString, tripId.SpcTripNumber);

            // Build ViewModel from the appropriate Trip sub entity/entities
            var vm = LoadViewModel(tripId);

            // v2007 workbooks without a TripMonitor result in a null view model
            // object.  This should fix the issue with the least amount of fuss.
            if (null == vm)
            {
                vm = new Gen3ViewModel();
                vm.TripId = tripId.Id;
                vm.VersionNumber = 2007;
                vm.TripNumber = tripId.SpcTripNumber;
            }

            // Ensure that, for v2009 workbooks, the ViewModel has 31 possible questions
            // in the same display order as the printed workbook
            vm.PrepareIncidents();

            if (IsApiRequest)
                return GettableJsonNetData(vm);

            // Min/max dates in the ViewBag are only useful outside of API calls
            AddMinMaxDates(tripId);
            return View(CurrentAction, vm);
        }

        /// <summary>
        /// MVC action for displaying GEN-3 data for a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter]
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC action for displaying GEN-3 edit form for a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC action for modifying GEN-3 data for a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="vm">GEN-3 data</param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, Gen3ViewModel vm)
        {
            if (null == vm)
            {
                return ViewActionImpl(tripId);
            }

            // Complex validation here
            if (null != vm.Notes)
            {
                foreach (var note in vm.Notes)
                {
                    if (null != note && note.Date.HasValue && !note._destroy)
                    {
                        if (!tripId.IsDuringTrip(note.Date.Value))
                        {
                            ModelState.AddModelError("Date", "Date doesn't fall between departure and return dates");
                        }
                    }
                }
            }

            // Simple validation here
            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();

                return View(CurrentAction, vm);
            }

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                var deletedNoteIds = vm.Notes.Where(n => null != n && n._destroy).Select(n => n.Id);
                
                if (Spc.Ofp.Tubs.DAL.Common.WorkbookVersion.v2009 == tripId.Version.Value)
                {
                    // MockTripMonitor is a disposable container for the two lists of entities
                    // we _do_ care about
                    var tm = Mapper.Map<Gen3ViewModel, MockTripMonitor>(vm);

                    var arepo = TubsDataService.GetRepository<Gen3Answer>(MvcApplication.CurrentSession);
                    foreach (var answer in tm.Answers)
                    {
                        answer.Trip = tripId;
                        answer.SetAuditTrail(User.Identity.Name, DateTime.Now);
                        if (!answer.IsNew())
                        {
                            AuditHelper.BackfillTrail<Gen3Answer>(answer.Id, answer, arepo);
                        }

                        arepo.Save(answer);

                    }

                    var drepo = TubsDataService.GetRepository<Gen3Detail>(MvcApplication.CurrentSession);
                    foreach (var detail in tm.Details)
                    {
                        if (deletedNoteIds.Contains(detail.Id))
                        {
                            continue;
                        }

                        detail.Trip = tripId;
                        detail.SetAuditTrail(User.Identity.Name, DateTime.Now);
                        if (!detail.IsNew())
                        {
                            AuditHelper.BackfillTrail<Gen3Detail>(detail.Id, detail, drepo);
                        }

                        drepo.Save(detail);
                    }

                    deletedNoteIds.ToList().ForEach(id => drepo.DeleteById(id));

                }
                else
                {
                    var tm = Mapper.Map<Gen3ViewModel, TripMonitor>(vm);
                    tm.Trip = tripId;
                    tm.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    var repo = TubsDataService.GetRepository<TripMonitor>(MvcApplication.CurrentSession);

                    if (!tm.IsNew())
                    {
                        AuditHelper.BackfillTrail<TripMonitor>(tm.Id, tm, repo);
                    }

                    var drepo = TubsDataService.GetRepository<TripMonitorDetail>(MvcApplication.CurrentSession);
                    foreach (var detail in tm.Details)
                    {
                        if (deletedNoteIds.Contains(detail.Id))
                        {
                            continue;
                        }
                        
                        detail.Header = tm;
                        detail.SetAuditTrail(User.Identity.Name, DateTime.Now);
                        if (!detail.IsNew())
                        {
                            AuditHelper.BackfillTrail<TripMonitorDetail>(detail.Id, detail, drepo);
                        }
                    }

                    deletedNoteIds.ToList().ForEach(id => drepo.DeleteById(id));
                    repo.Save(tm);
                }

                xa.Commit();
            }

            if (IsApiRequest)
            {
                using (var trepo = TubsDataService.GetRepository<Trip>(false))
                {
                    var trip = trepo.FindById(tripId.Id);
                    vm = LoadViewModel(trip);
                }

                return GettableJsonNetData(vm);
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.  It's been saved, so an Add is counter-productive
            return RedirectToAction("Edit", "Gen3", new { tripId = tripId.Id });

        }

    }
}
