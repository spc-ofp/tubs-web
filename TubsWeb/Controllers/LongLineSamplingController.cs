// -----------------------------------------------------------------------
// <copyright file="SamplingController.cs" company="Secretariat of the Pacific Community">
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
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class LongLineSamplingController : SamplingController
    {

        // TODO: This needs to be fixed for the "missing set" problem that comes up
        // when this data is published to users that don't have viewing privileges
        // for the entire trip
        
        internal override ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            // This should never happen, but a little defensive coding goes a long way
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == trip.Id);

            // If I was just a _bit_ smarter, I could probably figure out how to extract the
            // following 10 lines into something that can be used between both PS and LL
            // data.


            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = sets.Count();
            var checkpoint = NeedsRedirect(trip.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy setNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex())
            {
                if (setNumber < 1) { setNumber = 1; }
                if (setNumber > maxSets) { setNumber = maxSets; }
            }

            // Based on NeedsRedirect, we should be okay -- the
            // setNumber should be perfect for the action
            var fset = (
                from s in sets
                where s.SetNumber == setNumber
                select s).FirstOrDefault();

            // This is where it gets tricky.  LL doesn't have a "header" object for all bio samples
            // in a set -- it's just a list of entities hanging off the set.  AutoMapper doesn't work
            // very well in that situation.
            var header = new LongLineCatchHeader();
            header.SetId = fset.Id;
            header.FishingSet = fset;
            header.Samples = fset.CatchList;

            var svm = Mapper.Map<LongLineCatchHeader, LongLineSampleViewModel>(header);
            // Set some properties that AutoMapper can't manage for us
            svm.ActionName = CurrentAction();
            svm.HasNext = setNumber < maxSets;
            svm.HasPrevious = setNumber > 1;
            svm.SetCount = maxSets;

            if (IsApiRequest())
                return GettableJsonNetData(svm);


            return View(CurrentAction(), svm);
        }

        /// <summary>
        /// MVC action for viewing list of LL-4 forms associated with a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        public ActionResult List(Trip tripId)
        {
            // This should never happen, but a little defensive coding goes a long way
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            ViewBag.TripId = trip.Id;
            ViewBag.TripNumber = trip.SpcTripNumber;

            // NHibernate issue NH-3043
            // https://nhibernate.jira.com/browse/NH-3043
            // If this list isn't materialized, then the following query, which calls an aggregation
            // function (Count), results in a "Code supposed to be unreachable" exception
            // This results in an N+1 select, but there shouldn't be that many sets associated
            // with a trip.
            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == tripId.Id).ToList();

            var summaries =
                from s in sets
                select new LongLineCatchSummaryViewModel
                {
                    TripId = tripId.Id,
                    SetNumber = s.SetNumber.Value,
                    SetDate = s.SetDate,
                    SampleCount = null == s.CatchList ? 0 : s.CatchList.Count
                };
            
            
            return View(summaries.ToList());
        }

        /// <summary>
        /// MVC action for adding/editing a single LL-4 form.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Location of the set within the trip</param>
        /// <param name="vm">LL-4 data</param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, int setNumber, LongLineSampleViewModel vm)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            // TODO Validation
            // Possible validations:
            // 1)  Any sample date less than start of haul
            // 2)  Length out of range (really needs to be on client side too)

            if (!ModelState.IsValid)
            {
                LogModelErrors();
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(vm);
            }

            var header = Mapper.Map<LongLineSampleViewModel, LongLineCatchHeader>(vm);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                // We need a LongLineSet repository to get the set that will be the parent for all the catch
                IRepository<LongLineSet> srepo = TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession);
                IRepository<LongLineCatch> crepo = TubsDataService.GetRepository<LongLineCatch>(MvcApplication.CurrentSession);

                vm.DeletedCatch.ToList().ForEach(id => crepo.DeleteById(id));

                var fset = srepo.FindById(vm.SetId);
                

                int index = 1;
                foreach (var sample in header.Samples.OrderBy(s => s.Date))
                {
                    sample.SampleNumber = index;
                    sample.FishingSet = fset;
                    sample.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    crepo.Save(sample);
                    ++index;
                }

                // MeasuringInstrument is recorded on the LongLineSet entity
                // Save here, after samples, to avoid transient object problems
                fset.MeasuringInstrument = vm.MeasuringInstrument.MeasuringInstrumentFromString();
                srepo.Save(fset);

                xa.Commit();

            }

            if (IsApiRequest())
            {
                using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
                {
                    var updatedSet = repo.FindById(vm.SetId);
                    var xheader = new LongLineCatchHeader();
                    xheader.SetId = updatedSet.Id;
                    xheader.FishingSet = updatedSet;
                    xheader.Samples = updatedSet.CatchList;

                    var svm = Mapper.Map<LongLineCatchHeader, LongLineSampleViewModel>(xheader);
                    // Set some properties that AutoMapper can't manage for us
                    svm.ActionName = CurrentAction();
                    svm.HasNext = vm.HasNext;
                    svm.HasPrevious = setNumber > 1;
                    svm.SetCount = vm.SetCount;

                    return GettableJsonNetData(svm);
                }
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.  It's been saved, so an Add is counter-productive
            // (besides, redirecting to Add with the current dayNumber will redirect to Edit anyways...)
            return RedirectToAction("Edit", "LongLineSampling", new { tripId = tripId.Id, setNumber = setNumber });
        }

    }
}
