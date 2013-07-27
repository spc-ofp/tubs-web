// -----------------------------------------------------------------------
// <copyright file="Ps1Controller.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// MVC Controller for working with PS-1 form data.
    /// </summary>
    public class Ps1Controller : SuperController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var ps1vm = Mapper.Map<PurseSeineTrip, Ps1ViewModel>(tripId as PurseSeineTrip);

            if (IsApiRequest())
                return GettableJsonNetData(ps1vm);

            // If we wanted to, we could set up the new NavPills here...
            return View(CurrentAction(), ps1vm);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// GET: /Trip/{tripId}/PS-1
        /// </example>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
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
        /// <param name="ps1vm">ViewModel containing PS-1 form data.</param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, Ps1ViewModel ps1vm)
        {
            var trip = tripId as PurseSeineTrip;
            // TODO:  What kind of validation needs to occur?
            // Should we pull this snippet out into a new function?
            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();

                return View(ps1vm);
            }

            var gear = Mapper.Map<Ps1ViewModel.FishingGear, PurseSeineGear>(ps1vm.Gear);
            var inspection = Mapper.Map<SafetyInspectionViewModel, SafetyInspection>(ps1vm.Inspection);
            var characteristics = Mapper.Map<Ps1ViewModel.VesselCharacteristics, PurseSeineVesselAttributes>(ps1vm.Characteristics);

            if (ps1vm.VersionNumber == 2009)
            {
                trip.HasWasteDisposal =
                    string.IsNullOrEmpty(ps1vm.HasWasteDisposal) ? (bool?)null :
                    "YES".Equals(ps1vm.HasWasteDisposal, StringComparison.InvariantCultureIgnoreCase) ? true :
                    "NO".Equals(ps1vm.HasWasteDisposal, StringComparison.InvariantCultureIgnoreCase) ? false :
                    (bool?)null;

                trip.WasteDisposalDescription = ps1vm.WasteDisposalDescription;               
            }

            if (null == trip.VesselNotes)
            {
                trip.VesselNotes = new VesselNotes();
            }
            trip.VesselNotes.Licenses = ps1vm.PermitNumbers;
            trip.VesselNotes.Comments = ps1vm.Page1Comments;
            //ps1vm.Page2Comments;

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                if (null != gear)
                {
                    gear.Trip = trip;
                    gear.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    var repo = TubsDataService.GetRepository<PurseSeineGear>(MvcApplication.CurrentSession);
                    // TODO AuditHelper is a pretty ugly hack, so see about fixing this problem
                    if (!gear.IsNew())
                    {
                        AuditHelper.BackfillTrail<PurseSeineGear>(gear.Id, gear, repo);
                    }
                    repo.Save(gear);
                }

                if (null != inspection)
                {
                    inspection.Trip = trip;
                    inspection.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    var repo = TubsDataService.GetRepository<SafetyInspection>(MvcApplication.CurrentSession);
                    if (!inspection.IsNew())
                    {
                        AuditHelper.BackfillTrail<SafetyInspection>(inspection.Id, inspection, repo);
                    }
                    repo.Save(inspection);
                }

                if (null != characteristics)
                {
                    characteristics.Trip = trip;
                    characteristics.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    var repo = TubsDataService.GetRepository<PurseSeineVesselAttributes>(MvcApplication.CurrentSession);
                    if (!inspection.IsNew())
                    {
                        AuditHelper.BackfillTrail<PurseSeineVesselAttributes>(characteristics.Id, characteristics, repo);
                    }
                    repo.Save(characteristics);
                }

                var trepo = TubsDataService.GetRepository<Trip>(MvcApplication.CurrentSession);
                trip.SetAuditTrail(User.Identity.Name, DateTime.Now);
                AuditHelper.BackfillTrail<Trip>(trip.Id, trip, trepo);
                trepo.Update(trip);

                xa.Commit();
            }

            if (IsApiRequest())
            {
                using (var rrepo = TubsDataService.GetRepository<Trip>(false))
                {
                    var ntrip = rrepo.FindById(tripId.Id) as PurseSeineTrip;
                    var vm = Mapper.Map<PurseSeineTrip, Ps1ViewModel>(ntrip);
                    return GettableJsonNetData(vm);
                }
            }           

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.
            return RedirectToAction("Edit", "Ps1", new { tripId = tripId.Id });
        }

    }
}
