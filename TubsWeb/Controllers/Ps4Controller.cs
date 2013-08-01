// -----------------------------------------------------------------------
// <copyright file="Ps4Controller.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class Ps4Controller : AbstractSetController
    {
        /*
         * List of all PS-4 pages for a trip:
         * /Trip/{tripId}/PS-4/
         * 
         * Direct link to a PS-4 page
         * /Trip/{tripId}/PS-4/{setNumber}/{pageNumber}/Index
         * 
         * Edit link to a PS-4 page
         * /Trip/{tripId}/PS-4/{setNumber}/{pageNumber}/Edit
         * 
         * Add a new PS-4 to a set
         * /Trip/{tripId}/PS-4/{setNumber}/Add
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trip">Current trip</param>
        /// <param name="setNumber">Set number within the trip</param>
        /// <param name="pageNumber">Page number within 'group' of PS-4 pages for this set</param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(PurseSeineTrip trip, int setNumber, int pageNumber)
        {
            int maxSets = trip.FishingSets.Count();
            if (setNumber > maxSets)
            {
                // This isn't like set, where someone can use
                // setNumber = 999 to get to the last set.
                // For now, redirect to List, but a better response is probably called for
                return RedirectToAction("List", new { tripId = trip.Id });
            }

            var repo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);

            var fset =
                repo.FilterBy(
                    s => s.Activity.Day.Trip.Id == trip.Id && s.SetNumber == setNumber
                ).FirstOrDefault();

            // Shouldn't happen, but then again...
            if (null == fset)
                return RedirectToAction("List", new { tripId = trip.Id });

            // We can (and should) be lenient on page number
            int maxPages = fset.SamplingHeaders.Count;

            if (pageNumber > maxPages)
                pageNumber = maxPages;

            var header = fset.SamplingHeaders.Skip(pageNumber - 1).Take(1).FirstOrDefault();
            if (null == header)
                return RedirectToAction("List", new { tripId = trip.Id });

            var vm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(header);
            vm.ActionName = CurrentAction;

            if (IsApiRequest)
                return GettableJsonNetData(vm);

            if (IsIndex)
                return View(vm);

            return View("_Editor", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult List(Trip tripId)
        {
            var vm = Mapper.Map<PurseSeineTrip, TripSamplingViewModel>(tripId as PurseSeineTrip);
            return View(vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Set number within the current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Add(Trip tripId, int setNumber)
        {
            var repo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);

            var fset =
                repo.FilterBy(
                    s => s.Activity.Day.Trip.Id == tripId.Id && s.SetNumber == setNumber
                ).FirstOrDefault();

            // Shouldn't happen, but then again...
            if (null == fset)
                return RedirectToAction("List", new { tripId = tripId.Id });

            var vm = new LengthFrequencyViewModel();
            // Add in some properties
            vm.SetNumber = setNumber;
            vm.PageNumber = fset.SamplingHeaders.Count + 1;
            vm.PageCount = vm.PageNumber;
            vm.ActionName = "Add";
            vm.SetId = fset.Id;
            vm.TripId = tripId.Id;
            vm.TripNumber = tripId.SpcTripNumber;
            foreach (var brailNumber in Enumerable.Range(1, 30))
            {
                vm.Brails.Add(new LengthFrequencyViewModel.Brail() { Number = brailNumber });
            }

            return View("_Editor", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Set number within the current trip</param>
        /// <param name="pageNumber">Page number within the current set</param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId, int setNumber, int pageNumber)
        {
            return ViewActionImpl(tripId as PurseSeineTrip, setNumber, pageNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Set number within the current trip</param>
        /// <param name="pageNumber">Page number within the current set</param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId, int setNumber, int pageNumber)
        {
            return ViewActionImpl(tripId as PurseSeineTrip, setNumber, pageNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Set number within the current trip</param>
        /// <param name="pageNumber">Page number within the current set</param>
        /// <param name="lfvm"></param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, int setNumber, int pageNumber, LengthFrequencyViewModel lfvm)
        {
            var trip = tripId as PurseSeineTrip;

            if (!ModelState.IsValid)
            {
                LogModelErrors();
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(lfvm);
            }

            var header = Mapper.Map<LengthFrequencyViewModel, LengthSamplingHeader>(lfvm);
            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                // We need a PurseSeineSet repository to get the set that will be the parent for this header
                var srepo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);
                var brepo = TubsDataService.GetRepository<Brail>(MvcApplication.CurrentSession);
                var repo = TubsDataService.GetRepository<LengthSamplingHeader>(MvcApplication.CurrentSession);

                header.Set = srepo.FindById(lfvm.SetId);
                header.SetAuditTrail(User.Identity.Name, DateTime.Now);

                // Nothing is ever easy
                // If Brails was more complex, it would be worth setting it from AutoMapper
                // However, all the real work happens in an AfterMap.
                if (lfvm.BrailId != 0)
                    header.Brails = brepo.FindById(lfvm.BrailId);

                if (null == header.Brails)
                    header.Brails = new Brail();

                header.Brails.Header = header;
                header.Brails.SetAuditTrail(User.Identity.Name, DateTime.Now);

                foreach (var brail in lfvm.Brails)
                {
                    int index = brail.Number - 1;
                    if (index < 0 || index > 29)
                        continue;
                    header.Brails[index] = Mapper.Map<LengthFrequencyViewModel.Brail, BrailRecord>(brail);
                }

                // If it's not already the case, LengthSamplingHeader needs to 'own' the saving of
                // the child 'Brails' entity
                repo.Save(header);
                xa.Commit();
            }

            // Read a clean copy of the header from the database
            if (IsApiRequest)
            {
                using (var rrepo = TubsDataService.GetRepository<LengthSamplingHeader>(false))
                {
                    var xheader = rrepo.FindById(header.Id);
                    lfvm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(header);
                    lfvm.ActionName = CurrentAction;
                    return GettableJsonNetData(lfvm);
                }
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.
            return RedirectToAction("Edit", "Ps4", new { tripId = tripId.Id, setNumber = setNumber, pageNumber = pageNumber });
        }

    }
}
