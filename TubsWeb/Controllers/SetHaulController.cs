// -----------------------------------------------------------------------
// <copyright file="SetHaulController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels; 

    /// <summary>
    /// SetHaulController manages LL-2/3 data.
    /// As it's for LL-2/3, it only works for Long Line trips
    /// </summary>
    public class SetHaulController : SuperController
    {
        internal void Validate(LongLineTrip trip, LongLineSetViewModel svm)
        {
            // The delta between Ship's date and UTC date shouldn't be more than +/- 1 day 
            DateTime? shipStartOfSet = svm.ShipsDate.Merge(svm.ShipsTime);
            DateTime? utcStartOfSet = svm.UtcDate.Merge(svm.UtcTime);
            if (shipStartOfSet.HasValue && utcStartOfSet.HasValue)
            {
                var deltaT = shipStartOfSet.Value.Subtract(utcStartOfSet.Value);
                if (Math.Abs(deltaT.TotalDays) > 1.0d)
                {
                    var msg = string.Format("Difference of {0} days between ship's and local time too large", Math.Abs(deltaT.TotalDays));
                    ModelState["UtcDate"].Errors.Add(msg);
                }
            }

            if (shipStartOfSet.HasValue && shipStartOfSet.Value.CompareTo(trip.DepartureDate) < 0)
            {
                ModelState["ShipsDate"].Errors.Add("Start of set can't be before the trip departure date");
            }


        }        
        
        /// <summary>
        /// NeedsRedirect validates the setNumber param for a trip based on the
        /// action and the number of sets already existing for this trip.
        /// 
        /// The logic is as follows:
        /// - Index:  Don't care -- Index will cap dayNumber @ maxDays
        /// - Add:  If dayNumber is a day that already exists, push to Edit.
        ///         If dayNumber isn't the appropriate value based on maxDays,
        ///         redirect to Add with the appropriate value
        /// - Edit: If dayNumber is past the end of the trip, redirect to edit
        ///         with the final dayNumber
        /// </summary>
        /// <param name="tripId">obstrip_id</param>
        /// <param name="setNumber">Index (one based) of set number for this trip</param>
        /// <param name="maxSets">Number of sets already added to this trip</param>
        /// <returns></returns>
        internal Tuple<bool, RouteValueDictionary> NeedsRedirect(int tripId, int setNumber, int maxSets)
        {
            var needsRedirect = false;
            // Fill values that don't change
            var rvd = new RouteValueDictionary(
                new { controller = "SetHaul", tripId = tripId }
            );

            if (IsAdd())
            {
                if (setNumber <= maxSets)
                {
                    // Redirect to edit
                    rvd["setNumber"] = setNumber;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
                else if (setNumber > (maxSets + 1))
                {
                    // Redirect to add, but with correct dayNumber
                    rvd["setNumber"] = (maxSets + 1);
                    rvd["action"] = "Add";
                    needsRedirect = true;
                }
            }
            else if (IsEdit())
            {
                if (setNumber > maxSets)
                {
                    rvd["setNumber"] = maxSets;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
            }

            return Tuple.Create(needsRedirect, rvd);
        }       
        
        /// <summary>
        /// ViewActionImpl implements the process of getting a set
        /// by trip and set number.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber">Set number within the trip</param>
        /// <returns>SetHaul ViewModel</returns>
        internal ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            // It was getting crazy to duplicate views for what is essentially the
            // same thing
            string viewName = (IsAdd() || IsEdit()) ? "_Editor" : CurrentAction();          

            // Unlike PS data, set number looks pretty sane in the database
            // so we can navigate directly by tripId and setNumber

            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = trip.FishingSets.Count();

            ViewBag.TitleSuffix = IsAdd() ? 
                String.Format("Add Set {0}", setNumber) :
                String.Format("Set {0} of {1}", setNumber, maxSets); 
            ViewBag.Title = String.Format("{0}: {1}", tripId.SpcTripNumber ?? "This Trip", ViewBag.TitleSuffix);

            var checkpoint = NeedsRedirect(tripId.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy dayNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex())
            {
                if (setNumber < 1) { setNumber = 1; }
                if (setNumber > maxSets) { setNumber = maxSets; }
            }

            // If this is an Add, then we won't find the entity.  Better to create a new
            // empty ViewModel, set some minimal properties, and throw it back to the user
            if (IsAdd())
            {
                var addVm = new LongLineSetViewModel();
                addVm.TripId = tripId.Id;
                addVm.TripNumber = tripId.SpcTripNumber;
                addVm.SetNavDetails(setNumber, maxSets);
                addVm.HasNext = false; // Force it
                addVm.SetNumber = setNumber;
                addVm.ActionName = CurrentAction();
                return View(viewName, addVm);
            }

            var repo = TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession);
            // NeedsRedirect should have protected us from a really crazy SetNumber here
            var fset = repo.FilterBy(s => s.Trip.Id == tripId.Id && s.SetNumber == setNumber).FirstOrDefault();
            var vm = Mapper.Map<LongLineSet, LongLineSetViewModel>(fset);
            vm.SetNavDetails(setNumber, maxSets);
            vm.ActionName = CurrentAction();

            if (IsApiRequest())
                return GettableJsonNetData(vm);

            return View(viewName, vm);
        }

        internal ActionResult SaveActionImpl(Trip tripId, int setNumber, LongLineSetViewModel svm)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            Validate(trip, svm);

            if (!ModelState.IsValid)
            {
                LogModelErrors();
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(svm);
            }

            var fset = Mapper.Map<LongLineSetViewModel, LongLineSet>(svm);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<LongLineSet> srepo = TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession);
                IRepository<LongLineSetHaulEvent> erepo = TubsDataService.GetRepository<LongLineSetHaulEvent>(MvcApplication.CurrentSession);
                IRepository<LongLineSetHaulNote> nrepo = TubsDataService.GetRepository<LongLineSetHaulNote>(MvcApplication.CurrentSession);

                fset.Trip = trip;
                bool isNewSet = fset.IsNew();
                if (isNewSet)
                {
                    srepo.Save(fset);
                }
                
                // Deletes first
                svm.DeletedPositions.ToList().ForEach(e => erepo.DeleteById(e.Id));
                svm.DeletedComments.ToList().ForEach(n => nrepo.DeleteById(n.Id));

                foreach (var position in fset.EventList)
                {
                    position.Latitude = position.Latitude.AsSpcLatitude();
                    position.Longitude = position.Longitude.AsSpcLongitude();
                    position.SetAuditTrail(User.Identity.Name, DateTime.Now);

                    erepo.Save(position);
                }               

                foreach (var note in fset.NotesList)
                {
                    note.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    nrepo.Save(note);
                }

                if (!isNewSet)
                {
                    srepo.Save(fset);
                }
                

                xa.Commit();

            }

            if (IsApiRequest())
            {
                // For some reason (could be a bug, could be something I'm forgetting to do)
                // the ISession that was used for the updates doesn't reflect said updates
                // after the commit.
                // This didn't appear in CrewController because the reload used the
                // stateless session (no dependent objects on crew).
                // To be clear, if I use MvcApplication.CurrentSession here, the parent object
                // (PurseSeineSeaDay) is loaded, but the child objects (Activities) are not.
                // This isn't a great workaround, but it's a workaround nonetheless.
                using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
                {
                    // Force a reload after a save to ensure that PKs are set for all
                    // appropriate entities.
                    int maxDays = repo.FilterBy(d => d.Trip.Id == tripId.Id).Count();

                    fset = repo.FindById(fset.Id);

                    svm = Mapper.Map<LongLineSet, LongLineSetViewModel>(fset);
                    svm.SetNavDetails(setNumber, maxDays);
                    svm.ActionName = CurrentAction();
                }

                return GettableJsonNetData(svm);
            }


            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.  It's been saved, so an Add is counter-productive
            // (besides, redirecting to Add with the current dayNumber will redirect to Edit anyways...)
            return RedirectToAction("Edit", "SetHaul", new { tripId = tripId.Id, setNumber = setNumber });
        }

        /// <summary>
        /// List all sets associated with this Longline trip.
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        public ActionResult List(Trip tripId)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var repo = TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession);
            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == tripId.Id)
                    .OrderBy(s => s.SetNumber);

            ViewBag.Title = String.Format("Sets for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(sets);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        public ActionResult Index(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        [EditorAuthorize]
        public ActionResult Add(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult Add(Trip tripId, int setNumber, LongLineSetViewModel svm)
        {
            return SaveActionImpl(tripId, setNumber, svm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, int setNumber, LongLineSetViewModel svm)
        {
            return SaveActionImpl(tripId, setNumber, svm);
        }

    }
}
