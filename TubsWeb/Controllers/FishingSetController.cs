// -----------------------------------------------------------------------
// <copyright file="FishingSetController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// FishingSetController provides access to fishing sets.  This is currently Purse Seine only,
    /// as Longline fishing doesn't use the same concept of 'Set' and there is no Pole-and-line support
    /// in TUBS.
    /// 
    /// NOTE:  Due to referential integrity constraints, the users are not allowed to add a set past
    /// the maximum set number.  If a set needs to be added, the associated activity from a PS-2 must
    /// be entered.
    /// </summary>
    public class FishingSetController : AbstractSetController
    {

        internal ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            var trip = tripId as PurseSeineTrip;
            // While PurseSeineTrip can access sets directly,
            // the current implementation is preferred as it
            // doesn't actually hydrate the sets in question
            // until we're sure the provided setNumber is correct.

            var sets = TubsDataService.GetRepository<PurseSeineSet>(
                MvcApplication.CurrentSession).FilterBy(
                    s => s.Activity.Day.Trip.Id == tripId.Id);

            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = sets.Count();
            var checkpoint = NeedsRedirect(tripId.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy setNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex)
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

            var fsvm = Mapper.Map<PurseSeineSet, PurseSeineSetViewModel>(fset);

            // Fill in what it's a pain to get AutoMapper to do
            fsvm.MaxSets = maxSets;
            fsvm.HasNext = setNumber < maxSets;
            fsvm.HasPrevious = setNumber > 1;

            if (IsApiRequest)
                return GettableJsonNetData(fsvm);

            return View(CurrentAction, fsvm);
        }

        /// <summary>
        /// MVC Action for displaying a list of all sets in a trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult List(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("{0}: All Sets", tripId.SpcTripNumber ?? "This Trip");
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";

            // Materialize list so that sets can be sorted
            var sets = trip.FishingSets.ToList();
            sets.Sort(delegate(PurseSeineSet s1, PurseSeineSet s2)
            {
                return Comparer<int?>.Default.Compare(s1.SetNumber, s2.SetNumber);
            });
            return View(sets);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber"></param>
        /// <param name="fsvm"></param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [HandleTransactionManually]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId, int setNumber, PurseSeineSetViewModel fsvm)
        {
            var trip = tripId as PurseSeineTrip;
            // TODO Any validation that the attributes don't cover

            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(fsvm);
            }

            // Convert to entity
            var fset = Mapper.Map<PurseSeineSetViewModel, PurseSeineSet>(fsvm);
            // Set audit trails
            fset.SetAuditTrail(User.Identity.Name, DateTime.Now);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                var fsrepo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);
                var erepo = TubsDataService.GetRepository<Activity>(MvcApplication.CurrentSession);
                var screpo = TubsDataService.GetRepository<PurseSeineSetCatch>(MvcApplication.CurrentSession);

                // A read-only property on the view model relieves us of some hassle
                fsvm.DeletedCatchIds.ToList().ForEach(x => screpo.DeleteById(x));

                // Deletes first
                //fsvm.TargetCatch.DefaultIfEmpty().Where(x => null != x && x._destroy).ToList().ForEach(x => screpo.DeleteById(x.Id));
                //fsvm.ByCatch.DefaultIfEmpty().Where(x => x != null && x._destroy).ToList().ForEach(x => screpo.DeleteById(x.Id));

                var parent = erepo.FindById(fsvm.ActivityId) as PurseSeineActivity;
                fset.Activity = parent;
                fset.CatchList.ToList().ForEach(cl =>
                {
                    cl.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    screpo.Save(cl);
                });

                fsrepo.Save(fset);              

                xa.Commit();
                fsrepo.Reload(fset);
            }

            if (IsApiRequest)
            {
                var maxSets = fsvm.MaxSets;
                fsvm = Mapper.Map<PurseSeineSet, PurseSeineSetViewModel>(fset);
                fsvm.MaxSets = maxSets;
                fsvm.HasNext = setNumber < maxSets;
                fsvm.HasPrevious = setNumber > 1;
                return GettableJsonNetData(fsvm);
            }

            return RedirectToAction("Edit", new { tripId = tripId.Id, setNumber = setNumber });
        }

    }
}
