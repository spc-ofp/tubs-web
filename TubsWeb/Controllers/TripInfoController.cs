// -----------------------------------------------------------------------
// <copyright file="TripInfoController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    /// TripInfoController is for working with LL-1 form information.
    /// On the purse seine side, this is called Ps1Controller.
    /// "Ll1Controller" is a name too poor to consider.
    /// This may be further renamed to LongLineTripInfoController
    /// (with Ps1Controller renamed to PurseSeineTripInfoController)
    /// but that is work for another day.
    /// </summary>
    public class TripInfoController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var trip = tripId as LongLineTrip;
            var tivm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(trip);

            if (IsApiRequest())
                return GettableJsonNetData(tivm);

            // If we wanted to, we could set up the new NavPills here...
            return View(CurrentAction(), tivm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// GET: /Trip/{tripId}/LL-1
        /// </example>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(LongLineTrip))]
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
        [ValidTripFilter(TripType = typeof(LongLineTrip))]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="tivm"></param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(LongLineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, LongLineTripInfoViewModel tivm)
        {
            var trip = tripId as LongLineTrip;
            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();

                return View(tivm);
            }

            // LL gear is a bit different from most other operations
            // There are two ViewModels that contribute to a single DAL entity
            // Start with the smallest set (refrigeration)...
            var gear = Mapper.Map<LongLineTripInfoViewModel.RefrigerationMethod, LongLineGear>(tivm.Refrigeration);
            // ...then add the rest of the gear information
            Mapper.Map<LongLineTripInfoViewModel.FishingGear, LongLineGear>(tivm.Gear, gear);
            var inspection = Mapper.Map<SafetyInspectionViewModel, SafetyInspection>(tivm.Inspection);

            if (tivm.VersionNumber == 2009)
            {
                trip.HasWasteDisposal =
                    string.IsNullOrEmpty(tivm.HasWasteDisposal) ? (bool?)null :
                    "YES".Equals(tivm.HasWasteDisposal, StringComparison.InvariantCultureIgnoreCase) ? true :
                    "NO".Equals(tivm.HasWasteDisposal, StringComparison.InvariantCultureIgnoreCase) ? false :
                    (bool?)null;

                trip.WasteDisposalDescription = tivm.WasteDisposalDescription;
            }

            if (null == trip.VesselNotes)
                trip.VesselNotes = new VesselNotes();

            Mapper.Map<LongLineTripInfoViewModel.VesselCharacteristics, VesselNotes>(tivm.Characteristics, trip.VesselNotes);
            trip.VesselNotes.Comments = tivm.Comments;
            if (null != tivm.Nationality)
            { 
                trip.VesselNotes.CaptainCountryCode = tivm.Nationality.CaptainCountryCode;
                trip.VesselNotes.MasterCountryCode = tivm.Nationality.MasterCountryCode;
            }

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                if (null != gear)
                {
                    gear.Trip = trip;
                    gear.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    var repo = TubsDataService.GetRepository<LongLineGear>(MvcApplication.CurrentSession);
                    // TODO AuditHelper is a pretty ugly hack, so see about fixing this problem
                    if (!gear.IsNew())
                    {
                        AuditHelper.BackfillTrail<LongLineGear>(gear.Id, gear, repo);
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
                    var ntrip = rrepo.FindById(tripId.Id) as LongLineTrip;
                    var vm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(ntrip);
                    return GettableJsonNetData(vm);
                }
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.
            return RedirectToAction("Edit", "TripInfo", new { tripId = tripId.Id });
        }

    }
}
